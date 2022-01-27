using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardTiler : MonoBehaviour
{
    public Vector2Int TileCount;
    public float TileSize = 1f;
    public bool CenterBoard;
    public GameObject prefabTile1;
    public GameObject prefabTile2;

    Vector2 boardSize;

    private void OnValidate()
    {
        InitBoard();
    }

    private void Awake()
    {
        InitBoard();
    }

    void InitBoard()
    {
        DeleteBoard();
        ExtensionMethods.DelayedInvoke(-2, this, () =>
        {
            CreateBoard();
        });
        boardSize = new Vector2(TileCount.x * TileSize, TileCount.y * TileSize);
    }

    void DeleteBoard()
    {
        foreach(Transform t in this.transform)
        {
            ExtensionMethods.DelayedInvoke(-1, this, () =>
            {
                if (t != null)
                    DestroyImmediate(t?.gameObject);
            });
        }
    }

    void CreateBoard()
    {
        for(int x = 0; x < TileCount.x; x++)
        {
            for(int y = 0; y < TileCount.y; y++)
            {
                Vector2 spawnPosXY = new Vector2(x, y) * TileSize + (Vector2)transform.position;

                if(CenterBoard)
                {
                    Vector2 centeringOffset = -boardSize * 0.5f + Vector2.one * TileSize * 0.5f;
                    spawnPosXY += centeringOffset;
                }

                Vector3 spawnPos = (Vector3)spawnPosXY + Vector3.forward * transform.position.z;

                int tileType = (x + y) % 2;
                GameObject prefabToSpawn = tileType == 0 ? prefabTile1 : prefabTile2;

                if (prefabToSpawn == null) continue;
                
                GameObject spawnedTile = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity, this.transform);
                spawnedTile.transform.localScale = Vector3.one * TileSize;
            }
        }
    }
}
