using UnityEngine;

namespace Unity_DMX.Scripts.Effects
{
    public abstract class DmxEffect : ScriptableObject
    {
        protected DmxMatrix Matrix;
        
        public virtual void Initialize(int _width, int _height)
        {
            Matrix = new DmxMatrix(_width, _height);
        }

        public virtual void SetGeometry(bool _flipX, bool _flipY, bool _transpose, bool _serpentine)
        {
            if (Matrix == null) return;
            Matrix.FlipX = _flipX;
            Matrix.FlipY = _flipY;
            Matrix.Transpose = _transpose;
            Matrix.Serpentine = _serpentine;
        }

        public abstract void Step(float _speed, float _brightness);

        public virtual void Send(DmxController _controller)
        {
            foreach (DmxUniverse universe in Matrix.Universes)
                if (_controller != null)
                    _controller.Send(universe.Index, universe.Data);
        }
    }
}