using UnityEngine;

namespace SilverTau.NSR.Samples
{
	public class IEGaussian : IEBase
	{
		[SerializeField] private int kernelSize = 21;

		private void Start()
		{
			material.SetInt("_KernelSize", kernelSize);
		}

		protected override void OnRenderImage(RenderTexture src, RenderTexture dst)
		{
			RenderTexture tmp = RenderTexture.GetTemporary(src.width, src.height, 0, src.format);

			Graphics.Blit(src, tmp, material, 0);
			Graphics.Blit(tmp, dst, material, 1);

			RenderTexture.ReleaseTemporary(tmp);
		}
	}
}
