namespace BasicGameFrameworkLibrary.Core.GeneratorHelpers;
public abstract class SerializeContext
{
    public const string ConfigureName = nameof(Configure);
    protected abstract void Configure(ISerializeConfig config);
}