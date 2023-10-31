using UnityEngine;

namespace SilverTau.NSR.Samples
{
	public class IEPixelate : IEBase
	{
		[SerializeField] private int pixelSize = 2;

		protected override void OnRenderImage(RenderTexture src, RenderTexture dst)
		{
			int width  = src.width / pixelSize;
			int height = src.height / pixelSize;

			var temp = RenderTexture.GetTemporary(width, height, 0, src.format);

			temp.filterMode = FilterMode.Point;

			Graphics.Blit(src, temp);
			Graphics.Blit(temp, dst, material);
		}
	}
}
