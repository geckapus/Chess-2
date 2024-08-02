using System.Collections;
using UnityEngine;

public class MovePlate : MonoBehaviour
{
    public GameObject Reference { set; get; }

    public Vector2Int position;
    public bool capturing = false;
    public string indicator = null;
    private ChessPiece rf;
    private Controller sc;
    private UIControl uiControl;

    private void Start()
    {
        uiControl = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIControl>();
        rf = Reference.GetComponent<ChessPiece>();
        sc = GameObject.FindGameObjectWithTag("GameController").GetComponent<Controller>();
        if (sc.settings["flip_board"] == "true" && sc.currentPlayer == "black")
        {
            this.transform.position = ChessPiece.RotateBoard(this.transform.position);
        }
        if (sc.settings["showMovePlates"] == "true") ShowMovePlates(false);
        else ShowMovePlates(true);
        if (capturing)
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        if (indicator == "selected")
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 0.5f);
            gameObject.GetComponent<SpriteRenderer>().color = new Color(0.0f, 1.0f, 0.0f, 1.0f);
        }
        if (indicator == "move")
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 0.5f);
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }
    }
    public void ShowMovePlates(bool x)
    {
        this.GetComponent<SpriteRenderer>().enabled = x;
    }
    private void Update()
    {
        ShowMovePlates(sc.settings["showMovePlates"] == "true");
        if (indicator == "selected")
            ShowMovePlates(true);
        if (indicator == "move")
            ShowMovePlates(true);
    }
    /// <summary>
    /// Handles the mouse up event for the MovePlate object.
    /// This is activated when a moveplate is clicked on.
    /// If capturing is true, it checks if the captured object is a king and performs a dexterity saving throw.
    /// If the captured object is an "enPassant" piece, it updates the pawn count and destroys the associated pawn.
    /// If the king captures an object, it promotes the king's moveset to whatever it captured.
    /// If the captured object is a pawn, it updates the pawn count.
    /// It then updates the pawn count UI, destroys the captured object, and sets the position empty.
    /// If the indicator is not null, it performs a move action based on the indicator type.
    /// If the indicator is "2squares", it creates an "enPassant" piece at the new position.
    /// If the indicator is "promote", it shows the promotion UI and waits for an input before continuing the game.
    /// If the indicator contains "castle", it performs a castle action
    /// </summary>
    public void OnMouseUp()
    {
        if ((indicator == null || indicator == "2squares" || indicator == "promote" || indicator.Contains("castle")) && !sc.GetComponent<Controller>().isPaused)
        {
            sc.halfMove += 1;
            if (capturing)
            {
                GameObject cp = sc.GetPosition(position.x, position.y);
                if (cp.GetComponent<ChessPiece>().color == "white") sc.whiteCapturedPiece = true;
                else sc.blackCapturedPiece = true;
                if (cp.GetComponent<ChessPiece>().isKing)
                {
                    int roll = Random.Range(1, 21);
                    if (roll == 20)
                    {
                        uiControl.ChangeSavingThrow(true, roll, "Natural 20! Killing all checking pieces.");
                        Debug.Log("Natural 20! Killing all checking pieces.");
                        cp = rf.gameObject;
                    }
                    else if (roll >= 15)
                    {
                        uiControl.ChangeSavingThrow(true, roll, "Saving throw succeeded! You get an extra move to escape checkmate.");
                        Debug.Log("Saving throw succeeded! Escaping checkmate.");
                        sc.NextTurn();
                        return;
                    }
                    else
                    {
                        sc.gameOver = true;
                        uiControl.ChangeSavingThrow(true, roll, "Saving throw failed. Checkmate stands.");
                        Debug.Log("Saving throw failed. Checkmate stands. starting coroutine.");
                    }
                }
                else if (cp.name.Contains("enPassant"))
                {
                    if (cp.GetComponent<ChessPiece>().color == "white") sc.whitePawnsLost++;
                    else sc.blackPawnsLost++;
                    Destroy(sc.GetPosition(position.x, position.y + (rf.color == "white" ? -1 : 1)));
                    sc.SetPositionEmpty(position.x, position.y + (rf.color == "white" ? -1 : 1));
                }
                else if (rf.GetComponent<ChessPiece>().isKing)
                {
                    rf.KingPromote(rf.color + cp.name[cp.name.IndexOf("_")..]);
                }
                else if (cp.name.Contains("pawn"))
                {
                    if (cp.GetComponent<ChessPiece>().color == "white") sc.whitePawnsLost++;
                    else sc.blackPawnsLost++;
                }
                uiControl.UpdatePawnsLostCounter();
                Destroy(cp);
                sc.SetPositionEmpty(position.x, position.y);
                sc.halfMove = 0;
                uiControl.PlayAudio("capture");
            }
            else uiControl.PlayAudio("move");

            //Making a move

            sc.SetPositionEmpty(rf.Position.x, rf.Position.y);

            rf.firstMove = false;


            if (indicator == "castleKingSide")
            {
                sc.SetPosition(sc.Create(rf.color + "_rook", 5, position.y));
                Destroy(sc.GetPosition(7, position.y));
                sc.SetPositionEmpty(7, position.y);
            }
            if (indicator == "castleQueenSide")
            {
                sc.SetPosition(sc.Create(rf.color + "_rook", 3, position.y));
                Destroy(sc.GetPosition(0, position.y));
                sc.SetPositionEmpty(0, position.y);
            }

            if (indicator == "promote" && !rf.isKing)
            {
                rf.ShowPromoteMenu();
                StartCoroutine(WaitForPromotion());
            }
            else
            {
                rf.previousPosition = rf.Position;
                //StartCoroutine(sc.MoveObject(Reference, rf.transform.position, 0.3f));
                rf.Position = position;

                sc.SetPosition(Reference);
                sc.NextTurn();

                rf.MovePlateSpawn(rf.previousPosition.x, rf.previousPosition.y, false, "move");
                rf.MovePlateSpawn(rf.Position.x, rf.Position.y, false, "move");
                rf.WallGenerator();
            }
            if (indicator == "2squares")
            {
                GameObject enPassant = sc.Create(rf.color + "_enPassant", position.x, position.y + (rf.color == "white" ? -1 : 1));
                sc.SetPosition(enPassant);
                ChessPiece enPassantPiece = enPassant.GetComponent<ChessPiece>();
                enPassantPiece.FlipBoardFix();

                (GameObject left, GameObject right) = enPassantPiece.GetDiagonalPieces("_pawn");
                if (sc.settings["forceEnPassant"] == "true")
                {
                    if (left != null && right != null) sc.DoubleEnPassant(left, right, enPassant);
                    else if (left != null) sc.ForceEnPassant(left);
                    else if (right != null) sc.ForceEnPassant(right);

                    //sc.RemovePieceAt(new Vector2Int(rf.Position.x, rf.Position.y));
                }
            }

            if (rf.name.Contains("pawn"))
                sc.halfMove = 0;
        }
    }
    /// <summary>
    /// Waits for the promotion of a chess piece to complete. This coroutine was needed for when a pawn promotes, as the game needs to wait for the user to make a selection before the game continues.
    /// </summary>
    /// <returns>An enumerator that waits for the promotion to complete.</returns>
    private IEnumerator WaitForPromotion()
    {
        // Wait for the first frame to ensure the promoted bool is set
        yield return null;


        while (!rf.promoted)
        {
            yield return null; // Pause the coroutine and wait for the next frame
        }

        // Continue with the rest of the code after the promotion is complete
        rf.previousPosition = rf.Position;
        //StartCoroutine(sc.MoveObject(Reference, rf.transform.position, 0.3f));
        rf.Position = position;

        sc.SetPosition(Reference);
        sc.NextTurn();

        rf.MovePlateSpawn(rf.previousPosition.x, rf.previousPosition.y, false, "move");
        rf.MovePlateSpawn(rf.Position.x, rf.Position.y, false, "move");
        rf.WallGenerator();
    }

    public void SetPosition(int x, int y)
    {
        position = new Vector2Int(x, y);
    }
}
