using UnityEngine;

namespace Genzan
{
    public class Node
    {
        public Vector2Int position = new(0, 0);
        public enum State { None , Open , Closed , Lock};
        public State state = State.None;
        public float gCost = 0;
        public float hCost = 0;
        public float FCost => gCost + hCost;
        public Node parentNode;
    }
}