using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapGenerator : MonoBehaviour
{
    
    [SerializeField] private Tilemap grid;
    [SerializeField] private Tile[] tiles;

    private void Start() {
        GenerateMap(50, 50);
    }

    private void GenerateMap(int x, int y) {
        for(int i = -(x/2); i < x/2; i++) {
            for (int j = -(y/2); j < y/2; j++) {
                grid.SetTile(new Vector3Int(i,j,0), tiles[0]);
            }
        }
    }
}
