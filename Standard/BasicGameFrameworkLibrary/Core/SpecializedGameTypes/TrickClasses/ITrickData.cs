namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.TrickClasses;
public interface ITrickData
{
    bool FirstPlayerAnySuit { get; }
    bool FollowSuit { get; }
    bool MustFollow { get; }
    bool HasTrump { get; }
    bool MustPlayTrump { get; }
    EnumTrickStyle TrickStyle { get; }
    bool HasDummy { get; }
}