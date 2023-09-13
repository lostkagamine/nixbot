using System.ComponentModel.DataAnnotations;

namespace NixBot.Entities;

public class DbBlacklistEntry
{
    [Key]
    public ulong Id { get; set; }
}