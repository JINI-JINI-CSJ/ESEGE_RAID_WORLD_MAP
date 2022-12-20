using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SJ_TileCommon
{

    static  public  void    DeleteChildAll(GameObject go)
    {
        List<Transform> lt = new List<Transform>();
        for( int i = 0 ; i < go.transform.childCount ; i++ )
        {
            lt.Add( go.transform.GetChild(i) );
        }

        foreach( Transform t in lt )
        {
            GameObject.DestroyImmediate( t.gameObject );
        }
    }

    public  static  Vector2Int  Random_Vec2Int( int val )
    {
        Vector2Int vi = new Vector2Int();
        vi.x = UnityEngine.Random.Range( 0,val+1 );
        vi.y = UnityEngine.Random.Range( 0,val+1 );
        return vi;
    }

    public  static  Vector2Int  Random_Vec2Int_Rect( RectInt rc )
    {
        Vector2Int vi = new Vector2Int();
        vi.x = UnityEngine.Random.Range( rc.xMin,rc.xMax+1 );
        vi.y = UnityEngine.Random.Range( rc.yMin,rc.yMax+1 );
        return vi;
    }

    public  static  Vector2Int  Random_Vec2Int_Rect( Vector2Int ct , int size_x , int size_y )
    {
        ct.x -= (size_x / 2);
        ct.y -= (size_y / 2);        
        RectInt rc = new RectInt(ct.x , ct.y , size_x , size_y);
        return Random_Vec2Int_Rect( rc );
    }

    static  public  bool    Check_TilemapEmpty( Tilemap tilemap , int sx , int sy , int w , int h )
    {
        for( int y = sy ; y < sy + h + 1 ; y++ )
        {
            for( int x = sx ; x < sx + w + 1 ; x++ )
            {
                TileBase tb = tilemap.GetTile( new Vector3Int( x , y ) );
                if( tb != null ) return false;
            }
        }
        return true;
    }

    static  public  List<Vector2Int>    LineCell( Vector2Int s , Vector2Int e , bool inc_se = true , RectInt rectInt = default )
    {
        List<Vector2Int>    lt = new List<Vector2Int>();


        Vector2Int  sei = e - s;
        Vector2     step = sei;

        step.Normalize();
        step *= 0.5f;


        if( inc_se )
            lt.Add(s);

        Vector2     cur = s;
        Vector2Int  cur_i = s;
        Vector2Int  next_i = s;


//Debug.Log( "시작 라인 체크  : " + s + " : " + e );
        int c = 0;
        while(true)
        {
            cur += step;
            next_i.x = (int)cur.x;
            next_i.y = (int)cur.y;

            if( next_i == e )
            {
                if( inc_se )
                    lt.Add(e);
                break;
            }

            // 긴축기준으로 넘어간거 체크
            if( Mathf.Abs( sei.x ) > Mathf.Abs( sei.y ) )
            {
                if( sei.x > 0 )
                {
                    if( next_i.x > e.x )
                    {
                        break;
                    }
                }else{
                    if( next_i.x < e.x )
                    {
                        break;
                    }
                }
            }else{
                if( sei.y > 0 )
                {
                    if( next_i.y > e.y )
                    {
                        break;
                    }
                }else{
                    if( next_i.y < e.y )
                    {
                        break;
                    }
                }
            }

//Debug.Log( " 체크  : " + next_i + " : " + c );
            if( cur_i != next_i )
            {
                cur_i = next_i;
                if( rectInt.size.sqrMagnitude > 1 )
                {
                    if(rectInt.Contains( next_i ) == false )
                    {
                        Debug.Log( "외곽 종료 : " + next_i );
                        break;
                    }
                }
                lt.Add(next_i);
            }
            c++;
            
            if( c > 100000 )
            {
                Debug.LogError( "!!!! LineCell : 오버!!!" );
                break;
            }
            
        }
//Debug.Log( "끝 라인 체크" );
        return lt;
    }

    
}
