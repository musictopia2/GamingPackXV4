namespace MonopolyCardGame.Blazor;
public partial class MonopolyFinderComponent
{
    [Parameter]
    public EnumManuelStatus Status { get; set; }
    [Inject]
    private IToast? Toast { get; set; }

    [Parameter]
    public EventCallback OnOrganizedCards { get; set; }

    private MonopolyCardGameVMData? _vmData;
    private ICustomCommand PutBackCommand => DataContext!.PutBackCommand!;
    protected override void OnInitialized()
    {
        _vmData = aa1.Resolver!.Resolve<MonopolyCardGameVMData>();
    }
    private async Task FirstGoOutAsync()
    {
        if (_vmData!.TempHand1.HandList.Count > 0)
        {
            Toast!.ShowUserErrorToast("You cannot initially go out because there are cards left");
            return;
        }
        if (HasAllValidMonopolies() == false)
        {
            Toast!.ShowUserErrorToast("You do not have all valid monopolies");
            return;
        }
        await Task.Delay(0);
        Toast!.ShowSuccessToast("You did go out part 1");
    }
    private DeckRegularDict<MonopolyCardGameCardInformation> WhatSet(int whichOne)
    {
        return _vmData!.TempSets1!.ObjectList(whichOne);
    }

    private static bool CanGoOut(IDeckDict<MonopolyCardGameCardInformation> whatGroup, bool onlyOne = false)
    {
        //SingleInfo = PlayerList!.GetWhoPlayer();
        var tempCol = whatGroup.ToRegularDeckDict();
        var groupList = tempCol.GroupBy(items => items.Group).ToBasicList();
        bool hasRailRoad = whatGroup.Any(items => items.WhatCard == EnumCardType.IsRailRoad);
        bool hasUtilities = whatGroup.Any(items => items.WhatCard == EnumCardType.IsUtilities);
        int numWilds = whatGroup.Count(items => items.WhatCard == EnumCardType.IsChance);
        if (hasRailRoad && hasUtilities && onlyOne)
        {
            return false; //this means you cannot have both railroads and utilties.
        }
        if (numWilds < 2 && hasRailRoad == false && hasUtilities == false && groupList.Count == 0)
        {
            return false; //cannot go out because do not have any properties, no railroads, no utilities, and not enough wilds
        }
        var temps = tempCol.Where(items => (int)items.WhatCard > 3 && (int)items.WhatCard < 7).ToRegularDeckDict();
        tempCol.RemoveGivenList(temps);
        BasicList<int> setList = [];
        DeckRegularDict<MonopolyCardGameCardInformation> monCol;
        foreach (var thisGroup in groupList)
        {
            if (thisGroup.Key > 0)
            {
                monCol = MonopolyCardGameMainGameClass.MonopolyCol(tempCol, thisGroup.Key, EnumCardType.IsProperty);
                if (monCol.Count == 0)
                {
                    return false;
                }
                setList.Add(thisGroup.Key);
            }
        }
        if (hasRailRoad && setList.Count > 0 && onlyOne)
        {
            return false;
        }
        if (hasUtilities && setList.Count > 0 && onlyOne)
        {
            return false;
        }
        if (setList.Count > 1 && onlyOne)
        {
            return false;
        }
        if (hasRailRoad)
        {
            monCol = MonopolyCardGameMainGameClass.MonopolyCol(tempCol, 0, EnumCardType.IsRailRoad);
        }
        if (hasUtilities)
        {
            monCol = MonopolyCardGameMainGameClass.MonopolyCol(tempCol, 0, EnumCardType.IsUtilities);
        }
        setList.ForEach(thisSet =>
        {
            MonopolyCardGameMainGameClass.HouseCollection(tempCol); //to filter further
        });
        tempCol.RemoveAllOnly(items => items.WhatCard == EnumCardType.IsChance);
        return tempCol.Count == 0;
    }


    private bool HasAllValidMonopolies()
    {
        for (int x = 1; x <= _vmData!.TempSets1.HowManySets; x++)
        {
            var list = WhatSet(x);
            //if (list.Any(x => x.WhatCard == EnumCardType.IsChance))
            //{
            //    usedChance = true;
            //}
            if (list.Count > 0)
            {
                if (CanGoOut(list, true) == false)
                {
                    return false;
                }
            }
        }
        return true;
    }

}