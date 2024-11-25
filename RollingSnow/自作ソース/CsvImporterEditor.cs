using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Codice.CM.Client.Differences.Graphic;
using System.IO;
using System;

[CustomEditor(typeof(CsvImporter))]
public class CsvImpoterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var csvImporter = target as CsvImporter;
        DrawDefaultInspector();

        if (GUILayout.Button("�n�ʂ̐���"))
        {
            Debug.Log("�쐬�{�^���������ꂽ");
            CreateGroundfromCsvData(csvImporter);
        }
        if (GUILayout.Button("�I�u�W�F�N�g�̐���"))
        {
            Debug.Log("�쐬�{�^���������ꂽ");
            CreateObjectfromCsvData(csvImporter);
        }
    }

    void CreateGroundfromCsvData(CsvImporter csvImporter)
    {
        GameObject[] ground = csvImporter.ground;

        GameObject groundParent = new GameObject("Grounds");
        Undo.RegisterCreatedObjectUndo(groundParent,"Create groundParent");

        if (csvImporter.groundCsvFile == null)
        {
            Debug.LogWarning("�ǂݍ���CSV�t�@�C�����Z�b�g����Ă��܂���B");
            return;
        }

        string csvText = csvImporter.groundCsvFile.text;

        string[] afterParse = csvText.Split('\n');

        for (int i = 0; i < afterParse.Length-1; i++)
        {
            string[] parseByComma = afterParse[i].Split(',');

            for (int j = 0; j < parseByComma.Length; j++)
            {
                int mapNumber = int.Parse(parseByComma[j]);
                if (mapNumber != 0) 
                {
                    GameObject gameObject = Instantiate(ground[mapNumber], new Vector3(i * 10,
                        ground[mapNumber].gameObject.transform.position.y, j * 10), Quaternion.identity);
                    gameObject.name = ground[mapNumber].name;
                    gameObject.transform.parent = groundParent.transform;
                }
            }
        }
        Debug.Log("�n�ʂ̍쐬���������܂����B");

    }
    void CreateObjectfromCsvData(CsvImporter csvImporter)
    {
        GameObject[] stageObject = csvImporter.objects;

        GameObject objectParent = new GameObject("Obejcts");
        Undo.RegisterCreatedObjectUndo(objectParent, "Create groundParent");
        if (csvImporter.objectCsvFile == null)
        {
            Debug.LogWarning("�ǂݍ���CSV�t�@�C�����Z�b�g����Ă��܂���B");
            return;
        }

        string csvText = csvImporter.objectCsvFile.text;

        string[] afterParse = csvText.Split('\n');

        for (int i = 0; i < afterParse.Length-1; i++)
        {
            string[] parseByComma = afterParse[i].Split(',');

            for (int j = 0; j < parseByComma.Length; j++)
            {
                int objectNumber = int.Parse(parseByComma[j]);
                if (objectNumber != 0)
                {
                    GameObject gameObject = Instantiate(stageObject[objectNumber],
                    new Vector3(i * 10, stageObject[objectNumber].gameObject.transform.position.y, j * 10),
                    Quaternion.Euler(stageObject[objectNumber].gameObject.transform.eulerAngles.x, stageObject[objectNumber].gameObject.transform.eulerAngles.y, stageObject[objectNumber].gameObject.transform.eulerAngles.z));
                    gameObject.name = stageObject[objectNumber].name;
                    gameObject.transform.parent = objectParent.transform;
                }
                
            }
        }

    }
}