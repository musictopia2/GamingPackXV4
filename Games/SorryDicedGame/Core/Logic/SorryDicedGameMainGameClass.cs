using System.Drawing;
using System.Security.Cryptography;

namespace SorryDicedGame.Core.Logic;
[SingletonGame]
public class SorryDicedGameMainGameClass
    : SimpleBoardGameClass<SorryDicedGamePlayerItem, SorryDicedGameSaveInfo, EnumColorChoice, int>
    , IBeginningColors<EnumColorChoice, SorryDicedGamePlayerItem, SorryDicedGameSaveInfo>
    , IMiscDataNM, ISerializable
{
#pragma warning disable IDE0290 // Use primary constructor
    public SorryDicedGameMainGameClass(IGamePackageResolver resolver,
#pragma warning restore IDE0290 // Use primary constructor
        IEventAggregator aggregator,
        BasicData basic,
        TestOptions test,
        SorryDicedGameVMData model,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        CommandContainer command,
        SorryDicedGameGameContainer gameContainer,
        ISystemError error,
        IToast toast,
        SorryCompleteDiceSet completeDice
        ) : base(resolver, aggregator, basic, test, model, state, delay, command, gameContainer, error, toast)
    {
        //_model = model;
        _completeDice = completeDice;
    }
    //private readonly SorryDicedGameVMData? _model;
    private readonly SorryCompleteDiceSet _completeDice;
    public override Task FinishGetSavedAsync()
    {
        LoadControls();
        SorryDicedGameGameContainer.IsGameOver = false;
        SorryDicedGameGameContainer.CanStart = PlayerList.DidChooseColors();
        BoardGameSaved(); //i think.
        //anything else needed is here.
        return Task.CompletedTask;
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
    public override async Task SetUpGameAsync(bool isBeginning)
    {
        LoadControls();
        SorryDicedGameGameContainer.IsGameOver = false;
        if (FinishUpAsync == null)
        {
            throw new CustomBasicException("The loader never set the finish up code.  Rethink");
        }
        SorryDicedGameGameContainer.CanStart = false;
        SaveRoot!.ImmediatelyStartTurn = true; //most of the time, needs to immediately start turn.  if i am wrong, rethink.
        await FinishUpAsync(isBeginning);
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        EnumColorChoice color;
        WaitingModel wait;
        switch (status) //can't do switch because we don't know what the cases are ahead of time.
        {
            //put in cases here.
            case "dicelist":
                var dice = await _completeDice.GetDiceList(content);
                await ShowRollingAsync(dice);
                return;
            case "start":
                color = await js1.DeserializeObjectAsync<EnumColorChoice>(content);
                await StartPieceAsync(color);
                return;
            case "slide":
                wait = await js1.DeserializeObjectAsync<WaitingModel>(content);
                await SlideAsync(wait);
                return;
            case "sorry":
                int id = int.Parse(content);
                var player = PlayerList[id];
                await SorryAsync(player);
                return;
            case "wait":
                wait = await js1.DeserializeObjectAsync<WaitingModel>(content);
                await WaitAsync(wait);
                return;
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    public override async Task StartNewTurnAsync()
    {
        if (PlayerList.DidChooseColors())
        {
            PrepStartTurn();
            SaveRoot.DiceList.Clear();
        }

        await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
    }
    private bool IsGameOver()
    {
        int count = SaveRoot.BoardList.Count(x => x.PlayerOwned == WhoTurn && x.At == EnumBoardCategory.Home && x.Color == SingleInfo!.Color);
        if (count == 4)
        {
            return true;
        }
        return false;
    }
    public override async Task ContinueTurnAsync()
    {
        GetPlayerToContinueTurn();
        if (PlayerList.DidChooseColors())
        {
            //can do extra things upon continue turn.  many board games require other things.
            if (IsGameOver())
            {
                SorryDicedGameGameContainer.IsGameOver = true;
                await ShowWinAsync();
                return;
            }
        }
        await base.ContinueTurnAsync();
    }
    public override async Task MakeMoveAsync(int space)
    {
        //well see what we need for the move.
        await Task.CompletedTask;
    }
    public override async Task EndTurnAsync()
    {
        WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
        //if anything else is needed, do here.
        if (PlayerList.DidChooseColors())
        {
            //can do extra things upon ending turn.  many board games require other things. only do if the player actually chose colors.

        }
        await StartNewTurnAsync();
    }
    public override async Task AfterChoosingColorsAsync()
    {
        //anything else that is needed after they finished choosing colors.

        if (MiscDelegates.FillRestColors == null)
        {
            throw new CustomBasicException("Nobody is handling filling the rest of the colors.  Rethink");
        }
        MiscDelegates.FillRestColors.Invoke();
        PlayerList.ForConditionalItems(x => x.PlayerCategory != EnumPlayerCategory.Computer, x =>
        {
            x.SlideColor = x.Color;
        });
        SorryDicedGameGameContainer.CanStart = true;
        SaveRoot.BoardList.Clear();
        foreach (var item in PlayerList)
        {
            4.Times(x =>
            {
                BoardModel board = new()
                {
                    PlayerOwned = item.Id,
                    At = EnumBoardCategory.Start,
                    Color = item.Color
                };
                SaveRoot.BoardList.Add(board);
            });
        }
        await EndTurnAsync();
    }
    public async Task RollAsync()
    {
        var firsts = _completeDice.RollDice();
        if (BasicData.MultiPlayer)
        {
            await _completeDice.SendMessageAsync("dicelist", firsts);
        }
        await ShowRollingAsync(firsts);
    }
    private bool HasAnyProperMove(SorryDiceModel dice)
    {
        if (dice.Category == EnumDiceCategory.Slide)
        {
            return true; //slides are always valid.
        }
        if (dice.Category == EnumDiceCategory.Wild)
        {
            return true;
        }
        bool rets;
        if (dice.Category == EnumDiceCategory.Sorry)
        {
            rets = SaveRoot.BoardList.Any(x => x.At == EnumBoardCategory.Home && x.PlayerOwned != WhoTurn);
            return rets; //hopefully this simple.
        }
        int count = SaveRoot.BoardList.Count(x => x.PlayerOwned == WhoTurn && x.At != EnumBoardCategory.Start && x.Color == dice.Color);
        if (count == 4)
        {
            return false;
        }
        return true;
    }
    private void FigureOutAnyProperMovesFromDice()
    {
        foreach (var item in SaveRoot.DiceList)
        {
            item.IsEnabled = HasAnyProperMove(item);
        }
    }
    private async Task ShowRollingAsync(BasicList<BasicList<SorryDiceModel>> thisList)
    {
        await _completeDice.ShowRollingAsync(thisList);
        FigureOutAnyProperMovesFromDice();
        await ContinueTurnAsync();
    }
    public async Task WaitAsync(WaitingModel wait)
    {
        var item = SaveRoot.BoardList.First(x => x.PlayerOwned == wait.Player!.Id && x.Color == wait.ColorUsed);
        //this will decide what to do.
        await MovePieceAsync(item, wait.ColorUsed);
    }
    public async Task StartPieceAsync(EnumColorChoice color)
    {
        //choose a color from the board.
        BoardModel item = SaveRoot.BoardList.First(x => x.Color == color && x.At == EnumBoardCategory.Start);
        //this will decide what to do.
        await MovePieceAsync(item, color);
        //if there is none, will give error.
    }
    private async Task MovePieceAsync(BoardModel item, EnumColorChoice color)
    {
        item.PlayerOwned = WhoTurn;
        GetPlayerToContinueTurn();
        if (color == SingleInfo!.SlideColor)
        {
            item.At = EnumBoardCategory.Home;
        }
        else
        {
            item.At = EnumBoardCategory.Waiting;
        }
        //eventually disable dice (but not yet)
        await ContinueTurnAsync();
    }

    public async Task SlideAsync(WaitingModel wait)
    {
        SaveRoot.BoardList.ForConditionalItems(x => x.PlayerOwned == wait.Player!.Id && x.At == EnumBoardCategory.Home, board =>
        {
            board.At = EnumBoardCategory.Waiting; //you are now waiting
        });

        wait.Player!.SlideColor = wait.ColorUsed;
        SaveRoot.BoardList.ForConditionalItems(x => x.PlayerOwned == wait.Player.Id && x.At == EnumBoardCategory.Waiting && x.Color == wait.ColorUsed, board =>
        {
            board.At = EnumBoardCategory.Home;
        });
        await ContinueTurnAsync();
    }
    public async Task SorryAsync(SorryDicedGamePlayerItem player)
    {
        var piece = SaveRoot.BoardList.First(x => x.PlayerOwned == player.Id && x.At == EnumBoardCategory.Home);
        piece.PlayerOwned = WhoTurn;
        GetPlayerToContinueTurn();
        if (SingleInfo!.SlideColor == piece.Color)
        {
            piece.At = EnumBoardCategory.Home;
        }
        else
        {
            piece.At = EnumBoardCategory.Waiting;
        }
        await ContinueTurnAsync();
    }
}