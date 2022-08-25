namespace VegasSolitaire.Core.ViewModels;
[InstanceGame]
[UseLabelGrid]
public partial class VegasSolitaireMainViewModel : SolitaireMainViewModel<VegasSolitaireSaveInfo>
{
    public VegasSolitaireMainViewModel(IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        MoneyModel money
        )
        : base(aggregator, command, resolver)
    {
        _money1 = money;
        Money = money.Money;
    }
    private decimal _money = 500; //start out with 500.
    private readonly MoneyModel _money1;
    [LabelColumn]
    public decimal Money
    {
        get { return _money; }
        set
        {
            if (SetProperty(ref _money, value))
            {
                _money1.Money = value;
            }
        }
    }
    protected override SolitaireGameClass<VegasSolitaireSaveInfo> GetGame(IGamePackageResolver resolver)
    {
        VegasSolitaireMainGameClass game;
        game = resolver.ReplaceObject<VegasSolitaireMainGameClass>();
        game.AddMoney = (() => Money += 5);
        game.ResetMoney = (() => Money -= 52);
        return game;
    }
}