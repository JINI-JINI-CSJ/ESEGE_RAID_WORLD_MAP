using System.Collections;
using System.Collections.Generic;


// 분석된 월드맵의 각 픽셀당 블럭정보
public  class _WORLD_INFO
{
    public  string  ID;
    public  string  desc;

    public  int     size_x;
    public  int     size_y;

    public  List<byte>   lt_block_id = new List<byte>();

}


// 영역 존 설정
public  class _WORLD_ZONE_INFO
{
    public  string  ID;
    public  string  desc;

    public  int     x;
    public  int     y;
    public  int     radius;
}

// 거점같은 객체 배치 설정
public  class _WORLD_STRONG_POINT_INFO
{
    public  string  ID;    
    public  string  desc;
    public  int     cell_size;                  
    public  int     arrangement_type;           // 배치 타입 , 일반 , 바다위, 산맥 등등 
    public  int     distance_min;               // 다른 배치나 제한셀과 최소한의 거리
    public  int     distance_common_string_point;   // 다른 모든 거점과의 거리 (제한셀은 제외)
    public  int     distance_eq;                // 이 타입과 같은 거점끼리의 거리
}

public class DefineWorldMap 
{

}
