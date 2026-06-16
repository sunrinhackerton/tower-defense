using UnityEngine;

/// <summary>
/// Destroys the GameObject after a set duration.
/// Useful for visual effects like slashes or explosions.
/// </summary>
public class SelfDestruct2D : MonoBehaviour
{
    public float lifetime = 0.2f;
    public bool animate = false;
    
    private SpriteRenderer _sr;
    private Color _startColor;
    private Vector3 _startScale;
    private float _timer;

    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        if (_sr != null) _startColor = _sr.color;
        _startScale = transform.localScale;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (!animate) return;
        
        _timer += Time.deltaTime;
        float t = _timer / lifetime;

        transform.localScale = Vector3.Lerp(_startScale, _startScale * 1.5f, t);
        if (_sr != null)
        {
            _sr.color = new Color(_startColor.r, _startColor.g, _startColor.b, Mathf.Lerp(_startColor.a, 0f, t));
        }
    }
}
