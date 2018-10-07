using System;
using System.IO;
using System.Threading.Tasks;
using Assets.Scripts.DungeonMaster;
using DSharpPlus;
using DSharpPlus.CommandsNext;

namespace ShrinelandsDiscordBot
{
    class Program
    {
        static DiscordClient discord;
        private static string key;
        public static Battle battle;
        static CommandsNextModule commands;

        static void Main(string[] args)
        {
            TextReader tr = new StreamReader(@"Key.txt");
            key = tr.ReadLine();

            battle = DebugData.GetFunDebugBattle();

            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();

        }

        static async Task MainAsync(string[] args)
        {
            discord = new DiscordClient(new DiscordConfiguration
            {
                Token = key,
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug
            });

            commands = discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = "!"
            });

            commands.RegisterCommands<Commands>();

            discord.MessageCreated += async e =>
            {
                if (e.Message.Content.ToLower().StartsWith("ping"))
                    await e.Message.RespondAsync("pong!");
            };

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}