// <copyright file="MatrixMultiplicationTests.cs" company="AlexMagikov">
// Copyright (c) AlexMagikov. All rights reserved.
// </copyright>

namespace MatrixMultiplicationTests;

using MatrixMultiplication;

public class MatrixMultiplicationTests
{
    private SequentialMatrixMultiplier sequentialMultiplier = new SequentialMatrixMultiplier();
    private ParallelMatrixMultiplier parallelMultiplier = new ParallelMatrixMultiplier();

    [Test]
    public void EqualityOfParallelAndSequentialMultipliers()
    {
        var matrix1 = new int[,]
        {
        { 1, 2, 3, 4 },
        { 1, 2, 3, 4 },
        { 1, 2, 3, 44 },
        };
        var matrix2 = new int[,]
        {
        { 1, 2, 3, 4, 5 },
        { 1, 2, 3, 4, 5 },
        { 1, 2, 3, 4, 5 },
        { 1, 2, 3, 4, 5 },
        };

        var resultMatrix1 = this.parallelMultiplier.Multiply(matrix1, matrix2);
        var resultMatrix2 = this.sequentialMultiplier.Multiply(matrix1, matrix2);

        Assert.That(MatrixBenchmark.AreMatricesEqual(resultMatrix1, resultMatrix2), Is.True);
    }

    [Test]
    public void SequentialMultiplicationForNormalData()
    {
        var matrix1 = new int[,]
        {
        { 1, 2, 3, 4 },
        { 1, 2, 3, 4 },
        { 1, 2, 3, 44 },
        };
        var matrix2 = new int[,]
        {
        { 1, 2, 3, 4, 5 },
        { 1, 2, 3, 4, 5 },
        { 1, 2, 3, 4, 5 },
        { 1, 2, 3, 4, 5 },
        };

        var expectedResultMatrix = new int[,]
        {
        { 10, 20, 30, 40, 50 },
        { 10, 20, 30, 40, 50 },
        { 50, 100, 150, 200, 250 },
        };

        var resultMatrix = this.sequentialMultiplier.Multiply(matrix1, matrix2);

        Assert.That(MatrixBenchmark.AreMatricesEqual(resultMatrix, expectedResultMatrix), Is.True);
    }

    [Test]
    public void ParallelMultiplicationForNormalData()
    {
        var matrix1 = new int[,]
         {
        { 1, 2, 3, 4 },
        { 1, 2, 3, 4 },
        { 1, 2, 3, 44 },
         };
        var matrix2 = new int[,]
        {
        { 1, 2, 3, 4, 5 },
        { 1, 2, 3, 4, 5 },
        { 1, 2, 3, 4, 5 },
        { 1, 2, 3, 4, 5 },
        };

        var expectedResultMatrix = new int[,]
        {
        { 10, 20, 30, 40, 50 },
        { 10, 20, 30, 40, 50 },
        { 50, 100, 150, 200, 250 },
        };
        var resultMatrix = this.parallelMultiplier.Multiply(matrix1, matrix2);

        Assert.That(MatrixBenchmark.AreMatricesEqual(resultMatrix, expectedResultMatrix), Is.True);
    }

    [Test]
    public void MultiplicationOfEmptyMatrices()
    {
        var matrix1 = new int[0, 0];
        var matrix2 = new int[0, 0];
        Assert.Throws<ArgumentNullException>(() => this.sequentialMultiplier.Multiply(matrix1, matrix2));
        Assert.Throws<ArgumentNullException>(() => this.parallelMultiplier.Multiply(matrix1, matrix2));
    }

    [Test]
    public void MultiplicationOfSingleElementMatrices()
    {
        var matrix1 = new int[,] { { 5 } };
        var matrix2 = new int[,] { { 7 } };

        var expectedResultMatrix = new int[,] { { 35 } };

        var resultMatrix1 = this.sequentialMultiplier.Multiply(matrix1, matrix2);
        var resultMatrix2 = this.parallelMultiplier.Multiply(matrix1, matrix2);

        Assert.That(MatrixBenchmark.AreMatricesEqual(resultMatrix1, expectedResultMatrix), Is.True);
        Assert.That(MatrixBenchmark.AreMatricesEqual(resultMatrix2, expectedResultMatrix), Is.True);
    }

    [Test]
    public void MultiplicationWithIncompatibleDimensions()
    {
        var matrix1 = new int[,]
        {
        { 1, 2 },
        { 3, 4 },
        };
        var matrix2 = new int[,] { { 1, 2, 3 } };

        Assert.Throws<ArgumentException>(() => this.sequentialMultiplier.Multiply(matrix1, matrix2));
        Assert.Throws<ArgumentException>(() => this.parallelMultiplier.Multiply(matrix1, matrix2));
    }

    [Test]
    public void MultiplicationWithZeroMatrix()
    {
        var matrix = new int[,]
        {
        { 1, 2 },
        { 3, 4 },
        };

        var zeroMatrix = new int[,]
        {
        { 0, 0 },
        { 0, 0 },
        };

        var expectedResultMatrix = new int[,]
        {
        { 0, 0 },
        { 0, 0 },
        };

        var resultMatrix1 = this.sequentialMultiplier.Multiply(matrix, zeroMatrix);
        var resultMatrix2 = this.parallelMultiplier.Multiply(matrix, zeroMatrix);

        Assert.That(MatrixBenchmark.AreMatricesEqual(resultMatrix1, expectedResultMatrix), Is.True);
        Assert.That(MatrixBenchmark.AreMatricesEqual(resultMatrix2, expectedResultMatrix), Is.True);
    }
}
