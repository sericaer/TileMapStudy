using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DynamicMapWithRandomDirect : MonoBehaviour
{
    public Tilemap tilemap;
    public Sprite sprite;

    public HashSet<(int x, int y)> centers;
    public HashSet<(int x, int y)> edges;

    public Dictionary<(int x, int y), int> dictPowerValue = new Dictionary<(int x, int y), int>()
    {
        { (0, 1), 9},
        { (1, 1), 4},
        { (1, 0), 8},
        { (1, -1), 5},
        { (0, -1), 9},
        { (-1, -1), 5},
        { (-1, 0), 10},
        { (-1, 1), 5},
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

    private BlockBuilder blockBuilder;

    // Start is called before the first frame update
    void Start()
    {
        blockBuilder = new BlockBuilder(100, dictPowerValue);

        StartCoroutine(OnTimer());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator OnTimer()
    {
        yield return new WaitForSeconds(1);

        foreach (var elems in blockBuilder.Build())
        {
            foreach (var elem in elems)
            {
                var pos = new Vector3Int(elem.x, elem.y, 0);
                var color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));

                SetTileColor(pos, color);
            }
        }

        StartCoroutine(OnTimer());
    }

    private void SetTileColor(Vector3Int pos, Color color)
    {
        tilemap.SetTile(pos, tile);
        tilemap.SetTileFlags(pos, TileFlags.None);
        tilemap.SetColor(pos, color);
    }


    public class BlockBuilder
    {
        private HashSet<(int x, int y)> centers;
        private HashSet<(int x, int y)> edges;
        private int size;
        private Dictionary<(int x, int y), int> dictWeight;
        private Dictionary<(int x, int y), int> dictCalcValue;
        public BlockBuilder(int size, Dictionary<(int x, int y), int> dictWeight)
        {
            centers = new HashSet<(int x, int y)>();
            edges = new HashSet<(int x, int y)>();

            this.size = size;
            this.dictWeight = dictWeight;
            this.dictCalcValue = new Dictionary<(int x, int y), int>();
        }

        public IEnumerable<(int x, int y)[]> Build()
        {
            if (centers.Count() == 0)
            {
                var startIndex = (0, 0);
                edges.Add(startIndex);
                centers.Add(startIndex);

                yield return edges.ToArray();
            }

            var tempEdges = edges.SelectMany(x => MapMath.GetNeighbours(x)).Where(n => !centers.Contains(n) && !edges.Contains(n)).ToHashSet();
            var newEdges = new HashSet<(int x, int y)>();
            var revEdges = new HashSet<(int x, int y)>();

            foreach (var edge in tempEdges)
            {
                var oldEdges = MapMath.GetNeighbours(edge).Where(x => edges.Contains(x)).ToArray();

                var value = oldEdges.Max(e => dictWeight[(edge.x - e.x, edge.y - e.y)]);

                var real = Random.Range(value, 11);
                //var real = value;

                if (!dictCalcValue.ContainsKey(edge))
                {
                    dictCalcValue[edge] = real;
                }
                else
                {
                    dictCalcValue[edge] += real;
                }

                if (dictCalcValue[edge] >= 10)
                {
                    newEdges.Add(edge);
                }
                else
                {
                    revEdges.UnionWith(oldEdges);
                }
            }

            centers.UnionWith(edges.Except(revEdges));
            edges = newEdges.Union(revEdges).ToHashSet();

            yield return newEdges.ToArray();
        }
    }
}
