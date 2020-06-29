//-----------------------------------------------------------------------
// <copyright file="AnchorController.cs" company="Google">
//
// Copyright 2018 Google LLC. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at

using GoogleARCore;
using GoogleARCore.CrossPlatform;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// A Controller for the Anchor object that handles hosting and resolving the Cloud Anchor.
/// </summary>
#pragma warning disable 618
public class AnchorPointController : NetworkBehaviour
#pragma warning restore 618
    {
    /// <summary>
    /// The customized timeout duration for resolving request to prevent retrying to resolve
    /// indefinitely.
    /// </summary>
    private const float k_ResolvingTimeout = 10.0f;

    /// <summary>
    /// The Cloud Anchor ID that will be used to host and resolve the Cloud Anchor. This
    /// variable will be syncrhonized over all clients.
    /// </summary>
#pragma warning disable 618
    [SyncVar(hook = "_OnChangeId")]
#pragma warning restore 618
    private string m_CloudAnchorId = string.Empty;

    /// <summary>
    /// Indicates whether this script is running in the Host.
    /// </summary>
    private bool m_IsHost = false;

    /// <summary>
    /// Indicates whether an attempt to resolve the Cloud Anchor should be made.
    /// </summary>
    private bool m_ShouldResolve = false;

    /// <summary>
    /// Record the time since started resolving.
    /// If it passed the resolving timeout, additional instruction displays.
    /// </summary>
    private float m_TimeSinceStartResolving = 0.0f;

    /// <summary>
    /// Indicates whether passes the resolving timeout duration or the anchor has been
    /// successfully resolved.
    /// </summary>
    private bool m_PassedResolvingTimeout = false;

    /// <summary>
    /// The Cloud Anchors example controller.
    /// </summary>
    private ARController ARController;

    /// <summary>
    /// The Unity Awake() method.
    /// </summary>
    public void Awake() {
        ARController = GameLogic.Instance.AR;
    }

    /// <summary>
    /// The Unity OnStartClient() method.
    /// </summary>
    public override void OnStartClient() {
        if (m_CloudAnchorId != string.Empty) {
            m_ShouldResolve = true;
        }
    }

    /// <summary>
    /// The Unity Update() method.
    /// </summary>
    public void Update() {
        if (!m_ShouldResolve) {
            return;
        }

        if (!ARController.IsResolvingPrepareTimePassed()) {
            return;
        }

        if (!m_PassedResolvingTimeout) {
            m_TimeSinceStartResolving += Time.deltaTime;

            if (m_TimeSinceStartResolving > k_ResolvingTimeout) {
                m_PassedResolvingTimeout = true;
                ARController.OnResolvingTimeoutPassed();
            }
        }

        _ResolveAnchorFromId(m_CloudAnchorId);
    }

    /// <summary>
    /// Command run on the server to set the Cloud Anchor Id.
    /// </summary>
    /// <param name="cloudAnchorId">The new Cloud Anchor Id.</param>
#pragma warning disable 618
    [Command]
#pragma warning restore 618
    public void CmdSetCloudAnchorId(string cloudAnchorId) {
        m_CloudAnchorId = cloudAnchorId;
    }

    /// <summary>
    /// Gets the Cloud Anchor Id.
    /// </summary>
    /// <returns>The Cloud Anchor Id.</returns>
    public string GetCloudAnchorId() {
        return m_CloudAnchorId;
    }

    /// <summary>
    /// Hosts the user placed cloud anchor and associates the resulting Id with this object.
    /// </summary>
    /// <param name="lastPlacedAnchor">The last placed anchor.</param>
    public void HostLastPlacedAnchor(Component lastPlacedAnchor) {
        m_IsHost = true;

#if !UNITY_IOS
        var anchor = (Anchor)lastPlacedAnchor;
#elif ARCORE_IOS_SUPPORT
            var anchor = (UnityEngine.XR.iOS.UnityARUserAnchorComponent)lastPlacedAnchor;
#endif

#if !UNITY_IOS || ARCORE_IOS_SUPPORT
        GameLogic.Instance.ShowAndroidToastMessage("Criando Ancora");
        XPSession.CreateCloudAnchor(anchor).ThenAction(result => {
            if (result.Response != CloudServiceResponse.Success) {
                Debug.Log(string.Format("Failed to host Cloud Anchor: {0}", result.Response));

                ARController.OnAnchorHosted(
                    false, result.Response.ToString());
                return;
            }

            Debug.Log(string.Format(
                "Cloud Anchor {0} was created and saved.", result.Anchor.CloudId));
            CmdSetCloudAnchorId(result.Anchor.CloudId);

            ARController.OnAnchorHosted(true, result.Response.ToString());
        });
#endif
    }

    /// <summary>
    /// Resolves an anchor id and instantiates an Anchor prefab on it.
    /// </summary>
    /// <param name="cloudAnchorId">Cloud anchor id to be resolved.</param>
    private void _ResolveAnchorFromId(string cloudAnchorId) {
        ARController.OnAnchorInstantiated(false);

        // If device is not tracking, let's wait to try to resolve the anchor.
        if (Session.Status != SessionStatus.Tracking) {
            return;
        }

        m_ShouldResolve = false;

        GameLogic.Instance.ShowAndroidToastMessage("Recuperando Ancora");
        XPSession.ResolveCloudAnchor(cloudAnchorId).ThenAction(
            (System.Action<CloudAnchorResult>)(result => {
                if (result.Response != CloudServiceResponse.Success) {
                    Debug.LogError(string.Format(
                        "Client could not resolve Cloud Anchor {0}: {1}",
                        cloudAnchorId, result.Response));

                    ARController.OnAnchorResolved(
                        false, result.Response.ToString());
                    m_ShouldResolve = true;
                    return;
                }

                Debug.Log(string.Format(
                    "Client successfully resolved Cloud Anchor {0}.",
                    cloudAnchorId));

                ARController.OnAnchorResolved(
                    true, result.Response.ToString());
                _OnResolved(result.Anchor.transform);
            }));
    }

    /// <summary>
    /// Callback invoked once the Cloud Anchor is resolved.
    /// </summary>
    /// <param name="anchorTransform">Transform of the resolved Cloud Anchor.</param>
    private void _OnResolved(Transform anchorTransform) {
        var cloudAnchorController = GameLogic.Instance.AR ;
        cloudAnchorController.SetWorldOrigin(anchorTransform);
        GameLogic.Instance.AR._globalARSelected = anchorTransform.gameObject;
        GameLogic.Instance.SetGameState("Basic");
        // Mark resolving timeout passed so it won't fire OnResolvingTimeoutPassed event.
        m_PassedResolvingTimeout = true;
    }

    /// <summary>
    /// Callback invoked once the Cloud Anchor Id changes.
    /// </summary>
    /// <param name="newId">New identifier.</param>
    private void _OnChangeId(string newId) {
        if (!m_IsHost && newId != string.Empty) {
            m_CloudAnchorId = newId;
            m_ShouldResolve = true;
        }
    }
}

