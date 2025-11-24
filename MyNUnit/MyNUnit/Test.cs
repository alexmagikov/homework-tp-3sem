// <copyright file="Test.cs" company="Alexander Kuchin">
// Copyright (c) Alexander Kuchin. All rights reserved.
// </copyright>

namespace MyNUnit;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class Test : Attribute
{
    public Test(string ignore, Type expected)
    {
        Ignore = ignore;
        Expected = expected;
    }

    public string? Ignore { get; set; }

    public Type? Expected { get; set; }
}