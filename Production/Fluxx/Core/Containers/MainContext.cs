namespace Fluxx.Core.Containers;
internal class MainContext : SerializeContext
{
    protected override void Configure(ISerializeConfig config)
    {
        config.Make<BasicList<string>>()
            .Make<EnumDirection>() //try this.
            .Make<BasicList<PointF>>(); //if anything else, put here,
    }
}