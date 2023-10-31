using UnityEngine;
using UnityEngine.UI;

namespace SilverTau.NSR.Samples
{
    public class SCEffectWorker : MonoBehaviour, ISCProcess<SCEffect>
    {
        [SerializeField] private SCEffectsDataSettings effectsDataSettings;
        public SCEffectsDataSettings EffectsDataSettings => effectsDataSettings;
        
        [SerializeField] private SCDataOption prefabDataOption;
        [SerializeField] private Transform containerUIEffects;
        [SerializeField] private Transform containerEffects;

        private GameObject _lastEffect;
        
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

            if (effectsDataSettings.effects.Count > 0)
            {
                foreach (var effect in effectsDataSettings.effects)
                {
                    var go = Instantiate(prefabDataOption, containerUIEffects);
                    go.title.text = effect.title;
                    go.image.sprite = effect.sprite;
                    go.button.onClick.AddListener(() => { Change(effect); });
                }
            }
        }
        
        /// <summary>
        /// Function to change the option.
        /// </summary>
        /// <param name="option">Effect option.</param>
        public void Change(SCEffect option)
        {
            if(option == null) return;
            
            if (option.prefabEffect == null)
            {
                ResetProcess();
                return;
            }

            if (_lastEffect != null)
            {
                Destroy(_lastEffect);
            }

            _lastEffect = Instantiate(option.prefabEffect, containerEffects);
        }

        /// <summary>
        /// Function for resetting options.
        /// </summary>
        public void ResetProcess()
        {
            if(_lastEffect == null) return;
            Destroy(_lastEffect.gameObject);
        }
    }
}