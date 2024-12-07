using UnityEngine;
using UnityEngine.Profiling;

namespace Unity_DMX.Scripts.Effects
{
    /// <summary>
    /// Procedural effect of expanding and contracting circles
    /// </summary>
    [CreateAssetMenu(fileName = "Circle Effect", menuName = Project.ProjectName + "/Circle Effect", order = 1)]
    public class DmxEffectCircles : DmxEffect
    {
        public override void Step(float _speed)
        {
            Profiler.BeginSample("Filling DmxMatrix");
            for (int y = 0; y < Display.Height; y++)
            {
                for (int x = 0; x < Display.Width; x++)
                {
                    // Circles red expanding, green contracting
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2((Display.Width-1)/2.0f, (Display.Height-1)/2.0f));
                    float time = Time.time * _speed;
                    Matrix.SetLedRGB(x, y, 
                        (Mathf.Sin(time - dist)),
                        (Mathf.Cos(time * 0.33f + dist)),
                        (Mathf.Sin(time - dist)) * 0);
                }
            }
            Profiler.EndSample();
        }
    }
}