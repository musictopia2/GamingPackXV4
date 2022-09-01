namespace SkuckCardGame.Core.Logic;
[SingletonGame]
[AutoReset]
public class PlayChoiceProcesses : IPlayChoiceProcesses
{
    private readonly SkuckCardGameGameContainer _gameContainer;
    public PlayChoiceProcesses(SkuckCardGameGameContainer gameContainer)
    {
        _gameContainer = gameContainer;
    }
    async Task IPlayChoiceProcesses.ChooseToPassAsync()
    {
        if (_gameContainer.WhoTurn == 1)
        {
            _gameContainer.WhoTurn = 2;
        }
        else
        {
            _gameContainer.WhoTurn = 1;
        }
        _gameContainer.SaveRoot!.WhatStatus = EnumStatusList.NormalPlay;
        await _gameContainer.StartNewTrickAsync!.Invoke();
    }
    async Task IPlayChoiceProcesses.ChooseToPlayAsync()
    {
        _gameContainer.SaveRoot!.WhatStatus = EnumStatusList.NormalPlay;
        await _gameContainer.StartNewTrickAsync!.Invoke();
    }
}