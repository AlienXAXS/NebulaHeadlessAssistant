using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NebulaAPI;
using NebulaHeadlessAssistant.HiveRestorer.Data;
using UnityEngine;

namespace NebulaHeadlessAssistant.HiveRestorer
{
    internal class HiveRestorerManager
    {
        private static readonly HiveRestorerManager _instance;
        public static HiveRestorerManager Instance = _instance ??= new HiveRestorerManager();

        private readonly Dictionary<int,string> _hiveDataRecorderValues = new();
        private bool IsActive;
        private int PlayerCount;

        private const string FileName = "hiveData.json";

        public void Init()
        {
            IsActive = true;
        }

        public void ReadHiveDataFromGame()
        {
            ReadHiveDataFromInput(GameMain.spaceSector.dfHives);
        }

        public void ReadHiveDataFromInput(EnemyDFHiveSystem[] hiveData)
        {
            _hiveDataRecorderValues.Clear();

            foreach (var hive in hiveData)
            {
                if (hive == null)
                    continue;

                try
                {
                    if (!hive.realized) continue;

                    var hiveId = hive.hiveAstroId;
                    var hiveThreat = hive.evolve.threat ;
                    var hiveLevel = hive.evolve.level;
                    
                    string hiveLevelAndThreat = $"{hiveLevel}|{hiveThreat}";
                    Log.LogDebug($"Hive {hiveId} dat:{hiveLevelAndThreat}");
                    _hiveDataRecorderValues.Add(hiveId, hiveLevelAndThreat);
                }
                catch (Exception ex)
                {
                    Log.LogError(ex);
                }
            }
        }

        private void WriteHiveDataFromInput(Dictionary<int, string> input)
        {
            var hiveData = GameMain.spaceSector.dfHives;

            foreach (var hive in hiveData)
            {
                if (hive == null)
                    continue;

                try
                {
                    if (input.TryGetValue(hive.hiveAstroId, out var hiveDataValue))
                    {
                        var dataSplit = hiveDataValue.Split('|');
                        if ( dataSplit.Length != 2 ) continue;

                        if (int.TryParse(dataSplit[0], out var hiveLevel))
                        {
                            if (int.TryParse(dataSplit[1], out var hiveThreat))
                            {
                                Log.LogInfo($"Updating Hive {hive.hiveAstroId} to level: {hiveLevel} with {hiveThreat} threat");
                                hive.evolve.level = hiveLevel;
                                hive.evolve.threat = hiveThreat;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.LogError(ex);
                }
            }
        }

        public void SaveGameState()
        {
            Log.LogInfo($"[HiveDataRecorder] Saving {_hiveDataRecorderValues.Count} Hive Entries");

            try
            {
                ReadHiveDataFromGame();
                var hiveData = $"{GameMain.data.history.combatSettings.aggressiveLevel}\n{JsonHelper.Serialize(_hiveDataRecorderValues)}";
                System.IO.File.WriteAllText(FileName, hiveData);
                Log.LogInfo("[HiveDataRecorder] Initial game state saved!");
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                IsActive = true;
            }
        }

        public void LoadGameState()
        {
            try
            {
                if (!System.IO.File.Exists(FileName)) return;

                var fileContents = System.IO.File.ReadAllText(FileName);
                var fileContentsSplit = fileContents.Split('\n');

                if (fileContentsSplit.Length != 2)
                {
                    Log.LogError($"Hive data file invalid, removing it and generating a new one");
                    System.IO.File.Delete(FileName);
                    SaveGameState();
                    return;
                }

                switch (fileContentsSplit[0])
                {
                    case "Dummy":
                        SetHiveAgressiveness(EAggressiveLevel.Dummy);
                        break;
                    case "Passive":
                        SetHiveAgressiveness(EAggressiveLevel.Passive);
                        break;
                    case "Torpid":
                        SetHiveAgressiveness(EAggressiveLevel.Torpid);
                        break;
                    case "Normal":
                        SetHiveAgressiveness(EAggressiveLevel.Normal);
                        break;
                    case "Sharp":
                        SetHiveAgressiveness(EAggressiveLevel.Sharp);
                        break;
                    case "Rampage":
                        SetHiveAgressiveness(EAggressiveLevel.Rampage);
                        break;
                    default:
                        Log.LogError($"I don't know what aggressiveness {fileContentsSplit[0]} is, so i cannot set it!");
                        break;
                }

                if (JsonHelper.Deserialize(fileContentsSplit[1]) is Dictionary<int, string> jObject)
                {
                    Log.LogInfo($"Applying Hive data from {jObject.Count} saved entries");
                    WriteHiveDataFromInput(jObject);
                }
                else
                {
                    Log.LogError("Unable to deserialise hive data from file");
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex);
                IsActive = true;
            }
        }

        private void SetHiveAgressiveness(EAggressiveLevel level)
        {
            float aggressiveness = 0f;
            EAggressiveLevel previousLevel = GameMain.data.history.combatSettings.aggressiveLevel;
            switch (level)
            {
                case EAggressiveLevel.Dummy:
                    aggressiveness = -1f;
                    break;

                case EAggressiveLevel.Passive:
                    aggressiveness = 0f;
                    break;

                case EAggressiveLevel.Torpid:
                    aggressiveness = 0.5f;
                    break;

                case EAggressiveLevel.Normal:
                    aggressiveness = 1f;
                    break;

                case EAggressiveLevel.Sharp:
                    aggressiveness = 2f;
                    break;

                case EAggressiveLevel.Rampage:
                    aggressiveness = 3f;
                    break;
            }

            Log.LogInfo($"Setting aggressiveness from {previousLevel} to {level} with a score of {aggressiveness}");

            GameMain.data.history.combatSettings.aggressiveness = aggressiveness;
            GameMain.spaceSector.OnDFAggressivenessChanged(previousLevel, level);
        }

        public void PlayerJoined()
        {
            if (!IsActive) return;

            // Check if PlayerCount was zero, if so we need to restore the threat.
            if (PlayerCount == 0)
            {
                Log.LogInfo("Server is populated, enabling Dark Fog");
                LoadGameState();
            }

            ++PlayerCount;
        }

        public void PlayerLeft()
        {
            if (!IsActive) return;

            --PlayerCount;

            if (PlayerCount == 0)
            {
                SaveGameState();
                //Log.LogInfo("Server is empty, setting Dark Fog to be 'Dummy'");
                //SetHiveAgressiveness(EAggressiveLevel.Dummy);
            }
        }

        public void FirstStart()
        {
            if (!IsActive) return;

            // We have a file, we should load it to ensure the correct state is in the game, as it
            // is possible the server closed without warning.
            if (System.IO.File.Exists(FileName))
            {
                LoadGameState();
            }
            else
            {
                //If we do not have an initial gamestate, we must save it first in order to not lose it.
                SaveGameState();
            }

            // We have no players, set ourselves back to Dummy
            Log.LogInfo("First Load, Setting Hive to Dummy as the server is empty.");
            SetHiveAgressiveness(EAggressiveLevel.Dummy);
        }
    }
}
