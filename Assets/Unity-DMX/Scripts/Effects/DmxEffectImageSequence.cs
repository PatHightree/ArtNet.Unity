using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

namespace Unity_DMX.Scripts.Effects
{
    /// <summary>
    /// Image sequence animation playback effect.
    /// </summary>
    [CreateAssetMenu(fileName = "Image Sequence Effect", menuName = Project.ProjectName + "/Image Sequence Effect", order = 1)]
    public class DmxEffectImageSequence : DmxEffect
    {
        public List<Texture2D> ImageSequence;
        public float BrightnessMultiplier = 0.01f;
        public float SpeedMultiplier = 1.0f;
        
        private float playbackTime;
        private List<DmxMatrix> matrices;

        public override void Initialize(DisplayDescriptor _display)
        {
            base.Initialize(_display);
            
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
                        tImporter.npotScale = TextureImporterNPOTScale.None;
                        tImporter.isReadable = true;
                        AssetDatabase.ImportAsset( assetPath );
                        AssetDatabase.Refresh();
                    }
                }
                DmxMatrix imageMatrix = new DmxMatrix(_display);
                Color[] colors = tex.GetPixels();
                for (int y = 0; y < _display.Height; y++)
                {
                    for (int x = 0; x < _display.Width; x++)
                    {
                        int ledIndex = x + y * _display.Width;
                        imageMatrix.SetLedRGB(x, y, 
                            colors[ledIndex].r * BrightnessMultiplier, 
                            colors[ledIndex].g * BrightnessMultiplier, 
                            colors[ledIndex].b * BrightnessMultiplier);
                    }
                }
                matrices.Add(imageMatrix);
            }
            Initializing = false;
        }

        public override void Step(float _speed)
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