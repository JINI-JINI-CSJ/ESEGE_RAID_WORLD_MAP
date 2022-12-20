using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class Menu_WorldMap : EditorWindow
{

    [MenuItem("ESEGE/WorldMap/Parse Image")]
    static public   void    WorldMap_ParseImage()
    {   
        GameObject obj = Selection.activeGameObject;

        if( obj == null )
        {
            Debug.Log("선택된 GameObject 없다!!!!!!!!!");
            return;
        } 

        WorldImageParser wip = obj.GetComponent<WorldImageParser>();

        if( wip == null )
        {
            Debug.Log("선택된 WorldImageParser 없다!!!!!!!!!");
            return;
        } 

        if(  wip.Step_1() == false ) return;
        wip.Step_1_Once();
    }

    [MenuItem("ESEGE/Town/BorderLine")]
    static public   void    Town_BorderLine()
    {
        Base_CityTownGenerator comp = Menu_SJ_Tile2D.GetComponent_Select<Base_CityTownGenerator>();
        if( comp == null ) return;
        //comp.Work_BorderLine();
    }

    [MenuItem("ESEGE/Town/LoadLine")]
    static public   void    Town_LoadLine()
    {
        Base_CityTownGenerator comp = Menu_SJ_Tile2D.GetComponent_Select<Base_CityTownGenerator>();
        if( comp == null ) return;
        //comp.Work_LineContent();
    }

    [MenuItem("ESEGE/Town/Area")]
    static public   void    Town_Area()
    {
        Base_CityTownGenerator comp = Menu_SJ_Tile2D.GetComponent_Select<Base_CityTownGenerator>();
        if( comp == null ) return;
        //comp.Work_AREA_CONTENT();
    }
}
