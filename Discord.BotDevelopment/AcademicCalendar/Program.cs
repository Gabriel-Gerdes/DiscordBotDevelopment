using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AcademicCalendar
{
    class Program
    {
        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _client.MessageReceived += CommandHandler;

            _client.Log += Log;

            //Move this to an encrypted file
            var token = File.ReadAllText("Token.txt");


            await _client.LoginAsync(Discord.TokenType.Bot, token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private Task CommandHandler(SocketMessage message)
        {
            string command = "";
            int lengthOfCommand = -1;

            //Filter non Prefix
            if (!message.Content.StartsWith('!'))
                return Task.CompletedTask;

            //Removes messages from bots.
            if (message.Author.IsBot)
                return Task.CompletedTask;

            if (message.Content.Contains(' '))
                lengthOfCommand = message.Content.IndexOf(' ');
            else
                lengthOfCommand = message.Content.Length;

            command = message.Content.Substring(1, lengthOfCommand - 1).ToLower();

            //Actual Command processing
            ExecuteCommand(command, message);

            return Task.CompletedTask;
        }
        private Task ExecuteCommand(string Command, SocketMessage message)
        {
            try
            {
                switch (Command)
                {
                    case "hello":
                        message.Channel.SendMessageAsync($"Hello {message.Author.Mention}");
                        break;
                    case "calendar":
                        CalendarCommand(message);
                        break;

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return Task.CompletedTask;
        }
        private void CalendarCommand(SocketMessage message)
        {
            HtmlScrapper scrapper = new HtmlScrapper();
            var calender = scrapper.GetAcademicCalender();

            StringBuilder sb = new StringBuilder();
            foreach (var supCalender in calender)
            {
                foreach (var eventDate in supCalender)
                {
                    sb.AppendLine(eventDate);
                }
                Console.WriteLine($"Sending {supCalender.Count} dates.");
                message.Channel.SendMessageAsync(sb.ToString());
                sb.Clear();
            }
        }
    }
}

