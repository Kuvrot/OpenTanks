using Stride.Engine;
using Stride.Audio;

namespace OpenTanks.Core
{
    public class SoundManager : SyncScript
    {
        public override void Start()
        {
            // Initialization of the script.
        }

        public override void Update()
        {
            // Do stuff every new frame
        }

        public void PlaySound(Sound sound)
        {
           if (sound != null)
           {
            var soundInstace = sound.CreateInstance();
            soundInstace.Play();
           }
        }
    }
}
