namespace LifeBoardGame.Core.Logic;
[SingletonGame]
[AutoReset]
public class BasicSalaryProcesses(LifeBoardGameVMData model, LifeBoardGameGameContainer gameContainer) : IBasicSalaryProcesses
{
    public async Task ChoseSalaryAsync(int salary)
    {
        if (gameContainer.CanSendMessage())
        {
            await gameContainer.Network!.SendAllAsync("chosesalary", salary);
        }
        if (gameContainer.GameStatus == EnumWhatStatus.NeedTradeSalary)
        {
            throw new CustomBasicException("I think if the salary is being traded; must use TradedSalary method instead");
        }
        else if (gameContainer.GameStatus == EnumWhatStatus.NeedChooseSalary)
        {
            var thisSalary = CardsModule.GetSalaryCard(salary);
            await gameContainer.ShowCardAsync(thisSalary);
            gameContainer.SingleInfo!.Hand.Add(thisSalary);
            gameContainer.SingleInfo.Salary = thisSalary.PayCheck;
            PopulatePlayerProcesses.FillInfo(gameContainer.SingleInfo);
        }
        if (gameContainer.SaveRoot.EndAfterSalary)
        {
            gameContainer.GameStatus = EnumWhatStatus.NeedToEndTurn;
        }
        else
        {
            gameContainer.GameStatus = EnumWhatStatus.NeedToSpin;
        }
        await gameContainer.ContinueTurnAsync!.Invoke();
    }
    public Task LoadSalaryListAsync()
    {
        model.HandList.Text = "List Of Salaries";
        model.Instructions = "Choose a salary";
        var firstList = gameContainer.Random.GenerateRandomList(27, 9, 19);
        var tempList = firstList.GetSalaryList(gameContainer.PlayerList!);
        var careerList = gameContainer.SingleInfo!.GetCareerList();
        if (careerList.Count == 0)
        {
            throw new CustomBasicException("Must have a career in order to load the salary list");
        }
        CareerInfo career;
        if (careerList.Count == 1)
        {
            career = careerList.Single();
        }
        else if (careerList.First().Career == EnumCareerType.Teacher)
        {
            career = careerList.Last();
        }
        else
        {
            career = careerList.First();
        }
        tempList.KeepConditionalItems(items => items.WhatGroup == career.Scale1 || items.WhatGroup == career.Scale2);
        model.HandList.HandList.Clear();
        model.HandList.HandList.ReplaceRange(tempList);
        model.HandList.AutoSelect = EnumHandAutoType.SelectOneOnly;
        return Task.CompletedTask;
    }
}