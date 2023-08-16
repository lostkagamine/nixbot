using System.Diagnostics.CodeAnalysis;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace NixBot.Commands;

[SuppressMessage("Performance", "CA1822:Mark members as static")]
public class OwnerCommands : BaseCommandModule
{
    [RequireOwner]
    [Command("stop")]
    [Description("Stops the bot.")]
    public async Task StopCommand(CommandContext ctx)
    {
        await ctx.Channel.SendMessageAsync("Goodbye.");
        await ctx.Client.UpdateStatusAsync(new DiscordActivity(), UserStatus.Invisible);
        
        Environment.Exit(0);
    }
}