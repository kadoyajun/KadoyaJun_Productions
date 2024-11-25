using UnityEngine;

namespace Genzan 
{
    public class PathFinding
    {
        Node[,] grid;

        public int FindPath(Vector2Int start, Vector2Int goal, bool[,] isMovable)
        {
            grid = new Node[CNum.ROW, CNum.COLUMN];
            for (int i = 0; i < CNum.ROW; i++)
            {
                for (int j = 0; j < CNum.COLUMN; j++)
                {
                    grid[i, j] = new Node();
                    grid[i, j].position = new Vector2Int(i, j);
                    if (isMovable[i, j] == false)
                    {
                        grid[i, j].state = Node.State.Lock;
                    }
                }
            }

            grid[start.x, start.y].state = Node.State.Open;
            grid[start.x,start.y].hCost = HCostCal(start, goal);

            Node baseNode = grid[start.x, start.y];

            while (true)
            {
                for (int i = 0; i < CNum.DIRECTIONCOUNT; i++)
                {
                    Vector2Int nextNode = baseNode.position;
                    switch (i)
                    {
                        case CNum.UP:
                            nextNode.y++;
                            break;
                        case CNum.DOWN:
                            nextNode.y--;
                            break;
                        case CNum.LEFT:
                            nextNode.x--;
                            break;
                        case CNum.RIGHT:
                            nextNode.x++;
                            break;
                        default:
                            break;
                    }
                    if (nextNode.x >= 0 && nextNode.x < CNum.ROW && nextNode.y >= 0 && nextNode.y < CNum.COLUMN)
                    {
                        if (grid[nextNode.x, nextNode.y].state == Node.State.None)
                        {
                            grid[nextNode.x, nextNode.y].state = Node.State.Open;
                            grid[nextNode.x, nextNode.y].parentNode = baseNode; //親ノードを現在のノードに設定
                            grid[nextNode.x, nextNode.y].gCost = baseNode.gCost + 1; //コストを現在のノードから増加
                            grid[nextNode.x, nextNode.y].hCost = HCostCal(nextNode, goal); //予想コストの計算
                            //次ノードがゴールと同じ位置であれば探索を終了する
                            if (grid[nextNode.x,nextNode.y].hCost == 0)
                            {
                                baseNode = grid[nextNode.x, nextNode.y];
                                goto finish;
                            }
                        }
                    }
                }
                baseNode.state = Node.State.Closed;


                //最もコストの低いノードをオープン
                Node lowCostNode = null;
                bool isOpenNode = false;
                for (int i = 0; i < CNum.ROW; i++)
                {
                    for (int j = 0; j < CNum.COLUMN; j++)
                    {
                        if (grid[i, j].state == Node.State.Open)
                        {
                            isOpenNode = true;
                            if (lowCostNode == null)
                            {
                                lowCostNode = grid[i, j];
                            }
                            else if (lowCostNode.FCost > grid[i, j].FCost)
                            {
                                lowCostNode = grid[i, j];
                            }
                        }
                    }
                }
                baseNode = lowCostNode;
                //オープンできるノードがなければ終了
                if (!isOpenNode)
                {
                    goto undefind;
                }
            }

            //探索成功時終了処理
            finish:
            //親ノードを辿り最初のノードまで戻る
            while (true)
            {
                //最初のノードまで戻ったら、移動方向を求める
                if(baseNode.parentNode.position == start)
                {
                    int moveDirection = DirectionCal(baseNode.parentNode.position,baseNode.position);
                    return moveDirection;
                }
                baseNode = baseNode.parentNode;
            }
            //探索失敗時終了処理
            undefind:
            return -1;
        }

        int HCostCal(Vector2Int start, Vector2Int goal)
        {
            return Mathf.Abs(start.x - goal.x) + Mathf.Abs(start.y - goal.y);
        }

        int DirectionCal(Vector2Int basePos, Vector2Int nextPos)
        {
            if(nextPos.y > basePos.y)
            {
                return CNum.UP;
            }
            else if(nextPos.y < basePos.y)
            {
                return CNum.DOWN;
            }
            else if(nextPos.x < basePos.x)
            {
                return CNum.LEFT;
            }
            else if(nextPos.x > basePos.x) 
            { 
                return CNum.RIGHT;
            }
            return -1;
        }
    }
}
