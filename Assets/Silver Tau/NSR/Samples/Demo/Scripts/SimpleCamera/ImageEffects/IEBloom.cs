using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SilverTau.NSR.Samples
{
	internal enum BlurMode
	{
		SinglePass, MultiPass
	}
	
	public class IEBloom : IEBase
	{
		private const int thresholdPass = 0;
		private const int blurPass = 1;
		private const int horizontalPass = 2;
		private const int verticalPass = 3;
		private const int bloomPass = 4;

		[SerializeField]
		private BlurMode blurMode = BlurMode.MultiPass;

		protected override void OnRenderImage(RenderTexture src, RenderTexture dst)
		{
			var thresholdTex = RenderTexture.GetTemporary(src.width, src.height, 0, src.format);

			Graphics.Blit(src, thresholdTex, material, thresholdPass);

			var blurTex = RenderTexture.GetTemporary(src.width, src.height, 0, src.format);

			material.SetInt("_KernelSize", 21);
			material.SetFloat("_Spread", 5.0f);

			if(blurMode == BlurMode.SinglePass)
			{
				Graphics.Blit(thresholdTex, blurTex, material, blurPass);
				RenderTexture.ReleaseTemporary(thresholdTex);
			}
			else
			{
				var temp = RenderTexture.GetTemporary(src.width, src.height, 0, src.format);

				Graphics.Blit(thresholdTex, temp, material, horizontalPass);
				Graphics.Blit(temp, blurTex, material, verticalPass);

				RenderTexture.ReleaseTemporary(thresholdTex);
				RenderTexture.ReleaseTemporary(temp);
			}

			material.SetTexture("_SrcTex", src);

			Graphics.Blit(blurTex, dst, material, bloomPass);

			RenderTexture.ReleaseTemporary(blurTex);
		}
	}
}
