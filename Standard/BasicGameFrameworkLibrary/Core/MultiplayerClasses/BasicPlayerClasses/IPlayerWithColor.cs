namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.BasicPlayerClasses;
public interface IPlayerWithColor<E> : IPlayerColorChoice
    where E : IFastEnumColorSimple
{
    E? Color { get; set; }
}