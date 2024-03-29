﻿namespace SkuckCardGame.Core.Logic;
[SingletonGame]
[AutoReset]
public class TrumpProcesses : ITrumpProcesses
{
    private readonly SkuckCardGameVMData _model;
    private readonly SkuckCardGameGameContainer _gameContainer;
    public TrumpProcesses(SkuckCardGameVMData model, SkuckCardGameGameContainer gameContainer)
    {
        _model = model;
        _gameContainer = gameContainer;
    }
    async Task ITrumpProcesses.TrumpChosenAsync()
    {
        if (_gameContainer.SaveRoot!.TrumpSuit == EnumSuitList.None)
        {
            throw new CustomBasicException("Suit must have been chosen before you can run the method to show the trump actually chosen");
        }
        if (_gameContainer.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
        {
            if (_gameContainer.Test!.NoAnimations == false)
            {
                _model!.Suit1!.SelectSpecificItem(_gameContainer.SaveRoot.TrumpSuit);
                _gameContainer.Command.UpdateAll();
                await _gameContainer.Delay!.DelaySeconds(1);
            }
        }
        _gameContainer.SaveRoot.WhatStatus = EnumStatusList.ChooseBid;
        //try to make it where both players can't do the same time.
        await _gameContainer.ContinueTurnAsync!.Invoke();
    }
}