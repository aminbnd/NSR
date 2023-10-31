using UnityEngine;
using UnityEngine.UI;

namespace SilverTau.NSR.Samples
{
    public class SCViewController : MonoBehaviour
    {
        [Header("Menu Actions")]
        [Space(10)]
        [SerializeField] private GameObject menuActions;
        [SerializeField] private Button buttonActionAudioSources;
        [SerializeField] private Button buttonActionEffects;
        [SerializeField] private Button buttonActionFilters;
        [SerializeField] private Button buttonActionSettings;
        
        [Header("Menu Options")]
        [Space(10)]
        [SerializeField] private GameObject menuOptions;
        [SerializeField] private Button buttonOptionsBack;
        [SerializeField] private GameObject audioSourcesList;
        [SerializeField] private GameObject effectsList;
        [SerializeField] private GameObject filtersList;
        [SerializeField] private GameObject settingsList;

        private OptionType _currentOptionType = OptionType.None;
        
        private void Start()
        {
            Init();
        }
        
        /// <summary>
        /// Initialization of components.
        /// </summary>
        private void Init()
        {
            menuActions.SetActive(true);
            
            buttonOptionsBack.onClick.AddListener(CloseOptions);
            buttonActionAudioSources.onClick.AddListener(() => { OpenOption(OptionType.AudioSources); });
            buttonActionEffects.onClick.AddListener(() => { OpenOption(OptionType.Effects); });
            buttonActionFilters.onClick.AddListener(() => { OpenOption(OptionType.Filters); });
            buttonActionSettings.onClick.AddListener(() => { OpenOption(OptionType.Settings); });
        }
         
        /// <summary>
        /// A method that resets options.
        /// </summary>
        private void ResetOptions()
        {
            audioSourcesList.SetActive(false);
            effectsList.SetActive(false);
            filtersList.SetActive(false);
            settingsList.SetActive(false);
        }

        /// <summary>
        /// A method that closes options.
        /// </summary>
        private void CloseOptions()
        {
            menuActions.SetActive(true);
            menuOptions.SetActive(false);
            ResetOptions();
        }

        /// <summary>
        /// A method for opening a specific option.
        /// </summary>
        /// <param name="optionType">The type of option that will be opened.</param>
        private void OpenOption(OptionType optionType)
        {
            ResetOptions();

            switch (optionType)
            {
                case OptionType.None:
                    CloseOptions();
                    break;
                case OptionType.AudioSources:
                    audioSourcesList.SetActive(true);
                    break;
                case OptionType.Effects:
                    effectsList.SetActive(true);
                    break;
                case OptionType.Filters:
                    filtersList.SetActive(true);
                    break;
                case OptionType.Settings:
                    settingsList.SetActive(true);
                    break;
                default:
                    CloseOptions();
                    break;
            }
            
            _currentOptionType = optionType;
            
            menuActions.SetActive(false);
            menuOptions.SetActive(true);
        }
    }
}