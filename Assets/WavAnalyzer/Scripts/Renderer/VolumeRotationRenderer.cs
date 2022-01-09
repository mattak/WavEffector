using System;
using UnityEngine;
using WaveAnalyzer.Analyzer;

namespace WaveAnalyzer.UI
{
    public class VolumeRotationRenderer : MonoBehaviour, IMusicRender
    {
        [SerializeField] private float lerp = 0.9f;
        [SerializeField] private float stepSize = 1f;
        [SerializeField] private bool isRotationX = false;
        [SerializeField] private bool isRotationY = false;
        [SerializeField] private bool isRotationZ = true;

        private AudioSource source = default;
        private float[] data = default;
        private Quaternion initialRotation = default;
        private int sampleStep = default;

        private void Awake()
        {
            this.initialRotation = this.transform.rotation;
        }

        public void Prepare(AudioSource source, float[] data)
        {
            this.source = source;
            this.data = data;

            var fps = Mathf.Max(60f, 1f / Time.fixedDeltaTime);
            var clip = source.clip;
            this.sampleStep = (int) (clip.frequency / fps);
        }

        private void FixedUpdate()
        {
            if (source.isPlaying && source.timeSamples < data.Length)
            {
                var startIndex = source.timeSamples;
                var endIndex = Math.Min(source.timeSamples + sampleStep, data.Length);
                var level = WavAnalyzer.DetectVolumeLevel(data, startIndex, endIndex);
                Render(level);
            }
            else
            {
                Reset();
            }
        }

        private void Render(float level)
        {
            var current = this.transform.rotation;
            var diff = level * stepSize;
            var rx = isRotationX ? diff : 0;
            var ry = isRotationY ? diff : 0;
            var rz = isRotationZ ? diff : 0;
            var next = current * Quaternion.Euler(rx, ry, rz);
            current = Quaternion.Lerp(current, next, lerp);
            this.transform.rotation = current;
        }

        private void Reset()
        {
            this.transform.rotation = initialRotation;
        }
    }
}