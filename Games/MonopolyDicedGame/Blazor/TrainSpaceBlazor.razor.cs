namespace MonopolyDicedGame.Blazor;
public partial class TrainSpaceBlazor
{
    [Parameter]
    [EditorRequired]
    public string TargetHeight { get; set; } = "";
    [Parameter]
    public BasicList<OwnedModel> OwnList { get; set; } = []; //this will filter out the trains.
    [Parameter]
    [EditorRequired]
    public bool IsEnabled { get; set; }
    [Inject]
    public IToast? Toast { get; set; }
    private BasicList<OwnedModel> _trains = [];
    protected override void OnParametersSet()
    {
        _trains = GetOwnedTrains();
        base.OnParametersSet();
    }
    private void PossibleClick()
    {
        if (IsEnabled == false)
        {
            return;
        }    
        if(_trains.All(x => x.UsedOn == EnumBasicType.Railroad))
        {
            Toast!.ShowUserErrorToast("You already have 4 trains owned");
            return;
        }
        OnClicked.InvokeAsync();
    }

    //[Parameter]
    //public int Owned { get; set; }
    [Parameter]
    public EventCallback OnClicked { get; set; } //you can click on these.
    private static string Color => cc1.DarkGray.ToWebColor;
    private BasicList<OwnedModel> GetOwnedTrains()
    {
        //must return 4 items.
        BasicList<OwnedModel> output = [];
        OwnedModel own;
        4.Times(x =>
        {
            own = new();
            output.Add(own);
        });
        BasicList<OwnedModel> list = OwnList.Where(x => x.UsedOn == EnumBasicType.Railroad).ToBasicList();
        if (list.Count > 4)
        {
            throw new CustomBasicException("Cannot own more than 4 trains");
        }
        int x = 0;
        foreach (var item in list)
        {
            output[x] = item;
            x++;
        }
        return output;
    }
    private BasicList<TempSpace> GetSpaces()
    {
        BasicList<TempSpace> output = [];
        TempSpace space;
        space = new()
        {
            Column = 3,
            Row = 2,
            Own = _trains[0]
        };
        output.Add(space);
        space = new()
        {
            Column = 4,
            Row = 2,
            Own = _trains[1]
        };
        output.Add(space);
        space = new()
        {
            Column = 5,
            Row = 2,
            Own = _trains[2]
        };
        output.Add(space);
        space = new()
        {
            Column = 6,
            Row = 2,
            Own = _trains[3]
        };
        output.Add(space);
        return output;
    }
    private static BasicDiceModel GetDice(OwnedModel own)
    {
        BasicDiceModel output = new();
        if (own.WasChance)
        {
            output.UseChance();
        }
        else
        {
            output.Populate(11);
        }
        return output;
    }
}