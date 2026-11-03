namespace LifeBoardGame.Core.Logic;
[SingletonGame]
[AutoReset]
public class HouseProcesses(LifeBoardGameVMData model, LifeBoardGameGameContainer gameContainer) : IHouseProcesses
{
    public async Task ChoseHouseAsync(int deck)
    {
        if (gameContainer.CanSendMessage())
        {
            await gameContainer.Network!.SendAllAsync("chosehouse", deck);
        }
        HouseInfo thisHouse = CardsModule.GetHouseCard(deck);
        await gameContainer.ShowCardAsync(thisHouse);
        gameContainer.SingleInfo!.Hand.Add(thisHouse);
        gameContainer.SaveRoot!.HouseList.Clear();
        PopulatePlayerProcesses.FillInfo(gameContainer.SingleInfo); //i think here too.
        gameContainer.TakeOutExpense(thisHouse.HousePrice);
        gameContainer.GameStatus = EnumWhatStatus.NeedToSpin;
        await gameContainer.ContinueTurnAsync!.Invoke();
    }
    public void LoadHouseList()
    {
        model.HandList!.Text = "House List";
        if (gameContainer!.SaveRoot!.HouseList.Count != 2)
        {
            throw new CustomBasicException("The house list must have 2 items");
        }
        model.HandList.HandList.Clear(); //try this way now (?)
        model.HandList.HandList.ReplaceRange(gameContainer.SaveRoot.HouseList);
        model.Instructions = "Choose a house to buy or spin to not purchase either house";
        model.HandList.AutoSelect = EnumHandAutoType.SelectOneOnly;
    }
    public async Task ShowYourHouseAsync()
    {
        if (gameContainer.CardVisible == null)
        {
            throw new CustomBasicException("Nobody is handling card visible.  Rethink");
        }
        var card = gameContainer.SingleInfo!.Hand.Single(x => x.CardCategory == EnumCardCategory.House);
        card.IsUnknown = false;
        model.SinglePile.AddCard(card);
        if (gameContainer.CardVisible.Invoke() == true)
        {
            return;
        }
        await gameContainer.ShowHouseAsync();
    }
}
