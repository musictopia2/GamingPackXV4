namespace GameLoaderBlazorLibrary;
public interface ILoaderVM
{
    string GameName { get; set; }
    RenderFragment? GameRendered { get; }
    Action? StateChanged { get; set; } //so components can call into it.
    string Title { get; }
    void ChoseGame(string game);
    BasicList<string> GameList { get; }
}