namespace CrazyEights.Blazor.Views;
public partial class ChooseSuitView
{
    private CrazyEightsVMData? _data;
    protected override void OnInitialized()
    {
        _data = aa1.Resolver!.Resolve<CrazyEightsVMData>();
        base.OnInitialized();
    }
}