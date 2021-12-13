using System;
using System.Windows.Controls;
using Sandbox;
using Sandbox.ModAPI;
using Torch;
using Torch.API;
using Torch.API.Plugins;

namespace AndNetwork9.War.Plugin;

public class Plugin : TorchPluginBase, IWpfPlugin, ITorchPlugin, IDisposable
{
    public override void Init(ITorchBase torch)
    {
        torch.
        base.Init(torch);
    }

    public override void Update()
    {
        base.Update();
    }

    public UserControl GetControl() => throw new NotImplementedException();

    private void TorchOnGameStateChanged(MySandboxGame game, TorchGameState newstate)
    {
        switch (newstate)
        {
            case TorchGameState.Creating:
                break;
            case TorchGameState.Created:
                break;
            case TorchGameState.Loading:
                InitScripts();
                break;
            case TorchGameState.Loaded:
                break;
            case TorchGameState.Unloading:
                break;
            case TorchGameState.Unloaded:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newstate), newstate, null);
        }
    }

    private void InitScripts(MySandboxGame game)
    {
        MyAPIGateway.
    }

    private void DisposeAddons()
    {
        foreach (IDisposable? addon in _addons) addon?.Dispose();
        _addons.Clear();
    }
}