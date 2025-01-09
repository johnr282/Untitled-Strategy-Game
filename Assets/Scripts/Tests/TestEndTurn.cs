using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEndTurn : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayerManager.RequestEndActivePlayersTurn();
        }
    }
}
