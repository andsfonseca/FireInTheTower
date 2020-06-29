using GoogleARCore.Examples.CloudAnchors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//-----------------------------------------------------------------------
// <copyright file="LocalPlayerController.cs" company="Google">
//
// Copyright 2018 Google LLC. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Local player controller. Handles the spawning of the networked Game Objects.
/// </summary>
#pragma warning disable 618
public class LocalPlayerPaintingController : NetworkBehaviour
#pragma warning restore 618
    {

    public GameObject Anchor;


    /// <summary>
    /// The Unity OnStartLocalPlayer() method.
    /// </summary>
    public override void OnStartLocalPlayer() {
        base.OnStartLocalPlayer();

        // A Name is provided to the Game Object so it can be found by other Scripts, since this
        // is instantiated as a prefab in the scene.
        gameObject.name = "LocalPlayer";
    }

    /// <summary>
    /// Will spawn the origin anchor and host the Cloud Anchor. Must be called by the host.
    /// </summary>
    /// <param name="position">Position of the object to be instantiated.</param>
    /// <param name="rotation">Rotation of the object to be instantiated.</param>
    /// <param name="anchor">The ARCore Anchor to be hosted.</param>
    public void SpawnAnchor(Vector3 position, Quaternion rotation, Component anchor) {

        //Caso a posição seja nula, cria o objeto
        if (GameLogic.Instance.AR._globalARSelected == null) {
            GameLogic.Instance.AR._globalARSelected = Instantiate(Anchor);
        }

        GameLogic.Instance.AR._globalARSelected.transform.position = position;
        GameLogic.Instance.AR._globalARSelected.transform.rotation = rotation;

        //Faz com que o objeto se rotacione para o lado do usuário, 
        //_globalARSelected.transform.Rotate(0, 180, 0, Space.Self);

        //// Instantiate Anchor model at the hit pose.
        //var anchorObject = Instantiate(AnchorPrefab, position, rotation);

        // Anchor must be hosted in the device.
        GameLogic.Instance.AR._globalARSelected.GetComponent<AnchorPointController>().HostLastPlacedAnchor(anchor);

        // Host can spawn directly without using a Command because the server is running in this
        // instance.
#pragma warning disable 618
        NetworkServer.Spawn(GameLogic.Instance.AR._globalARSelected);
#pragma warning restore 618
    }

}


