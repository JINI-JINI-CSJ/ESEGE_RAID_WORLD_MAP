using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
# if UNITY_EDITOR
using UnityEditor;
# endif

/// <summary>
/// 픽셀별로 그룹을 만든다
/// 가장 많은 높은 그룹을 우선적으로 나열
/// </summary>
public class WorldImageParser : MonoBehaviour
{
    public  Tilemap     tilemap;
    public  Texture2D   tex_image;
    // 픽셀 그룹과 블록아이디 매칭 정의
    // 유저가 많이 나올꺼 같은 색깔들을 미리 매칭
    [System.Serializable]
    public  class _BLOCK_MATCH_PIXEL
    {
        public  Color32 col;
        public  int     count_parse;
        public  int     define_block_id;

        public  Sprite  spr_block_view;     // 확인용 타일
        public  TileBase    tb;

        public  List<_PIXEL_INFO>   lt_PIXEL_INFO = new List<_PIXEL_INFO>();
    }
    
    public  List<_BLOCK_MATCH_PIXEL>    lt_BLOCK_MATCH_PIXEL = new List<_BLOCK_MATCH_PIXEL>();

    // 각 픽셀 정보
    public class _PIXEL_INFO
    {
        public  Color32 col;
        public  int     x;
        public  int     y;
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    /**
     * 0. 블록 매칭 픽셀 정의
     * 1. 모든 픽셀 분석
     * 2. 가장 많은 픽셀별로 순위를 정한다
     * 3. 순위정한 픽셀그룹을 블록 아이디를 지정한다
     * 4. 픽셀마다 블록아이디를 매칭한다. 정확히 같은 색이 없으면 순위픽셀그룹중에 가장 가까운 픽셀그룹을 매칭한다.
    */

   public   int     prc_count_PerFrame = 500;
   int             play = 0;
   public   int     cur_prc_x = 0;
   public   int     cur_prc_y = 0;

   public   int     total_pixel;

   Color32[] cols;

   public   TileBase tb_test;

    public  void    Test()
    {
        for( int y = 0 ; y < 10 ; y++ )
        {
            for( int x = 0 ; x < 100 ; x++ )
            {
                tilemap.SetTile( new Vector3Int( x , y ) , tb_test);
            }
        }
    }

    public   bool    Step_1()
    {
        if( play != 0 )
        {
            Debug.Log( "이미 실행중이다!!!!!" );
            return false;
        } 

        if( tilemap == null )
        {
            Debug.Log( "연결된 타일맵 없다!!!!!" );
            return false;
        } 

        if( tex_image == null )
        {
            Debug.Log( "파싱할 이미지가 없다!!!!" );
            return false;
        }

        if( lt_BLOCK_MATCH_PIXEL.Count < 1 )
        {
            Debug.Log( "매칭 픽셀 블록 정의 없다!!!!" );
            return false;
        }

        cur_prc_x = 0;
        cur_prc_y = 0;
        play = 0;
        cols = tex_image.GetPixels32();

        total_pixel = tex_image.height * tex_image.width;

        Debug.Log( "cur_prc_x : " + cur_prc_x );
        Debug.Log( "cur_prc_y : " + cur_prc_y );
        Debug.Log( "tex_image.width : " + tex_image.width );
        Debug.Log( "tex_image.height : " + tex_image.height );
        Debug.Log( "total_pixel : " + total_pixel );

        for( int i = 0 ; i < lt_BLOCK_MATCH_PIXEL.Count ; i++ )
        {
            lt_BLOCK_MATCH_PIXEL[i].lt_PIXEL_INFO.Clear();
        }

        return true;
    }

    public  void    Step_1_Once()
    {
        for (int y = 0; y < tex_image.height; y++) 
        {
            for ( int x = 0; x < tex_image.width; x++) 
            {
                _PIXEL_INFO p = new _PIXEL_INFO();
                p.col = cols[y*tex_image.width+x];
                p.x = x;
                p.y = y;

                addMatchBlock_put(p);
            }
        }
    }

    public  bool    Step_1_update()
    {
        if( cur_prc_y + 1 >= tex_image.height  && cur_prc_x + 1 >= tex_image.width )
        {
            
            play = 1;
            Fill_ViewTile();
            play = 0;
            return true;
        }

        int c = 0;
        int x = 0,y = 0;
        for ( y = cur_prc_y; y < tex_image.height; y++) 
        {
            for ( x = cur_prc_x; x < tex_image.width; x++) 
            {
                _PIXEL_INFO p = new _PIXEL_INFO();
                p.col = cols[y*tex_image.width+x];
                p.x = x;
                p.y = y;

                addMatchBlock(p);

                c++;
                if( c >= prc_count_PerFrame )
                {
                    cur_prc_x = x;
                    cur_prc_y = y;
                    return false;
                }
            }
            Debug.Log( "Step_1_update : " + y );
        }
        cur_prc_x = x;
        cur_prc_y = y;
        return false;
    } 

    public  float   GetProgress()
    {
        return (float)(cur_prc_x*cur_prc_y) / (float)total_pixel;
    }

    void    addMatchBlock_put(_PIXEL_INFO p)
    {
        _BLOCK_MATCH_PIXEL near_bm = null;
        int t_c = 1000000000;
        foreach( _BLOCK_MATCH_PIXEL bm in lt_BLOCK_MATCH_PIXEL )
        {
            int t = Mathf.Abs( (int)p.col.r - (int)bm.col.r ) + 
            Mathf.Abs( (int)p.col.g - (int)bm.col.g ) +
            Mathf.Abs( (int)p.col.b - (int)bm.col.b );

            if( t < t_c )
            {
                near_bm = bm;
                t_c = t;
            }
        }

        tilemap.SetTile( new Vector3Int( p.x , p.y ) , near_bm.tb );
    }

    void    addMatchBlock(_PIXEL_INFO p)
    {
        _BLOCK_MATCH_PIXEL near_bm = null;
        int t_c = 1000000000;
        foreach( _BLOCK_MATCH_PIXEL bm in lt_BLOCK_MATCH_PIXEL )
        {
            int t = Mathf.Abs( (int)p.col.r - (int)bm.col.r ) + 
            Mathf.Abs( (int)p.col.g - (int)bm.col.g ) +
            Mathf.Abs( (int)p.col.b - (int)bm.col.b );

            if( t < t_c )
            {
                near_bm = bm;
                t_c = t;
            }
        }

        near_bm.lt_PIXEL_INFO.Add(p);
    }

    public  void    Fill_ViewTile()
    {
        int c = 0;
        Debug.Log( "Fill_ViewTile : Start " );
        foreach( _BLOCK_MATCH_PIXEL s in lt_BLOCK_MATCH_PIXEL )
        {
            foreach( _PIXEL_INFO pi in s.lt_PIXEL_INFO )
            {
                tilemap.SetTile( new Vector3Int( pi.x , pi.y ) , s.tb );
                Debug.Log( "Fill_ViewTile : x " + pi.x + " : y : " + pi.y );
            }

            Debug.Log( "Fill_ViewTile BLOCK_MATCH_PIXEL : " + s.lt_PIXEL_INFO.Count );
            c+=s.lt_PIXEL_INFO.Count;
            //c++;
        }
        Debug.Log( "Fill_ViewTile : End : c : " + c + " : total_pixel : " + total_pixel );
    }

    public  void    RollBack()
    {
        play = 0;
    }
}


