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
                            grid[nextNode.x, nextNode.y].parentNode = baseNode; //�e�m�[�h�����݂̃m�[�h�ɐݒ�
                            grid[nextNode.x, nextNode.y].gCost = baseNode.gCost + 1; //�R�X�g�����݂̃m�[�h���瑝��
                            grid[nextNode.x, nextNode.y].hCost = HCostCal(nextNode, goal); //�\�z�R�X�g�̌v�Z
                            //���m�[�h���S�[���Ɠ����ʒu�ł���ΒT�����I������
                            if (grid[nextNode.x,nextNode.y].hCost == 0)
                            {
                                baseNode = grid[nextNode.x, nextNode.y];
                                goto finish;
                            }
                        }
                    }
                }
                baseNode.state = Node.State.Closed;


                //�ł��R�X�g�̒Ⴂ�m�[�h���I�[�v��
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
                //�I�[�v���ł���m�[�h���Ȃ���ΏI��
                if (!isOpenNode)
                {
                    goto undefind;
                }
            }

            //�T���������I������
            finish:
            //�e�m�[�h��H��ŏ��̃m�[�h�܂Ŗ߂�
            while (true)
            {
                //�ŏ��̃m�[�h�܂Ŗ߂�����A�ړ����������߂�
                if(baseNode.parentNode.position == start)
                {
                    int moveDirection = DirectionCal(baseNode.parentNode.position,baseNode.position);
                    return moveDirection;
                }
                baseNode = baseNode.parentNode;
            }
            //�T�����s���I������
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
