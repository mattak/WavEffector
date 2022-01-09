using System;
using UnityEngine;
using WavEffector.Analyzer;

namespace WavEffector.UI
{
    public class VolumePositionRenderer : MonoBehaviour, IMusicRender
    {
        [SerializeField] private float lerp = 0.1f;

        private AudioSource source = default;
        private float[] data = default;
        private Vector3 initialPosition = default;
        private int sampleStep = 0;

        private void Awake()
        {
            this.initialPosition = this.transform.position;
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
            var next = new Vector3(0, level, 0) + initialPosition;
            this.transform.position = Vector3.Lerp(this.transform.position, next, lerp);
        }

        private void Reset()
        {
            this.transform.position = initialPosition;
        }
    }
}