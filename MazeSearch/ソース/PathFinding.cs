using System.Collections.Generic;
using UnityEngine;
public class PathFinding
{
    Node[,] grid;

    public List<int> FindPath(Vector2Int start, Vector2Int goal, bool[,] isMovable,int width,int height)
    {
        grid = new Node[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
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
        grid[start.x, start.y].hCost = HCostCal(start, goal);

        Node baseNode = grid[start.x, start.y];

        while (true)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2Int nextNode = baseNode.position;
                switch (i)
                {
                    case CNum.UP:
                        nextNode = new(nextNode.x, nextNode.y - 1);
                        break;
                    case CNum.DOWN:
                        nextNode = new(nextNode.x, nextNode.y + 1);
                        break;
                    case CNum.LEFT:
                        nextNode = new(nextNode.x - 1, nextNode.y);
                        break;
                    case CNum.RIGHT:
                        nextNode = new(nextNode.x + 1, nextNode.y);
                        break;
                    default:
                        break;
                }
                if (nextNode.x >= 0 && nextNode.x < width && nextNode.y >= 0 && nextNode.y < height)
                {
                    if (grid[nextNode.x, nextNode.y].state == Node.State.None)
                    {
                        grid[nextNode.x, nextNode.y].state = Node.State.Open;
                        grid[nextNode.x, nextNode.y].parentNode = baseNode;
                        grid[nextNode.x, nextNode.y].gCost = baseNode.gCost + 1;
                        grid[nextNode.x, nextNode.y].hCost = HCostCal(nextNode, goal);
                        if (grid[nextNode.x, nextNode.y].hCost == 0)
                        {
                            baseNode = grid[nextNode.x, nextNode.y];
                            goto finish;
                        }
                    }
                }
            }
            baseNode.state = Node.State.Closed;

            Node lowCostNode = null;
            bool isOpenNode = false;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
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
            if (!isOpenNode)
            {
                goto undefind;
            }
        }

    finish:
        List<int> moveDirection = new();
        while (true)
        {
            moveDirection.Add(DirectionCal(baseNode.parentNode.position, baseNode.position));
            if (baseNode.parentNode.position == start)
            {
                moveDirection.Reverse();
                return moveDirection;
            }
            baseNode = baseNode.parentNode;
        }
    undefind:
        return null;
    }

    int HCostCal(Vector2Int start, Vector2Int goal)
    {
        return Mathf.Abs(start.x - goal.x) + Mathf.Abs(start.y - goal.y);
    }

    int DirectionCal(Vector2Int basePos, Vector2Int nextPos)
    {
        if (nextPos.y < basePos.y)
        {
            return CNum.UP;
        }
        else if (nextPos.y > basePos.y)
        {
            return CNum.DOWN;
        }
        else if (nextPos.x < basePos.x)
        {
            return CNum.LEFT;
        }
        else if (nextPos.x > basePos.x)
        {
            return CNum.RIGHT;
        }
        return -1;
    }
}
