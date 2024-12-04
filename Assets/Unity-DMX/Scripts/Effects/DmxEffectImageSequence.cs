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
        private bool transpose;
        private bool serpentine;
        private bool initializing;

        public override void Initialize(int _width, int _height, bool _transpose, bool _serpentine)
        {
            initializing = true;
            serpentine = _serpentine;
            transpose = _transpose;
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
                imageMatrix.Transpose = _transpose;
                imageMatrix.Serpentine = _serpentine;
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
            initializing = false;
        }

        public override void SetTranspose(bool _transpose)
        {
            transpose = _transpose;
            if (matrices != null && matrices[0] != null) 
                Initialize(matrices[0].Width, matrices[0].Height, transpose, serpentine);
        }

        public override void SetSerpentine(bool _serpentine)
        {
            serpentine = _serpentine;
            if (matrices != null && matrices[0] != null) 
                Initialize(matrices[0].Width, matrices[0].Height, transpose, serpentine);
        }

        public override void Step(float _speed, float _brightness)
        {
            Profiler.BeginSample("Filling DmxMatrix");
            playbackTime+=Time.deltaTime * _speed * SpeedMultiplier;
            Profiler.EndSample();
        }

        public override void Send(DmxController _controller)
        {
            if (initializing) return;
            
            foreach (DmxUniverse universe in matrices[Mathf.FloorToInt(playbackTime) % ImageSequence.Count].Universes)
                _controller.Send(universe.Index, universe.Data);
        }

        public override void Blank(DmxController _controller)
        {
            matrices[Mathf.FloorToInt(playbackTime) % ImageSequence.Count].Blank();
            Send(_controller);
        }
    }
}