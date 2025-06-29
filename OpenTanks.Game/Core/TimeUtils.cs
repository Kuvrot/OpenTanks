using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;
using System.Windows.Media.Animation;

namespace OpenTanks.Core
{
    public class TimeUtils : SyncScript
    {
        private float timer = 0;
        private float clock = 0;

        #region singleton
        public static TimeUtils Instance;
        public override void Start()
        {
            Instance = this;
        }
        #endregion

        public override void Update()
        {
            timer += 1 * (float)Game.UpdateTime.Elapsed.TotalSeconds;
            clock += 1 * (float)Game.UpdateTime.Elapsed.TotalSeconds;

            if (timer > 99999f)
            {
                timer = 0;
            }
        }

        public float GetTime()
        {
            return timer;
        }

        //Returns when a second has passed
        public bool IsSecond()
        {
            if (clock >= 1f)
            {
                clock = 0;
                return true;
            }

            return false;
        }

        public bool HasSecondsPassed (float lastTime , float seconds)
        {
            if (timer - lastTime >= seconds)
            {
                return true;
            }

            return false;
        }
    }
}
