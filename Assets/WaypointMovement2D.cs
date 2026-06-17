using UnityEngine;

/// <summary>
/// Moves a GameObject sequentially through an array of waypoint Transforms.
/// Supports straight flight (isFlying) and stopping for combat (isBlocked).
/// Destroys the GameObject upon reaching the final waypoint.
/// </summary>
public class WaypointMovement2D : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform[] waypoints;

    [Header("Movement Options")]
    public float speed          = 3.5f;
    [HideInInspector] public float baseSpeed;
    [HideInInspector] public float slowMultiplier = 1f;
    private float _slowTimer = 0f;

    public float arrivalRadius  = 0.15f;
    public bool  isFlying       = false; // If true, ignore intermediate waypoints

    [Header("Combat State")]
    public bool isBlocked       = false; // Set to true by SoldierAI to stop movement

    private int  _wpIndex;
    private bool _arrived;
    
    // For flying
    private Vector2 _flyStartPos;
    private Vector2 _flyEndPos;
    private bool _flyInitialized = false;

    void Start()
    {
        baseSpeed = speed;
    }

    void Update()
    {
        if (_slowTimer > 0)
        {
            _slowTimer -= Time.deltaTime;
            if (_slowTimer <= 0)
            {
                slowMultiplier = 1f;
                speed = baseSpeed;
            }
        }

        if (_arrived || isBlocked || waypoints == null || waypoints.Length == 0) return;

        Transform target = waypoints[_wpIndex];
        if (target == null)
        {
            AdvanceWaypoint();
            return;
        }

        // Move toward current waypoint
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime);

        // Arrival check
        if (Vector2.Distance(transform.position, target.position) <= arrivalRadius)
            AdvanceWaypoint();
    }

    private void AdvanceWaypoint()
    {
        _wpIndex++;
        if (_wpIndex >= waypoints.Length)
        {
            _arrived = true;
            if (GameManager2D.Instance != null)
            {
                GameManager2D.Instance.TakeBaseDamage(1);
            }
            Destroy(gameObject);
        }
    }

    public void ApplySlow(float factor, float duration)
    {
        if (factor < slowMultiplier) // Apply strongest slow
        {
            slowMultiplier = factor;
            speed = baseSpeed * slowMultiplier;
        }
        _slowTimer = Mathf.Max(_slowTimer, duration);
    }
}
