namespace ItalianDominos.Core.Data;
[SingletonGame]
public class ItalianDominosSaveInfo : BasicSavedDominosClass<SimpleDominoInfo, ItalianDominosPlayerItem>, IMappable, ISaveInfo
{
    private int _upTo;
    public int UpTo
    {
        get { return _upTo; }
        set
        {
            if (SetProperty(ref _upTo, value))
            {
                ItalianDominosVMData model = aa1.Resolver!.Resolve<ItalianDominosVMData>(); //other choice is to have the extra function there.
                model.UpTo = value;
            }
        }
    }
    private int _nextNumber;
    public int NextNumber
    {
        get { return _nextNumber; }
        set
        {
            if (SetProperty(ref _nextNumber, value))
            {
                ItalianDominosVMData model = aa1.Resolver!.Resolve<ItalianDominosVMData>();
                model.NextNumber = value;
            }
        }
    }
}