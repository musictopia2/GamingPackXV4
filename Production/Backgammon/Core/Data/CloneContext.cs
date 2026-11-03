namespace Backgammon.Core.Data;
internal class CloneContext : MappingCloningContext
{
    protected override void Configure(ICustomConfig config)
    {
        config.Make<BackgammonPlayerDetails>(x =>
        {
            x.Cloneable(false);
        });
    }
}