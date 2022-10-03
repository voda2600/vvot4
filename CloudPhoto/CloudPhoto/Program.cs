using CloudPhoto;
using CloudPhoto.Services;

if (args[0] == "init")
{
    var initHandler = new InitCommand();
    await initHandler.Handle();
    return;
}

var applicationName = "cloudphoto";
var builder = ConsoleApp.CreateBuilder(args, options => { options.ApplicationName = applicationName; });
builder.ConfigureServices(Startup.ConfigureServices);
var app = builder.Build();

app.AddCommands<InitCommand>();
app.AddCommands<UploadCommand>();
app.AddCommands<DownloadCommand>();
app.AddCommands<ListCommand>();
app.AddCommands<DeleteCommand>();

app.Run();
