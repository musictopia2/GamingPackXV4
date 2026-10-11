namespace BowlingDiceGame.Core.Data;
[SingletonGame]
[UseLabelGrid]
public partial class BowlingDiceGameVMData : IViewModelData
{
    [LabelColumn]
    public partial string NormalTurn { get; set; } = "";
    [LabelColumn]
    public partial string Status { get; set; } = "";
    private BowlingDiceGameSaveInfo? _saves;
    private void GetSaves()
    {
        if (_saves is not null)
        {
            return;
        }
        _saves = Resolver!.Resolve<BowlingDiceGameSaveInfo>();
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
    public void UpdateFrame()
    {
        OnLabelValueChanged(nameof(WhatFrame));
    }
}