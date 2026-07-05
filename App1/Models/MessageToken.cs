using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace SettingsClone.Models;

public enum TokenType
{
    Barcode,
    Index,
    STX,
    ETX,
    Separator,
    Constant,
    Date,
    Time
}

public class MessageToken
{
    public Guid Id { get; set; }
    public TokenType Type { get; set; }
    public string Name { get; set; } = string.Empty;

    private uint _length = 0;
    public uint Length
    {
        get => _length;
        set => _length = value;
    }
    public string? Value { get; set; } = string.Empty;
    public override string ToString() => Name;
    public TokenType ColorToken {  get; set; }
}