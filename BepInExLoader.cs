using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace RF5.HisaCat.MonsterDropTablePatcher
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    internal class BepInExLoader : BepInEx.IL2CPP.BasePlugin
    {
        public const string
            MODNAME = "MonsterDropTablePatcher",
            AUTHOR = "HisaCat",
            GUID = "RF5." + AUTHOR + "." + MODNAME,
            VERSION = "1.0.0";

        public static BepInEx.Logging.ManualLogSource log;

        public static ConfigEntry<bool> bDoDump;
        public static ConfigEntry<bool> bDoPatch;

        public BepInExLoader()
        {
            log = Log;

            bDoDump = Config.Bind("Options", "DoDump", false,
                new ConfigDescription(
                    "Dump enemy drop tables to json\r\n" +
                    "the json files will saved at \'~~~\'"));

            bDoPatch = Config.Bind("Options", "DoPatch", false,
                new ConfigDescription(
                    "Patch enemy drop tables from json\r\n" +
                    "Patch json filed must be placed at \'~~~\'"));
        }

        public override void Load()
        {
            try
            {
                Harmony.CreateAndPatchAll(typeof(MonsterDropItemDataTableHooker));
            }
            catch
            {
                log.LogError("[Lib.InputHelper] Harmony - FAILED to Apply Patch's!");
            }
        }

        [HarmonyPatch]
        public class MonsterDropItemDataTableHooker
        {
            [HarmonyPatch(typeof(MonsterDropItemDataTable), nameof(MonsterDropItemDataTable.LoadData))]
            [HarmonyPostfix]
            public static void LoadDataPostfix(MonsterDropItemDataTable __instance)
            {
                if (bDoDump.Value)
                {
                    try
                    {
                        log.LogMessage($"[MonsterDropTablePatcher] Start dump monster drop table...");
                        Dumper.DoDump(__instance);
                    }
                    catch (System.Exception e)
                    {
                        log.LogError($"[MonsterDropTablePatcher] Dump failed. {e}");
                    }
                }

                if (bDoPatch.Value)
                {
                    try
                    {
                        log.LogMessage($"[MonsterDropTablePatcher] Start patch monster drop table...");
                        Patcher.DoPatch(__instance);
                    }
                    catch (System.Exception e)
                    {
                        log.LogError($"[MonsterDropTablePatcher] Patch failed. {e}");
                    }
                }
            }
        }
    }
}
