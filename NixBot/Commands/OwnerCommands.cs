using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace NixBot.Commands;

public class ScriptGlobals
{
    public CommandContext ctx = null!;
}

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

    [RequireOwner]
    [Command("eval")]
    [Description("Run arbitrary C#.")]
    public async Task EvalCommand(CommandContext ctx,
        [RemainingText]
        string code)
    {
        if (ctx.Member!.Id != 190544080164487168L)
        {
            await ctx.RespondAsync("absolutely not");
            return;
        }

        var outMsg = await ctx.Channel.SendMessageAsync("`Running script...`");
        var sw = new Stopwatch();

        var globals = new ScriptGlobals
        {
            ctx = ctx
        };

        var references = new List<Assembly>
        {
            // Ourselves
            Assembly.GetExecutingAssembly(),
            Assembly.GetAssembly(typeof(DSharpPlus.DiscordClient))!
        };

        code = code.Replace("```cs", "").Replace("```", "");

        sw.Start();

        try
        {
            var res =
                await CSharpScript.EvaluateAsync(code,
                    ScriptOptions.Default.WithImports(
                        "System",
                        "System.Math"
                    ).AddReferences(references),
                    globals);
            sw.Stop();
            var outStr = $"Completed in {sw.ElapsedMilliseconds}ms:\n```\n{res ?? "<null>"}```";
            if (outStr.Length >= 2000)
            {
                var pasteContents = $"{res ?? "<null>"}";
                var haste = new Hastebin("https://p.kagamine.tech");
                var pres = await haste.CreatePaste(pasteContents);
                await outMsg.ModifyAsync(
                    $"Completed in {sw.ElapsedMilliseconds}ms, but message too long.\n" +
                    $"Result here: {pres.URL} ({pasteContents.Length} characters)");
            } else
            {
                await outMsg.ModifyAsync(outStr);
            }
        }
        catch (Exception ex)
        {
            var outStr = $"caught exception `{ex.GetType().FullName}`:\n" +
                $"```\n{ex.GetType().FullName}: {ex.Message}\n{ex.StackTrace}```";
            if (outStr.Length >= 2000)
            {
                var pasteContents = $"{ex.GetType().FullName}: {ex.Message}\n{ex.StackTrace}";
                var haste = new Hastebin("https://p.kagamine.tech");
                var pres = await haste.CreatePaste(pasteContents);
                await outMsg.ModifyAsync(
                    $"Caught `{ex.GetType().FullName}` in {sw.ElapsedMilliseconds}ms, but message too long.\n" +
                    $"Result here: {pres.URL} ({pasteContents.Length} characters)");
            }
            else
            {
                await outMsg.ModifyAsync(outStr);
            }
        }
    }
}