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

    public Dictionary<TerrainType, Color> colors = new Dictionary<TerrainType, Color>()
    {
        { TerrainType.Plain, Color.green},
        { TerrainType.Hill, Color.yellow},
        { TerrainType.Mount, new Color(128 / 255f, 0, 128 / 255f)},
        { TerrainType.Water, Color.blue},
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

    // Start is called before the first frame update
    void Start()
    {
        builderGroup = new BlockBuilderGroup(50);
        var blocks = builderGroup.Build();

        //var seaBlocks = blocks.Where(x => x.edges.Any(r => r.x == -49 || r.y == 49)).ToArray();
        //blocks = blocks.Except(seaBlocks).ToArray();

        //var mountBlocks = blocks.Where(x => x.edges.Any(r => r.x == 49 || r.y == -49)).ToArray();
        //blocks = blocks.Except(mountBlocks).ToArray();

        //var hillBlock = blocks.Where(x => mountBlocks.Any(m => x.isNeighbor(m))).ToArray();
        //blocks = blocks.Except(hillBlock).ToArray();

        //hillBlock = hillBlock.Concat(blocks.Where(x => hillBlock.Any(m => x.isNeighbor(m)))).ToArray();
        //blocks = blocks.Except(hillBlock).ToArray();

        //var plainBlock = blocks;

        var terrainBuilder = new TerrainBuilder();
        var terrainDict = terrainBuilder.Build(blocks);

        foreach (var pair in terrainDict)
        {
            foreach (var index in pair.Value.SelectMany(x=>x.elements))
            {
                var pos = new Vector3Int(index.x, index.y, 0);
                SetTileColor(pos, colors[pair.Key]);
            }
        }



        //var seaBlocks = blocks.Where(x => x.edges.Any(r => r.x == -49 || r.y == 49)).ToArray();
        //blocks = blocks.Except(seaBlocks).ToArray();

        //var mountBlocks = blocks.Where(x => x.edges.Any(r => r.x == 49 || r.y == -49)).ToArray();
        //blocks = blocks.Except(mountBlocks).ToArray();

        //var hillBlock = blocks.Where(x => mountBlocks.Any(m=>x.isNeighbor(m))).ToArray();
        //blocks = blocks.Except(hillBlock).ToArray();

        //hillBlock = hillBlock.Concat(blocks.Where(x => hillBlock.Any(m => x.isNeighbor(m)))).ToArray();
        //blocks = blocks.Except(hillBlock).ToArray();

        //var plainBlock = blocks;

        //var count = Random.Range(10, 20);

        //var singleHills = plainBlock.SelectMany(x => x.elements).OrderBy(_ => Random.Range(0, 100)).Take(count).ToArray();
        //var round = singleHills.SelectMany(x =>
        //{
        //    var neighbours = Hexagon.GetNeighbors(x).Take(3);
        //    return neighbours.SelectMany(n => Hexagon.GetNeighbors(n)).Take(3).Concat(neighbours);
        //}).ToArray();

        //foreach (var elem in plainBlock.SelectMany(x => x.elements))
        //{
        //    var pos = new Vector3Int(elem.x, elem.y, 0);
        //    SetTileColor(pos, Color.green);
        //}

        //foreach (var elem in seaBlocks.SelectMany(x=>x.elements))
        //{
        //    var pos = new Vector3Int(elem.x, elem.y, 0);
        //    SetTileColor(pos, Color.blue);
        //}

        //foreach (var elem in mountBlocks.SelectMany(x => x.elements))
        //{
        //    var pos = new Vector3Int(elem.x, elem.y, 0);
        //    SetTileColor(pos, new Color(128 / 255f, 0, 128 / 255f));
        //}

        //foreach (var elem in hillBlock.SelectMany(x => x.elements).Concat(singleHills).Concat(round))
        //{
        //    var pos = new Vector3Int(elem.x, elem.y, 0);
        //    SetTileColor(pos, Color.yellow);
        //}


    }

    // Update is called once per frame
    void Update()
    {

    }

    //IEnumerator OnTimer()
    //{
    //    yield return new WaitForSeconds(1);

    //    var stepResults = builderGroup.BuildInStep();
    //    for (int i = 0; i < stepResults.Length; i++)
    //    {
    //        foreach (var elem in stepResults[i].elements)
    //        {
    //            var pos = new Vector3Int(elem.x, elem.y, 0);
    //            SetTileColor(pos, colors.ElementAt(i));
    //        }
    //    }

    //    StartCoroutine(OnTimer());
    //}

    private void SetTileColor(Vector3Int pos, Color color)
    {
        tilemap.SetTile(pos, tile);
        tilemap.SetTileFlags(pos, TileFlags.None);
        tilemap.SetColor(pos, color);
    }
}

