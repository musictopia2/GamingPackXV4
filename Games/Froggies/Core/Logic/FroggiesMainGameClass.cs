namespace Froggies.Core.Logic;
[SingletonGame]
public class FroggiesMainGameClass : IAggregatorContainer
{
    private readonly IRandomGenerator _rs;
    private readonly IToast _toast;
    public FroggiesMainGameClass(
        IEventAggregator aggregator,
        IRandomGenerator rs,
        IToast toast
        )
    {
        Aggregator = aggregator;
        _rs = rs;
        _toast = toast;
    }
    private Dictionary<string, LilyPadModel>? _col_Pads;
    private Dictionary<string, LilyPadModel>? _col_TargetPads = new();
    private BasicList<LilyPadModel>? _col_RedoPads;
    private LilyPadModel? _obj_SelectedPad;
    public BasicList<LilyPadModel> GetCompleteLilyList()
    {
        if (_col_Pads is null)
        {
            return new(); //try this.
        }
        return (from items in _col_Pads
                select items.Value).ToBasicList();
    }
    public IEventAggregator Aggregator { get; }
    public Task NewGameAsync(FroggiesMainViewModel model)
    {
        CreateLilyPads(model);
        CopyToRedo();
        model.MovesLeft = MovesLeft();
        model.DrawVersion++;
        return Task.CompletedTask;
    }
    public int NumberOfMoves()
    {
        return MovesLeft();
    }
    private void CreateLilyPads(FroggiesMainViewModel model)
    {
        LilyPadModel? obj_TempPad1 = null;
        LilyPadModel? obj_TempPad2 = null;
        LilyPadModel obj_CurrentPad;
        int int_RowCount;
        int int_ColCount;
        int int_FrogCount;
        int int_TryCount = 0;
        int int_RandomNumber;
        int int_RandomIndex;
        int int_NewX = 0;
        int int_NewY = 0;
        int int_NewX2 = 0;
        int int_NewY2 = 0;
        bool bln_Pad1OK;
        bool bln_Pad2OK;
        _col_Pads = new Dictionary<string, LilyPadModel>();
        _col_TargetPads = new Dictionary<string, LilyPadModel>();
        _obj_SelectedPad = null;
        int_ColCount = 4;
        int_RowCount = 4;

        // *** This will be the final pad in the puzzle
        obj_CurrentPad = new LilyPadModel(int_ColCount, int_RowCount, true);
        _col_Pads.Add($"{int_ColCount}, {int_RowCount}", obj_CurrentPad);
        int_FrogCount = 1;
        while (int_FrogCount < model.NumberOfFrogs)
        {

            // *** Get a random occupied pad
            int_RandomIndex = GetRandomInteger(_col_Pads.Count - 1);
            obj_CurrentPad = _col_Pads.ElementAt(int_RandomIndex).Value;
            // *** Only find a new path if the pad has a frog on it
            if (obj_CurrentPad.HasFrog)
            {
                int_RandomNumber = GetRandomInteger(4);

                // *** Pick a random direction in which to move
                switch (int_RandomNumber)
                {
                    case 0: // move up
                        {
                            int_NewX = obj_CurrentPad.Column;
                            int_NewY = obj_CurrentPad.Row - 1;
                            int_NewX2 = obj_CurrentPad.Column;
                            int_NewY2 = obj_CurrentPad.Row - 2;
                            break;
                        }

                    case 1: // move down
                        {
                            int_NewX = obj_CurrentPad.Column;
                            int_NewY = obj_CurrentPad.Row + 1;
                            int_NewX2 = obj_CurrentPad.Column;
                            int_NewY2 = obj_CurrentPad.Row + 2;
                            break;
                        }

                    case 2: // move left
                        {
                            int_NewX = obj_CurrentPad.Column - 1;
                            int_NewY = obj_CurrentPad.Row;
                            int_NewX2 = obj_CurrentPad.Column - 2;
                            int_NewY2 = obj_CurrentPad.Row;
                            break;
                        }

                    case 3: // move right
                        {
                            int_NewX = obj_CurrentPad.Column + 1;
                            int_NewY = obj_CurrentPad.Row;
                            int_NewX2 = obj_CurrentPad.Column + 2;
                            int_NewY2 = obj_CurrentPad.Row;
                            break;
                        }
                }

                bln_Pad1OK = false;
                bln_Pad2OK = false;

                if ((int_NewX != int_NewX2) | (int_NewY != int_NewY2))
                {

                    // *** If the target pad already exists, make sure it is blank
                    if (_col_Pads.ContainsKey(int_NewX + ", " + int_NewY))
                    {
                        obj_TempPad1 = _col_Pads[int_NewX + ", " + int_NewY];
                        if (!obj_TempPad1.HasFrog)
                            bln_Pad1OK = true;
                    }
                    else
                    {
                        bln_Pad1OK = true;
                    }

                    // *** If the target pad already exists, make sure it is blank
                    if (_col_Pads.ContainsKey(int_NewX2 + ", " + int_NewY2))
                    {
                        obj_TempPad2 = _col_Pads[int_NewX2 + ", " + int_NewY2];
                        if (!obj_TempPad2.HasFrog)
                            bln_Pad2OK = true;
                    }
                    else
                    {
                        bln_Pad2OK = true;
                    }

                    if (bln_Pad1OK & bln_Pad2OK & (int_NewX2 >= 0) & (int_NewX2 <= 9) & (int_NewX >= 0) & (int_NewX <= 9) & (int_NewY2 >= 0) & (int_NewY2 <= 9) & (int_NewY >= 0) & (int_NewY <= 9))
                    {
                        if (_col_Pads.ContainsKey(int_NewX + ", " + int_NewY))
                        {
                            obj_TempPad1!.HasFrog = true;
                        }
                        else
                        {
                            obj_TempPad1 = new LilyPadModel(int_NewX, int_NewY, true);
                            _col_Pads.Add(int_NewX + ", " + int_NewY, obj_TempPad1);
                        }

                        if (_col_Pads.ContainsKey(int_NewX2 + ", " + int_NewY2))
                        {
                            obj_TempPad2!.HasFrog = true;
                        }
                        else
                        {
                            obj_TempPad2 = new LilyPadModel(int_NewX2, int_NewY2, true);
                            _col_Pads.Add(int_NewX2 + ", " + int_NewY2, obj_TempPad2);
                        }

                        int_TryCount = 0;
                        obj_CurrentPad.HasFrog = false;
                        int_FrogCount = GetNumberOfFrogs;
                    }
                }
            }

            int_TryCount += 1;

            // *** Make sure we didn't get stuck - if we did, start over
            if (int_TryCount >= 1000)
            {
                _col_Pads.Clear();
                _col_Pads = new Dictionary<string, LilyPadModel>();
                obj_CurrentPad = new LilyPadModel(4, 4, true);
                _col_Pads.Add(int_ColCount + ", " + int_RowCount, obj_CurrentPad);
                int_FrogCount = 1;
            }
        }
    }
    private int GetNumberOfFrogs => _col_Pads!.Values.Count(items => items.HasFrog);
    private int GetRandomInteger(int int_Span)
    {
        int int_RandomInteger;
        //string str_Random;
        if (int_Span <= 0)
        {
            int_RandomInteger = 0;
        }
        else
        {
            int_RandomInteger = _rs.GetRandomNumber(int_Span, 0); //because its 0 based.  try this way.  taking a risk.  hopefully this way works.
        }
        return int_RandomInteger;
    }
    internal int MovesLeft()
    {
        IEnumerator enum_Pads;
        enum_Pads = _col_Pads!.GetEnumerator();
        LilyPadModel obj_TempTab;
        LilyPadModel obj_TargetPad;
        LilyPadModel obj_LeapPad;
        int int_CountMoves = 0;
        while (enum_Pads.MoveNext())
        {
            var ThisItem = (KeyValuePair<string, LilyPadModel>)enum_Pads.Current;
            obj_TempTab = ThisItem.Value;
            if (obj_TempTab.HasFrog)
            {

                // *** Check up

                if (_col_Pads.ContainsKey($"{obj_TempTab.Column}, {obj_TempTab.Row - 2}") & _col_Pads.ContainsKey($"{obj_TempTab.Column}, {obj_TempTab.Row - 1}"))
                {
                    obj_TargetPad = _col_Pads[$"{obj_TempTab.Column}, {obj_TempTab.Row - 2}"];
                    obj_LeapPad = _col_Pads[$"{obj_TempTab.Column}, {obj_TempTab.Row - 1}"];
                    if ((!obj_TargetPad.HasFrog) & obj_LeapPad.HasFrog)
                    {
                        int_CountMoves += 1;
                    }
                }

                // *** Check down

                if (_col_Pads.ContainsKey($"{obj_TempTab.Column}, {obj_TempTab.Row + 2}") & _col_Pads.ContainsKey($"{obj_TempTab.Column}, {obj_TempTab.Row + 1}"))
                {
                    obj_TargetPad = _col_Pads[$"{obj_TempTab.Column}, {obj_TempTab.Row + 2}"];
                    obj_LeapPad = _col_Pads[$"{obj_TempTab.Column}, {obj_TempTab.Row + 1}"];
                    if ((!obj_TargetPad.HasFrog) & obj_LeapPad.HasFrog)
                    {
                        int_CountMoves += 1;
                    }
                }



                // *** Check left

                if (_col_Pads.ContainsKey($"{obj_TempTab.Column - 2}, {obj_TempTab.Row}") & _col_Pads.ContainsKey($"{obj_TempTab.Column - 1}, {obj_TempTab.Row}"))
                {
                    obj_TargetPad = _col_Pads[$"{obj_TempTab.Column - 2}, {obj_TempTab.Row}"];
                    obj_LeapPad = _col_Pads[$"{obj_TempTab.Column - 1}, {obj_TempTab.Row}"];
                    if ((!obj_TargetPad.HasFrog) & obj_LeapPad.HasFrog)
                    {
                        int_CountMoves += 1;
                    }
                }


                // *** Check right

                if (_col_Pads.ContainsKey($"{obj_TempTab.Column + 2}, {obj_TempTab.Row}") & _col_Pads.ContainsKey($"{obj_TempTab.Column + 1}, {obj_TempTab.Row}"))
                {
                    obj_TargetPad = _col_Pads[$"{obj_TempTab.Column + 2}, {obj_TempTab.Row}"];
                    obj_LeapPad = _col_Pads[$"{obj_TempTab.Column + 1}, {obj_TempTab.Row}"];
                    if ((!obj_TargetPad.HasFrog) & obj_LeapPad.HasFrog)
                    {
                        int_CountMoves += 1;
                    }
                }


            }
        }

        return int_CountMoves;
    }

