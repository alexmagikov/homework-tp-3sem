// <copyright file="Program.cs" company="Alexander Kuchin">
// Copyright (c) Alexander Kuchin. All rights reserved.
// </copyright>

namespace MyNUnit;

/// <summary>
/// Main class.
/// </summary>
internal static class Program
{
    /// <summary>
    /// Main method.
    /// </summary>
    /// <param name="args">Args.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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

        await MyNUnit.Run(path);
    }
}
