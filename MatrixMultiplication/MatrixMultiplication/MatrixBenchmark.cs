// <copyright file="MatrixBenchmark.cs" company="AlexMagikov">
// Copyright (c) AlexMagikov. All rights reserved.
// </copyright>

namespace MatrixMultiplication;

using System.Diagnostics;

/// <summary>
/// Class for benchmarking.
/// </summary>
public static class MatrixBenchmark
{
    /// <summary>
    /// Benchmarking.
    /// </summary>
    public static void RunBenchmark()
    {
        var benchmarkingSize = new (int, int)[]
        {
        (40, 40),
        (100, 100),
        (500, 500),
        (1000, 1000),
        };

        var numExperiment = 100;

        var sequentialMultiplier = new SequentialMatrixMultiplier();
        var parallelMultiplier = new ParallelMatrixMultiplier();

        Console.WriteLine(
        $"{"Type", -10}" +
        $" {"Size", -10} " +
        $"{"Mean time", -15}" +
        $" {"Num Of Tests", -15} " +
        $"{"Standard Deviation", -20}");

        for (int i = 0; i < benchmarkingSize.Length; i++)
        {
            var parallelTimeArray = new double[numExperiment];
            var sequentialTimeArray = new double[numExperiment];

            for (int j = 0; j < numExperiment; j++)
            {
                var matrix1 = CreateMatrix(benchmarkingSize[i].Item1, benchmarkingSize[i].Item2, -100, 100);
                var matrix2 = CreateMatrix(benchmarkingSize[i].Item2, benchmarkingSize[i].Item1, -100, 100);

                var sw = Stopwatch.StartNew();
                parallelMultiplier.Multiply(matrix1, matrix2);
                sw.Stop();
                parallelTimeArray[j] = sw.ElapsedMilliseconds;

                sw.Restart();
                sequentialMultiplier.Multiply(matrix1, matrix2);
                sw.Stop();
                sequentialTimeArray[j] = sw.ElapsedMilliseconds;
            }

            var parallelMean = CalculateMean(parallelTimeArray.Sum(), numExperiment);
            var sequentialMean = CalculateMean(sequentialTimeArray.Sum(), numExperiment);

            var parallelStandardDeviation = CalculateStandardDeviation(parallelTimeArray, parallelMean);
            var sequentialStandardDeviation = CalculateStandardDeviation(sequentialTimeArray, sequentialMean);

            Console.WriteLine(
            $"{"Parallel",-10} " +
            $"{$"{benchmarkingSize[i].Item1}x{benchmarkingSize[i].Item2}",-10} " +
            $"{parallelMean,-15:F2} " +
            $"{numExperiment,-15} " +
            $"{parallelStandardDeviation,-20:F4}");

            Console.WriteLine(
            $"{"Sequential",-10} " +
            $"{$"{benchmarkingSize[i].Item1}x{benchmarkingSize[i].Item2}",-10} " +
            $"{sequentialMean,-15:F2} " +
            $"{numExperiment,-15} " +
            $"{sequentialStandardDeviation,-20:F4}");
        }
    }

    /// <summary>
    /// Create matrix by parameters.
    /// </summary>
    /// <param name="rowsNum">Number of rows.</param>
    /// <param name="columnsNum">Number of columns.</param>
    /// <param name="minNum">Min border of value.</param>
    /// <param name="maxNum">Max border of value.</param>
    /// <exception cref="ArgumentException">Argument exception.</exception>
    /// <returns>Matrix.</returns>
    public static int[,] CreateMatrix(int rowsNum, int columnsNum, int minNum, int maxNum)
    {
        if (rowsNum <= 0 || columnsNum <= 0 || minNum > maxNum)
        {
            throw new ArgumentException();
        }

        var matrix = new int[rowsNum, columnsNum];

        for (int i = 0; i < rowsNum; i++)
        {
            for (int j = 0; j < columnsNum; j++)
            {
                matrix[i, j] = Random.Shared.Next(minNum, maxNum + 1);
            }
        }

        return matrix;
    }

    /// <summary>
    /// Write resultMatrix to file.
    /// </summary>
    /// <param name="matrix">ResultMatrix.</param>
    public static void WriteFile(int[,] matrix)
    {
        using (StreamWriter writer = new StreamWriter("resultMatrix.txt"))
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    writer.Write(matrix[i, j] + " ");
                }

                writer.WriteLine();
            }
        }
    }

    /// <summary>
    /// Checking for equality of matrices.
    /// </summary>
    /// <param name="matrix1">Matrix1.</param>
    /// <param name="matrix2">Matrix2.</param>
    /// <returns>True if they are equal, else - false.</returns>
    public static bool AreMatricesEqual(int[,] matrix1, int[,] matrix2)
    {
        if ((matrix1.GetLength(0) != matrix2.GetLength(0)) || (matrix1.GetLength(1) != matrix2.GetLength(1)))
        {
            return false;
        }

        for (int i = 0; i < matrix1.GetLength(0); i++)
        {
            for (int j = 0; j < matrix1.GetLength(1); j++)
            {
                if (matrix1[i, j] != matrix2[i, j])
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static double CalculateMean(double time, int experimentNum)
        => time / experimentNum;

    private static double CalculateStandardDeviation(double[] time, double mean)
    {
        double sum = 0;

        for (int i = 0; i < time.Length; i++)
        {
            sum += (time[i] - mean) * (time[i] - mean);
        }

        return Math.Sqrt(sum / time.Length);
    }
}