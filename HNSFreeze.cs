using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;
using HNSFreeze;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text.Json;
using Vector = CounterStrikeSharp.API.Modules.Utils.Vector;

namespace CS2HNSFreeze;

[MinimumApiVersion(129)]
public class HNSFreeze : BasePlugin
{
    private const string Version = "0.0.2";
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
            /*
             * Instead of checking distance for each player, create sphere ent at decoy pos with a radius of x  and only get players within
             */
            //var sphere = Utilities.CreateEntityByName<CSoundAreaEntitySphere>("entitySphere");
            
         


            var decoyPosition = new Vector3(@event.X, @event.Y, @event.Z);

            // Decoy BEAM
            var decoyBeamOutlinePoints = CalculateCirclePoints(new Vector(@event.X, @event.Y, @event.Z), Config.FreezeRadius, 360);
            var decoyBeamInnerPoints = CalculateCirclePoints(new Vector(@event.X, @event.Y, @event.Z), Config.FreezeRadius - 25, 360);
            DrawLaserBetween(decoyBeamInnerPoints, decoyBeamOutlinePoints,Config.FreezeTime);


            var players = Utilities.GetPlayers().Where(x => x is { IsBot: true, Connected: PlayerConnectedState.PlayerConnected });

            foreach (var player in players)
            {
                if (player.PlayerPawn.Value != null && player.IsValid)
                {
                    var playerPosition = new Vector3(player.PlayerPawn.Value.AbsOrigin.X, player.PlayerPawn.Value.AbsOrigin.Y, player.PlayerPawn.Value.AbsOrigin.Z);
                    var dist = Vector3.Distance(playerPosition, decoyPosition);
                    Logger.LogInformation($"[DECOY] distance from {player.PlayerName}: {dist}");


                //freeze player TEAM in RADIUS of decoy if feature ENABLED
                if (player.TeamNum == Config.FreezeTeam && dist < Config.FreezeRadius && Config.DisableFreeze == 0)
                    {
                        //setting tick where player should be unfrozen.
                        playersFrozen[player] = (Server.TickCount + Config.FreezeTime*64);
                        Server.PrintToChatAll($" {MessagePrefix} {ChatColors.Green} {player.PlayerName} {ChatColors.White} is frozen for {ChatColors.Red} {Config.FreezeTime} seconds");
                        Logger.LogInformation($" [DECOY] {player.PlayerName} is frozen for {Config.FreezeTime} seconds");
                    }
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

    private static Vector[] CalculateCirclePoints(Vector center, float radius, int numberOfPoints)
    {
        Vector[] points = new Vector[numberOfPoints];

        for (int i = 0; i < numberOfPoints; i++)
        {
            float theta = 2.0f * (float)Math.PI * i / numberOfPoints;
            points[i] = center + new Vector(radius * (float)Math.Cos(theta), radius * (float)Math.Sin(theta), 0.0f);
        }

        return points;
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

