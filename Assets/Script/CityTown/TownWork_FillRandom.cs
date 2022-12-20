using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TownWork_FillRandom : Base_TownWorkGenerator
{
    public  SJ_Tile2DFill_8Side     fill_8Side;

    public  int     range = 20;

    public  int     TryNum = 30;

    public override void Work(Base_CityTownGenerator ctg , Tilemap tilemaps = null , Dictionary<string , object> dic = null )
    {
        Vector2Int vi = SJ_TileCommon.Random_Vec2Int( ctg.size );

        List<Vector2Int>    lt_filled = new List<Vector2Int>();
        for( int i = 0 ; i < TryNum ; i++ )
        {
            Vector2Int vi_fill = SJ_TileCommon.Random_Vec2Int_Rect( vi, ctg.size , ctg.size );
            List<Vector2Int>    filled = fill_8Side.Fill( tilemaps , vi_fill );
            lt_filled.AddRange(filled);
        }

        dic["lt_filled"] = lt_filled;
    }

}
