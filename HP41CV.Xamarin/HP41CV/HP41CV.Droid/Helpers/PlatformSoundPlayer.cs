using HP41CV.Helpers;
using Xamarin.Forms;

[assembly: Dependency(typeof(HP41CV.Droid.Helpers.PlatformSoundPlayer))]

namespace HP41CV.Droid.Helpers
{
    public class PlatformSoundPlayer : IPlatformSoundPlayer
    {
        public void PlaySound(int samplingRate, byte[] byteBuffer)
        {
            ;
        }
    }
}