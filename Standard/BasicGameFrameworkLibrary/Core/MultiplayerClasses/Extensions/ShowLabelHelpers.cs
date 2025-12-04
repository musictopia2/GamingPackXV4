namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.Extensions;
public static class ShowLabelHelpers
{
    extension <P>(IBasicGameProcesses<P> game)
        where P : class, IPlayerItem, new()
    {
        public void ShowTurn()
        {
            if (game.CurrentMod == null)
            {
                return;
            }
            if (game.SingleInfo == null)
            {
                return;
            }
            game.SingleInfo = game.PlayerList!.GetWhoPlayer();
            game.CurrentMod.NormalTurn = game.SingleInfo.NickName;
        }
        public void StartingStatus()
        {
            if (game.CurrentMod == null)
            {
                return;
            }
            if (game.BasicData!.MultiPlayer == true)
            {
                game.CurrentMod.Status = "Multiplayer game in progress";
            }
            else
            {
                game.CurrentMod.Status = "Single player game in progress";
            }
        }
        public void ProtectedShowTie()
        {
            game.CurrentMod.NormalTurn = "None";
            game.CurrentMod.Status = "Game Over.  It was a tie";
            game.Toast.ShowInfoToast("It was a tie");
        }
        public void ProtectedShowWin()
        {
            game.CurrentMod.NormalTurn = "None";
            game.CurrentMod.Status = $"Game over.  {game.SingleInfo!.NickName} has won";
            CommandContainer command = Resolver!.Resolve<CommandContainer>();
            command.UpdateAll();
            if (game.BasicData!.MultiPlayer == false)
            {
                game.Toast.ShowInfoToast($"{game.SingleInfo.NickName} has won");
            }
            else if (game.SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
            {
                game.Toast.ShowSuccessToast($"{game.SingleInfo.NickName} wins the game");
            }
            else
            {
                game.Toast.ShowWarningToast("You lose the game");
            }
        }
        public void ProtectedShowCustomWin(string playersWonMessage)
        {
            game.CurrentMod.NormalTurn = "None";
            game.CurrentMod.Status = $"Game Over.  {playersWonMessage} has won"; //this time, no messagebox.
        }
        public void ProtectedShowLoss() //this is for games like old maid.
        {
            game.CurrentMod.NormalTurn = "None";
            game.CurrentMod.Status = $"Game over.  {game.SingleInfo!.NickName} has lost";
            game.Toast.ShowInfoToast($"{game.SingleInfo.NickName} is a loser");
        }
    }
    
}