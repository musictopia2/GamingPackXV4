namespace GolfCardGame.Core.ViewModels;
[InstanceGame]
public partial class FirstViewModel(
    CommandContainer commandContainer,
    GolfCardGameVMData model,
    GolfCardGameGameContainer gameContainer,
    IBeginningProcesses processes,
    IToast toast
        ) : IPromptVM
{
    public string Instructions { get; set; } = "";
    partial void CreateCommands(CommandContainer command);

    [Command(EnumCommandCategory.Plain)]
    public async Task ChooseFirstCardsAsync()
    {
        if (model.Beginnings1.CanContinue == false)
        {
            toast.ShowUserErrorToast("Sorry, must select 2 and only 2 cards to put into your hand");
            return;
        }
        model.Beginnings1.GetSelectInfo(out DeckRegularDict<RegularSimpleCard> selectList, out DeckRegularDict<RegularSimpleCard> unselectList);
        if (gameContainer.BasicData!.MultiPlayer == true)
        {
            SendBeginning thisB = new();
            thisB.SelectList = selectList;
            thisB.UnsSelectList = unselectList;
            thisB.Player = gameContainer.PlayerList!.Where(items => items.PlayerCategory == EnumPlayerCategory.Self).Single().Id;
            await gameContainer.Network!.SendAllAsync("selectbeginning", thisB);
        }
        int player = gameContainer.PlayerList!.Single(items => items.PlayerCategory == EnumPlayerCategory.Self).Id;
        await processes.SelectBeginningAsync(player, selectList, unselectList);
    }

    void IPromptVM.Initialize()
    {
        Instructions = "Choose the 2 cards to put into your hand";
        CreateCommands(commandContainer);
    }
}