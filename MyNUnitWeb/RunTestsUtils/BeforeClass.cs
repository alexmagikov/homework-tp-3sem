// <copyright file="BeforeClass.cs" company="Alexander Kuchin">
// Copyright (c) Alexander Kuchin. All rights reserved.
// </copyright>

namespace RunTestsUtils;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class BeforeClass : Attribute
{
}