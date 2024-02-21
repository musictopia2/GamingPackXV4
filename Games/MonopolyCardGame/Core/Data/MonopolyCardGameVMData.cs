namespace MonopolyCardGame.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class MonopolyCardGameVMData : IBasicCardGamesData<MonopolyCardGameCardInformation>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    public MonopolyCardGameVMData(CommandContainer command, IGamePackageResolver resolver)
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
        TempHand1 = new(command)
        {
            AutoSelect = EnumHandAutoType.SelectAsMany
        };
        TempHand1.ManualSelectUnselect = AfterSelectOne;
        AdditionalInfo1 = new();
        TempSets1 = new(command, resolver)
        {
            HowManySets = 5
        };
    }
    private void AfterSelectOne(MonopolyCardGameCardInformation payLoad)
    {
        if (payLoad.WhatCard != EnumCardType.IsRailRoad
            && payLoad.WhatCard != EnumCardType.IsUtilities
            && payLoad.WhatCard != EnumCardType.IsProperty)
        {
            AdditionalInfo1.Clear();
            return; //certain cards don't need more information for this stage.
        }
        if (payLoad.IsSelected == false)
        {
            AdditionalInfo1.Clear();
            return;
        }
        if (payLoad.Deck == AdditionalInfo1!.CurrentCard.Deck)
        {
            AdditionalInfo1.Clear();
        }
        else
        {
            AdditionalInfo1.AdditionalInfo(payLoad.Deck);
        }
    }
    public DetailCardViewModel AdditionalInfo1 { get; set; }
    public DeckObservablePile<MonopolyCardGameCardInformation> Deck1 { get; set; }
    public SingleObservablePile<MonopolyCardGameCardInformation> Pile1 { get; set; }
    public HandObservable<MonopolyCardGameCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<MonopolyCardGameCardInformation>? OtherPile { get; set; }
    public HandObservable<MonopolyCardGameCardInformation> TempHand1 { get; set; }
    public TempSets TempSets1 { get; set; }
}