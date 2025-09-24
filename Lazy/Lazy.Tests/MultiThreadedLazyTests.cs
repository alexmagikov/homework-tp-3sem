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
            return counter;
        });

        var numThreades = 10;
        var threads = new Thread[numThreades];

        for (int i = 0; i < numThreades; i++)
        {
            threads[i] = new Thread(() => lazyMulti.Get());
            threads[i].Start();
        }

        for (int i = 0; i < numThreades; i++)
        {
            threads[i].Join();
        }

        Assert.That(counter, Is.EqualTo(1));
    }
}