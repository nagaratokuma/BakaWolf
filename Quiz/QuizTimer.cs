using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuizTimer : MonoBehaviour
{
    public float width = 1100;
    public float height = 14;
    //タイマーの時間
    public float LimitTime = 10;
    //経過時間
    private float ElapsedTime = 0;
    public RectTransform rectTransform;
    // Start is called before the first frame update
    void Start()
    {
        float width = 1100;
        float height = 14;       

        rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(width, height);
    }

    // Update is called once per frame
    void Update()
    {
        //新しいサイズ
        float NewWidth;

        //経過時間を加算
        ElapsedTime += Time.deltaTime;
        NewWidth = width - width * (ElapsedTime/ LimitTime);
        //サイズを更新する
        rectTransform.sizeDelta = new Vector2(NewWidth, height);

        //経過時間が制限時間を超えたら
        if (ElapsedTime >= LimitTime)
        {
            Quiz.instance.TimeOver();
        }
    }
}
