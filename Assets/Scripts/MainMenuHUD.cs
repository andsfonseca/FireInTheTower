using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;

public class MainMenuHUD : MonoBehaviour {
    public GameObject menuDialog;
    public GameObject lobbyDialog;

    [SerializeField]
    public List<GameObject> ListOfItemsInMenu;

    private List<MatchInfoSnapshot> m_matchList;
    public void Awake() {
        menuDialog.SetActive(false);
        lobbyDialog.SetActive(false);
    }

    /// <summary>
    /// Inicializa a HUD
    /// </summary>
    public void InitializeHUD() {
        this.gameObject.SetActive(true);
        menuDialog.SetActive(true);
    }

    /// <summary>
    /// Desliga a HUD
    /// </summary>
    public void UnloadHUD() {
        menuDialog.SetActive(false);
        lobbyDialog.SetActive(false);
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Ao clica no botão de Dialogo
    /// </summary>
    public void OnClickPlayButton() {
        lobbyDialog.SetActive(true);
    }

    /// <summary>
    /// Ao clica no botão de Dialogo
    /// </summary>
    public void OnClickCloseButton() {
        lobbyDialog.SetActive(false);
    }

    /// <summary>
    /// Ao clica no botão de Dialogo
    /// </summary>
    public void OnClickCreateButton() {
        GameLogic.Instance.NetworkManager.matchMaker.CreateMatch(GameLogic.Instance.NetworkManager.matchName, GameLogic.Instance.NetworkManager.matchSize,
                                           true, string.Empty, string.Empty, string.Empty,
                                           0, 0, _OnMatchCreate);
    }

    /// <summary>
    /// Callback that happens when a <see cref="NetworkMatch.CreateMatch"/> request has been
    /// processed on the server.
    /// </summary>
    /// <param name="success">Indicates if the request succeeded.</param>
    /// <param name="extendedInfo">A text description for the error if success is false.</param>
    /// <param name="matchInfo">The information about the newly created match.</param>
#pragma warning disable 618
    private void _OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
#pragma warning restore 618
        {
        if (!success) {
            GameLogic.Instance.ShowAndroidToastMessage("Error:" + extendedInfo);
            return;
        }

        GameLogic.Instance.NetworkManager.OnMatchCreate(success, extendedInfo, matchInfo);
        GameLogic.Instance.LobbyNumber = _GetRoomNumberFromNetworkId(matchInfo.networkId);
        GameLogic.Instance.ShowAndroidToastMessage("Conectando...");

        UnloadHUD();
        GameLogic.Instance.SetGameState("BeforeHosting");
        //_ChangeLobbyUIVisibility(false);
        //CurrentRoomLabel.GetComponentInChildren<Text>().text = "Room: " + m_CurrentRoomNumber;
    }

    private void _OnJoinRoomClicked(MatchInfoSnapshot match)
#pragma warning restore 618
        {
        GameLogic.Instance.NetworkManager.matchName = match.name;
        GameLogic.Instance.NetworkManager.matchMaker.JoinMatch(match.networkId, string.Empty, string.Empty,
                                     string.Empty, 0, 0, _OnMatchJoined);
    }

    /// <summary>
    /// Callback that happens when a <see cref="NetworkMatch.JoinMatch"/> request has been
    /// processed on the server.
    /// </summary>
    /// <param name="success">Indicates if the request succeeded.</param>
    /// <param name="extendedInfo">A text description for the error if success is false.</param>
    /// <param name="matchInfo">The info for the newly joined match.</param>
#pragma warning disable 618
    private void _OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
#pragma warning restore 618
        {
        if (!success) {
            GameLogic.Instance.ShowAndroidToastMessage("Error:" + extendedInfo);
            return;
        }

        GameLogic.Instance.NetworkManager.OnMatchJoined(success, extendedInfo, matchInfo);
        GameLogic.Instance.LobbyNumber = _GetRoomNumberFromNetworkId(matchInfo.networkId);
        GameLogic.Instance.ShowAndroidToastMessage("Conectando...");
        //_ChangeLobbyUIVisibility(false);
        //CurrentRoomLabel.GetComponentInChildren<Text>().text = "Room: " + m_CurrentRoomNumber;

        UnloadHUD();
        GameLogic.Instance.SetGameState("BeforeGuestEnter");
    }

    /// <summary>
    /// Ao clica no botão de Dialogo
    /// </summary>
    public void OnClickRefreshButton() {
        //Atualiza a Lista
    }

    /// <summary>
    /// Recupera o ID da Sala
    /// </summary>
    /// <param name="networkID">NetworkID</param>
    /// <returns>Retorna um inteiro com o valor da sala</returns>
    private string _GetRoomNumberFromNetworkId(NetworkID networkID) {
        return (System.Convert.ToInt64(networkID.ToString()) % 10000).ToString();
    }

    public void CreateLobbyList(List<MatchInfoSnapshot> matches) {

        m_matchList = matches;
        foreach (var item in ListOfItemsInMenu) {
            item.SetActive(false);
        }

        int count = matches.Count;
        if (ListOfItemsInMenu.Count < count) {
            count = ListOfItemsInMenu.Count;
        }
        for (int i = 0; i < count; i++) {

            GameObject item = ListOfItemsInMenu[i];
            MatchInfoSnapshot match = m_matchList[i];
            item.SetActive(true);

            item.GetComponentInChildren<UnityEngine.UI.Text>().text = "Sala: " + _GetRoomNumberFromNetworkId(match.networkId);
            var x = item.GetComponentInChildren<UnityEngine.UI.Button>();
            item.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(() =>
                _OnJoinRoomClicked(match));

            item.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(
                GameLogic.Instance.OnEnterResolvingModeClick);

            //var text = "Room " + _GetRoomNumberFromNetworkId(match.networkId);
            //GameObject button = m_JoinRoomButtonsPool[i++];
            //button.GetComponentInChildren<Text>().text = text;
            //button.GetComponentInChildren<Button>().onClick.AddListener(() =>
            //    _OnJoinRoomClicked(match));
            //button.GetComponentInChildren<Button>().onClick.AddListener(
            //    CloudAnchorsExampleController.OnEnterResolvingModeClick);
            //button.SetActive(true);
        }

    }

}
