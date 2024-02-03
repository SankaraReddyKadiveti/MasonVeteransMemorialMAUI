using System;
using System.IO;
using System.Threading.Tasks;

namespace MasonVeteransMemorial
{
    public interface IPicturePicker
    {
        Task<Stream> GetImageStreamAsync();
    }
}