using UnityEngine;

/// <summary>
/// World-space HP bar displayed above the monster.
/// Self-contained: creates child sprite renderers at runtime.
/// Call SetHealth(current, max) to update.
/// </summary>
public class HealthBar2D : MonoBehaviour
{
    [Header("Bar Layout")]
    public float barWidth  = 0.85f;
    public float barHeight = 0.12f;
    public Vector2 offset  = new Vector2(0f, 0.68f);

    private Transform      _fgTransform;
    private SpriteRenderer _fgRenderer;

    // ---------------------------------------------------------
    // Unity
    // ---------------------------------------------------------
    void Start()
    {
        BuildBarObjects();
        SetHealth(1, 1);   // start full
    }

    // ---------------------------------------------------------
    // Public API
    // ---------------------------------------------------------
    /// <summary>Update the HP bar display.</summary>
    public void SetHealth(int current, int max)
    {
        if (_fgTransform == null) return;
        float ratio      = (max > 0) ? Mathf.Clamp01((float)current / max) : 0f;
        float targetW    = barWidth * ratio;

        _fgTransform.localScale    = new Vector3(targetW, barHeight, 1f);
        _fgTransform.localPosition = new Vector3(
            offset.x - (barWidth - targetW) * 0.5f,
            offset.y, 0f);

        // Green -> Yellow -> Red
        if (_fgRenderer != null)
        {
            _fgRenderer.color = ratio > 0.5f
                ? Color.Lerp(Color.yellow, new Color(0.15f, 0.9f, 0.15f), (ratio - 0.5f) * 2f)
                : Color.Lerp(Color.red,    Color.yellow,                    ratio * 2f);
        }
    }

    // ---------------------------------------------------------
    // Private
    // ---------------------------------------------------------
    private void BuildBarObjects()
    {
        // Background
        var bgGO = new GameObject("HP_BG");
        bgGO.transform.SetParent(transform, false);
        bgGO.transform.localPosition = new Vector3(offset.x, offset.y, 0f);
        bgGO.transform.localScale    = new Vector3(barWidth + 0.06f, barHeight + 0.04f, 1f);
        var bgSR = bgGO.AddComponent<SpriteRenderer>();
        bgSR.sprite       = PixelSprite(new Color(0.05f, 0.05f, 0.05f, 0.9f));
        bgSR.sortingOrder = 20;

        // Foreground
        var fgGO = new GameObject("HP_FG");
        fgGO.transform.SetParent(transform, false);
        fgGO.transform.localPosition = new Vector3(offset.x, offset.y, 0f);
        fgGO.transform.localScale    = new Vector3(barWidth, barHeight, 1f);
        _fgRenderer = fgGO.AddComponent<SpriteRenderer>();
        _fgRenderer.sprite       = PixelSprite(Color.white);
        _fgRenderer.sortingOrder = 21;
        _fgTransform = fgGO.transform;
    }

    private static Sprite PixelSprite(Color color)
    {
        var tex = new Texture2D(1, 1) { filterMode = FilterMode.Point };
        tex.SetPixel(0, 0, color);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
    }
}
