using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


///
// 사각텍스쳐 마스크 영역으로 타일맵 위를 지정한 타일로 채운다
//  
///
public class SJ_Tile2DFill : MonoBehaviour
{
    public  Tilemap     tilemap;
    public  Texture2D   tex; // 월드 유닛당 픽셀은 원본 이미지 사이즈크기로 해야 한다. (예:1280 )
    public  Color32     color_mask;
    public  float       per_pixel_unit; 
    public  int         tilemap_max_x;
    public  int         tilemap_max_y;
    public  TileBase    tb;

    public  delegate void Dlg_FillTile(Vector3Int vi);

    public  Dlg_FillTile    dlg_FillTile;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public  void    Fill()
    {
        // 스탬프 픽셀 단위로 행렬 곱해서 나온 위치값이
        // 타일맵위치의 칠한 타일이 있으면 타일 색칠

        int tx = -1 , ty = -1;
        Color32[]  cols = tex.GetPixels32();

        for (int y = 0; y < tex.height; y++) 
        {
            for ( int x = 0; x < tex.width; x++) 
            {
                Color32 c = cols[y*tex.width+x];

                if( c.r == color_mask.r && c.g == color_mask.g  && c.b == color_mask.b )
                {
                    continue;
                }

                // 월드 좌표는 유니티 타일맵 xy 좌표
                // 위로 갈수록 y 증가 , 텍스쳐 좌표도 보이는건 똑같다.
                // 대신 원점이 이미지 중점이므로 -0.5 비율 이동

                float fx = (float)x / per_pixel_unit - 0.5f;
                float fy = (float)y / per_pixel_unit - 0.5f;

                Vector3 v1 = transform.TransformPoint( fx , fy, 0 );

                Vector3Int vi = new Vector3Int( (int)v1.x,(int)v1.y,0 );

                if( tx == vi.x && ty == vi.y ) continue;

                // 타일맵은 유니티 기본 생성 좌표 기준으로 계산
                // 이 범위를 나가면 취소
                if( vi.x < 0 || vi.y < 0 ) continue;
                if( vi.x > tilemap_max_x || vi.y > tilemap_max_y ) continue;

                tilemap.SetTile(vi , tb);

                if( dlg_FillTile != null )
                {
                    dlg_FillTile.Invoke(vi);
                }
            }
        }
    }
    
}
