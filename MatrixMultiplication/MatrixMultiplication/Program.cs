// <copyright file="Program.cs" company="AlexMagikov">
// Copyright (c) AlexMagikov. All rights reserved.
// </copyright>

using System.Diagnostics;
using MatrixMultiplication;

if (args.Length < 2)
{
    Console.WriteLine("Usage: program <inputPath> <inputPath>");
    return 1;
}

var inputPath1 = args[0];
var inputPath2 = args[1];

try
{
    var matrix1 = InitMatrix.ReadFile(inputPath1);
    var matrix2 = InitMatrix.ReadFile(inputPath2);

    var sequentialMultiplier = new SequentialMatrixMultiplier();
    var parallelMultiplier = new ParallelMatrixMultiplier();

    int iterations = 1000;
    long parallelTotalTime = 0;
    long sequentialTotalTime = 0;

    var resultMatrix = parallelMultiplier.Multiply(matrix1, matrix2);

    for (int i = 0; i < iterations; i++)
    {
        var sw = Stopwatch.StartNew();
        parallelMultiplier.Multiply(matrix1, matrix2);
        sw.Stop();
        parallelTotalTime += sw.ElapsedMilliseconds;

        sw.Restart();
        sequentialMultiplier.Multiply(matrix1, matrix2);
        sw.Stop();
        sequentialTotalTime += sw.ElapsedMilliseconds;
    }

    double parallelAvg = (double)parallelTotalTime / iterations;
    double sequentialAvg = (double)sequentialTotalTime / iterations;

    Console.WriteLine($"Avg time parallel multiplication: {parallelAvg:F3} мс");
    Console.WriteLine($"Avg time sequential multiplication: {sequentialAvg:F3} мс");

    MatrixBenchmark.WriteFile(resultMatrix);
    Console.WriteLine("Result has written to file with name 'resultMatrix.txt' ");

    Console.WriteLine();
    Console.WriteLine("Results of benchmark: ");
    MatrixBenchmark.RunBenchmark();
}
catch (Exception exception)
{
    Console.WriteLine(exception.Message);
    return 1;
}

return 0;