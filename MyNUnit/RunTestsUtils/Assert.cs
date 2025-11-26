// <copyright file="Assert.cs" company="Alexander Kuchin">
// Copyright (c) Alexander Kuchin. All rights reserved.
// </copyright>

namespace RunTestsUtils;

public static class Assert
{
    public static void IsEqual(object actual, object expected)
    {
        if (!Equals(expected, actual))
        {
            throw new Exception($"Expected {expected} but was {actual}");
        }
    }

    public static void Throws<TException>(Action action)
        where TException : Exception
    {
        try
        {
            action();
            throw new Exception($"Expected exception {typeof(TException).Name} but no exception was");
        }
        catch (TException)
        {
        }
        catch (Exception e)
        {
            throw new Exception($"Expected exception {typeof(TException).Name} but was {e.GetType().Name}");
        }
    }
}