namespace YaBlewIt.Core.Logic;
[SingletonGame]
[AutoReset]
public class ComputerAI
{
    private readonly YaBlewItGameContainer _gameContainer;
    private readonly YaBlewItVMData _model;
    public ComputerAI(YaBlewItGameContainer gameContainer, YaBlewItVMData model)
    {
        _gameContainer = gameContainer;
        _model = model;
    }
    public bool TakeFirstClaim()
    {
        var jumps = _gameContainer.SingleInfo!.MainHandList.Where(x => x.CardCategory == EnumCardCategory.Jumper).ToBasicList();
        if (jumps.Any() == false)
        {
            return false; //because you have none.
        }
        var last = _model.Claims.HandList.Last();
        if (last.CardColor == _gameContainer.SingleInfo.CursedGem)
        {
            return false;
        }
        if (last.SecondNumber > 0)
        {
            return true;
        }
        bool rets = _gameContainer.Random.NextBool(40);
        return rets;
    }
    private int ChanceOfPassing(BasicList<YaBlewItCardInformation> gems, YaBlewItPlayerItem player)
    {
        int wilds = gems.Count(x => x.CardColor == EnumColors.Wild);
        int curses = gems.Count(x => x.CardColor == player.CursedGem);
        BasicList<YaBlewItPlayerItem> gamePlayers = _gameContainer.PlayerList!.Where(x => x.InGame == true).ToBasicList();
        gamePlayers.RemoveSpecificItem(player);
        YaBlewItPlayerItem? onlyOther = null;
        int otherCurses = 0;
        if (gamePlayers.Count == 0)
        {
            onlyOther = _gameContainer.PlayerList!.GetWhoPlayer();
            otherCurses = gems.Count(x => x.CardColor == onlyOther.CursedGem);
        }
        if (curses == gems.Count)
        {
            return 100;
        }
        if (otherCurses == gems.Count)
        {
            return 100;
        }
        if (gems.Count == 1)
        {
            if (wilds == 1)
            {
                if (onlyOther is not null)
                {
                    return 50; //50 percent chance the only player left will not even get the wild.
                }
                return 70; //70 percent chance it will pass
            }
            if (onlyOther is not null)
            {
                return 80;
            }
            return 90;
        }
        if (gems.Count == 2)
        {
            if (wilds == 2)
            {
                if (otherCurses == 1)
                {
                    return 80;
                }
                if (onlyOther is not null)
                {
                    return 70;
                }
                return 60;   
            }
            if (wilds == 1)
            {
                if (onlyOther is null)
                {
                    return 70;
                }
                return 70;
            }
            if (curses == 1)
            {
                return 90;
            }
            if (onlyOther is null)
            {
                return 80;
            }
            return 85;
        }
        if (gems.Count == 3)
        {
            if (wilds == 3)
            {
                return 40;
            }
            if (wilds == 2)
            {
                return 50;
            }
            if (curses == 2)
            {
                return 80;
            }
            if (curses == 1)
            {
                return 70;
            }
            if (wilds == 1)
            {
                return 60;
            }
            return 70;
        }
        BasicList<int> possibleList = _model.Claims.HandList.GetContainedNumbers();
        if (possibleList.Count == 8)
        {
            return 0; //because its a guarantee.
        }
        if (possibleList.Count == 4)
        {
            //means 50 percent chance you can win.
            if (onlyOther is not null && gems.Count >= 6)
            {
                return 20;
            }
            if (gems.Count >= 6)
            {
                return 10;
            }
            if (onlyOther is not null)
            {
                //means you have a chance they will not get it.
                return 30;
            }
            return 40;
        }
        if (possibleList.Count == 7)
        {
            //almost guarantee.
            if (onlyOther is not null && gems.Count >= 6)
            {
                return 5;
            }
            if (onlyOther is not null)
            {
                return 10;
            }
            return 20;
        }
        if (possibleList.Count == 6)
        {
            if (onlyOther is not null && gems.Count >= 6)
            {
                return 10;
            }
            if (onlyOther is not null)
            {
                return 15;
            }
            return 25;
        }
        if (possibleList.Count == 5)
        {
            if (onlyOther is not null && gems.Count >= 6)
            {
                return 15;
            }
            if (onlyOther is not null)
            {
                return 20;
            }
            return 30;
        }
        if (gems.Count > 6)
        {
            return 20;
        }
        return 70;
    }
    private int ChanceOfTakingClaim(BasicList<YaBlewItCardInformation> gems, YaBlewItPlayerItem player)
    {
        int wilds = gems.Count(x => x.CardColor == EnumColors.Wild);
        int curses = gems.Count(x => x.CardColor == player.CursedGem);
        BasicList<YaBlewItPlayerItem> gamePlayers = _gameContainer.PlayerList!.Where(x => x.InGame == true).ToBasicList();
        YaBlewItPlayerItem? onlyOther = null;
        int otherCurses = 0;
        if (gamePlayers.Count == 1)
        {
            onlyOther = gamePlayers.Single();
            otherCurses = gems.Count(x => x.CardColor == onlyOther.CursedGem);
        }
        if (curses == gems.Count)
        {
            return 0;
        }
        if (otherCurses == gems.Count)
        {
            return 0;
        }
        if (gems.Count == 1)
        {
            var gem = gems.Single();

            if (wilds == 1)
            {
                return 40;
            }
            if (gem.SecondNumber > 0)
            {
                return 80;
            }
            return 30;
        }
        if (gems.Count == 2)
        {
            if (wilds == 2)
            {
                return 70;
            }
            if (wilds == 1)
            {
                return 60;
            }
            if (curses == 1)
            {
                return 30;
            }
            return 40;
        }
        if (wilds > 2)
        {
            return 100; //i really want it if there are 2 wilds (guaranteed at this point).
        }
        if (gems.Count == 3)
        {
            
            if (wilds == 2)
            {
                return 90;
            }
            if (curses == 2)
            {
                return 20;
            }
            if (curses == 1)
            {
                return 40;
            }
            if (wilds == 1)
            {
                return 80;
            }
            return 60;
        }
        return 95; //if it reaches this point, will most likely take it.
    }
    public int SafeCardPlayed()
    {
        if (SafeColorListClass.GetColorChoices().Count == 0)
        {
            return 0; //because none is even needed
        }
        YaBlewItCardInformation? card = _gameContainer.SingleInfo!.MainHandList.FirstOrDefault(x => x.CardCategory == EnumCardCategory.Safe);
        if (card is null)
        {
            return 0;
        }
        return card.Deck;
    }
    public EnumColors SafeColorChosen()
    {
        var list = SafeColorListClass.GetColorChoices();
        int maxCount = 0;
        EnumColors chosenSoFar = list.First();
        foreach (var color in list)
        {
            int count = _gameContainer.SingleInfo!.MainHandList.Count(x => x.CardColor == color);
            if (count > maxCount)
            {
                chosenSoFar = color;
                maxCount = count;
            }
        }
        return chosenSoFar;
    }
    public int FaultyCardPlayed()
    {
        //0 means no card.
        YaBlewItCardInformation? card = _gameContainer.SingleInfo!.MainHandList.FirstOrDefault(x => x.CardCategory == EnumCardCategory.Faulty);
        if (card is null)
        {
            return 0;
        }
        return card.Deck;
    }
    public bool TakeClaim()
    {
        //true means it will take the claim.  does not make gambling decision at this point.
        var gems = _model.Claims.HandList.Where(x => x.CardCategory == EnumCardCategory.Gem).ToBasicList();
        if (gems.Count == 0)
        {
            throw new CustomBasicException("Cannot even consider whether to take claim because should had at least one gem");
        }
        int percents;
        if (gems.Count == 1)
        {
            if (gems.Single().CardColor == _gameContainer.SingleInfo!.CursedGem)
            {
                return false; //don't take it for sure because you will lose the points
            }
        }
        percents = ChanceOfTakingClaim(gems, _gameContainer.SingleInfo!);
        if (percents == 100)
        {
            return true; //this means it will take the claim
        }
        if (percents == 0)
        {
            return false;
        }
        bool output = _gameContainer.Random.NextBool(percents);
        return output;
    }
    public bool DoPass()
    {
        //this means the computer will decide whether to play or pass.
        //there will always be a chance the computer will not pass.
        var gems = _model.Claims.HandList.Where(x => x.CardCategory == EnumCardCategory.Gem).ToBasicList();
        if (gems.Count == 0)
        {
            throw new CustomBasicException("Cannot even consider whether to pass because should had at least one gem");
        }
        int percents;
        if (gems.Count == 1)
        {
            if (gems.Single().CardColor == _gameContainer.SingleInfo!.CursedGem)
            {
                return true;
            }
        }
        percents = ChanceOfPassing(gems, _gameContainer.SingleInfo!);
        if (percents == 100)
        {
            return true; //in this case its, a guarantee its going to pass
        }
        if (percents == 0)
        {
            return false;
        }
        bool output =  _gameContainer.Random.NextBool(percents);
        return output;
    }
}
