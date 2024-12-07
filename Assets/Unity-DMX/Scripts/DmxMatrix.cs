using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity_DMX.Scripts
{
    /// <summary>
    /// DmxMatrix abstracts the set of universes that are required for a rectangle of pixels. 
    /// </summary>
    public class DmxMatrix
    {
        public readonly List<DmxUniverse> Universes = new List<DmxUniverse>();
        
        private DisplayDescriptor display;
        private short redOffset;
        private short greenOffset;
        private short blueOffset;
        
        public DmxMatrix(DisplayDescriptor _display)
        {
            display = _display;
            switch (_display.ColorOrder)
            {
                case ColorOrder.RGB:
                    redOffset = 0;
                    greenOffset = 1;
                    blueOffset = 2;
                    break;
                case ColorOrder.GRB:
                    redOffset = 1;
                    greenOffset = 0;
                    blueOffset = 2;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            int universeCount = Mathf.CeilToInt(_display.Width * _display.Height * 3 / 512f);
            for (short i = 0; i < universeCount; i++)
                Universes.Add(new DmxUniverse (i,512));
        }
        
        public void SetLedRGB(int _x, int _y, float _r, float _g, float _b)
        {
            int x = display.FlipX ? display.Width - 1 - _x : _x;
            int y = display.FlipY ? display.Height - 1 - _y : _y;
            if (display.Serpentine)
                x = y % 2 != 0 ? display.Width - 1 - x : x;
            int ledIndex = display.Transpose ? y + x * display.Height :  x + y * display.Width;
            int universe = Mathf.FloorToInt(ledIndex / 170f);
            ledIndex %= 170;
            // DMX is GRB, so these are shuffled
            Universes[universe].SetChannelValue(ledIndex*3+redOffset, _r * display.Brightness);
            Universes[universe].SetChannelValue(ledIndex*3+greenOffset, _g * display.Brightness);
            Universes[universe].SetChannelValue(ledIndex*3+blueOffset, _b * display.Brightness);
        }
    }
}