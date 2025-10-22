// <copyright file="Client.cs" company="AlexanderKuchin">
// Copyright (c) AlexanderKuchin. All rights reserved.
// </copyright>

using System.Text;

namespace SimpleFTP;

using System.Net.Sockets;

/// <summary>
/// Part of client with List and Get requests.
/// </summary>
/// <param name="port">Port of connection.</param>
/// <param name="host">Host to connect.</param>
public class Client(int port, string host)
{
    public async Task SendListRequestAsync(string path)
    {
        using var client = new TcpClient(host, port);
        Console.WriteLine($"Connected to {host}:{port}");

        var stream = client.GetStream();
        var writer = new StreamWriter(stream);

        await writer.WriteLineAsync($"1 {path}");
        await writer.FlushAsync();

        var reader = new StreamReader(stream);

        var response = await reader.ReadLineAsync();

        Console.WriteLine($"Response: {response}");
    }

    public async Task SendGetRequestAsync(string path)
    {
        using var client = new TcpClient(host, port);
        Console.WriteLine($"Connected to {host}:{port}");

        var stream = client.GetStream();
        var writer = new StreamWriter(stream);

        await writer.WriteLineAsync($"2 {path}");
        await writer.FlushAsync();

        var sizeBytes = new byte[8];
        await stream.ReadExactlyAsync(sizeBytes);
        var size = BitConverter.ToInt64(sizeBytes);
        if (size != -1)
        {
            byte[] buffer = new byte[size];
            await stream.ReadExactlyAsync(buffer);
        }
        else
        {
            var reader = new StreamReader(stream);
            var response = await reader.ReadLineAsync();

            Console.WriteLine($"Response: {size} {response}");
        }
    }
}