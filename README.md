# UnityCameraCrossFade

A very simple demo using a render texture and the ScreenOverlay image effect to cross fade between two cameras. This particular setup allows one to cross-fade between two meshes without worrying about the usual z-depth rendering issues.

A few solutions I saw floating around are pretty old and avoid using Render Textures because they used to be only avalable in the paid version. Also, they use OnGUI to render the RT full screen, which immediately made my laptop fan go to full blast. So, here is an alternative.

### Scene Setup

The demo scene uses two cameras, one of which can see a cube, the other a sphere. Both cameras see the rest of the scene. This is done using layers and the Camera culling settings. This way, the other objects in the scene do not appear to change. 

The fade button simply triggers an animation that interpolates the ```intensity``` field of the ScreenOverlay image effect.

### Render Texture

For this scene, the RenderTexture was created in the editor and assigned to the second camera. In practice, it would probably be better to do this via script and creating a RT for the current resolution of the application.

Once Camera2 is set to render to the texture, it needs to be sent to the ScreenOverlay Component. However, as far as I can tell, we must copy the RT into a normal texture (I suspect the ScreenOverlay and the shader it uses could be modified to directly use the RT and save a costly copy?). This is done with the following (see [Assets/CameraRTBehavior.cs](https://github.com/bcgilliom/UnityCameraCrossFade/blob/master/Assets/CameraRTBehavior.cs)):

```C#
RenderTexture.active = renderTexture;
texture.ReadPixels (new Rect (0, 0, texture.width, texture.height), 0, 0);
texture.Apply ();
```

where ```texture``` is a reference to the ScreenOverlay texture and renderTexture is a reference to Camera2's render target.

### ScreenOverlay

ScreenOverlay is fairly self explanatory. Blend mode controls how the texture is overlayed on the scene. Intensity controls how much to blend the texture. This demo uses ```AlphaBlend``` so it evenly crossfades between the two cameras, and fakes the effect of cross fading between two meshes.

