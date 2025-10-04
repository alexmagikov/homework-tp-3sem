// <copyright file="InitMatrix.cs" company="AlexMagikov">
// Copyright (c) AlexMagikov. All rights reserved.
// </copyright>

namespace MatrixMultiplication;

/// <summary>
/// Class for working with files with matrix.
/// </summary>
public class InitMatrix
{
    /// <summary>
    /// Read matrix from file.
    /// </summary>
    /// <param name="path">Path of the file.</param>
    /// <returns>Matrix.</returns>
    public static int[,] ReadFile(string path)
    {
        string[] lines = File.ReadAllLines(path);

        if (lines.Length == 0)
        {
            throw new FormatException();
        }

        int rows = lines.Length;
        int columns = lines[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;

        int[,] matrix = new int[rows, columns];

        for (int i = 0; i < rows; i++)
        {
            string[] parts = lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != columns)
            {
                throw new FormatException("Wrong elements number.");
            }

            for (int j = 0; j < columns; j++)
            {
                matrix[i, j] = int.Parse(parts[j]);
            }
        }

        return matrix;
    }
}