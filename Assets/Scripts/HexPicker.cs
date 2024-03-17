using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

// ------------------------------------------------------------------
// Allows a client to select a hex on their turn
// ------------------------------------------------------------------

public class HexPicker : MonoBehaviour
{
    [SerializeField] ClientPlayerData _playerData;

    void Update()
    {
        if (!_playerData.MyTurn)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _playerData.EndTurn();
        }
    }
}
