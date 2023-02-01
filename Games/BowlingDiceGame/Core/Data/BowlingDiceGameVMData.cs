namespace BowlingDiceGame.Core.Data;
[SingletonGame]
[UseLabelGrid]
public partial class BowlingDiceGameVMData : IViewModelData
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    private BowlingDiceGameSaveInfo? _saves;
    private void GetSaves()
    {
        if (_saves is not null)
        {
            return;
        }
        _saves = aa1.Resolver!.Resolve<BowlingDiceGameSaveInfo>();
    }
    public BowlingDiceGameVMData()
    {
    }
    public bool IsExtended
    {
        get
        {
            GetSaves();
            return _saves!.IsExtended;
        }
    }
    public int WhichPart
    {
        get
        {
            GetSaves();
            return _saves!.WhichPart;
        }
    }
    [LabelColumn]
    public int WhatFrame
    {
        get
        {
            GetSaves();
            return _saves!.WhatFrame;
        }
    }
}