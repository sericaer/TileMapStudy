using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RectHeightMap : MonoBehaviour
{
    public Tilemap tilemap;
    public Sprite sprite;

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

        var dictHeight = new Dictionary<(int x, int y), float>();

        int startIndex = -100;
        int count = 100;

        while (dictHeight.Count < count)
        {
            var index = (Random.Range(startIndex, startIndex + count), Random.Range(startIndex, startIndex + count));
            if (dictHeight.ContainsKey(index))
            {
                continue;
            }

            dictHeight.Add(index, Random.Range(0f, 1f));
        }

        var rslt = MakeHeightMap(dictHeight, startIndex, count);

        foreach (var elem in rslt)
        {
            var pos = new Vector3Int(elem.Key.x, elem.Key.y);
            SetTileColor(pos, new Color(elem.Value, elem.Value, elem.Value));
        }
    }

    public static Dictionary<(int x, int y), float> MakeHeightMap(Dictionary<(int x, int y), float> cores, int startIndex, int count)
    {

        var positions = Enumerable.Range(startIndex, count)
            .SelectMany(x => Enumerable.Range(startIndex, count).Select(y => (x, y)))
            .OrderBy(_ => System.Guid.NewGuid());

        var rslt = cores.ToDictionary(x => x.Key, y => y.Value);

        foreach (var pos in positions.Where(x=>!cores.ContainsKey(x)))
        {
            var nearbys = GetNearby(pos, rslt);
            rslt.Add(pos, CalcHeight(pos, nearbys));
        }

        foreach (var pos in positions)
        {
            var nearbys = GetNearby(pos, rslt);
            rslt[pos] = CalcHeight(pos, nearbys);
        }

        var max = rslt.Values.Max();

        return rslt;
    }

    private static float CalcHeight((int x, int y) pos, Dictionary<(int x, int y), float> nearbys)
    {
        var dictPow = nearbys.ToDictionary(k => k.Key, v => (float)(System.Math.Pow(pos.x - v.Key.x, 2) + System.Math.Pow(pos.y - v.Key.y, 2)));

        float rslt = 0;
        foreach (var elem in nearbys)
        {
            rslt += Mathf.Lerp(0f,  elem.Value, dictPow[elem.Key] / dictPow.Values.Sum());
        }
        return rslt;
    }

    //float InterpolationCalculation1(float num)
    //{
    //    return Random.Range(0.8f, 1f) * num;
    //}

    private static Dictionary<(int i, int j), float> GetNearby((int i, int j) pos, Dictionary<(int x, int y), float> map)
    {
        var rlst = new Dictionary<(int i, int j), float>();

        int dist = 1;
        while (rlst.Count< 4)
        {
            var nearbys = MapMath.GetRings(pos, dist);
            foreach(var near in nearbys)
            {
                if(map.ContainsKey(near))
                {
                    rlst.Add(near, map[near]);
                }
            }
            dist++;
        }

        return rlst;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetTileColor(Vector3Int pos, Color color)
    {
        tilemap.SetTile(pos, tile);
        tilemap.SetTileFlags(pos, TileFlags.None);
        tilemap.SetColor(pos, color);
    }
}