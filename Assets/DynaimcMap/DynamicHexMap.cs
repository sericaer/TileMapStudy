using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class DynamicHexMap : MonoBehaviour
{
    public Tilemap tilemap;
    public Sprite sprite;

    //public HashSet<(int x, int y)> centers;
    //public HashSet<(int x, int y)> edges;

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

    BlockBuilder blockBuilder;
    // Start is called before the first frame update
    void Start()
    {
        blockBuilder = new BlockBuilder(100);

        StartCoroutine(OnTimer());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator OnTimer()
    {
        foreach (var elems in blockBuilder.Build())
        {
            foreach(var elem in elems)
            {
                var pos = new Vector3Int(elem.x, elem.y, 0);
                var color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));

                SetTileColor(pos, color);
            }
        }

        yield return new WaitForSeconds(1);

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
        public BlockBuilder(int size)
        {
            centers = new HashSet<(int x, int y)>();
            edges = new HashSet<(int x, int y)>();

            this.size = size;
        }

        public IEnumerable<(int x, int y)[]> Build()
        {
            if(isStart)
            {
                var startIndex = (0, 0);
                edges.Add(startIndex);

                yield return edges.ToArray();

                isStart = false;
            }

            var newEdges = edges.SelectMany(x => Hexagon.GetNeighbors(x))
                .Where(n => !centers.Contains(n) && !edges.Contains(n))
                .Where(n => (int)Math.Abs(n.x) < size && (int)Math.Abs(n.y) < size)
                .ToHashSet();

            centers.UnionWith(newEdges);
            edges = newEdges;

            yield return newEdges.ToArray();
        }
    }
}

class Hexagon
{

    const int def = 1; //ODD_Q -1, EVEN_Q 1

    static (int q, int r)[] directs = new (int q, int r)[]
    {
        (1, 0),
        (1, -1),
        (0, -1),
        (-1, 0),
        (-1, 1),
        (0, 1)
    };

    public static IEnumerable<(int x, int y)> GetNeighbors((int x, int y) pos)
    {
        var axial = ToAxial(pos);

        return directs.Select(d => ToOffset((axial.q + d.q, axial.r + d.r)));


        //return  new (int x, int y)[]
        //{
        //        (axial.q+1, axial.r),
        //        (axial.q+1, axial.r-1),
        //        (axial.q, axial.r-1),
        //        (axial.q-1, axial.r),
        //        (axial.q-1, axial.r+1),
        //        (axial.q, axial.r+1),
        //}.Select(x => ToOffset(x));
    }

    public static int GetDirectIndex((int x, int y) target, (int x, int y) origin)
    {
        var axialTarget = ToAxial(target);
        var axialOrigin = ToAxial(origin);

        return Array.IndexOf(directs, (axialTarget.q - axialOrigin.q, axialTarget.r - axialOrigin.r));
    }

    internal static  (int x, int y) ToOffset((int q, int r) axial)
    {
        int col = axial.q;
        int row = axial.r + (int)((axial.q + def * (axial.q & 1)) / 2);

        return (row * -1, col);
    }

    internal static (int q, int r) ToAxial((int row, int col) offset)
    {
        offset = (offset.row * -1, offset.col);

        int q = offset.col;
        int r = offset.row - (int)((offset.col + def * (offset.col & 1)) / 2);

        return (q, r);
    }
}
