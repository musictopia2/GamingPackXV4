namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.BasicGameClasses;
public abstract class DominosGameClass<D, P, S>(
    IGamePackageResolver mainContainer,
    IEventAggregator aggregator,
    BasicData basicData,
    TestOptions test,
    IDominoGamesData<D> currentMod,
    IMultiplayerSaveState state,
    IAsyncDelayer delay,
    CommandContainer command,
    BasicGameContainer<P, S> gameContainer,
    ISystemError error,
    IToast toast
        ) : BasicGameClass<P, S>(mainContainer, aggregator, basicData, test, currentMod, state, delay, command, gameContainer, error, toast)
    , IDrewDominoNM, IPlayDominoNM, IDominoDrawProcesses<D>
    where D : IDominoInfo, new()
    where P : class, IPlayerSingleHand<D>, new()
    where S : BasicSavedDominosClass<D, P>, new()
{
    public int DominosToPassOut { get; set; } //i think this is fine.
    protected void LoadUpDominos()
    {
        if (IsLoaded == true)
        {
            throw new CustomBasicException("Should not load the dominos if its already loaded.  Otherwise, rethink");
        }
        currentMod.PlayerHand1!.Text = "Your Dominos";
        currentMod.PlayerHand1.Visible = true;
    }
    protected void ClearBoneYard()
    {
        currentMod.BoneYard!.PopulateBoard();//i think.
    }
    protected void PassDominos()
    {
        if (currentMod.BoneYard!.RemainingList.Count == 0)
        {
            throw new CustomBasicException("Cannot have 0 dominos after shuffling");
        }
        PlayerList!.ForEach(player =>
        {
            player.MainHandList.ReplaceRange(currentMod.BoneYard.FirstDraw(DominosToPassOut));
            if (player.MainHandList.Count == 0)
            {
                throw new CustomBasicException("Cannot have 0 dominos when passing out");
            }
        });
        AfterPassedDominos();
    }
    protected void AfterPassedDominos()
    {
        SingleInfo = PlayerList.GetSelf();
        currentMod.PlayerHand1!.ReplaceHandList(SingleInfo.MainHandList); //forgot to hook up here.
        SingleInfo.MainHandList.Sort(); //i think.
        currentMod.BoneYard!.PopulateTotals();
    }
    public async Task DrawDominoAsync(D thisDomino)
    {
        if (SingleInfo!.CanSendMessage(BasicData!) == true)
        {
            await Network!.SendAllAsync("drawdomino", thisDomino.Deck);
        }
        currentMod.BoneYard!.RemoveDomino(thisDomino);
        currentMod.BoneYard.PopulateTotals();
        thisDomino.IsUnknown = false;
        thisDomino.Drew = true;
        SingleInfo!.MainHandList.Add(thisDomino);
        if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
        {
            SingleInfo.MainHandList.Sort();
        }
        await AfterDrawingDomino();
    }
    protected virtual async Task AfterDrawingDomino()
    {
        await ContinueTurnAsync();
    }
    public async Task DrewDominoReceivedAsync(int deck)
    {
        D thisDomino = currentMod.BoneYard!.RemainingList.GetSpecificItem(deck);
        await DrawDominoAsync(thisDomino);
    }
    protected void ProtectedStartTurn()
    {
        currentMod.BoneYard!.NewTurn(); //i think
        this.ShowTurn();
    }
    protected void ProtectedSaveBone()
    {
        SaveRoot!.BoneYardData = currentMod.BoneYard!.SavedData();
    }
    protected void ProtectedLoadBone()
    {
        currentMod.BoneYard!.SavedGame(SaveRoot!.BoneYardData!);
    }
    protected async Task SendPlayDominoAsync(int deck)
    {
        if (SingleInfo!.CanSendMessage(BasicData!) == false)
        {
            return;
        }
        await Network!.SendPlayDominoAsync(deck);
    }
    public abstract Task PlayDominoAsync(int deck); //every game like this requires playing domino.  if i am wrong, rethink
    async Task IPlayDominoNM.PlayDominoMessageAsync(int deck)
    {
        await PlayDominoAsync(deck);
    }
}