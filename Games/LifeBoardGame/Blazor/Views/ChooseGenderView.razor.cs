namespace LifeBoardGame.Blazor.Views;
public partial class ChooseGenderView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private static string GetColor(EnumGender gender) => gender.Color;
    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(ChooseGenderViewModel.Turn))
            .AddLabel("Instructions", nameof(ChooseGenderViewModel.Instructions));
        base.OnInitialized();
    }
}