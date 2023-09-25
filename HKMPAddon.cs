using Hkmp.Api.Client;
using Hkmp.Api.Server;
using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using HKMPAddon.HKMP;

using UObject = UnityEngine.Object;

namespace HKMPAddon;

internal class HkmpAddon : Mod
{
    internal static HkmpAddon Instance { get; private set; }

    /// <summary>
    /// An instance of the client add-on class.
    /// </summary>
    private HKMPAddonClientAddon _clientAddon;
        
    /// <summary>
    /// An instance of the server add-on class.
    /// </summary>
    private HKMPAddonServerAddon _serverAddon;

    public HkmpAddon() : base("HKMPAddon") { }

    public override string GetVersion()
    {
        return Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }

    public override void Initialize()
    {
        Log("Initializing");

        Instance = this;

        _clientAddon = new HKMPAddonClientAddon();
        _serverAddon = new HKMPAddonServerAddon();

        // Register the client and server add-ons.
        ClientAddon.RegisterAddon(_clientAddon);
        ServerAddon.RegisterAddon(_serverAddon);

        Log("Initialized");
    }
}