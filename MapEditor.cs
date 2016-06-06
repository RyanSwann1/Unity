using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(MapGenerator))]
public class MapEditor : Editor
{

    public override void OnInspectorGUI()
    {
        MapGenerator mapGen = target as MapGenerator;


        if(DrawDefaultInspector())
        {
            mapGen.GenerateMap();
        }
        
    }
}
