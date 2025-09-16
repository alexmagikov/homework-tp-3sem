// <copyright file="Program.cs" company="AlexMagikov">
// Copyright (c) AlexMagikov. All rights reserved.
// </copyright>

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
    var result = 
}
catch (Exception exception)
{
    Console.WriteLine(exception.Message);
    return 1;
}

return 0;