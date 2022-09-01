namespace Fluxx.Core.Cards;
public class ActionCard : FluxxCardInformation
{
    public ActionCard()
    {
        CardType = EnumCardType.Action;
    }
    public new EnumActionMain Deck
    {
        get
        {
            return (EnumActionMain)base.Deck;
        }
        set
        {
            base.Deck = (int)value;
        }
    }
    public EnumActionScreen Category { get; set; }
    public override string Text() //now text has to be function.
    {
        return Deck switch
        {
            EnumActionMain.Draw2AndUseEm => "Draw 2 And Use 'Em",
            EnumActionMain.Draw3Play2OfThem => "Draw 3, Play 2 Of Them",
            EnumActionMain.DiscardDraw => "Discard & Draw",
            EnumActionMain.LetsSimplify => "Let's Simplify",
            _ => Deck.ToString().TextWithSpaces(),
        };
    }
    public override void Populate(int chosen)
    {
        Deck = (EnumActionMain)chosen;
        PopulateDescription();
        Category = Deck switch
        {
            EnumActionMain.DiscardDraw or EnumActionMain.EmptyTheTrash or EnumActionMain.Jackpot or EnumActionMain.NoLimits or EnumActionMain.RulesReset or EnumActionMain.ScrambleKeepers or EnumActionMain.TakeAnotherTurn => EnumActionScreen.None,
            EnumActionMain.ExchangeKeepers or EnumActionMain.StealAKeeper or EnumActionMain.TrashAKeeper => EnumActionScreen.KeeperScreen,
            EnumActionMain.Taxation => EnumActionScreen.OtherPlayer,
            _ => EnumActionScreen.ActionScreen,
        };
    }
}