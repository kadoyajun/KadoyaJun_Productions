using UnityEngine;

namespace Genzan
{
    public class TreasureBox : MonoBehaviour
    {
        [SerializeField]
        int[] changeLevelValue = null;

        [SerializeField]
        Mesh mesh;

        bool isOpen = false;

        public void BoxOpen()
        {
            if (!isOpen)
            {
                //宝箱のモデルを開いたものに変更
                isOpen = true;
                MeshFilter filter = GetComponent<MeshFilter>();
                filter.mesh = mesh;

                //付与レベルを出力
                int randomNum = Random.Range(0, changeLevelValue.Length);
                GameManager gameManager = transform.root.GetComponent<GameManager>();
                gameManager.DisplayMessage(LogText.OpenTreasureBox);
                AudioManager.Instance.PlaySE("BoxOpenSE");
                if (changeLevelValue[randomNum] == 0)
                {
                    gameManager.DisplayMessage(LogText.EmptyTreasureBox);
                }
                else
                {
                    gameManager.LevelUpDown(changeLevelValue[randomNum], false);
                }
            }
        }
    }
}

