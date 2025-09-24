// <copyright file="LazyTests.cs" company="AlexanderKuchin">
// Copyright (c) AlexanderKuchin. All rights reserved.
// </copyright>

namespace Lazy.Tests;

public class GeneralLazyTests
{
    [Test]
    public void LazyMethodsAreEqual()
    {
        var lazySingle = new SingleThreadedLazy<int>(() => 52);
        var lazyMulti = new MultiThreadedLazy<int>(() => 52);

        Assert.That(lazySingle.Get(), Is.EqualTo(lazyMulti.Get()));
    }

    [Test]
    public void LazySupplierBackNull()
    {
        var lazyMulti = new MultiThreadedLazy<object?>(() => null);
        Assert.That(lazyMulti.Get(), Is.Null);
    }

    [Test]
    public void LazyNormalSupplier()
    {
        var lazyMulti = new MultiThreadedLazy<int>(() => 52);
        Assert.That(lazyMulti.Get(), Is.EqualTo(52));
    }

    [Test]
    public void LazyNormalSupplierNormalBehaviour()
    {
        var num = 0;
        var lazyMulti = new MultiThreadedLazy<int>(() =>
        {
            num++;
            return num;
        });

        Assert.That(lazyMulti.Get(), Is.EqualTo(1));
        Assert.That(lazyMulti.Get(), Is.EqualTo(1));
    }

    [Test]
    public void LazyNullSupplier()
        => Assert.Throws<ArgumentNullException>(() => { new MultiThreadedLazy<int>(null); });
}