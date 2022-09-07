namespace BasicGameFrameworkLibrary.Core.DIContainers;
public interface IGamePackageResolver : IIgnoreSerialize
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Tag">This is extra info so it can more easily return the proper implementation  its an object so can represent anything</param>
    /// <returns></returns>
    T Resolve<T>(string tag);
    /// <summary>
    /// This is used in cases where the object was replaced.
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="NewObject"></param>
    void ReplaceObject<T>(T newObject);

    /// <summary>
    /// This is used in cases where the object was replaced.
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="NewObject"></param>
    void ReplaceObject<T>(T newObject, string tag);

    /// <summary>
    /// If the tag looking up is not found, then it returns false
    /// </summary>
    /// <param name="Tag"></param>
    /// <returns></returns>
    bool LookUpValue(string tag);
    bool RegistrationExist<T>(string tag);

    T ReplaceObject<T>();
    /// <summary>
    /// this is used in cases where you not only replace the game but also reset other misc objects since it relies on new information now.
    /// something else is responsible for figuring out the list.
    /// the purpose of this is so when something else asks for it, has the proper data.
    /// </summary>
    /// <param name="list"></param>
    void ResetSeveralObjects(BasicList<Type> list);
    Type? LookUpType<T>(); //has to be combined since i don't have a di container for most cases.
    object GetInstance(Type type);
    void RegisterInstanceType(Type type);
    bool RegistrationExist<T>();
    T Resolve<T>();
    T Resolve<T>(object tag);
}