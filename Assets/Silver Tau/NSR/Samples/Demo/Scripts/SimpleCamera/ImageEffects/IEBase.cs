using UnityEngine;

namespace SilverTau.NSR.Samples
{
	[RequireComponent(typeof(Camera))]
	public class IEBase : MonoBehaviour
	{
		[SerializeField] protected Shader shader;
		protected Material material;

		private void Awake()
		{
			material = new Material(shader);
		}
	
		protected virtual void OnRenderImage(RenderTexture src, RenderTexture dst)
		{
			Graphics.Blit(src, dst, material);
		}
	}
}
