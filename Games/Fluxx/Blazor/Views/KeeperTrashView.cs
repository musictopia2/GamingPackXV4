namespace Fluxx.Blazor.Views;
public class KeeperTrashView : KeeperProcessView<KeeperTrashViewModel>
{
    protected override ICustomCommand? Command => DataContext!.ProcessCommand!;
}