using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyScriptable/Create CSV Impoter")]
public class CsvImporter : ScriptableObject
{
    public TextAsset groundCsvFile;
    public TextAsset objectCsvFile;
    public GameObject[] ground = new GameObject[0];
    public GameObject[] objects = new GameObject[0];
}