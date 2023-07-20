using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using Unity.Netcode;
using System.Collections;

[System.Serializable]
public class TileDictionaryEntry
{
    public float noiseValue;
    public TileBase tile;
    public bool hasCollision;
}

public class WorldGenerator : NetworkBehaviour
{
    [SerializeField] private Vector2Int mapSize = new Vector2Int(100,100);
    [SerializeField] private Tilemap map;
    [SerializeField] private List<TileDictionaryEntry> tileDictionary;

    [SerializeField] private float scale = 0.1f;

    IEnumerator Start() {
        yield return new WaitForSeconds(10);
        GenerateAllWorld();
        Debug.Log("Generating world");
    }

    void GenerateAllWorld()
    {
        // Generate the world with specified size, tilemap, and tile dictionary
        if (IsHost || IsServer) GenerateWorldClientRpc(); // Generate on clients
        if (IsServer) GenerateWorld(); // Generate on server
    }

    [ClientRpc]
    void GenerateWorldClientRpc() {
        GenerateWorld();
    }

    // Generate the world based on the size, tilemap, and tile dictionary
    void GenerateWorld()
    {
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                // Calculate the normalized coordinates by multiplying with the scale
                float perlinValue = Mathf.PerlinNoise(x * scale, y * scale);

                // Find the best matching noise value in the tile dictionary
                TileBase selectedTile = null;

                for (int i = 0; i < tileDictionary.Count; i++)
                {
                    if(perlinValue < tileDictionary[i].noiseValue) 
                    {
                        selectedTile = tileDictionary[i].tile; 
                        break;
                    }
                }

                // Set the tile based on the selected TileBase from the dictionary
                map.SetTile(new Vector3Int(x, y, 0), selectedTile);

            }
        }
    }
}
