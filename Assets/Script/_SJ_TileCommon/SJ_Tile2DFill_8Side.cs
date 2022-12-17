using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// 타일 외곽8방향 
public class SJ_Tile2DFill_8Side : MonoBehaviour
{
    public  TileBase    tb_Center;

    public  bool        Lock_Import = true;

    [System.Serializable]
    // 외곽 연결 그래픽
    public class _SIDE_TILE
    {
        public  byte    top_left;
        public  byte    top_right;
        public  byte    bottom_left;
        public  byte    bottom_right;
        public  TileBase    tb;

        public  int     Hash()
        {
            return top_left*1000 + top_right*100 + bottom_left*10 + bottom_right;
        }
    }
    public  List<_SIDE_TILE>    lt_SIDE_TILE;
    public  Dictionary<int , _SIDE_TILE>    dic_SIDE_TILE = new Dictionary<int, _SIDE_TILE>();


    public  Tilemap             tilemap_test;
    public  List<Vector2Int>    lt_test;


    public  void    Dic_List()
    {
        dic_SIDE_TILE.Clear();
        foreach( _SIDE_TILE s in lt_SIDE_TILE )
        {
            dic_SIDE_TILE[ s.Hash() ] = s;
        }
    }

    public  bool    Check_CenterTile( Tilemap tilemap , int x , int y )
    {
        TileBase tb_1 = tilemap.GetTile( new Vector3Int( x,y ) );
        if( tb_1 == tb_Center ) return true;
        return false;
    }

    public  _SIDE_TILE  Find_SideTile( Tilemap tilemap , int x , int y )
    {
        if( lt_SIDE_TILE.Count > 0 && dic_SIDE_TILE.Count < 1 )
        {
            Dic_List();
        }

        _SIDE_TILE f = new _SIDE_TILE();
        f.top_left = f.top_right = f.bottom_left = f.bottom_right = 1;

        if( Check_CenterTile( tilemap , x - 1 , y + 1 ) ) // 좌상
        {
            f.top_left = 0;
        }
        if( Check_CenterTile( tilemap , x    , y + 1 ) ) // 상
        {
            f.top_left = 0;
            f.top_right = 0;
        }
        if( Check_CenterTile( tilemap , x + 1 , y + 1 ) ) // 우상
        {
            f.top_right = 0;
        }


        if( Check_CenterTile( tilemap , x - 1 , y    ) ) // 좌
        {
            f.top_left = 0;
            f.bottom_left = 0;
        }
        if( Check_CenterTile( tilemap , x + 1 , y    ) ) // 우
        {
            f.top_right = 0;
            f.bottom_right = 0;
        }

        if( Check_CenterTile( tilemap , x - 1 , y - 1 ) ) // 좌하
        {
            f.bottom_left = 0;
        }
        if( Check_CenterTile( tilemap , x    , y - 1 ) ) // 하
        {
            f.bottom_right = 0;
            f.bottom_left = 0;
        }
        if( Check_CenterTile( tilemap , x + 1 , y - 1 ) ) // 우하
        {
            f.bottom_right = 0;
        }

        _SIDE_TILE find = null;

        dic_SIDE_TILE.TryGetValue( f.Hash() , out find );

        return find;
    }

    // 처음 위치 찍고 
    // 8 방향 체크
    // 외곽타일이 한칸 주위에 센터타일 있으면 그 방향으로 센터타일 찍기

    public  List<Vector2Int>    Fill( Tilemap tilemap , Vector2Int vi )
    {
        List<Vector2Int>    lt_fill = new List<Vector2Int>();
        tilemap.SetTile( new Vector3Int( vi.x , vi.y ) , tb_Center );
        lt_fill.Add( vi );
        Link_FillCenterTile_2( tilemap , vi , lt_fill );
        Check_Side( tilemap , vi , lt_fill );

        return lt_fill;
    }



