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

        public Shader overlayShader = null;
        private Material overlayMaterial = null;

        public override bool CheckResources ()
		{
            CheckSupport (false);

            overlayMaterial = CheckShaderAndCreateMaterial (overlayShader, overlayMaterial);

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
            overlayMaterial.SetTexture ("_Overlay", texture);
			overlayMaterial.SetTexture ("_Cutout", cutoutAlphaTexture);
            Graphics.Blit (source, destination, overlayMaterial, 0);
        }
    }
}
