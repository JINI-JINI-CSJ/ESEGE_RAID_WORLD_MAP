using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//
// 타일맵 레이어 
// 0 : 기본 바닥
// 1 : 컨턴츠 ( 돌판,길,집 , 등등 겹치지 말아야 하는것들)
// 2 : 케릭터 충돌 ( 여기에만 콜리더 붙이자. )
// 3 : 케릭터 가려지는 레이어
// 

[System.Serializable]
public  class _SIZE_PER_NUM
{
    public  int     num_fix;
    public  int     num_per_size;
    public  int     num_per_size_min;   // 사이즈당 갯수
    public  int     num_per_size_max;   
    public  int     Random( int size )
    {
        if( num_fix > 0 ) return num_fix;

        int n1 = size / num_per_size;
        int n_min = n1 * num_per_size_min;
        int n_max = n1 * num_per_size_max;

        return UnityEngine.Random.Range( n_min , n_max + 1 );
    }
}


public class Base_CityTownGenerator : SJ_Tile2DBuffer
{
    public  List<Tilemap>           tilemaps_layer;

    [System.Serializable]
    public  class  _WORK_GROUP
    {
        public  string      DESC;
        public  List<Base_TownWorkGenerator>    lt_work;
        public  bool        work_select;
    }
    public  List<_WORK_GROUP>   lt_WORK_GROUP;

