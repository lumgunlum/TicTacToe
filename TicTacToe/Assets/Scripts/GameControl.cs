using UnityEngine;

// game manager 单例
// 主要控制模式和游戏进程
namespace TacGame
{
    public class GameControl : MonoBehaviour
    {
        private static GameControl instance;
        public static GameControl Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameControl();
                }
                return instance;
            }
        }
        // 模式
        public PlayMode playMode = PlayMode.Stop;
        // 轮次
        public int inputRound = 0;
        public bool isAIRound = false;

        public void StartGame()
        {
            playMode = PlayMode.Stop;
            inputRound = 0;
        }

        public void EndGame()
        {
            playMode = PlayMode.Stop;
            inputRound = 0;
        }

        // 设置模式
        public void SetGameMode(PlayMode mode, bool ai)
        {
            inputRound = 0;
            playMode = mode;
            isAIRound = ai;
        }

        public void NextRoundAfterAI()
        {
            if (playMode == PlayMode.Stop)
                return;
            inputRound += 1;
            isAIRound = false;
            return;
        }
        public void NextRoundAfterPlayer()
        {
            if (playMode == PlayMode.Stop)
                return;
            inputRound += 1;
            isAIRound = playMode != PlayMode.PlayerVsPlayer;
            return;
        }



        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }

    }
}