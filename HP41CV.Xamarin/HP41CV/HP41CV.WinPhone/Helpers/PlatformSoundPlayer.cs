using System.Reflection;
using HP41CV.Helpers;
using Xamarin.Forms;

[assembly: Dependency(typeof(HP41CV.WinPhone.Helpers.PlatformSoundPlayer))]

namespace HP41CV.WinPhone.Helpers
{
    public class PlatformSoundPlayer : IPlatformSoundPlayer
    {
        public void PlaySound(int samplingRate, byte[] byteBuffer)
        {
            ;
        }
    }
}
