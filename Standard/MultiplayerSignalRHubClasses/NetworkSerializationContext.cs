namespace MultiplayerSignalRHubClasses;
public class NetworkSerializationContext : SerializeContext
{
    protected override void Configure(ISerializeConfig config)
    {
        config.Make<HostCatchUpResult>();
    }
}