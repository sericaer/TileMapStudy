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
        foreach(var pos in Enumerable.Range(0, 100).SelectMany(x => Enumerable.Range(0, 100).Select(y => new Vector3Int(x, y, 0))))
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
