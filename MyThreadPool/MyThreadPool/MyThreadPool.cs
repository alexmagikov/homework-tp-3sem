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
        lock (this.lockObject)
        {
            if (this.cts.IsCancellationRequested)
            {
                throw new InvalidOperationException("Already shutdown");
            }

            var task = new MyTask<TResult>(inputFunc, this);

            this.taskQueue.Enqueue(() => task.Execute());
            Monitor.Pulse(this.lockObject);

            return task;
        }
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

        private readonly List<(Action Continuation, Action<Exception> SetException)> continuations = new();

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

        public TResult Result
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

                    if (this.result == null)
                    {
                        throw new InvalidOperationException("No result");
                    }

                    return this.result;
                }
            }
        }

        public void Execute()
        {
            TResult? localResult = default;
            Exception? localException = null;

            try
            {
                localResult = inputFunc();
            }
            catch (Exception inputException)
            {
                localException = inputException;
            }

            lock (this.lockObject)
            {
                this.result = localResult;
                this.exception = localException;
                this.isCompleted = true;

                foreach (var (continuation, setException) in this.continuations)
                {
                    try
                    {
                        inputThreadPool.EnqueueTask(continuation);
                    }
                    catch (InvalidOperationException inputException)
                    {
                        setException(inputException);
                    }
                }

                this.continuations.Clear();

                Monitor.PulseAll(this.lockObject);
            }
        }

        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continuation)
        {
            var newTask = new MyTask<TNewResult>(
                () =>
            {
                var computingResult = this.Result;
                return continuation(computingResult);
            },
                inputThreadPool);

            void SetException(Exception inputException)
            {
                lock (newTask.lockObject)
                {
                    newTask.exception = inputException;
                    newTask.isCompleted = true;
                    Monitor.PulseAll(newTask.lockObject);
                }
            }

            lock (this.lockObject)
            {
                if (this.isCompleted)
                {
                    try
                    {
                        inputThreadPool.EnqueueTask(() => newTask.Execute());
                    }
                    catch (Exception inputException)
                    {
                        SetException(inputException);
                    }
                }
                else
                {
                    this.continuations.Add((() => newTask.Execute(), SetException));
                }
            }

            return newTask;
        }
    }
}