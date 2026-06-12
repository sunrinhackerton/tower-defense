using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Placed on a Build Site sprite in the scene.
/// When the player clicks this object and has enough coins,
/// it deducts the cost and instantiates a tower prefab at this location.
/// Prevents duplicate builds via hasBuilt flag.
/// Uses IPointerClickHandler for compatibility with the new Input System UI.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class BuildSite2D : MonoBehaviour, IPointerClickHandler
{
    [Header("Tower Build")]
    public GameObject towerPrefab;
    public int        towerCost = 100;

    [Header("Visual Feedback")]
    [Tooltip("Color shown when the site is available to build.")]
    public Color availableColor = new Color(0.4f, 1f, 0.4f, 0.35f);
    [Tooltip("Color shown after a tower is built here.")]
    public Color builtColor     = new Color(0.5f, 0.5f, 0.5f, 0.15f);

    // State
    public bool HasTower { get; private set; }

    private SpriteRenderer _sr;

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _sr.color = availableColor;

        // Ensure collider is a trigger so it doesn't block physics
        var col = GetComponent<BoxCollider2D>();
        col.isTrigger = false;   // must be non-trigger for Raycaster to work reliably
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (HasTower)
        {
            Debug.Log("[BuildSite] 이미 타워가 배치된 자리입니다.");
            return;
        }

        if (BuildManager2D.Instance != null)
        {
            BuildManager2D.Instance.OpenBuildUI(this);
        }
    }

    public void SetBuilt()
    {
        HasTower = true;
        _sr.color = builtColor;
    }
}
