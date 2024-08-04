using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ReligionSelector : MonoBehaviour
{
    //Scripts
    private Religion religion;
    private Controller controller;

    //UI
    private GameObject followButton;

    //Properties
    public bool followed;
    public string religionName;
    // Start is called before the first frame update
    void Start()
    {
        religionName = name;
        followButton = transform.Find("Follow Button").gameObject;
        religion = GameObject.FindGameObjectWithTag("Religion").GetComponent<Religion>();
        controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<Controller>();
    }

    public void SetFollowed(bool x)
    {
        if (followButton != null)
            followButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = x ? "Unfollow" : "Follow";
        followed = x;
        if (x)
        {
            if (religion)
                religion.AddReligionToCurrentPlayer(religionName);
        }
        else
        {
            if (religion)
                religion.RemoveReligionFromCurrentPlayer(religionName);
        }
    }

    public void ToggleFollow()
    {
        SetFollowed(!followed);
    }

    public void ToggleInteractable()
    {
        followButton.GetComponent<Button>().interactable = !followButton.GetComponent<Button>().interactable;
    }
    public void SetInteractable(bool x)
    {
        Debug.Log(x);
        if (followButton == null) return;
        followButton.GetComponent<Button>().interactable = x;
    }
    public void Sync()
    {
        if (religion)
        {
            bool x = religion.GetReligions(controller.currentPlayer).Contains(religionName);
            if (followButton != null)
                followButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = x ? "Unfollow" : "Follow";
            followed = x;
            SetInteractable(x);
        }
    }
}
