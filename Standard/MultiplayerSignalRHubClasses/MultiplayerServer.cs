namespace MultiplayerSignalRHubClasses;
public static class MultiplayerServer
{
    public static async Task RunAsync(
        string[] args,
        string? welcomeMessage = null,
        params BasicList<string> allowedHosts)
    {
        //this means if someone wants it the most easy possible this is now its done.   if doing via RunAsync, you get multiplayer and welcome and that is it.
        //eventually may allow other options to decide if someone is allowed to host (but that is future).
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddMultiplayerSignalRServices();
        await using var app = builder.Build();
        app.AddMultiplayerSignalRServices(welcomeMessage, allowedHosts);
        await app.RunAsync();
    }
}