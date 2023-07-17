using Microsoft.Xna.Framework;
using TShockAPI;

namespace AfkApi
{
    public class AfkApi
    {
        public Settings Settings => AfkPlugin.Settings;
        public List<TSPlayer> AfkPlayers { get; set; } = new();
        public System.Timers.Timer AfkTimer { get; set; }
        public void SetAfkTicks(TSPlayer player, int ticks) => player.SetData("afk_ticks", ticks);
        public int GetAfkTicks(TSPlayer player) => player.GetData<int>("afk_ticks");
        public void IncrementAfkTick(TSPlayer player)
        {
            var curr = GetAfkTicks(player);
            SetAfkTicks(player, curr++);
        }
        public Vector2 GetLastNetPos(TSPlayer player) => player.GetData<Vector2>("afk_lastPos");
        public void SetLastNetPos(TSPlayer player) => player.SetData("afk_lastPos", player.LastNetPosition);
        public void AddToAfk(TSPlayer player) => AfkPlayers.Add(player);
        public TSPlayer? RetrieveIfAfk(TSPlayer player) => AfkPlayers.FirstOrDefault(x => x == player, null);

        public void RemoveFromAfk(TSPlayer player)
        {
            SetAfkTicks(player, 0);
            AfkPlayers.RemoveAll(x=>x==player);
        }
    }
}