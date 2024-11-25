using UnityEngine;

namespace Genzan
{
    public class GroundController : MonoBehaviour
    {
        public int AreaNumber;
        public int[,] groundNumber;
        void Awake()
        {
            groundNumber = new int[CNum.ROW, CNum.COLUMN];
            GameObject[,] gameObjects = new GameObject[CNum.ROW, CNum.COLUMN];
            for (int i = 0; i < CNum.ROW; i++)
            {
                for (int j = 0; j < CNum.COLUMN; j++)
                {
                    gameObjects[i, j] = transform.GetChild(i * CNum.ROW + j).gameObject;
                    if (gameObjects[i, j].gameObject.tag == "Ground_Grass")
                    {
                        groundNumber[i, j] = 0;
                    }
                    else if (gameObjects[i, j].gameObject.tag == "Ground_Poison")
                    {
                        groundNumber[i, j] = 1;
                    }
                    else if (gameObjects[i, j].gameObject.tag == "Ground_Goal")
                    {
                        groundNumber[i, j] = 2;
                    }
                }
            }
        }
    }
}