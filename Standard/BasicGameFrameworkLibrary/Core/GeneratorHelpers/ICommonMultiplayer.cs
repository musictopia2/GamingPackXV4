namespace BasicGameFrameworkLibrary.Core.GeneratorHelpers;

public interface ICommonMultiplayer<P, S>
    where P : class, IPlayerItem, new()
    where S : BasicSavedGameClass<P>, new()
{
}