using System.Reflection; //needed because of assembly.  don't put under globals though.
namespace SnakesAndLadders.Blazor;
public partial class GameBoardBlazor
{
    [CascadingParameter]
    public SnakesAndLaddersMainViewModel? DataContext { get; set; }
    [Parameter]
    public BasicList<SnakesAndLaddersPlayerItem>? PlayerList { get; set; }
    private BasicList<SnakesAndLaddersPlayerItem> ModifiedList
    {
        get
        {
            var firstList = PlayerList!.ToBasicList();
            firstList.RemoveAllOnly(x => x.SpaceNumber == 0);
            var nextList = firstList.GroupBy(x => x.SpaceNumber);
            BasicList<SnakesAndLaddersPlayerItem> output = new();
            foreach (var item in nextList)
            {
                output.Add(item.Last());
            }
            return output;
        }
    }
    private class TempSpace
    {
        public RectangleF Bounds { get; set; }
        public string Fill { get; set; } = ""; //i think.
    }
    private readonly Dictionary<int, TempSpace> _spaceList = new();
    private Assembly GetAssembly => Assembly.GetAssembly(GetType())!;
    private IToast? _toast;
    protected override void OnInitialized()
    {
        _toast = aa1.Resolver!.Resolve<IToast>();
        _spaceList.Clear();
        LoadSpaces();
        base.OnInitialized();
    }
    private async Task SpaceClicked(int number)
    {
        if (DataContext!.CanMakeMove(number) == false)
        {
            _toast!.ShowUserErrorToast("Illegal Move");
            DataContext.CommandContainer.StopExecuting(); //because it starts automatically now.
            return;
        }
        await DataContext.CommandContainer.ProcessCustomCommandAsync(DataContext.MakeMoveAsync, number); //try this way.
    }
    private void LoadSpaces()
    {
        RectangleF bounds = new(0, 0, 500, 500);
        int int_RowCount;
        for (int_RowCount = 1; int_RowCount <= 10; int_RowCount++)
        {
            int int_ColCount;
            for (int_ColCount = 1; int_ColCount <= 10; int_ColCount++)
            {
                TempSpace thisExp = new();
                int int_Count;
                if ((int_RowCount % 2) == 0)
                {
                    int_Count = (100 - (((int_RowCount - 1) * 10) + (11 - int_ColCount))) + 1;
                }
                else
                {
                    int_Count = (100 - (((int_RowCount - 1) * 10) + (int_ColCount))) + 1;
                }
                // *** If it's an even row, number it backwards
                if (int_Count == 100)
                {
                    thisExp.Fill = cc1.DarkRed;
                }
                else if ((int_Count % 2) == 0)
                {
                    thisExp.Fill = cc1.Gold;
                }
                else
                {
                    thisExp.Fill = cc1.DodgerBlue;
                }
                thisExp.Bounds = new(bounds.Location.X + ((bounds.Width * (int_ColCount - 1)) / 10), bounds.Location.Y + ((bounds.Height * (int_RowCount - 1)) / 10), ((bounds.Width * int_ColCount) / 10) - ((bounds.Width * (int_ColCount - 1)) / 10), ((bounds.Height * int_RowCount) / 10) - ((bounds.Height * (int_RowCount - 1)) / 10));
                _spaceList.Add(int_Count, thisExp);
            }
        }
    }
}