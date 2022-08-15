using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainHexMap : MonoBehaviour
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


        builderGroup = new BlockBuilderGroup(50);
        var blocks = builderGroup.Build();

        var seaBlocks = blocks.Where(x => x.edges.Any(r => r.x == -49 || r.y == 49)).ToArray();
        blocks = blocks.Except(seaBlocks).ToArray();

        var mountBlocks = blocks.Where(x => x.edges.Any(r => r.x == 49 || r.y == -49)).ToArray();
        blocks = blocks.Except(mountBlocks).ToArray();

        var hillBlock = blocks.Where(x => mountBlocks.Any(m=>x.isNeighbor(m))).ToArray();
        blocks = blocks.Except(hillBlock).ToArray();

        var plainBlock = blocks;

        foreach (var elem in seaBlocks.SelectMany(x=>x.elements))
        {
            var pos = new Vector3Int(elem.x, elem.y, 0);
            SetTileColor(pos, Color.blue);
        }

        foreach (var elem in mountBlocks.SelectMany(x => x.elements))
        {
            var pos = new Vector3Int(elem.x, elem.y, 0);
            SetTileColor(pos, new Color(128 / 255f, 0, 128 / 255f));
        }

        foreach (var elem in hillBlock.SelectMany(x => x.elements))
        {
            var pos = new Vector3Int(elem.x, elem.y, 0);
            SetTileColor(pos, Color.yellow);
        }

        foreach (var elem in plainBlock.SelectMany(x => x.elements))
        {
            var pos = new Vector3Int(elem.x, elem.y, 0);
            SetTileColor(pos, Color.green);
        }

        //colors = new HashSet<Color>();
        //while (colors.Count < rslt.Length)
        //{
        //    colors.Add(new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
        //}
        //for (int i = 0; i < blocks.Length; i++)
        //{
        //    var color = Color.green;

        //    if(blocks[i].edges.Any(r=>r.x == -49 || r.y == 49))
        //    {
        //        color = Color.blue;
        //    }
        //    if (blocks[i].edges.Any(r => r.y == -49))
        //    {
        //        color = Color.yellow;
        //    }

        //    foreach (var elem in blocks[i].elements)
        //    {
        //        var pos = new Vector3Int(elem.x, elem.y, 0);
        //        SetTileColor(pos, color);
        //    }
        //}
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator OnTimer()
    {
        yield return new WaitForSeconds(1);

        var stepResults = builderGroup.BuildInStep();
        for (int i = 0; i < stepResults.Length; i++)
        {
            foreach (var elem in stepResults[i].elements)
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
}
