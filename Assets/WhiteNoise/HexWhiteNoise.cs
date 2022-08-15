using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HexWhiteNoise : MonoBehaviour
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
        foreach (var pos in MapMath.Generate(100).Select(p => new Vector3Int(p.x, p.y, 0)))
        {
            tilemap.SetTile(pos, tile);

            tilemap.SetTileFlags(pos, TileFlags.None);

            tilemap.SetColor(pos, new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
