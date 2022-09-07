namespace BasicGameFrameworkLibrary.Core.RegularDeckOfCards;
public interface IGenerateRegularCard<R> where R : IRegularCard
{
    R GetRegularCard(int chosen);
}