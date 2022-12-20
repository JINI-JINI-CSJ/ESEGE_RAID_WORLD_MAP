using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class Menu_SJ_Tile2D : EditorWindow
{
    static  public  T GetComponent_Select<T>()
    {
        GameObject obj = Selection.activeGameObject;
        if( obj == null )
        {
            Debug.Log("선택된 GameObject 없다!!!!!!!!!");
            return default(T);
        } 
        T comp = obj.GetComponent<T>();
        if( comp == null )
        {
            Debug.Log(" 컴포넌트 없다!!!!!!!!!");
            return default(T);
        } 
        return comp;
    }

    static  public  T[] GetComponent_Selects<T>()
    {
        List<T> lt = new List<T>();        
        GameObject[] objs = Selection.gameObjects;
        if( objs == null )
        {
            Debug.Log("선택된 GameObject 없다!!!!!!!!!");
            //return default(T[]);
            return lt.ToArray();
        } 


        foreach( GameObject s in objs )
        {
            T comp = s.GetComponent<T>();
            if( comp != null )
            {
                lt.Add(comp);
            }
        }

        return lt.ToArray();
    }

    static  public  T[] GetComponent_Select_Child<T>()
    {
        List<T> lt = new List<T>(); 
        GameObject obj = Selection.activeGameObject;
        if( obj == null )
        {
            Debug.Log("선택된 GameObject 없다!!!!!!!!!");
            return lt.ToArray();
        } 
        T[] comps = obj.GetComponentsInChildren<T>();
        return comps;
    }

    [MenuItem("SJ_Menu/TileMap/Clear")]
    static  public  void Tile_Clear()
    {
        Tilemap[] tilemap = Menu_SJ_Tile2D.GetComponent_Select_Child<Tilemap>();
        foreach( Tilemap s in tilemap )s.ClearAllTiles();
    }

    [MenuItem("SJ_Menu/Tile2DStampBrush/Tile2DStampBrush_Import")]
    static public   void    Tile2DStampBrush_Import()
    {
        SJ_Tile2DStampBrush comp = GetComponent_Select<SJ_Tile2DStampBrush>();
        if( comp != null )
             comp.Work_Import();
    }

    [MenuItem("SJ_Menu/Tile2DStampBrush/Tile2DStampBrush_FillTest")]
    static public   void    Tile2DStampBrush_FillTest()
    {
        SJ_Tile2DStampBrush comp = GetComponent_Select<SJ_Tile2DStampBrush>();
        if( comp != null )
            comp.Fill(null , 0,0);
    }

    [MenuItem("SJ_Menu/Tile2DStampBrush/Tile2DStampBrush_FillAreaRandom")]
    static public   void    Tile2DStampBrush_FillAreaRandom()
    {
        SJ_Tile2DStampBrush comp = GetComponent_Select<SJ_Tile2DStampBrush>();
        if( comp != null )comp.Work_Fill_AreaRandom();
    }

    [MenuItem("SJ_Menu/Tile2DFill_8Side/Tile2DFill_8Side_Test")]
    static public   void    SJ_Tile2DFill_8Side_Test()
    {
        SJ_Tile2DFill_8Side comp = GetComponent_Select<SJ_Tile2DFill_8Side>();
        if( comp != null )
            comp.Test();
    }

    [MenuItem("SJ_Menu/Tile2DFill_8Side/Tile2DFill_8Side_ImportAuto")]
    static public   void    Tile2DFill_8Side_ImportAuto()
    {
        SJ_Tile2DFill_8Side comp = GetComponent_Select<SJ_Tile2DFill_8Side>();
        if( comp != null )
            comp.Import_Auto();
    }
}
