namespace BasicGameFrameworkLibrary.Core.TestUtilities;
public class TestContext : SerializeContext
{
    protected override void Configure(ISerializeConfig config)
    {
        config.Make<TestOptions>();
    }
}