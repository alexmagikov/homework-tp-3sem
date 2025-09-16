// <copyright file="MatrixMultiplicationBase.cs" company="AlexMagikov">
// Copyright (c) AlexMagikov. All rights reserved.
// </copyright>

namespace MatrixMultiplication;

/// <summary>
/// Abstract class for solution of Sequential and Parallel Multiplication.
/// </summary>
public abstract class MatrixMultiplicationBase
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
    /// <param name="rowsNum">String number.</param>
    /// <param name="columnsNum">Column number.</param>
    /// <returns>Element of result matrix of Multiplication.</returns>
    protected int CalculateResultMatrixElement(
        int[,] matrix1,
        int[,] matrix2,
        int rowsNum,
        int columnsNum)
    {
        int sum = 0;
        for (int i = 0; i < rowsNum; i++)
        {
            sum += matrix1[rowsNum, i] * matrix2[i, columnsNum];
        }

        return sum;
    }

    /// <summary>
    /// Check dimensions matching.
    /// </summary>
    /// <param name="matrix1">Matrix1.</param>
    /// <param name="matrix2">Matrix2</param>
    /// <returns>Yes if dimensions are matching, else - false.</returns>
    protected  CheckDimensions(int[,] matrix1, int[,] matrix2)
    {
        var matrix1NumStrings = matrix1.GetLength(0);
        var matrix1NumColumns = matrix1.GetLength(1);
        var matrix2NumStrings = matrix2.GetLength(0);
        var matrix2NumColumns = matrix2.GetLength(1);

        if ((matrix1NumColumns != matrix2NumStrings) || (matrix1NumStrings == matrix2NumColumns))
        {
            result = Multiply(matrix1, matrix2, matrix1NumStrings, matrix2NumColumns);
        }
        else if (matrix1NumStrings == matrix2NumColumns)
        {
            result = Multiply(matrix2, matrix1, matrix2NumColumns, matrix1NumStrings);
        }
        else
        {
            throw new FormatException("Dimensions of the matrices don't match");
        }
    }
}