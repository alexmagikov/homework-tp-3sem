// <copyright file="MyThreadPool.cs" company="AlexanderKuchin">
// Copyright (c) AlexanderKuchin. All rights reserved.
// </copyright>

namespace MyThreadPool;

public class MyThreadPool
{
    private readonly Thread[] threads;

    private readonly Queue<Action> taskQueue = new Queue<Action>();

    private readonly object lockObject = new object();

    private readonly CancellationTokenSource cts = new CancellationTokenSource();

    public MyThreadPool(int numThreads)
    {
        if (numThreads <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(numThreads));
        }

        this.threads = new Thread[numThreads];

        for (int i = 0; i < numThreads; i++)
        {
            this.threads[i] = new Thread(() =>
            {
                while (true)
                {
                    Action? action = null;
                    lock (this.lockObject)
                    {
                        while (this.taskQueue.Count == 0 && !this.cts.IsCancellationRequested)
                        {
                            Monitor.Wait(this.lockObject);
                        }

                        if (this.cts.IsCancellationRequested && this.taskQueue.Count == 0)
                        {
                            return;
                        }

                        if (taskQueue.Count > 0)
                        {
                            action = taskQueue.Dequeue();
                        }
                    }

                    action?.Invoke();
                }
            })
            {
                IsBackground = true,
            };

            this.threads[i].Start();
        }
    }

    /// <summary>
    /// Add Task to ThreadPool.
    /// </summary>
    /// <param name="inputFunc">Input function.</param>
    /// <typeparam name="TResult">Expected type of result.</typeparam>
    /// <exception cref="InvalidOperationException">Exception, if ThreadPool is already stopped.</exception>
    /// <returns>Task.</returns>
    public IMyTask<TResult> Submit<TResult>(Func<TResult> inputFunc)
    {
        if (this.cts.IsCancellationRequested)
        {
            throw new InvalidOperationException("Already shutdown");
        }

        var task = new MyTask<TResult>(inputFunc, this);

        lock (this.lockObject)
        {
            this.taskQueue.Enqueue(() => task.Execute());
            Monitor.Pulse(this.lockObject);
        }

        return task;
    }

    /// <summary>
    /// Finish work of threads.
    /// </summary>
    public void Shutdown()
    {
        lock (this.lockObject)
        {
            this.cts.Cancel();
            Monitor.PulseAll(this.lockObject);
        }

        foreach (var thread in this.threads)
        {
            thread.Join();
        }
    }

    private void EnqueueTask(Action action)
    {
        lock (this.lockObject)
        {
            if (this.cts.IsCancellationRequested)
            {
                throw new InvalidOperationException("Already shutdown");
            }

            this.taskQueue.Enqueue(action);
            Monitor.Pulse(this.lockObject);
        }
    }

    private class MyTask<TResult>(Func<TResult> inputFunc, MyThreadPool inputThreadPool) : IMyTask<TResult>
    {
        private readonly object lockObject = new();

        private readonly List<Action> continuations = new();

        private Exception? exception;

        private bool isCompleted;

        private TResult? result;

        public bool IsCompleted
        {
            get
            {
                lock (this.lockObject)
                {
                    return this.isCompleted;
                }
            }
        }

        public TResult? Result
        {
            get
            {
                lock (this.lockObject)
                {
                    while (!this.isCompleted)
                    {
                        Monitor.Wait(this.lockObject);
                    }

                    if (this.exception != null)
                    {
                        throw new AggregateException(this.exception);
                    }

                    return this.result;
                }
            }
        }

        public void Execute()
        {
            try
            {
                this.result = inputFunc();
                lock (this.lockObject)
                {
                    this.isCompleted = true;

                    foreach (var continuation in this.continuations)
                    {
                        inputThreadPool.EnqueueTask(continuation);
                    }

                    this.continuations.Clear();
                    Monitor.PulseAll(this.lockObject);
                }
            }
            catch (Exception inputException)
            {
                lock (this.lockObject)
                {
                    this.exception = inputException;
                    this.isCompleted = true;
                    Monitor.PulseAll(this.lockObject);
                }
            }
        }

        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continuation)
        {
            var newTask = new MyTask<TNewResult>(() => continuation(this.Result), inputThreadPool);

            lock (this.lockObject)
            {
                if (this.isCompleted)
                {
                    inputThreadPool.EnqueueTask(() => newTask.Execute());
                }
                else
                {
                    this.continuations.Add(() => newTask.Execute());
                }
            }

            return newTask;
        }
    }
}