namespace Savannah.Core.Piles;
public class SelfDiscardCP : HandObservable<RegularSimpleCard>
{
    private readonly SavannahPlayerItem _player;
    private readonly SavannahGameContainer _gameContainer;
    //all players has to do even for host.  otherwise, gets hosed for other players unfortunately.
    public SelfDiscardCP(CommandContainer command, SavannahPlayerItem player, SavannahGameContainer gameContainer) : base(command)
    {
        AutoSelect = EnumHandAutoType.SelectOneOnly;
        _gameContainer = gameContainer;
        _player = player; //not just self anymore.  however, for other players has to put as part of playeritem.  (however, will be json ignore).
        HasFrame = true;
        Text = "Discard Piles";
    }
    //this is only for self.
    public void ClearBoard()
    {
        if (_player.DiscardList.Count != 6)
        {
            throw new CustomBasicException("There must be only 6 cards for overlapping");
        }
        HandList = _player.DiscardList; //try this way.
        bool unknowns = true;
        foreach (var card in HandList)
        {
            card.IsUnknown = unknowns;
            unknowns = !unknowns;
        }
        if (HandList.Last().IsUnknown)
        {
            throw new CustomBasicException("The last card cannot be unknown");
        }
        Maximum = 6;
    }
    public void RemoveCard()
    {
        HandList.RemoveLastItem();
        if (HandList.Count <= _player.WhenToStackDiscards) //no need for plus 1 because will be reduced anyways.
        {
            _player.WhenToStackDiscards--; //i think.  hopefully this will work.  this means that for now one starts stacking sooner.
            Maximum--;
        }
        if (HandList.Count > 0)
        {
            HandList.Last().IsUnknown = false;
        }
    }
    public void Reload()
    {
        HandList = _player.DiscardList;
        Maximum = _player.WhenToStackDiscards + 1;
        //if (_player.WhenToStackDiscards == -1)
        //{
        //    Maximum = 1;
        //}
        //else
        //{
            
        //}
    }
    protected override bool CanSelectSingleObject(RegularSimpleCard thisObject)
    {
        return thisObject.Deck == HandList.Last().Deck; //you can only select the last card.
    }
    //private bool RequiresStacking => HandList.Count - 1 > _player.WhenToStackDiscards;
    protected override async Task ProcessSelectOneOnlyAsync(RegularSimpleCard payLoad)
    {
        if (payLoad.IsSelected)
        {
            payLoad.IsSelected = false;
            return;
        }
        //if (NeedsToDiscardToSelf() == false)
        //{
        //    await SelectOnlySingleCardAsync(payLoad);
        //    return;
        //}
        if (_player.MainHandList.HasSelectedObject())
        {
            var card = _player.MainHandList.GetSelectedItems().Single();
            if (card.Value == EnumRegularCardValueList.Joker)
            {
                _gameContainer.WrongDiscardProcess?.Invoke();
                return;
            }
            if (_gameContainer.DiscardAsync is null)
            {
                throw new CustomBasicException("Nobody is handling the DiscardAsync");
            }
            await _gameContainer.DiscardAsync.Invoke();
            return;
        }
        await SelectOnlySingleCardAsync(payLoad);
    }
    public async Task DiscardEmptyAsync()
    {
        if (_player.MainHandList.HasSelectedObject())
        {
            if (_gameContainer.DiscardAsync is null)
            {
                throw new CustomBasicException("Nobody is handling the DiscardAsync");
            }
            Maximum = 1; //will be one again because you are discarding from empty.
            _player.WhenToStackDiscards = 0; //i think.
            await _gameContainer.DiscardAsync.Invoke();
            return;
        }
    }
    private async Task SelectOnlySingleCardAsync(RegularSimpleCard card)
    {
        if (_gameContainer.UnselectAllPilesAsync is null)
        {
            throw new CustomBasicException("Nobody is handling the unselecting all piles");
        }
        await _gameContainer.UnselectAllPilesAsync.Invoke();
        card.IsSelected = true;
    }
    //private int MaxStacks()
    //{
    //    if (_player.ReserveList.Count == 0)
    //    {
    //        return 1;
    //    }
    //    return 3;
    //}
    //public bool NeedsToDiscardToSelf()
    //{
    //    if (RequiresStacking == false)
    //    {
    //        return true;
    //    }

    //    int maxs = _player.WhenToStackDiscards + MaxStacks();
    //    return HandList.Count <= maxs;
    //}
}