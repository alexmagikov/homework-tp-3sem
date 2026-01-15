// <copyright file="TestRun.cs" company="Alexander Kuchin">
// Copyright (c) Alexander Kuchin. All rights reserved.
// </copyright>

namespace MyNUnitWeb.Data;

using System.ComponentModel.DataAnnotations;

public class TestRun
{
    public int Id { get; set; }

    [MaxLength(512)]
    public required string DirectoryName { get; set; }

    public List<TestInfo> TestInfos { get; set; } = [];
}