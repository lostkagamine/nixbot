using System.ComponentModel.DataAnnotations;

namespace NixBot.Entities;

public class DbMessage
{
    [Key]
    public ulong UserId { get; set; }
    public long LastSaid { get; set; }
    
    public int Count { get; set; }
}