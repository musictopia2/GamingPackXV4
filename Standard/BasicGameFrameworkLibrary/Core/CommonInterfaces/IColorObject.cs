namespace BasicGameFrameworkLibrary.Core.CommonInterfaces;

public interface IColorObject<E> where E : IFastEnumColorSimple
{
    E GetColor { get; }
}