namespace ClueCardGame.Core.Logic;
[SingletonGame]
public class ClueCardGameMainGameClass
    : CardGameClass<ClueCardGameCardInformation, ClueCardGamePlayerItem, ClueCardGameSaveInfo>
    , IMiscDataNM, ISerializable
{
    private readonly ClueCardGameVMData _model;
    private readonly CommandContainer _command; //most of the time, needs this.  if not needed, take out.
    private readonly ClueCardGameGameContainer _gameContainer; //if we don't need it, take it out.

    public ClueCardGameMainGameClass(IGamePackageResolver mainContainer,
        IEventAggregator aggregator,
        BasicData basicData,
        TestOptions test,
        ClueCardGameVMData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        ICardInfo<ClueCardGameCardInformation> cardInfo,
        CommandContainer command,
        ClueCardGameGameContainer gameContainer,
        ISystemError error,
        IToast toast
        ) : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer, error, toast)
    {
        _model = currentMod;
        _command = command;
        _gameContainer = gameContainer;
    }
    public static BasicList<int> ExcludeList { get; set; } = [];
    public override Task FinishGetSavedAsync()
    {
        LoadControls();
        //anything else needed is here.
        return base.FinishGetSavedAsync();
    }
    private void LoadControls()
    {
        if (IsLoaded == true)
        {
            return;
        }
        IsLoaded = true; //i think needs to be here.
    }
    protected override async Task ComputerTurnAsync()
    {
        //if there is nothing, then just won't do anything.
        await Task.CompletedTask;
    }
    protected override Task StartSetUpAsync(bool isBeginning)
    {
        LoadControls();

        //at this point, all cards has been used.
        ExcludeList.Clear();
        var list = _gameContainer.DeckList.Where(x => x.WhatType == EnumCardType.IsWeapon).ToBasicList();
        var item = list.GetRandomItem();
        SaveRoot.Solution = new();
        SaveRoot.Solution.WeaponName = item.Name;
        ExcludeList.Add(item.Deck);
        list = _gameContainer.DeckList.Where(x => x.WhatType == EnumCardType.IsCharacter).ToBasicList();
        item = list.GetRandomItem();
        ExcludeList.Add(item.Deck);
        SaveRoot.Solution.CharacterName = item.Name;
        list = _gameContainer.DeckList.Where(x => x.WhatType == EnumCardType.IsRoom).ToBasicList();
        item = list.GetRandomItem();
        ExcludeList.Add(item.Deck);
        SaveRoot.Solution.RoomName = item.Name;


        return base.StartSetUpAsync(isBeginning);
    }

    Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status) //can't do switch because we don't know what the cases are ahead of time.
        {
            //put in cases here.

            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    public override async Task StartNewTurnAsync()
    {
        await base.StartNewTurnAsync();

        await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
    }
    public override async Task EndTurnAsync()
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        SingleInfo.MainHandList.UnhighlightObjects(); //i think this is best.

        //anything else is here.  varies by game.


        _command.ManuelFinish = true; //because it could be somebody else's turn.
        WhoTurn = await PlayerList.CalculateWhoTurnAsync();
        await StartNewTurnAsync();
    }
}