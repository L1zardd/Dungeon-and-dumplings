using UnityEngine;

[ExecuteInEditMode]
public class SpriteSizeHelper : MonoBehaviour
{
    public bool autoDetectSize = true;

    [Space]
    public Vector2 customWholeSize;
    public Vector2 customSlicedSize;

    private Vegetable vegetable;
    private SpriteRenderer spriteRenderer;

    void OnEnable()
    {
        vegetable = GetComponent<Vegetable>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!Application.isPlaying && autoDetectSize)
        {
            UpdateSizes();
        }
    }

    void UpdateSizes()
    {
        if (vegetable != null && spriteRenderer != null)
        {
            // Автоматическое определение размеров based on current sprite
            if (spriteRenderer.sprite != null)
            {
                Vector2 currentSize = spriteRenderer.sprite.bounds.size;

                if (vegetable.wholeSprite == spriteRenderer.sprite)
                {
                    vegetable.wholeSize = currentSize;
                    customWholeSize = currentSize;
                }
                else if (vegetable.slicedSprite == spriteRenderer.sprite)
                {
                    vegetable.slicedSize = currentSize;
                    customSlicedSize = currentSize;
                }
            }
        }
    }

    // Методы для кнопок в кастомном редакторе
    public void SetWholeSize()
    {
        if (vegetable != null && spriteRenderer.sprite != null)
        {
            vegetable.wholeSize = customWholeSize;
            Debug.Log("Whole size set to: " + customWholeSize);
        }
    }

    public void SetSlicedSize()
    {
        if (vegetable != null)
        {
            vegetable.slicedSize = customSlicedSize;
            Debug.Log("Sliced size set to: " + customSlicedSize);
        }
    }
}