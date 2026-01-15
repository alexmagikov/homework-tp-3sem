// <copyright file="UploadedAssembly.cs" company="Alexander Kuchin">
// Copyright (c) Alexander Kuchin. All rights reserved.
// </copyright>

namespace MyNUnitWeb.Data;

using System.ComponentModel.DataAnnotations;

public class UploadedAssembly
{
    public int Id { get; set; }

    [MaxLength(512)]
    public required string Name { get; init; }

    [MaxLength(512)]
    public required string Path { get; init; }

}