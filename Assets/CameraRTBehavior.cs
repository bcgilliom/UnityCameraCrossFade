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
	
	// Update is called once per frame
	void Update () {
		RenderTexture.active = renderTexture;

		texture.ReadPixels (new Rect (0, 0, texture.width, texture.height), 0, 0);

		texture.Apply ();
	}
}
