using Microsoft.EntityFrameworkCore;
using NixBot.Entities;

namespace NixBot;

public class Blacklist
{
    public static async Task<bool> IsUserBlacklisted(ulong id)
        => await Nixbot.DbContext.Blacklist.FindAsync(id) != null;

    public static async Task<(bool, bool)> AddToBlacklist(ulong id)
    {
        var exists = await Nixbot.DbContext.Blacklist.FindAsync(id);
        if (exists != null)
        {
            return (false, false);
        }
        
        Console.WriteLine($"Blacklisting user {id}...");
        await Nixbot.DbContext.Blacklist.AddAsync(new DbBlacklistEntry
        {
            Id = id
        });

        var deleted = false;
        var data = await Nixbot.DbContext.Messages.FindAsync(id);
        if (data != null)
        {
            Nixbot.DbContext.Messages.Remove(data);
            Console.WriteLine("Deleted user's data!");
            deleted = true;
        }

        await Nixbot.DbContext.SaveChangesAsync();
        Console.WriteLine("Done!");
        return (true, deleted);
    }

    public static async Task<bool> RemoveFromBlacklist(ulong id)
    {
        var bl = await Nixbot.DbContext.Blacklist.FindAsync(id);
        if (bl == null) return false;
        Nixbot.DbContext.Blacklist.Remove(bl);
        await Nixbot.DbContext.SaveChangesAsync();
        return true;
    }
}