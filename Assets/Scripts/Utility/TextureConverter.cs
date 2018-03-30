using UnityEngine;

public class TextureConverter : MonoBehaviour {

	public static Texture2D getTexture(string file)
    {
        return (Texture2D)Resources.Load("Textures/" + file) as Texture2D;
    }

}
