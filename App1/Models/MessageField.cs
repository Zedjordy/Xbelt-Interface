using System;

namespace SettingsClone.Models;

public abstract class MessageField
{
    public Guid Id { get; } = Guid.NewGuid();

    public string DisplayName { get; set; } = "";

    public abstract string Preview { get; }

    public abstract byte[] Serialize();
}