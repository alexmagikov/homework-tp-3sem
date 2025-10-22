
using System.Reflection;
using SimpleFTP;


var root = Directory.GetCurrentDirectory();
Console.WriteLine(root);
var server = new Server(8080);
var client = new Client(8080, "127.0.0.1");

server.StartAsync();

await Task.Delay(500);
await client.SendGetRequestAsync(root + "/SimpleFTP.deps.json");
