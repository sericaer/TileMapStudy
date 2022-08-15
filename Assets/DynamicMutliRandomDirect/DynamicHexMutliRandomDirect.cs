using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DynamicHexMutliRandomDirect : MonoBehaviour
{
    public Tilemap tilemap;
    public Sprite sprite;

    public BlockBuilderGroup builderGroup;

    public HashSet<Color> colors;

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
        colors = new HashSet<Color>();
        while (colors.Count < 100)
        {
            colors.Add(new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
        }

        builderGroup = new BlockBuilderGroup(100, 100);

        //StartCoroutine(OnTimer());

        var rslt = builderGroup.Build();
        for (int i = 0; i < rslt.Length; i++)
        {
            foreach (var elem in rslt[i])
            {
                var pos = new Vector3Int(elem.x, elem.y, 0);
                SetTileColor(pos, colors.ElementAt(i));
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator OnTimer()
    {
        yield return new WaitForSeconds(1);

        var stepResults = builderGroup.BuildInStep();
        for(int i=0; i< stepResults.Length; i++)
        {
            foreach(var elem in stepResults[i].elements)
            {
                var pos = new Vector3Int(elem.x, elem.y, 0);
                SetTileColor(pos, colors.ElementAt(i));
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

    public class BlockBuilderGroup
    {
        private IEnumerable<Builder> builders;

        public BlockBuilderGroup(int size, int blockCount)
        {
            Builder.isExist = (n) =>
            {
                return builders.Any(x => x.centers.Contains(n));
            };

            builders = Enumerable.Range(0, blockCount)
                .Select(_ => new Builder(100, 
                                            (Random.Range(-100, 100), Random.Range(-100, 100)), 
                                            new int[]
                                            {
                                                Random.Range(1,10),
                                                Random.Range(1,10),
                                                Random.Range(1,10),
                                                Random.Range(1,10),
                                                Random.Range(1,10),
                                                Random.Range(1,10),
                                            })
                        ).ToArray();
        }

        public StepResult[] BuildInStep()
        {
            return builders.Select(x => x.BuildInStep()).ToArray();
        }

        public (int x, int y)[][] Build()
        {
            do
            {
                foreach (var builder in builders.Where(x => !x.isFinish))
                {
                    builder.BuildInStep();
                }
            } while (builders.Any(x => !x.isFinish));

            return builders.Select(x => x.centers.ToArray()).ToArray();
        }

        public class StepResult
        {
            public (int x, int y)[] elements;
        }

        private class Builder
        {
            internal static System.Func<(int x, int y), bool> isExist { get; set; }

            public bool isFinish { get; private set; } = false;

            public HashSet<(int x, int y)> centers;
            private HashSet<(int x, int y)> edges;
            private int size;
            private bool isStart = true;

            private (int x, int y) originPoint;
            private int[] weightDirectValues;

            public Builder(int size, (int x, int y) originPoint, int[] weightDirectValues)
            {
                edges = new HashSet<(int x, int y)>();

                this.centers = new HashSet<(int x, int y)>();
                this.originPoint = originPoint;
                this.size = size;
                this.weightDirectValues = weightDirectValues;
            }

            public StepResult BuildInStep()
            {
                if (isStart)
                {
                    isStart = false;

                    edges.Add(originPoint);

                    return new StepResult() { elements = edges.ToArray() };
                }

                var tempEdges = edges.SelectMany(x => Hexagon.GetNeighbors(x))
                    .Where(n => !isExist(n))
                    .Where(n => (int)System.Math.Abs(n.x) < size && (int)System.Math.Abs(n.y) < size)
                    .ToHashSet();

                isFinish = tempEdges.Count() == 0;
                
                var newEdges = new HashSet<(int x, int y)>();
                var revEdges = new HashSet<(int x, int y)>();

                foreach (var edge in tempEdges)
                {
                    var oldEdges = Hexagon.GetNeighbors(edge).Where(x => edges.Contains(x)).ToArray();

                    var value = oldEdges.Max(e => weightDirectValues[Hexagon.GetDirectIndex(e, edge)]);

                    var real = Random.Range(1, 10);
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

                return new StepResult() { elements = edges.ToArray() };
            }
        }
    }
}
