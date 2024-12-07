using System;
using UnityEngine;

namespace Unity_DMX.Scripts
{
    public enum ColorOrder { RGB, GRB }

    /// <summary>
    /// A display device.
    /// </summary>
    [CreateAssetMenu(fileName = "Display", menuName = Project.ProjectName + "/Display Descriptor", order = 20)]
    public class DisplayDescriptor : ScriptableObject
    {
        public int Width = 20;
        public int Height = 20;
        [Range(0, 0.25f)]
        public float Brightness = 0.1f;
        public bool FlipX;
        public bool FlipY;
        public bool Transpose;
        public bool Serpentine;
        public ColorOrder ColorOrder = ColorOrder.RGB;
        public Action IsDirty;

        private void OnValidate()
        {
            IsDirty?.Invoke();
        }
    }
}