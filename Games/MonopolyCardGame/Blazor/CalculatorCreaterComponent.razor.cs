namespace MonopolyCardGame.Blazor;
public partial class CalculatorCreaterComponent
{
    [Parameter]
    [EditorRequired]
    public BasicList<MonopolyCardGameCardInformation> YourCards { get; set; } = [];
    [Parameter]
    public EventCallback OnCompleted { get; set; }
    [Parameter]
    [EditorRequired]
    public BasicList<MonopolyCardGameCardInformation> StartCards { get; set; } = [];
    [Parameter]
    [EditorRequired]
    public CalculatorViewModel? Calculator1 { get; set; }
    private BasicList<MonopolyCardGameCardInformation> _properties = [];
    private MonopolyCardGameCardInformation? _railroad;
    private void ChoseCard(MonopolyCardGameCardInformation card)
    {
        if (card.WhatCard == EnumCardType.IsRailRoad)
        {
            _railroad = Calculator1!.ChooseRailroads();
            return;
        }
        if (card.WhatCard == EnumCardType.IsUtilities)
        {
            Calculator1!.ChooseUtilities();
            return; //this will cancel automatically.
        }
        if (card.WhatCard == EnumCardType.IsProperty)
        {
            _properties = Calculator1!.ChooseProperties(card.Group);
            return;
        }
        throw new CustomBasicException("Only railroads, utilties and properties are supported");
    }
    private void ChoseRailroad(int howMany)
    {
        Calculator1!.EnterRailroads(howMany);
        OnCompleted.InvokeAsync();
    }
    private void Cancel()
    {
        Calculator1!.Cancel();
    }
    private void ChoseProperty(MonopolyCardGameCardInformation card)
    {
        Calculator1!.EnterPropertyFromCard(card);
    }
    private string TextDivSize => "7vh";
}