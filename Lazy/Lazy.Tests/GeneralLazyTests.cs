// <copyright file="GeneralLazyTests.cs" company="AlexanderKuchin">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Lazy.Tests;

public class GeneralLazyTests
{
    public static IEnumerable<TestCaseData> LazyFactories()
    {
        yield return new TestCaseData(
            new Func<Func<object?>, ILazy<object?>>(s => new SingleThreadedLazy<object?>(s))).SetName("SingleThreadedLazy<object?>");

        yield return new TestCaseData(
            new Func<Func<object?>, ILazy<object?>>(s => new MultiThreadedLazy<object?>(s))).SetName("MultiThreadedLazy<object?>");
    }

    [TestCaseSource(nameof(LazyFactories))]
    public void LazySupplierBackNull(Func<Func<object?>, ILazy<object?>> factory)
    {
        var lazy = factory(() => null);
        Assert.That(lazy.Get(), Is.Null);
    }

    [TestCaseSource(nameof(LazyFactories))]
    public void LazyNormalSupplier(Func<Func<object?>, ILazy<object?>> factory)
    {
        var lazy = factory(() => 52);
        Assert.That(lazy.Get(), Is.EqualTo(52));
    }

    [TestCaseSource(nameof(LazyFactories))]
    public void LazyNormalSupplierNormalBehaviour(Func<Func<object?>, ILazy<object?>> factory)
    {
        var num = 0;
        var lazy = factory(() =>
        {
            num++;
            return num;
        });

        Assert.That(lazy.Get(), Is.EqualTo(1));
        Assert.That(lazy.Get(), Is.EqualTo(1));
    }

    [TestCaseSource(nameof(LazyFactories))]
    public void LazyNullSupplier(Func<Func<object?>, ILazy<object?>> factory)
        => Assert.Throws<ArgumentNullException>(() => factory(null));
}