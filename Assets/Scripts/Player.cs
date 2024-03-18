using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Used by server to store and manage data representing a player
// ------------------------------------------------------------------

public class Player
{
    // Reference to the client corresponding to this player
    public PlayerRef ClientRef { get; }     
    public Vector3Int SelectedHex {  get; set; }
    int _playerID;    
    

    public Player(PlayerRef playerRefIn, 
        int playerIDIn)
    {
        ClientRef = playerRefIn;
        _playerID = playerIDIn;
    }
}
