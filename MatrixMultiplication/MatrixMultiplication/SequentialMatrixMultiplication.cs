// <copyright file="SequentialMatrixMultiplication.cs" company="AlexMagikov">
// Copyright (c) AlexMagikov. All rights reserved.
// </copyright>

namespace MatrixMultiplication;

/// <summary>
/// Matrix sequential multiplication.
/// </summary>
public class SequentialMatrixMultiplication : MatrixMultiplicationBase
{
    public override Multiply()
    {
        for (int i = 0; i < rowsNum; i++)
        {
            for (int j = 0; j < columnsNum; j++)
            {
                result[i, j] = CalculateMatrixElement(matrix1, matrix2, i, j);
            }
        }

        return result;
    }
}