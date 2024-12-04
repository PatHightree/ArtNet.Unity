using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading;
using Unity_DMX.Scripts.Effects;
using UnityEngine.Profiling;

namespace Unity_DMX.Scripts
{
    public class Cubicle : MonoBehaviour
    {
        [Range(0, 10)]
        public float Speed = 1;
        [Range(0, 0.25f)]
        public float Brightness = 0.1f;
        public int FPS = 30;
        [SerializeField] private DmxController Controller;
        public bool Transpose;
        public bool Serpentine;

        public List<DmxEffect> Effects;
        private DmxEffect currentEffect;
        private Thread dmxSender;

        void Start()
        {
            if (Effects != null)
            {
                currentEffect = Effects.First();
                currentEffect.Initialize(16, 16, Transpose, Serpentine);
            }
            else
                Debug.LogWarning("No effect attached to " + gameObject.name);
        }

        private void Update()
        {
            if (dmxSender == null)
                SetSendingDMX(true);

            if (Effects != null) currentEffect.Step(Speed, Brightness);
        }

        private void OnValidate()
        {
            if (Effects.First() != currentEffect)
            {
                currentEffect = Effects.First();
                currentEffect.Initialize(16, 16, Transpose, Serpentine);
            }
            currentEffect.SetTranspose(Transpose);
            currentEffect.SetSerpentine(Serpentine);
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
                if (currentEffect != null) currentEffect.Send(Controller);
                Profiler.EndSample();
                Thread.Sleep(System.Math.Max(1, 1000 / FPS));
            }
        }

        private void OnDisable()
        {
            if (dmxSender != null)
                dmxSender.Abort();
        }
    }
}