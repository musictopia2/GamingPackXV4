namespace BasicGameFrameworkLibrary.Core.DIContainers;
public class GamePackageDIContainer : IGamePackageResolver, IGamePackageRegister, IGamePackageDIContainer, IGamePackageGeneratorDI
{
    private readonly HashSet<ContainerData> _thisSet = new();
    private readonly object _lockObj = new();
    private readonly HashSet<string> _trueList = new();
    private readonly IRandomGenerator _rans;
    public GamePackageDIContainer()
    {
        _rans = RandomHelpers.GetRandomGenerator();
    }
    public bool LookUpValue(string tag)
    {
        lock (_lockObj)
        {
            return _trueList.Contains(tag.ToLower());
        }
    }
    public bool RegistrationExist<T>(string tag)
    {
        lock (_lockObj)
        {
            Type thisType = typeof(T);
            return _thisSet.Any(xx => xx.CanAssignFrom(thisType) && xx.Tag == tag);
        }
    }
    public bool RegistrationExist<T>()
    {
        return RegistrationExist<T>(""); //has to be no tag period this time.
    }
    private void SetResults(ContainerData thisResults, string tag)
    {
        if (_thisSet.Any(x => x.TypeIn == thisResults.TypeIn && x.Tag == tag))
        {
            return;
        }
        thisResults.Tag = tag;
        _thisSet.Add(thisResults);
    }
    public void RegisterInstanceType(Type type)
    {
        //this needs to capture the instance type.
        ContainerData thisResults = new()
        {
            IsSingle = false,
            TypeOut = type,
            TypeIn = type
        };
        SetResults(thisResults, "");
    }
    public void RegisterType<TIn>(bool isSingleton = true) //i think if you want to register a type, you are not allowed to use a factory.
    {
        ContainerData thisResults = new()
        {
            IsSingle = isSingleton,
            TypeOut = typeof(TIn),
            TypeIn = typeof(TIn)
        };
        SetResults(thisResults, "");
    }
    void IGamePackageRegister.RegisterSingleton(Type thisType)
    {
        ContainerData thisResults = new()
        {
            TypeOut = thisType,
            TypeIn = thisType,
            IsSingle = true
        };
        SetResults(thisResults, "");
    }
    public void RegisterSingleton<TIn, TOut>() where TOut : TIn
    {
        ContainerData results = new()
        {
            IsSingle = true,
            TypeOut = typeof(TOut), //i think
            TypeIn = typeof(TIn)
        };
        SetResults(results, "");
    }
    public void RegisterSingleton<TIn, TOut>(string tag)
    {
        ContainerData thisResults = new()
        {
            IsSingle = true,
            TypeOut = typeof(TOut),
            TypeIn = typeof(TIn)
        };
        SetResults(thisResults, tag);
    }
    public void RegisterStartup(IStartUp start)
    {
        if (start is null)
        {
            throw new CustomBasicException("You have to have the object already created for IStartup");
        }
        ContainerData results = new()
        {
            IsSingle = true,
            TypeIn = typeof(IStartUp),
            TypeOut = typeof(IStartUp),
            ThisObject = start
        };
        results.AssignedFrom.Add(typeof(IStartUp));
        SetResults(results, "");
    }
    public void RegisterSingleton<TIn>(TIn ourObject, string tag = "")
    {
        if (ourObject == null)
        {
            throw new CustomBasicException($"You can't register an object that does not exist.  Most likely, you tried to register it too soon.  The type was {typeof(TIn)}");
        }
        ContainerData thisResults = new()
        {
            IsSingle = true,
            TypeIn = typeof(TIn),
            TypeOut = ourObject.GetType(),
            ThisObject = ourObject
        };
        SetResults(thisResults, tag);
    }
    public void RegisterSingleton(Type thisType, string tag = "") //i like this shows up last.  perfect for source generation.
    {
        ContainerData thisResults = new()
        {
            IsSingle = true,
            TypeOut = thisType,
            TypeIn = thisType
        };
        SetResults(thisResults, tag);
    }
    public void RegisterTrue(string tag)
    {
        if (_trueList.Contains(tag.ToLower()) == true)
        {
            throw new CustomBasicException($"{tag} was already registered as true.  Maybe ignore.  Rethink");
        }
        _trueList.Add(tag.ToLower());
    }
    public void ReplaceObject<T>(T newObject)
    {
        ReplaceObject(newObject, "");
    }
    public void ReplaceObject<T>(T newObject, string tag)
    {
        lock (_lockObj)
        {
            Type thisType = newObject!.GetType();
            try
            {
                ContainerData thisData = _thisSet.Where(xx => xx.CanAssignFrom(thisType) && xx.IsSingle == true && xx.Tag == tag).Single();
                thisData.ThisObject = newObject;
            }
            catch (Exception ex)
            {
                throw new CustomBasicException($"Unable to replace object.  The type you were trying to replace is {thisType.Name}.  Error was {ex.Message}");
            }
        }
    }
    public void ReplaceRegistration<TIn, TOut>() //i think if we are replacing, try with no tags.
    {
        lock (_lockObj)
        {
            Type thisType = typeof(TIn);
            List<ContainerData> tempList = _thisSet.Where(xx => xx.CanAssignFrom(thisType)).ToList();
            if (tempList.Count == 0)
            {
                throw new CustomBasicException($"Nothing registered with requesting type of {thisType.Name} to replace registration");
            }
            if (tempList.Count > 1)

            {
                tempList = tempList = _thisSet.Where(Items => Items.TypeIn == thisType).ToList();
                if (tempList.Count == 0)
                {
                    throw new CustomBasicException($"Nothing registered with requesting type of {thisType.Name} when attempting to use in for replacing registrations");
                }
                else if (tempList.Count > 1)
                {
                    throw new CustomBasicException($"Was a duplicate.  Registered {tempList.Count} for requesting type of {thisType.Name}  Happened even when using in argument.  Was trying to replace registration");
                }
            }
            ContainerData thisData = tempList.Single();
            thisData.ThisObject = null;
        }
    }
    public void DeleteRegistration<TIn>()
    {
        lock (_lockObj)
        {
            Type thisType = typeof(TIn);
            BasicList<ContainerData> tempList = _thisSet.Where(xx => xx.CanAssignFrom(thisType)).ToBasicList();
            tempList.ForEach(Items =>
            {
                _thisSet.Remove(Items);
            });
        }
    }
    public object GetInstance(Type thisType)
    {
        return GetInstance(thisType, false);
    }
    private object GetInstance(Type thisType, bool needsReplacement = false) //needs to be public to implement interface.
    {
        if (thisType == typeof(IRandomGenerator))
        {
            return _rans;
        }
        BasicList<ContainerData> tempList = _thisSet.Where(x => x.CanAssignFrom(thisType) && x.Tag == "").ToBasicList();
        if (tempList.Count == 0)
        {
            throw new CustomBasicException($"Nothing registered for parameters with type of {thisType.Name}");
        }
        if (tempList.Count > 1)
        {
            throw new CustomBasicException($"Was a duplicate.  Registered {tempList.Count} for parameters of type of {thisType.Name}");
        }
        ContainerData thisData = tempList.Single();
        if (thisData.IsSingle == true && thisData.ThisObject != null && needsReplacement == false)
        {
            return thisData.ThisObject;
        }
        if (thisData.IsSingle == true && needsReplacement == false)
        {
            if (thisData.GetNewObject == null)
            {
                throw new CustomBasicException("Must have a custom function");
            }
            thisData.ThisObject = thisData.GetNewObject();
            return thisData.ThisObject;
        }
        if (thisData.GetNewObject == null)
        {
            throw new CustomBasicException("Must have a custom function");
        }
        if (needsReplacement == false)
        {
            return thisData.GetNewObject();
        }
        object output = thisData.GetNewObject();
        return output;
    }
    public T Resolve<T>(object tag)
    {
        return Resolve<T>(tag.ToString()!);
    }
    public T Resolve<T>(string tag)
    {
        if (typeof(T) == typeof(IRandomGenerator)) //this is an exception.
        {
            object thisObj = _rans;
            return (T)thisObj;
        }
        lock (_lockObj)
        {
            Type thisType = typeof(T);
            List<ContainerData> tempList = _thisSet.Where(xx => xx.CanAssignFrom(thisType) && xx.Tag == tag).ToList();
            if (tempList.Count == 0)
            {
                throw new CustomBasicException($"Nothing registered requesting type of {thisType.Name}");
            }
            if (tempList.Count > 1)
            {
                tempList = _thisSet.Where(xx => xx.TypeIn == thisType && xx.Tag == tag).ToList();
                if (tempList.Count == 0)
                {
                    throw new CustomBasicException($"Nothing registered requesting type of {thisType.Name} when attempting to use in");
                }
                else if (tempList.Count > 1)
                {
                    throw new CustomBasicException($"Was a duplicate.  Registered {tempList.Count} for  requesting type of {thisType.Name}  Happened even when using in argument");
                }
            }
            ContainerData thisData = tempList.Single();
            if (thisData.IsSingle == true && thisData.ThisObject != null)
            {
                return (T)thisData.ThisObject;
            }
            if (thisData.IsSingle == true)
            {
                if (thisData.GetNewObject == null)
                {
                    throw new CustomBasicException("Must have a custom function");
                }
                thisData.ThisObject = thisData.GetNewObject();
                return (T)thisData.ThisObject;
            }
            if (thisData.GetNewObject == null)
            {
                throw new CustomBasicException("Must have a custom function");
            }
            return (T)thisData.GetNewObject();
        }
    }
    public T Resolve<T>()
    {
        return Resolve<T>("");
    }
    public T ReplaceObject<T>()
    {
        Type type = typeof(T);

        T output = (T)GetInstance(type, true);
        ReplaceObject(output);
        return output;
    }
    public void ResetSeveralObjects(BasicList<Type> list)
    {
        list.ForEach(type =>
        {
            BasicList<ContainerData> tempList = _thisSet.Where(x => x.CanAssignFrom(type) && x.Tag == "" && x.IsSingle == true).ToBasicList();
            if (tempList.Count == 0)
            {
                throw new CustomBasicException($"The type of {type.Name} was never registered or was not singleton.  Rethink");
            }
            if (tempList.Count > 1)
            {
                throw new CustomBasicException($"The type of {type.Name} was registered more than once.  Rethink");
            }
            var output = tempList.Single();
            output.ThisObject = null; //just set to null so when it asks for it again, will be fine.
        });
    }
    public void RegisterControl(object control, string tag)
    {
        lock (_lockObj)
        {
            Type type = control.GetType();
            var container = _thisSet.SingleOrDefault(x => x.CanAssignFrom(type) && x.Tag == tag);
            if (container != null)
            {
                container.ThisObject = control;
                return;
            }
        }
        RegisterSingleton(control, tag);
    }
    public Type? LookUpType<T>()
    {
        Type type = typeof(T);
        var possResults = _thisSet.Where(x => x.CanAssignFrom(type)).ToBasicList();
        if (possResults.Count == 0)
        {
            return null; //go ahead and return null.  more efficient to just return null and let whoever requests it do something with it.
        }
        if (possResults.Count > 1)
        {
            throw new CustomBasicException($"Duplicate registration for {type.Name}");
        }
        return possResults.Single().TypeOut!;
    }
    public object LaterGetObject(Type type, bool needsReplacement)
    {
        if (type == typeof(IRandomGenerator))
        {
            return _rans;
        }
        BasicList<ContainerData> tempList = _thisSet.Where(xx => xx.CanAssignFrom(type)).ToBasicList();
        if (tempList.Count == 0)
        {
            throw new CustomBasicException($"Nothing registered for parameters with type of {type.Name}");
        }
        if (tempList.Count > 1)
        {
            throw new CustomBasicException($"Was a duplicate.  Registered {tempList.Count} for parameters of type of {type.Name}");
        }
        ContainerData thisData = tempList.Single();
        if (thisData.IsSingle == true && thisData.ThisObject != null && needsReplacement == false)
        {
            return thisData.ThisObject;
        }
        if (thisData.IsSingle == true && needsReplacement == false)
        {
            if (thisData.GetNewObject == null)
            {
                throw new CustomBasicException("Must have a custom function");
            }
            thisData.ThisObject = thisData.GetNewObject();
            return thisData.ThisObject;
        }
        if (thisData.GetNewObject == null)
        {
            throw new CustomBasicException("Must have a custom function");
        }
        if (needsReplacement == false)
        {
            return thisData.GetNewObject();
        }
        object output = thisData.GetNewObject();
        return output;
    }
    public void LaterRegister(Type type, BasicList<Type> assignedFrom, Func<object> action, string tag)
    {
        var item = _thisSet.SingleOrDefault(x => x.TypeOut == type && x.Tag == tag);
        if (item is null)
        {
            return; //ignore because was not used.  this means it could be used but does not mean it will be used.
        }
        item.AssignedFrom = assignedFrom;
        item.GetNewObject = action;
    }

    public void LaterRegister(Type type, BasicList<Type> assignedFrom, string tag = "")
    {
        var item = _thisSet.SingleOrDefault(x => x.TypeIn == type && x.Tag == tag); //try this way (?)
        if (item is null)
        {
            return; //ignore because was not used.  this means it could be used but does not mean it will be used.
        }
        item.AssignedFrom = assignedFrom;
    }
}