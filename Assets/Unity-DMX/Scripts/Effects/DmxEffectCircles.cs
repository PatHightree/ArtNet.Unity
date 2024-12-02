using UnityEngine;
using UnityEngine.Profiling;

namespace Unity_DMX.Scripts
{
    [CreateAssetMenu(fileName = "Circle Effect", menuName = "Cubicle/Circle Effect", order = 1)]
    public class DmxEffectCircles : DmxEffect
    {
        public override void Step(float speed, float brightness)
        {
            Profiler.BeginSample("Filling DmxMatrix");
            for (int y = 0; y < Matrix.Height; y++)
            {
                for (int x = 0; x < Matrix.Width; x++)
                {
                    // Circles red expanding, green contracting
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2((Matrix.Width-1)/2.0f, (Matrix.Height-1)/2.0f));
                    float time = Time.time * speed;
                    Matrix.SetLedRGB(x, y, 
                        (Mathf.Sin(time - dist))*brightness,
                        (Mathf.Cos(time * 0.33f + dist))*brightness,
                        (Mathf.Sin(time - dist))*brightness * 0);
                }
            }
            Profiler.EndSample();
        }
    }
}