    // 센터 중심으로 2칸거리에 있는 센터타일과 연결
    public  void    Link_FillCenterTile_2( Tilemap tilemap , Vector2Int vi , List<Vector2Int>    lt_fill )
    {

        for( int y = -2 ; y <= 2 ; y++ )
        {
            for( int x = -2 ; x <=2 ; x++ )
            {
                // 2칸 외곽만 , x y 중 거리가 2 이상인게 한개 이상
                if( Mathf.Abs( x ) < 2 && Mathf.Abs( y ) < 2 )
                {
                    continue;
                }
                Vector3Int ch_vi = new Vector3Int( vi.x + x , vi.y + y , 0 );
                TileBase tb_2 = tilemap.GetTile( ch_vi );
                if( tb_2 == tb_Center )
                {
                    List<Vector2Int> lt_cell = SJ_TileCommon.LineCell( 
                        new Vector2Int( vi.x , vi.y ) , new Vector2Int( ch_vi.x , ch_vi.y ) , false );

                    foreach( Vector2Int s in lt_cell )
                    {
                        // lt_cell -> 어차피 무조건 1개만 온다
                        tilemap.SetTile( new Vector3Int( s.x ,s.y ) , tb_Center );
                        lt_fill.Add( s );
                        Check_Side(tilemap , s , lt_fill);
                    }
                }
            }
        }
    }



    public  void    Check_Side( Tilemap tilemap , Vector2Int vi , List<Vector2Int>    lt_fill )
    {
        for( int y = -1 ; y <= 1 ; y++ )
        {
            for( int x = -1 ; x <=1 ; x++ )
            {
                Vector3Int ch_vi = new Vector3Int( vi.x + x , vi.y + y , 0 );
                TileBase tb_1 = tilemap.GetTile( ch_vi );
                if( tb_1 != tb_Center )
                {
                    _SIDE_TILE st = Find_SideTile(tilemap , ch_vi.x , ch_vi.y );
                    if( st != null )
                    {
                        tilemap.SetTile( ch_vi , st.tb ); 
                        if( lt_fill != null )
                            lt_fill.Add( new Vector2Int( ch_vi.x , ch_vi.y ) );
                    }
                }
            }
        }
    }

    public  void    Fix_EmptyCell( Tilemap tilemap , int size ,  RectInt rc = default )
    {
        // 1. 센터타일중에 사이드 타일 없는 셀은 지운다. 지울때 8방향의 사이드 타일을 지운다.
        // 2. 남은 센터타일셀을 사이드 타일 다시 체크 및 세팅한다
        int sx = 0;
        int sy = 0;
        int ex = size;
        int ey = size;

        if( rc.size.sqrMagnitude > 1 )
        {
            sx = rc.min.x;
            sy = rc.min.y;
            ex = rc.max.x;
            ey = rc.max.y;
        }

        // 지워질 예정 타일 찾기
        List<Vector2Int>    lt_del_rev = new List<Vector2Int>();
        for( int y = sy ; y <= ey ; y++ )
        {
            for( int x = sx ; x <= ex ; x++ )
            {
                if( Fix_CheckEraseCenterTile( tilemap , x,y ) )
                {
                    lt_del_rev.Add( new Vector2Int( x,y ) );
                }
            }
        }

        // 찾은 지워질 센터타일들 지운다. (사이드도 같이 지운다.)
        foreach( Vector2Int s in lt_del_rev )
        {
            Fix_EraseCenterTile( tilemap , s.x , s.y );
        }

        // 최종 사이드 타일 보정한다.
        for( int y = sy ; y <= ey ; y++ )
        {
            for( int x = sx ; x <= ex ; x++ )
            {
                Check_Side( tilemap , new Vector2Int(x,y) , null );
            }
        }
    }

    // 사이드 타일 없는거 체크
    public  bool    Fix_CheckEraseCenterTile( Tilemap tilemap , int x , int y )
    {
        TileBase tb = tilemap.GetTile( new Vector3Int( x,y ) );
        if( tb != tb_Center ) return false;
        // 주위 8방향에 빈칸이 있는지..
        // 빈칸 있으면 지워질 예정 타일이다.
        for( int sy = -1 ; sy <= 1 ; sy++ )
        {
            for( int sx = -1 ; sx <= 1 ; sx++ )
            {
                TileBase tb_s = tilemap.GetTile( new Vector3Int( sx+x,sy+y ) );
                if( tb_s == null )
                {
                    return true;
                }
            }
        }
        return false;
    }

