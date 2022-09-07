namespace BasicGameFrameworkLibrary.Core.CommonInterfaces;
public interface IPopulateObject<T> where T : IConvertible
{
    void Populate(T chosen);
}