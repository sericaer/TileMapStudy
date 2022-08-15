using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DynamicHexMutliRandomDirect : MonoBehaviour
{
    public Tilemap tilemap;
    public Sprite sprite;

    public HashSet<(int x, int y)> centers;
    public Dictionary<Color, BlockBuilder> builderDict;
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
        builderDict = Enumerable.Range(0, 100)
            .Select(_ => new BlockBuilder(100, (Random.Range(-100, 100), Random.Range(-100, 100)), centers, 
                                        new int[]
                                        {
                                            Random.Range(1,9),
                                            Random.Range(1,9),
                                            Random.Range(1,9),
                                            Random.Range(1,9),
                                            Random.Range(1,9),
                                            Random.Range(1,9),
                                        })
                    ).ToDictionary(_ => new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)), v => v);

        StartCoroutine(OnTimer());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator OnTimer()
    {
        yield return new WaitForSeconds(1);

        foreach (var builder in builderDict)
        {
            foreach (var elems in builder.Value.Build())
            {
                foreach (var elem in elems)
                {
                    var pos = new Vector3Int(elem.x, elem.y, 0);
                    SetTileColor(pos, builder.Key);
                }
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
        private bool isStart = true;
        private (int x, int y) originPoint;
        private int[] weightDirectValues;

        public BlockBuilder(int size, (int x, int y) originPoint, HashSet<(int x, int y)> centers, int[] weightDirectValues)
        {
            edges = new HashSet<(int x, int y)>();

            this.centers = centers;
            this.originPoint = originPoint;
            this.size = size;
            this.weightDirectValues = weightDirectValues;
        }

        public IEnumerable<(int x, int y)[]> Build()
        {
            if (isStart)
            {
                edges.Add(originPoint);

                yield return edges.ToArray();

                isStart = false;
            }

            var tempEdges = edges.SelectMany(x => Hexagon.GetNeighbors(x))
                .Where(n => !centers.Contains(n))
                .Where(n => (int)System.Math.Abs(n.x) < size && (int)System.Math.Abs(n.y) < size)
                .ToHashSet();

            var newEdges = new HashSet<(int x, int y)>();
            var revEdges = new HashSet<(int x, int y)>();

            foreach (var edge in tempEdges)
            {
                var oldEdges = Hexagon.GetNeighbors(edge).Where(x => edges.Contains(x)).ToArray();

                var value = oldEdges.Max(e => weightDirectValues[Hexagon.GetDirectIndex(e, edge)]);

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

            newEdges.UnionWith(revEdges);
            centers.UnionWith(newEdges);
    
            edges = newEdges;

            yield return newEdges.ToArray();
        }
    }
}
