using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DynamicMapWithDirect : MonoBehaviour
{
    public Tilemap tilemap;
    public Sprite sprite;

    public HashSet<(int x, int y)> centers;
    public HashSet<(int x, int y)> edges;

    public Dictionary<(int x, int y), int> dictPowerValue = new Dictionary<(int x, int y), int>()
    {
        { (0, 1), 10},
        { (1, 1), 40},
        { (1, 0), 100},
        { (1, -1), 50},
        { (0, -1), 50},
        { (-1, -1), 50},
        { (-1, 0), 100},
        { (-1, 1), 50},
    };

    public Dictionary<(int x, int y), int> dictCalcValue = new Dictionary<(int x, int y), int>();

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

        var dict = new Dictionary<(int x, int y), int>();
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

        var tempEdges = edges.SelectMany(x => MapMath.GetNeighbours(x)).Where(n => !centers.Contains(n) && !edges.Contains(n)).ToHashSet();
        var newEdges = new HashSet<(int x, int y)>();
        var revEdges = new HashSet<(int x, int y)>();

        foreach (var edge in tempEdges)
        {
            var oldEdges = MapMath.GetNeighbours(edge).Where(x => edges.Contains(x)).ToArray();

            var value = oldEdges.Max(e => dictPowerValue[(edge.x-e.x, edge.y-e.y)]);
            if(!dictCalcValue.ContainsKey(edge))
            {
                dictCalcValue[edge] = value;
            }
            else
            {
                dictCalcValue[edge] += value;
            }

            if(dictCalcValue[edge] >= 100)
            {
                var pos = new Vector3Int(edge.x, edge.y, 0);
                var color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

                SetTileColor(pos, color);
                newEdges.Add(edge);
            }
            else
            {
                revEdges.UnionWith(oldEdges);
            }
        }

        centers.UnionWith(edges.Except(revEdges));
        edges = newEdges.Union(revEdges).ToHashSet();


        StartCoroutine(OnTimer());
    }

    private void SetTileColor(Vector3Int pos, Color color)
    {
        tilemap.SetTile(pos, tile);
        tilemap.SetTileFlags(pos, TileFlags.None);
        tilemap.SetColor(pos, color);
    }

}
