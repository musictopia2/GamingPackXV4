namespace DealCardGame.Core.Logic;
public class PropertySetHand(CommandContainer command) : HandObservable<DealCardGameCardInformation>(command)
{
    public Func<PropertySetHand, Task>? SetClickedAsync { get; set; }
    public bool DidClickObject { get; set; } = false; //sometimes this is needed for mobile.
    private static bool CanChooseCard(DealCardGameCardInformation card)
    {
        if (card.ActionCategory == EnumActionCategory.House || card.ActionCategory == EnumActionCategory.Hotel)
        {
            return true; //you can move them around if you want.
        }
        if (card.FirstColorChoice == EnumColor.None)
        {
            return false;
        }
        return true;
    }
    protected override Task ProcessObjectClickedAsync(DealCardGameCardInformation card, int index)
    {
        if (CanChooseCard(card) == false)
        {
            return Task.CompletedTask;
        }
        DidClickObject = true; //this is needed too.  so if other gets raised, will be ignored because already handled.
        return base.ProcessObjectClickedAsync(card, index); //go ahead and let the other process do its thing.
    }
    protected override async Task PrivateBoardSingleClickedAsync()
    {
        if (SetClickedAsync == null)
        {
            return;
        }
        await SetClickedAsync.Invoke(this);
    }
}