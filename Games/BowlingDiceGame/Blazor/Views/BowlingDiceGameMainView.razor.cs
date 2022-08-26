namespace BowlingDiceGame.Blazor.Views;
public partial class BowlingDiceGameMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(BowlingDiceGameVMData.NormalTurn))
            .AddLabel("Status", nameof(BowlingDiceGameVMData.Status))
             .AddLabel("Frame", nameof(BowlingDiceGameVMData.WhatFrame));
        base.OnInitialized();
    }
}