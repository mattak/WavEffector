using UnityEngine;

namespace WaveAnalyzer.UI
{
    public interface IMusicRender
    {
        public void Prepare(AudioSource source, float[] data);
    }
}