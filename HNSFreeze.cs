using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Logging;
using System.Numerics;

namespace CS2HNSFreeze;

[MinimumApiVersion(129)]
public class HNSFreeze : BasePlugin
{
    private const string Version = "0.0.1";
    public override string ModuleName => "HNS Freeze Plugin";
    public override string ModuleVersion => Version;
    public override string ModuleAuthor => "Lucas Hunter";
    public override string ModuleDescription => "Decoy Freeze-CT & NoFlash-T for HNS Gamemode .";
    
    public static readonly string LogPrefix = $"[HNS Freeze {Version}] ";
    public static readonly string MessagePrefix = $"[{ChatColors.Blue}HNS Freeze{ChatColors.White}] ";


    //freeze time in ticks
    private const int FREEZE_TIME = 225;
    private const int FREEZE_RADIUS = 200;

    private Dictionary<CCSPlayerController, int> playersFrozen = new Dictionary<CCSPlayerController, int>();


    public override void Load(bool hotReload)
    {
        Console.WriteLine($"{ModuleName} loaded!");

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
    public HookResult OnPlayerDecoy(EventDecoyStarted @event, GameEventInfo info)
    {
       
            var decoyPosition = new Vector3(@event.X, @event.Y, @event.Z);
            var players = Utilities.GetPlayers().Where(x => x is { IsBot: false, Connected: PlayerConnectedState.PlayerConnected });

            foreach (var player in players)
            {

                if (player.PlayerPawn.Value != null && player.IsValid)
                {

                    var playerPosition = new Vector3(player.PlayerPawn.Value.AbsOrigin.X, player.PlayerPawn.Value.AbsOrigin.Y, player.PlayerPawn.Value.AbsOrigin.Z);
                    var dist = Vector3.Distance(playerPosition, decoyPosition);

                    Logger.LogInformation($"[DECOY] distance from {player.PlayerName}: {dist}");

                    //freeze if CT and in radius of decoy

                    if (player.TeamNum == 3 && dist < FREEZE_RADIUS)
                    {
                        //setting tick where player should be unfrozen.
                        playersFrozen[player] = (Server.TickCount + FREEZE_TIME);
                        Server.PrintToChatAll($" {MessagePrefix} {ChatColors.Green} {player.PlayerName} {ChatColors.White} is frozen for {ChatColors.Red} 5 seconds");
                        Logger.LogInformation($" [DECOY] {player.PlayerName} is frozen for 5 seconds");
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
            if (player.TeamNum == 2 )
            {
               player.PlayerPawn.Value.BlindUntilTime = Server.CurrentTime;
            }
        }

        return HookResult.Continue;
    }



























}

