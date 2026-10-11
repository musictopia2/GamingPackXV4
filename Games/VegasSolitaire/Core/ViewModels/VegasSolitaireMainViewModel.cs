namespace VegasSolitaire.Core.ViewModels;
[InstanceGame]
[UseLabelGrid]
public partial class VegasSolitaireMainViewModel(IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    MoneyModel money
        ) : SolitaireMainViewModel<VegasSolitaireSaveInfo>(aggregator, command, resolver)
{
    [LabelColumn]
    public partial decimal Money { get; set; } = money.Money;
    partial void OnMoneyChanged(decimal value)
    {
        money.Money = value;
    }
    protected override SolitaireGameClass<VegasSolitaireSaveInfo> GetGame(IGamePackageResolver resolver)
    {
        VegasSolitaireMainGameClass game;
        game = resolver.ReplaceObject<VegasSolitaireMainGameClass>();
        game.AddMoney = () => Money += 5;
        game.ResetMoney = () => Money -= 52;
        return game;
    }
}