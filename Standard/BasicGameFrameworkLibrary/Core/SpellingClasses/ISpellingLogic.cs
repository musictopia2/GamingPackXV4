namespace BasicGameFrameworkLibrary.Core.SpellingClasses;
public interface ISpellingLogic
{
    Task<BasicList<WordInfo>> GetWordsAsync(EnumDifficulty? difficulty, int? letters);
}