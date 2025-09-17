// <copyright file="FileGenerator.cs" company="AlexMagikov">
// Copyright (c) AlexMagikov. All rights reserved.
// </copyright>

namespace MatrixMultiplication;

internal class MatrixGenerator
{
    /// <summary>
    /// Create matrix by parameters.
    /// </summary>
    /// <param name="rowsNum">Number of rows.</param>
    /// <param name="columnsNum">Number of columns.</param>
    /// <param name="minNum">Min border of value.</param>
    /// <param name="maxNum">Max border of value.</param>
    /// <param name="experimentNum">Number of generation.</param>
    /// <exception cref="ArgumentException">Argument exception.</exception>
    public static void CreateMatrix(int rowsNum, int columnsNum, int minNum, int maxNum, int experimentNum)
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
                matrix[i, j] = Random.Shared.Next(minNum, maxNum);
            }
        }
    }
}