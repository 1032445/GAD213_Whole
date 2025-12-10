using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorGenerator : MonoBehaviour
{
    public GameObject floorPrefab;
    public int width = 10;    // number of tiles along X
    public int height = 10;   // number of tiles along Z
    public float tileSize = 6f; // tile is 6x6

    [ContextMenu("Generate Floor")]
    public void GenerateFloor()
    {
        if (floorPrefab == null)
        {
            Debug.LogError("Assign a floor prefab!");
            return;
        }

        // Delete old tiles inside this generator
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);

        // Build new grid
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 pos = new Vector3(x * tileSize, 0f, z * tileSize);
                Instantiate(floorPrefab, pos, Quaternion.identity, transform);
            }
        }

        Debug.Log("Generated floor: " + (width * height) + " tiles.");
    }

    [ContextMenu("Clear Floor")]
    public void ClearFloor()
    {
        // Remove all children under this generator
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);

        Debug.Log("Floor cleared.");
    }
}
