namespace MahJongSolitaire.Core.Data;
internal class CloningContext : MappingCloningContext
{
    protected override void Configure(ICustomConfig config)
    {
        config.Make<BasicList<BoardInfo>>(x =>
        {
            x.Cloneable(false, x =>
            {

            });
        });
    }
}