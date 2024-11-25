using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Genzan
{
    public class StageSelectScene : MonoBehaviour
    {
        GameManager gM;

        [SerializeField]
        GameObject nowLoadingUI;

        [SerializeField]
        AreaRandomSetting areaRandomSetting;

        void Start()
        {
            gM = GetComponent<GameManager>();
            if (PlayerPrefs.GetInt("Stage1Clear") == 1)
            {
                List<int> deleteObjectNumber = new List<int>();
                for (int i = 0; i < gM.fieldObject.Count; i++)
                {
                    if (gM.fieldObject[i].objectType != ObjectController.ObjectType.Fire)
                    {
                        DestroyObject destroyObject = gM.fieldObject[i].gameObject.GetComponent<DestroyObject>();
                        destroyObject.DeleteThisObject(true);
                        //gM._object.Remove(gM._object[i]);
                        deleteObjectNumber.Insert(0, i);
                    }
                }
                int deleteObjectCount = deleteObjectNumber.Count;
                for (int i = 0; i < deleteObjectCount; i++)
                {
                    gM.fieldObject.Remove(gM.fieldObject[deleteObjectNumber[i]]);
                }
                PlayerPrefs.SetInt("Stage1Clear", 2);
            }
            else if (PlayerPrefs.GetInt("Stage1Clear") == 2)
            {
                List<int> deleteObjectNumber = new List<int>();
                for (int i = 0; i < gM.fieldObject.Count; i++)
                {
                    if (gM.fieldObject[i].objectType != ObjectController.ObjectType.Fire)
                    {
                        DestroyObject destroyObject = gM.fieldObject[i].gameObject.GetComponent<DestroyObject>();
                        destroyObject.DeleteThisObject(false);
                        deleteObjectNumber.Insert(0, i);
                    }
                }
                int deleteObjectCount = deleteObjectNumber.Count;
                for (int i = 0; i < deleteObjectCount; i++)
                {
                    gM.fieldObject.Remove(gM.fieldObject[deleteObjectNumber[i]]);
                }
            }
        }

        void Update()
        {
            if (gM.AreaNumber == 3)
            {
                nowLoadingUI.SetActive(true);
                SceneManager.LoadScene("Stage1Scene");
            }
            if (gM.AreaNumber == 7)
            {
                nowLoadingUI.SetActive(true);
                SceneManager.LoadScene("Stage2Scene");
            }
        }
    }

}

