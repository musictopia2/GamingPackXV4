namespace Risk.Core.Data;
internal class MainContext : SerializeContext
{
    protected override void Configure(ISerializeConfig config)
    {
        config.Make<EnumColorChoice>()
             .Make<Dictionary<string, string>>();
    }
}