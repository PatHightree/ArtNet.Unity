using System.Collections.Generic;
using UnityEngine;

namespace Unity_DMX.Scripts
{
    public class DmxMatrix
    {
        public readonly int Width;
        public readonly int Height;
        public bool FlipX;
        public bool FlipY;
        public bool Transpose;
        public bool Serpentine;
        public readonly List<DmxUniverse> Universes = new List<DmxUniverse>();
        
        public DmxMatrix(int _width, int _height)
        {
            Width = _width;
            Height = _height;
            int universeCount = Mathf.CeilToInt(_width * _height * 3 / 512f);
            for (short i = 0; i < universeCount; i++)
                Universes.Add(new DmxUniverse (i,512));
        }
        
        public void SetLedRGB(int _x, int _y, float _r, float _g, float _b)
        {
            int x = FlipX ? Width - 1 - _x : _x;
            int y = FlipY ? Height - 1 - _y : _y;
            if (Serpentine)
                x = y % 2 != 0 ? Width - 1 - x : x;
            int ledIndex = Transpose ? y + x * Height :  x + y * Width;
            int universe = Mathf.FloorToInt(ledIndex / 170f);
            ledIndex %= 170;
            // DMX is GRB, so these are shuffled
            Universes[universe].SetChannelValue(ledIndex*3+1, _r);
            Universes[universe].SetChannelValue(ledIndex*3, _g);
            Universes[universe].SetChannelValue(ledIndex*3+2, _b);
        }

        public void Blank()
        {
            for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                SetLedRGB(x, y, 0, 0, 0);
        }
    }
}