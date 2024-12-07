using UnityEngine;
using UnityEngine.Profiling;

namespace Unity_DMX.Scripts.Effects
{
    /// <summary>
    /// Procedural effect of a twinkling pattern.
    /// </summary>
    [CreateAssetMenu(fileName = "Twinkle Effect", menuName = Project.ProjectName + "/Twinkle Effect", order = 1)]
    public class DmxEffectTwinkle : DmxEffect
    {
        public override void Step(float _speed)
        {
            Profiler.BeginSample("Filling DmxMatrix");
            for (int y = 0; y < Display.Height; y++)
            {
                for (int x = 0; x < Display.Width; x++)
                {
                    int ledIndex = x + y * Display.Width;
                    float time = Time.time * _speed * 0.1f;
                    Matrix.SetLedRGB(x, y, 
                        Mathf.Abs(Mathf.Sin(time + ledIndex)),
                        Mathf.Abs(Mathf.Cos(time + ledIndex)),
                        Mathf.Abs(Mathf.Sin(time + ledIndex))* 0);
                }
            }
            Profiler.EndSample();
        }
    }
}