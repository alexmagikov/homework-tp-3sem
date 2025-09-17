// <copyright file="ParallelMatrixMultiplication.cs" company="AlexMagikov">
// Copyright (c) AlexMagikov. All rights reserved.
// </copyright>

namespace MatrixMultiplication;

/// <summary>
/// Parallel matrix multiplication.
/// </summary>
public class ParallelMatrixMultiplication : MatrixMultiplicationBase
{
    /// <summary>
    /// Parallel matrix multiplication.
    /// </summary>
    /// <param name="matrix1">Matrix1.</param>
    /// <param name="matrix2">Matrix2.</param>
    /// <returns>ResultMatrix.</returns>
    public override int[,] Multiply(int[,] matrix1, int[,] matrix2)
    {
        CheckDimensions(matrix1, matrix2);

        var rowsNum = matrix1.GetLength(0);
        var columnsNum = matrix2.GetLength(1);
        var resultMatrix = new int[rowsNum, columnsNum];

        int threadsNum = Math.Min(Environment.ProcessorCount, rowsNum);
        Thread[] threads = new Thread[threadsNum];

        int rowsForThread = (int)Math.Ceiling((double)rowsNum / threadsNum);

        for (int i = 0; i < threadsNum; i++)
        {
            int startRow = i * rowsForThread;
            int endRow = Math.Min(startRow + rowsForThread, rowsNum);

            threads[i] = new Thread(() =>
            {
                for (int j = startRow; j < endRow; j++)
                {
                    for (int k = 0; k < columnsNum; k++)
                    {
                        resultMatrix[j, k] = CalculateResultMatrixElement(matrix1, matrix2, j, k);
                    }
                }
            });
            threads[i].Start();
        }

        for (int i = 0; i < threadsNum; i++)
        {
            threads[i].Join();
        }

        return resultMatrix;
    }
}