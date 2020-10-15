using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Service
{
	public class StableAspect : MonoBehaviour
	{
		// 画像のサイズ
		[SerializeField] private float width = 800f;
		[SerializeField] private float height = 600f;
		// 画像のPixel Per Unit
		[SerializeField] private float pixelPerUnit = 100f;

		float bgAcpect;

		private void Awake()
		{
			bgAcpect = height / width;

			// カメラのorthographicSizeを設定
			Camera.main.orthographicSize = (height / 2f / pixelPerUnit);

			Debug.Log("Camera.main.orthographicSize = " + Camera.main.orthographicSize);
		}

		private void Update()
		{
			float aspect = (float)Screen.height / (float)Screen.width;


			if (bgAcpect > aspect)
			{
				// 倍率
				float bgScale = height / Screen.height;
				// viewport rectの幅
				float camWidth = width / (Screen.width * bgScale);
				// viewportRectを設定
				Camera.main.rect = new Rect((1f - camWidth) / 2f, 0f, camWidth, 1f);
			}
			else
			{
				// 倍率
				float bgScale = width / Screen.width;
				// viewport rectの幅
				float camHeight = height / (Screen.height * bgScale);
				// viewportRectを設定
				Camera.main.rect = new Rect(0f, (1f - camHeight) / 2f, 1f, camHeight);
			}
		}
	}
}