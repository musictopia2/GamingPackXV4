namespace BasicGameFrameworkLibrary.Core.DIContainers;
public interface IGamePackageRegister
{
    void RegisterSingleton<TIn, TOut>() where TOut : TIn;
    void RegisterSingleton(Type thisType);
    void RegisterSingleton<TIn, TOut>(string tag);
    void RegisterSingleton(Type thisType, string tag);
    void RegisterType<TIn>(bool isSingleton = true); //has to be here so source generator can pick it up.
    void RegisterTrue(string tag);
    void RegisterInstanceType(Type type);
    /// <summary>
    /// what should happen is if the control is already registered, then replace.  otherwise, simply register.
    /// </summary>
    /// <param name="control"></param>
    /// <param name="tag"></param>
    void RegisterControl(object control, string tag);
}