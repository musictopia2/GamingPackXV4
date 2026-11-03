namespace LifeBoardGame.Core.ViewModels;
[InstanceGame]
[UseLabelGrid]
public partial class ChooseGenderViewModel(CommandContainer commandContainer, LifeBoardGameVMData model, IGenderProcesses processes, LifeBoardGameGameContainer gameContainer) : IPromptVM, IStateChanged
{
    [LabelColumn]
    public partial string Turn { get; set; } = "";
    [LabelColumn]
    public partial string Instructions { get; set; } = "";
    public LifeBoardGamePlayerItem? GetPlayer => gameContainer.SingleInfo;
    public SimpleEnumPickerVM<EnumGender> GetGenderPicker => model.GenderChooser;
    public event Action? StateChanged;
    private async Task GenderChooser_ItemClickedAsync(EnumGender piece)
    {
        await processes.ChoseGenderAsync(piece);
    }
    void IPromptVM.Initialize()
    {
        gameContainer.SelectGenderAsync = processes.ChoseGenderAsync;
        processes.SetInstructions = x => Instructions = x;
        processes.RefreshPlayer = OnStateChanged;
        processes.SetTurn = x => Turn = x; //has to set delegates before init obviously.
        model.GenderChooser.ItemClickedAsync = GenderChooser_ItemClickedAsync;
    }
    public CommandContainer CommandContainer { get; set; } = commandContainer;
    private void OnStateChanged()
    {
        StateChanged?.Invoke();
    }
}