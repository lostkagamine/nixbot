using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace NixBot.Commands;

public class BlacklistCommands : BaseCommandModule
{
    [RequireOwner]
    [Command("blacklist-add")]
    [Description("Adds someone to the blacklist and stops tracking for them.")]
    public async Task BlacklistAddCommand(CommandContext ctx, DiscordUser target)
    {
        var (ok, deleted) = await Blacklist.AddToBlacklist(target.Id);
        if (ok)
        {
            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":thumbsup:"));
            if (deleted)
            {
                await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":wastebasket:"));
            }
        }
        else
        {
            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":x:"));
        }
    }

    [RequireOwner]
    [Command("blacklist-remove")]
    [Description("Removes someone from the blacklist and starts tracking for them.")]
    public async Task BlacklistRemoveCommand(CommandContext ctx, DiscordUser target)
    {
        var ok = await Blacklist.RemoveFromBlacklist(target.Id);
        if (ok)
        {
            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":thumbsup:"));
        }
        else
        {
            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":x:"));
        }
    }
}