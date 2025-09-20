// <copyright file="MatrixMultiplierBase.cs" company="AlexMagikov">
// Copyright (c) AlexMagikov. All rights reserved.
// </copyright>

namespace MatrixMultiplication;

/// <summary>
/// Abstract class for solution of Sequential and Parallel Multiplication.
/// </summary>
public abstract class MatrixMultiplierBase
{
    /// <summary>
    /// Multiplication of 2 matrices.
    /// </summary>
    /// <param name="matrix1">Matrix1.</param>
    /// <param name="matrix2">Matrix2.</param>
    /// <returns>Result matrix of multiplication.</returns>
    public abstract int[,] Multiply(int[,] matrix1, int[,] matrix2);

    /// <summary>
    /// Calculate matrix element by indexes.
    /// </summary>
    /// <param name="matrix1">Matrix1.</param>
    /// <param name="matrix2">Matrix2.</param>
    /// <param name="rowIndex">String number.</param>
    /// <param name="columnIndex">Column number.</param>
    /// <returns>Element of result matrix of Multiplication.</returns>
    protected static int CalculateResultMatrixElement(
        int[,] matrix1,
        int[,] matrix2,
        int rowIndex,
        int columnIndex)
    {
        int sum = 0;
        for (int i = 0; i < matrix1.GetLength(1); i++)
        {
            sum += matrix1[rowIndex, i] * matrix2[i, columnIndex];
        }

        return sum;
    }

    /// <summary>
    /// Check dimensions matching.
    /// </summary>
    /// <param name="matrix1">Matrix1.</param>
    /// <param name="matrix2">Matrix2.</param>
    protected static void CheckDimensions(int[,] matrix1, int[,] matrix2)
    {
        var matrix1NumColumns = matrix1.GetLength(1);
        var matrix2NumStrings = matrix2.GetLength(0);

        if (matrix1NumColumns != matrix2NumStrings)
        {
            throw new ArgumentException("Dimensions of the matrices don't match");
        }

        if (matrix1.GetLength(0) == 0)
        {
            throw new ArgumentNullException("Null matrix");
        }
    }
}