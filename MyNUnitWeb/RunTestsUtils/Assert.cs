// <copyright file="Assert.cs" company="Alexander Kuchin">
// Copyright (c) Alexander Kuchin. All rights reserved.
// </copyright>

namespace RunTestsUtils;

public static class Assert
{
    public static void AreEqual(object actual, object expected)
    {
        if (!Equals(expected, actual))
        {
            throw new Exception($"Expected {expected} but was {actual}");
        }
    }
}