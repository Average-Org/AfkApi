using System.Timers;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace AfkApi
{
    [ApiVersion(2, 1)]
    public class AfkPlugin : TerrariaPlugin
    {
        public AfkPlugin(Main game) : base(game) { }
        public static AfkPlugin Instance { get; private set; }
        public static AfkApi Api { get; set; } = new();
        public static Settings Settings { get; set; } = new();
        public override string Author => "Average";
        public override string Description => "Tracking whether or not a player is AFK, and providing an API for it.";
        public override string Name => "AfkApi";
        public override Version Version => new(1, 0);

        public override void Initialize()
        {
            Instance = this;
            Settings = Settings.Read(Path.Combine(TShock.SavePath, "AfkApi.json"));

            #region Timer initialization
            Api.AfkTimer = new(1000)
            {
                AutoReset = true
            };
            Api.AfkTimer.Elapsed += (_, x)
                => Update(x);
            Api.AfkTimer.Start();
            #endregion
        }

        private void Update(ElapsedEventArgs _)
        {
            foreach(var plr in TShock.Players)
            {
                if (plr is null || !plr.Active)
                    continue;

                var lastNetPos = Api.GetLastNetPos(plr);
                var ticks = Api.GetAfkTicks(plr);

                if(Api.RetrieveIfAfk(plr) is not null)
                {
                    if (plr.LastNetPosition != lastNetPos)
                    {
                        Api.RemoveFromAfk(plr);
                        TSPlayer.All.SendInfoMessage($"{plr.Name} is no longer AFK!");
                        Api.SetLastNetPos(plr);
                        continue;
                    }
                    else
                    {
                        Api.IncrementAfkTick(plr);
                        if (Settings.KickForAFK)
                        {
                            if (ticks < Settings.KickThreshold)
                                continue;
                            else
                                plr.Kick("Kicked for being AFK for too long! (over 15 minutes)", false, false);
                                continue;
                        }
                    }
                }

                if(lastNetPos == plr.LastNetPosition)
                {
                    Api.IncrementAfkTick(plr);
                    if(ticks > 120)
                    {
                        Api.AddToAfk(plr);
                        TSPlayer.All.SendInfoMessage($"{plr.Name} is now AFK!");
                        continue;
                    }
                }
                else
                {
                    Api.RemoveFromAfk(plr);
                    TSPlayer.All.SendInfoMessage($"{plr.Name} is no longer AFK!");
                    Api.SetLastNetPos(plr);
                    continue;
                }


            }
        }
    }

}
