using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiTargetController : MonoBehaviour
{
    public static MultiTargetController Instance;

    public List<TargetController> allTargets = new List<TargetController>();
    public TargetController activeTarget;
    public GameObject targetPrefab;
    public Transform orbitCenter;

    [Header("Level Settings")]
    public int baseInactiveTargets = 1;
    public int inactiveTargetsPerLevel = 1;
    public int maxTotalTargets = 10;

    private int currentLevel = 1;

    void Awake()
    {
       

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        
        InitializeTargets();
    }

    public void InitializeTargets(int level = 1)
    {
        currentLevel = level;
        int inactiveCount = baseInactiveTargets + (level - 1) * inactiveTargetsPerLevel;
        int totalTargets = Mathf.Min(1 + inactiveCount, maxTotalTargets);

        ClearTargets();
        CreateTargets(totalTargets);
        StartCoroutine(DelayedActivation());
    }

    private IEnumerator DelayedActivation()
    {
        yield return new WaitForEndOfFrame();
        ActivateRandomTarget();
    }

    private void ClearTargets()
    {
        
        foreach (var target in allTargets)
        {
            if (target != null) Destroy(target.gameObject);
        }
        allTargets.Clear();
        activeTarget = null;
    }

    private void CreateTargets(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject newTarget = Instantiate(targetPrefab, Vector3.zero, Quaternion.identity, transform);
            TargetController controller = newTarget.GetComponent<TargetController>();
            allTargets.Add(controller);

            controller.SetActiveState(false); // Сначала неактивна
            controller.PlaceAtRandomPosition(); // Позиционируем
        }
    }

    public void OnTargetHit(TargetController hitTarget)
    {
        if (hitTarget == activeTarget)
        {
            hitTarget.SetActiveState(false);
            activeTarget = null;

            // Деактивируем все мишени перед активацией новой
            foreach (var target in allTargets)
            {
                if (target != null) target.SetActiveState(false);
            }

            ActivateRandomTarget();

            // Добавляем новую неактивную мишень
            AddNewInactiveTarget();
        }
    }

    private void AddNewInactiveTarget()
    {
        if (allTargets.Count >= maxTotalTargets) return;

        GameObject newTarget = Instantiate(targetPrefab, Vector3.zero, Quaternion.identity, transform);
        TargetController controller = newTarget.GetComponent<TargetController>();
        allTargets.Add(controller);
        controller.SetActiveState(false);
        controller.PlaceAtRandomPosition();
    }

    private void ActivateRandomTarget()
    {
        // Деактивируем все мишени
        foreach (var target in allTargets)
        {
            if (target != null)
            {
                // Принудительно обновляем состояние
                target.SetActiveState(false);
            }
        }

        List<TargetController> inactiveTargets = GetInactiveTargets();

        if (inactiveTargets.Count > 0)
        {
            int randomIndex = Random.Range(0, inactiveTargets.Count);
            activeTarget = inactiveTargets[randomIndex];
            activeTarget.SetActiveState(true); // Будет применён активный цвет
        }
    }

    private List<TargetController> GetInactiveTargets()
    {
        Debug.Log("Added new inactive target");
        List<TargetController> inactive = new List<TargetController>();
        foreach (var target in allTargets)
        {
            if (target != null && !target.IsActive())
            {
                inactive.Add(target);
            }
        }
        return inactive;
        
    }
}