    public  void    Fix_EraseCenterTile( Tilemap tilemap , int x , int y )
    {
        // 본인 삭제 및 사이드 타일 삭제
        tilemap.SetTile( new Vector3Int( x,y ) , null );
        for( int sy = -1 ; sy <= 1 ; sy++ )
        {
            for( int sx = -1 ; sx <= 1 ; sx++ )
            {
                TileBase tb_s = tilemap.GetTile( new Vector3Int( sx+x , sy+y ) );
                if( tb_s != tb_Center )
                {
                    tilemap.SetTile( new Vector3Int( sx+x , sy+y ) , null );
                }
            }
        }
    }

    

    public  void    Test()
    {
        Dic_List();
        foreach( Vector2Int s in lt_test )
        {
            Fill( tilemap_test , s );
        }
    }

    // 자동 임포트
    //
    // 
    //  0         0
    //    0  0  0
    //    0  0  0
    //    0  0  0
    //  0         0
    // 위 배치를 기본으로 임포트
    // 작은 조각 그래픽외곽 연결점은 안쪽으로
    // 센터타일은 미리 세팅해야 한다.
    // 우상단 (0,0 의 양수 좌표계) 에  있어야 한다.

    public  void    Import_Auto()
    {
        if( Lock_Import )
        {
            Debug.Log("!!! 임포트 잠금 상태!!!!");
            return;
        }

        if( tilemap_test == null )
        {
            Debug.Log("!!! 임포트할 타일맵 없다!");
            return;
        }

        lt_SIDE_TILE.Clear();
        int cx = -1;
        int cy = -1;
        for( int y = 0 ; y < 100 ; y++ )
        {
            for( int x = 0 ; x < 100 ; x++ )
            {
                TileBase tb = tilemap_test.GetTile(new Vector3Int( x,y ));
                if( tb_Center==tb )
                {
                    cx = x;
                    cy = y;
                    break;
                }
            }
        }

        if( cx == -1 )
        {
            Debug.Log( "!!! 센터타일 못찾음!!!!" );
            return;
        }

        add_import_sideTile( cx - 2 , cy + 2 , 0,0,0,1 ); // 최 좌상
        add_import_sideTile( cx + 2 , cy + 2 , 0,0,1,0 ); // 최 우상
        add_import_sideTile( cx - 2 , cy - 2 , 0,1,0,0 ); // 최 좌하
        add_import_sideTile( cx + 2 , cy - 2 , 1,0,0,0 ); // 최 우하

        add_import_sideTile( cx - 1 , cy + 1 , 1,1,1,0 ); // 좌상
        add_import_sideTile( cx + 1 , cy + 1 , 1,1,0,1 ); // 우상
        add_import_sideTile( cx - 1 , cy - 1 , 1,0,1,1 ); // 좌하
        add_import_sideTile( cx + 1 , cy - 1 , 0,1,1,1 ); // 우하

        add_import_sideTile( cx     , cy + 1 , 1,1,0,0 ); // 상
        add_import_sideTile( cx     , cy - 1 , 0,0,1,1 ); // 하
        add_import_sideTile( cx - 1 , cy     , 1,0,1,0 ); // 좌
        add_import_sideTile( cx + 1 , cy     , 0,1,0,1 ); // 우
    }

    void    add_import_sideTile( int x , int y , byte tl , byte tr , byte bl , byte br )
    {
        TileBase tb = tilemap_test.GetTile(new Vector3Int( x,y ));

        if( tb == null )
        {
            Debug.Log( " !!! 임포트 사이드 타일 없다!!! " );
            return;
        }

        _SIDE_TILE s = new _SIDE_TILE();
        s.tb = tb;
        s.top_left = tl;
        s.top_right = tr;
        s.bottom_left = bl;
        s.bottom_right = br;
        lt_SIDE_TILE.Add( s );
    }

}
