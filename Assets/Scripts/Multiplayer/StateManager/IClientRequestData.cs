using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ------------------------------------------------------------------
// Contains the IClientRequestData interface, which is used to store
// client request data for the StateManager system. Structs implementing
// this interface will be used in RPCs, so IClientRequestData needs to 
// implement the INetworkStruct interface
// ------------------------------------------------------------------

public interface IClientRequestData : INetworkStruct
{ }
