using UnityEngine;

namespace SilverTau.NSR.Samples
{
    public class SCAudioWorker : MonoBehaviour, ISCProcess<SCAudioSource>
    {
        [SerializeField] private SCAudioDataSettings audioDataSettings;
        public SCAudioDataSettings AudiosDataSettings => audioDataSettings;
        
        [SerializeField] private AudioSource targetAudioSource;
        [SerializeField] private SCDataOption prefabDataOption;
        [SerializeField] private Transform containerAudioSources;
        
        private void Start()
        {
            Init();
        }
        
        /// <summary>
        /// Initialization of components.
        /// </summary>
        private void Init()
        {
            CreateUICells();
        }

        /// <summary>
        /// Creating UI components.
        /// </summary>
        private void CreateUICells()
        {
            if(prefabDataOption == null) return;

            if (AudiosDataSettings.audioSources.Count > 0)
            {
                foreach (var audioSource in AudiosDataSettings.audioSources)
                {
                    var go = Instantiate(prefabDataOption, containerAudioSources);
                    go.title.text = audioSource.title;
                    go.image.sprite = audioSource.sprite;
                    go.button.onClick.AddListener(() => { Change(audioSource); });
                }
            }
        }
        
        /// <summary>
        /// Function to change the option.
        /// </summary>
        /// <param name="option">Audio option.</param>
        public void Change(SCAudioSource option)
        {
            if(option == null) return;

            targetAudioSource.Stop();
            
            if (option.audioClip == null)
            {
                ResetProcess();
                return;
            }
            
            targetAudioSource.clip = option.audioClip;
            targetAudioSource.Play();
        }
        
        /// <summary>
        /// Function for resetting options.
        /// </summary>
        public void ResetProcess()
        {
            targetAudioSource.Stop();
            targetAudioSource.clip = null;
        }
    }
}