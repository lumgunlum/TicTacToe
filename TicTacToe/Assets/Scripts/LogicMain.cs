using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Mode
{
    Stop = 0,
    PlayerVsPlayer = 1,
    PlayerFirst = 2,
    AiFirst = 3,
}

public class LogicMain : MonoBehaviour
{
    Mode playMode = Mode.Stop;
    public Button[] buttons;
    public Sprite[] sprites;
    int putChessRound = 0;
    bool isAIRound = false;
    public Text titleText;

    public int[,] point = new int[3, 3]{
        {0, 0, 0},
        {0, 0, 0},
        {0, 0, 0},
    };

    public List<string> indexs = new List<string>(){
        "1_1", "1_2", "1_3",
        "2_1", "2_2", "2_3",
        "3_1", "3_2", "3_3",
    };

    // Start is called before the first frame update
    void Start()
    {
        ResetBoard();
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
        putChessRound = 0;
    }

    // 手动下棋
    public void ChessDown(Button b)
    {
        if (playMode == Mode.Stop)
            return;
        if (isAIRound)
            return;
        // 下过了
        if (b.image.color.a > 0.5f)
            return;
        var r = putChessRound % 2;
        SetChess(b, sprites[r]);
        putChessRound += 1;
        var arr = b.name.Split(new char[1] { '_' });
        var i = int.Parse(arr[1]) - 1;
        var j = int.Parse(arr[2]) - 1;
        point[i, j] = r + 1;

        // 判断游戏胜利
        CheckWinner();

        if (playMode == Mode.PlayerFirst || playMode == Mode.AiFirst)
        {
            StartAI();
        }
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

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (point[i, j] == 0)
                    return 0;
            }

        }

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
            playMode = Mode.Stop;
            return;
        }
        if (playMode == Mode.PlayerVsPlayer)
        {
            titleText.text = "玩家" + win + "获胜！请重新开始游戏";
            playMode = Mode.Stop;
            return;
        }
        if (playMode == Mode.PlayerFirst)
        {
            var winName = win == 1 ? "玩家" : "电脑";
            titleText.text = winName + "获胜！请重新开始游戏";
            playMode = Mode.Stop;
            return;
        }

        if (playMode == Mode.AiFirst)
        {
            var winName = win == 2 ? "玩家" : "电脑";
            titleText.text = winName + "获胜！请重新开始游戏";
            playMode = Mode.Stop;
            return;
        }

    }

    void StartAI()
    {
        isAIRound = true;
        AITurn();
        CheckWinner();
    }

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

    public void PVP()
    {
        playMode = Mode.PlayerVsPlayer;
        titleText.text = "玩家对战模式";
        ResetBoard();
    }
    public void PVE()
    {
        playMode = Mode.PlayerFirst;
        titleText.text = "玩家先手模式";
        isAIRound = false;
        ResetBoard();
    }
    public void EVP()
    {
        playMode = Mode.AiFirst;
        titleText.text = "玩家后手模式";
        ResetBoard();
        StartAI();
    }

    public void ResetGame()
    {
        switch (playMode)
        {
            case Mode.AiFirst:
                EVP();
                break;
            case Mode.PlayerFirst:
                PVE();
                break;
            case Mode.PlayerVsPlayer:
                PVP();
                break;
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void AITurn()
    {
        if (playMode == Mode.Stop)
            return;
        if (!isAIRound)
            return;
        var round = putChessRound % 2;
        var tmpI = -1;
        var tmpJ = -1;
        int[] left = new int[9];
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
                        var name = (i + 1) + "_" + (j + 1);
                        var index = indexs.IndexOf(name);
                        SetChess(buttons[index], sprites[round]);
                        putChessRound += 1;
                        isAIRound = false;
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
                    left[leftCount++] = i * 3 + j;
                }
            }
        }

        if (tmpI == -1)
        {
            int p = Random.Range(0, leftCount);
            tmpI = left[p] / 3;
            tmpJ = left[p] % 3;
        }

        point[tmpI, tmpJ] = round + 1;
        var name1 = (tmpI + 1) + "_" + (tmpJ + 1);
        var index1 = indexs.IndexOf(name1);
        SetChess(buttons[index1], sprites[round]);
        putChessRound += 1;
        isAIRound = false;
        return;

    }

}
