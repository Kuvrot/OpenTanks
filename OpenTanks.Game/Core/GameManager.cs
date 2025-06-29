using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;

namespace OpenTanks.Core
{
    public class GameManager : SyncScript
    {
        private float timer = 0;
        private float clock = 0;

        #region singleton
        public static GameManager Instance;
        public override void Start()
        {
            Instance = this;
        }
        #endregion

        public override void Update()
        {

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
    }
}
