using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DynamicRectMap : MonoBehaviour
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

            var newEdges = edges.SelectMany(x => MapMath.GetNeighbours(x))
                .Where(n => !centers.Contains(n) && !edges.Contains(n))
                .Where(n => (int)Math.Abs(n.x) < size && (int)Math.Abs(n.y) < size)
                .ToHashSet();

            centers.UnionWith(newEdges);
            edges = newEdges;

            yield return newEdges.ToArray();
        }
    }
}
