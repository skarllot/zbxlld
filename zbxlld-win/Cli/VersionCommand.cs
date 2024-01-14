namespace zbxlld.Windows.Cli;

public class VersionCommand
{
    public void ShowVersion()
    {
        Console.WriteLine($"{ThisAssembly.Info.Title} version {ThisAssembly.Info.InformationalVersion}");
    }
}
