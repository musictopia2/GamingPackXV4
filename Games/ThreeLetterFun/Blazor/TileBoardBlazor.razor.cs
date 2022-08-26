namespace ThreeLetterFun.Blazor;
public partial class TileBoardBlazor
{
    [Parameter]
    public int TargetHeight { get; set; } //hopefully this would work (for the cards) (?)
    private SizeF DefaultSize { get; set; }
    protected override void OnInitialized()
    {
        ThreeLetterFunCardData obj = new();
        DefaultSize = obj.DefaultSize;
        base.OnInitialized();
    }
    private SizeF GetViewSize()
    {
        SizeF output = new(DefaultSize.Width * DataContext!.GameBoard.Columns, DefaultSize.Height * DataContext.GameBoard.Rows);
        output.Width += (DataContext.GameBoard.Columns * 2);
        output.Height += (DataContext.GameBoard.Rows * 2);
        return output;
    }
    private string GetTargetString
    {
        get
        {
            int totals = TargetHeight * DataContext!.GameBoard.Rows;
            //can't do vh this time.  has to assume.
            string output = totals.HeightString();
            return output;
        }
    }
}