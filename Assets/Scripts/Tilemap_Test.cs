using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class Tilemap_Test : MonoBehaviour
{
    // Assign these fields in the inspector or use GetComponent
    public Tilemap tilemap;
    public Grid grid;

    // A list to store the positions of the occupied cells
    public List<Vector3> occupiedCells;

    void Start()
    {
        // Initialize the list
        occupiedCells = new List<Vector3>();

        // Loop through the cell bounds of the tilemap
        foreach (Vector3Int cellPosition in tilemap.cellBounds.allPositionsWithin)
        {
            // Check if there is a tile at the cell position
            if (tilemap.GetTile(cellPosition) != null)
            {
                // Get the world position of the center of the cell
                Vector3 worldPosition = grid.CellToWorld(cellPosition);

                // Alternatively, get the local position of the top-right corner of the cell
                // Vector3 localPosition = grid.CellToLocalInterpolated(cellPosition + new Vector3(0.5f, 0.5f, 0));

                // Add the position to the list
                occupiedCells.Add(worldPosition);
                Debug.Log(worldPosition);
            }
        }
    }
}