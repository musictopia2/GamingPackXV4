namespace SkuckCardGame.Core.Logic;
[SingletonGame]
[AutoReset]
public class PlayChoiceProcesses(SkuckCardGameGameContainer gameContainer) : IPlayChoiceProcesses
{
    async Task IPlayChoiceProcesses.ChooseToPassAsync()
    {
        if (gameContainer.WhoTurn == 1)
        {
            gameContainer.WhoTurn = 2;
        }
        else
        {
            gameContainer.WhoTurn = 1;
        }
        gameContainer.SaveRoot!.WhatStatus = EnumStatusList.NormalPlay;
        await gameContainer.StartNewTrickAsync!.Invoke();
    }
    async Task IPlayChoiceProcesses.ChooseToPlayAsync()
    {
        gameContainer.SaveRoot!.WhatStatus = EnumStatusList.NormalPlay;
        await gameContainer.StartNewTrickAsync!.Invoke();
    }
}