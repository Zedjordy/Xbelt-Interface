using System;

namespace SettingsClone.ViewModels.Field;

public abstract class MessageFieldViewModel : ViewModelBase
{
    private event EventHandler<int>? update_value;
    public enum TokenType
    {
        Text,
        Choice,
        Range,
        Counter
    }

    public TokenType Type { get; protected set; }
}