namespace MonopolyDicedGame.Blazor;
public partial class TrainSpaceBlazor
{
    [Parameter]
    [EditorRequired]
    public string TargetHeight { get; set; } = "";
    [Parameter]
    public int Owned { get; set; }
    [Parameter]
    public EventCallback OnClicked { get; set; } //you can click on these.
    private static string Color => cc1.DarkGray.ToWebColor();
    private BasicList<TempSpace> GetSpaces()
    {
        BasicList<TempSpace> output = [];
        TempSpace space;
        space = new()
        {
            Column = 3,
            Row = 2
        };
        if (Owned >= 1)
        {
            space.Owned = true;
        }
        output.Add(space);
        space = new()
        {
            Column = 4,
            Row = 2
        };
        if (Owned >= 2)
        {
            space.Owned = true;
        }
        output.Add(space);
        space = new()
        {
            Column = 5,
            Row = 2
        };
        if (Owned >= 3)
        {
            space.Owned = true;
        }
        output.Add(space);
        space = new()
        {
            Column = 6,
            Row = 2
        };
        if (Owned >= 4)
        {
            space.Owned= true;
        }
        output.Add(space);
        return output;
    }
    private static BasicDiceModel GetDice()
    {
        BasicDiceModel output = new();
        output.Populate(11);
        return output;
    }
}