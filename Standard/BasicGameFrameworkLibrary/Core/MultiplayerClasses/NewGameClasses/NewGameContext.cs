namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.NewGameClasses;
public class NewGameContext : SerializeContext
{
    protected override void Configure(ISerializeConfig config)
    {
        config.Make<RawGameClient>().Make<RawGameHost>();
    }
}