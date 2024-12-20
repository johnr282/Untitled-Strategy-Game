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
    public PlayerRef PlayerRef { get; }     
    public PlayerID PlayerID { get; }    
    
    public Player(PlayerRef playerRefIn, 
        PlayerID playerIDIn)
    {
        PlayerRef = playerRefIn;
        PlayerID = playerIDIn;
    }
}
