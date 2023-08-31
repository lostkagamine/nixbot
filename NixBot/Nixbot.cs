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

    private static async Task NixCheck(DiscordMessage msg)
    {
        if (msg.Author.IsBot) return;

        var content = msg.Content.ToLower();
        content = content.Replace("\u200b", "")
            .Replace("\u00ad", "");

        if (content.Contains("nix"))
        {
            var t = await DbContext.Messages.FindAsync(msg.Author.Id);
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (t is null)
            {
                Console.WriteLine($"creating new DB entity for user {msg.Author.Username} ({msg.Author.Id})");
                DbContext.Messages.Add(new DbMessage
                {
                    UserId = msg.Author.Id,
                    LastSaid = now,
                    Count = 1,
                    LastTriggerMessage = msg.Id
                });
            }
            else
            {
                if (t.LastTriggerMessage == msg.Id)
                    return;
                t.LastSaid = now;
                t.Count++;
                t.LastTriggerMessage = msg.Id;
            }

            await DbContext.SaveChangesAsync();
        }
    }
    
    private static async Task OnEdit(DiscordClient sender, MessageUpdateEventArgs args)
    {
        await NixCheck(args.Message);
    }

    private static async Task OnMessage(DiscordClient sender, MessageCreateEventArgs args)
    {
        await NixCheck(args.Message);
    }

    private static async Task OnReactionAdded(DiscordClient sender, MessageReactionAddEventArgs args)
    {
        if (args.User.Id == 190544080164487168L &&
            args.Emoji.Name == "x" &&
            args.Message.Author.Id == sender.CurrentUser.Id)
        {
            await args.Message.DeleteAsync();
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
        await DbContext.Database.MigrateAsync();

        Discord = new DiscordClient(new DiscordConfiguration
        {
            Token = token,
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.All
        });

        Discord.MessageCreated += OnMessage;
        Discord.MessageUpdated += OnEdit;
        Discord.MessageReactionAdded += OnReactionAdded;

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