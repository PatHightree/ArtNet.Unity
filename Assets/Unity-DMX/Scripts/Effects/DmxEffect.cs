using UnityEngine;

namespace Unity_DMX.Scripts
{
    public abstract class DmxEffect : ScriptableObject
    {
        protected DmxMatrix Matrix;
        
        public virtual void Initialize(int width, int height)
        {
            Matrix = new DmxMatrix(width, height);
        }
        public abstract void Step(float speed, float brightness);

        public void Send(DmxController controller)
        {
            foreach (DmxUniverse universe in Matrix.Universes)
                controller.Send(universe.Index, universe.Data);
        }

        public void Blank(DmxController controller)
        {
            Matrix.Blank();
            Send(controller);
        }
    }
}