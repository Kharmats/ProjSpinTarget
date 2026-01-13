using UnityEngine;

public class TargetController : MonoBehaviour
{
    [Header("References")]
    public SpriteRenderer targetRenderer;
    private TargetSkinApplier skinApplier;

    [Header("Position Settings")]
    public float minRadius = 2f;
    public float maxRadius = 5f;
    public float minAngle = 0f;
    public float maxAngle = 360f;

    private bool isActive = false;

    void Awake()
    {
        

        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<SpriteRenderer>();
           
        }

        skinApplier = GetComponent<TargetSkinApplier>();
        
    }

    void Start()
    {
        
       // PlaceAtRandomPosition();
    }

    public void PlaceAtRandomPosition()
    {
        float randomRadius = Random.Range(minRadius, maxRadius);
        float randomAngle = Random.Range(minAngle, maxAngle) * Mathf.Deg2Rad;

        float x = Mathf.Cos(randomAngle) * randomRadius;
        float y = Mathf.Sin(randomAngle) * randomRadius;

        transform.position = new Vector3(x, y, 0);
    }

    public void SetActiveState(bool active)
    {
        isActive = active;

        // Всегда получаем ссылку если её нет
        if (skinApplier == null)
        {
            skinApplier = GetComponent<TargetSkinApplier>();
        }

        if (skinApplier != null && targetRenderer != null)
        {
            targetRenderer.color = active ?
                skinApplier.GetActiveColor() :
                skinApplier.GetInactiveColor();
        }

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = active;
        }
    }

    public bool IsActive()
    {
        return isActive;
    }
}