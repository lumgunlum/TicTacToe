using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// 下棋逻辑
namespace TacGame
{
    public class ChessLogicMain : MonoBehaviour
    {
        GameControl gameControl = GameControl.Instance;
        public Button[] buttons;
        public Sprite[] sprites;
        public Text titleText;

        public int[,] point = new int[3, 3]{
            {0, 0, 0},
            {0, 0, 0},
            {0, 0, 0},
        };

        // Start is called before the first frame update
        void Start()
        {
            ResetBoard();
            gameControl.StartGame();
        }

        void ResetBoard()
        {
            foreach (var b in buttons)
            {
                SetEmptyChess(b);
            }
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    point[i, j] = 0;
                }
            }
        }

        void StartAI()
        {
            AITurn();
            CheckWinner();
        }

        // 手动下棋
        public void ChessDown(Button b)
        {
            if (gameControl.playMode == PlayMode.Stop)
                return;
            if (gameControl.isAIRound)
                return;
            // 下过了
            if (b.image.color.a > 0.5f)
                return;
            var r = gameControl.inputRound % 2;
            SetChess(b, sprites[r]);
            var arr = b.name.Split(new char[1] { '_' });
            var i = int.Parse(arr[1]) - 1;
            var j = int.Parse(arr[2]) - 1;
            point[i, j] = r + 1;

            // 判断游戏胜利
            CheckWinner();
            gameControl.NextRoundAfterPlayer();
            StartAI();

        }

        // 电脑下棋
        void AITurn()
        {
            if (gameControl.playMode == PlayMode.Stop)
                return;
            if (!gameControl.isAIRound)
                return;
            var round = gameControl.inputRound % 2;
            var tmpI = -1;
            var tmpJ = -1;
            List<int> left = new List<int>();
            int leftCount = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (point[i, j] == 0)
                    {
                        // 如果是ai下这里
                        point[i, j] = round + 1;
                        var win = CheckPoint();
                        // 结束 
                        if (win == round + 1)
                        {
                            var index = i * 3 + j;
                            SetChess(buttons[index], sprites[round]);
                            return;
                        }

                        // 如果是对方下这里
                        point[i, j] = 2 - round;
                        // 对方赢了
                        win = CheckPoint();
                        if (win == 2 - round)
                        {
                            tmpI = i;
                            tmpJ = j;
                        }
                        point[i, j] = 0;
                        left.Add(i * 3 + j);
                    }
                }
            }

            if (tmpI == -1)
            {
                // 占据中心点
                if (left.Contains(4))
                {
                    tmpI = tmpJ = 1;
                }
                else
                {
                    var conner = left.Where(t => t == 0 || t == 2 || t == 6 || t == 8);
                    var count = conner.Count();
                    if (count > 0)
                    {
                        var p = Random.Range(0, count);
                        tmpI = conner.ToArray()[p] / 3;
                        tmpJ = conner.ToArray()[p] % 3;
                    }
                    else
                    {
                        var p = Random.Range(0, leftCount);
                        tmpI = left[p] / 3;
                        tmpJ = left[p] % 3;
                    }
                }
            }
            point[tmpI, tmpJ] = round + 1;
            var index1 = tmpI * 3 + tmpJ;
            SetChess(buttons[index1], sprites[round]);
            gameControl.NextRoundAfterAI();
            return;
        }

        // 检查获胜条件
        int CheckPoint()
        {
            // 常规行列
            for (int i = 0; i < 3; i++)
            {
                if (point[i, 0] != 0 && point[i, 0] == point[i, 1] && point[i, 0] == point[i, 2])
                    return point[i, 0];

                if (point[0, i] != 0 && point[0, i] == point[1, i] && point[0, i] == point[2, i])
                    return point[0, i];
            }
            // 斜
            if (point[0, 0] != 0 && point[0, 0] == point[1, 1] && point[0, 0] == point[2, 2])
                return point[0, 0];
            if (point[2, 0] != 0 && point[2, 0] == point[1, 1] && point[2, 0] == point[0, 2])
                return point[2, 0];
            // 没下完
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (point[i, j] == 0)
                        return 0;
                }
            }
            // 下完了
            return -1;
        }

        void CheckWinner()
        {
            var win = CheckPoint();
            // 继续游戏
            if (win == 0)
                return;
            // 平局
            if (win == -1)
            {
                titleText.text = "平局，请重新开始游戏";
                gameControl.EndGame();
                return;
            }
            if (gameControl.playMode == PlayMode.PlayerVsPlayer)
            {
                titleText.text = "玩家" + win + "获胜！请重新开始游戏";
                gameControl.EndGame();
                return;
            }
            if (gameControl.playMode == PlayMode.PlayerFirst)
            {
                var winName = win == 1 ? "玩家" : "电脑";
                titleText.text = winName + "获胜！请重新开始游戏";
                gameControl.EndGame();
                return;
            }

            if (gameControl.playMode == PlayMode.AiFirst)
            {
                var winName = win == 2 ? "玩家" : "电脑";
                titleText.text = winName + "获胜！请重新开始游戏";
                gameControl.EndGame();
                return;
            }

        }

        // 清除棋格
        public void SetEmptyChess(Button b)
        {
            b.image.sprite = null;
            b.image.color = new Color(0, 0, 0, 0.1f);
        }

        public void SetChess(Button b, Sprite s)
        {
            b.image.sprite = s;
            b.image.color = new Color(0, 0, 0, 1f);
        }

        // 下面都是按钮触发的逻辑，直接赋到button上了
        // 模式选择 
        public void PVP()
        {
            gameControl.SetGameMode(PlayMode.PlayerVsPlayer, false);
            titleText.text = "玩家对战模式";
            ResetBoard();
        }
        public void PVE()
        {
            gameControl.SetGameMode(PlayMode.PlayerFirst, false);
            titleText.text = "玩家先手模式";
            ResetBoard();
        }
        public void EVP()
        {
            gameControl.SetGameMode(PlayMode.AiFirst, true);
            titleText.text = "玩家后手模式";
            ResetBoard();
            StartAI();
        }

        // 重置
        public void ResetGame()
        {
            switch (gameControl.playMode)
            {
                case PlayMode.AiFirst:
                    EVP();
                    break;
                case PlayMode.PlayerFirst:
                    PVE();
                    break;
                case PlayMode.PlayerVsPlayer:
                    PVP();
                    break;
            }
        }

        public void QuitGame()
        {
            gameControl.QuitGame();
        }
    }
}