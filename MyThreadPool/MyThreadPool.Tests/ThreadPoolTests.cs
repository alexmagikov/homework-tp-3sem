// <copyright file="ThreadPoolTests.cs" company="AlexanderKuchin">
// Copyright (c) AlexanderKuchin. All rights reserved.
// </copyright>

namespace MyThreadPool.Tests;

using System.Collections.Concurrent;

public class ThreadPoolTests
{
    [Test]
    public void ThreadPoolTestShouldCreateThreads()
    {
        var threadPool = new MyThreadPool(4);
        var results = new ConcurrentBag<int>();
        var tasks = new List<IMyTask<int>>();

        for (int i = 0; i < 4; i++)
        {
            tasks.Add(threadPool.Submit(() =>
            {
                results.Add(Thread.CurrentThread.ManagedThreadId);
                Thread.Sleep(100);
                return 1;
            }));
        }

        foreach (var task in tasks)
        {
            _ = task.Result;
        }

        Assert.That(results.Distinct().Count(), Is.EqualTo(4));

        threadPool.Shutdown();
    }

    [Test]
    public void ThreadPoolConstructorShouldThrowOnInvalidArgument()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var threadPool = new MyThreadPool(0);
        });
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var threadPool = new MyThreadPool(-1);
        });
    }

    [Test]
    public void ThreadPoolTestBaseScenarios()
    {
        var threadPool = new MyThreadPool(4);
        var task = threadPool.Submit(() =>
        {
            Thread.Sleep(100);
            return 4;
        });
        Assert.That(task.IsCompleted, Is.False);
        Assert.That(task.Result, Is.EqualTo(4));
        threadPool.Shutdown();
    }

    [Test]
    public void ThreadPoolTestShutdownShouldNotStopWork()
    {
        var threadPool = new MyThreadPool(4);
        var task = threadPool.Submit(() =>
        {
            Thread.Sleep(100);
            return 4;
        });
        threadPool.Shutdown();

        Assert.That(task.Result, Is.EqualTo(4));
        Assert.That(task.IsCompleted, Is.True);
    }

    [Test]
    public void ThreadPoolTestShutdownShouldNotAddNewTasks()
    {
        var threadPool = new MyThreadPool(4);
        threadPool.Submit(() =>
        {
            Thread.Sleep(100);
            return 4;
        });
        threadPool.Shutdown();

        Assert.Throws<InvalidOperationException>(() => threadPool.Submit(() => 4));
    }

    [Test]
    public void TaskShouldPropagateExceptions()
    {
        var threadPool = new MyThreadPool(4);
        var task = threadPool.Submit<int>(() => throw new InvalidOperationException("Test exception"));

        Assert.Throws<AggregateException>(() => { var result = task.Result; });
        threadPool.Shutdown();
    }

    [Test]
    public void TaskTestContinueWithBaseScenarios()
    {
        var threadPool = new MyThreadPool(4);
        var task = threadPool.Submit(() => 2 * 2).ContinueWith(x => x.ToString());
        task = task.ContinueWith(x => x + "1");

        Assert.That(task.Result, Is.EqualTo("41"));

        threadPool.Shutdown();
    }
}