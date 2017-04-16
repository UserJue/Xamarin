using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HP41CV.Helpers
{
    public class MarketPlace
    {
        public static async Task<Dictionary<string, Tuple<string, string, bool>>> GetOptions()
        {
            var task = DependencyService.Get<IPlatformMarketPlace>().GetOptions().ConfigureAwait(true);
            await task;
            return task.GetAwaiter().GetResult();
        }

        public static bool IsAvtivated(string option)
        {
            if (string.IsNullOrEmpty(option)) return true;
            var activatedOptions = DependencyService.Get<IPlatformMarketPlace>().ActivatedOptions;
            if (activatedOptions == null) return false;
            if (option == "BaseProgramming")
                return activatedOptions.Contains("Programming") ||
                       activatedOptions.Contains("RobustStatistic");
            return activatedOptions.Contains(option);
        }


        public static async Task<bool> BuyOption(string option)
        {
            var task = DependencyService.Get<IPlatformMarketPlace>().BuyOption(option).ConfigureAwait(true);
            await task;
            return task.GetAwaiter().GetResult();
        }
    }
}
