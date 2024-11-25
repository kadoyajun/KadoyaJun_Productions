using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Navigation : MonoBehaviour
{
    MakeMaze mM;
    int moveCount = 0;

    [SerializeField]
    TextMeshProUGUI moveCountText;

    [SerializeField]
    Toggle smooth;

    Coroutine coroutine;

    public void StartNavi()
    {
        StopNavi();
        mM = transform.root.GetComponent<MakeMaze>();
        moveCount = 0;
        coroutine = StartCoroutine(MoveNavi());
    }

    public IEnumerator MoveNavi()
    {
        int firstColor = Random.Range(0, 100);
        mM.mazeBlock[mM.startBlock.position.x, mM.startBlock.position.y].GetComponent<Image>().color = Color.HSVToRGB((float)(moveCount + firstColor) / 100 % 1, 1, 1);
        Vector2Int position = mM.startBlock.position;
        PathFinding pathFinding = new();
        List<int> moveDir = pathFinding.FindPath(mM.startBlock.position, mM.goalBlock.position, mM.Movable(),mM.Width,mM.Height);
        for (int i = 0; i < moveDir.Count; i++)
        {
            switch (moveDir[i])
            {
                case 0:
                    position.y -= 1;
                    break;
                case 1:
                    position.y += 1;
                    break;
                case 2:
                    position.x -= 1;
                    break;
                case 3:
                    position.x += 1;
                    break;
                default:
                    break;
            }
            moveCount++;
            mM.mazeBlock[position.x, position.y].GetComponent<Image>().color = Color.HSVToRGB((float)(moveCount + firstColor) / 100 % 1, 1, 1);
            if (smooth.isOn)
            {
                yield return null;
            }
        } 
    }

    public void StopNavi()
    {
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
        }
    }

    private void Update()
    {
        moveCountText.text = "Move:" + moveCount;
    }
}
