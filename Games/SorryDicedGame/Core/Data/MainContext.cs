namespace SorryDicedGame.Core.Data;
internal class MainContext : SerializeContext
{
    protected override void Configure(ISerializeConfig config)
    {
        config.Make<EnumColorChoice>();
    }
}