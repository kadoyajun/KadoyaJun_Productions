using UnityEngine;
using UnityEngine.UI;

namespace Genzan
{
    [System.Serializable]
    class CloudImage
    {
        public Image[] cloud;
    }

    public class TitleBackground : MonoBehaviour
    {
        //“®‚©‚·‰æ‘œ
        [SerializeField]
        CloudImage[] backgroundImage;

        //1ü‚É‚©‚¯‚é•b”
        [SerializeField]
        float[] oneLapsSeconds;

        int[] imageWidth = new int[3];

        void Start()
        {
            for (int i = 0; i < backgroundImage.Length; i++)
            {
                imageWidth[i] = backgroundImage[i].cloud[0].sprite.texture.width;
            }
        }

        void Update()
        {
            for (int i = 0; i < backgroundImage.Length; i++)
            {
                for (int j = 0; j < backgroundImage[i].cloud.Length; j++)
                {
                    Vector2 rect = backgroundImage[i].cloud[j].rectTransform.anchoredPosition;
                    rect.x -= imageWidth[i] / oneLapsSeconds[i] / (1 / Time.deltaTime);
                    if (rect.x <= -imageWidth[i])
                    {
                        rect.x = imageWidth[i] + (rect.x + imageWidth[i]);
                    }
                    backgroundImage[i].cloud[j].rectTransform.anchoredPosition = rect;
                }
            }
        }
    }


}
