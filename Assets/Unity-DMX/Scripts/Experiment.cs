using System.Collections.Generic;
using UnityEngine;
using System.Threading;

namespace Unity_DMX.Scripts
{
    public class Experiment : MonoBehaviour
    {
        public int fps;
        public float brightness = 0.1f;

        [SerializeField] DmxController controller;
        List<DmxUniverse> universes = new List<DmxUniverse>();
        Thread dmxSender;

        public class DmxUniverse
        {
            public short Index;
            public byte[] DmxData;

            public DmxUniverse(short index)
            {
                this.Index = index;
                DmxData = new byte[512];
            }

            public void SetChannelValue(int channel, float value)
            {
                DmxData[channel] = (byte)Mathf.FloorToInt(Mathf.Lerp(0, 255, value));
            }

            public void Send()
            {
                
            }
        }

        void Start()
        {
            universes.Add(new DmxUniverse(0));
            universes.Add(new DmxUniverse(1));
            fps = 30;
        }

        private void Update()
        {
            if (dmxSender == null)
                SetSendingDMX(true);

            foreach (DmxUniverse universe in universes)
            {
                for (int i = 0; i < 170; i++)
                {
                    universe.SetChannelValue(i*3, Mathf.Abs(Mathf.Sin(Time.time + i))*brightness);
                    universe.SetChannelValue(i*3+1, Mathf.Abs(Mathf.Cos(Time.time + i))*brightness);
                    universe.SetChannelValue(i*3+2,  Mathf.Abs(Mathf.Sin(Time.time + i)) * brightness);
                }
            }
        }

        private void OnDisable()
        {
            Shutdown();
        }

        private void Shutdown()
        {
            foreach (DmxUniverse universe in universes)
            {
                for (int i = 0; i < 512; i++)
                    universe.SetChannelValue(i, 0);
                controller.Send(universe.Index, universe.DmxData);
            }
            
            if (dmxSender != null)
                dmxSender.Abort();
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
                foreach (DmxUniverse universe in universes)
                    controller.Send(universe.Index, universe.DmxData);
                Thread.Sleep(System.Math.Max(1, 1000 / fps));
            }
        }
    }
}