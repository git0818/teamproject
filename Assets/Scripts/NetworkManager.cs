using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager instance;

    [SerializeField]
    private TMP_InputField NicknameInput;
    [SerializeField]
    private GameObject DisconnectPanel;
    [SerializeField]
    private GameObject RespawnPanel;
    [SerializeField]
    private Camera Localcamera;
    [SerializeField]
    private Camera Remotecamera;
    [SerializeField]
    private Transform LocalSpwanPos;
    [SerializeField]
    private Transform RemoteSpwanPos;
    [SerializeField]
    private GameObject LocalUI;
    [SerializeField]
    private GameObject RemoteUI;
    [SerializeField]
    private TextMeshProUGUI LocalCountdown;
    [SerializeField]
    private TextMeshProUGUI RemoteCountdown;
    [SerializeField]
    private PhotonView PV;

    //처음에 대기하는거
    public static bool gamestartcheck = false;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

            Screen.SetResolution(960, 540, false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
    }

    public void Connect() => PhotonNetwork.ConnectUsingSettings();


    public override void OnConnectedToMaster()
    {
        PhotonNetwork.LocalPlayer.NickName = NicknameInput.text;
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 2 }, null);
    }

    public override void OnJoinedRoom()
    {
        Localcamera.rect = new Rect(0, 0.5f, 1, 0.5f);
        Remotecamera.gameObject.SetActive(true);
        DisconnectPanel.SetActive(false);
        Spawn();
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            PV.RPC("GameStart", RpcTarget.All);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }
    [PunRPC]
    void GameStart()
    {
        StartCoroutine("CountdownStart");
    }
    IEnumerator CountdownStart()
    {
        LocalCountdown.gameObject.SetActive(true);
        RemoteCountdown.gameObject.SetActive(true);
        LocalCountdown.text = "준비...";
        RemoteCountdown.text = "준비...";
        yield return new WaitForSeconds(1f);
        LocalCountdown.text = "3";
        RemoteCountdown.text = "3";
        yield return new WaitForSeconds(1f);
        LocalCountdown.text = "2";
        RemoteCountdown.text = "2";
        yield return new WaitForSeconds(1f);
        LocalCountdown.text = "1";
        RemoteCountdown.text = "1";
        yield return new WaitForSeconds(1f);
        gamestartcheck = true;
        LocalCountdown.text = "출발!";
        RemoteCountdown.text = "출발!";
        yield return new WaitForSeconds(0.5f);
        LocalCountdown.gameObject.SetActive(false);
        RemoteCountdown.gameObject.SetActive(false);

    }
    public void Spawn()
    {
        //Resources 폴더 안에 있는 프리팹의 이름
        if (PhotonNetwork.IsMasterClient)
        {
            LocalUI.SetActive(true);
            RemoteUI.SetActive(false);
            PhotonNetwork.Instantiate("Player", LocalSpwanPos.position, Quaternion.identity);
        }
        else
        {
            LocalUI.SetActive(false);
            RemoteUI.SetActive(true);
            PhotonNetwork.Instantiate("Player", RemoteSpwanPos.position, Quaternion.identity);
        }
        RespawnPanel.SetActive(false);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        DisconnectPanel.SetActive(true);
        RespawnPanel.SetActive(false);
        Localcamera.rect = new Rect(0, 0, 1, 1);
        Remotecamera.gameObject.SetActive(false);
    }
}
