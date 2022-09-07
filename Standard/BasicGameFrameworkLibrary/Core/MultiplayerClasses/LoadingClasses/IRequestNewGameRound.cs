namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.LoadingClasses;
/// <summary>
/// this is everything that shows you are requesting new round or new game.
/// 
/// </summary>
public interface IRequestNewGameRound
{
    Task RequestNewGameAsync();
    Task RequestNewRoundAsync();
}