using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class Launcher : MonoBehaviourPunCallbacks
{
    
    [Tooltip("The UI Label to inform the user that the connection is in progress")]
    [SerializeField] private GameObject progressLabel;

    [Tooltip("The Ui Panel to let the user enter name, connect and play")]
    [SerializeField] private GameObject ControlPanel;

    // プレイ人数を格納する変数
    [SerializeField] public int playerCount = 3;

    // 待機状態を表す定数
    [SerializeField] private bool IsWaiting = false;
    void Awake()
    {
        // PhotonNetwork.AutomaticallySyncScene を有効にするとマスタークライアントがシーンをロードすると他のクライアントも同じシーンをロードするようになる
        // PhotonNetwork.AutomaticallySyncScene = true;
    }
    // Start is called before the first frame update
    private void Start() {
        progressLabel.SetActive(false);
        ControlPanel.SetActive(true);

        // PhotonServerSettingsの設定内容を使ってマスターサーバーへ接続する
        PhotonNetwork.ConnectUsingSettings();
    }

    // プレイボタンが押されたときに呼ばれる
    public void Connect()
    {
        progressLabel.SetActive(true);
        ControlPanel.SetActive(false);

        // 選択されたプレイ人数の名前のルームに参加する（ルームが存在しなければ作成して参加する）
        PhotonNetwork.JoinOrCreateRoom("Room" + playerCount, new RoomOptions(), TypedLobby.Default);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        progressLabel.SetActive(false);
        ControlPanel.SetActive(true);
        Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
    }

    void Update()
    {
        
    }

    // プルダウンメニューの値が変更されたときに呼ばれる
    public void OnValueChanged()
    {
        GameObject DropPlayerNumber = GameObject.Find ("PlayerNumber");
        Dropdown dropdown = DropPlayerNumber.GetComponent<Dropdown>();

        //DropdownのValueが1のとき（3人部屋が選択されているとき）
        if (dropdown.value == 0)
        {
            playerCount = 3;
        }
        //DropdownのValueが1のとき（4人部屋が選択されているとき）
        else if (dropdown.value == 1)
        {
            playerCount = 4;
        }
        //DropdownのValueが2のとき（5人部屋が選択されているとき）
        else if (dropdown.value == 2)
        {
            playerCount = 5;
        }
    }

    // ルームに参加した時に呼ばれるコールバック
    public override void OnJoinedRoom()
    {
        Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");

        // ルーム内のプレイヤーオブジェクトの配列（ローカルプレイヤーを含む）を取得する
        var players = PhotonNetwork.PlayerList;
        Debug.Log("players.Length = " + players.Length);
        //最初のプレイヤーである場合のみロードする。それ以外の場合は"PhotonNetwork.AutomaticallySyncScene"に依存してインスタンスシーンを同期する。
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            Debug.Log("We load the 'Room" + playerCount + "' ");

            // #Critical
            // Roomレベルをロードする
            PhotonNetwork.LoadLevel("Quiz");
        }
    }

    
}
