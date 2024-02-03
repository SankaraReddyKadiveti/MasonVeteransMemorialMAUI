using System;
namespace MasonVeteransMemorial.BusinessServices.Base
{
    public abstract class BaseService<T, TInterface> : SingletonBase<T, TInterface> where T : TInterface, new() where TInterface : class
    {
    }
}
