namespace BasicGameFrameworkLibrary.Core.GeneratorHelpers;

public interface IBeginningRegularCards<R>
    where R : IRegularCard
{
    bool AceLow { get; }
    bool CustomDeck { get; } //if customdeck is true, then you have to do manually (already in templates anyways).
}