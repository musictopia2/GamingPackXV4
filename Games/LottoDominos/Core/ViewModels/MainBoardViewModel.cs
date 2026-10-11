//namespace LottoDominos.Core.ViewModels;
//[InstanceGame]
//public class MainBoardViewModel
//{
//    public MainBoardViewModel(CommandContainer commandContainer, LottoDominosMainGameClass mainGame)
//    {
//        if (mainGame.SaveRoot.GameStatus != EnumStatus.NormalPlay)
//        {
//            throw new CustomBasicException("Can't load the board view model when the status is not even normal play.  Rethink");
//        }
//        CommandContainer = commandContainer;
//    }
//    public CommandContainer CommandContainer { get; set; }
//}