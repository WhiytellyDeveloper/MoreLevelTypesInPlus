using System.Collections.Generic;
using UnityEngine;

namespace MoreLevelTypesInPlus.Structures.Objects
{
    public class CastleBridgeLever : GameButtonBase
    {
        public override void Pressed(int playerNumber)
        {
            base.Pressed(playerNumber);

            Toggle();

            foreach (IButtonReceiver buttonReceiver in buttonReceivers)
                buttonReceiver.ButtonPressed(false);
        }

        public void Toggle(int setValue = -1)
        {
            if (setValue == -1)
            {
                value += direction;

                if (value >= 2)
                {
                    value = 2;
                    direction = -1;
                }
                else if (value <= 0)
                {
                    value = 0;
                    direction = 1;
                }
            }
            else
            {
                value = setValue;
            }

            switch (value)
            {
                case 0:
                    renderer.material = on;
                    audMan.PlaySingle(up);
                    break;
                case 1:
                    renderer.material = mid;
                    audMan.PlaySingle(middle);
                    break;
                case 2:
                    renderer.material = off;
                    audMan.PlaySingle(down);
                    break;
            }

            last = this;
        }

        public int value = 0;
        private int direction = 1;

        public Material on, off, mid;
        public MeshRenderer renderer;
        public AudioManager audMan;
        public SoundObject up, down, middle;
        public static CastleBridgeLever last;
    }
}
