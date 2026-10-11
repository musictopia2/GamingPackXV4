namespace LifeBoardGame.Core.Logic;
[SingletonGame]
[AutoReset]
public class CareerProcesses(LifeBoardGameVMData model, LifeBoardGameGameContainer gameContainer) : ICareerProcesses
{
    private void RemoveCareer(int career)
    {
        var careerList = gameContainer.SingleInfo!.GetCareerList();
        careerList.ForEach(thisCard =>
        {
            if (thisCard.Deck != career)
            {
                gameContainer.SingleInfo!.Hand.RemoveObjectByDeck(thisCard.Deck);
            }
        });
    }
    private static bool PrivateCanGetSalary(string career1, string career2)
    {
        if (career1 != "Teacher")
        {
            return true;
        }
        return career2 == "";
    }
    public async Task ChoseCareerAsync(int deck)
    {
        if (gameContainer.CanSendMessage())
        {
            await gameContainer.Network!.SendAllAsync("chosecareer", deck);
        }
        if (gameContainer.WasNight)
        {
            RemoveCareer(deck);
        }
        var thisCareer = CardsModule.GetCareerCard(deck);
        gameContainer.SingleInfo!.Hand.Add(thisCareer);
        await gameContainer.ShowCardAsync(thisCareer);
        string career1 = PopulatePlayerProcesses.CareerChosen(gameContainer.SingleInfo, out string career2);
        if (career1 != "Teacher" && gameContainer.SaveRoot.WasNight == false)
        {
            RemoveCareer(deck);
        }
        PopulatePlayerProcesses.FillInfo(gameContainer.SingleInfo);
        if (PrivateCanGetSalary(career1, career2))
        {
            gameContainer.SaveRoot.EndAfterSalary = gameContainer.GameStatus == EnumWhatStatus.NeedNewCareer || gameContainer.SaveRoot.WasNight;
            var thisSalary = gameContainer.SingleInfo.GetSalaryCard();
            gameContainer.SingleInfo.Hand.RemoveSpecificItem(thisSalary);
            gameContainer.GameStatus = EnumWhatStatus.NeedChooseSalary;
            await gameContainer.ContinueTurnAsync!.Invoke();
            return;
        }
        if (gameContainer.SaveRoot.WasNight)
        {
            gameContainer.SaveRoot.EndAfterSalary = true;
        }
        else if (gameContainer.TeacherChooseSecondCareer)
        {
            gameContainer.GameStatus = EnumWhatStatus.NeedNewCareer;
            gameContainer.SaveRoot.MaxChosen = 1;
        }
        else
        {
            gameContainer.GameStatus = EnumWhatStatus.NeedToSpin;
        }
        gameContainer.RepaintBoard();
        await gameContainer.ContinueTurnAsync!.Invoke();
    }
    private bool ShowAllCareers => gameContainer.SingleInfo!.OptionChosen == EnumStart.College || gameContainer.WasNight == true;
    public void LoadCareerList() //if it does not load the career list, then rethink
    {
        model.HandList.Text = "Career List";
        var firstList = gameContainer.Random.GenerateRandomList(9);
        bool canShowAll = ShowAllCareers;
        DeckRegularDict<CareerInfo> tempList = [];
        firstList.ForEach(thisItem =>
        {
            tempList.Add(CardsModule.GetCareerCard(thisItem));
        });
        if (canShowAll == false)
        {
            tempList.KeepConditionalItems(items => items.DegreeRequired == false);
        }
        var finList = tempList.GetLoadedCards(gameContainer!.PlayerList!);
        model.HandList.HandList.Clear(); //try this way now (?)
        model.HandList.HandList.ReplaceRange(finList); //hopefully smart enough on whether its unknown or not (?)
        gameContainer!.MaxChosen = 1;
        if (gameContainer.WasNight)
        {
            model.Instructions = "Pick 2 cards at random for your new career for night school.";
            gameContainer.MaxChosen = 2;
        }
        else if (gameContainer.SingleInfo!.OptionChosen == EnumStart.College && gameContainer.GameStatus == EnumWhatStatus.NeedChooseFirstCareer)
        {
            model.Instructions = "Pick 3 cards at random for your first career since you went to college.";
            gameContainer.MaxChosen = 3;
        }
        else if (gameContainer.GameStatus == EnumWhatStatus.NeedChooseFirstCareer)
        {
            model.Instructions = "Pick one card to choose your career.";
        }
        else
        {
            model.Instructions = "Pick one card for your new career.";
        }
        if (gameContainer.MaxChosen == 1)
        {
            model.HandList.AutoSelect = EnumHandAutoType.SelectOneOnly;
        }
        else
        {
            model.HandList.AutoSelect = EnumHandAutoType.SelectAsMany;
        }
    }
}