    public  void    Work( bool select_work )
    {
        foreach( _WORK_GROUP s in lt_WORK_GROUP ) 
        {
            bool b = true;
            if( select_work && s.work_select == false  )
            {
                b = false;
            }
            if( b )
            {
                foreach( Base_TownWorkGenerator w in s.lt_work )
                {
                    w.Work(this);
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public  Tilemap     GetTilemap_Ground()  {return tilemaps_layer[0];}
    public  Tilemap     GetTilemap_Field()   {return tilemaps_layer[1];}
    public  Tilemap     GetTilemap_Collider(){return tilemaps_layer[2];}
    public  Tilemap     GetTilemap_OverView(){return tilemaps_layer[3];}

    // public  void    Work_BorderLine()
    // {
    //     if( fill_base != null )
    //     {
    //         fill_base.fill_AreaRandom_x = size;
    //         fill_base.fill_AreaRandom_y = size;
    //         fill_base.tilemap_fill_AreaRandom = tilemaps_layer[0];
    //         fill_base.Work_Fill_AreaRandom();
    //     }

    //     Line( 0 , 0 , size , 0 , _border.tile_int_val , tilemaps_layer[2] );
    //     Line( 0 , 0 , 0 , size , _border.tile_int_val , tilemaps_layer[2] );
    //     Line( 0 , size , size , size , _border.tile_int_val , tilemaps_layer[2] );
    //     Line( size , 0 , size , size , _border.tile_int_val , tilemaps_layer[2] );

    //     int cp = size / 2;
    //     int s_t = cp - _border.empty_enter_size;
    //     int e_t = cp + _border.empty_enter_size;

    //     Line( s_t , 0 , e_t , 0 , -1 , tilemaps_layer[2] );
    //     Line( 0 , 0 , s_t , e_t , -1 , tilemaps_layer[2] );
    //     Line( s_t , size , e_t , size , -1 , tilemaps_layer[2] );
    //     Line( size , s_t , size , e_t , -1 , tilemaps_layer[2] );
    // }

    // public  void    Work_LineContent()
    // {
    //     foreach( _LINE_CONTENT s in lt_LINE_CONTENT )
    //     {
    //         Prc_LINE_CONTENT( s );
    //     }
    // }

    // public  void    Prc_LINE_CONTENT( _LINE_CONTENT lc )
    // {
    //     if( lc.base_LineLoad != null )
    //     {
    //         lc.base_LineLoad.Work( this );
    //         return;
    //     }

    //     // 총 객체 
    //     // 사이즈 대비 길 수
    //     int num_line = lc.num_fix;

    //     if( num_line == 0 )
    //     {
    //         int num_per_size_c = size / lc.num_per_size;
    //         int num_per_size_min = num_per_size_c * lc.num_per_size_min;
    //         int num_per_size_max = num_per_size_c * lc.num_per_size_max;
    //         num_line = UnityEngine.Random.Range( num_per_size_min , num_per_size_max );
    //     }

    //     for( int i = 0 ; i < num_line ; i++ )
    //     {
    //         Vector2Int vStart = new Vector2Int();
    //         Vector2Int vDirEnd = new Vector2Int();
            
    //         if( lc.use_FixHori_Vert == 0 )
    //         {
    //             if( UnityEngine.Random.Range(0,1) == 0 )
    //             {
    //                 vStart.x = UnityEngine.Random.Range(0,size);
    //                 vStart.y = 0;                
    //                 vDirEnd.x = UnityEngine.Random.Range(0,size);
    //                 vDirEnd.y = size;
    //             }else{
    //                 vStart.x = 0;
    //                 vStart.y = UnityEngine.Random.Range(0,size);                
    //                 vDirEnd.x = size;
    //                 vDirEnd.y = UnityEngine.Random.Range(0,size);
    //             }
    //             Prc_LINE_CONTENT_UNIT( lc ,vStart , vDirEnd );                
    //         }else{
    //             int step = 0;

    //             if( lc.num_fix > 0 )
    //             {
    //                 step = size / (lc.num_fix + 1);
    //                 step = step * (i+1);
    //             }else{
    //                 step = UnityEngine.Random.Range(0,size);
    //             }

    //             if( lc.use_FixHori_Vert == 1 )
    //             {
    //                 vStart.x = 0;
    //                 vStart.y = step;

    //                 vDirEnd.x = size;
    //                 vDirEnd.y = step;
    //             }else{
    //                 vStart.x = step;
    //                 vStart.y = 0;

    //                 vDirEnd.x = step;
    //                 vDirEnd.y = size;
    //             }
    //             Line( vStart.x , vStart.y , vDirEnd.x , vDirEnd.y , lc.tile_int_val );
    //         }
    //     }
    // }

    // // 랜덤 길 생성
    // // 최초 시작점은 0,0 기준으로 랜덤 생성
    // // 목표 방향은 반대편 대칭점
    // // 한 라인씩 늘려가며 생성
    // // 다음번 라인 생성할때 연결점이 바깥쪽이면 종료
    // public  void    Prc_LINE_CONTENT_UNIT( _LINE_CONTENT lc , Vector2Int vStart , Vector2Int vDirEnd )
    // {
    //     Vector2Int  vIntDir = vDirEnd - vStart;
    //     Vector2     vDir = new Vector2( vIntDir.x , vIntDir.y );
    //     vDir.Normalize();

    //     while( true )
    //     {
    //         if( vStart.x < 0 || vStart.y < 0 || vStart.x >= size || vStart.y >= size )
    //             break;

    //         int length = UnityEngine.Random.Range( lc.curve_length_min , lc.curve_length_max );
    //         int angle = UnityEngine.Random.Range( -lc.angle_range , lc.angle_range );

    //         vDir = Quaternion.Euler( 0,0,angle ) * vDir;

    //         Vector2Int vTarget = new Vector2Int( (int)(vDir.x * length) , (int)(vDir.y * length) );
    //         vTarget += vStart;

    //         Line( vStart.x , vStart.y , vTarget.x , vTarget.y , lc.tile_int_val );
    //         vStart = vTarget;
    //     }
    // }


    // public  void    Work_AREA_CONTENT()
    // {
    //     SJ_TileCommon.DeleteChildAll(go_Event);
    //     lt_BUILD_OBJ_DATA.Clear();
    //     SJ_TileCommon.DeleteChildAll( view_AreaContentData );
    //     lt_AREA_CONTENT_DATA.Clear();
    //     foreach( _AREA_CONTENT s in lt_AREA_CONTENT )
    //     {
    //         Prc_AreaContent(s);
    //     }
    // }

    // // 구역 배치
    // public  void    Prc_AreaContent( _AREA_CONTENT ac )
    // {
    //     int put_c = 0;
    //     for( int i = 0 ; i < ac.TryWork ;i++ )
    //     {
    //         if( Prc_AreaContent_UNIT( ac ) )
    //         {
    //             put_c++;
    //         }
    //         if( ac.num_fix > 0 && put_c >= ac.num_fix  )
    //         {
    //             break;
    //         }
    //     }
    // }

    // public  bool    Prc_AreaContent_UNIT( _AREA_CONTENT ac )
    // {
    //     Vector2Int  vPos = new Vector2Int();

    //     if( ac.exception_range > 0 )
    //     {
    //         Vector2 v = new Vector2( 0 , UnityEngine.Random.Range(ac.exception_range , size - ac.exception_range ) );
    //         v = Quaternion.Euler(0,0,UnityEngine.Random.Range( 0 , 360 )) * v;
    //         vPos.x = (int)v.x;
    //         vPos.y = (int)v.y;
    //     }else{
    //         if( ac.use_FixPos )
    //         {
    //             vPos.x = UnityEngine.Random.Range( ac.vFixPos_Min.x , ac.vFixPos_Max.x );
    //             vPos.y = UnityEngine.Random.Range( ac.vFixPos_Min.y , ac.vFixPos_Max.y );
    //         }else{
    //             vPos.x = UnityEngine.Random.Range( 0 , size );
    //             vPos.y = UnityEngine.Random.Range( 0 , size );
    //         }
    //     }

    //     int size_succ = 0;
    //     bool succ = true;

    //     for( int size_cur = ac.size_max ; size_cur > ac.size_min ; size_cur-- )
    //     {
    //         succ = true;
    //         size_succ = size_cur;
    //         // 다른 지역과 겹치지 않게
    //         foreach( _AREA_CONTENT_DATA s in lt_AREA_CONTENT_DATA )
    //         {
    //             float len = Vector2Int.Distance(s.pos,vPos);
    //             int len_t = s.Radius + size_cur;
    //             if( len_t < len )
    //             {
    //                 size_succ = size_cur;
    //             }else{
    //                 succ = false;
    //             }
    //         }
    //         if( succ ) break;
    //     }

    //     if( succ == false )
    //     {
    //         return false;
    //     }

    //     _AREA_CONTENT_DATA inst = new _AREA_CONTENT_DATA();   
    //     inst.pos = vPos;     
    //     inst.Radius = size_succ;
    //     inst.ID = ac.ID;
    //     lt_AREA_CONTENT_DATA.Add(inst);
        
    //     GameObject inst_view = GameObject.Instantiate( prefab_AreaView );
    //     SpriteRenderer sr = inst_view.GetComponent<SpriteRenderer>();
    //     sr.color = ac.view_Color;
    //     inst_view.transform.localPosition = new Vector3( vPos.x , vPos.y , 0 );
    //     // 유니티 서클 기본 반지름이 0.5
    //     // 2배를 해준다.
    //     inst_view.transform.localScale = new Vector3( size_succ * 2 , size_succ * 2 , 1 );
    //     inst_view.transform.parent = view_AreaContentData.transform;
    //     inst_view.SetActive(true);

    //     Work_Build_Obj( ac , inst );

    //     return true;
    // }

    // // 구역별로 건물 배치
    // // 실제 타일로 찍어보면서 없는 곳에만 배치한다
    // public  void    Work_Build_Obj( _AREA_CONTENT ac , _AREA_CONTENT_DATA acd )
    // {
    //     RectInt rc = new RectInt( build_obj_inSide , build_obj_inSide , size - build_obj_inSide*2 , size - build_obj_inSide*2 );
    //     for( int i = 0 ; i < ac.TryWork ; i++ )
    //     {
    //         Vector2 vPos = new Vector2Int( 0 , UnityEngine.Random.Range(0,acd.Radius) );
    //         vPos = Quaternion.Euler( 0, 0 , UnityEngine.Random.Range(0,360) ) * vPos;
    //         Vector2Int vi = new Vector2Int( (int)vPos.x + acd.pos.x , (int)vPos.y+ acd.pos.y );

    //         // 고정 위치는 일단 한개만 찍기
    //         if( ac.use_FixPos )
    //         {
    //             vi.x = UnityEngine.Random.Range( ac.vFixPos_Min.x , ac.vFixPos_Max.x );
    //             vi.y = UnityEngine.Random.Range( ac.vFixPos_Min.y , ac.vFixPos_Max.y );
    //         }

    //         int idx = UnityEngine.Random.Range(0 , ac.lt_stampBrush.Count );
    //         SJ_Tile2DStampBrush stampBrush = ac.lt_stampBrush[idx];

    //         if( stampBrush.Check_InRect( rc , vi.x , vi.y ) )
    //         {
    //             if( stampBrush.Fill( tilemaps_layer , vi.x , vi.y ,true , go_Event ) )
    //             {
    //                 _BUILD_OBJ_DATA bod = new _BUILD_OBJ_DATA();
    //                 bod.pos = vi;
    //                 bod.ID = ac.ID;
    //                 lt_BUILD_OBJ_DATA.Add(bod);
    //             }
    //         }

    //         if( ac.use_FixPos )
    //         {
    //             break;
    //         }
    //     }
    // }

}
