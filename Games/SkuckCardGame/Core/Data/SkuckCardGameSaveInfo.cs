namespace SkuckCardGame.Core.Data;
[SingletonGame]
public class SkuckCardGameSaveInfo : BasicSavedTrickGamesClass<EnumSuitList, SkuckCardGameCardInformation, SkuckCardGamePlayerItem>, IMappable, ISaveInfo
{
    private int _roundNumber;
    public int RoundNumber
    {
        get { return _roundNumber; }
        set
        {
            if (SetProperty(ref _roundNumber, value))
            {
                if (_model != null)
                {
                    _model.RoundNumber = value;
                }
            }
        }
    }
    private EnumStatusList _whatStatus;
    public EnumStatusList WhatStatus
    {
        get { return _whatStatus; }
        set
        {
            if (SetProperty(ref _whatStatus, value))
            {
                if (_model != null)
                {
                    _model.GameStatus = WhatStatus;
                }
            }
        }
    }
    private SkuckCardGameVMData? _model;
    public void LoadMod(SkuckCardGameVMData model)
    {
        _model = model;
        _model.RoundNumber = RoundNumber;
        _model.GameStatus = WhatStatus;
    }
}