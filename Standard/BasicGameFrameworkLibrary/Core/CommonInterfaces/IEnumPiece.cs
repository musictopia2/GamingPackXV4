namespace BasicGameFrameworkLibrary.Core.CommonInterfaces;
public interface IEnumPiece<E> : ISelectableObject, IEnabledObject where E : IFastEnumSimple
{
    E EnumValue { get; set; }
}