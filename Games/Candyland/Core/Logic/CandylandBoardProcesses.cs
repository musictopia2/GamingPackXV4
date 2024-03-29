﻿namespace Candyland.Core.Logic;
[SingletonGame]
[AutoReset]
public class CandylandBoardProcesses
{
    public CandylandBoardProcesses(TestOptions test, IAsyncDelayer delayer, IEventAggregator aggregator)
    {
        _test = test;
        _delayer = delayer;
        _aggregator = aggregator;
    }
    private Dictionary<int, EnumCandyLandType>? _spaceList; //decided to use dictionary this time.
    private readonly TestOptions _test;
    private readonly IAsyncDelayer _delayer;
    private readonly IEventAggregator _aggregator;
    private int FindNextPosition(int oldPos, int howMany, EnumCandyLandType thisCard)
    {
        int x;
        int manys = 0;
        for (x = oldPos + 1; x <= 126; x++)
        {
            if (_spaceList![x] == thisCard)
            {
                manys += 1;
                if (manys == howMany)
                {
                    return x;
                }
            }
        }
        return 0;
    }
    private int FindSpecificPicture(EnumCandyLandType thisCard)
    {
        int x;
        for (x = 1; x <= 126; x++)
        {
            if (thisCard == _spaceList![x])
            {
                return x;
            }
        }
        throw new CustomBasicException("There is no picture card");
    }
    public void LoadBoard()
    {
        ContinueLoading();
    }
    public void NewTurn()
    {
        _aggregator.Publish(new NewTurnEventModel());
    }
    private void ContinueLoading()
    {
        _spaceList = new Dictionary<int, EnumCandyLandType>();
        EnumCandyLandType thisCard;
        int x;
        for (x = 1; x <= 126; x++)
        {
            switch (x)
            {
                case 1:
                case 7:
                case 14:
                case 21:
                case 27:
                case 34:
                case 40:
                case 46:
                case 52:
                case 59:
                case 65:
                case 72:
                case 78:
                case 84:
                case 90:
                case 97:
                case 103:
                case 109:
                case 115:
                case 121:
                    {
                        thisCard = EnumCandyLandType.IsRed;
                        break;
                    }

                case 2:
                case 8:
                case 15:
                case 22:
                case 28:
                case 35:
                case 41:
                case 47:
                case 53:
                case 60:
                case 66:
                case 73:
                case 79:
                case 85:
                case 91:
                case 98:
                case 104:
                case 110:
                case 116:
                case 122:
                    {
                        thisCard = EnumCandyLandType.IsPurple;
                        break;
                    }

                case 3:
                case 10:
                case 17:
                case 23:
                case 29:
                case 36:
                case 42:
                case 48:
                case 54:
                case 61:
                case 67:
                case 74:
                case 80:
                case 86:
                case 93:
                case 99:
                case 105:
                case 111:
                case 117:
                case 123:
                    {
                        thisCard = EnumCandyLandType.IsYellow;
                        break;
                    }

                case 4:
                case 11:
                case 18:
                case 24:
                case 30:
                case 37:
                case 43:
                case 49:
                case 55:
                case 62:
                case 68:
                case 75:
                case 81:
                case 87:
                case 94:
                case 100:
                case 106:
                case 112:
                case 118:
                case 124:
                    {
                        thisCard = EnumCandyLandType.IsBlue;
                        break;
                    }

                case 5:
                case 12:
                case 19:
                case 25:
                case 32:
                case 38:
                case 44:
                case 50:
                case 56:
                case 63:
                case 69:
                case 76:
                case 82:
                case 88:
                case 95:
                case 101:
                case 107:
                case 113:
                case 119:
                case 125:
                    {
                        thisCard = EnumCandyLandType.IsOrange;
                        break;
                    }

                case 6:
                case 13:
                case 20:
                case 26:
                case 33:
                case 39:
                case 45:
                case 51:
                case 58:
                case 64:
                case 71:
                case 77:
                case 83:
                case 89:
                case 96:
                case 102:
                case 108:
                case 114:
                case 120:
                case 126:
                    {
                        thisCard = EnumCandyLandType.IsGreen;
                        break;
                    }

                case 9:
                    {
                        thisCard = EnumCandyLandType.IsTree;
                        break;
                    }

                case 16:
                    {
                        thisCard = EnumCandyLandType.IsGuard;
                        break;
                    }

                case 31:
                    {
                        thisCard = EnumCandyLandType.IsAngel;
                        break;
                    }

                case 57:
                    {
                        thisCard = EnumCandyLandType.IsMagic;
                        break;
                    }

                case 70:
                    {
                        thisCard = EnumCandyLandType.IsGirl;
                        break;
                    }

                case 92:
                    {
                        thisCard = EnumCandyLandType.IsFairy;
                        break;
                    }

                default:
                    {
                        throw new CustomBasicException("There is nothing for " + x);
                    }
            }
            _spaceList.Add(_spaceList.Count + 1, thisCard);
        }
    }
    public static bool WillMissNextTurn(CandylandMainGameClass mainGame)
    {
        if (mainGame.SingleInfo!.SpaceNumber == 36 || mainGame.SingleInfo.SpaceNumber == 62 || mainGame.SingleInfo.SpaceNumber == 115)
        {
            return true;
        }
        return false;
    }
    public async Task MakeMoveAsync(int space, CandylandMainGameClass mainGame)
    {
        if (space < 1 || space > 127)
        {
            throw new Exception("The spaces has to be between 1 and 127");
        }
        int currents;
        currents = mainGame.SingleInfo!.SpaceNumber;
        int x;
        if (space > mainGame.SingleInfo.SpaceNumber)
        {
            var loopTo = space;
            for (x = currents + 1; x <= loopTo; x++)
            {
                mainGame.SingleInfo.SpaceNumber = x; // this will handle this as well
                _aggregator.RepaintBoard();
                if (_test.NoAnimations == false)
                {
                    await _delayer.DelayMilli(100);
                }
            }
            bool extraSpace = false;
            if (space == 4)
            {
                space = 49;
                extraSpace = true;
            }
            else if (space == 26)
            {
                space = 37;
                extraSpace = true;
            }
            if (extraSpace == true)
            {
                mainGame.SingleInfo.SpaceNumber = space;
            }
            return;
        }
        var loopTo1 = space;
        for (x = currents - 1; x >= loopTo1; x += -1)
        {
            mainGame.SingleInfo.SpaceNumber = x;
            _aggregator.RepaintBoard();
            if (_test.NoAnimations == false)
            {
                await _delayer.DelayMilli(100);
            }
        }
    }
    public static void ClearBoard(CandylandMainGameClass mainGame)
    {
        foreach (var thisPlayer in mainGame.PlayerList!)
        {
            thisPlayer.SpaceNumber = 0;
        }
    }
    public bool IsValidMove(int newPosition, EnumCandyLandType whichCard, CandylandMainGameClass mainGame, int howMany = 1)
    {
        if (_test.AllowAnyMove == true && mainGame.SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
        {
            return true; //human can make any move.  computer never can.
        }
        if (newPosition == 127)
        {
            if (whichCard == EnumCandyLandType.IsAngel || whichCard == EnumCandyLandType.IsFairy || whichCard == EnumCandyLandType.IsGirl || whichCard == EnumCandyLandType.IsGuard || whichCard == EnumCandyLandType.IsMagic || whichCard == EnumCandyLandType.IsTree)
            {
                return false;
            }
            if (FindNextPosition(mainGame.SingleInfo!.SpaceNumber, howMany, whichCard) == 0)
            {
                return true;
            }
            return false;
        }
        if (whichCard == EnumCandyLandType.IsAngel || whichCard == EnumCandyLandType.IsFairy || whichCard == EnumCandyLandType.IsGirl || whichCard == EnumCandyLandType.IsGuard || whichCard == EnumCandyLandType.IsMagic || whichCard == EnumCandyLandType.IsTree)
        {
            if (newPosition == FindSpecificPicture(whichCard))
            {
                return true;
            }
            return false;
        }
        if (FindNextPosition(mainGame.SingleInfo!.SpaceNumber, howMany, whichCard) == newPosition)
        {
            return true;
        }
        return false;
    }
}