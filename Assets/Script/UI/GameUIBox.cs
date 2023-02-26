using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class GameUIBox : UIBoxBase {

	public Text scoreText;
	public Text timeText;
	public Text hightestText;


	public int totalTime = 60;
	private float timeCal=0;




	private int scoreTemp=0;

	private int curscoreTemp = 0;

	private float timeReward=0;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void ShowBox ()
	{
		this.gameObject.SetActive (true);
		StartCoroutine ("TimeDown");

		BubbleManager.Instance.scoreChangeEvent += ScoreChange;

		hightestText.text = PlayerData.Instance.GetHightestScore ().ToString();

		AudioManager.Instance.PlayMusic ("GameMusic");
	} 

	public override void HideBox ()
	{
		base.HideBox ();

		BubbleManager.Instance.scoreChangeEvent -= ScoreChange;
	}


	void ScoreChange(int score)
	{
		timeRewardAdd (score);
		DOTween.To (() => this.scoreTemp, x => this.scoreTemp = x, score, 0.6f).OnUpdate (ScoreChangeUpdate);;
	}
	void ScoreChangeUpdate()
	{
		scoreText.text = this.scoreTemp.ToString ();
	}



	float getCurRewardCoefficient(int score)
	{

		if (score > 500000) {
			return 0.0f;
		} else if (score > 200000) {
			return 0.05f;
		} else if (score > 100000) {
			return 0.1f;
		} else if (score > 50000) {
			return 0.2f;
		} else if (score > 40000) {
			return 0.3f;
		} else if (score > 30000) {
			return 0.5f;
		} else if (score > 20000) {
			return 0.75f;
		} else if (score > 10000) {
			return 0.9f;
		} else {
			return 1.0f;
		}
	}


	void timeRewardAdd(int score)
	{
		float newtime = (float)(score-curscoreTemp) * 0.03f*getCurRewardCoefficient(score);
		if (newtime > 1.0f) {
			newtime = 1.0f;
		}

		timeReward += newtime;
		curscoreTemp = score;
	}


	public void OnPauseBtnClick()
	{
		UIManager.Instance.ShowBox (UIBoxType.PauseBox);
		UIManager.Instance.isPause = true;

		AudioManager.Instance.ButtonClick ();
	}


	/*
	修改游戏模式，计时模式改为类模式
	 */
	IEnumerator TimeDown()
	{
		float maxtimeSize = 1f;
		timeCal = totalTime;
		while (true) {
		    while(UIManager.Instance.isPause ==true)
			{
				yield return 0;
			}

			timeCal -=Time.deltaTime;
			timeText.text = ((int)timeCal).ToString();

			if(timeReward>=0)
			{

				if(timeCal>100)
				{
					maxtimeSize=0.0f;
				}
				else if (timeCal>90)
				{
					maxtimeSize=0.5f;
				}
				else if (timeCal>80)
				{
					maxtimeSize=0.8f;
				}
				else
				{
					maxtimeSize=1f;
				}
				timeCal+=(int)(timeReward*maxtimeSize);
				timeReward=0;
			}

			if(timeCal <= 0)
			{
				/*
				GameEnd();
				StopCoroutine("TimeDown");
				*/
				GameEndStopCoroutine();
			}

			yield return 0;
		}
	}



	void GameEndStopCoroutine(){
		curscoreTemp = 0;
		GameEnd();
		StopCoroutine("TimeDown");
	}



	void GameEnd()
	{
		UIManager.Instance.ShowBox (UIBoxType.GameEndBox);
	}
}
