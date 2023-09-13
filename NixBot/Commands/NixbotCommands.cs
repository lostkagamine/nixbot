using System.Diagnostics.CodeAnalysis;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace NixBot.Commands;

[SuppressMessage("Performance", "CA1822:Mark members as static")]
public class NixbotCommands : BaseCommandModule
{
    [Command("lastsaid")]
    public async Task LastSaid(CommandContext ctx)
    {
        var dbmsg = await Nixbot.DbContext.Messages.FindAsync(ctx.User.Id);
        
        if (dbmsg is null)
        {
            await ctx.Channel.SendMessageAsync($"You have never said 'nix'.");
        }
        else
        {
            await ctx.Channel.SendMessageAsync($"You last said 'nix' <t:{dbmsg.LastSaid}:R>. You've said it {dbmsg.Count} time(s).");
        }
    }
    
    [Command("lastsaid")]
    // ReSharper disable once UnusedMember.Local
    public async Task LastSaid(CommandContext ctx, DiscordUser target)
    {
        if (target.IsBot)
        {
            await ctx.Channel.SendMessageAsync(
                $"{target.Username} is a bot, and thus is not tracked by the Nix Tracking System 9000™. Sorry.");
            return;
        }

        if (await Blacklist.IsUserBlacklisted(ctx.User.Id))
        {
            await ctx.Channel.SendMessageAsync(
                $"{target.Username} has bothered Sylvie enough to get blacklisted from the bot due to GDPR concerns, " +
                "and thus is not tracked by the Nix Tracking System 9000™. Sorry.");
            return;
        }
        
        var dbmsg = await Nixbot.DbContext.Messages.FindAsync(target.Id);
        
        if (dbmsg is null)
        {
            await ctx.Channel.SendMessageAsync($"{target.GlobalName} has never said 'nix'.");
        }
        else
        {
            await ctx.Channel.SendMessageAsync($"{target.GlobalName} last said 'nix' <t:{dbmsg.LastSaid}:R>. "
                + $"They've said it {dbmsg.Count} time(s).");
        }
    }
}