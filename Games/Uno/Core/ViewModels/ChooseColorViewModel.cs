namespace Uno.Core.ViewModels;
[InstanceGame]
public class ChooseColorViewModel(CommandContainer commandContainer, UnoVMData model, IChooseColorProcesses processes) : IPromptVM
{
    public readonly UnoVMData Model = model;
    private async Task ColorPicker_ItemClickedAsync(EnumColorTypes piece)
    {
        await processes!.ColorChosenAsync(piece);
    }
    void IPromptVM.Initialize()
    {
        Model.ColorPicker.ItemClickedAsync = ColorPicker_ItemClickedAsync;
    }
    public CommandContainer CommandContainer { get; set; } = commandContainer;
}