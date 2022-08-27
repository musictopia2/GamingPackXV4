namespace Checkers.Core.Data;
internal class CloneContext : MappingCloningContext
{
    protected override void Configure(ICustomConfig config)
    {
        config.Make<BasicList<PlayerSpace>>(x =>
        {
            x.Cloneable(false);
        })
        .Make<PlayerSpace>(x => x.Cloneable(false));
    }
}