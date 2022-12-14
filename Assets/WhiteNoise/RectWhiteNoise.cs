using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System;

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

            tilemap.SetColor(pos, new Color(UnityEngine.Random.Range(0f,1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f)));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class MapMath
{

    public readonly static (int x, int y)[] NeighbourIndexsRect = new (int x, int y)[]
    {
        (0, 1), (1, 1), (1, 0), (1, -1), (0, -1), (-1, -1), (-1, 0), (-1, 1)
    };

    public static IEnumerable<(int x, int y)> Generate(int size)
    {
        return Enumerable.Range(size/2*-1, size).SelectMany(x => Enumerable.Range(size / 2 * -1, size).Select(y => (x, y)));
    }

    public static IEnumerable<(int x, int y)> GetNeighbours((int x, int y) coord)
    {
        return NeighbourIndexsRect.Select(index => (coord.x + index.x, coord.y + index.y));
    }

    internal static int GetDirect((int x, int y) target, (int x, int y) origin)
    {
        var direct = (target.x - origin.x, target.y - origin.y);
        return Array.IndexOf(NeighbourIndexsRect, direct);
    }

    internal static IEnumerable<(int x, int y)> GetRings((int x, int y) pos, int dist)
    {
        var range = Enumerable.Range(dist * -1, dist * 2+1);

        return range.Select(x => (x, dist))
            .Concat(range.Select(x => (x, dist * -1)))
            .Concat(range.Select(y => (dist, y)))
            .Concat(range.Select(y => (dist * -1, y))).Distinct()
            .Select(index=> (pos.x + index.Item1, pos.y+index.Item2));
    }
}