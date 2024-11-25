using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Block
{
    public Vector2Int position;
    public bool dug = false;
    public int digDirection = 0;
    public Block parent = null;
}

public class MakeMaze : MonoBehaviour
{
    public Block[,] block;
    public Block startBlock;
    public Block goalBlock;
    int width = 0;
    public int Width => width;
    int height = 0;
    public int Height => height;
    [SerializeField]
    GameObject digBlockImg;
    [SerializeField]
    GameObject maze;
    public GameObject[,] mazeBlock;
    [SerializeField]
    TMP_InputField widthInput;
    [SerializeField]
    TMP_InputField heightInput;

    public void MazeGenerateStart()
    {
        ResetMaze();

        int widthInputValue = int.Parse(widthInput.text);
        int heightInputValue = int.Parse(heightInput.text);

        //3未満では迷路が生成できないため
        if(widthInputValue < 3 || heightInputValue < 3)
        {
            return;
        }

        //奇数でなければいけないため
        if (widthInputValue % 2 == 0)
        {
            widthInputValue++;
            widthInput.text = widthInputValue.ToString();
        }
        if(heightInputValue % 2 == 0)
        {
            heightInputValue++;
            heightInput.text = heightInputValue.ToString();
        }

        width = widthInputValue;
        height = heightInputValue;

        block = new Block[width, height];
        mazeBlock = new GameObject[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                block[i, j] = new()
                {
                    dug = false,
                    position = new Vector2Int(i, j)
                };
            }
        }
        GenerateMaze();
        SetStartGoal();
        DrawingMaze();
    }

    private void ResetMaze()
    {
        foreach (Transform child in maze.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        Navigation navigation = GetComponent<Navigation>();
        navigation.StopNavi();
    }

    void GenerateMaze()
    {
        //基準点を設定
        Block baseBlock = new();
        int baseX;
        int baseY;
        while (true)
        {
            baseX = Random.Range(0, width);
            baseY = Random.Range(0, height);
            if(baseX % 2 != 0 && baseY % 2 != 0)
            {
                baseBlock = block[baseX, baseY];
                block[baseX, baseY].dug = true;
                break;
            }
        }
        while (true)
        {
            baseX = baseBlock.position.x;
            baseY = baseBlock.position.y;

            //移動方向を設定
            List<int> digDirection = new();
            while (true)
            {
                bool w = true;
                int d = Random.Range(0, 4);
                for (int i = 0; i < digDirection.Count; i++)
                {
                    if (digDirection[i] == d)
                    {
                        w = false;
                    }
                }
                if (w)
                {
                    digDirection.Add(d);
                    if (digDirection.Count == 4)
                    {
                        break;
                    }
                }
            }

            //指定の方向に掘ることができるかチェックし掘る
            for (int i = 0; i < digDirection.Count; i++)
            {
                bool canMove = false;
                switch (digDirection[i])
                {
                    case CNum.UP:
                        if (baseBlock.position.y + 2 >= 0 && baseY + 2 < height)
                        {
                            if (block[baseX, baseY + 2].dug == false)
                            {
                                block[baseX, baseY + 1].dug = true;
                                block[baseX, baseY + 2].dug = true;
                                canMove = true;
                                block[baseX, baseY].digDirection = i;
                                block[baseX, baseY + 2].parent = block[baseX, baseY];
                                baseBlock = block[baseX, baseY + 2];
                            }
                        }
                        break;
                    case CNum.DOWN:
                        if (baseY - 2 >= 0 && baseY - 2 < height)
                        {
                            if (block[baseX, baseY - 2].dug == false)
                            {
                                block[baseX, baseY - 1].dug = true;
                                block[baseX, baseY - 2].dug = true;
                                canMove = true;
                                block[baseX, baseY].digDirection = i;
                                block[baseX, baseY - 2].parent = block[baseX, baseY];
                                baseBlock = block[baseX, baseY - 2];
                            }
                        }
                        break;
                    case CNum.LEFT:
                        if (baseX - 2 >= 0 && baseX - 2 < width)
                        {
                            if (block[baseX - 2, baseY].dug == false)
                            {
                                block[baseX - 1, baseY].dug = true;
                                block[baseX - 2, baseY].dug = true;
                                canMove = true;
                                block[baseX, baseY].digDirection = i;
                                block[baseX - 2, baseY].parent = block[baseX, baseY];
                                baseBlock = block[baseX - 2, baseY];
                            }
                        }
                        break;
                    case CNum.RIGHT:
                        if (baseX + 2 >= 0 && baseX + 2 < width)
                        {
                            if (block[baseX + 2, baseY].dug == false)
                            {
                                block[baseX + 1, baseY].dug = true;
                                block[baseX + 2, baseY].dug = true;
                                canMove = true;
                                block[baseX, baseY].digDirection = i;
                                block[baseX + 2, baseY].parent = block[baseX, baseY];
                                baseBlock = block[baseX + 2, baseY];
                            }
                        }
                        break;
                    default:
                        break;
                }
                if (canMove)
                {
                    break;
                }

                //4方向掘れなかったら一つ戻る
                if (i == digDirection.Count - 1)
                {
                    if (baseBlock.parent != null)
                    {
                        baseBlock = baseBlock.parent;
                    }
                    else
                    {
                        goto finish;
                    }
                }
            }
        }

    finish:
        return;
    }

    void SetStartGoal()
    {
        //上辺または左辺にスタートを配置
        bool startSet = false;
        while (!startSet)
        {
            int verticalOrHorizontal = Random.Range(0, 2);
            if (verticalOrHorizontal == 0)
            {
                int vertical = Random.Range(1, width - 1);
                if (block[vertical, 1].dug)
                {
                    startBlock = block[vertical, 0];
                    startSet = true;
                }
            }
            else
            {
                int horizontal = Random.Range(1, height - 1);
                if (block[1, horizontal].dug)
                {
                    startBlock = block[0, horizontal];
                    startSet = true;
                }
            }
        }
        startBlock.dug = true;

        //下辺または右辺にゴールを配置
        bool goalSet = false;
        while (!goalSet)
        {
            int verticalOrHorizontal = Random.Range(0, 2);
            if (verticalOrHorizontal == 0)
            {
                int vertical = Random.Range(1, width - 1);
                if (block[vertical, height - 2].dug)
                {
                    goalBlock = block[vertical, height - 1];
                    goalSet = true;
                }
            }
            else
            {
                int horizontal = Random.Range(1, height - 1);
                if (block[width - 2, horizontal].dug)
                {
                    goalBlock = block[width - 1, horizontal];
                    goalSet = true;
                }
            }
        }
        goalBlock.dug = true;
    }

    void DrawingMaze()
    {
        GameObject[,] setBlockImg = new GameObject[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                setBlockImg[i, j] = digBlockImg;
                float sideW = 1.0f / width;
                float sideH = 1.0f / height;
                Image image = setBlockImg[i, j].GetComponent<Image>();
                image.rectTransform.anchorMin = new Vector2(i * sideW, 1 - (j + 1) * sideH);
                image.rectTransform.anchorMax = new Vector2((i + 1) * sideW, 1 - j * sideH);
                if (block[i, j].dug == true)
                {
                    image.color = Color.gray;
                }
                else
                {
                    image.color = Color.black;
                }
                mazeBlock[i,j] = Instantiate(setBlockImg[i, j], maze.transform);
            }
        }
    }

    public bool[,] Movable()
    {
        bool[,] movable = new bool[width, height];
        for(int i = 0;i < width; i++)
        {
            for(int j = 0;j < height; j++)
            {
                movable[i, j] = block[i, j].dug;

            }
        }
        return movable;
    }
}
