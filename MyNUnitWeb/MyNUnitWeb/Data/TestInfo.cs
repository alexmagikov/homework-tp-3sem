// <copyright file="TestInfo.cs" company="Alexander Kuchin">
// Copyright (c) Alexander Kuchin. All rights reserved.
// </copyright>

namespace MyNUnitWeb.Data;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Test info dto.
/// </summary>
public class TestInfo
{
    /// <summary>
    /// Gets primary key.
    /// </summary>
    public int Id { get; set; }

    public int TestId { get; set; }

    public TestRun? TestRun { get; set; }

    /// <summary>
    /// Gets name of assembly.
    /// </summary>
    [MaxLength(512)]
    public required string AssemblyName { get; init; }

    /// <summary>
    /// Gets name of Test.
    /// </summary>
    [MaxLength(512)]
    public required string TestName { get; init; }

    /// <summary>
    /// Gets a value indicating whether it gets info of passing.
    /// </summary>
    public bool IsPassed { get; init; }

    /// <summary>
    /// Gets workTime.
    /// </summary>
    public long WorkTime { get; init; }

    /// <summary>
    /// Gets error message.
    /// </summary>
    [MaxLength(512)]
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Gets ignore reason.
    /// </summary>
    [MaxLength(512)]
    public string? IgnoreReason { get; init; }
}