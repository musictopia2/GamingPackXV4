namespace BasicGameFrameworkLibrary.Core.Extensions;

public static class BasicDataExtensions
{
    public static IGameNetwork? GetNetwork(this BasicData basicData)
    {
        if (basicData.MultiPlayer == false)
        {
            return null;
        }
        return Resolver!.Resolve<IGameNetwork>();
    }
}