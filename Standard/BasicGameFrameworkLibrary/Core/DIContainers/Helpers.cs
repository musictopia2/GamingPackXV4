namespace BasicGameFrameworkLibrary.Core.DIContainers;

public static class Helpers
{
    public static IGamePackageResolver? Resolver { get; set; }
    public static void PopulateContainer(IAdvancedDIContainer thisMain) //this is probably the best thing to do.
    {

        if (thisMain.MainContainer is not null)
        {
            return;
        }
        if (Resolver is null)
        {
            throw new CustomBasicException("Never populated the di container");
        }
        thisMain.MainContainer = Resolver;
        if (thisMain.MainContainer is IGamePackageGeneratorDI di)
        {
            thisMain.GeneratorContainer = di;
        }
        else
        {
            throw new CustomBasicException("Never populated the container that implemented the IGamePackageGeneratorDI");
        }
    }
}