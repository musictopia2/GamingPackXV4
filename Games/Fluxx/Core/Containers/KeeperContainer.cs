﻿namespace Fluxx.Core.Containers;
[SingletonGame]
[AutoReset]
public class KeeperContainer
{
    private readonly FluxxGameContainer _gameContainer;
    private readonly FluxxDelegates _delegates;
    public BasicList<HandObservable<KeeperCard>> KeeperHandList;
    public DetailCardObservable CardDetail;
    public KeeperContainer(FluxxGameContainer gameContainer, FluxxDelegates delegates)
    {
        _gameContainer = gameContainer;
        _delegates = delegates;
        KeeperHandList = new();
        CardDetail = new DetailCardObservable();
    }
    public void Init()
    {
        LinkKeepers();
    }
    private void LinkKeepers()
    {
        _gameContainer.PlayerList!.ForEach(thisPlayer =>
        {
            HandObservable<KeeperCard> thisHand = new (_gameContainer.Command);
            thisHand.Visible = true;
            thisHand.AutoSelect = EnumHandAutoType.None;
            thisHand.HandList = thisPlayer.KeeperList;
            thisHand.Text = thisPlayer.NickName;
            thisHand.ObjectClickedAsync += ThisHand_ObjectClickedAsync;
            KeeperHandList!.Add(thisHand);
        });
    }
    private Task ThisHand_ObjectClickedAsync(KeeperCard payLoad, int index)
    {

        if (Section != EnumKeeperSection.None)
        {
            return Task.CompletedTask;
        }
        if ((int)payLoad.Deck == CardDetail.CurrentCard.Deck)
        {
            CardDetail.ResetCard();
        }
        else
        {
            CardDetail.ShowCard(payLoad);
        }
        return Task.CompletedTask;
    }
    private void ChangeKeeperClick(EnumHandAutoType select)
    {
        KeeperHandList.ForEach(x => x.AutoSelect = select);
    }
    public void LoadSavedGame()
    {
        KeeperHandList!.Clear();
        LinkKeepers();
    }
    public EnumKeeperSection Section { get; set; } = EnumKeeperSection.None;
    public void ShowSelectedKeepers(BasicList<KeeperPlayer> tempList)
    {
        tempList.ForEach(thisTemp =>
        {
            var thisKeep = KeeperHandList![thisTemp.Player - 1];
            thisKeep.SelectOneFromDeck(thisTemp.Card);
        });
        _gameContainer.Command.UpdateAll();
    }
    public void ShowKeepers()
    {
        Section = EnumKeeperSection.None;
        ChangeKeeperClick(EnumHandAutoType.None);
    }
    public async Task LoadKeeperScreenAsync()
    {
        if (_gameContainer.CurrentAction == null)
        {
            throw new CustomBasicException("Must have a current action in order to load the keeper screen.  If a player wants to see the keepers only; use ShowKeepers method.");
        }
        if (_gameContainer.CurrentAction.Deck == EnumActionMain.TrashAKeeper)
        {
            Section = EnumKeeperSection.Trash;
        }
        else if (_gameContainer.CurrentAction.Deck == EnumActionMain.StealAKeeper)
        {
            Section = EnumKeeperSection.Steal;
        }
        else if (_gameContainer.CurrentAction.Deck == EnumActionMain.ExchangeKeepers)
        {
            Section = EnumKeeperSection.Exchange;
        }
        else
        {
            throw new CustomBasicException("Can't figure out the section when loading keepers");
        }
        CardDetail!.ShowCard(_gameContainer.CurrentAction);
        ChangeKeeperClick(EnumHandAutoType.SelectOneOnly);
        if (_delegates.LoadKeeperScreenAsync == null)
        {
            throw new CustomBasicException("Nobody is loading keeper screen.  Rethink");
        }
        await _delegates.LoadKeeperScreenAsync.Invoke(this);
    }
    public HandObservable<KeeperCard> GetKeeperHand(FluxxPlayerItem thisPlayer)
    {
        foreach (var thisHand in KeeperHandList!)
        {
            if (thisHand.HandList.Equals(thisPlayer.KeeperList))
            {
                return thisHand;
            }
        }
        throw new CustomBasicException("Keeper Hand Not Found");
    }
    public string ButtonText { get; set; } = "";
}