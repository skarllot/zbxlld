using Microsoft.Extensions.Logging;
using zbxlld.Windows;
using zbxlld.Windows.DependencyInjection;

var container = new ZbxlldContainer(LogLevel.Information);
var app = container.GetService<CommandApp>();

return app.Run(args);