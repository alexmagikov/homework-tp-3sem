// <copyright file="TestInfo.cs" company="Alexander Kuchin">
// Copyright (c) Alexander Kuchin. All rights reserved.
// </copyright>

namespace MyNUnitWeb.Data;

using System.ComponentModel.DataAnnotations;

public class TestInfo
{
    public int Id { get; init; }

    [MaxLength(512)]
    public required string AssemblyName { get; init; }

    [MaxLength(512)]
    public required string TestName { get; init; }

    public bool IsPassed { get; init; }

    public long WorkTime { get; init; }

    [MaxLength(512)]
    public string? ErrorMessage { get; init; }

    [MaxLength(512)]
    public string? IgnoreReason { get; init; }
}