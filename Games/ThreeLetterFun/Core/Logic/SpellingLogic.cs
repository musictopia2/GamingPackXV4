namespace ThreeLetterFun.Core.Logic;
public class SpellingLogic : ISpellingLogic
{
    BasicList<WordInfo>? _words;
    async Task<BasicList<WordInfo>> ISpellingLogic.GetWordsAsync(EnumDifficulty? difficulty, int? letters)
    {
        if (_words is null)
        {
            _words = await Resources.SavedWordList.GetResourceAsync<BasicList<WordInfo>>();
        }
        return _words.ToBasicList();
    }
}