    private bool CheckIfWon()
    {
        IEnumerator enum_Pads;
        LilyPadModel obj_TempPad;
        int int_FrogCount = 0;
        enum_Pads = _col_Pads!.GetEnumerator();

        // *** Find the selected pad
        while (enum_Pads.MoveNext())
        {
            var ThisItem = (KeyValuePair<string, LilyPadModel>)enum_Pads.Current;
            obj_TempPad = ThisItem.Value;
            if (obj_TempPad.HasFrog)
            {
                int_FrogCount += 1;
            }
        }
        if (int_FrogCount > 1)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private async Task LeapFrogAsync(LilyPadModel obj_Target, FroggiesMainViewModel model)
    {
        LilyPadModel obj_Leap;
        string str_Index = "";
        if (obj_Target.Row < _obj_SelectedPad!.Row)
        {
            str_Index = $"{_obj_SelectedPad.Column}, {_obj_SelectedPad.Row - 1}";
        }
        else if (obj_Target.Row > _obj_SelectedPad.Row)
        {
            str_Index = $"{_obj_SelectedPad.Column}, {_obj_SelectedPad.Row + 1}";
        }
        else if (obj_Target.Column < _obj_SelectedPad.Column)
        {
            str_Index = $"{_obj_SelectedPad.Column - 1}, {_obj_SelectedPad.Row}";
        }
        else if (obj_Target.Column > _obj_SelectedPad.Column)
        {
            str_Index = $"{_obj_SelectedPad.Column + 1}, {_obj_SelectedPad.Row}";
        }
        obj_Leap = _col_Pads![str_Index];
        obj_Leap.HasFrog = false;
        _obj_SelectedPad.HasFrog = false;
        obj_Leap.IsSelected = false;
        obj_Leap.IsTarget = false;
        _obj_SelectedPad.IsSelected = false;
        _obj_SelectedPad.IsTarget = false;
        obj_Target.HasFrog = true;
        _obj_SelectedPad = null;
        _col_TargetPads = new Dictionary<string, LilyPadModel>();
        //await NeedsToDrawAsync();
        model.MovesLeft = NumberOfMoves();
        // *** Check if won
        if (CheckIfWon())
        {
            model.NumberOfFrogs++; //i think has to be the model now.
            await NewGameAsync(model);
        }
        else if (MovesLeft() == 0)
        {
            _toast.ShowWarningToast("No Moves Left");
            await NewGameAsync(model);
            model.MovesLeft = NumberOfMoves();
        }
    }
    public async Task ProcessLilyClickAsync(LilyPadModel obj_TempPad, FroggiesMainViewModel model)
    {
        LilyPadModel obj_TargetPad;
        LilyPadModel obj_LeapPad;
        foreach (var item in _col_Pads!)
        {
            item.Value.IsSelected = false; // i think
            item.Value.IsTarget = false;
        }
        if (obj_TempPad.HasFrog)
        {
            _obj_SelectedPad = obj_TempPad;
            _obj_SelectedPad.IsSelected = true;
        }
        else
            // *** Check if it is a target pad
            if (_col_TargetPads!.ContainsKey(obj_TempPad.Column + ", " + obj_TempPad.Row))
        {
            await LeapFrogAsync(obj_TempPad, model);
            return;
        }
        _col_TargetPads = new Dictionary<string, LilyPadModel>();
        // *** Find the target pads
        if (!(_obj_SelectedPad == null))
        {

            // *** Check the different directions we can leap

            // *** Check up

            if (_col_Pads.ContainsKey($"{_obj_SelectedPad.Column}, {_obj_SelectedPad.Row - 2}") & _col_Pads.ContainsKey($"{_obj_SelectedPad.Column}, {_obj_SelectedPad.Row - 1}"))
            {
                obj_TargetPad = _col_Pads[$"{_obj_SelectedPad.Column}, {_obj_SelectedPad.Row - 2}"];
                obj_LeapPad = _col_Pads[$"{_obj_SelectedPad.Column}, {_obj_SelectedPad.Row - 1}"];
                if ((!obj_TargetPad.HasFrog) & obj_LeapPad.HasFrog)
                {
                    _col_TargetPads.Add($"{obj_TargetPad.Column}, {obj_TargetPad.Row}", obj_TargetPad);
                }
            }



            // *** Check down

            if (_col_Pads.ContainsKey($"{_obj_SelectedPad.Column}, {_obj_SelectedPad.Row + 2}") & _col_Pads.ContainsKey($"{_obj_SelectedPad.Column}, {_obj_SelectedPad.Row + 1}"))
            {
                obj_TargetPad = _col_Pads[$"{_obj_SelectedPad.Column}, {_obj_SelectedPad.Row + 2}"];
                obj_LeapPad = _col_Pads[$"{_obj_SelectedPad.Column}, {_obj_SelectedPad.Row + 1}"];
                if ((!obj_TargetPad.HasFrog) & obj_LeapPad.HasFrog)
                {
                    _col_TargetPads.Add($"{obj_TargetPad.Column}, {obj_TargetPad.Row}", obj_TargetPad);
                }
            }



            // *** Check left

            if (_col_Pads.ContainsKey($"{_obj_SelectedPad.Column - 2}, {_obj_SelectedPad.Row}") & _col_Pads.ContainsKey($"{_obj_SelectedPad.Column - 1}, {_obj_SelectedPad.Row}"))
            {
                obj_TargetPad = _col_Pads[$"{_obj_SelectedPad.Column - 2}, {_obj_SelectedPad.Row}"];
                obj_LeapPad = _col_Pads[$"{_obj_SelectedPad.Column - 1}, {_obj_SelectedPad.Row}"];
                if ((!obj_TargetPad.HasFrog) & obj_LeapPad.HasFrog)
                {
                    _col_TargetPads.Add($"{obj_TargetPad.Column}, {obj_TargetPad.Row}", obj_TargetPad);
                }
            }




            // *** Check right

            if (_col_Pads.ContainsKey($"{_obj_SelectedPad.Column + 2}, {_obj_SelectedPad.Row}") & _col_Pads.ContainsKey($"{_obj_SelectedPad.Column + 1}, {_obj_SelectedPad.Row}"))
            {
                obj_TargetPad = _col_Pads[$"{_obj_SelectedPad.Column + 2}, {_obj_SelectedPad.Row}"];
                obj_LeapPad = _col_Pads[$"{_obj_SelectedPad.Column + 1}, {_obj_SelectedPad.Row}"];
                if ((!obj_TargetPad.HasFrog) & obj_LeapPad.HasFrog)
                {
                    _col_TargetPads.Add($"{obj_TargetPad.Column}, {obj_TargetPad.Row}", obj_TargetPad);
                }
            }
        }
        foreach (var item in _col_TargetPads)
        {
            item.Value.IsTarget = true;
        }
    }
    private void CopyToRedo()
    {
        _col_RedoPads = new();
        _col_RedoPads.Clear();
        IEnumerator enum_Pads;
        LilyPadModel obj_TempPad;
        LilyPadModel obj_NewPad;
        enum_Pads = _col_Pads!.GetEnumerator();
        while (enum_Pads.MoveNext())
        {
            var firsts = (KeyValuePair<string, LilyPadModel>)enum_Pads.Current;
            obj_TempPad = firsts.Value;
            obj_NewPad = new LilyPadModel(obj_TempPad.Column, obj_TempPad.Row, obj_TempPad.HasFrog);
            if (obj_TempPad.HasFrog)
            {
                obj_NewPad.StartedWithFrog = true; //hopefully this simple.
            }
            _col_RedoPads.Add(obj_NewPad);
        }
    }
    public Task RedoAsync()
    {
        LilyPadModel obj_TempPad;
        _col_Pads!.Clear();
        IEnumerator enum_Pads;
        enum_Pads = _col_RedoPads!.GetEnumerator();
        while (enum_Pads.MoveNext())
        {
            obj_TempPad = (LilyPadModel)enum_Pads.Current; //hopefully this works.
            _col_Pads.Add(obj_TempPad.Column + ", " + obj_TempPad.Row, new LilyPadModel(obj_TempPad.Column, obj_TempPad.Row, obj_TempPad.StartedWithFrog));
        }
        _col_TargetPads = new Dictionary<string, LilyPadModel>();
        _obj_SelectedPad = null;
        return Task.CompletedTask;
    }
}