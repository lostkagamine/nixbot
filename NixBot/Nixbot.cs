using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.EntityFrameworkCore;
using NixBot.Entities;
using NixBot.Commands;

namespace NixBot;

public class Nixbot
{
    public static NixbotContext DbContext = null!;
    private static DiscordClient Discord = null!;

    private static async Task OnMessage(DiscordClient sender, MessageCreateEventArgs args)
    {
        if (args.Author.IsBot) return;

        var content = args.Message.Content.ToLower();
        content = content.Replace("\u200b", "");

        if (content.Contains("nix"))
        {
            var t = await DbContext.Messages.FindAsync(args.Author.Id);
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (t is null)
            {
                Console.WriteLine($"creating new DB entity for user {args.Author.Username} ({args.Author.Id})");
                DbContext.Messages.Add(new DbMessage
                {
                    UserId = args.Author.Id,
                    LastSaid = now
                });
            }
            else
            {
                t.LastSaid = now;
            }

            await DbContext.SaveChangesAsync();
        }
    }
    
    public static async Task Main(string[] args)
    {
        var token = Environment.GetEnvironmentVariable("NIXBOT_TOKEN")!;
        var prefix = Environment.GetEnvironmentVariable("NIXBOT_PREFIX") ?? "-";

        if (string.IsNullOrEmpty(token))
        {
            throw new Exception("set a token fucko");
        }

        DbContext = new NixbotContext();
        var dbExists = File.Exists(DbContext.DbPath);
        if (!dbExists)
        {
            Console.WriteLine("db doesnt exist, migrating it");
            await DbContext.Database.MigrateAsync();
        }

        Discord = new DiscordClient(new DiscordConfiguration
        {
            Token = token,
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.All
        });

        Discord.MessageCreated += OnMessage;

        Console.WriteLine($"prefix is '{prefix}'");
        
        var cmds = Discord.UseCommandsNext(new CommandsNextConfiguration()
        {
            StringPrefixes = new[] { "-" }
        });
        
        cmds.RegisterCommands<NixbotCommands>();
        cmds.RegisterCommands<OwnerCommands>();

        Discord.GuildDownloadCompleted += async (_, _) =>
        {
            await Discord.UpdateStatusAsync(new DiscordActivity
            {
                Name = "NixOS NixOS NixOS NixOS NixOS NixOS NixOS",
                ActivityType = ActivityType.Watching
            }, UserStatus.Online);
        };

        await Discord.ConnectAsync();
        await Task.Delay(-1);
    }
}