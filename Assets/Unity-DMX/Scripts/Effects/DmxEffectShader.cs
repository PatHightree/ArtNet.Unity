using UnityEngine;

namespace Unity_DMX.Scripts.Effects
{
    /// <summary>
    /// Effect that blits a shader into DmxMatrix
    /// </summary>
    [CreateAssetMenu(fileName = "Shader Effect", menuName = Project.ProjectName + "/Shader Effect", order = 1)]
    public class DmxEffectShader : DmxEffect
    {
        public Material ShaderMaterial;
        private RenderTexture _renderTexture;
        private Texture2D _texture;

        public override void Initialize(DisplayDescriptor _display)
        {
            base.Initialize(_display);
            
            _renderTexture = new RenderTexture(_display.Width, _display.Height,  8, RenderTextureFormat.ARGB32);
            _texture = new Texture2D(_display.Width, _display.Height, TextureFormat.ARGB32, false);
            if (Camera.main != null) Camera.main.targetTexture = _renderTexture;
        }

        public override void Step(float _speed)
        {
            // Blit shader into RT and then into Texture2D
            RenderTexture currentActiveRT = RenderTexture.active;
            RenderTexture.active = _renderTexture;
            Graphics.Blit(null, ShaderMaterial);
            _texture.ReadPixels(new Rect(0, 0, _renderTexture.width, _renderTexture.height), 0, 0);
            RenderTexture.active = currentActiveRT;
            
            // Transfer Texture2D to DmxMatrix
            Color[] colors = _texture.GetPixels();
            for (int y = 0; y < Display.Height; y++)
            {
                for (int x = 0; x < Display.Width; x++)
                {
                    int ledIndex = x + y * Display.Width;
                    Matrix.SetLedRGB(x, y, 
                        colors[ledIndex].r, 
                        colors[ledIndex].g, 
                        colors[ledIndex].b);
                }
            }
        }
    }
}