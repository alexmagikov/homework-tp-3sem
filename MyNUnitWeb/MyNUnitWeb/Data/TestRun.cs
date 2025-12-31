// <copyright file="TestRun.cs" company="Alexander Kuchin">
// Copyright (c) Alexander Kuchin. All rights reserved.
// </copyright>

using MyNUnit;

namespace MyNUnitWeb.Data;

public class TestRun
{
    public int Id { get; set; }

    public required string DirectoryName { get; set; }

    public List<TestInfo> TestInfos { get; set; } = [];
}