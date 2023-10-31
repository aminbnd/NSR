using UnityEngine;

namespace SilverTau.NSR.Samples
{
	[RequireComponent(typeof(Camera))]
	public class IECrt : IEBase
	{
		[SerializeField] private float brightness = 27.0f;
		[SerializeField] private float contrast = 2.1f;
		
		protected override void OnRenderImage(RenderTexture src, RenderTexture dst)
		{
			material.SetFloat("_Brightness", brightness);
			material.SetFloat("_Contrast", contrast);

			Graphics.Blit(src, dst, material);
		}
	}
}
