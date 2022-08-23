namespace BasicGameFrameworkLibrary.Core.SolitaireClasses.MiscClasses;
internal class SolitaireCloningContext : MappingCloningContext
{
    protected override void Configure(ICustomConfig config)
    {
        config.Make<SavedDiscardPile<SolitaireCard>>(x =>
        {
            x.Cloneable(false, x =>
            {

            });
        });
    }
}