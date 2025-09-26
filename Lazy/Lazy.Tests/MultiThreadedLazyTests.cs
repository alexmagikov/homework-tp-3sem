// <copyright file="MultiThreadedLazyTests.cs" company="AlexanderKuchin">
// Copyright (c) AlexanderKuchin. All rights reserved.
// </copyright>

namespace Lazy.Tests;

public class MultiThreadedLazyTests
{
    [Test]
    public void MultiThreadedLazyMethodsDoesntLocks()
    {
        var counter = 0;
        var lazyMulti = new MultiThreadedLazy<int>(() =>
        {
            Interlocked.Increment(ref counter);
            Thread.Sleep(50);
            return counter + 1;
        });

        var numThreads = 10;
        var threads = new Thread[numThreads];
        var results = new int[numThreads];

        var barrier = new Barrier(numThreads);

        for (var i = 0; i < numThreads; i++)
        {
            var localIndex = i;
            threads[i] = new Thread(() =>
            {
                barrier.SignalAndWait();
                results[localIndex] = lazyMulti.Get();
            });
            threads[i].Start();
        }

        for (var i = 0; i < numThreads; i++)
        {
            threads[i].Join();
        }

        for (int i = 0; i < numThreads; i++)
        {
            Assert.That(results[i], Is.EqualTo(2));
        }
    }
}