using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DynamicHexMapWithRandomDirect : MonoBehaviour
{
    public Tilemap tilemap;
    public Sprite sprite;

    public HashSet<(int x, int y)> centers;
    public HashSet<(int x, int y)> edges;

    public int[] weightDirectValues = new int[]
    {
      10,
      10,
      10,
      10,
      10,
      10
    };

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
        blockBuilder = new BlockBuilder(100, weightDirectValues);

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
        private int[] weightDirectValues;

        public BlockBuilder(int size, int[] weightDirectValues)
        {
            centers = new HashSet<(int x, int y)>();
            edges = new HashSet<(int x, int y)>();

            this.size = size;
            this.weightDirectValues = weightDirectValues;
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

            var tempEdges = edges.SelectMany(x => Hexagon.GetNeighbors(x))
                .Where(n => !centers.Contains(n) && !edges.Contains(n))
                .Where(n => (int)System.Math.Abs(n.x) < size && (int)System.Math.Abs(n.y) < size)
                .ToHashSet();

            var newEdges = new HashSet<(int x, int y)>();
            var revEdges = new HashSet<(int x, int y)>();

            foreach (var edge in tempEdges)
            {
                var oldEdges = Hexagon.GetNeighbors(edge).Where(x => edges.Contains(x)).ToArray();

                var value = oldEdges.Max(e => weightDirectValues[Hexagon.GetDirectIndex(edge, e)]);


                var real = Random.Range(1, 11);
                if (real <= value)
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
