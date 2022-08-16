// config当初比较少，因此没有选择专门挑出来放
// 当然对下棋的先后不想要有magic number也可以都放在这
// 想法是比较少且显而易见的就不希望专门config记录
namespace TacGame
{
    public enum PlayMode
    {
        Stop = 0,
        PlayerVsPlayer = 1,
        PlayerFirst = 2,
        AiFirst = 3,
    }
}