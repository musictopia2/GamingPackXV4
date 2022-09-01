namespace Fluxx.Blazor.Views;
public class KeeperExchangeView : KeeperProcessView<KeeperExchangeViewModel>
{
    protected override ICustomCommand? Command => DataContext!.ProcessCommand!;
}