// <copyright file="RunTests.cs" company="Alexander Kuchin">
// Copyright (c) Alexander Kuchin. All rights reserved.
// </copyright>

using System.Collections.Concurrent;
using System.Diagnostics;

namespace MyNUnit.Test;

public class RunTests
{
    private string path;
    private string pathParallelTests;
    private ConcurrentBag<string> resultForTests;
    private ConcurrentBag<string> resultForParallelTests;

    [SetUp]
    public async Task Setup()
    {
        var exeDir = AppDomain.CurrentDomain.BaseDirectory;
        var solutionDir = Path.GetFullPath(Path.Combine(exeDir, @"..\..\..\.."));
        this.path = Path.Combine(solutionDir, "TestProjectsDll");
        this.pathParallelTests = Path.Combine(solutionDir, "TestProjectsDll/ParallelTests");

        this.resultForTests = await MyNUnit.Run(this.path);
    }

    [Test]
    public void RunShouldAssertReturnForValue()
    {
        Assert.That(resultForTests.Count(s => s.Contains("passed")), Is.EqualTo(3));
        Assert.That(resultForTests.Count(s => s.Contains("failed")), Is.EqualTo(5));
    }

    [Test]
    public void RunShouldConsiderIgnore()
    {
        Assert.That(resultForTests.Count(s => s.Contains("ignored")), Is.EqualTo(1));
    }

    [Test]
    public void RunShouldConsiderAfterAndBeforeClasses()
    {
        Assert.That(resultForTests.Count(s => s.Contains("BeforeClass") && !s.Contains("static")), Is.EqualTo(1));
        Assert.That(resultForTests.Count(s => s.Contains("AfterClass") && !s.Contains("static")), Is.EqualTo(1));
    }

    [Test]
    public void RunShouldConsiderStaticBeforeAndAfterClasses()
    {
        Assert.That(resultForTests.Count(s => s.Contains("BeforeClass") && s.Contains("static")), Is.EqualTo(1));
    }

    [Test]
    public void RunShouldConsiderBeforeAndAfter()
    {
        Assert.That(resultForTests.Count(s => s.Contains("Before") && s.Contains("Method") && !s.Contains("BeforeClass")), Is.EqualTo(2));
        Assert.That(resultForTests.Count(s => s.Contains("After") && s.Contains("Method") && !s.Contains("AfterClass")), Is.EqualTo(2));
    }

    [Test]
    public async Task RunShouldWorkingInParallel()
    {
        var sw = Stopwatch.StartNew();
        this.resultForParallelTests = await MyNUnit.Run(this.pathParallelTests);
        sw.Stop();

        Assert.That(sw.ElapsedMilliseconds, Is.LessThan(300));
    }
}