//internal class TerrainBuilder
//{
//    public TerrainBuilder()
//    {
//    }

//    internal Dictionary<TerrainType, IEnumerable<(int x, int y)>> Build(BlockBuilderGroup.Block[] blocks)
//    {
//        var rslt = new Dictionary<TerrainType, IEnumerable<(int x, int y)>>();

//        var seaBlocks = blocks.Where(x => x.edges.Any(r => r.x == -49 || r.y == 49));
//        rslt.Add(TerrainType.Water, seaBlocks.SelectMany(x=>x.elements).ToArray());

//        blocks = blocks.Except(seaBlocks).ToArray();

//        var mountBlocks = blocks.Where(x => x.edges.Any(r => r.x == 49 || r.y == -49));
//        rslt.Add(TerrainType.Mount, mountBlocks.SelectMany(x => x.elements).ToArray());

//        blocks = blocks.Except(mountBlocks).ToArray();

//        var hillBlock = blocks.Where(x => mountBlocks.Any(m => x.isNeighbor(m))).ToArray();
//        blocks = blocks.Except(hillBlock).ToArray();
//        hillBlock = hillBlock.Concat(blocks.Where(x => hillBlock.Any(m => x.isNeighbor(m)))).ToArray();
//        blocks = blocks.Except(hillBlock).ToArray();

//        rslt.Add(TerrainType.Mount, hillBlock.SelectMany(x => x.elements).ToArray());

//        return rslt;
//    }
//}

internal class TerrainBuilder
{
    public TerrainBuilder()
    {
    }

    internal Dictionary<TerrainType, IEnumerable<Block>> Build(Block[] blocks)
    {
        var rslt = new Dictionary<TerrainType, IEnumerable<Block>>();

        rslt.Add(TerrainType.Water, blocks.Where(x => x.edges.Any(r => r.x == -49 || r.y == 49)));

        blocks = blocks.Except(rslt.Values.SelectMany(x=>x)).ToArray();

        rslt.Add(TerrainType.Mount, blocks.Where(x => x.edges.Any(r => r.x == 49 || r.y == -49)));

        blocks = blocks.Except(rslt.Values.SelectMany(x => x)).ToArray();

        var hills = new List<Block>();
        var plains = new List<Block>();
        foreach (var block in blocks)
        {
            if(rslt[TerrainType.Mount].Any(m => block.isNeighbor(m)))
            {
                hills.Add(block);
            }
            else if(rslt[TerrainType.Water].Any(m => block.isNeighbor(m)))
            {
                plains.Add(block);
            }
            else
            {
                var hillPercent = 50;
                var plainPercent = 50;

                if (hills.Any(m => block.isNeighbor(m)))
                {
                    hillPercent += 50;
                }
                if (hills.Any(m => block.isNeighbor(m)))
                {
                    plainPercent += 100;
                }

                var random = Random.Range(0, hillPercent + plainPercent);
                if(random < hillPercent)
                {
                    hills.Add(block);
                }
                else
                {
                    plains.Add(block);
                }
            }
        }

        rslt.Add(TerrainType.Hill, hills);
        rslt.Add(TerrainType.Plain, plains);

        return rslt;
    }
}
public enum TerrainType
{
    Plain,
    Hill,
    SmallHill,
    Mount,
    Water
}