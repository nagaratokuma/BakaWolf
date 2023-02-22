using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Launcher : MonoBehaviourPunCallbacks
{

    [Tooltip("The Ui Panel to let the user enter name, connect and play")]
    [SerializeField] private GameObject ControlPanel;

    [Tooltip("The UI Label to inform the Number of Player")]
    [SerializeField] private GameObject PlayerCountLabel;

    [Tooltip("The UI Button to Start Game")]
    [SerializeField] private GameObject StartButton;

    [Tooltip("The UI Button to Enter the Room")]
    [SerializeField] private GameObject EnterButton;

    /*
    // 待機状態を表す定数
    [SerializeField] private bool IsWaiting = false;
    */

    // プレイ人数を格納する変数
    [SerializeField] public int playerCount = 3;

    // 選択された問題を格納する変数
    [SerializeField] public int selectedQuiz = 0;

    Hashtable RoomHastable = new ExitGames.Client.Photon.Hashtable();

    void Awake()
    {
        // PhotonNetwork.AutomaticallySyncScene を有効にするとマスタークライアントがシーンをロードすると他のクライアントも同じシーンをロードするようになる
        PhotonNetwork.AutomaticallySyncScene = true;
        
    }
    // Start is called before the first frame update
    private void Start() {
        ControlPanel.SetActive(true);
        PlayerCountLabel.SetActive(false);
        StartButton.SetActive(false);
        //EnterButtoninteractableをfalseにする
        EnterButton.GetComponent<Button>().interactable = false;
        // PhotonServerSettingsの設定内容を使ってマスターサーバーへ接続する
        PhotonNetwork.ConnectUsingSettings();
    }

    // ルーム入室ボタンが押されたときに呼ばれる
    public void Connect()
    {
        ControlPanel.SetActive(false);
        PlayerCountLabel.SetActive(true);
        

        // 選択されたプレイ人数の名前のルームに参加する（ルームが存在しなければ作成して参加する）
        PhotonNetwork.JoinOrCreateRoom("Room" + playerCount, new RoomOptions(), TypedLobby.Default);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        ControlPanel.SetActive(true);
        PlayerCountLabel.SetActive(false);
        StartButton.SetActive(false);

        Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
    }

    void Update()
    {
        
    }

    // マスターサーバーへの接続に成功した時に呼ばれるコールバック
    public override void OnConnectedToMaster()
    {
        Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
        // EnterButtoninteractableをtrueにする
        EnterButton.GetComponent<Button>().interactable = true;
    }

    // ルームに参加した時に呼ばれるコールバック
    public override void OnJoinedRoom()
    {
        Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");

        // マスタクライアントだけがスタートボタンを押せるようにする
        if (PhotonNetwork.IsMasterClient)
        {
            StartButton.SetActive(true);
        }
        // ルーム内のプレイヤーオブジェクトの配列（ローカルプレイヤーを含む）を取得する
        var players = PhotonNetwork.PlayerList;
        Debug.Log("players.Length = " + players.Length);
        
        // PlayerCountLabelに待機人数を表示する
        PlayerCountLabel.GetComponent<Text>().text = "[現在の待機人数：" + players.Length + "人] 3人以上から開始可能";
        
        //ルーム人数のカスタムプロパティを更新する
        RoomHastable["PlayerCount"] = players.Length;
        PhotonNetwork.CurrentRoom.SetCustomProperties(RoomHastable);

    }

    // カスタムプロパティが更新された時に呼ばれるコールバック
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged){
        // PlayerCountLabelに待機人数を表示する
        if (propertiesThatChanged.ContainsKey("PlayerCount"))
        {
            PlayerCountLabel.GetComponent<Text>().text = "[現在の待機人数：" + propertiesThatChanged["PlayerCount"] + "人] 3人以上から開始可能";
            ////////////////////////////////////////
            //// テスト用に一人でもプレイ可能にする////
            ////////////////////////////////////////
            // ルーム内のプレイヤーがプレイ可能人数に達しているかどうかを判定する
            if ((int)propertiesThatChanged["PlayerCount"] >= 1)
            {
                // StartButtoninteractableをtrueにする
                StartButton.GetComponent<Button>().interactable = true;
            }
        }
    }

    // StartButtonが押された時に呼ばれる
    public void OnClickStartButton()
    {
        // ルーム内のプレイヤーがプレイ可能人数に達している場合、プレイ開始を通知する
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel("Quiz");
    }

    // QuizDropDownの値が変更された時に呼ばれる
    public void OnValueChanged(int value)
    {
        // 選択された問題を格納する
        selectedQuiz = value;

        // selectedQuizの値を確認する
        Debug.Log("selectedQuiz = " + selectedQuiz);
    }
    
    // selectedQuizを返す関数
    public int GetSelectedQuiz()
    {
        return selectedQuiz;
    }

}
