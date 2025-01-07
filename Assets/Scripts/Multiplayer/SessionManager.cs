using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// ------------------------------------------------------------------
// Handles starting, joining, and managing multiplayer sessions
// ------------------------------------------------------------------

public class SessionManager : SimulationBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] int _numPlayers;

    NetworkRunner _networkRunner;
    int _playersJoined = 0;

    // Used to keep track of joining players; when all players join, used to
    // update the PlayerManager
    List<PlayerRef> _joinedPlayers = new();

    // Creates buttons to choose whether to host or join
    void OnGUI()
    {
        if (_networkRunner == null)
        {
            if (GUI.Button(new Rect(0, 0, 200, 40), "Host"))
            {
                StartGame(GameMode.Host);
            }
            if (GUI.Button(new Rect(0, 40, 200, 40), "Join"))
            {
                StartGame(GameMode.Client);
            }
        }
    }

    // Creates or joins a session depending on mode
    async void StartGame(GameMode mode)
    {
        // Create the Fusion runner and let it know that we will be
        // providing user input
        _networkRunner = gameObject.AddComponent<NetworkRunner>();
        _networkRunner.ProvideInput = true;

        // Create the NetworkSceneInfo from the current scene
        SceneRef scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        NetworkSceneInfo sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        StartGameArgs startGameArgs = new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestRoom",
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        };

        // Start or join (depends on gamemode) a session with a specific name
        await _networkRunner.StartGame(startGameArgs);
    }

    public void OnPlayerJoined(NetworkRunner runner, 
        PlayerRef player) 
    {
        if (runner.IsServer)
            OnPlayerJoinedServer(runner, player);
    }

    // Called by server when a player joins the session
    void OnPlayerJoinedServer(NetworkRunner runner, 
        PlayerRef player)
    {
        _playersJoined++;
        _joinedPlayers.Add(player);
        Debug.Log(_playersJoined.ToString() + " players joined");
        bool allPlayersJoined = (_playersJoined == _numPlayers);

        if (allPlayersJoined)
        {
            InitializeSession();
        }
    }

    // Does all necessary work to initialize the session
    void InitializeSession()
    {
        int playerID = 0;
        foreach (PlayerRef joinedPlayer in _joinedPlayers)
        {
            PlayerID id = new(playerID);
            playerID++;
            StateManager.RequestStateUpdate(new AddPlayerUpdate(joinedPlayer, id));
            PlayerManager.RPC_SendPlayerID(_networkRunner, joinedPlayer, id);
        }

        StartGameUpdate gameStarted = new(MapGenerator.GenerateRandomSeed());
        StateManager.RequestStateUpdate(gameStarted);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) 
    {
        Debug.Log("Player left");
    }

    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
}