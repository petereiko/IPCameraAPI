using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraAPI.Business.Modules.Authentication
{
    public interface ICookieStore
    {
        void StoreStringData(string key, string data);

        bool GetBooleanData(string key);

        string GetStringData(string key);

        void StoreBooleanData(string key, bool data);

        void RemoveData(string key);
    }
}
