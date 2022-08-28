﻿namespace LifeBoardGame.Core.Logic;
[SingletonGame]
[AutoReset]
public class BasicSalaryProcesses : IBasicSalaryProcesses
{
    private readonly LifeBoardGameVMData _model;
    private readonly LifeBoardGameGameContainer _gameContainer;
    public BasicSalaryProcesses(LifeBoardGameVMData model, LifeBoardGameGameContainer gameContainer)
    {
        _model = model;
        _gameContainer = gameContainer;
    }
    public async Task ChoseSalaryAsync(int salary)
    {
        if (_gameContainer.CanSendMessage())
        {
            await _gameContainer.Network!.SendAllAsync("chosesalary", salary);
        }
        if (_gameContainer.GameStatus == EnumWhatStatus.NeedTradeSalary)
        {
            throw new CustomBasicException("I think if the salary is being traded; must use TradedSalary method instead");
        }
        else if (_gameContainer.GameStatus == EnumWhatStatus.NeedChooseSalary)
        {
            var thisSalary = CardsModule.GetSalaryCard(salary);
            await _gameContainer.ShowCardAsync(thisSalary);
            _gameContainer.SingleInfo!.Hand.Add(thisSalary);
            _gameContainer.SingleInfo.Salary = thisSalary.PayCheck;
            PopulatePlayerProcesses.FillInfo(_gameContainer.SingleInfo);
        }
        if (_gameContainer.SaveRoot.EndAfterSalary)
        {
            _gameContainer.GameStatus = EnumWhatStatus.NeedToEndTurn;
        }
        else
        {
            _gameContainer.GameStatus = EnumWhatStatus.NeedToSpin;
        }
        await _gameContainer.ContinueTurnAsync!.Invoke();
    }
    public Task LoadSalaryListAsync()
    {
        _model.HandList.Text = "List Of Salaries";
        _model.Instructions = "Choose a salary";
        var firstList = _gameContainer.Random.GenerateRandomList(27, 9, 19);
        var tempList = firstList.GetSalaryList(_gameContainer.PlayerList!);
        var careerList = _gameContainer.SingleInfo!.GetCareerList();
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
        _model.HandList.HandList.Clear();
        _gameContainer.Command.UpdateAll();
        _model.HandList.HandList.ReplaceRange(tempList);
        _model.HandList.AutoSelect = EnumHandAutoType.SelectOneOnly;
        return Task.CompletedTask;
    }
}