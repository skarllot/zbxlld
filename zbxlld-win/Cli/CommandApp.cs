using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace zbxlld.Windows.Cli;

public partial class CommandApp
{
    private readonly ILogger<CommandApp> _logger;
    private readonly HelpCommand _helpCommand;
    private readonly VersionCommand _versionCommand;
    private readonly Dictionary<string, CommandHandler> _commands;

    public CommandApp(
        ILogger<CommandApp> logger,
        IEnumerable<ICommandProvider> providers,
        HelpCommand helpCommand,
        VersionCommand versionCommand)
    {
        _logger = logger;
        _helpCommand = helpCommand;
        _versionCommand = versionCommand;
        _commands = providers
            .SelectMany(static provider => provider.GetCommands())
            .ToDictionary(static it => it.Key, StringComparer.OrdinalIgnoreCase);
    }

    public int Run(string[] args)
    {
        var arguments = new Arguments(args);
        if (!arguments.IsValid)
        {
            _helpCommand.ShowError();
            return ErrorId.ParameterInvalid;
        }

        if (arguments.IsHelp)
        {
            _helpCommand.ShowHelp();
            return ErrorId.NoError;
        }

        if (arguments.IsVersion)
        {
            _versionCommand.ShowVersion();
            return ErrorId.NoError;
        }

        if (string.IsNullOrWhiteSpace(arguments.Key))
        {
            Console.WriteLine("At least one parameter should be provided.");
            return ErrorId.ParameterNone;
        }

        if (!_commands.TryGetValue(arguments.Key, out var handler))
        {
            LogInvalidArg(arguments.Key);

            Console.WriteLine("Invalid argument");
            Console.WriteLine("Available keys are:");
            foreach (var item in _commands.Keys)
                Console.WriteLine(" {0}", item);

            return ErrorId.ParameterInvalid;
        }

        var options = new JsonWriterOptions();
        if (arguments.PrettyOutput) options.Indented = true;
        var writer = new Utf8JsonWriter(Console.OpenStandardOutput(), options);
        writer.WriteStartObject();
        writer.WritePropertyName("data");
        handler.Handle(writer, arguments.KeySuffix);
        writer.WriteEndObject();
        writer.Flush();

        return ErrorId.NoError;
    }

    [LoggerMessage(LogLevel.Error, "Invalid argument: {ArgValue}")]
    private partial void LogInvalidArg(string argValue);
}
