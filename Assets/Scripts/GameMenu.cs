using UnityEngine;
using System.Collections;

public class GameMenu : MonoBehaviour
{

	// Use this for initialization
	public GameManager gameManager;
	private GameManager _gameManager;
	private bool isStarted = true;

	void OnGUI ()
	{
		if (isStarted) {
			if (GUI.Button (new Rect (10, 0, 200, 50), "点击开始游戏")) {
				isStarted = false;
				_gameManager = Instantiate (gameManager)as GameManager;
				//_gameManager.music.Stop ();
			}
		}
		if (!isStarted) {
			if (GUI.Button (new Rect (10, 0, 200, 50), "重新开始游戏")) {
				if (_gameManager != null) {
					Destroy (_gameManager.gameObject);
					print ("Destroy(_gameManager.gameObject);");
				}
				_gameManager = Instantiate (gameManager)as GameManager;
			}
		}
        if(GUI.Button(new Rect(10, 60, 200, 50), "结束游戏"))
        {

            Application.Quit();
            
        }
	}
}
