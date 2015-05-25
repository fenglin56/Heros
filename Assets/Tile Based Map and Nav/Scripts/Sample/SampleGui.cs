// ====================================================================================================================
//
// Created by Leslie Young
// http://www.plyoung.com/ or http://plyoung.wordpress.com/
// ====================================================================================================================

using UnityEngine;
using System.Collections;

public class SampleGui : MonoBehaviour 
{

	private GameController game;
	private Rect winRect = new Rect(10f, 10f, 120f, 50f);

	void Start()
	{
		game = gameObject.GetComponent<GameController>();
	}

	void OnGUI()
	{
		if (game.allowInput)
		{
			winRect = GUILayout.Window(0, winRect, theWindow, "Game GUI");
		}
	}

	private void theWindow(int id)
	{
		GUILayout.Space(10f);
		game.useTurns = GUILayout.Toggle(game.useTurns, "USE TURNS");

		if (game.useTurns)
		{
			GUILayout.Space(10f);
			if (GUILayout.Button("Change Turns")) game.ChangeTurn();

			GUILayout.Space(10f);
			GUILayout.Label(string.Format("Player {0}'s Turn", game.currPlayerTurn+1));
		}
	}
}
