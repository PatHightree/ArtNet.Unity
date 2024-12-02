using UnityEngine;

namespace Unity_DMX.Scripts
{
    public class DmxUniverse
    {
        public short Index;
        public byte[] Data;

        public DmxUniverse(short index, short size)
        {
            Index = index;
            Data = new byte[size];
        }

        public void SetChannelValue(int channel, float value)
        {
            Data[channel] = (byte)Mathf.FloorToInt(Mathf.Lerp(0, 255, value));
        }

    }
}