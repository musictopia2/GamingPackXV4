namespace MonopolyDicedGame.Blazor;
public partial class PropertyBoardBlazor
{
    [Parameter]
    public string TargetHeight { get; set; } = "";
    [Parameter]
    public int Group { get; set; }
    [Parameter]
    public int Owned { get; set; }
    [Parameter]
    public EventCallback OnClicked { get; set; } //you can click on these.
    private BasicList<TempSpace> GetSpaces()
    {
        BasicList<TempSpace> output = [];
        TempSpace space;
        if (Group == 5)
        {
            space = new()
            {
                Column = 2,
                Row = 1
            };
            if (Owned >= 1)
            {
                space.Owned = true;
            }
            output.Add(space);
            space = new()
            {
                Column = 3,
                Row = 1
            };
            if (Owned >= 2)
            {
                space.Owned = true;
            }
            output.Add(space);
            space = new()
            {
                Column = 4,
                Row = 1
            };
            if (Owned >= 3)
            {
                space.Owned = true;
            }
            output.Add(space);
            return output;
        }
        if (Group == 6)
        {
            space = new()
            {
                Column = 5,
                Row = 1
            };
            if (Owned >= 1)
            {
                space.Owned = true;
            }
            output.Add(space);
            space = new()
            {
                Column = 6,
                Row = 1
            };
            if (Owned >= 2)
            {
                space.Owned = true;
            }
            output.Add(space);
            space = new()
            {
                Column = 7,
                Row = 1
            };
            if (Owned >= 3)
            {
                space.Owned = true;
            }
            output.Add(space);
            return output;
        }
        if (Group == 7)
        {
            space = new()
            {
                Column = 8,
                Row = 2
            };
            if (Owned >= 1)
            {
                space.Owned = true;
            }
            output.Add(space);
            space = new()
            {
                Column = 8,
                Row = 3
            };
            if (Owned >= 2)
            {
                space.Owned = true;
            }
            output.Add(space);
            space = new()
            {
                Column = 8,
                Row = 4
            };
            if (Owned >= 3)
            {
                space.Owned = true;
            }
            output.Add(space);
            return output;
        }
        if (Group == 8)
        {
            space = new()
            {
                Column = 8,
                Row = 6
            };
            if (Owned >= 1)
            {
                space.Owned = true;
            }
            output.Add(space);
            space = new()
            {
                Column = 8,
                Row = 7
            };
            if (Owned >= 2)
            {
                space.Owned = true;
            }
            output.Add(space);
            return output;
        }
        if (Group == 4)
        {
            space = new()
            {
                Column = 1,
                Row = 2
            };
            if (Owned >= 1)
            {
                space.Owned = true;
            }
            output.Add(space);
            space = new()
            {
                Column = 1,
                Row = 3
            };
            if (Owned >= 2)
            {
                space.Owned = true;
            }
            output.Add(space);
            space = new()
            {
                Column = 1,
                Row = 4
            };
            if (Owned >= 3)
            {
                space.Owned = true;
            }
            output.Add(space);
            return output;
        }
        if (Group == 3)
        {
            space = new()
            {
                Column = 1,
                Row = 5
            };
            if (Owned >= 1)
            {
                space.Owned = true;
            }
            output.Add(space);
            space = new()
            {
                Column = 1,
                Row = 6
            };
            if (Owned >= 2)
            {
                space.Owned = true;
            }
            output.Add(space);
            space = new()
            {
                Column = 1,
                Row = 7
            };
            if (Owned >= 3)
            {
                space.Owned = true;
            }
            output.Add(space);
            return output;
        }
        if (Group == 2)
        {
            space = new()
            {
                Column = 2,
                Row = 8
            };
            if (Owned >= 1)
            {
                space.Owned = true;
            }
            output.Add(space);
            space = new()
            {
                Column = 3,
                Row = 8
            };
            if (Owned >= 2)
            {
                space.Owned = true;
            }
            output.Add(space);
            space = new()
            {
                Column = 4,
                Row = 8
            };
            if (Owned >= 3)
            {
                space.Owned = true;
            }
            output.Add(space);
            return output;
        }
        if (Group == 1)
        {
            space = new()
            {
                Column = 6,
                Row = 8
            };
            if (Owned >= 1)
            {
                space.Owned = true;
            }
            output.Add(space);
            space = new()
            {
                Column = 7,
                Row = 8
            };
            if (Owned >= 2)
            {
                space.Owned = true;
            }
            output.Add(space);
            return output;
        }
        throw new CustomBasicException("Not Found");
    }
    public BasicDiceModel GetDice()
    {
        BasicDiceModel output = new();
        output.Populate(Group);
        return output;
    }
}