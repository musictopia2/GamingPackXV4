namespace GoFish.Core.Logic;
[SingletonGame]
[AutoReset]
public class AskProcesses(GoFishGameContainer gameContainer, GoFishVMData model, IToast toast) : IAskProcesses
{
    void IAskProcesses.LoadAskList()
    {
        gameContainer.LoadAskScreenAsync?.Invoke();
        if (gameContainer!.SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
        {
            model.AskList.LoadFromHandCardValues(gameContainer.SingleInfo);
        }
        else
        {
            model.AskList.LoadEntireList();
        }
        model.CardYouAsked = EnumRegularCardValueList.None;
        model.AskList.UnselectAll();
    }
    async Task IAskProcesses.NumberToAskAsync(EnumRegularCardValueList asked)
    {
        gameContainer.SaveRoot!.NumberAsked = true;
        if (gameContainer.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
        {
            model!.AskList!.SelectSpecificItem(asked);
        }
        GoFishPlayerItem otherPlayer = GetPlayer();
        await NumberToAskAsync(asked, otherPlayer);
    }
    private async Task NumberToAskAsync(EnumRegularCardValueList whichOne, GoFishPlayerItem otherPlayer)
    {
        if (otherPlayer.MainHandList.Count == 0)
        {
            await gameContainer.ContinueTurnAsync!();
            return;
        }
        model!.AskList!.SelectSpecificItem(whichOne);
        if (gameContainer.Test!.NoAnimations == false)
        {
            await gameContainer.Delay!.DelaySeconds(1);
        }
        DeckRegularDict<RegularSimpleCard> thisList;
        if (gameContainer.Test.AllowAnyMove == false || otherPlayer.MainHandList.Count != 1)
        {
            thisList = otherPlayer.MainHandList.Where(items => items.Value == whichOne).ToRegularDeckDict();
        }
        else
        {
            thisList = otherPlayer.MainHandList.ToRegularDeckDict();
        }
        if (thisList.Count == 0)
        {
            if (model.Deck1!.IsEndOfDeck() == false)
            {
                if (gameContainer.SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                {
                    toast.ShowInfoToast("Go Fish");
                }
                gameContainer.LeftToDraw = 0;
                gameContainer.PlayerDraws = 0;
                await gameContainer.DrawAsync!.Invoke();
                return;
            }
            if (gameContainer.SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
            {
                toast.ShowInfoToast("No more cards left to draw.  Therefore; will have to end your turn");
            }
            await gameContainer.ContinueTurnAsync!.Invoke();
            return;
        }
        if (otherPlayer.PlayerCategory == EnumPlayerCategory.Self && gameContainer.Test.NoAnimations == false)
        {
            thisList.ForEach(items =>
            {
                model.PlayerHand1!.SelectOneFromDeck(items.Deck);
            });
            await gameContainer.Delay!.DelaySeconds(1); //so you can see what you have to get rid of.
        }
        thisList.ForEach(items =>
        {
            otherPlayer.MainHandList.RemoveObjectByDeck(items.Deck);
            if (gameContainer.SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
            {
                items.Drew = true;
            }
            gameContainer.SingleInfo.MainHandList.Add(items);
        });
        if (gameContainer.SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
        {
            gameContainer.SortAfterDrawing!.Invoke(); //i think.
        }
        if (otherPlayer.MainHandList.Count == 0)
        {
            int cards = model.Deck1!.CardsLeft();
            if (cards < 5 && cards > 0)
            {
                gameContainer.LeftToDraw = cards;
                gameContainer.PlayerDraws = otherPlayer.Id;
                await gameContainer.DrawAsync!.Invoke();
                return;
            }
            else if (cards > 0)
            {
                gameContainer.LeftToDraw = 5;
                gameContainer.PlayerDraws = otherPlayer.Id;
                await gameContainer.DrawAsync!.Invoke();
                return;
            }
        }
        await gameContainer.ContinueTurnAsync!.Invoke();
    }
    private GoFishPlayerItem GetPlayer()
    {
        if (gameContainer.PlayerList!.Count > 2)
        {
            throw new CustomBasicException("Since there are more than 2 players; needs to know the nick name");
        }
        int nums;
        GoFishPlayerItem tempPlayer;
        if (gameContainer.WhoTurn == 1)
        {
            nums = 2;
        }
        else
        {
            nums = 1;
        }
        gameContainer.SaveRoot!.PlayOrder.OtherTurn = nums;
        tempPlayer = gameContainer.PlayerList![nums]; //one based now
        return tempPlayer;
    }
}