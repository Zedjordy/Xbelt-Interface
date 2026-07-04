using System;
namespace SettingsClone.Models;

public enum TokenType
{
    Barcode,
    Index,
    STX,
    ETX,
    CR,
    LF,
    Separator,
    Constant,
    Date,
    Time
}

public class MessageToken
{
    public Guid Id { get; } = Guid.NewGuid();
    public TokenType Type { get; set; }
    public string Name { get; set; } = string.Empty;
    public uint Length { get; set; }
    public string? Value { get; set; }
    public override string ToString() => Name;
}