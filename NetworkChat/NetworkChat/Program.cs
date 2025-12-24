// <copyright file="Program.cs" company="Alexander Kuchin">
// Copyright (c) Alexander Kuchin. All rights reserved.
// </copyright>

using System.Net;
using NetworkChat;

if (args.Length == 1)
{
    if (!int.TryParse(args[0], out var port))
    {
        Console.WriteLine("Invalid port");
        return;
    }

    Console.WriteLine($"Listening by server on port {port}");

    var server = new ChatServer(port);
    await server.Start();
}
else if (args.Length == 2)
{
    var ip = args[0];
    if (!IPAddress.TryParse(ip, out var _))
    {
        Console.WriteLine("Invalid ip");
        return;
    }

    if (!int.TryParse(args[1], out var port))
    {
        Console.WriteLine("Invalid port");
        return;
    }

    Console.WriteLine($"Connecting to server {ip}:{port}");
    var client = new ChatClient(ip, port);
    await client.ConnectAsync();
}
else
{
    Console.WriteLine("Invalid args");
}