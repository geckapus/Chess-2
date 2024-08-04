using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;

public class Religion : MonoBehaviour
{
    //Properties
    public ArrayList whiteReligions = new();
    public ArrayList blackReligions = new();
    public int whiteWorship = 0;
    public int blackWorship = 0;
    public int whiteChurches = 0;
    public int blackChurches = 0;
    private bool newChurch = false;

    //Scripts
    private Controller controller;
    private UIControl uiControl;
    private ReligionSelector[] religionSelectors = new ReligionSelector[4];

    //UI
    public GameObject religionModal;
    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<Controller>();
        uiControl = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIControl>();
        for (int i = 0; i < 4; i++)
        {
            religionSelectors[i] = religionModal.transform.Find("religions").transform.GetChild(i).gameObject.GetComponent<ReligionSelector>();
        }
    }
    public void OnNewChurch()
    {
        if (controller.currentPlayer == "white") whiteChurches++;
        else blackChurches++;
        newChurch = true;
        ShowReligionModal();
        //gameObject.transform.Find("Edit Religions").gameObject.SetActive(true);
    }

    public void AddReligionToCurrentPlayer(string religion)
    {
        if (FixReligionSelectors())
        {
            Debug.Log("Religion added");
            if (controller.currentPlayer == "white")
            {
                whiteReligions.Add(religion);
            }
            else if (controller.currentPlayer == "black")
            {
                blackReligions.Add(religion);
            }
        }
        FixReligionSelectors();
        UpdateReligionIndicator();
    }
    public void RemoveReligionFromCurrentPlayer(string religion)
    {
        FixReligionSelectors();
        Debug.Log("Religion removed");
        if (controller.currentPlayer == "white")
        {
            whiteReligions.Remove(religion);
        }
        else if (controller.currentPlayer == "black")
        {
            blackReligions.Remove(religion);
        }
        Debug.Log(whiteReligions.Count + " " + blackReligions.Count);
        Debug.Log(whiteChurches + " " + blackChurches);
        FixReligionSelectors();
        UpdateReligionIndicator();
    }

    public bool FixReligionSelectors()
    {
        bool x = true;
        if ((controller.currentPlayer == "white" ? whiteChurches : blackChurches) <= GetReligions(controller.currentPlayer).Count)
        {
            foreach (ReligionSelector selector in religionSelectors)
            {
                if (!selector.followed)
                    selector.SetInteractable(false);
            }
            x = false;
        }
        else
        {
            foreach (ReligionSelector selector in religionSelectors)
            {
                selector.SetInteractable(true);
            }
        }
        return x;
    }

    public void ShowReligionModal()
    {
        religionModal.SetActive(true);
        for (int i = 0; i < 4; i++)
        {
            religionSelectors[i].Sync();
        }
        FixReligionSelectors();
    }

    public void UpdateReligionIndicator()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject image = gameObject.transform.Find("religions").transform.GetChild(i).gameObject;
            if (GetReligions(controller.currentPlayer).Contains(image.name))
                image.SetActive(true);
            else
                image.SetActive(false);
        }
        if (GetReligions(controller.currentPlayer).Count == 0)
        {
            gameObject.transform.Find("Edit Religions").gameObject.SetActive(false);
        }
        else
        {
            gameObject.transform.Find("Edit Religions").gameObject.SetActive(true);
        }
    }
    public void NextTurn()
    {
        UpdateReligionIndicator();
        //FixReligionSelectors();
    }
    public void OK()
    {
        if (newChurch)
        {
            newChurch = false;
            controller.NextTurn();
        }
        religionModal.SetActive(false);
    }

    public ArrayList GetReligions(string color)
    {
        if (color == "white") return whiteReligions;
        else return blackReligions;
    }
}
