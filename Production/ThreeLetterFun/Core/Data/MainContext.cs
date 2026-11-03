
namespace ThreeLetterFun.Core.Data;
internal class MainContext : SerializeContext
{
    protected override void Configure(ISerializeConfig config)
    {
        config.Make<BasicList<SavedCard>>()
            .Make<BasicList<SavedTile>>()
            .Make<BasicList<WordInfo>>();
    }
}