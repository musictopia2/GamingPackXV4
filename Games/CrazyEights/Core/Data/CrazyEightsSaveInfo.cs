namespace CrazyEights.Core.Data;
[SingletonGame]
public class CrazyEightsSaveInfo : BasicSavedCardClass<CrazyEightsPlayerItem, RegularSimpleCard>, IMappable, ISaveInfo
{
    private bool _chooseSuit;
    public bool ChooseSuit
    {
        get { return _chooseSuit; }
        set
        {
            if (SetProperty(ref _chooseSuit, value))
            {
                if (_model != null)
                {
                    _model.ChooseSuit = value;
                    if (_aggregator is not null)
                    {
                        _aggregator.Publish(new ChooseSuitEventModel());
                    }
                }
            }
        }
    }
    public EnumSuitList CurrentSuit { get; set; }
    public EnumRegularCardValueList CurrentNumber { get; set; }
    private CrazyEightsVMData? _model;
    private IEventAggregator? _aggregator;
    public void LoadMod(CrazyEightsVMData model)
    {
        _model = model;
        _aggregator = aa.Resolver!.Resolve<IEventAggregator>();
    }
}