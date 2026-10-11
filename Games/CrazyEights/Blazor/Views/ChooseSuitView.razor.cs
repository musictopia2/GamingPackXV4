namespace CrazyEights.Blazor.Views;
public partial class ChooseSuitView
{
    private CrazyEightsVMData? _data;
    protected override void OnInitialized()
    {
        _data = Resolver!.Resolve<CrazyEightsVMData>();
        base.OnInitialized();
    }
}