// <copyright file="ChatServer.cs" company="Alexander Kuchin">
// Copyright (c) Alexander Kuchin. All rights reserved.
// </copyright>

namespace NetworkChat;

using System.Net;
using System.Net.Sockets;

public class ChatServer(int port) : IDisposable
{
    private readonly TcpListener listener = new(IPAddress.Any, port);
    private readonly CancellationTokenSource cts = new();

    public async Task Start()
    {
        try
        {
            this.listener.Start();
            Console.WriteLine($"Server listening on port: {port}");

            while (!this.cts.IsCancellationRequested)
            {
                Socket socket;
                try
                {
                    socket = await this.listener.AcceptSocketAsync();
                }
                catch (ObjectDisposedException)
                {
                    this.listener.Stop();
                    break;
                }

                var messenger = new ChatMessenger(Console.In, Console.Out);

                var stream = new NetworkStream(socket);
                await messenger.RunAsync(stream,  this.cts.Token);
            }
        }
        finally
        {
            this.listener.Dispose();
        }
    }

    public void Dispose()
    {
        this.listener.Stop();
        this.listener.Dispose();
        this.cts.Cancel();
    }
}