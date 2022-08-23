﻿namespace BasicGameFrameworkLibrary.Core.ChooserClasses;

public partial class NumberPicker : SimpleControlObservable
{
    private readonly ItemChooserClass<NumberModel> _privateChoose;
    public readonly BasicList<NumberModel> NumberList = new();
    public ControlCommand? NumberPickedCommand { get; set; }
    public event ChangedNumberValueEventHandler? ChangedNumberValueAsync;
    public delegate Task ChangedNumberValueEventHandler(int Chosen);
    [Command(EnumCommandCategory.Control)]
    private async Task ChooseNumberAsync(NumberModel piece)
    {
        SelectNumberValue(piece.NumberValue);
        await ChangedNumberValueAsync!.Invoke(piece.NumberValue);
    }
    public NumberPicker(CommandContainer command, IGamePackageResolver resolver) : base(command)
    {
        _privateChoose = new(resolver)
        {
            ValueList = NumberList
        };
        CreateCommands();
        //MethodInfo method = this.GetPrivateMethod(nameof(ChooseNumberAsync));
        //NumberPickedCommand = new ControlCommand(this, method, command);
    }
    partial void CreateCommands();
    public int NumberToChoose(bool requiredToChoose = true, bool useHalf = true)
    {
        return _privateChoose.ItemToChoose(requiredToChoose, useHalf);
    }
    public void UnselectAll()
    {
        NumberList.UnselectAllObjects();
    }
    protected override void PrivateEnableAlways() { }
    protected override void EnableChange()
    {
        NumberList.SetEnabled(IsEnabled);
    }
    public void LoadNormalNumberRangeValues(int lowestNumber, int highestNumber, int diffs = 1)
    {
        if (lowestNumber > highestNumber)
        {
            throw new CustomBasicException("The largest number must be higher than lowest number");
        }
        if (lowestNumber < 0)
        {
            throw new CustomBasicException("The lowest number cannot be less than 0.  If that is needed, then rethinking is required");
        }
        if (diffs < 1)
        {
            throw new CustomBasicException("Must have a differential of at least 1.  Otherwise, will loop forever");
        }
        BasicList<int> tempList = new();
        int x;
        var loopTo = highestNumber;
        for (x = lowestNumber; x <= loopTo; x += diffs)
        {
            tempList.Add(x);
        }
        LoadNumberList(tempList);
    }
    public void LoadNumberList(BasicList<int> thisList)
    {
        if (thisList.Any(Items => Items < 0))
        {
            throw new CustomBasicException("You should not be allowed to use less than 0.  If that is needed, then rethinking is required");
        }
        BasicList<NumberModel> tempList = new();
        thisList.ForEach(items =>
        {
            NumberModel number = new();
            number.NumberValue = items;
            tempList.Add(number);
        });
        NumberList.ReplaceRange(tempList);
    }
    public void SelectNumberValue(int number)
    {
        NumberList.SelectSpecificItem(items => items.NumberValue, number);
    }
}