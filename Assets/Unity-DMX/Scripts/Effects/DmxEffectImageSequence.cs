using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

namespace Unity_DMX.Scripts.Effects
{
    [CreateAssetMenu(fileName = "Image Sequence Effect", menuName = "Cubicle/Image Sequence Effect", order = 1)]
    public class DmxEffectImageSequence : DmxEffect
    {
        public List<Texture2D> ImageSequence;
        public float Brightness = 0.01f;
        public float SpeedMultiplier = 1.0f;
        
        private float playbackTime;
        private List<DmxMatrix> matrices;

        public override void Initialize(int _width, int _height)
        {
            Initializing = true;
            matrices = new List<DmxMatrix>();
            foreach (Texture2D imageAsset in ImageSequence)
            {
                string assetPath = AssetDatabase.GetAssetPath(imageAsset);
                Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
                if (!tex.isReadable)
                {
                    var tImporter = AssetImporter.GetAtPath( assetPath ) as TextureImporter;
                    if ( tImporter != null)
                    {
                        tImporter.textureType = TextureImporterType.Default;
                        tImporter.isReadable = true;
                        AssetDatabase.ImportAsset( assetPath );
                        AssetDatabase.Refresh();
                    }
                }
                DmxMatrix imageMatrix = new DmxMatrix(_width, _height);
                imageMatrix.FlipX = FlipX;
                imageMatrix.FlipY = FlipY;
                imageMatrix.Transpose = Transpose;
                imageMatrix.Serpentine = Serpentine;
                Color[] colors = tex.GetPixels();
                for (int y = 0; y < _height; y++)
                {
                    for (int x = 0; x < _width; x++)
                    {
                        int ledIndex = x + y * _width;
                        imageMatrix.SetLedRGB(x, y, 
                            colors[ledIndex].r * Brightness, 
                            colors[ledIndex].g * Brightness, 
                            colors[ledIndex].b * Brightness);
                    }
                }
                matrices.Add(imageMatrix);
            }
            Initializing = false;
        }

        public override void SetGeometry(bool _flipX, bool _flipY, bool _transpose, bool _serpentine)
        {
            FlipX = _flipX;
            FlipY = _flipY;
            Transpose = _transpose;
            Serpentine = _serpentine;
            Initialize(Width, Height);
        }

        public override void Step(float _speed, float _brightness)
        {
            Profiler.BeginSample("Filling DmxMatrix");
            playbackTime+=Time.deltaTime * _speed * SpeedMultiplier;
            Profiler.EndSample();
        }

        public override void Send(DmxController _controller)
        {
            if (Initializing) return;
            
            foreach (DmxUniverse universe in matrices[Mathf.FloorToInt(playbackTime) % ImageSequence.Count].Universes)
                _controller.Send(universe.Index, universe.Data);
        }
    }
}