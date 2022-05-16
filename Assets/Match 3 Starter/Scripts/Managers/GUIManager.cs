/*
 * Copyright (c) 2017 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUIManager : MonoBehaviour {
	public static GUIManager instance;

	public GameObject gameOverPanel;
	public Text yourScoreTxt;
	public Text highScoreTxt;
	[SerializeField]private int moveCounter;
	public GameObject BMI, HighBP, LowBP;
	public Text scoreTxt;
	public Text moveCounterTxt;
	public float Health;
	public float MaxHealth = 100;
	public int score;
	public Button btn;
	public InputField Ht, Wt, Sys, Dia;
	[SerializeField] private float ht, wt;
	[SerializeField] private int  sys, dia;
	[SerializeField] private Image HealthImage;
	public int Score
	{
		get
		{
			return score;
		}

		set
		{
			score = value;
			scoreTxt.text = score.ToString();
		}
	}

	public int MoveCounter
	{
		get
		{
			return moveCounter;
		}

		set
		{
			moveCounter = value;
			if (moveCounter <= 0)
			{
				moveCounter = 0;
				StartCoroutine(WaitForShifting());
				
			}

			moveCounterTxt.text = moveCounter.ToString();
		}
	}
    public void Start()
    {
		btn.onClick.AddListener(GetInputOnClickHandler);
    }
	public void GetInputOnClickHandler()
    {
		ht = float.Parse(Ht.text);
		wt = float.Parse(Wt.text);
		sys = int.Parse(Sys.text);
		dia = int.Parse(Dia.text);
		if ((wt / (ht * ht) > 25f || (wt / ht * ht) < 18f))
		{
			Health -= 20;
		}
		if (sys > 120 || dia > 80)
		{
			Health -= 20;
		}

	}
    public void Update()
    {
        if (Health > 100)
        {
			Health = 100;
        }
		if (Health < 0){
			StartCoroutine(WaitForShifting());

		}
		UpdateHealth();
	}
	private void UpdateHealth()
    {
		HealthImage.fillAmount = Health / MaxHealth;
    }

    void Awake() {
		instance = GetComponent<GUIManager>();
		moveCounter = 60;
		moveCounterTxt.text = moveCounter.ToString();
		Health = 100;
	}

	// Show the game over panel
	public void GameOver() {
		GameManager.instance.gameOver = true;

		gameOverPanel.SetActive(true);

		if (score > PlayerPrefs.GetInt("HighScore")) {
			PlayerPrefs.SetInt("HighScore", score);
			highScoreTxt.text = "New Best: " + PlayerPrefs.GetInt("HighScore").ToString();
		} else {
			highScoreTxt.text = "Best: " + PlayerPrefs.GetInt("HighScore").ToString();
		}
        if (sys > 140)
        {
			HighBP.SetActive(true);
        }
        else
        {
            if (dia < 60)
            {
				LowBP.SetActive(true);
            }
        }
		if(wt/(ht*ht)>25 || wt / (ht * ht) < 18)
        {
			BMI.SetActive(true);
        }

		yourScoreTxt.text = score.ToString();
	}
	private IEnumerator WaitForShifting()
	{
		yield return new WaitUntil(() => !BoardManager.instance.IsShifting);
		yield return new WaitForSeconds(.25f);
		GameOver();
	}


}
