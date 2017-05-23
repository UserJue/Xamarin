using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace HP41CV.Helpers
{
    public interface IPlatformMarketPlace
    {
        Collection<string> ActivatedOptions { get; }
        //              Key         Name/Title Price  activ
        Task<Dictionary<string, Tuple<string, string, bool>>> GetOptions();
        Task<bool> BuyOption(string option);
    }
}
