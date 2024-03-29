﻿namespace CrazyEights.Core.Logic;
[SingletonGame]
[AutoReset]
public class SuitProcesses : ISuitProcesses, IChoosePieceNM, ISerializable
{
    private readonly CrazyEightsGameContainer _gameContainer;
    private readonly CrazyEightsVMData _model;
    public SuitProcesses(CrazyEightsGameContainer gameContainer, CrazyEightsVMData model)
    {
        _gameContainer = gameContainer;
        _model = model;
    }
    public async Task ChoosePieceReceivedAsync(string data)
    {
        EnumSuitList Suit = await js1.DeserializeObjectAsync<EnumSuitList>(data);
        await SuitChosenAsync(Suit);
    }
    public async Task SuitChosenAsync(EnumSuitList chosen)
    {
        _gameContainer.SingleInfo = _gameContainer.PlayerList!.GetWhoPlayer();
        if (_gameContainer.SingleInfo.CanSendMessage(_gameContainer.BasicData!))
        {
            await _gameContainer.Network!.SendAllAsync(mm1.ChosenPiece, chosen);
        }
        _gameContainer.SaveRoot!.CurrentSuit = chosen;
        _gameContainer.SaveRoot.ChooseSuit = false;
        var thisCard = _model!.Pile1!.CurrentCard;
        thisCard.DisplaySuit = chosen;
        await _gameContainer.EndTurnAsync!.Invoke();
    }
}