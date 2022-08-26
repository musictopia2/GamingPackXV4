using System.Timers; //not common enough.
namespace Bingo.Core.ViewModels;
[InstanceGame]
public partial class BingoMainViewModel : BasicMultiplayerMainVM, IHandle<EndGameEarlyEventModel>
{
    private readonly BingoMainGameClass _mainGame; //if we don't need, delete.
    public BingoVMData VMData { get; set; }
    private readonly BasicData _basicData;
    private readonly TestOptions _test;
    private readonly Timer _timer;
    public BingoMainViewModel(CommandContainer commandContainer,
        BingoMainGameClass mainGame,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        BingoVMData data
        )
        : base(commandContainer, mainGame, basicData, test, resolver, aggregator)
    {
        _mainGame = mainGame;
        VMData = data;
        _basicData = basicData;
        _test = test;
        _timer = new();
        _timer.Enabled = false;
        _timer.Interval = 6000;
        _timer.Elapsed += TimerElapsed;
        _mainGame.SetTimerEnabled = rets =>
        {
            _timer.Enabled = rets;
        };
        CreateCommands(commandContainer);
    }
    protected override Task TryCloseAsync()
    {
        _timer.Stop();
        _timer.Dispose();
        _timer.Elapsed -= TimerElapsed; //has to un
        return base.TryCloseAsync();
    }
    partial void CreateCommands(CommandContainer command);
    private async void TimerElapsed(object? sender, ElapsedEventArgs e)
    {
        await Execute.OnUIThreadAsync(async () =>
        {
            CommandContainer!.ManuelFinish = true;
            CommandContainer.IsExecuting = true;
            _timer.Enabled = false;
            if (_basicData.MultiPlayer == true)
            {
                if (_basicData.Client == true)
                {
                    _mainGame!.Network!.IsEnabled = true; //maybe needs to be after you checked.  or it could get hosed.
                    return; //has to wait for host.
                }
                if (_mainGame!.PlayerList.Any(Items => Items.PlayerCategory == EnumPlayerCategory.Computer))
                {
                    await _mainGame.FinishAsync();
                    return;
                }
                await _mainGame.Network!.SendAllAsync("callnextnumber");
                await _mainGame.CallNextNumberAsync();
                return;
            }
            await _mainGame!.FinishAsync();
        });
    }
    protected override async Task ActivateAsync()
    {
        await base.ActivateAsync();
        if (_basicData.MultiPlayer && _basicData.Client)
        {
            await _mainGame.CallNextNumberAsync();
        }
    }
    [Command(EnumCommandCategory.Game)]
    public async Task BingoAsync()
    {
        BingoPlayerItem selfPlayer = _mainGame!.PlayerList!.GetSelf();
        if (selfPlayer.BingoList.HasBingo == false)
        {
            string oldStatus = VMData.Status;
            VMData.Status = "No Bingos Here";
            CommandContainer.UpdateAll();
            await _mainGame!.Delay!.DelayMilli(500);
            VMData.Status = oldStatus;
            return;
        }
        if (_basicData!.MultiPlayer == true)
        {
            await _mainGame.Network!.SendAllAsync("bingo", selfPlayer.Id);
        }
        await _mainGame.GameOverAsync(selfPlayer.Id);
    }
    public bool CanSelectSpace(SpaceInfoCP space)
    {
        if (space.IsEnabled == false)
        {
            return false;
        }
        if (space.AlreadyMarked == true)
        {
            return false;
        }
        if (space.Text == "Free")
        {
            return true;
        }
        if (space.Text == _mainGame.CurrentInfo!.WhatValue.ToString())
        {
            return true;
        }
        if (_test.AllowAnyMove == true)
        {
            return true;
        }
        return false;
    }
    [Command(EnumCommandCategory.Game)]
    public void SelectSpace(SpaceInfoCP space)
    {
        space.AlreadyMarked = true;
        BingoPlayerItem selfPlayer = _mainGame.PlayerList!.GetSelf();
        var thisBingo = selfPlayer.BingoList[space.Vector.Row - 1, space.Vector.Column];
        thisBingo.DidGet = true;
    }
    void IHandle<EndGameEarlyEventModel>.Handle(EndGameEarlyEventModel message)
    {
        _timer.Stop();
        _timer.Dispose();
        //hopefully this will close automatically anyways.  trying on host first.
    }
}