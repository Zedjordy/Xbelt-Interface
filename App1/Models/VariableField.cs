using System;

namespace SettingsClone.Models;

public class VariableField : MessageField
{
    public string VariableName { get; set; } = "";

    public override string Preview => $"{{{VariableName}}}";

    public override byte[] Serialize()
    {
        return Array.Empty<byte>();
    }
}