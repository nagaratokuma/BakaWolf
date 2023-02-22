using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;// 追記
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class Quiz : MonoBehaviourPunCallbacks {
    public static Quiz instance;
    public TextAsset csvFile;// GUIでcsvファイルを割当

    // AnswerInputField
    public InputField answerInputField;

    // 問題文を表示するText
    public Text questionText;

    // 解答ボタン
    public Button answerButton;

    // 正誤判定を格納する変数
    bool isCorrect = false;

    // CSVのデータを入れるリスト
    List<string[]> csvDatas = new List<string[]>();// 追記

    
    void Awake(){
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if(instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    // Use this for initialization
    void Start () {

        // 格納
        string[] lines = csvFile.text.Replace("\r\n", "\n").Split("\n"[0]);
        foreach (var line in lines){
            if (line == "") {continue;}
            csvDatas.Add(line.Split(','));  // string[]を追加している
        }

        // 書き出し
        Debug.Log (csvDatas.Count); // 行数
        Debug.Log (csvDatas[0].Length); // 項目数
        Debug.Log (csvDatas [1] [2]);   // 2行目3列目

        ShowQuestion();
    }

    // 問題文を表示する関数
    public void ShowQuestion(){
        // 問題文を表示
        questionText.text = csvDatas [1] [1];
    }

    // AnswerButtonを押したときに呼ばれる関数
    public void AnswerButton(){
        // AnswerInputFieldのテキストを取得
        string answer = answerInputField.text;
        
        // 答え合わせ
        if (csvDatas [1] [2] == answer) {
            Debug.Log ("正解");
            isCorrect = true;
        } else {
            Debug.Log ("不正解");
            isCorrect = false;
        }
        
        var hashtable = new ExitGames.Client.Photon.Hashtable();
        hashtable.Add("isCorrect", isCorrect);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);

    }

    // カスタムプロパティを受け取ったときに呼ばれる関数
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("isCorrect"))
        {
            Debug.Log("Player: " + targetPlayer.NickName);
            Debug.Log("isCorrect: " + changedProps["isCorrect"]);
        }
    }

    // タイマーが制限時間を超えたら呼ばれる関数
    public void TimeOver(){
        // AnswerButtonのボタンを押せなくする
        answerButton.interactable = false;

        // 正誤判定をfalseにする
        isCorrect = false;
        // カスタムプロパティを送信
        var hashtable = new ExitGames.Client.Photon.Hashtable();
        hashtable.Add("isCorrect", isCorrect);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
    }
    
}