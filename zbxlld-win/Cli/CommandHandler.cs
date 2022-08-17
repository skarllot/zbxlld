using System;
using System.Text.Json;

namespace zbxlld.Windows.Cli;

public readonly struct CommandHandler
{
    public readonly string Key;
    public readonly string Description;
    private readonly Action<Utf8JsonWriter, string?> _handle;

    public CommandHandler(string key, string description, Action<Utf8JsonWriter, string?> handle)
    {
        Description = description;
        _handle = handle;
        Key = key;
    }

    public void Handle(Utf8JsonWriter writer, string? keySuffix)
    {
        _handle(writer, keySuffix);
    }
}