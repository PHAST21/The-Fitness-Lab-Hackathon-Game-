using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {
	private static Color selectedColor = new Color(.5f, .5f, .5f, 1.0f);
	private static Tile previousSelected = null;

	private SpriteRenderer render;
	private bool isSelected = false;
	private bool matchFound = false;

	private Vector2[] adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

	void Awake() {
		render = GetComponent<SpriteRenderer>();
    }

	private void Select() {
		isSelected = true;
		render.color = selectedColor;
		previousSelected = gameObject.GetComponent<Tile>();
		SFXManager.instance.PlaySFX(Clip.Select);
	}

	private void Deselect() {
		isSelected = false;
		render.color = Color.white;
		previousSelected = null;
	}
	void OnMouseDown()
	{
		// 1
		if (render.sprite == null || BoardManager.instance.IsShifting)
		{
			return;
		}

		if (isSelected)
		{ // 2 Is it already selected?
			Deselect();
		}
		else
		{
			if (previousSelected == null)
			{ // 3 Is it the first tile selected?
				Select();
			}
			else
			{
				if (GetAllAdjacentTiles().Contains(previousSelected.gameObject))
				{ // 1
					SwapSprite(previousSelected.render); // 2
					previousSelected.ClearAllMatches();
					previousSelected.Deselect();
					ClearAllMatches();

				}
				else
				{ // 3
					previousSelected.GetComponent<Tile>().Deselect();
					Select();
				}
			}

		}
	}
	public void SwapSprite(SpriteRenderer render2)
	{ // 1
		if (render.sprite == render2.sprite)
		{ // 2
			return;
		}

		Sprite tempSprite = render2.sprite; // 3
		render2.sprite = render.sprite; // 4
		render.sprite = tempSprite; // 5
		SFXManager.instance.PlaySFX(Clip.Swap); // 6
		GUIManager.instance.MoveCounter--;

	}
	private GameObject GetAdjacent(Vector2 castDir)
	{
		RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
		if (hit.collider != null)
		{
			return hit.collider.gameObject;
		}
		return null;
	}
	private List<GameObject> GetAllAdjacentTiles()
	{
		List<GameObject> adjacentTiles = new List<GameObject>();
		for (int i = 0; i < adjacentDirections.Length; i++)
		{
			adjacentTiles.Add(GetAdjacent(adjacentDirections[i]));
		}
		return adjacentTiles;
	}
	private List<GameObject> FindGMatch(Vector2 castDir)
	{ // 1
		List<GameObject> matchingTiles = new List<GameObject>(); // 2
		RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir); // 3
		while (hit.collider != null && hit.collider.GetComponent<SpriteRenderer>().sprite == render.sprite && BoardManager.instance.characters.Contains(render.sprite))
		{ // 4
			matchingTiles.Add(hit.collider.gameObject);
			hit = Physics2D.Raycast(hit.collider.transform.position, castDir);
		}
		return matchingTiles; // 5
	}
	private List<GameObject> FindBMatch(Vector2 castDir)
	{ // 1
		List<GameObject> matchingTiles = new List<GameObject>(); // 2
		RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir); // 3
		while (hit.collider != null && hit.collider.GetComponent<SpriteRenderer>().sprite == render.sprite && BoardManager.instance.EvilCharacters.Contains(render.sprite))
		{ // 4
			matchingTiles.Add(hit.collider.gameObject);
			hit = Physics2D.Raycast(hit.collider.transform.position, castDir);
		}
		return matchingTiles; // 5
	}
	private void GClearMatch(Vector2[] paths) // 1
	{
		List<GameObject> matchingTiles = new List<GameObject>(); // 2
		for (int i = 0; i < paths.Length; i++) // 3
		{
			matchingTiles.AddRange(FindGMatch(paths[i]));
		}
		if (matchingTiles.Count >= 2) // 4
		{
			for (int i = 0; i < matchingTiles.Count; i++) // 5
			{
				matchingTiles[i].GetComponent<SpriteRenderer>().sprite = null;
			}
			matchFound = true; // 6
		}
	}
	private void BClearMatch(Vector2[] paths) // 1
	{
		List<GameObject> matchingTiles = new List<GameObject>(); // 2
		for (int i = 0; i < paths.Length; i++) // 3
		{
			matchingTiles.AddRange(FindBMatch(paths[i]));
		}
		if (matchingTiles.Count >= 2) // 4
		{
			for (int i = 0; i < matchingTiles.Count; i++) // 5
			{
				matchingTiles[i].GetComponent<SpriteRenderer>().sprite = null;
			}
			matchFound = true; // 6
		}
	}
	public void ClearAllMatches()
	{
		if (render.sprite == null)
			return;

		GClearMatch(new Vector2[2] { Vector2.left, Vector2.right });
		GClearMatch(new Vector2[2] { Vector2.up, Vector2.down });
		if (matchFound)
		{
			render.sprite = null;
			matchFound = false;
			GUIManager.instance.Score += 50;
			GUIManager.instance.Health += 10;
			StopCoroutine(BoardManager.instance.FindNullTiles());
			StartCoroutine(BoardManager.instance.FindNullTiles());
			SFXManager.instance.PlaySFX(Clip.Clear);
		}
		BClearMatch(new Vector2[2] { Vector2.left, Vector2.right });
		BClearMatch(new Vector2[2] { Vector2.up, Vector2.down });
		if (matchFound)
		{
			render.sprite = null;
			matchFound = false;
			GUIManager.instance.Score -= 50;
			GUIManager.instance.Health -= 10;
			StopCoroutine(BoardManager.instance.FindNullTiles());
			StartCoroutine(BoardManager.instance.FindNullTiles());
			SFXManager.instance.PlaySFX(Clip.Clear);
		}
	}



}