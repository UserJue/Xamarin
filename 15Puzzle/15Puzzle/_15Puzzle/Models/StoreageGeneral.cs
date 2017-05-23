using System;
using System.Collections.Generic;
using Common.Models;
using Xamarin.Forms;

[assembly:Dependency(typeof(_15Puzzle.Models.StoreageGeneral))]

namespace _15Puzzle.Models
{
    public class StoreageGeneral : IStoreage
    {
        private readonly IDictionary<string, object> propertiesDictionary;
        public StoreageGeneral()
        {
            propertiesDictionary = Application.Current.Properties;

        }

        public bool GetValue(string key, out int value)
        {
            value = 0;
            if (!propertiesDictionary.ContainsKey(key)) return false;
            int? intValue;
            intValue = propertiesDictionary[key] as int?;
            if (intValue == null) return false;
            value = intValue.Value;
            return true;
        }

        public bool GetValue(string key, out double value)
        {
            value = double.NaN;
            if (!propertiesDictionary.ContainsKey(key)) return false;
            double? doubleValue;
            doubleValue = propertiesDictionary[key] as double?;
            if (doubleValue == null) return false;
            value = doubleValue.Value;
            return true;
        }

        public bool GetValue(string key, out string value)
        {
            value = string.Empty;
            if (!propertiesDictionary.ContainsKey(key)) return false;
            value = propertiesDictionary[key] as string;
            return true;
        }

        public void SetValue(string key, int value)
        {
            propertiesDictionary[key] = value;
        }

        public void SetValue(string key, string value)
        {
            propertiesDictionary[key] = value;
        }

        public string ReadBuffer(string account, string path, string name)
        {
            throw new NotImplementedException();
        }

        public bool WriteBuffer(string account, string path, string name, string buffer)
        {
            throw new NotImplementedException();
        }
    }
}
