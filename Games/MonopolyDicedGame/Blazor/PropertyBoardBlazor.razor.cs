namespace MonopolyDicedGame.Blazor;
public partial class PropertyBoardBlazor
{
    [Parameter]
    public string TargetHeight { get; set; } = "";
    [Parameter]
    public int Group { get; set; }
    [Parameter]
    public BasicList<OwnedModel> OwnList { get; set; } = [];
    [Parameter]
    public EventCallback OnClicked { get; set; } //you can click on these.
    [Parameter]
    [EditorRequired]
    public bool IsEnabled { get; set; }
    [Inject]
    private IToast? Toast { get; set; }
    private BasicList<OwnedModel> _properties = [];
    protected override void OnParametersSet()
    {
        _properties = GetOwnedProperties();
        base.OnParametersSet();
    }

    private void PossibleClick()
    {
        if (IsEnabled == false)
        {
            return;
        }
        if (_properties.All(x => x.Group == Group))
        {
            Toast!.ShowUserErrorToast("Already owned all the properties");
            return;
        }
        OnClicked.InvokeAsync();
    }

    private BasicList<OwnedModel> GetOwnedProperties()
    {
        //must return 2 or 4 items.
        BasicList<OwnedModel> output = [];
        OwnedModel own;
        2.Times(x =>
        {
            own = new();
            output.Add(own);
        });
        if (Group != 1 && Group != 8)
        {
            own = new();
            output.Add(own);
        }
        BasicList<OwnedModel> list = OwnList.Where(x => x.Group == Group).ToBasicList();
        if (list.Count > 3)
        {
            throw new CustomBasicException("Cannot own more than 3 properties for group");
        }
        if (list.Count > 2 && (Group == 1 || Group == 8))
        {
            throw new CustomBasicException("Group 1 and group 8 cannot own more than 2 properties");
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
        if (Group == 5)
        {
            space = new()
            {
                Column = 2,
                Row = 1,
                Own = _properties[0]
            };
            output.Add(space);
            space = new()
            {
                Column = 3,
                Row = 1,
                Own = _properties[1]
            };
            output.Add(space);
            space = new()
            {
                Column = 4,
                Row = 1,
                Own = _properties[2]
            };
            output.Add(space);
            return output;
        }
        if (Group == 6)
        {
            space = new()
            {
                Column = 5,
                Row = 1,
                Own = _properties[0]
            };
            output.Add(space);
            space = new()
            {
                Column = 6,
                Row = 1,
                Own = _properties[1]
            };
            output.Add(space);
            space = new()
            {
                Column = 7,
                Row = 1,
                Own = _properties[2]
            };
            output.Add(space);
            return output;
        }
        if (Group == 7)
        {
            space = new()
            {
                Column = 8,
                Row = 2,
                Own = _properties[0]
            };
            output.Add(space);
            space = new()
            {
                Column = 8,
                Row = 3,
                Own = _properties[1]
            };
            output.Add(space);
            space = new()
            {
                Column = 8,
                Row = 4,
                Own = _properties[2]
            };
            output.Add(space);
            return output;
        }
        if (Group == 8)
        {
            space = new()
            {
                Column = 8,
                Row = 6,
                Own = _properties[0]
            };
            output.Add(space);
            space = new()
            {
                Column = 8,
                Row = 7,
                Own = _properties[1]
            };
            output.Add(space);
            return output;
        }
        if (Group == 4)
        {
            space = new()
            {
                Column = 1,
                Row = 2,
                Own = _properties[0]
            };
            output.Add(space);
            space = new()
            {
                Column = 1,
                Row = 3,
                Own = _properties[1]
            };
            output.Add(space);
            space = new()
            {
                Column = 1,
                Row = 4,
                Own = _properties[2]
            };
            output.Add(space);
            return output;
        }
        if (Group == 3)
        {
            space = new()
            {
                Column = 1,
                Row = 5,
                Own = _properties[0]
            };
            output.Add(space);
            space = new()
            {
                Column = 1,
                Row = 6,
                Own = _properties[1]
            };
            output.Add(space);
            space = new()
            {
                Column = 1,
                Row = 7,
                Own = _properties[2]
            };
            output.Add(space);
            return output;
        }
        if (Group == 2)
        {
            space = new()
            {
                Column = 2,
                Row = 8,
                Own = _properties[0]
            };
            output.Add(space);
            space = new()
            {
                Column = 3,
                Row = 8,
                Own = _properties[1]
            };
            output.Add(space);
            space = new()
            {
                Column = 4,
                Row = 8,
                Own = _properties[2]
            };
            output.Add(space);
            return output;
        }
        if (Group == 1)
        {
            space = new()
            {
                Column = 6,
                Row = 8,
                Own = _properties[0]
            };
            output.Add(space);
            space = new()
            {
                Column = 7,
                Row = 8,
                Own = _properties[1]
            };
            output.Add(space);
            return output;
        }
        throw new CustomBasicException("Not Found");
    }
    public BasicDiceModel GetDice(OwnedModel own)
    {
        BasicDiceModel output = new();
        if (own.WasChance)
        {
            output.UseChance();
        }
        else
        {
            output.Populate(Group);
        }
        return output;
    }
}