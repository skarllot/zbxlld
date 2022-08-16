using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace zbxlld.Windows;

public class CommandApp
{
    private readonly ILogger<CommandApp> _logger;
    private readonly Dictionary<string, CommandHandler> _commands;

    public CommandApp(ILogger<CommandApp> logger, IEnumerable<ICommandProvider> providers)
    {
        _logger = logger;
        _commands = providers
            .SelectMany(static provider => provider.GetCommands())
            .ToDictionary(static it => it.Key, StringComparer.OrdinalIgnoreCase);
    }

    public int Run(string[] args)
    {
        string? key = null;
        string? keySuffix = null;
        bool prettyOutput = false;

        foreach (string item in args)
        {
            if (key is null)
                key = item;
            else if (item == "--pretty")
                prettyOutput = true;
            else if (!string.Equals(item, "NULL", StringComparison.OrdinalIgnoreCase))
                keySuffix = item.ToUpperInvariant();
        }

        if (string.IsNullOrWhiteSpace(key))
        {
            Console.WriteLine("At least one parameter should be provided.");
            return (int)ErrorId.ParameterNone;
        }

        if (!_commands.TryGetValue(key, out var handler))
        {
            _logger.LogError("Invalid argument: {ArgValue}", key);

            Console.WriteLine("Invalid argument");
            Console.WriteLine("Available keys are:");
            foreach (string item in _commands.Keys)
                Console.WriteLine(" {0}", item);

            return (int)ErrorId.ParameterInvalid;
        }

        var options = new JsonWriterOptions();
        if (prettyOutput) options.Indented = true;
        var writer = new Utf8JsonWriter(Console.OpenStandardOutput(), options);
        writer.WriteStartObject();
        writer.WritePropertyName("data");
        handler.Handle(writer, keySuffix);
        writer.WriteEndObject();
        writer.Flush();

        return (int)ErrorId.NoError;
    }
}