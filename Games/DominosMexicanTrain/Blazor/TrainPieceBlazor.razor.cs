namespace DominosMexicanTrain.Blazor;
public partial class TrainPieceBlazor
{
    [Parameter]
    public TrainInfo? TrainInfo { get; set; }
    [Parameter]
    public bool WasDouble { get; set; }
    [Parameter]
    public bool Satisfy { get; set; }
    [Parameter]
    public PrivateTrain PositionInfo { get; set; }
    [Parameter]
    public int Player { get; set; }
    [Parameter]
    public bool Self { get; set; }
    [Parameter]
    public EventCallback<int> TrainClicked { get; set; }
    private SizeF TrainSize { get; set; } = new SizeF(75, 46);
    private string GetFill()
    {
        if (TrainInfo!.IsPublic)
        {
            return cc1.Green.ToWebColor();
        }
        return cc1.Blue.ToWebColor();
    }
    private bool CanClick()
    {
        if (WasDouble == true && Satisfy == false)
        {
            return false;
        }
        return true;
    }
    private bool CanShowTrains()
    {
        if (WasDouble == true && Satisfy == false)
        {
            return false;
        }
        return TrainInfo!.TrainUp || Satisfy;
    }
}