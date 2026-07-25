using System;

namespace SettingsClone.ViewModels.Field;

public abstract class MessageFieldViewModel : ViewModelBase
{
    public enum TokenType
    {
        Text,
        Choice,
        Range,
        Counter
    }

    public TokenType Type { get; protected set; }
}