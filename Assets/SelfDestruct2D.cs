using UnityEngine;

/// <summary>
/// Destroys the GameObject after a set duration.
/// Useful for visual effects like slashes or explosions.
/// </summary>
public class SelfDestruct2D : MonoBehaviour
{
    public float lifetime = 0.2f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
