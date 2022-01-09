using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace WavEffector.UI
{
    public class MusicGroupRenderer : MonoBehaviour
    {
        [SerializeField] private AudioSource source;

        [Tooltip("Please put IMusicRenderer")] [SerializeField]
        private MonoBehaviour[] renderers;

        private IMusicRender[] musicRenderers = null;

        private void Awake()
        {
            musicRenderers = renderers
                .Select(x => x.GetComponent<IMusicRender>())
                .Where(x => x != null)
                .ToArray();

            Assert.AreEqual(musicRenderers.Length, renderers.Length,
                $"IMusicRenderer must be specified on {this.gameObject.name}/{this.name}");
        }

        private void Start()
        {
            Prepare();
            Play();
        }

        private void Prepare()
        {
            var clip = source.clip;
            var data = new float[clip.channels * clip.samples];
            source.clip.GetData(data, 0);

            for (var i = 0; i < renderers.Length; i++)
            {
                musicRenderers[i].Prepare(source, data);
            }
        }

        private void Play()
        {
            source.Stop();
            source.Play();
        }
    }
}