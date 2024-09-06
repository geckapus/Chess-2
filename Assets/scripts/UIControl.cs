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
    private Religion religion;
    public GameObject shopButton;
    public GameObject pawnShop;
    public GameObject flipBoardToggle;
    public GameObject pawnsLostCounter;
    public GameObject savingThrow;
    public GameObject anishButton;
    public GameObject gukeshButton;
    public GameObject anishMode;
    public GameObject anishCancel;
    public GameObject anishConfirm;
    private GameObject cursorRule;
    public GameObject enPassantModal;
    private Controller ct;
    public Camera mainCamera;
    public Camera perspectiveCamera;
    public GameObject alert;
    public GameObject whiteTilemap;
    public GameObject blackTilemap;
    public GameObject gukeshConfirmationModal;
    public GameObject gukeshModal;
    public GameObject gukeshFail;
    public GameObject moveCounter;
    public GameObject trueEnding;
    public GameObject communistRevolutionModal;
    public GameObject communistRevolutionText;
    public GameObject audioSources;
    public GameObject d20UpgradeOK;
    public GameObject worshipCounter;
    public GameObject CRUpgradeModal;
    public GameObject playerIndicator;
    public GameObject[] martinStars = new GameObject[5];
    public bool audioEnabled = true;
    public Sprite starFull;
    public Sprite starEmpty;
    public string[] rules = new string[] { //List of all rules, used for Rule Alerts, which are activated by other scripts when a rule is triggered
        "Rules",
        "✅When the King eats a piece, he gains the piece's power. However, the king can't promote as a pawn, and may get stuck trying!",
        "✅Two towers can create a wall if there is only 1 space between them (bricks your pipi)",
        "✅When two pawns are forced to en passant the same pawn at the same time, they fuse into a bishop and release an antipawn with some photons",
        "✅The square J7 now exists. There are no requirements that need to be filled in order for it to exist",
        "✅You may steal one (1) pawn from the board at any point in the game. If the opponent doesn't catch you in the act it is considered a legal move.",
        "✅Rule 10 only applies on even days of the week",
        "✅When you would be in checkmate you can roll a dexterity saving throw, 15 or higher succeeds (nat 20 kills all checking pieces)",
        "✅If someone gets caught in the act stated in rule 5, they will face the same consequences as stated in rule 12.",
        "✅Pressing F5 will change perspective",
        "✅Rule 6 only applies on uneven days of the week",
        "✅If you win a Chess 2 game without eating any piece, you get the true ending.",
        "✅If this rule is activated, the opposing player may analyze the current position on chess.com/analysis or lichess.org/analysis.",
        "✅If more than 3 pawns die on either side, start a communist revolution.",
        "✅Add the spaces i4 and i5. When a piece enters one of these squares it goes on vacation. Pieces can come back from vacation at any time. Bishops cannot come back from vacation. If your king is on vacation for more than 3 consecutive turns he gets assassinated. At the start of your turn, for each piece you have on vacation you get +2 enjoyment. When a knight and a rook of the same colour are on vacation at the same time, they fuse into a knook.",
        "✅If your queen is next to your king you can use enjoyment to buy new units. After you buy them for three round the queen can only move like a king, but afterwards the unit arrives at a designated space where they can't be taken: J4 and J5. They are teleported on a free field to the left of them after at most two turns. If that fails the player who bought them loses. Pawn: 3 Enjoyment. Horsey/Bishop: 9 Enjoyment. Rook: 15 Enjoyment. Queen: 27 Enjoyment. Hybrid Units: less expensive*more expensive = Unit.",
        "✅Stealing a pawn in vacation is considered a war crime by the Geneva Convention, war crimes are punished as said in rule 20.",
        "✅Add a new piece: Vladimir Lenin   When the communist revolution is activated, he is placed on the board in the place of the king, who is getting shot.  After that, a D20 is rolled 5 times. Each roll is for 1 pawn. 1-12 turns it into a knight, 13-19 turns it into a rook, 20 turns it in a knook",
@"Add religion to chess. Every turn, you can choose to convert one of your adjacent rooks and bishops to fuse into a brook- I mean church -- which moves like both and can generate 1 worship point per turn. Worship points are used as stated in rule 22. You can choose to worship one of four different deities for different effects. If you have 2+ churches, you can worship 2+ at a time.
If the communist revolution starts, churches are burned and lost, and you will no longer be able to choose a religion.
-✅Martin from chess. com: You can use AI to make the moves for you. The AI, however, will intentionally suggest bad moves.
-✅Gavin from 3rd grade: When your opponent isn't looking, you may cheat by deleting one of their pieces -- even those that aren't pawns. If your opponent notices afterwards, the piece is returned and rule 12 activates. You can also en passant from any file.
-✅Knook: You have the ability to create knooks by fusing adjacent knights and rooks. You can also enjoy out knooks for half the price.
-Jessica: The demonic cult. You have the ability to conduct sacrifices of your pieces to the Elder Goddess Jessica, generating 3 worship points for each pawn, 9 for each major piece. Jessica also becomes a summonable troop provided that you have a 4x4 clear space anywhere on the board. Jessica moves as stated in rule 24.",
        "✅Delete the b4 square.",
        "✅If you commit a war crime, you go directly to jail. DO NOT PASS GO, DO NOT COLLECT $200"
        };

    private void Start() //initializes variables
    {
        religion = GameObject.FindGameObjectWithTag("Religion").GetComponent<Religion>();
        ct = controller.GetComponent<Controller>();
        System.DateTime currentDate = System.DateTime.Now;
        System.DayOfWeek currentDay = currentDate.DayOfWeek;
        dayOfWeekText.GetComponent<TextMeshProUGUI>().text = currentDay.ToString();
    }
    private void Update() //changes perspective when F5 is pressed
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            ChangePerspective(true);
        }
        if (Input.GetKeyUp(KeyCode.F5))
        {
            ChangePerspective(false);
        }
        if (cursorRule != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            cursorRule.transform.position = new Vector3(mousePos.x + 1.25f, mousePos.y - 1, -2.0f);
        }
    }
    private void ChangePerspective(bool x)
    {
        if (x)
        {
            perspectiveCamera.enabled = true;
            mainCamera.enabled = false;
            ct.isPaused = true;
            Destroy(GameObject.FindGameObjectWithTag("cursor"));
            gameObject.GetComponent<Canvas>().worldCamera = perspectiveCamera;
        }
        else
        {
            perspectiveCamera.enabled = false;
            mainCamera.enabled = true;
            ct.isPaused = false;
            gameObject.GetComponent<Canvas>().worldCamera = mainCamera;
        }
    }
    public void TogglePerspective()
    {
        ChangePerspective(!perspectiveCamera.enabled);
    }
    /// <summary>
    /// Creates an alert with the given title and text.
    /// </summary>
    /// <param name="title">The title of the alert.</param>
    /// <param name="text">The text content of the alert.</param>
    public void CreateAlert(string title, string text)
    {
        if (alert != null) Destroy(alert);
        //Debug.Log("Alert: " + title + " " + text);
        alert = Instantiate(alertPrefab, new Vector3(6.5f, -2, 0), Quaternion.identity, gameObject.transform.Find("Alerts"));
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
    public void UpdateWorshipCounter()
    {
        worshipCounter.transform.Find("White").GetComponent<TextMeshProUGUI>().text = "White - " + religion.whiteWorship.ToString();
        worshipCounter.transform.Find("Black").GetComponent<TextMeshProUGUI>().text = "Black - " + religion.blackWorship.ToString();
    }
    /// <summary>
    /// Updates the text of the "White" and "Black" TextMeshProUGUI components in the "pawnsLostCounter" GameObject.
    /// The text is set to "White - {whitePawnsTaken}" and "Black - {blackPawnsTaken}" respectively.
    /// </summary>
    public void UpdatePawnsLostCounter()
    {
        pawnsLostCounter.transform.Find("White").GetComponent<TextMeshProUGUI>().text = "White - " + ct.whitePawnsLost.ToString();
        pawnsLostCounter.transform.Find("Black").GetComponent<TextMeshProUGUI>().text = "Black - " + ct.blackPawnsLost.ToString();
    }
    /// <summary>
    /// Changes the interactability of the shop button based on the provided boolean value.
    /// This is used every move after checking whether the queen is by the king and not pregnant.
    /// </summary>
    /// <param name="x">A boolean value indicating whether the button should be enabled or disabled.</param>
    public void ChangeShopButton(bool x)
    {
        shopButton.GetComponent<Button>().interactable = x;
    }
    /// <summary>
    /// Sets the active state of the pawn shop based on the provided boolean value.
    /// </summary>
    /// <param name="x">A boolean value indicating whether the shop should be active or inactive.</param>
    public void ChangeShop(bool x)
    {
        pawnShop.SetActive(x);
        FixPawnShopPrices();
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
    public void ChangeAnish(bool x)
    {
        ct.anishMode = x;
        if (x)
        {
            cursorRule = Instantiate(anishMode, new Vector3(0, 0, 0), Quaternion.identity, gameObject.transform);
            ChessPiece.DestroyMovePlates(false, true);
        }
        else
            Destroy(cursorRule);
        ChangeAnishButton(!x, true);
    }
    public void ChangeAnishButton(bool x, bool cancel)
    {
        if (cancel)
        {
            anishButton.SetActive(x);
            anishCancel.SetActive(!x);
            anishConfirm.SetActive(!x);
        }
        else
        {
            anishButton.GetComponent<Button>().interactable = x;
            anishCancel.SetActive(false);
            anishConfirm.SetActive(false);
        }
    }

    public void AnishConfirm()
    {
        ChangeAnish(false);
        controller.GetComponent<Controller>().anishMovePlate.GetComponent<SpriteRenderer>().color = Color.clear;
    }
    public void AnishCancel()
    {
        ChangeAnish(false);
        Destroy(ct.anishMovePlate);
    }
    public void ChangeGukeshButton(bool x)
    {
        gukeshButton.GetComponent<Button>().interactable = x;
    }
    public void ChangeEnPassantModal(bool x)
    {
        enPassantModal.SetActive(x);
    }
    public void ToggleTileMap()
    {
        if (ct.currentPlayer == "black" && ct.settings["flip_board"] == "true")
        {
            gameObject.transform.Find("camera_right").gameObject.SetActive(false);
            gameObject.transform.Find("camera_left").gameObject.SetActive(true);
            whiteTilemap.SetActive(false);
            blackTilemap.SetActive(true);
        }
        else
        {
            gameObject.transform.Find("camera_right").gameObject.SetActive(true);
            gameObject.transform.Find("camera_left").gameObject.SetActive(false);
            whiteTilemap.SetActive(true);
            blackTilemap.SetActive(false);
        }
    }

    public void Gukesh()
    {
        gukeshConfirmationModal.SetActive(false);
        //string color = "";
        if (ct.catchGukesh)
        {
            gukeshModal.SetActive(true);
        }
        else
        {
            gukeshFail.SetActive(true);
        }
        if (ct.currentPlayer == "black")
            ct.gukeshValidBlack = false;
        else if (ct.currentPlayer == "white")
            ct.gukeshValidWhite = false;
        gukeshButton.GetComponent<Button>().interactable = false;
    }
    public void UpdateMoveCounter()
    {
        moveCounter.transform.Find("Halfmoves").GetComponent<TextMeshProUGUI>().text = "Half moves: " + ct.halfMove.ToString();
        moveCounter.transform.Find("Move").GetComponent<TextMeshProUGUI>().text = "Move " + ct.move.ToString();
    }
    public void TrueEnding()
    {
        trueEnding.SetActive(true);
    }
    public void CommunistRevolution(string color)
    {
        communistRevolutionModal.SetActive(true);
        communistRevolutionText.GetComponent<TextMeshProUGUI>().text = $@"Comrades of the glorious chessboard revolution,

Today, we stand at the precipice of a new dawn, a dawn where the workers of this grand chessboard rise against the tyranny of the bourgeois pieces. In the midst of our struggle, we have witnessed a pivotal moment. Three brave pawns have fallen in the line of duty, sacrificing themselves for the greater good of our {color} army.

These pawns, humble yet valiant, embody the spirit of our revolution. They are the unsung heroes who, with their unwavering dedication, have paved the way for our victory. Their sacrifice is not in vain, for it has weakened the grip of our adversaries and strengthened the resolve of our comrades.

In their honor, we must press on with even greater determination. The fall of these pawns serves as a reminder of the price of freedom and the necessity of our cause. We will not let their sacrifices be forgotten. We will march forward, side by side, and ensure that the ideals of our revolution are realized.

The {color} army stands united, ready to overcome any obstacle, ready to dismantle the structures of oppression piece by piece. We will not rest until every square on this board is liberated, until every piece enjoys the equality and justice that is their right.

Comrades, let us honor the fallen pawns by continuing our struggle with unwavering fervor. Let us remember their bravery as we advance towards our ultimate goal. Together, we will triumph, and the chessboard will be transformed into a land of true equality and comradeship.

Forward, to victory! For the fallen pawns! For the {color} army! For the revolution!";
        PlayAudio("Soviet Anthem");
        foreach (D20Roller roller in CRUpgradeModal.transform.GetComponentsInChildren<D20Roller>())
        {
            roller.Start();
            roller.Reset();
        }
    }
    public void ToggleAudio()
    {
        audioEnabled = !audioEnabled;
        if (audioEnabled) audioSources.transform.Find("Soviet Anthem").GetComponent<AudioSource>().volume = 1;
        else audioSources.transform.Find("Soviet Anthem").GetComponent<AudioSource>().volume = 0;
    }
    public void PlayAudio(string name)
    {
        audioSources.transform.Find(name).GetComponent<AudioSource>().Play();
    }
    public void ChangeD20UpgradeOK(bool x)
    {
        d20UpgradeOK.GetComponent<Button>().interactable = x;
    }

    public void FixPawnShopPrices()
    {
        if (religion.GetReligions(ct.currentPlayer).Contains("Knook"))
        {
            pawnShop.transform.Find("knook").gameObject.transform.Find("text").GetComponent<TextMeshProUGUI>().text = "12 Enjoyment (50% off)";
        }
        else pawnShop.transform.Find("knook").gameObject.transform.Find("text").GetComponent<TextMeshProUGUI>().text = "24 Enjoyment";
    }
    public void ChangePlayerIndicator()
    {
        playerIndicator.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = (ct.currentPlayer == "white" ? "White" : "Black") + " to play";
        playerIndicator.transform.Find("Square").GetComponent<Image>().color = ct.currentPlayer == "white" ? Color.white : Color.black;
    }

    public void ChangeMartinStars(int stars)
    {
        foreach (GameObject star in martinStars)
        {
            star.GetComponent<Image>().sprite = starEmpty;
        }
        for (int i = 0; i <= stars; i++)
        {
            martinStars[i].GetComponent<Image>().sprite = starFull;
        }
    }
}