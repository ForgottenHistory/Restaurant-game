using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Managing.Server;
using FishNet.Object;
using UnityEngine;

public class GameManager : NetworkBehaviour, IInitialize
{
    /////////////////////////////////////////////////////////////////////////////////////
    // PUBLIC VARIABLES
    /////////////////////////////////////////////////////////////////////////////////////

    public static GameManager Instance { get; private set; }

    public GameObject player; 

    float startupTime = 2f;

    public Transform spawnPointsParent;
    
    public bool isActive { get; set; } = true;
    
    /////////////////////////////////////////////////////////////////////////////////////
    // PRIVATE VARIABLES 
    /////////////////////////////////////////////////////////////////////////////////////

    List<Vector3> spawnPoints = new List<Vector3>();

    ServerManager serverManager;

    /////////////////////////////////////////////////////////////////////////////////////

    public void Initialize()
    {
        /////////////////////////////////////////////////////////////////////////////////////

        if (spawnPointsParent != null) {
            foreach (Transform t in spawnPointsParent) {
                spawnPoints.Add(t.position);
            }
        } else {
            Debug.LogError("No spawn points parent set!");
        }

        /////////////////////////////////////////////////////////////////////////////////////

        serverManager = InstanceFinder.ServerManager;
        foreach( int playerID in NetworkManager.ClientManager.Clients.Keys )
        {
            NetworkConnection conn = NetworkManager.ClientManager.Clients[playerID];
            SpawnPlayerRpc(conn);
        }
        
        /////////////////////////////////////////////////////////////////////////////////////
        
        StartGameClientRPC();
    }

    /////////////////////////////////////////////////////////////////////////////////////

    public void Deinitialize()
    {
        isActive = false;
    }

    /////////////////////////////////////////////////////////////////////////////////////

    private IEnumerator InitializeCoroutine()
    {
        yield return new WaitForSeconds(startupTime);
        Initialize();
    }

    /////////////////////////////////////////////////////////////////////////////////////

    public override void OnStartServer()
    {
        base.OnStartServer();
        //StartCoroutine(InitializeCoroutine());
    }

    /////////////////////////////////////////////////////////////////////////////////////
    // 
    /////////////////////////////////////////////////////////////////////////////////////

    public override void OnStartClient()
    {
        base.OnStartClient();
        Instance = this;
    }    

    /////////////////////////////////////////////////////////////////////////////////////

    [Server]
    public void SpawnPlayerRpc( NetworkConnection conn ){
        GameObject p = GameObject.Instantiate( player, spawnPoints[ Random.Range( 0, spawnPoints.Count) ], Quaternion.identity );
        serverManager.Spawn( p, conn );
    }

    /////////////////////////////////////////////////////////////////////////////////////

    [ObserversRpc]
    void StartGameClientRPC() {
        GameObject startupScreen = GameObject.Find("StartupScreen");
        if( startupScreen == null )
            Debug.LogError("No object with name StartupScreen found!");
        GameObject.Find("StartupScreen").SetActive( false );
    }
    
    /////////////////////////////////////////////////////////////////////////////////////
}
