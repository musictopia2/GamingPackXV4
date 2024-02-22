namespace MonopolyCardGame.Blazor;
public partial class MonopolyFinderComponent
{
    [Parameter]
    public EnumManuelStatus Status { get; set; }
    [Parameter]
    [EditorRequired]
    public MonopolyCardGamePlayerItem? Player { get; set; }
    [Inject]
    private IToast? Toast { get; set; }

    [Parameter]
    public EventCallback OnOrganizedCards { get; set; }

    private MonopolyCardGameVMData? _vmData;
    private ICustomCommand PutBackCommand => DataContext!.PutBackCommand!;
    private ICustomCommand ManuelCommand => DataContext!.ManuallyPlaySetsCommand!;
    protected override void OnInitialized()
    {
        _vmData = aa1.Resolver!.Resolve<MonopolyCardGameVMData>();
    }
    private void PopulateManuelCards()
    {
        Player!.PopulateManuelCards(_vmData!, false);
        DataContext!.MainGame.SortTempHand();
    }
    //private async Task FirstGoOutAsync()
    //{
    //    if (_vmData!.TempHand1.HandList.Count > 0)
    //    {
    //        Toast!.ShowUserErrorToast("You cannot initially go out because there are cards left");
    //        return;
    //    }
    //    if (HasAllValidMonopolies() == false)
    //    {
    //        Toast!.ShowUserErrorToast("You do not have all valid monopolies");
    //        return;
    //    }
    //    await Task.Delay(0);
    //    Toast!.ShowSuccessToast("You did go out part 1");
    //}
    //private DeckRegularDict<MonopolyCardGameCardInformation> WhatSet(int whichOne)
    //{
    //    return _vmData!.TempSets1!.ObjectList(whichOne);
    //}

    


    //private bool HasAllValidMonopolies()
    //{
    //    for (int x = 1; x <= _vmData!.TempSets1.HowManySets; x++)
    //    {
    //        var list = WhatSet(x);
    //        //if (list.Any(x => x.WhatCard == EnumCardType.IsChance))
    //        //{
    //        //    usedChance = true;
    //        //}
    //        if (list.Count > 0)
    //        {
    //            if (CanGoOut(list, true) == false)
    //            {
    //                return false;
    //            }
    //        }
    //    }
    //    return true;
    //}

}