using System.Collections.Generic;
using UnityEngine;

namespace Unity_DMX.Scripts
{
    public class DmxMatrix
    {
        public readonly int Width;
        public readonly int Height;
        public List<DmxUniverse> Universes = new List<DmxUniverse>();
        
        public DmxMatrix(int width, int height)
        {
            Width = width;
            Height = height;
            int universeCount = Mathf.CeilToInt(width * height * 3 / 512f);
            for (short i = 0; i < universeCount; i++)
                Universes.Add(new DmxUniverse (i,512));
        }
        
        public void SetLedRGB(int x, int y, float r, float g, float b)
        {
            int ledIndex = x + y * Width;
            int universe = Mathf.FloorToInt(ledIndex / 170f);
            ledIndex %= 170;
            // DMX is GRB, so these are shuffled
            Universes[universe].SetChannelValue(ledIndex*3+1, r);
            Universes[universe].SetChannelValue(ledIndex*3, g);
            Universes[universe].SetChannelValue(ledIndex*3+2, b);
        }

        public void Blank()
        {
            for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                SetLedRGB(x, y, 0, 0, 0);
        }
    }
}