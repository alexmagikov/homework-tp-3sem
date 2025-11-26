// <copyright file="Program.cs" company="Alexander Kuchin">
// Copyright (c) Alexander Kuchin. All rights reserved.
// </copyright>

namespace MyNUnit;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Use path argument");
            return;
        }

        var path = args[0];

        if (!Directory.Exists(path))
        {
            Console.WriteLine("Directory not found");
            return;
        }

        var result = await MyNUnit.Run(path);
    }
}
