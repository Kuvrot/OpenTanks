using Stride.Engine;
using Stride.Audio;
using System;

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
                if (new Random().Next(0 , 99) >= 33)
                {
                    soundInstace.Pitch = 0.7f;
                }
                else if (new Random().Next(0, 99) >= 33 + 33)
                {
                    soundInstace.Pitch = 0.5f;
                }
                soundInstace.Play();
           }
        }
    }
}
