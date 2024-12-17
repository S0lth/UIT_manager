

using Microsoft.AspNetCore.SignalR.Client;

var connection = new HubConnectionBuilder().WithUrl( "http://localhost:5014/ApiHub").Build();

connection.On<int>("ReceiveMessage", message => {
    Console.WriteLine(message);
});

try {
    await connection.StartAsync();
    Console.WriteLine("Connection established");
    Console.ReadLine();
}
catch (Exception e) {
    Console.WriteLine(e);
}