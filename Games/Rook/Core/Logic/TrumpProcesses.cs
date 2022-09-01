﻿namespace Rook.Core.Logic;
[SingletonGame]
[AutoReset]
public class TrumpProcesses : ITrumpProcesses
{
    private readonly RookVMData _model;
    private readonly RookGameContainer _gameContainer;
    public TrumpProcesses(RookVMData model, RookGameContainer gameContainer)
    {
        _model = model;
        _gameContainer = gameContainer;
    }
    public async Task ProcessTrumpAsync()
    {
        _gameContainer.SaveRoot!.TrumpSuit = _model!.ColorChosen;
        if (_gameContainer.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
        {
            _model.Color1!.SelectSpecificItem(_model.ColorChosen);
        }
        if (_gameContainer.SingleInfo.CanSendMessage(_gameContainer.BasicData!))
        {
            await _gameContainer.Network!.SendAllAsync("colorselected", _model.ColorChosen);
        }
        if (_gameContainer.Test!.NoAnimations == false)
        {
            await _gameContainer.Delay!.DelaySeconds(1);
        }
        ResetTrumps();
        _gameContainer.SaveRoot.GameStatus = EnumStatusList.SelectNest;
        _gameContainer.SingleInfo.MainHandList.AddRange(_gameContainer.SaveRoot.NestList);
        if (_gameContainer.SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
        {
            _model.PlayerHand1!.AutoSelect = EnumHandAutoType.SelectAsMany;
            _gameContainer.SortCards!.Invoke();
        }
        if (_gameContainer.PlayerList!.Count == 2)
        {
            _model.Dummy1!.MakeAllKnown();
            _model.Dummy1.HandList.Sort();
        }
        _model.Status = "Choose the 5 cards to get rid of";
        await _gameContainer.StartNewTurnAsync!.Invoke();
    }
    public void ResetTrumps()
    {
        _model.ColorChosen = EnumColorTypes.None; //i think
        _model.Color1!.UnselectAll();
    }
}