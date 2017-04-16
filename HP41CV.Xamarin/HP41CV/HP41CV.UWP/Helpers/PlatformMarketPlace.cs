using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store;
using Windows.Storage;
using HP41CV.Helpers;
using Xamarin.Forms;

[assembly:Dependency(typeof(HP41CV.UWP.Helpers.PlatformMarketPlace))]

namespace HP41CV.UWP.Helpers
{
    public class PlatformMarketPlace : IPlatformMarketPlace
    {
        private static LicenseChangedEventHandler licenseChangeHandler = null;
        private static Collection<string> activatedOptions = null;

        public delegate void EventHandler(string e);

        public static event EventHandler OptionActivated;

        public Collection<string> ActivatedOptions => activatedOptions;

        Task<Dictionary<string, Tuple<string, string, bool>>> IPlatformMarketPlace.GetOptions()
        {
            return GetOptions();
        }

        Task<bool> IPlatformMarketPlace.BuyOption(string option)
        {
            return BuyOption(option);
        }

        public PlatformMarketPlace()
        {
            Init();
        }

        public static async Task Init()
        {
            if (activatedOptions != null) return;
            activatedOptions = new Collection<string>();
            await GetOptions();
            if (OptionActivated != null)
                foreach (var activatedOption in activatedOptions)
                {
                    OptionActivated.Invoke(activatedOption);
                }
        }

        private static async Task CheckInit()
        {
            if (licenseChangeHandler == null)
            {
                licenseChangeHandler = new LicenseChangedEventHandler(InAppPurchaseRefreshScenario);
#if DEBUG
                StorageFolder proxyDataFolder = await Package.Current.InstalledLocation.GetFolderAsync("data");
                StorageFile proxyFile = await proxyDataFolder.GetFileAsync("in-app-purchase.xml");
                CurrentAppSimulator.LicenseInformation.LicenseChanged += licenseChangeHandler;
                await CurrentAppSimulator.ReloadSimulatorAsync(proxyFile);
#else
                CurrentApp.LicenseInformation.LicenseChanged += licenseChangeHandler;
#endif
            }
        }

        private static async Task<LicenseInformation> getLicenseInformation()
        {
#if DEBUG
            await CheckInit();
            return CurrentAppSimulator.LicenseInformation;
#else
            return CurrentApp.LicenseInformation;
#endif
        }

        private static void InAppPurchaseRefreshScenario()
        {
            GetOptions();
        }

        public static async Task<bool> IsActive(string option)
        {
            var licenseInformation = await getLicenseInformation();
            return licenseInformation.ProductLicenses[option].IsActive;
        }

        private static async Task<Dictionary<string, Tuple<string, string, bool>>> GetOptions()
        {
            var result = new Dictionary<string, Tuple<string, string, bool>>();
            try
            {
                await CheckInit();
#if DEBUG
                ListingInformation listing = await CurrentAppSimulator.LoadListingInformationAsync();
#else
                ListingInformation listing = await CurrentApp.LoadListingInformationAsync();
#endif
                foreach (var item in listing.ProductListings)
                {
                    var activ = await IsActive(item.Key);
                    if (activatedOptions != null)
                    {
                        if (activ && !activatedOptions.Contains(item.Key))
                        {
                            activatedOptions.Add(item.Key);
                            if (OptionActivated != null)
                                OptionActivated.Invoke(item.Key);
                        }
                        else if (!activ && activatedOptions.Contains(item.Key))
                            activatedOptions.Remove(item.Key);
                    }
                    result.Add(item.Key, new Tuple<string, string, bool>(item.Value.Name, item.Value.FormattedPrice.Replace("0", string.Empty), activ));
                }
            }
            catch (Exception e)
            {
                ;
            }
            return result;
        }

        public static async Task<bool> BuyOption(string option)
        {
            var licenseInformation = await getLicenseInformation();
            try
            {
                if (!licenseInformation.ProductLicenses[option].IsActive)
                {
#if DEBUG
                    await CurrentAppSimulator.RequestProductPurchaseAsync(option);
#else
                    await CurrentApp.RequestProductPurchaseAsync(option);
#endif
                    await Init();
                    if (licenseInformation.ProductLicenses[option].IsActive && !activatedOptions.Contains(option))
                    {
                        activatedOptions.Add(option);
                        if (OptionActivated != null)
                            OptionActivated.Invoke(option);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return licenseInformation.ProductLicenses[option].IsActive;
        }
#if DEBUG
        public static void Activate(string option)
        {
            if (activatedOptions == null) 
                activatedOptions = new Collection<string>();
            if (activatedOptions.Contains(option)) return;
            activatedOptions.Add(option);
            if (OptionActivated != null)
                OptionActivated.Invoke(option);
        }
#endif
    }
}
