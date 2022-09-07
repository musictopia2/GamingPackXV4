namespace BasicGameFrameworkLibrary.Blazor.BasicControls.ChoicePickers;
public partial class NumberChooserBlazor
{
    [Parameter]
    public string TextHeight { get; set; } = "15vh"; //default to 15 now.  could change if necessary.
    [Parameter]
    public int TextWidth { get; set; } = 40;
    [Parameter]
    public bool CanHighlight { get; set; } = true;
    [Parameter]
    public int Columns { get; set; }
    [Parameter]
    public int Rows { get; set; } = 1;
    [Parameter]
    public string TextColor { get; set; } = cs.Navy;
    [Parameter]
    public NumberPicker? NumberPicker { get; set; }
    private int TotalRows()
    {
        if (Columns == 0 && Rows == 1)
        {
            return 1;
        }
        if (Columns == 1 && Rows == 1)
        {
            return NumberPicker!.NumberList.Count;
        }
        var (_, rows) = SmartRowsColumns();
        return rows;
    }
    private (int columns, int rows) SmartRowsColumns()
    {
        int c;
        int r;
        c = 0;
        r = 1;
        int mc;
        int mr;
        if (Columns < 2)
        {
            mc = 1;
        }
        else
        {
            mc = 3;
        }
        if (Rows < 2)
        {
            mr = 1;
        }
        else
        {
            mr = Rows;
        }
        bool maxedColumns = false;
        foreach (var item in NumberPicker!.NumberList)
        {
            if (Columns > 1)
            {
                c++;
                if (c > Columns)
                {
                    if (maxedColumns == false)
                    {
                        mc = c;
                    }
                    else
                    {
                        mc = Columns;
                    }
                }

                if (c > Columns)
                {
                    maxedColumns = true;
                    c = 0;
                    r++;
                    mr = r;
                }
            }
            else
            {
                if (r > mr)
                {
                    mr = r;
                }
                if (r > Rows)
                {
                    r = 0;
                    c++;
                    mc = c;
                }
            }
        }
        if (mc * mr < NumberPicker!.NumberList.Count)
        {
            mr++; //lean towards one more row.
        }
        return (mc, mr);
    }
    private int TotalColumns()
    {
        if (Columns == 0 && Rows == 1)
        {
            return NumberPicker!.NumberList.Count;
        }
        if (Columns == 1 && Rows == 1)
        {
            return 1; //has to have at least 1.
        }
        var (columns, _) = SmartRowsColumns();
        return columns;
    }
}