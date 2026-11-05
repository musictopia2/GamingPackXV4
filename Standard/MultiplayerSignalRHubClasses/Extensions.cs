namespace MultiplayerSignalRHubClasses;
public static class Extensions
{

    //this means my idea can be to have a custom extension where i can add cors and allow larger sizes.
    //for the appbuilder, use generics plus the route name.

    extension (IServiceCollection services)
    {
        public void AddMultiplayerSignalRServices()
        {
            js1.RequireCustomSerialization = true;
            Core.AutoResumeContexts.GlobalRegistrations.Register();
            services.AddSignalR(options =>
            {
                options.MaximumReceiveMessageSize = 72428800;
            }); //don't use core. core is only bare necessities.
            services.AddCors(options =>
            {
                options.AddPolicy("AllowOrigin", builder =>
                {
                    builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            });
        }
    }
    extension (WebApplication app)
    {
        public void AddMultiplayerSignalRServices(
            string? welcomeMessage = null,
            params BasicList<string> allowedHosts
            )
        {
            app.UseRouting();
            app.UseCors("AllowOrigin");

            app.MapHub<MultiplayerConnectionHub>(
                "/hubs/gamepackage/messages",
                options =>
                {
                    options.TransportMaxBufferSize = 72428800;
                    options.ApplicationMaxBufferSize = 72428800;
                });

            if (welcomeMessage is not null)
            {
                app.MapGet("/", () => welcomeMessage);
            }
            if (allowedHosts is not null)
            {
                MultiplayerConnectionHub.AllowedHosts.AddRange(allowedHosts);
            }
        }
    }   
}