using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
    [ExecuteInEditMode]
    [RequireComponent (typeof(Camera))]
    public class CutoutOverlay : PostEffectsBase
	{
		[Range(0, 1)]
        public float cutoutThreshold = 1.0f;
        public Texture2D texture = null;
		public Texture2D cutoutAlphaTexture;

		[Range(0, 1)]
		public float blendSize = 0.1f;

        public Shader overlayShader = null;
        private Material overlayMaterial = null;


		//public RenderTexture renderTexture;

        public override bool CheckResources ()
		{
            CheckSupport (false);

            overlayMaterial = CheckShaderAndCreateMaterial (overlayShader, overlayMaterial);
			//renderTexture = GameObject.Find("Camera2").GetComponent<Camera> ().targetTexture;
			//texture = new Texture2D (renderTexture.width, renderTexture.height);

            if	(!isSupported)
                ReportAutoDisable ();
            return isSupported;
        }

        void OnRenderImage (RenderTexture source, RenderTexture destination)
		{
            if (CheckResources() == false)
			{
                Graphics.Blit (source, destination);
                return;
            }

            Vector4 UV_Transform = new  Vector4(1, 0, 0, 1);

//			RenderTexture.active = renderTexture;
//			int readW = Mathf.Min (texture.width, renderTexture.width);
//			int readH = Mathf.Min (texture.height, renderTexture.height);
//			RenderTexture.active = renderTexture;
//			texture.ReadPixels (new Rect (0, 0, readW, readH), 0, 0);
//			texture.Apply ();

			#if UNITY_WP8
	    	// WP8 has no OS support for rotating screen with device orientation,
	    	// so we do those transformations ourselves.
			if (Screen.orientation == ScreenOrientation.LandscapeLeft) {
				UV_Transform = new Vector4(0, -1, 1, 0);
			}
			if (Screen.orientation == ScreenOrientation.LandscapeRight) {
				UV_Transform = new Vector4(0, 1, -1, 0);
			}
			if (Screen.orientation == ScreenOrientation.PortraitUpsideDown) {
				UV_Transform = new Vector4(-1, 0, 0, -1);
			}
			#endif

            overlayMaterial.SetVector("_UV_Transform", UV_Transform);
			overlayMaterial.SetFloat ("_CutoutThreshold", cutoutThreshold);
			overlayMaterial.SetFloat ("_BlendSize", blendSize);
            overlayMaterial.SetTexture ("_Overlay", texture);
			overlayMaterial.SetTexture ("_Cutout", cutoutAlphaTexture);

            Graphics.Blit (source, destination, overlayMaterial, 0);
        }
    }
}
