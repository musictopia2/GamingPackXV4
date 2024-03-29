﻿namespace DealCardGame.Core.ViewModels;
[InstanceGame]
public partial class JustSayNoViewModel : IBasicEnableProcess
{
    private readonly DealCardGameGameContainer _gameContainer;
    public readonly DealCardGameVMData VMData;
    private readonly DealCardGameMainGameClass _mainGame;
    private readonly DealCardGameCardInformation _actionCard;
    public JustSayNoViewModel(DealCardGameGameContainer gameContainer,
        DealCardGameVMData model,
        DealCardGameMainGameClass mainGame)
    {
        CreateCommands(gameContainer.Command);
        _gameContainer = gameContainer;
        VMData = model;
        _mainGame = mainGame;
        _actionCard = _gameContainer.DeckList.GetSpecificItem(_gameContainer.SaveRoot.ActionCardUsed);
    }
    public void AddAction(Action action)
    {
        _gameContainer.Command.CustomStateHasChanged += action;
    }
    public void RemoveAction()
    {
        _gameContainer.Command.ResetCustomStates();
    }
    public DealCardGameCardInformation ActionCard => _actionCard;
    public CommandContainer GetCommandContainer => _gameContainer.Command;
    public DealCardGamePlayerItem GetSelf => _gameContainer.PlayerList!.GetSelf(); //i think.
    public DealCardGamePlayerItem GetOpponent => _gameContainer.PlayerList!.Single(x => x.Id == SaveRoot.PlayerUsedAgainst);
    public bool IsWhoTurn => GetSelf.Id == _gameContainer.WhoTurn;
    public DealCardGameSaveInfo SaveRoot => _gameContainer.SaveRoot;
    public BasicList<DealCardGamePlayerItem> PlayerList => _gameContainer.PlayerList!.Where(x => x.AllPlayerStatus == EnumAllPlayerStatus.Reject).ToBasicList();
    partial void CreateCommands(CommandContainer command);
    [Command(EnumCommandCategory.Game)]
    public async Task AcceptAsync()
    {
        if (_mainGame.BasicData.MultiPlayer)
        {
            await _mainGame.Network!.SendAllAsync("accept");
        }
        RemoveAction();
        await _mainGame.ProcessAcceptanceAsync();
    }
    [Command(EnumCommandCategory.Game)]
    public async Task RejectAsync()
    {
        RemoveAction();
        if (_mainGame.BasicData.MultiPlayer)
        {
            await _mainGame.Network!.SendAllAsync("reject");
        }
        await _mainGame.ProcessRejectionAsync();
    }
    public bool CanEnableBasics()
    {
        return true;
    }
    


    //public TradePropertyModel TradeInfo => _gameContainer.PersonalInformation.TradeInfo;
    //public DealCardGamePlayerItem GetYourPlayer => _gameContainer.SingleInfo!;
    //public DealCardGamePlayerItem GetChosenPlayer => _gameContainer.PlayerList!.Single(x => x.Id == _gameContainer.PersonalInformation.TradeInfo.PlayerId);
}