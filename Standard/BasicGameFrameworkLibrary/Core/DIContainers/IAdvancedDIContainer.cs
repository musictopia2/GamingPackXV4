namespace BasicGameFrameworkLibrary.Core.DIContainers;
public interface IAdvancedDIContainer
{
    IGamePackageResolver? MainContainer { get; set; }
    IGamePackageGeneratorDI? GeneratorContainer { get; set; }
}