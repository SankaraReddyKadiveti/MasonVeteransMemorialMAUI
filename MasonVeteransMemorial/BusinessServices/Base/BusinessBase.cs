using System;
using System.Threading.Tasks;

namespace MasonVeteransMemorial.BusinessServices.Base
{
    public interface IBaseBusinessManager
    {
        Task<TData> GetDataFromDevice<TData>(string id) where TData : class, new();
    }
    public abstract class BaseBusinessManager<T, TInterface> : SingletonBase<T, TInterface>, IBaseBusinessManager where T : TInterface, new() where TInterface : class
    {
        public async Task<TData> GetDataFromDevice<TData>(string id) where TData : class, new()
        {
            var data = await Task.Run(() => { return new TData(); }); //await SecureRepository.Current.GetAsync<TData>(id);
            return new TData();
        }
    }
}
