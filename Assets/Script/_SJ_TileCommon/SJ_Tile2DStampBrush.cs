using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// 유니티 타일맵에 브러쉬 기능이 읍넹.;;;;
// 브러쉬 만들때는 임의의 타일맵에서 읽어오기

public class SJ_Tile2DStampBrush : MonoBehaviour
{
    
    public  string  Name;
    public  int     ID;
    [System.Serializable]
    public  class _TILE_ADD_OBJ
    {
        public  int         x;
        public  int         y;
        public  TileBase    tb;
        public  GameObject  go;
    }

    [System.Serializable]
    public  class _TILE_STAMP_BRUSH
    {
        public  int     layer;
        public  Tilemap tilemap_Import;
        public  int     size_x;
        public  int     size_y;
        public  List<_TILE_ADD_OBJ>  lt_tb;
    }

    public  int     stamp_border = 1;
    public  int     size_x_max;
    public  int     size_y_max;

    //public  _TILE_STAMP_BRUSH   stamp_brush;
    public  List<_TILE_STAMP_BRUSH> lt_stamp_brush;

    public  List<Tilemap>   lt_TileMap_Test;

    public  int     fill_AreaRandom_x = 1000;
    public  int     fill_AreaRandom_y = 1000;
    public  Tilemap tilemap_fill_AreaRandom;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
 
    // 대략 중심에서 1000 사이즈 타일맵 읽기 
    public  void    Work_Import()
    {
        foreach( _TILE_STAMP_BRUSH ts in lt_stamp_brush )
        {
            Import_Layer( ts );
        }

        size_x_max = 0;
        size_y_max = 0;
        foreach( _TILE_STAMP_BRUSH ts in lt_stamp_brush )
        {
            if( ts.size_x > size_x_max ) size_x_max = ts.size_x;
            if( ts.size_y > size_y_max ) size_y_max = ts.size_y;
        }
    } 

    public  void    Import_Layer( _TILE_STAMP_BRUSH ts )
    {
        ts.lt_tb.Clear();

        int size = 100;

        Vector2Int vMin = new Vector2Int( 100000,100000 );
        Vector2Int vMax = new Vector2Int( -100000,-100000 );

        BoundsInt   bi = new BoundsInt( new Vector3Int(0,0) , new Vector3Int(size,size,1) );
        TileBase[]  tbs = ts.tilemap_Import.GetTilesBlock( bi );

        for( int y = 0 ; y < size; y ++ )
        {
            for( int x = 0 ; x < size ; x++ )
            {
                TileBase tb = tbs[y*size+x];
                if( tb != null )
                {
                    _TILE_ADD_OBJ ta = new _TILE_ADD_OBJ();
                    ta.tb = tb;
                    ta.x = x;
                    ta.y = y;
                    ts.lt_tb.Add( ta );
                    if( vMin.x > x ) vMin.x = x;
                    if( vMin.y > y ) vMin.y = y;
                    if( vMax.x < x ) vMax.x = x;
                    if( vMax.y < y ) vMax.y = y;
                }
            }
        }

        ts.size_x = vMax.x - vMin.x + 1;
        ts.size_y = vMax.y - vMin.y + 1;


        if( ts.size_x < 1 || ts.size_y < 1 ) 
        {
            return;
        }

        foreach( _TILE_ADD_OBJ s in ts.lt_tb )
        {
            s.x -= vMin.x;
            s.y -= vMin.y;
        }
    }

    public  bool    Check_InRect( RectInt rc , int cx , int cy )
    {
        int tx = cx - (size_x_max/2);
        int ty = cy - (size_y_max/2);
        Vector2Int vs = new Vector2Int(tx,ty);
        Vector2Int ve = new Vector2Int(tx+size_x_max,ty+size_y_max);
        vs.x -= stamp_border;
        vs.y -= stamp_border;
        ve.x += stamp_border;
        ve.y += stamp_border;
        if( rc.Contains( vs ) == false || rc.Contains( ve ) == false ) return false;
        return true;
    }

    public  bool    Fill( List<Tilemap> _lt_tilemap ,int cx , int cy , bool check_target_tile = false , GameObject go_par_event = null)
    {
        if( _lt_tilemap == null || _lt_tilemap.Count < 1 )
        {
            _lt_tilemap = lt_TileMap_Test;
            if( _lt_tilemap.Count < 1) return false;
        }


        int c = 0;
        if( check_target_tile )
        {
            foreach( _TILE_STAMP_BRUSH ts in lt_stamp_brush )
            {
                int tx = cx - (ts.size_x/2);
                int ty = cy - (ts.size_y/2);

                Tilemap _tilemap = null;
                if( _lt_tilemap.Count > c )
                {
                    _tilemap = _lt_tilemap[c];
                }
                c++;

                if( _tilemap == null )
                    continue;

                if( SJ_TileCommon.Check_TilemapEmpty( _tilemap , tx - stamp_border , ty - stamp_border , 
                    ts.size_x + stamp_border*2, 
                    ts.size_y + stamp_border*2) == false )
                {
                    return false;
                }
                
            }
        }

        c = 0;
        foreach( _TILE_STAMP_BRUSH ts in lt_stamp_brush )
        {
            int tx = cx - (ts.size_x/2);
            int ty = cy - (ts.size_y/2);

            Tilemap _tilemap = null;
            if( _lt_tilemap.Count > c )
            {
                _tilemap = _lt_tilemap[c];
            }
            c++;
            Fill_Unit( _tilemap , ts , cx , cy , go_par_event );
        }

        return true;
    }

    public  bool    Fill_Unit( Tilemap _tilemap , _TILE_STAMP_BRUSH ts , int cx , int cy , GameObject go_par_event = null )
    {
        int tx = cx - (ts.size_x/2);
        int ty = cy - (ts.size_y/2);

        foreach( _TILE_ADD_OBJ s in ts.lt_tb )
        {
            Vector3Int vi = new Vector3Int( tx + s.x , ty + s.y  );
            _tilemap.SetTile( vi , s.tb );
            if( s.go != null && go_par_event != null )
            {
                GameObject inst = GameObject.Instantiate( s.go );
                inst.transform.parent = go_par_event.transform;
                inst.transform.localPosition = vi;
            }
        }
        return true;        
    }

    public  void    Work_Fill_AreaRandom( Tilemap _tilemap = null )
    {
        Tilemap tm = tilemap_fill_AreaRandom;
        if( _tilemap != null ) tm = _tilemap;
        _TILE_STAMP_BRUSH ts = lt_stamp_brush[0];
        for( int y = 0 ; y <= fill_AreaRandom_y ; y++ )
        {
            for( int x = 0 ; x <= fill_AreaRandom_x ; x++ )
            {
                int idx = UnityEngine.Random.Range( 0 , ts.lt_tb.Count );
                TileBase tb = ts.lt_tb[idx].tb;
                tm.SetTile( new Vector3Int( x,y,0 ) , tb );
            }
        }
    }
}
