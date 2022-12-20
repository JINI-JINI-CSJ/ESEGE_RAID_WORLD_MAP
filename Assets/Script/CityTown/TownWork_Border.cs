using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TownWork_Border : Base_TownWorkGenerator
{
    public  SJ_Tile2DStampBrush     fill_base;

    //public  int     layer = 2;
    public  int     tile_int_val;
    public  int     empty_enter_size = 4;   //4방위 입구 , 제거할 셀

    public  bool    enter_DIR_N = false;
    public  bool    enter_DIR_S = true;
    public  bool    enter_DIR_E = true;
    public  bool    enter_DIR_W = true;

    override public  void    Work( Base_CityTownGenerator ctg , Tilemap tilemaps = null , Dictionary<string , object> dic = null )
    {
        Work_BorderLine(ctg);
    }

    public  void    Work_BorderLine(Base_CityTownGenerator ctg)
    {
        // 바닥
        if( fill_base != null )
        {
            fill_base.fill_AreaRandom_x = ctg.size;
            fill_base.fill_AreaRandom_y = ctg.size;
            fill_base.tilemap_fill_AreaRandom = ctg.tilemaps_layer[0];
            fill_base.Work_Fill_AreaRandom( ctg.GetTilemap_Ground() );
        }

        // 외곽 둘러싸기
        ctg.Line( 0 , 0 , ctg.size , 0 , tile_int_val , ctg.GetTilemap_Collider() );
        ctg.Line( 0 , 0 , 0 , ctg.size , tile_int_val , ctg.GetTilemap_Collider() );
        ctg.Line( 0 , ctg.size , ctg.size , ctg.size , tile_int_val , ctg.GetTilemap_Collider() );
        ctg.Line( ctg.size , 0 , ctg.size , ctg.size , tile_int_val , ctg.GetTilemap_Collider() );

        int cp = ctg.size / 2;
        int s_t = cp - empty_enter_size;
        int e_t = cp + empty_enter_size;

        // 입구 뚫기
        if( enter_DIR_S ) ctg.Line( s_t         , 0         , e_t       , 0         , -1 , ctg.GetTilemap_Collider() );
        if( enter_DIR_N ) ctg.Line( s_t         , ctg.size  , e_t       , ctg.size  , -1 , ctg.GetTilemap_Collider() );   
        if( enter_DIR_W ) ctg.Line( 0           , s_t       , 0         , e_t       , -1 , ctg.GetTilemap_Collider() );
        if( enter_DIR_E ) ctg.Line( ctg.size    , s_t       , ctg.size  , e_t       , -1 , ctg.GetTilemap_Collider() );
    }
}
