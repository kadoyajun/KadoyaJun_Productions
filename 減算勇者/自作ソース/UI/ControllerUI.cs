using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Genzan
{
    public class ControllerUI : MonoBehaviour
    {
        [SerializeField]
        Sprite[] controllerImage;

        Image image;

        GameManager gM;

        [SerializeField]
        GameObject menu;

        private void Start()
        {
            gM = transform.root.GetComponent<GameManager>();
            image = GetComponent<Image>();
        }

        // Update is called once per frame
        void Update()
        {
            if(gM.gameState == GameManager.GameState.StageClear)
            {
                image.sprite = controllerImage[2];
            }
            else if (menu.activeSelf)
            {
                image.sprite = controllerImage[1];
            }
            else
            {
                image.sprite = controllerImage[0];
            }
        }
    }

}

