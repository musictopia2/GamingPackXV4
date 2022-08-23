namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.BasicPlayerClasses;
public class PlayerCollection<P> : IEnumerable<P>, IAdvancedDIContainer, IPlayerCollection<P> where P : class, IPlayerItem, new()
{
    internal enum EnumDirection
    {
        Normal, Reverse
    }
    public int Count => _privateDict.Count;
    public IGamePackageResolver? MainContainer { get; set; }
    public void ForConditionalItems(Predicate<P> match, Action<P> action) //decided to do this way.
    {
        BasicList<P> privateList = _privateDict.Values.ToBasicList();
        privateList.ForConditionalItems(match, action);
    }
    public void FixNetworkedPlayers(string yourName)
    {
        BasicList<P> tempList = _privateDict.Values.ToBasicList();
        tempList.ForConditionalItems(items => items.PlayerCategory != EnumPlayerCategory.Computer, x =>
        {
            if (x.NickName == yourName)
            {
                x.PlayerCategory = EnumPlayerCategory.Self;
            }
            else
            {
                x.PlayerCategory = EnumPlayerCategory.OtherHuman;
            }
        });
    }
    private bool _canAddMore = true; //default is true.
    private IPlayOrder? _order;
    private IMissTurnClass<P>? _thisMiss; //this means even though something else implements it, i still have to define this as well
    private Dictionary<string, P> _privateDict = new();
    public PlayerCollection() { }
    public PlayerCollection(IEnumerable<P> previousList)
    {
        _privateDict = previousList.ToDictionary(Items => Items.NickName);
        _nickList = _privateDict.Values.Select(Items => Items.NickName).ToBasicList();
        if (previousList.Any())
        {
            _canAddMore = false; //because yuo already added it.
        }
    }
    public int GetTemporaryCount => _tempList.Count; //this is needed so for multiplayers you know how many temporary players there are.
    public void DisconnectEverybody()
    {
        _tempList.RemoveAllOnly(x => x.IsHost == false);
        //_tempList.Clear(); //means no other players.
    }
    public IGamePackageGeneratorDI? GeneratorContainer { get; set; }

