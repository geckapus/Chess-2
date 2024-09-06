using System.Collections;
using System.Collections.Generic;
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
    public GameObject MartinAI;
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
        UpdateReligionIndicator();
        uiControl.CreateAlert("Rule 18", "Add religion to chess. Every turn, you can choose to convert one of your adjacent rooks and bishops to fuse into a brook- I mean church -- which moves like both and can generate 1 worship point per turn. Worship points are used as stated in rule 22. You can choose to worship one of four different deities for different effects. If you have 2+ churches, you can worship 2+ at a time.");
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
        if (GetReligions(controller.currentPlayer).Count < GetChurches(controller.currentPlayer) && GetChurches(controller.currentPlayer) > 0)
        {
            gameObject.transform.Find("Edit Religions").gameObject.SetActive(true);
        }
        else
        {
            gameObject.transform.Find("Edit Religions").gameObject.SetActive(false);
        }
    }
    public void NextTurn()
    {
        UpdateReligionIndicator();
        CloseMartin();
        if (GetReligions(controller.currentPlayer).Contains("Martin"))
        {
            MartinAI.SetActive(true);
            if (transform.parent.transform.Find("Alerts").transform.childCount > 0) //Martin will not spawn if there is an alert
                Destroy(transform.parent.transform.Find("Alerts").GetChild(0).gameObject);
        }
        else MartinAI.SetActive(false);
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
    public int GetChurches(string color)
    {
        if (color == "white") return whiteChurches;
        else return blackChurches;
    }
    public void MartinAccept()
    {
        ChessPiece.DestroyMovePlates();
        GameObject piece = controller.GetCurrentPlayerPieces()[Random.Range(0, controller.GetCurrentPlayerPieces().Length)];
        GameObject[] plates = AvailableMovePlates(piece);
        if (plates.Length > 0)
        {
            GameObject plate = plates[Random.Range(0, plates.Length)];
            ChessPiece chessPiece = piece.GetComponent<ChessPiece>();
            MovePlate plateComp = plate.GetComponent<MovePlate>();
            piece.GetComponent<ChessPiece>().MovePlateSpawn(plateComp.position.x, plateComp.position.y, plateComp.capturing, "ai");
            piece.GetComponent<ChessPiece>().MovePlateSpawn(chessPiece.Position.x, chessPiece.Position.y, plateComp.capturing, "ai");
            ChessPiece.DestroyMovePlates();
        }
        else MartinAccept();
    }
    private GameObject[] AvailableMovePlates(GameObject piece)
    {
        List<GameObject> availableMovePlates = new List<GameObject>();
        ChessPiece chessPiece = piece.GetComponent<ChessPiece>();
        chessPiece.InitiateMovePlates();
        foreach (GameObject plate in FindObjectsOfType(typeof(GameObject)))
        {
            if (plate.GetComponent<MovePlate>())
            {
                MovePlate plateComp = plate.GetComponent<MovePlate>();
                if (plateComp.indicator == null || plateComp.indicator == "2squares" || plateComp.indicator == "promote" || plateComp.indicator.Contains("castle") || plateComp.indicator.Contains("fuse"))
                {
                    Debug.Log(plate.name + plateComp.position);
                    availableMovePlates.Add(plate);
                }
            }
        }
        return availableMovePlates.ToArray();
    }

    public void CloseMartin()
    {
        MartinAI.transform.Find("Initial").gameObject.SetActive(true);
        MartinAI.transform.Find("Rating").gameObject.SetActive(false);
        uiControl.ChangeMartinStars(-1);
        MartinAI.SetActive(false);
    }
}
