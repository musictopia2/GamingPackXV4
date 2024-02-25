namespace MonopolyCardGame.Blazor;
public partial class CalculatorCreaterComponent
{
    [Parameter]
    [EditorRequired]
    public BasicList<MonopolyCardGameCardInformation> YourCards { get; set; } = [];
    [Parameter]
    public EventCallback OnCancelled { get; set; }
    [Parameter]
    [EditorRequired]
    public BasicList<MonopolyCardGameCardInformation> StartCards { get; set; } = [];
    [Parameter]
    [EditorRequired]
    public CalculatorViewModel? Calculator1 { get; set; }
    private void ChoseCard(MonopolyCardGameCardInformation card)
    {
        if (card.WhatCard == EnumCardType.IsRailRoad)
        {
            Calculator1!.ChooseRailroads();
            return;
        }
        if (card.WhatCard == EnumCardType.IsUtilities)
        {
            Calculator1!.ChooseUtilities();
            return; //this will cancel automatically.
        }
        if (card.WhatCard == EnumCardType.IsProperty)
        {
            Calculator1!.ChooseProperties(card.Group);
            return;
        }
        throw new CustomBasicException("Only railroads, utilties and properties are supported");
    }
}