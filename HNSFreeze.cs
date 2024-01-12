using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;
using HNSFreeze;
using Microsoft.Extensions.Logging;
using System.Drawing;
using System.Numerics;
using System.Text.Json;
using Vector = CounterStrikeSharp.API.Modules.Utils.Vector;

namespace CS2HNSFreeze;

[MinimumApiVersion(129)]
public class HNSFreeze : BasePlugin
{
    private const string Version = "0.0.3";
    public override string ModuleName => "HNS Freeze Plugin";
    public override string ModuleVersion => Version;
    public override string ModuleAuthor => "lhunter3";
    public override string ModuleDescription => "Decoy Freeze-CT & NoFlash-T for HNS Gamemode .";
    
    public static readonly string LogPrefix = $"[HNS Freeze {Version}] ";
    public static readonly string MessagePrefix = $"[{ChatColors.Blue}Freeze{ChatColors.White}] ";


    private Dictionary<CCSPlayerController, int> playersFrozen = new Dictionary<CCSPlayerController, int>();
    public Config? Config { get; set; }


    private Config LoadConfig()
    {
        var configPath = Path.Combine(ModuleDirectory, "config.json");

        if (!File.Exists(configPath)) return null;

        var config = JsonSerializer.Deserialize<Config>(File.ReadAllText(configPath))!;

        return config;
    }

    public override void Load(bool hotReload)
    {
        Console.WriteLine($"{ModuleName} loaded!");

        Config = LoadConfig();
        Logger.LogInformation($"{ModuleName} Config Loaded");
        RegisterListener<Listeners.OnTick>((() =>
        {
            foreach (KeyValuePair<CCSPlayerController, int> info in playersFrozen)
            {
                if (info.Value > Server.TickCount)
                {
                    if(info.Key.PlayerPawn.Value != null && info.Key.PlayerPawn.IsValid && info.Key.PawnIsAlive)
                    {

                        //freeze stop movement
                        info.Key.PlayerPawn.Value.VelocityModifier = -999999;
                        info.Key.PlayerPawn.Value.Friction = 999999;
                        info.Key.PlayerPawn.Value.MoveType = MoveType_t.MOVETYPE_CUSTOM;
                        info.Key.PlayerPawn.Value.MoveDoneTime = Server.CurrentTime;
                       
                        //stop damage
                        info.Key.PlayerPawn.Value.TakesDamage = false;

                        //Logger.LogInformation($"{LogPrefix} {info.Key.PlayerName} is frozen {playersFrozen[info.Key]}/{Server.TickCount}");
                    }
                }
                else
                {
                    if (info.Key.PlayerPawn.Value != null)
                    {
                        //remove from freeze list.
                        playersFrozen.Remove(info.Key);

                        //reset vals
                        info.Key.PlayerPawn.Value.MoveType = MoveType_t.MOVETYPE_WALK;
                        info.Key.PlayerPawn.Value.Friction = 1;
                        info.Key.PlayerPawn.Value.VelocityModifier = 1;
                        info.Key.PlayerPawn.Value.TakesDamage = true;

                    }
                }
            }
        }));
       
    }



    [GameEventHandler]
    public HookResult OnDecoyStarted(EventDecoyStarted @event, GameEventInfo info)
    {

        if(Config is not null)
        {
            // sphere ent
            SphereEntity sphereEntity = new SphereEntity(new Vector(@event.X, @event.Y, @event.Z), Config.FreezeRadius);
            DrawLaserBetween(sphereEntity.circleInnerPoints, sphereEntity.circleOutterPoints, Config.FreezeTime);

            var players = Utilities.GetPlayers().Where(x => x is { Connected: PlayerConnectedState.PlayerConnected });

            foreach (var player in players)
            {
                if (player.IsValid && player.TeamNum == Config.FreezeTeam && Config.DisableFreeze == 0 && player.PlayerPawn.Value != null && sphereEntity.colidesWithPlayer(player.PlayerPawn.Value.AbsOrigin))
                {
                    //setting tick where player should be unfrozen.
                    playersFrozen[player] = (Server.TickCount + Config.FreezeTime*64);
                    Server.PrintToChatAll($" {MessagePrefix} {ChatColors.Green} {player.PlayerName} {ChatColors.White} is frozen for {ChatColors.Red} {Config.FreezeTime} seconds");
                    Logger.LogInformation($" [DECOY] {player.PlayerName} is frozen for {Config.FreezeTime} seconds");
                    
                }
            }
        }

        return HookResult.Continue;
    }


    [GameEventHandler]
    public HookResult OnPlayerFlash(EventPlayerBlind @event, GameEventInfo info)
    {
        var player = @event.Userid;
        

        if (player.PlayerPawn.Value != null && player.IsValid)
        {
            //dont flash if player is T side.
            if (player.TeamNum == Config.NoFlashTeam )
            {
               player.PlayerPawn.Value.BlindUntilTime = Server.CurrentTime;
            }
        }

        return HookResult.Continue;
    }


    [GameEventHandler]
    public HookResult OnSmokeStarted(EventSmokegrenadeDetonate @event, GameEventInfo info)
    {
       // could look for a way to remove smoke for team but its not rly needed for hns so :/
        
       if(Config is not null)
       {
            if(Config.DisableSmoke == 1) 
            {
                var smoke = Utilities.GetEntityFromIndex<CEntityInstance>(@event.Entityid);
                smoke.Remove();
            }
       }
        return HookResult.Continue;
    }

    public void OnPlayerAttacked(CCSPlayerController player)
    {

    }
    

    

   

    private void DrawLaserBetween(Vector[] startPos, Vector[] endPos, float duration)
    {

        for(int i = 0; i < endPos.Length; i++)
        {

            CBeam beam = Utilities.CreateEntityByName<CBeam>("beam");
            if (beam == null)
            {
                return;
            }

            beam.Render = Color.Blue;
            beam.Width = 3.0f;

            beam.Teleport(startPos[i], new QAngle(0), new Vector(0,0,0));
            beam.EndPos.X = endPos[i].X;
            beam.EndPos.Y = endPos[i].Y;
            beam.EndPos.Z = endPos[i].Z;
            beam.DispatchSpawn();

            AddTimer(duration, () => { beam.Remove(); });

        }
        
        
    }


    
}

