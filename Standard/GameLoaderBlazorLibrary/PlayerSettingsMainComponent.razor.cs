namespace GameLoaderBlazorLibrary;

public partial class PlayerSettingsMainComponent
{
    [Parameter]
    public EventCallback<EnumPlayerMode> OnPlayerSelectionChosen { get; set; }
    [Parameter]
    public EventCallback OnCancel { get; set; }


    private bool _showHelp = false;


    [Parameter]
    [EditorRequired]
    public EnumPlayerMode PlayerMode { get; set; }

    private void ShowHelp()
    {
        _showHelp = true;
    }

    //private void ChooseOption(EnumPlayerMode mode)
    //{
    //    OnPlayerSelectionChosen.
    //}



}