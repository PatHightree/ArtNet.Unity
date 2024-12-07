using System;
using UnityEngine;

namespace Unity_DMX.Scripts.Effects
{
    /// <summary>
    /// Base class for visual effects which can draw into DmxMatrices to send over Artnet.
    /// </summary>
    public abstract class DmxEffect : ScriptableObject
    {
        public Action IsDirty;

        protected DmxMatrix Matrix;
        protected bool Initializing;
        protected DisplayDescriptor Display;

        private void OnValidate()
        {
            IsDirty?.Invoke();
        }

        public virtual void Initialize(DisplayDescriptor _display)
        {
            Display = _display;
            Matrix = new DmxMatrix(_display);
        }

        public abstract void Step(float _speed);

        public virtual void Send(DmxController _controller)
        {
            foreach (DmxUniverse universe in Matrix.Universes)
                if (_controller != null)
                    _controller.Send(universe.Index, universe.Data);
        }
    }
}