using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DynamicRectMap : MonoBehaviour
{
    public Tilemap tilemap;
    public Sprite sprite;

    public HashSet<(int x, int y)> centers;
    public HashSet<(int x, int y)> edges;

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

        centers = new HashSet<(int x, int y)>();
        edges = new HashSet<(int x, int y)>();

        var startCoord = (x: 0, y: 0);

        var pos = new Vector3Int(startCoord.x, startCoord.y, 0);
        var color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

        SetTileColor(pos, color);

        edges.Add(startCoord);
        centers.Add(startCoord);

        StartCoroutine(OnTimer());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator OnTimer()
    {
        yield return new WaitForSeconds(1);

        var newEdges = edges.SelectMany(x => MapMath.GetNeighbours(x)).Where(n => !centers.Contains(n)).ToHashSet();
        foreach (var edge in newEdges)
        {
            var pos = new Vector3Int(edge.x, edge.y, 0);
            var color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            
            SetTileColor(pos, color);
        }
        centers.UnionWith(newEdges);

        edges = newEdges;


        StartCoroutine(OnTimer());
    }

    private void SetTileColor(Vector3Int pos, Color color)
    {
        tilemap.SetTile(pos, tile);
        tilemap.SetTileFlags(pos, TileFlags.None);
        tilemap.SetColor(pos, color);
    }

}
