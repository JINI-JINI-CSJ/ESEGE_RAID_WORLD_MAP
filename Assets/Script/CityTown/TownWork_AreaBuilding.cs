using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TownWork_AreaBuilding : Base_TownWorkGenerator
{
    public  int     size;

    [Header("에리어뷰 프리펩")]
    public  GameObject  prefab_AreaView;
    [Header("에리어뷰 상위객체")]
    public  GameObject      view_AreaContentData;   

    // 구역 컨텐츠
    [System.Serializable]
    public  class  _AREA_CONTENT
    {
        public  string  name; 
        public  int     ID;
        public  int     num_fix;        // 배치 갯수 고정
        public  int     TryWork = 100;  // 시도 횟수      
        public  int     size_min;
        public  int     size_max;
        public  bool    use_FixPos;     // 고정위치 사용
        public  Vector2Int vFixPos_Min; // 고정위치
        public  Vector2Int vFixPos_Max;
        public  int     exception_range; //중앙으로부터 제외 , 슬럼가 등등

        public  Color   view_Color;


        public  List<SJ_Tile2DStampBrush>   lt_building_fix;    //고정으로 찍는 객체
        public  List<SJ_Tile2DStampBrush>   lt_building_all;
    }
    public List<_AREA_CONTENT>  lt_AREA_CONTENT;

    // 
    [System.Serializable]
    public  class  _AREA_CONTENT_DATA
    {
        public  int         ID;
        public  Vector2Int  pos;
        public  int         Radius;
    }
    public  List<_AREA_CONTENT_DATA>    lt_AREA_CONTENT_DATA;

    public  int      build_obj_inSide;

    [System.Serializable]
    public  class _BUILD_OBJ_DATA
    {
        public  int         ID;
        public  int         ID_game;
        public  Vector2Int  pos;
    }
    public  List<_BUILD_OBJ_DATA>       lt_BUILD_OBJ_DATA;

    [Header("건물 객체 타일맵")]
    public  GameObject                  go_Event;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    override public  void    Work( Base_CityTownGenerator ctg , Tilemap tilemaps = null , Dictionary<string , object> dic = null )
    {

    }



    public  void    Work_AREA_CONTENT( Base_CityTownGenerator ctg )
    {
        SJ_TileCommon.DeleteChildAll(go_Event);
        lt_BUILD_OBJ_DATA.Clear();
        SJ_TileCommon.DeleteChildAll( view_AreaContentData );
        lt_AREA_CONTENT_DATA.Clear();
        foreach( _AREA_CONTENT s in lt_AREA_CONTENT )
        {
            Prc_AreaContent(ctg , s);
        }
    }

    // 구역 배치
    public  void    Prc_AreaContent( Base_CityTownGenerator ctg , _AREA_CONTENT ac )
    {
        int put_c = 0;
        for( int i = 0 ; i < ac.TryWork ;i++ )
        {
            if( Prc_AreaContent_UNIT( ctg , ac ) )
            {
                put_c++;
            }
            if( ac.num_fix > 0 && put_c >= ac.num_fix  )
            {
                break;
            }
        }
    }

    public  bool    Prc_AreaContent_UNIT( Base_CityTownGenerator ctg , _AREA_CONTENT ac )
    {
        Vector2Int  vPos = new Vector2Int();

        if( ac.exception_range > 0 )
        {
            Vector2 v = new Vector2( 0 , UnityEngine.Random.Range(ac.exception_range , size - ac.exception_range ) );
            v = Quaternion.Euler(0,0,UnityEngine.Random.Range( 0 , 360 )) * v;
            vPos.x = (int)v.x;
            vPos.y = (int)v.y;
        }else{
            if( ac.use_FixPos )
            {
                vPos.x = UnityEngine.Random.Range( ac.vFixPos_Min.x , ac.vFixPos_Max.x );
                vPos.y = UnityEngine.Random.Range( ac.vFixPos_Min.y , ac.vFixPos_Max.y );
            }else{
                vPos.x = UnityEngine.Random.Range( 0 , size );
                vPos.y = UnityEngine.Random.Range( 0 , size );
            }
        }

        int size_succ = 0;
        bool succ = true;

        for( int size_cur = ac.size_max ; size_cur > ac.size_min ; size_cur-- )
        {
            succ = true;
            size_succ = size_cur;
            // 다른 지역과 겹치지 않게
            foreach( _AREA_CONTENT_DATA s in lt_AREA_CONTENT_DATA )
            {
                float len = Vector2Int.Distance(s.pos,vPos);
                int len_t = s.Radius + size_cur;
                if( len_t < len )
                {
                    size_succ = size_cur;
                }else{
                    succ = false;
                }
            }
            if( succ ) break;
        }

        if( succ == false )
        {
            return false;
        }

        _AREA_CONTENT_DATA inst = new _AREA_CONTENT_DATA();   
        inst.pos = vPos;     
        inst.Radius = size_succ;
        inst.ID = ac.ID;
        lt_AREA_CONTENT_DATA.Add(inst);
        
        GameObject inst_view = GameObject.Instantiate( prefab_AreaView );
        SpriteRenderer sr = inst_view.GetComponent<SpriteRenderer>();
        sr.color = ac.view_Color;
        inst_view.transform.localPosition = new Vector3( vPos.x , vPos.y , 0 );
        // 유니티 서클 기본 반지름이 0.5
        // 2배를 해준다.
        inst_view.transform.localScale = new Vector3( size_succ * 2 , size_succ * 2 , 1 );
        inst_view.transform.parent = view_AreaContentData.transform;
        inst_view.SetActive(true);

        Work_Build_Obj( ctg , ac , inst );

        return true;
    }

    // 구역별로 건물 배치
    // 실제 타일로 찍어보면서 없는 곳에만 배치한다
    public  void    Work_Build_Obj( Base_CityTownGenerator ctg , _AREA_CONTENT ac , _AREA_CONTENT_DATA acd )
    {
        List<SJ_Tile2DStampBrush>   lt_fix = new List<SJ_Tile2DStampBrush>( ac.lt_building_fix );

        RectInt rc = new RectInt( build_obj_inSide , build_obj_inSide , size - build_obj_inSide*2 , size - build_obj_inSide*2 );
        for( int i = 0 ; i < ac.TryWork ; i++ )
        {
            Vector2 vPos = new Vector2Int( 0 , UnityEngine.Random.Range(0,acd.Radius) );
            vPos = Quaternion.Euler( 0, 0 , UnityEngine.Random.Range(0,360) ) * vPos;
            Vector2Int vi = new Vector2Int( (int)vPos.x + acd.pos.x , (int)vPos.y+ acd.pos.y );

            // 고정 위치는 일단 한개만 찍기
            if( ac.use_FixPos )
            {
                vi.x = UnityEngine.Random.Range( ac.vFixPos_Min.x , ac.vFixPos_Max.x );
                vi.y = UnityEngine.Random.Range( ac.vFixPos_Min.y , ac.vFixPos_Max.y );
            }

            SJ_Tile2DStampBrush stampBrush = null;

            if( lt_fix.Count > 0 )
            {
                int idx = UnityEngine.Random.Range(0 , lt_fix.Count );
                stampBrush = lt_fix[idx];       
                lt_fix.RemoveAt(idx);
            }else{
                int idx = UnityEngine.Random.Range(0 , ac.lt_building_all.Count );
                stampBrush = ac.lt_building_all[idx];                
            }



            if( stampBrush.Check_InRect( rc , vi.x , vi.y ) )
            {
                if( stampBrush.Fill( ctg.tilemaps_layer , vi.x , vi.y ,true , go_Event ) )
                {
                    _BUILD_OBJ_DATA bod = new _BUILD_OBJ_DATA();
                    bod.pos = vi;
                    bod.ID = ac.ID;
                    lt_BUILD_OBJ_DATA.Add(bod);
                }
            }

            if( ac.use_FixPos )
            {
                break;
            }
        }
    }

}
