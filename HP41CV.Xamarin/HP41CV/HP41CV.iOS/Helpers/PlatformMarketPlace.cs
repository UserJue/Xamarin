using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using HP41CV.Helpers;
using Xamarin.Forms;

[assembly: Dependency(typeof(HP41CV.iOS.Helpers.PlatformMarketPlace))]

namespace HP41CV.iOS.Helpers
{
    public class PlatformMarketPlace : IPlatformMarketPlace
    {
        public Collection<string> ActivatedOptions { get; }

        public Task<Dictionary<string, Tuple<string, string, bool>>> GetOptions()
        {
			var result = new Task<Dictionary<string, Tuple<string, string, bool>>>(() => new Dictionary<string, Tuple<string, string, bool>>());
			result.Start();
            return result;
        }

        public Task<bool> BuyOption(string option)
        {
            var result = new Task<bool>(() => false);
			result.Start();
            return result;
        }
    }
}
