namespace BasicGameFrameworkLibrary.Core.BasicEventModels;

public class AnimateCardInPileEventModel<D> where D : class, IDeckObject, new()
{
    public EnumAnimcationDirection Direction { get; set; }
    public D? ThisCard { get; set; }
    public BasicPileInfo<D>? ThisPile { get; set; }
}