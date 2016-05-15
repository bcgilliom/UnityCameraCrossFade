using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class CameraRTBehavior : MonoBehaviour {

	CutoutOverlay screenOverlay;

	RenderTexture renderTexture;
	Texture2D texture;

	// Use this for initialization
	void Start () {
		screenOverlay = Camera.main.GetComponent<CutoutOverlay> ();
		renderTexture = GetComponent<Camera> ().targetTexture;

		texture = new Texture2D (renderTexture.width, renderTexture.height);
		screenOverlay.texture = texture;
	}

	void OnPostRender()
	{
		RenderTexture.active = renderTexture;

		int readW = Mathf.Min (texture.width, renderTexture.width);
		int readH = Mathf.Min (texture.height, renderTexture.height);

		texture.ReadPixels (new Rect (0, 0, readW, readH), 0, 0);

		texture.Apply ();
	}
}
