namespace HP41CV.Helpers
{
    public interface IPlatformSoundPlayer
    {
        void PlaySound(int samplingRate, byte[] byteBuffer);
    }
}
