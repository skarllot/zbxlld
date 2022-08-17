using System;

namespace zbxlld.Windows.Cli;

public readonly struct Arguments
{
    public readonly bool IsValid;
    public readonly bool PrettyOutput;
    public readonly bool IsHelp;
    public readonly bool IsVersion;
    public readonly string? Key;
    public readonly string? KeySuffix;

    public Arguments(ReadOnlySpan<string> args)
    {
        Key = null;
        KeySuffix = null;
        PrettyOutput = false;
        IsHelp = false;
        IsVersion = false;

        if (args.IsEmpty)
        {
            IsValid = false;
            return;
        }

        switch (args[0])
        {
            case "--help":
            case "-h":
                IsHelp = true;
                break;
            case "--version":
                IsVersion = true;
                break;
            default:
                Key = args[0];
                break;
        }

        var nextIsKeySuffix = false;
        foreach (string item in args[1..])
        {
            if (nextIsKeySuffix)
            {
                if (!string.Equals(item, "NULL", StringComparison.OrdinalIgnoreCase))
                    KeySuffix = item.ToUpperInvariant();
                nextIsKeySuffix = false;
            }
            else if (item == "--pretty")
                PrettyOutput = true;
            else if (item == "-s")
                nextIsKeySuffix = true;
            else
            {
                IsValid = false;
                return;
            }
        }

        IsValid = true;
    }
}