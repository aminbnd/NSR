using UnityEngine;

namespace SilverTau.NSR.Samples
{
    public class SCFilterWorker : MonoBehaviour, ISCProcess<SCFilter>
    {
        [SerializeField] private SCFilterCameraRender filterCameraRender;
        [SerializeField] private SCFiltersDataSettings filtersDataSettings;
        public SCFiltersDataSettings FiltersDataSettings => filtersDataSettings;

        [SerializeField] private SCDataOption prefabDataOption;
        [SerializeField] private Transform containerFilters;

        
        private void Start()
        {
            Init();
        }
        
        /// <summary>
        /// Initialization of components.
        /// </summary>
        private void Init()
        {
            ResetProcess();
            CreateUICells();
        }

        /// <summary>
        /// Creating UI components.
        /// </summary>
        private void CreateUICells()
        {
            if(prefabDataOption == null) return;

            if (FiltersDataSettings.filters.Count > 0)
            {
                foreach (var filter in FiltersDataSettings.filters)
                {
                    var go = Instantiate(prefabDataOption, containerFilters);
                    go.title.text = filter.title;
                    go.image.sprite = filter.sprite;
                    go.button.onClick.AddListener(() => { Change(filter); });
                }
            }
        }
        
        /// <summary>
        /// Function to change the option.
        /// </summary>
        /// <param name="option">Filter option.</param>
        public void Change(SCFilter option)
        {
            if(option == null) return;
                
            if (option.filterShader == null)
            {
                ResetProcess();
                return;
            }

            PrepareFilter(option.id, option.filterShader, option.additionalFilterShader);
        }

        /// <summary>
        /// Function for preparing the filter.
        /// </summary>
        /// <param name="id">Filter ID.</param>
        /// <param name="shader">Main shader.</param>
        /// <param name="additionalShader">Additional shader.</param>
        private void PrepareFilter(string id, Shader shader, Shader additionalShader)
        {
            switch (id)
            {
                case "none":
                case "bw":
                    filterCameraRender.Filter = new BaseFilter(id, Color.white, shader);
                    break;
                case "sepia":
                    filterCameraRender.Filter = new BaseFilter(id, new Color(1.00f, 1.00f, 0.79f), shader);
                    break;
                case "blur-full":
                case "blur-edge":
                    filterCameraRender.Filter = new BlurFilter(id, Color.white, shader);
                    break;
                case "outlines":
                    filterCameraRender.Filter = new BaseFilter(id, new Color(1.00f, 1.00f, 0.79f), shader);
                    break;
                case "neon":
                    filterCameraRender.Filter = new NeonFilter(id, Color.cyan, additionalShader, new BaseFilter("", Color.white, shader));
                    break;
                case "nes":
                    filterCameraRender.Filter = new CRTFilter(id, new Color(0.66f, 1.00f, 1.00f), additionalShader, new PixelFilter("", Color.white, shader));
                    break;
                case "snes":
                    filterCameraRender.Filter = new CRTFilter(id, new Color(0.80f, 1.00f, 1.00f), additionalShader, new PixelFilter("", Color.white, shader));
                    break;
                case "game-boy":
                    filterCameraRender.Filter = new PixelFilter(id, new Color(0.61f, 0.73f, 0.06f), shader);
                    break;
                default:
                    filterCameraRender.Filter = new BaseFilter(id, Color.white, shader);
                    break;
            }
        }

        /// <summary>
        /// Function for resetting options.
        /// </summary>
        public void ResetProcess()
        {
            var noneShader = Shader.Find("Silver Tau/Base");
            filterCameraRender.Filter = new BaseFilter("none", Color.white, noneShader);
        }
        
        /// <summary>
        /// Event function that Unity calls after a Camera has finished rendering, that allows you to modify the Camera's final image.
        /// </summary>
        /// <param name="src">Source RenderTexture.</param>
        /// <param name="dst">Destination RenderTexture.</param>
        private void OnRenderImage(RenderTexture src, RenderTexture dst)
        {
            filterCameraRender.Filter?.OnRenderImage(src, dst);
        }
    }
}