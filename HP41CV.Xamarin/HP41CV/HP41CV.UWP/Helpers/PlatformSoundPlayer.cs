using HP41CV.Helpers;
using Xamarin.Forms;

[assembly: Dependency(typeof(HP41CV.UWP.Helpers.PlatformSoundPlayer))]

namespace HP41CV.UWP.Helpers
{
    public class PlatformSoundPlayer : IPlatformSoundPlayer
    {
        public void PlaySound(int samplingRate, byte[] byteBuffer)
        {
            ;
        }
    }
}
