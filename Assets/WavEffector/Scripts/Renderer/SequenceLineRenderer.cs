using System;
using UnityEngine;

namespace WavEffector.UI
{
    public class SequenceLineRenderer : MonoBehaviour, IMusicRender
    {
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private float waveLength = 20.0f;
        [SerializeField] private float yLength = 10f;

        private AudioSource source = default;
        private float[] data = default;
        private int sampleStep = default;
        private Vector3[] samplingLinePoints = default;

        private void Start()
        {
            var source = GetComponent<AudioSource>();
            var clip = source.clip;
            var data = new float[clip.channels * clip.samples];
            source.clip.GetData(data, 0);

            Prepare(source, data);
        }

        public void Prepare(AudioSource source, float[] data)
        {
            this.source = source;
            this.data = data;

            var fps = Mathf.Max(60f, 1f / Time.fixedDeltaTime);
            var clip = source.clip;
            sampleStep = (int) (clip.frequency / fps);
            samplingLinePoints = new Vector3[sampleStep];
        }

        private void FixedUpdate()
        {
            if (source.isPlaying && source.timeSamples < data.Length)
            {
                var startIndex = source.timeSamples;
                var endIndex = source.timeSamples + sampleStep;
                Inflate(
                    data, startIndex, endIndex,
                    samplingLinePoints,
                    waveLength, -waveLength / 2f, yLength
                );
                Render(samplingLinePoints);
            }
            else
            {
                Reset();
            }
        }

        private void Render(Vector3[] points)
        {
            if (points == null) return;
            lineRenderer.positionCount = points.Length;
            lineRenderer.SetPositions(points);
        }

        private void Reset()
        {
            var x = -waveLength / 2;
            Render(new[]
            {
                new Vector3(-x, 0, 0) + this.transform.position,
                this.transform.position,
                new Vector3(x, 0, 0) + this.transform.position,
            });
        }

        public void Inflate(
            float[] target, int start, int end,
            Vector3[] result,
            float xLength, float xOffset, float yLength
        )
        {
            var samples = Math.Max(end - start, 1f);
            var xStep = xLength / samples;
            var j = 0;

            for (var i = start; i < end; i++, j++)
            {
                var x = xOffset + xStep * j;
                var y = i < target.Length ? target[i] * yLength : 0f;
                var p = new Vector3(x, y, 0) + this.transform.position;
                result[j] = p;
            }
        }
    }
}