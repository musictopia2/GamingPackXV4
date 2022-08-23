namespace BasicGameFrameworkLibrary.Core.DIContainers;

public interface IAdvancedDIContainer
{
    IGamePackageResolver? MainContainer { get; set; }
    IGamePackageGeneratorDI? GeneratorContainer { get; set; } //i think this may be needed as well.
}