using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.UI;

public class Link : MonoBehaviour
{
	public GameObject controller;
	public void OpenLichessFEN()
	{
#if !UNITY_EDITOR
		openWindow("https://lichess.org/analysis/standard/" + controller.GetComponent<Controller>().PositionToFEN());
#endif
	}
	public void OpenDay20()
	{
#if !UNITY_EDITOR
		openWindow("https://www.reddit.com/r/AnarchyChess/comments/1de6z62/top_comment_adds_a_new_rule_to_chess_2_day_20/");
#endif
	}

	[DllImport("__Internal")]
	private static extern void openWindow(string url);

}