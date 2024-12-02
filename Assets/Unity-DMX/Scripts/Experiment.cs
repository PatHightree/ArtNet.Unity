using UnityEngine;
using System.Threading;
using UnityEngine.Profiling;

namespace Unity_DMX.Scripts
{
    public partial class Experiment : MonoBehaviour
    {
        [Range(0, 10)]
        public float Speed = 1;
        [Range(0, 1)]
        public float Brightness = 0.1f;
        public int FPS = 30;
        [SerializeField] private DmxController controller;

        private DmxMatrix matrix;
        private Thread dmxSender;

        void Start()
        {
            matrix = new DmxMatrix(16, 16);
        }

        private void Update()
        {
            if (dmxSender == null)
                SetSendingDMX(true);

            Profiler.BeginSample("Filling DmxMatrix");
            for (int y = 0; y < matrix.Height; y++)
            {
                for (int x = 0; x < matrix.Width; x++)
                {
                    // Circles red expanding, green contracting
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2((matrix.Width-1)/2.0f, (matrix.Height-1)/2.0f));
                    float time = Time.time * Speed;
                    matrix.SetLedRGB(x, y, 
                        (Mathf.Sin(time - dist))*Brightness,
                        (Mathf.Cos(time * 0.33f + dist))*Brightness,
                        (Mathf.Sin(time - dist))*Brightness * 0);
                    
                    // Twinkle effect
                    // int ledIndex = x + y * matrix.Width;
                    // matrix.SetLedRGB(x, y, 
                    //     Mathf.Abs(Mathf.Sin(Time.time + ledIndex))*Brightness,
                    //     Mathf.Abs(Mathf.Cos(Time.time + ledIndex))*Brightness,
                    //     Mathf.Abs(Mathf.Tan(Time.time + ledIndex))*Brightness);
                }
            }
            Profiler.EndSample();
        }

        public void SetSendingDMX(bool b)
        {
            if (dmxSender != null)
                dmxSender.Abort();
            if (b)
            {
                dmxSender = new Thread(SendDmx);
                dmxSender.Start();
            }
            else
                dmxSender = null;
        }

        void SendDmx()
        {
            while (true)
            {
                Profiler.BeginSample("Sending Dmx");
                foreach (DmxUniverse universe in matrix.Universes)
                    controller.Send(universe.Index, universe.Data);
                Profiler.EndSample();
                Thread.Sleep(System.Math.Max(1, 1000 / FPS));
            }
        }

        private void OnDisable()
        {
            matrix.Blank();
            foreach (DmxUniverse universe in matrix.Universes)
                controller.Send(universe.Index, universe.Data);

            if (dmxSender != null)
                dmxSender.Abort();
        }
    }
}