using UnityEngine;
using System.Threading;

namespace Unity_DMX.Scripts
{
    public class Experiment : MonoBehaviour
    {
        public short universe;
        public int fps;
        public float brightness = 0.1f;

        [SerializeField] DmxController controller;
        [SerializeField] private byte[] dmxData;
        Thread dmxSender;

        void Start()
        {
            dmxData = new byte[512];
            universe = 0;
            fps = 30;
        }

        private void Update()
        {
            if (dmxSender == null)
                SetSendingDMX(true);

            // for (short u = 0; u < 1; u++)
            {
                // universe = u;
                for (int i = 0; i < 170; i++)
                {
                    SetDmxValue(i*3, Mathf.Abs(Mathf.Sin(Time.time + i))*brightness);
                    SetDmxValue(i*3+1, Mathf.Abs(Mathf.Cos(Time.time + i))*brightness);
                    // SetDmxValue(i*3, 0);
                    // SetDmxValue(i*3+1, 0);
                    
                    // SetDmxValue(i*3+2, Mathf.Sin(Time.time * 2)+1/2*brightness);
                    SetDmxValue(i*3+2,  Mathf.Abs(Mathf.Sin(Time.time + i)) * brightness);
                    // SetDmxValue(i*3+2,  (Mathf.Sin(Time.time + i) +1/2)  * brightness);
                }
            }
        }

        private void OnDisable()
        {
            Shutdown();
        }

        private void Shutdown()
        {
            for (int i = 0; i < 512; i++)
                SetDmxValue(i, 0);
            controller.Send(universe, dmxData);
            
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

        void SetDmxValue(int channel, float val)
        {
            dmxData[channel] = (byte)Mathf.FloorToInt(Mathf.Lerp(0, 255, val));
        }

        void SendDmx()
        {
            while (true)
            {
                controller.Send(universe, dmxData);
                Thread.Sleep(System.Math.Max(1, 1000 / fps));
            }
        }
    }
}