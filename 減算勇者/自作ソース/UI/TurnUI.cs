using UnityEngine;
using UnityEngine.UI;

namespace Genzan 
{
    public class TurnUI : MonoBehaviour
    {
        [SerializeField]
        Sprite[] turnImage;

        Image image;

        private void Start()
        {
            image = GetComponent<Image>();
        }

        public void DisplayTurnImage(GameManager.TurnState turnState)
        {
            switch(turnState)
            {
                case GameManager.TurnState.PlayerTurn:
                    image.sprite = turnImage[0];
                    break;
                case GameManager.TurnState.EnemyTurn:
                    image.sprite = turnImage[1];
                    break;
                case GameManager.TurnState.BossTurn:
                    image.sprite = turnImage[1];
                    break;
                default: break;
            }
        }
    }
}

