namespace BasicGameFrameworkLibrary.Core.Extensions;
public static class BasicDataExtensions
{
    extension (BasicData basicData)
    {
        public IGameNetwork? GetNetwork()
        {
            if (basicData.MultiPlayer == false)
            {
                return null;
            }
            return Resolver!.Resolve<IGameNetwork>();
        }
    }   
}