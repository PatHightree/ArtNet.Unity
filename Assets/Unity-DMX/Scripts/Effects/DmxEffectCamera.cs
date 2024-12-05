using UnityEngine;

namespace Unity_DMX.Scripts.Effects
{
    [CreateAssetMenu(fileName = "Camera Effect", menuName = "Cubicle/Camera Effect", order = 1)]
    public class DmxEffectCamera : DmxEffect
    {
        private RenderTexture _renderTexture;
        private Texture2D _texture;

        public override void Initialize(int _width, int _height)
        {
            base.Initialize(_width, _height);
            
            _renderTexture = new RenderTexture(_width, _height,  8, RenderTextureFormat.ARGB32);
            _texture = new Texture2D(_width, _height, TextureFormat.ARGB32, false);
            if (Camera.main != null) Camera.main.targetTexture = _renderTexture;
        }

        public override void Step(float _speed, float _brightness)
        {
            // Transfer camera RT to Texture2D
            RenderTexture currentActiveRT = RenderTexture.active;
            RenderTexture.active = _renderTexture;
            _texture.ReadPixels(new Rect(0, 0, _renderTexture.width, _renderTexture.height), 0, 0);
            RenderTexture.active = currentActiveRT;
            
            // Transfer Texture2D to DmxMatrix
            Color[] colors = _texture.GetPixels();
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int ledIndex = x + y * Width;
                    Matrix.SetLedRGB(x, y, 
                        colors[ledIndex].r*_brightness, 
                        colors[ledIndex].g*_brightness, 
                        colors[ledIndex].b*_brightness);
                }
            }

        }
    }
}