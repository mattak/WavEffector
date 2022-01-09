using UnityEngine;

namespace WavEffector.UI
{
    public interface IMusicRender
    {
        public void Prepare(AudioSource source, float[] data);
    }
}