using UnityEngine;

/// <summary>
/// Procedurally generates a grass path along the waypoints at runtime.
/// Attach this to the Path_Waypoints parent object.
/// </summary>
public class PathVisualizer2D : MonoBehaviour
{
    [Header("Visual Assets")]
    public Sprite pathTileSprite;
    public Sprite bushDecoSprite;

    [Header("Settings")]
    public float tileSpacing = 1.0f;
    public int pathSortingOrder = -10;

    void Awake()
    {
        if (pathTileSprite == null || bushDecoSprite == null)
        {
            Debug.LogWarning("[PathVisualizer] Sprites not assigned. Skipping path generation.");
            return;
        }

        int childCount = transform.childCount;
        if (childCount < 2) return;

        // Create a container for the generated visuals
        GameObject visualsRoot = new GameObject("Path_Visuals");

        // Iterate through segments
        for (int i = 0; i < childCount - 1; i++)
        {
            Transform startWP = transform.GetChild(i);
            Transform endWP = transform.GetChild(i + 1);

            Vector2 start = startWP.position;
            Vector2 end = endWP.position;
            float distance = Vector2.Distance(start, end);
            Vector2 direction = (end - start).normalized;

            // Generate tiles along the segment
            for (float d = 0; d < distance; d += tileSpacing)
            {
                Vector2 pos = start + direction * d;
                SpawnTile(pos, visualsRoot.transform);

                // Randomly spawn bushes for decoration
                if (Random.value < 0.3f)
                {
                    Vector2 offset = Random.insideUnitCircle * 0.8f;
                    SpawnBush(pos + offset, visualsRoot.transform);
                }
            }
        }
    }

    private void SpawnTile(Vector2 pos, Transform parent)
    {
        GameObject go = new GameObject("PathTile");
        go.transform.position = pos;
        go.transform.parent = parent;

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = pathTileSprite;
        sr.sortingOrder = pathSortingOrder;
    }

    private void SpawnBush(Vector2 pos, Transform parent)
    {
        GameObject go = new GameObject("BushDeco");
        go.transform.position = pos;
        go.transform.parent = parent;

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = bushDecoSprite;
        sr.sortingOrder = pathSortingOrder + 1; // Slightly above the path
    }
}
