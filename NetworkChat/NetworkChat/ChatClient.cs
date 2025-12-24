// <copyright file="ChatClient.cs" company="Alexander Kuchin">
// Copyright (c) Alexander Kuchin. All rights reserved.
// </copyright>

namespace NetworkChat;

using System.Net.Sockets;

/// <summary>
/// Chat client.
/// </summary>
/// <param name="ipAddress">ip.</param>
/// <param name="port">port.</param>
public class ChatClient(string ipAddress, int port)
{
    /// <summary>
    /// Connects to server.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task ConnectAsync()
    {
        var client = new TcpClient(ipAddress, port);
        Console.WriteLine($"Connecting to {ipAddress}:{port}");
        try
        {
            var stream = client.GetStream();

            var messenger = new ChatMessenger(Console.In, Console.Out);

            await messenger.RunAsync(stream, CancellationToken.None);
        }
        catch (SocketException)
        {
            Console.WriteLine("Connection failed");
        }
    }
}