namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.InterfaceModels;
public interface IDominoGamesData<D> : IViewModelData, IBasicEnableProcess, IEnableAlways
    where D : IDominoInfo, new()
{
    Func<D, Task>? DrewDominoAsync { get; set; }
    HandObservable<D> PlayerHand1 { get; set; }
    DominosBoneYardClass<D> BoneYard { get; set; }
    Func<Task>? PlayerBoardClickedAsync { get; set; }
    Func<D, int, Task>? HandClickedAsync { get; set; }
}