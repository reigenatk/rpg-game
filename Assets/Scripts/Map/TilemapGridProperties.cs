using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

// this script runs in the editor and the game
[ExecuteAlways]
public class TilemapGridProperties : MonoBehaviour
{

    // we filter so it only runs in editor tho
#if UNITY_EDITOR
    private Tilemap tilemap;
    [SerializeField] private SO_GridProperties gridProperties = null;
    [SerializeField] private GridBoolProperty gridBoolProperty = GridBoolProperty.isPath;

    private void OnEnable()
    {
        // Only populate in the editor
        if (!Application.IsPlaying(gameObject))
        {
            tilemap = GetComponent<Tilemap>();

            if (gridProperties != null)
            {
                gridProperties.gridPropertyList.Clear();
            }
        }
    }

    // saves everything we painted into the scriptable object
    private void OnDisable()
    {        // Only populate in the editor
        if (!Application.IsPlaying(gameObject))
        {
            UpdateGridProperties();

            if (gridProperties != null)
            {
                // This is required to ensure that the updated gridproperties gameobject gets saved when the game is saved - otherwise they are not saved.
                EditorUtility.SetDirty(gridProperties);
            }
        }
    }

    private void UpdateGridProperties()
    {
        // Compress timemap bounds
        tilemap.CompressBounds();
        Debug.Log("[TilemapGridProperties] cell bounds: " + tilemap.cellBounds);

        // Only populate in the editor
        if (!Application.IsPlaying(gameObject))
        {
            if (gridProperties != null)
            {
                Vector3Int startCell = tilemap.cellBounds.min;
                Vector3Int endCell = tilemap.cellBounds.max;

                for (int x = startCell.x; x < endCell.x; x++)
                {
                    for (int y = startCell.y; y < endCell.y; y++)
                    {
                        TileBase tile = tilemap.GetTile(new Vector3Int(x, y, 0));

                        if (tile != null)
                        {
                            // populate the scriptable object, painted tile = true value
                            gridProperties.gridPropertyList.Add(new GridProperty(new GridCoordinate(x, y), gridBoolProperty, true));
                        }
                    }
                }


            }
        }
    }

    private void Update()
    {        // Only populate in the editor
        if (!Application.IsPlaying(gameObject))
        {
            // Debug.Log("DISABLE PROPERTY TILEMAPS");
        }
    }
#endif
}