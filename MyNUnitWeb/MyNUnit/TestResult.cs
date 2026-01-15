// <copyright file="TestResult.cs" company="Alexander Kuchin">
// Copyright (c) Alexander Kuchin. All rights reserved.
// </copyright>

namespace MyNUnit;

/// <summary>
/// Test result data.
/// </summary>
/// <param name="AssemblyName">Name of assembly.</param>
/// <param name="TestName">Name of test method.</param>
/// <param name="IsPassed">Passed parameter.</param>
/// <param name="WorkTime">Start time.</param>
/// <param name="ErrorMessage">Error message.</param>
/// <param name="IgnoreReason">Ignore reason.</param>
public record struct TestResult(
    string AssemblyName,
    string TestName,
    bool IsPassed,
    long WorkTime,
    string? ErrorMessage,
    string? IgnoreReason);