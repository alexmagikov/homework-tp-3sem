// <copyright file="Test.cs" company="Alexander Kuchin">
// Copyright (c) Alexander Kuchin. All rights reserved.
// </copyright>

namespace RunTestsUtils;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class Test : Attribute
{
    public string? Ignore { get; set; } = null;

    public Type? Expected { get; set; } = null;
}