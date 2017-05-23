using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly:Dependency(typeof(HP41CV.Helpers.MockMarketPlace))]
namespace HP41CV.Helpers
{
    public class MockMarketPlace : IPlatformMarketPlace
    {
        private static Collection<string> activatedOptions = null;
        public Collection<string> ActivatedOptions => activatedOptions;

        public async Task<Dictionary<string, Tuple<string, string, bool>>> GetOptions()
        {
            var result = new Dictionary<string, Tuple<string, string, bool>>();
            return result;
        }

        public async Task<bool> BuyOption(string option)
        {
            return false;
        }
    }
}
