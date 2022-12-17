using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class    SJ_Tile2DBuffer : MonoBehaviour
{
    public  int         size;
    public  int[]       maps;
    public  Tilemap     tilemap_view;

    [System.Serializable]
    public  class _TILE_VIEW_INT
    {
        public  string  name;
        public  int val;
        public  TileBase    tb;
    }
    public  List<_TILE_VIEW_INT>        lt_TILE_VIEW_INT;
    public  Dictionary<int,TileBase>    dic_ValTile = new Dictionary<int, TileBase>();

    public  class _LINE
    {
        public  int     sx;
        public  int     sy;
        public  int     ex;
        public  int     ey;
        public  int     tile_val;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public  void    Put( int x , int y , int val , Tilemap _tilemap = null )
    {
        if( x < 0 || y < 0 || x > size || y > size ) return;
        if( maps.Length < 1 )
        {
            maps = new int[size*size];
        }
        maps[y*size+x] = val;

        Tilemap tilemap = tilemap_view;
        if( _tilemap != null ) tilemap = _tilemap;

        if( tilemap != null )
        {
            if( dic_ValTile.Count != lt_TILE_VIEW_INT.Count )
            {
                dic_ValTile.Clear();
                foreach( _TILE_VIEW_INT s in lt_TILE_VIEW_INT )
                {
                    dic_ValTile[s.val] = s.tb;
                }
            }

            TileBase f_tb = null;
            dic_ValTile.TryGetValue( val , out f_tb );
            tilemap.SetTile( new Vector3Int(x,y) , f_tb );
        }
    }

    public  void    Line( int sx , int sy , int ex , int ey , int val , Tilemap _tilemap = null )
    {
        if( sx == ex && sy == ey ) return;

        int dis_x = Mathf.Abs( ex - sx );
        int dis_y = Mathf.Abs( ey - sy );

        int c = 0;
        // 긴축 기준으로 계산
        if( dis_x > dis_y )
        {
            if( ex > sx ) 
            {
                for( int x = sx ; x <= ex  ; x++ )
                {
                    float f = (float)c / (dis_x);
                    int y = sy + (int)( (float)(ey-sy) * f );  
                    Put( x , y , val , _tilemap );
                    c++;
                }
            }else{
                for( int x = sx ; x >= ex ; x-- )
                {
                    float f = (float)c / (dis_x);
                    int y = sy + (int)( (float)(ey-sy) * f );  
                    Put( x , y , val , _tilemap );
                    c++;
                }
            }
        }else{
            if( ey > sy ) 
            {
                for( int y = sy ; y <= ey  ; y++ )
                {
                    float f = (float)c / (dis_y);
                    int x = sx + (int)( (float)(ex-sx) * f );  
                    Put( x , y , val , _tilemap );
                    c++;
                }
            }else{
                for( int y = sy ; y >= ey ; y-- )
                {
                    float f = (float)c / (dis_y);
                    int x = sx + (int)( (float)(ex-sx) * f );   
                    Put( x , y , val , _tilemap );
                    c++;
                }
            }
        }
    }

}
