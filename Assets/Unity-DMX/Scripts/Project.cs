using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading;
using Unity_DMX.Scripts.Effects;
using UnityEngine.Profiling;

namespace Unity_DMX.Scripts
{
    public class Project : MonoBehaviour
    {
        public const string ProjectName = "BigVoxelBox";

        [Range(0, 10)]
        public float Speed = 1;
        public int FPS = 30;
        public DmxController Controller;
        public DisplayDescriptor Display;

        public List<DmxEffect> Effects;
        private DmxEffect currentEffect;
        private Thread dmxSender;

        #region Unity Callbacks

        void Start()
        {
            if (Effects != null)
                StartEffect();
            else
                Debug.LogWarning("No effect attached to " + gameObject.name);

            Display.IsDirty += OnValidate;
        }

        private void Update()
        {
            if (dmxSender == null)
                SetSendingDMX(true);

            if (Effects != null) currentEffect.Step(Speed);
        }

        private void OnValidate()
        {
            StartEffect();
        }

        private void OnDisable()
        {
            if (dmxSender != null)
                dmxSender.Abort();
        }

        #endregion

        #region Functionality

        private void StartEffect()
        {
            if (currentEffect != null) currentEffect.IsDirty -= OnValidate;
            currentEffect = Effects.First();
            currentEffect.Initialize(Display);
            currentEffect.IsDirty += OnValidate;
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
                Thread.Sleep(Math.Max(1, 1000 / FPS));
            }
        }

        #endregion
    }
}