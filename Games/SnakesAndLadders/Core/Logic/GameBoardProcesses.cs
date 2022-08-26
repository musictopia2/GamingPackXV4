﻿namespace SnakesAndLadders.Core.Logic;
[SingletonGame]
[AutoReset]
public class GameBoardProcesses
{
    private int _currentSpace;
    private readonly Dictionary<int, SpaceInfo> _spaceList;
    private readonly IAsyncDelayer _delay;
    private readonly IEventAggregator _aggregator;
    private readonly TestOptions _test;
    private readonly SnakesAndLaddersVMData _model;
    internal Func<SnakesAndLaddersPlayerItem>? CurrentPlayer { get; set; }
    public GameBoardProcesses(IAsyncDelayer delay,
        EventAggregator aggregator,
        TestOptions test,
        SnakesAndLaddersVMData model
        )
    {
        _delay = delay;
        _aggregator = aggregator;
        _test = test;
        _model = model;
        _spaceList = new ();
        int x;
        for (x = 1; x <= 100; x++)
        {
            SpaceInfo thisSpace = new();
            thisSpace.JumpTo = x;
            if (x == 4)
            {
                thisSpace.JumpTo = 39;
            }
            else if (x == 30)
            {
                thisSpace.JumpTo = 12;
            }
            else if (x == 33)
            {
                thisSpace.JumpTo = 52;
            }
            else if (x == 36)
            {
                thisSpace.JumpTo = 8;
            }
            else if (x == 59)
            {
                thisSpace.JumpTo = 63;
            }
            else if (x == 70)
            {
                thisSpace.JumpTo = 50;
            }
            else if (x == 26)
            {
                thisSpace.JumpTo = 75;
            }
            else if (x == 73)
            {
                thisSpace.JumpTo = 93;
            }
            else if (x == 86)
            {
                thisSpace.JumpTo = 57;
            }
            else if (thisSpace.JumpTo == 99)
            {
                thisSpace.JumpTo = 42;
            }
            _spaceList.Add(x, thisSpace);
        }
    }
    public async Task MakeMoveAsync(int space)
    {
        SpaceInfo thisSpace;
        var loopTo = space;
        int x;
        if (CurrentPlayer == null)
        {
            throw new CustomBasicException("No current player was set.  Rethink");
        }
        for (x = _currentSpace + 1; x <= loopTo; x++)
        {
            CurrentPlayer.Invoke().SpaceNumber = x;
            _aggregator.RepaintBoard();
            if (_test.NoAnimations == false)
            {
                await _delay.DelaySeconds(.1); //looks like commenting out several lines don't work anymore.
            }
        }
        _currentSpace = space;
        thisSpace = _spaceList[space];
        if (thisSpace.JumpTo != space)
        {
            CurrentPlayer.Invoke().SpaceNumber = thisSpace.JumpTo;
        }
        _aggregator.RepaintBoard();
    }
    public bool IsValidMove(int space)
    {
        int Values;
        if (CurrentPlayer == null)
        {
            throw new CustomBasicException("No player function was set for isvalidmove.  Rethink");
        }
        if (_test.AllowAnyMove == true && CurrentPlayer.Invoke().PlayerCategory == EnumPlayerCategory.Self)
        {
            return true;
        }
        Values = _model.Cup!.ValueOfOnlyDice;
        if ((_currentSpace + Values) == space)
        {
            return true;
        }
        return false;
    }
    public bool HasValidMove()
    {
        if (CurrentPlayer == null)
        {
            throw new CustomBasicException("No current player was set for hasvalidmove.  Rethink");
        }
        if (_test.AllowAnyMove == true && CurrentPlayer.Invoke().PlayerCategory == EnumPlayerCategory.Self)
        {
            return true;
        }
        if ((_currentSpace + _model.Cup!.ValueOfOnlyDice) > 100)
        {
            return false;
        }
        return true;
    }
    public bool IsGameOver()
    {
        if (_currentSpace == 100)
        {
            return true;
        }
        return false;
    }
    public void NewTurn(SnakesAndLaddersMainGameClass mainGame)
    {
        mainGame.SingleInfo = mainGame.PlayerList!.GetWhoPlayer();
        _currentSpace = mainGame.SingleInfo.SpaceNumber;
        _aggregator.Publish(new NewTurnEventModel());
    }
    public void UpdateSpaceFromAutoResume(SnakesAndLaddersPlayerItem player)
    {
        _currentSpace = player.SpaceNumber;
    }
}