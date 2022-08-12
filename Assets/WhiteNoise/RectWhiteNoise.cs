using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class RectWhiteNoise : MonoBehaviour
{
    public Tilemap tilemap;
    public Sprite sprite;

    public Tile tile
    {
        get
        {
            if (_tile == null)
            {
                _tile = ScriptableObject.CreateInstance<Tile>();
                _tile.sprite = sprite;
            }

            return _tile;
        }
    }

    private Tile _tile;

    // Start is called before the first frame update
    void Start()
    {
        foreach(var pos in MapMath.Generate(100).Select(p => new Vector3Int(p.x, p.y, 0)))
        {
            tilemap.SetTile(pos, tile);

            tilemap.SetTileFlags(pos, TileFlags.None);

            tilemap.SetColor(pos, new Color(Random.Range(0f,1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class MapMath
{
    private static (int x, int y)[] NeighbourIndexs = new (int x, int y)[]
    {
        (1, 1), (1, 0), (1, -1), (0, 1), (0, -1), (-1, 1), (-1, 0), (-1, -1)
    };

    public static IEnumerable<(int x, int y)> Generate(int size)
    {
        return Enumerable.Range(size/2*-1, size).SelectMany(x => Enumerable.Range(size / 2 * -1, size).Select(y => (x, y)));
    }

    public static IEnumerable<(int x, int y)> GetNeighbours((int x, int y) coord)
    {
        return NeighbourIndexs.Select(index => (coord.x + index.x, coord.y + index.y));
    }
}