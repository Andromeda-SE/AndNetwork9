using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Cube;
using Sandbox.ModAPI;
using VRage.Game.VisualScripting;
using MyVisualScriptLogicProvider = Sandbox.Game.MyVisualScriptLogicProvider;

namespace AndNetwork9.War.Plugin.LevelScripts;

public class DischargeJumpDriveOnDamage : IMyLevelScript
{
    public void Dispose() { }

    public void Update() { }

    public void GameStarted()
    {
        MyVisualScriptLogicProvider.BlockDamaged += BlockDamaged;
    }

    public void GameFinished()
    {
        MyVisualScriptLogicProvider.BlockDamaged -= BlockDamaged;
    }

    private static void BlockDamaged(
        string entityName,
        string gridName,
        string typeid,
        string subtypeId,
        float damage,
        string damageType,
        long attackerId)
    {
        MyFatBlockReader<MyJumpDrive> blocks =
            ((MyCubeGrid)MyVisualScriptLogicProvider.GetEntityByName(gridName))
            .GetFatBlocks<MyJumpDrive>();


        foreach (IMyJumpDrive jumpDrive in blocks) jumpDrive.CurrentStoredPower -= damage;
    }
}