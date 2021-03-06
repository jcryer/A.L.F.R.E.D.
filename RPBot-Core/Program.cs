﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using Newtonsoft.Json;
using DSharpPlus.CommandsNext.Exceptions;
using System.Net.Sockets;

namespace RPBot
{
    public class Program
    {

        
        public static string token;
        public static Random random = new Random();

        public static void Main(string[] args) => new Program().Run(args).GetAwaiter().GetResult();

        private async Task Run(string[] args)
        {
           //File.WriteAllText("test.txt", WikiTests.CreateInfoBox(new InfoBoxObject("jeff", "the slayer", "no<br>maybe<br>perhaps", "gay", "Single", 25, "23/02/2000", "North Pier", "South Pier", "Human", "Male", "182cm", "70kg", "Bright Blue", "Hot pink", new string[] { "Image 1", "Image 2", "Image 3" }, new string[] { "https://cdn.discordapp.com/attachments/447483115250909184/473256482616508436/image.jpg", "https://cdn.discordapp.com/attachments/447483115250909184/473257220813881344/image.jpg", "https://cdn.discordapp.com/attachments/455019174385942530/472809787780759562/a7a1c3d8cffc5dff31021d5cfd3a135f.png" })));
           if (args.Any())
            {
                RPClass.Restarted = true;
            }
            RPClass.LoadData();
            var cfg = new Config();
            var json = string.Empty;
            if (File.Exists("config.json"))
            {
                json = File.ReadAllText("config.json", new UTF8Encoding(false));
            }
            else if (File.Exists("../../config.json"))
            {
                json = File.ReadAllText("../../config.json", new UTF8Encoding(false));
            }
            else
            {
                json = JsonConvert.SerializeObject(cfg);
                File.WriteAllText("config.json", json, new UTF8Encoding(false));
                Console.WriteLine("Config file was not found, a new one was generated. Fill it with proper values and rerun this program");
                Console.ReadKey();

                return;
            }
            
            cfg = JsonConvert.DeserializeObject<Config>(json);

            var tskl = new List<Task>();
            for (var i = 0; i < cfg.ShardCount; i++)
            {
                var bot = new RPBot(cfg, i);
                tskl.Add(bot.RunAsync());
                await Task.Delay(7500);
            }

            await Task.WhenAll(tskl);
            await Task.Delay(-1);
        }

        public static void Listener()
        {
            while (true)
            {
                try
                {
                    IPAddress localaddress = IPAddress.Any;
                    IPEndPoint endpoint = new IPEndPoint(localaddress, 80);
                    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                    socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);

                    socket.Bind(endpoint);
                    socket.Listen(10);

                    Socket clientsocket = socket.Accept();
                    clientsocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);

                    byte[] buffer = new byte[1024];
                    int receivelength = clientsocket.Receive(buffer, 1024, SocketFlags.None);
                    string requeststring = Encoding.UTF8.GetString(buffer, 0, receivelength);


                    try
                    {
                        string query = requeststring.Split(' ')[1].TrimStart('/');
                        
                         Console.WriteLine(query);
                    }
                    catch 
                    {
                    }

                    string responseBody = "<b>aaaaaaaaaaaah\ntest</b>";

                    string statusLine = "HTTP/1.1 200 OK\r\n";
                    string responseHeader = "Content-Type: text/html\r\n";

                    clientsocket.Send(Encoding.UTF8.GetBytes(statusLine));
                    clientsocket.Send(Encoding.UTF8.GetBytes(responseHeader));
                    clientsocket.Send(Encoding.UTF8.GetBytes("\r\n"));
                    clientsocket.Send(Encoding.UTF8.GetBytes(responseBody));

                    socket.Close();
                    socket.Dispose();
                    clientsocket.Close();
                    clientsocket.Dispose();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + "\n\n" + e.StackTrace + "\n\n" + e.InnerException + "\n\n" + e.Source);

                }
            }
        }


    }
}