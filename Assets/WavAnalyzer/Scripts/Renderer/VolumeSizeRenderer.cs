using System;
using UnityEngine;
using WaveAnalyzer.Analyzer;

namespace WaveAnalyzer.UI
{
    public class VolumeSizeRenderer : MonoBehaviour, IMusicRender
    {
        [SerializeField] private float scaleFactor = 1f;
        [SerializeField] private float lerp = 0.5f;

        private AudioSource source = default;
        private float[] data = default;
        private Vector3 initialScale = default;
        private int sampleStep = default;

        private void Awake()
        {
            this.initialScale = transform.localScale;
        }

        private void Start()
        {
            var source = GetComponent<AudioSource>();
            var clip = source.clip;
            var data = new float[clip.channels * clip.samples];
            source.clip.GetData(data, 0);

            Prepare(source, data);
        }

        public void Prepare(AudioSource source, float[] monoData)
        {
            this.source = source;
            this.data = monoData;

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

        private void Render(float size)
        {
            var diff = initialScale * this.scaleFactor * size;
            transform.localScale = Vector3.Lerp(transform.localScale, diff, lerp);
        }

        private void Reset()
        {
            transform.localScale = initialScale;
        }
    }
}