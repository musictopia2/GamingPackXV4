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
    extension (IApplicationBuilder app)
    {
        public void AddMultiplayerSignalRServices()
        {
            app.UseCors("AllowOrigin");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<MultiplayerConnectionHub>("/hubs/gamepackage/messages", options =>
                {
                    options.TransportMaxBufferSize = 72428800;
                    options.ApplicationMaxBufferSize = 72428800;
                });
            });
        }
    }   
}