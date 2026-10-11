using System.Timers; //not common enough.
namespace Bingo.Core.ViewModels;
[InstanceGame]
[EventAggregatorFromMemberClass(nameof(Aggregator))]
public partial class BingoMainViewModel(CommandContainer commandContainer,
        BingoMainGameClass mainGame,
        BasicData basicData,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        TestOptions test,
        BingoVMData data) : BasicMultiplayerMainVM(commandContainer, mainGame, basicData ,resolver, aggregator), IHandle<EndGameEarlyEventModel>
{
    public BingoVMData VMData { get; set; } = data;
    private Timer _timer = null!;
    private partial void Subscribe();
    private partial void Unsubscribe();
    protected override void AdditionalInitialize()
    {
        Subscribe();
        _timer = new();
        _timer.Enabled = false;
        _timer.Interval = 6000;
        _timer.Elapsed += TimerElapsed;
        mainGame.SetTimerEnabled = rets =>
        {
            _timer.Enabled = rets;
        };
        CreateCommands(CommandContainer);
        base.AdditionalInitialize();
    }
    protected override Task CloseAsync()
    {
        Unsubscribe();
        _timer.Stop();
        _timer.Dispose();
        _timer.Elapsed -= TimerElapsed; //has to un
        return base.CloseAsync();
    }
    partial void CreateCommands(CommandContainer command);
    private async void TimerElapsed(object? sender, ElapsedEventArgs e)
    {
        await Execute.OnUIThreadAsync(async () =>
        {
            CommandContainer!.ManuelFinish = true;
            CommandContainer.IsExecuting = true;
            _timer.Enabled = false;
            if (BasicData.MultiPlayer == true)
            {
                if (BasicData.Client == true)
                {
                    mainGame!.Network!.IsEnabled = true; //maybe needs to be after you checked.  or it could get hosed.
                    return; //has to wait for host.
                }
                if (mainGame!.PlayerList.Any(Items => Items.PlayerCategory == EnumPlayerCategory.Computer))
                {
                    await mainGame.FinishAsync();
                    return;
                }
                await mainGame.Network!.SendAllAsync("callnextnumber");
                await mainGame.CallNextNumberAsync();
                return;
            }
            await mainGame!.FinishAsync();
        });
    }
    protected override async Task OpenAsync()
    {
        if (BasicData.MultiPlayer && BasicData.Client)
        {
            await mainGame.CallNextNumberAsync();
        }
    }
    [Command(EnumCommandCategory.Game)]
    public async Task BingoAsync()
    {
        BingoPlayerItem selfPlayer = mainGame!.PlayerList!.GetSelf();
        if (selfPlayer.BingoList.HasBingo == false)
        {
            string oldStatus = VMData.Status;
            VMData.Status = "No Bingos Here";
            await mainGame!.Delay!.DelayMilli(500);
            VMData.Status = oldStatus;
            return;
        }
        if (BasicData!.MultiPlayer == true)
        {
            await mainGame.Network!.SendAllAsync("bingo", selfPlayer.Id);
        }
        await mainGame.GameOverAsync(selfPlayer.Id);
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
        if (space.Text == mainGame.CurrentInfo!.WhatValue.ToString())
        {
            return true;
        }
        if (test.AllowAnyMove == true)
        {
            return true;
        }
        return false;
    }
    [Command(EnumCommandCategory.Game)]
    public void SelectSpace(SpaceInfoCP space)
    {
        space.AlreadyMarked = true;
        BingoPlayerItem selfPlayer = mainGame.PlayerList!.GetSelf();
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