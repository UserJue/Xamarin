using System;

namespace Common.Models
{
    public interface IStoreage
    {
        bool GetValue(string key, out int value);
        bool GetValue(string key, out double value);
        bool GetValue(string key, out string value);
        void SetValue(string key, int value);
        void SetValue(string key, string value);
        string ReadBuffer(string account, string path, string name);
        bool WriteBuffer(string account, string path, string name, string buffer);
    }
}