    private readonly BasicList<P> _tempList = new();
    private BasicList<string> _nickList = new();
    private P? _dummyPlayer;
    public void AddDummy()
    {
        _dummyPlayer = new P();
        _privateDict.Add("dummy", _dummyPlayer);
    }
    public void ChangeReverse()
    {
        if (_privateDict.Count == 2)
        {
            _order!.IsReversed = false;
            return;
        }
        _order!.IsReversed = !_order.IsReversed;
    }
    public void RemoveNonHumanPlayers()
    {
        BasicList<P> tempList = _privateDict.Values.ToBasicList();
        foreach (var item in tempList)
        {
            if (item.PlayerCategory == EnumPlayerCategory.Computer)
            {
                _privateDict.Remove(item.NickName);
            }
        }
    }
    public void RemoveDummy()
    {
        if (_dummyPlayer == null)
        {
            throw new CustomBasicException("You never added dummy player.");
        }
        _privateDict.Remove("dummy");
    }
    public void AddPlayer(P thisPlayer)
    {
        if (_canAddMore == false)
        {
            throw new CustomBasicException("Cannot add more because you already started the game");
        }
        if (_tempList.Any(x => x.NickName == thisPlayer.NickName))
        {
            return; //can't add because you already have it.
        }
        _tempList.Add(thisPlayer);
    }
    public void ResetReady()
    {
        foreach (var item in _privateDict.Values)
        {
            if (item.PlayerCategory == EnumPlayerCategory.Computer)
            {
                item.IsReady = true;
            }
            else
            {
                item.IsReady = false;
            }
        }
    }
    public P this[string nickName]
    {
        get
        {
            return _privateDict[nickName];
        }
    }
    public P this[int id]
    {
        get
        {
            return PrivatePlayer(id);
        }
    }
    public BasicList<P> GetAllPlayersStartingWithSelf() //we can test this piece before moving on.
    {
        if (_canAddMore == true)
        {
            throw new CustomBasicException("You have to finish loading before you can get all players starting with self");
        }
        P thisP = GetSelf();
        int id = thisP.Id; //because i wanted to make it one based for this.
        BasicList<P> output = new();
        for (int i = 0; i < _privateDict.Count; i++)
        {
            output.Add(thisP);
            id++;
            if (id > _privateDict.Count)
            {
                id = 1;
            }
            thisP = PrivatePlayer(id);
        }
        return output;
    }
    private P PrivatePlayer(int id)
    {
        return _privateDict[_nickList[id - 1]];
    }
    public P GetOtherPlayer()
    {
        if (_order!.OtherTurn == 0)
        {
            throw new CustomBasicException("Other Turn Is 0.  Therefore there is no temporary turn currently");
        }
        Check();
        return PrivatePlayer(_order.OtherTurn);
    }
    public P GetWhoPlayer()
    {
        Check();
        return PrivatePlayer(_order!.WhoTurn);
    }
    public P GetSelf()
    {
        try
        {
            return _privateDict.Values.Where(items => items.PlayerCategory == EnumPlayerCategory.Self).Single();
        }
        catch (Exception ex)
        {
            throw new CustomBasicException($"Unable To Get Self. Most Likely, Dupliates.  Error Was {ex.Message}");
        }
    }
    public P GetOnlyOpponent()
    {
        if (_privateDict.Count != 2)
        {
            throw new CustomBasicException("Can only get opponent if there are only 2 players.  Rethink");
        }
        try
        {
            return _privateDict.Values.Where(items => items.PlayerCategory != EnumPlayerCategory.Self).Single();
        }
        catch (Exception ex)
        {
            throw new CustomBasicException($"Unable To Get Opponent. Most Likely, Dupliates.  Error Was {ex.Message}");
        }
    }
    public P GetEnabledPlayer()
    {
        if (_order!.OtherTurn == 0)
        {
            return GetWhoPlayer();
        }
        return GetOtherPlayer();
    }
    public BasicList<P> AllPlayersExceptForCurrent() //probably sort by nick name.
    {
        int exclude;
        exclude = GetExcludePlayer();
        return _privateDict.Values.Where(Items => Items.Id != exclude).OrderBy(Items => Items.NickName).ToBasicList();
    }
    private int GetExcludePlayer()
    {
        if (_order!.OtherTurn > 0)
        {
            return _order.OtherTurn;
        }
        else
        {
            return _order.WhoTurn;
        }
    }
    private void Check()
    {
        if (_canAddMore == true)
        {
            throw new CustomBasicException("Must finish loading before getting player information or lists");
        }
    }
    public BasicList<P> GetAllNonComputerPlayers(bool excludeCurrent = true)
    {
        Check();
        int pexclude;
        if (excludeCurrent == true)
        {
            pexclude = GetExcludePlayer();
        }
        else
        {
            pexclude = 0;
        }
        return _privateDict.Values.Where(x => x.Id != pexclude
        && x.PlayerCategory != EnumPlayerCategory.Computer).OrderBy(x => x.NickName).ToBasicList();
    }
    public async Task<int> CalculateWhoTurnAsync(bool useCurrentPlayer = false, bool includeOutPlayers = false)
    {
        if (_order!.IsReversed == true)
        {
            return await PrivateCalculateTurnAsync(EnumDirection.Reverse, useCurrentPlayer, includeOutPlayers);
        }
        return await PrivateCalculateTurnAsync(EnumDirection.Normal, useCurrentPlayer, includeOutPlayers);
    }
    public int SimpleCalulcateWhoTurn(bool useCurrentPlayer = false, bool includeOutPlayers = false)
    {
        if (_privateDict.Values.Any(items => items.MissNextTurn == true))
        {
            throw new CustomBasicException("If you require missing next turn, you must use the async version");
        }
        if (_order!.IsReversed == true)
        {
            throw new CustomBasicException("I think reversing requires using async version");
        }
        Check();
        int x = 0;
        int newPlayer = _order.WhoTurn;
        do
        {
            x++;
            newPlayer++;
            if (newPlayer > _privateDict.Count)
            {
                newPlayer = 1;
            }
            if (useCurrentPlayer == false && newPlayer == _order.WhoTurn)
            {
                return 0;
            }
            if (x > _privateDict.Count)
            {
                return 0;
            }
            var tempPlayer = PrivatePlayer(newPlayer);
            if (includeOutPlayers == true)
            {
                return newPlayer;
            }
            if (tempPlayer.InGame == true)
            {
                return newPlayer;
            }
        } while (true);
    }
    public async Task<int> CalculateOldTurnAsync(bool useCurrentPlayer = false, bool includeOutPlayers = false)
    {
        return await PrivateCalculateTurnAsync(EnumDirection.Reverse, useCurrentPlayer, includeOutPlayers);
    }
    public async Task<int> CalculateOtherTurnAsync(bool useCurrentPlayer = false, bool includeOutPlayers = true)
    {
        if (_order!.IsReversed == true)
        {
            throw new CustomBasicException("Not sure if we can do reversed for other turn.  Think about it then");
        }
        if (_order.OtherTurn == 0)
        {
            return await PrivateCalculateTurnAsync(EnumDirection.Normal, useCurrentPlayer, includeOutPlayers);
        }
        int newPlayer = _order.OtherTurn;
        newPlayer++;
        if (newPlayer > _privateDict.Count)
        {
            newPlayer = 1;
        }
        if (newPlayer == _order.WhoTurn && useCurrentPlayer == false)
        {
            return 0;
        }
        return newPlayer;
    }
    private async Task<int> PrivateCalculateTurnAsync(EnumDirection direction, bool useCurrentPlayer, bool includeOutPlayers)
    {
        Check();
        int newPlayer;
        newPlayer = _order!.WhoTurn;
        int x = 0;
        P tempPlayer;
        do
        {
            x++;
            if (direction == EnumDirection.Reverse)
            {
                newPlayer--;
                if (newPlayer <= 0)
                {
                    newPlayer = _privateDict.Count;
                }
            }
            else
            {
                newPlayer++;
                if (newPlayer > _privateDict.Count)
                {
                    newPlayer = 1;
                }
            }
            if (useCurrentPlayer == false && newPlayer == _order.WhoTurn)
            {
                return 0;
            }
            if (x > _privateDict.Count)
            {
                return 0;
            }
            tempPlayer = PrivatePlayer(newPlayer);
            if (tempPlayer.MissNextTurn == true)
            {
                if (_thisMiss == null)
                {
                    _thisMiss = MainContainer!.Resolve<IMissTurnClass<P>>("");
                }
                await _thisMiss.PlayerMissTurnAsync(tempPlayer); //this is a better way to do it.
                tempPlayer.MissNextTurn = false; //i think a player will never miss more than one turn.
                x = 0; //so you can be considered again.
            }
            else
            {
                if (includeOutPlayers == true)
                {
                    return newPlayer;
                }

                if (tempPlayer.InGame == true)
                {
                    return newPlayer;
                }
            }

        } while (true);
    }
    public BasicList<P> GetAllComputerPlayers(bool excludeCurrent = true)
    {
        Check();
        int pexclude;
        if (excludeCurrent == true)
        {
            pexclude = GetExcludePlayer();
        }
        else
        {
            pexclude = 0;
        }
        return _privateDict.Values.Where(Items => Items.Id != pexclude
        && Items.PlayerCategory == EnumPlayerCategory.Computer).OrderBy(Items => Items.NickName).ToBasicList();
    }
    private void LoadBulkPlayers(int howMany, EnumPlayerCategory category)
    {
        if (category == EnumPlayerCategory.OtherHuman)
        {
            throw new CustomBasicException("I don't think i want to load other human players.  If I am wrong, rethink");
        }
        for (int i = 0; i < howMany; i++)
        {
            P thisPlayer = new();
            thisPlayer.InGame = true;
            thisPlayer.IsReady = true;
            thisPlayer.PlayerCategory = category;
            if (category == EnumPlayerCategory.Self)
            {
                thisPlayer.NickName = $"Human {i + 1}";
            }
            else
            {
                thisPlayer.NickName = $"Computer {i + 1}";
            }
            AddPlayer(thisPlayer);
        }
    }
    /// <summary>
    /// This is used in cases where i started to load players and wanted to start over again
    /// </summary>
    public void ClearTempPlayers()
    {
        if (_canAddMore == false)
        {
            throw new CustomBasicException("You already loaded the players.  Therefore, can't clear");
        }
        _tempList.Clear();
    }
    internal BasicList<P> GetTemporaryList()
    {
        return _tempList.ToBasicList(); //just give them another copy.
    }
    public void LoadPlayers(int humans = 0, int computers = 0, bool reverse = false)
    {
        if (humans == 0 && computers == 0)
        {
            throw new CustomBasicException("You cannot have both 0 computer players and 0 human players");
        }
        if (reverse == false)
        {
            LoadBulkPlayers(humans, EnumPlayerCategory.Self);
            LoadBulkPlayers(computers, EnumPlayerCategory.Computer);
        }
        else
        {
            LoadBulkPlayers(computers, EnumPlayerCategory.Computer);
            LoadBulkPlayers(humans, EnumPlayerCategory.Self);
        }
    }
    public IEnumerator<P> GetEnumerator()
    {
        return _privateDict.Values.GetEnumerator();
    }
    public void TestFinish()
    {
        if (_tempList.Count == 0)
        {
            throw new CustomBasicException("Should not finish loading because there was no players");
        }
        _tempList.IncrementIntegers((ThisItem, Index) => ThisItem.Id = Index);
        _privateDict = _tempList.ToDictionary(Items => Items.NickName);
        _nickList = _privateDict.Values.Select(Items => Items.NickName).ToBasicList();
    }
    public void FinishLoading(bool needsToShufflePlayers = true) //default to true but we could decide not to shuffle (when we need more control)
    {
        if (_tempList.Count == 0)
        {
            throw new CustomBasicException("Should not finish loading because there was no players");
        }
        _canAddMore = false;
        PopulateContainer(this);
        if (needsToShufflePlayers == true)
        {
            _tempList.ShuffleList();
        }
        _tempList.IncrementIntegers((ThisItem, Index) => ThisItem.Id = Index);
        _privateDict = _tempList.ToDictionary(Items => Items.NickName);
        _nickList = _privateDict.Values.Select(Items => Items.NickName).ToBasicList();
        _order = MainContainer!.Resolve<IPlayOrder>("");
        _order.WhoTurn = 1;
        bool rets = MainContainer.RegistrationExist<IPlayerNeeds>("");
        if (rets)
        {
            IPlayerNeeds thisNeed = MainContainer.Resolve<IPlayerNeeds>("");
            LoadOtherComputerPlayers(thisNeed.PlayersNeeded - _privateDict.Count);
        }
    }
    private void LoadOtherComputerPlayers(int howMany) //this does not need to be shuffled.
    {
        if (howMany <= 0)
        {
            return; //just ignore in this case.
        }
        howMany.Times(x =>
        {
            P thisPlayer = new()
            {
                Id = _privateDict.Count + 1,
                NickName = "Computeridle" + x,
                PlayerCategory = EnumPlayerCategory.Computer
            };
            _privateDict.Add(thisPlayer.NickName, thisPlayer);
            _nickList.Add(thisPlayer.NickName); //i think this may have been missing too.
        });
    }
    public void AutoSaved(IPlayOrder thisOrder) //if anything else is needed, will be here.
    {
        _order = thisOrder;
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return _privateDict.Values.GetEnumerator();
    }
    public void PerformActionOnConditional(Predicate<P> predicate, Action<P> action)
    {
        foreach (var item in _privateDict.Values)
        {
            if (predicate(item) == true)
            {
                action.Invoke(item);
            }
        }
    }
    public async Task ForEachAsync(ActionAsync<P> action) //i think done.
    {
        foreach (P ThisItem in _privateDict.Values)
        {
            await action.Invoke(ThisItem);
        }
    }
    public void ForEach(Action<P> action)
    {
        foreach (var item in _privateDict.Values)
        {
            action.Invoke(item);
        }
    }
}