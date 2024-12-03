using UnityEngine;
using UnityEngine.Profiling;

namespace Unity_DMX.Scripts.Effects
{
    [CreateAssetMenu(fileName = "Twinkle Effect", menuName = "Cubicle/Twinkle Effect", order = 1)]
    public class DmxEffectTwinkle : DmxEffect
    {
        public override void Step(float _speed, float _brightness)
        {
            Profiler.BeginSample("Filling DmxMatrix");
            for (int y = 0; y < Matrix.Height; y++)
            {
                for (int x = 0; x < Matrix.Width; x++)
                {
                    int ledIndex = x + y * Matrix.Width;
                    float time = Time.time * _speed * 0.1f;
                    Matrix.SetLedRGB(x, y, 
                        Mathf.Abs(Mathf.Sin(time + ledIndex))*_brightness,
                        Mathf.Abs(Mathf.Cos(time + ledIndex))*_brightness,
                        Mathf.Abs(Mathf.Sin(time + ledIndex))* 0);
                }
            }
            Profiler.EndSample();
        }
    }
}