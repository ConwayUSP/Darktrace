using UnityEngine;
using System.Collections.Generic;

[ExecuteAlways]
public class DesktopGrid : MonoBehaviour
{
    public float spacingX = 20f;
    public float spacingY = 30f;
    public Vector2 startOffset = new Vector2(50, -50);
    public int maxRows = 5;

    public static DesktopGrid Instance;
    private List<Vector2> slotPositions = new List<Vector2>();
    private HashSet<int> occupiedSlots = new HashSet<int>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Gera grid
        GenerateGrid();

        // Encaixa os filhos na grid
        var filhos = FindObjectsByType<DesktopAppIcon>(FindObjectsSortMode.InstanceID);
        Debug.Log(filhos.Length);
        foreach (var item in filhos)
        {
            item.GetSlot();
        }
    }

    void OnRectTransformDimensionsChange()
    {
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        slotPositions.Clear();
        occupiedSlots.Clear();

        RectTransform rect = GetComponent<RectTransform>();
        if (!rect)
        {
            Debug.LogWarning("DesktopGrid precisa de um RectTransform!");
            return;
        }

        float gridWidth = rect.rect.width;
        float gridHeight = rect.rect.height;

        int columns = Mathf.Max(1, Mathf.FloorToInt((gridWidth - startOffset.x) / (DesktopAppIcon.WIDTH + spacingX)));

        float startX = startOffset.x;
        float startY = startOffset.y;

        for (int y = 0; y < maxRows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                float posX = startX + x * (DesktopAppIcon.WIDTH + spacingX);
                float posY = startY - y * (DesktopAppIcon.HEIGHT + spacingY);
                slotPositions.Add(new Vector2(posX, posY));
            }
        }

#if UNITY_EDITOR
        if (!Application.isPlaying)
            UnityEditor.SceneView.RepaintAll();
#endif
    }

    public Vector2 GetClosestFreeSlot(Vector2 position)
    {
        if (slotPositions.Count == 0)
            GenerateGrid();

        int bestIndex = -1;
        float minDist = float.MaxValue;

        for (int i = 0; i < slotPositions.Count; i++)
        {
            if (occupiedSlots.Contains(i)) continue;

            float dist = Vector2.Distance(slotPositions[i], position);
            if (dist < minDist)
            {
                minDist = dist;
                bestIndex = i;
            }
        }

        if (bestIndex == -1)
            bestIndex = 0; // fallback

        occupiedSlots.Add(bestIndex);
        return slotPositions[bestIndex];
    }

    public void FreeSlot(Vector2 position)
    {
        for (int i = 0; i < slotPositions.Count; i++)
        {
            if (Vector2.Distance(slotPositions[i], position) < 0.1f)
            {
                occupiedSlots.Remove(i);
                break;
            }
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        foreach (var slot in slotPositions)
        {
            Vector3 worldPos = transform.TransformPoint(slot);
            Gizmos.DrawWireCube(worldPos, new Vector3(DesktopAppIcon.WIDTH, DesktopAppIcon.HEIGHT, 0));
        }
    }
#endif
}
