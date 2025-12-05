namespace Payday.Core.Logic;
public static class Extensions
{
    extension (IDeckDict<CardInformation> list)
    {
        public DeckRegularDict<C> GetMailOrDealList<C>(EnumCardCategory whichType) where C : CardInformation, new()
        {
            var firstList = list.Where(items => items.CardCategory == whichType);
            DeckRegularDict<C> output = [];
            foreach (var thisItem in firstList)
            {
                output.Add((C)thisItem);
            }
            return output;
        }
    }
    extension (PaydayGameContainer gameContainer)
    {
        public async Task StartProcessPopUpAsync(PaydayVMData model)
        {
            if (gameContainer.SingleInfo!.CanSendMessage(gameContainer.BasicData))
            {
                await gameContainer.Network!.SendAllAsync("popupchosen", model.PopUpChosen);
            }
            model.PopUpList.ShowOnlyOneSelectedItem(model.PopUpChosen);
            gameContainer.Command.UpdateAll();
            if (gameContainer.Test.NoAnimations == false)
            {
                await gameContainer.Delay.DelaySeconds(1.5);
            }
        }
        internal void PopulateDeals(PaydayVMData model)
        {
            var tempList = gameContainer.SingleInfo!.Hand.GetMailOrDealList<DealCard>(EnumCardCategory.Deal);
            model!.CurrentDealList!.HandList.ReplaceRange(tempList);
        }
        internal void MonthLabel(PaydayVMData model)
        {
            model!.MonthLabel = $"Payday, month {gameContainer.SingleInfo!.CurrentMonth} of {gameContainer.SaveRoot!.MaxMonths}";
        }
        internal void ProcessExpense(GameBoardProcesses gameBoard, decimal amount)
        {
            gameBoard!.AddToJackPot(amount);
            gameContainer.SingleInfo!.ReduceFromPlayer(amount);
        }
        internal bool ShouldReshuffle
        {
            get
            {
                gameContainer.SingleInfo = gameContainer.PlayerList!.GetWhoPlayer();
                if (gameContainer.BasicData!.MultiPlayer == false)
                {
                    return true;
                }
                if (gameContainer.SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
                {
                    return true;
                }
                if (gameContainer.SingleInfo.PlayerCategory == EnumPlayerCategory.OtherHuman)
                {
                    return false;
                }
                if (gameContainer.BasicData.Client == false)
                {
                    return true;
                }
                return false;
            }   
        }
    }
    extension (PaydayPlayerItem player)
    {
        internal void ReduceFromPlayer(decimal amount)
        {
            player.MoneyHas -= amount;
            if (player.MoneyHas < 0)
            {
                player.Loans += Math.Abs(player.MoneyHas);
                player.MoneyHas = 0;
            }
            if (player.MoneyHas < 0 || player.Loans < 0)
            {
                throw new CustomBasicException("The money and loans must be 0 or greater");
            }
        }
    }
}