namespace SequenceDice.Core.Data;
internal class MainContext : SerializeContext
{
    protected override void Configure(ISerializeConfig config)
    {
        config.Make<BasicList<BasicList<SimpleDice>>>()
            .Make<EnumColorChoice>()
            .Make<SpaceInfoCP>();
    }
}