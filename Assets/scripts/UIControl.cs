using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIControl : MonoBehaviour
{
    public GameObject alertPrefab;
    public GameObject dayOfWeekText;
    public GameObject enjoymentCounter;
    public GameObject controller;
    public GameObject shopButton;
    public GameObject shop;
    public GameObject flipBoardToggle;
    public GameObject pawnsLostCounter;
    public GameObject savingThrow;
    private Controller ct;
    public Camera mainCamera;
    public Camera perspectiveCamera;
    GameObject alert;
    public string[] rules = new string[] { //List of all rules, used for Rule Alerts, which are activated by other scripts when a rule is triggered
        "Rules",
        "✅When the King eats a piece, he gains the piece's power. However, the king can't promote as a pawn, and may get stuck trying!",
        "✅Two towers can create a wall if there is only 1 space between them",
        "When two pawns are forced to en passant the same pawn at the same time, they fuse into a bishop and release an antipawn with some photons",
        "✅The square J7 now exists. There are no requirements that need to be filled in order for it to exist",
        "You may steal one (1) pawn from the board at any point in the game. If the opponent doesn't catch you in the act it is considered a legal move.",
        "✅Rule 10 only applies on even days of the week",
        "✅When you would be in checkmate you can roll a dexterity saving throw, 15 or higher succeeds (nat 20 kills all checking pieces)",
        "If someone gets caught in the act stated in rule 5, they will face the same consequences as stated in rule 12.",
        "✅Pressing F5 will change perspective",
        "✅Rule 6 only applies on uneven days of the week",
        "If you win a Chess 2 game without eating any piece, you get the true ending.",
        "If this rule is activated, the opposing player may analyze the current position on chess.com/analysis or lichess.org/analysis.",
        "If more than 3 pawns die on either side, start a communist revolution.",
        "✅Add the spaces i4 and i5. When a piece enters one of these squares it goes on vacation. Pieces can come back from vacation at any time. Bishops cannot come back from vacation. If your king is on vacation for more than 3 consecutive turns he gets assassinated. At the start of your turn, for each piece you have on vacation you get +2 enjoyment. When a knight and a rook of the same colour are on vacation at the same time, they fuse into a knook.",
        "✅If your queen is next to your king you can use enjoyment to buy new units. After you buy them for three round the queen can only move like a king, but afterwards the unit arrives at a designated space where they can't be taken: J4 and J5. They are teleported on a free field to the left of them after at most two turns. If that fails the player who bought them loses. Pawn: 3 Enjoyment. Horsey/Bishop: 9 Enjoyment. Rook: 15 Enjoyment. Queen: 27 Enjoyment. Hybrid Units: less expensive*more expensive = Unit.",
        "Stealing a pawn in vacation is considered a war crime by the Geneva Convention, war crimes are punished as said in rule 20.",
        "Add a new piece: Vladimir Lenin   When the communist revolution is activated, he is placed on the board in the place of the king, who is getting shot.  After that, a D20 is rolled 5 times. Each roll is for 1 pawn. 1-12 turns it into a knight, 13-19 turns it into a rook, 20 turns it in a knook"
        };

    private void Start() //initializes variables
    {
        ct = controller.GetComponent<Controller>();
        System.DateTime currentDate = System.DateTime.Now;
        System.DayOfWeek currentDay = currentDate.DayOfWeek;
        dayOfWeekText.GetComponent<TextMeshProUGUI>().text = currentDay.ToString();
    }
    private void Update() //changes perspective when F5 is pressed
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            perspectiveCamera.enabled = true;
            mainCamera.enabled = false;
            ct.isPaused = true;
            Destroy(GameObject.FindGameObjectWithTag("cursor"));
        }
        if (Input.GetKeyUp(KeyCode.F5))
        {
            perspectiveCamera.enabled = false;
            mainCamera.enabled = true;
            ct.isPaused = false;
        }
    }
    /// <summary>
    /// Creates an alert with the given title and text.
    /// </summary>
    /// <param name="title">The title of the alert.</param>
    /// <param name="text">The text content of the alert.</param>
    private void CreateAlert(string title, string text)
    {
        Debug.Log("Alert: " + title + " " + text);
        alert = Instantiate(alertPrefab, new Vector3(6.5f, -2, 0), Quaternion.identity, gameObject.transform);
        alert.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = title;
        alert.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = text;
    }
    /// <summary>
    /// Displays an alert with the given rule number and its corresponding text.
    /// Destroys the existing alert if it exists.
    /// </summary>
    /// <param name="rule">The rule number to display.</param>
    public void DisplayAlert(int rule)
    {
        if (alert != null) Destroy(alert);
        CreateAlert($"Rule {rule}", rules[rule]);
    }
    /// <summary>
    /// Updates the text of the "White" and "Black" TextMeshProUGUI components in the "enjoymentCounter" GameObject.
    /// The text is set to "White - {whiteEnjoyment}" and "Black - {blackEnjoyment}" respectively.
    /// </summary>
    public void UpdateEnjoymentCounter()
    {
        enjoymentCounter.transform.Find("White").GetComponent<TextMeshProUGUI>().text = "White - " + ct.whiteEnjoyment.ToString();
        enjoymentCounter.transform.Find("Black").GetComponent<TextMeshProUGUI>().text = "Black - " + ct.blackEnjoyment.ToString();
    }
    /// <summary>
    /// Updates the text of the "White" and "Black" TextMeshProUGUI components in the "pawnsLostCounter" GameObject.
    /// The text is set to "White - {whitePawnsTaken}" and "Black - {blackPawnsTaken}" respectively.
    /// </summary>
    public void UpdatePawnsLostCounter()
    {
        pawnsLostCounter.transform.Find("White").GetComponent<TextMeshProUGUI>().text = "White - " + ct.whitePawnsTaken.ToString();
        pawnsLostCounter.transform.Find("Black").GetComponent<TextMeshProUGUI>().text = "Black - " + ct.blackPawnsTaken.ToString();
    }
    /// <summary>
    /// Changes the interactability of the shop button based on the provided boolean value.
    /// This is used every move after checking whether the queen is by the king and not pregnant.
    /// </summary>
    /// <param name="x">A boolean value indicating whether the button should be enabled or disabled.</param>
    public void ChangeShopButton(bool x)
    {
        Debug.Log("changed button to" + x);
        shopButton.GetComponent<Button>().interactable = x;
    }
    /// <summary>
    /// Sets the active state of the pawn shop based on the provided boolean value.
    /// </summary>
    /// <param name="x">A boolean value indicating whether the shop should be active or inactive.</param>
    public void ChangeShop(bool x)
    {
        shop.SetActive(x);
    }
    /// <summary>
    /// Changes the interactability of the flip board toggle based on the provided boolean value.
    /// </summary>
    /// <param name="x">A boolean value indicating whether the toggle should be enabled or disabled.</param>
    public void ChangeFlipBoardToggle(bool x)
    {
        flipBoardToggle.GetComponent<Toggle>().interactable = x;
    }
    /// <summary>
    /// Changes the state of the saving throw UI element and updates its text.
    /// </summary>
    /// <param name="x">A boolean indicating whether the saving throw should be active or inactive.</param>
    /// <param name="roll">An optional integer representing the roll value to display.</param>
    /// <param name="instructions">An optional string representing the instructions to display.</param>
    public void ChangeSavingThrow(bool x, int roll = 20, string instructions = "instructions")
    {
        savingThrow.SetActive(x);
        savingThrow.transform.Find("Number").GetComponent<TextMeshProUGUI>().text = roll.ToString();
        savingThrow.transform.Find("Instructions").GetComponent<TextMeshProUGUI>().text = instructions;
    }

    public void SetSavingThrowOK() //This is used when the player clicks on the OK button on the dexterity saving throw screen. This method is not needed elsewhere.
    {
        ChangeSavingThrow(false);
        ct.GameOverChecked();
    }
}
