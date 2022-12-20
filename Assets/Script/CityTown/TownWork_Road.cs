using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TownWork_Road : Base_TownWorkGenerator
{
    public  string              Desc;
    public  SJ_Tile2DFill_8Side load_stamp;
    public  Tilemap             tilemap_Fill;       // 칠하는 타일맵
    public  Tilemap             tilemap_Discord;    // 칠할때 대응해서 지우는 맵 ( 길 타일맵위에 물길 타일 입히는 경우 )
    public  TownWork_Road  road_Discord;   // 지워진 맵 마무리 하는 거

    // 수평 수직 
    [System.Serializable]
    public  class _HORI_VERT_LINE_CONTENT
    {
        public _SIZE_PER_NUM size_per_num;
        
        public  int     distance_min;
        public  int     distance_max;

        public  int     curve_num_max = 2;
    }
    public    _HORI_VERT_LINE_CONTENT   _hori_vert_line;


    public  enum DIR_HORT_VERT
    {
        None = 0 ,
        N = 1,
        S = 2,
        E = 3,
        W = 4
    }

    // Start is called before the first frame update
    void Start(){}
    // Update is called once per frame
    void Update(){}


    override public  void    Work( Base_CityTownGenerator ctg , Tilemap tilemaps = null , Dictionary<string , object> dic = null )
    {
        List<Vector2Int>    lt_filled = new List<Vector2Int>();
        int c = _hori_vert_line.size_per_num.Random(ctg.size);

        for( int i = 0 ; i < c ; i++ ) 
        {
            int cc = UnityEngine.Random.Range( 1 , _hori_vert_line.curve_num_max );
            DIR_HORT_VERT dir = RandomDir_DIR_HORT_VERT( DIR_HORT_VERT.None );
            Vector2Int vStart = Start_Pos( dir , ctg );

            for( int o = 0; o < cc ; o++ )
            {
                vStart = Road_Work( dir , vStart , ctg , lt_filled );           
                RectInt rc = new RectInt(0,0,ctg.size , ctg.size);
                dir = RandomDir_DIR_HORT_VERT( dir );
                if( rc.Contains( vStart ) == false )
                {
                    break;
                }      
            }
        }


        if( tilemap_Discord != null && road_Discord != null )
        {
            RectInt rc = new RectInt();
            foreach( Vector2Int s in lt_filled )
            {
                if( s.x < rc.xMin ) rc.xMin = s.x;
                if( s.y < rc.yMin ) rc.yMin = s.y;
                if( s.x > rc.xMax ) rc.xMax = s.x;
                if( s.y > rc.yMax ) rc.yMax = s.y;


                tilemap_Discord.SetTile( new Vector3Int(s.x , s.y) , null );
            }

            rc.xMin--;rc.yMin--;
            rc.xMax++;rc.yMax++;

            road_Discord.load_stamp.Fix_EmptyCell( tilemap_Discord , ctg.size  , rc );
        }
    }

    public  Vector2Int    Start_Pos( DIR_HORT_VERT d , Base_CityTownGenerator ctg )
    {
        int r = UnityEngine.Random.Range( 0 , ctg.size + 1 );
        Vector2Int v = new Vector2Int();
        switch( d )
        {
            case DIR_HORT_VERT.N:
                v.x = r;        v.y = 0;            // 아래시작으로 위로 진행
            break;
            case DIR_HORT_VERT.S:
                v.x = r;        v.y = ctg.size;     // 위시작으로 아래로 진행
            break;
            case DIR_HORT_VERT.E:
                v.x = 0;        v.y = r;            // 왼쪽시작으로 오른쪽으로 진행
            break;
            case DIR_HORT_VERT.W:
                v.x = ctg.size; v.y = r;            // 오른쪽시작으로 왼쪽으로 진행
            break;
        }
        return v;
    }

    public  Vector2Int  DirToVec(DIR_HORT_VERT d)
    {
        switch( d )
        {
            case DIR_HORT_VERT.N:return Vector2Int.up;
            case DIR_HORT_VERT.S:return Vector2Int.down;
            case DIR_HORT_VERT.E:return Vector2Int.right;
            case DIR_HORT_VERT.W:return Vector2Int.left;
        }
        return Vector2Int.zero;
    }

    //public  DIR_HORT_VERT RandomDir_DIR_HORT_VERT( DIR_HORT_VERT[] exc_arr ) // 제외 리스트
    public  DIR_HORT_VERT RandomDir_DIR_HORT_VERT( DIR_HORT_VERT exc ) // 제외 리스트
    {
        List<DIR_HORT_VERT> lt = new List<DIR_HORT_VERT>();
        // 수직선분으로 꺽는다.
        if( exc == DIR_HORT_VERT.N || exc == DIR_HORT_VERT.S )
        {
            lt.Add( DIR_HORT_VERT.E );
            lt.Add( DIR_HORT_VERT.W );
        }else{
            lt.Add( DIR_HORT_VERT.N );
            lt.Add( DIR_HORT_VERT.S );
        }

        int idx = UnityEngine.Random.Range( 0 , lt.Count );
        return lt[idx];
    }

    public  Vector2Int    Road_Work( DIR_HORT_VERT dir , Vector2Int vStart , Base_CityTownGenerator ctg , List<Vector2Int> lt_filled)
    {
        Vector2Int v_dir = DirToVec( dir );
        int length = UnityEngine.Random.Range( _hori_vert_line.distance_min , _hori_vert_line.distance_max );
        v_dir *= length;
        Vector2Int vEnd = vStart + v_dir;

        RectInt rc = new RectInt(0,0,ctg.size , ctg.size);

Debug.Log( gameObject.name + " : vStart: " + vStart + " : vEnd : " + vEnd );
        List<Vector2Int> lt_cell =  SJ_TileCommon.LineCell( vStart , vEnd , true , rc );

        foreach( Vector2Int s in lt_cell )
        {
            List<Vector2Int> lt_fill_cur =   load_stamp.Fill( tilemap_Fill , s );
            lt_filled.AddRange( lt_fill_cur );
        }
        return vEnd;
    }



}
