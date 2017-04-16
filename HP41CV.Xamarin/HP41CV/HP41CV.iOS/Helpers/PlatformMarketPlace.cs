using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using HP41CV.Helpers;

namespace HP41CV.iOS.Helpers
{
    public class PlatformMarketPlace : IPlatformMarketPlace
    {
        public Collection<string> ActivatedOptions { get; }

        public Task<Dictionary<string, Tuple<string, string, bool>>> GetOptions()
        {
            return new Task<Dictionary<string, Tuple<string, string, bool>>>(() => new Dictionary<string, Tuple<string, string, bool>>());
        }

        public Task<bool> BuyOption(string option)
        {
            return new Task<bool>(() => false);
        }
    }
}
