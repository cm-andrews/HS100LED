using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

Console.WriteLine("Enter IP address of HS plug:");
IPAddress ip;
while (!IPAddress.TryParse(Console.ReadLine(), out ip))
{
    Console.WriteLine("Invalid IP address.");
}

string off = null;
Console.WriteLine("Enter 0 to turn LED OFF. Enter 1 to turn LED ON");
while (off == null)
{
    switch (Console.ReadLine())
    {
        case "1":
            off = "0";
            break;
        case "0":
            off = "1";
            break;
        default:
            Console.WriteLine("Enter 0 or 1.");
            break;
    }
}

var command = "{\"system\":{\"set_led_off\":{\"off\":" + off + "}}}";
byte key = 0xAB;
var cipherBytes = new byte[command.Length];
for (var i = 0; i < command.Length; i++)
{
    cipherBytes[i] = Convert.ToByte(command[i] ^ key);
    key = cipherBytes[i];
}
var value = (uint)command.Length;
var jsonPayload = BitConverter.GetBytes((value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 | (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24).Concat(cipherBytes).ToArray();

using var sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
sender.Connect(ip, 9999);
sender.Send(jsonPayload);