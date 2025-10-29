
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using SimpleFTP;

Console.WriteLine();
var root = Directory.GetCurrentDirectory();

var server = new Server(8080);

var serverTask = server.StartAsync();

await Task.Delay(500);

var client = new Client(8080, "127.0.0.1");

await client.SendGetRequestAsync("SimpleFTP.deps.json");
