using UnityEngine;

public class ImageBlur : MonoBehaviour
{
    public Texture2D originalTexture;
    public float blurRadius = 2.0f; // Радиус размытия

    void Start()
    {
        if (originalTexture != null)
        {
            Texture2D blurredTexture = ApplyBlur(originalTexture, blurRadius);
            // Здесь вы можете применить blurredTexture к UI элементу или Material'у
            GetComponent<Renderer>().material.mainTexture = blurredTexture;
        }
    }

    // Простой пример метода размытия. Для более сложных эффектов лучше использовать шейдеры.
    Texture2D ApplyBlur(Texture2D texture, float radius)
    {
        Texture2D blurred = new Texture2D(texture.width, texture.height);
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                Color avgColor = Color.clear;
                for (int i = -Mathf.RoundToInt(radius); i <= Mathf.RoundToInt(radius); i++)
                {
                    for (int j = -Mathf.RoundToInt(radius); j <= Mathf.RoundToInt(radius); j++)
                    {
                        int sampleX = Mathf.Clamp(x + i, 0, texture.width - 1);
                        int sampleY = Mathf.Clamp(y + j, 0, texture.height - 1);
                        avgColor += texture.GetPixel(sampleX, sampleY);
                    }
                }
                blurred.SetPixel(x, y, avgColor / ((2 * Mathf.RoundToInt(radius) + 1) * (2 * Mathf.RoundToInt(radius) + 1)));
            }
        }
        blurred.Apply();
        return blurred;
    }
}