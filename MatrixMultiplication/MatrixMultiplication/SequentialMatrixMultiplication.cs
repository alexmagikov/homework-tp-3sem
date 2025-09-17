// <copyright file="SequentialMatrixMultiplication.cs" company="AlexMagikov">
// Copyright (c) AlexMagikov. All rights reserved.
// </copyright>

namespace MatrixMultiplication;

/// <summary>
/// Matrix sequential multiplication.
/// </summary>
public class SequentialMatrixMultiplication : MatrixMultiplicationBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SequentialMatrixMultiplication"/> class.
    /// </summary>
    /// <param name="matrix1">Matrix1.</param>
    /// <param name="matrix2">Matrix2.</param>
    /// <returns>Result matrix.</returns>
    public override int[,] Multiply(int[,] matrix1, int[,] matrix2)
    {
        CheckDimensions(matrix1, matrix2);

        var rowsNum = matrix1.GetLength(0);
        var columnsNum = matrix2.GetLength(1);

        var result = new int[rowsNum, columnsNum];

        for (int i = 0; i < rowsNum; i++)
        {
            for (int j = 0; j < columnsNum; j++)
            {
                result[i, j] = CalculateResultMatrixElement(matrix1, matrix2, i, j);
            }
        }

        return result;
    }
}