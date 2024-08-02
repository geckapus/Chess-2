using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ChessPiece : MonoBehaviour
{
    public GameObject controller;
    public GameObject movePlate;
    public GameObject cursorPiece;
    private GameObject cursor;
    public GameObject po;
    public bool canMove = true;
    public bool promoted = false;
    public bool isKing = false;
    public int daysOnVacation = 0;
    public bool hasWall = false;
    public UIControl UIControl;
    public int daysPregnant = 0;
    public int daysAfterBirth = 0;
    public string pregnantWith = "";

    private Vector2Int position = new Vector2Int(-1, -1);
    public Vector2Int previousPosition = new Vector2Int(-1, -1);
    public bool firstMove = true;
    public Vector2Int Position
    {
        get { return position; }
        set
        {
            position = value;
            SetTransform();
        }
    }
    public string color;

    public Sprite black_pawn, black_rook, black_knight, black_bishop, black_queen, black_king, black_knook, black_antipawn, black_lenin;
    public Sprite white_pawn, white_rook, white_knight, white_bishop, white_queen, white_king, white_knook, white_antipawn, white_lenin;
    public Sprite wallSprite;
    /*private void Start()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        UIControl = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIControl>();
    }*/
    /// <summary>
    /// Activates the chess piece by setting its sprite, color, and other properties based on its name.
    /// </summary>
    /// <param name="obj">The GameObject to activate.</param>
    public void Activate(GameObject obj)
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        UIControl = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIControl>();
        SetTransform();
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();

        switch (this.name)
        {
            case "black_pawn": sr.sprite = black_pawn; color = "black"; break;
            case "black_rook": sr.sprite = black_rook; color = "black"; break;
            case "black_knight": sr.sprite = black_knight; color = "black"; break;
            case "black_bishop": sr.sprite = black_bishop; color = "black"; break;
            case "black_queen": sr.sprite = black_queen; color = "black"; break;
            case "black_knook": sr.sprite = black_knook; color = "black"; break;
            case "black_antipawn": sr.sprite = black_antipawn; color = "black"; break;
            case "black_king":
                sr.sprite = black_king; color = "black";
                isKing = true; break;

            case "white_pawn": sr.sprite = white_pawn; color = "white"; break;
            case "white_rook": sr.sprite = white_rook; color = "white"; break;
            case "white_knight": sr.sprite = white_knight; color = "white"; break;
            case "white_bishop": sr.sprite = white_bishop; color = "white"; break;
            case "white_queen": sr.sprite = white_queen; color = "white"; break;
            case "white_knook": sr.sprite = white_knook; color = "white"; break;
            case "white_antipawn": sr.sprite = white_antipawn; color = "white"; break;
            case "white_king":
                sr.sprite = white_king; color = "white";
                isKing = true; break;

            case "black_enPassant":
                sr.sprite = black_pawn; color = "black";
                canMove = false;
                sr.color = new Color(0.0f, 0.0f, 0.0f, 0.3f);
                break;
            case "white_enPassant":
                sr.sprite = black_pawn; color = "white";
                canMove = false;
                sr.color = new Color(0.0f, 0.0f, 0.0f, 0.3f);
                break;
            case "wall":
                sr.sprite = wallSprite;
                color = "wall";
                break;
            case "black_lenin":
                sr.sprite = black_lenin;
                color = "black";
                break;
            case "white_lenin":
                sr.sprite = white_lenin;
                color = "white";
                break;
        }
    }
    /// <summary>
    /// Updates the position of the cursor if it is not null.
    /// </summary>
    void Update()
    {
        if (cursor != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            cursor.transform.position = new Vector3(mousePos.x, mousePos.y, -2.0f);
        }
    }
    /// <summary>
    /// Checks if the chess piece (which must be a wall) is surrounded by rooks on both sides horizontally and vertically.
    /// If not, the chess piece is removed from the board.
    /// </summary>
    public void CheckWalls()
    {
        Controller sc = controller.GetComponent<Controller>();
        bool horizontalRooks = CheckRook(position.x + 1, position.y) && CheckRook(position.x - 1, position.y);
        bool verticalRooks = CheckRook(position.x, position.y + 1) && CheckRook(position.x, position.y - 1);

        if (!horizontalRooks && !verticalRooks)
        {
            sc.RemovePieceAt(position);
        }
    }
    /// <summary>
    /// Checks if a given position contains a rook piece.
    /// </summary>
    /// <param name="x">The x-coordinate of the position to check.</param>
    /// <param name="y">The y-coordinate of the position to check.</param>
    /// <returns>Returns true if the position contains a rook piece, false otherwise.</returns>
    public bool CheckRook(int x, int y, string color = null)
    {
        Controller sc = controller.GetComponent<Controller>();
        if (sc.PositionWithinBounds(x, y) && sc.GetPosition(x, y) != null && sc.GetPosition(x, y).GetComponent<ChessPiece>().name.Contains("rook"))
            if (color == null || sc.GetPosition(x, y).GetComponent<ChessPiece>().color == color)
                return true;
        return false;
    }
    /// <summary>
    /// Checks if the given position contains a king of the specified color.
    /// </summary>
    /// <param name="x">The x-coordinate of the position to check.</param>
    /// <param name="y">The y-coordinate of the position to check.</param>
    /// <param name="color">The color of the king to check for.</param>
    /// <returns>Returns true if the position contains a king of the specified color, false otherwise.</returns>
    public bool CheckKing(int x, int y, string color)
    {
        Controller sc = controller.GetComponent<Controller>();
        if (sc.PositionWithinBounds(x, y) && sc.GetPosition(x, y) != null)
        {
            if (sc.GetPosition(x, y).GetComponent<ChessPiece>().isKing && sc.GetPosition(x, y).GetComponent<ChessPiece>().color == color)
                return true;
        }

        return false;
    }
    /// <summary>
    /// Sets the transform of the object based on the flip board setting and color.
    /// If the flip board setting is true and the color is black, the transform position is set to the negated position minus 3.5f.
    /// Otherwise, the transform position is set to the position minus 3.5f.
    /// If the name of the object is "black_enPassant", the transform position is set based on the flip board setting. (This is done because I had no idea what else to do, as black en passants were not working.)
    /// </summary>
    public void SetTransform()
    {
        if (GameObject.FindGameObjectWithTag("GameController").GetComponent<Controller>().settings["flip_board"] == "true" && color == "black")
        {
            transform.position = new Vector3(-(position.x - 3.5f), -(position.y - 3.5f), -0.1f);

        }
        else
            //StartCoroutine(controller.GetComponent<Controller>().MoveObject(gameObject, new Vector3(position.x - 3.5f, position.y - 3.5f, -0.1f), 0.3f));
            transform.position = new Vector3(position.x - 3.5f, position.y - 3.5f, -0.1f);
        if (this.name == "black_enPassant") //DEBUGGING BLACK EN PASSANT
        {
            if (GameObject.FindGameObjectWithTag("GameController").GetComponent<Controller>().settings["flip_board"] == "true")
            {
                transform.position = new Vector3(-(position.x - 3.5f), -(position.y - 3.5f), -0.1f);
            }
            else
                transform.position = new Vector3(position.x - 3.5f, position.y - 3.5f, -0.1f);

        }
    }
    public void FlipBoardFix()
    {
        if (GameObject.FindGameObjectWithTag("GameController").GetComponent<Controller>().settings["flip_board"] == "true")
        {
            transform.position = new Vector3(-(position.x - 3.5f), -(position.y - 3.5f), -0.1f);
        }
        else
            transform.position = new Vector3(position.x - 3.5f, position.y - 3.5f, -0.1f);
    }
    /// <summary>
    /// Rotates the board by flipping the x and y coordinates of the given position.
    /// </summary>
    /// <param name="position">The position to rotate.</param>
    /// <returns>The rotated position.</returns>
    public static Vector3 RotateBoard(Vector3 position)
    {
        return new Vector3(-position.x, -position.y, position.z);
    }
    /// <summary>
    /// Handles the mouse down event for the chess piece.
    /// If the current player is the same as the piece's color, and the game is not over, and the game is not paused,
    /// destroy any existing move plates, instantiate a cursor piece at the current position, activate the cursor,
    /// set the cursor's sprite color to semi-transparent, and initiate the move plates. THE CURSOR CURRENTLY HAS NO FUNCTIONALITY
    /// </summary>
    private void OnMouseDown()
    {
        if (!controller.GetComponent<Controller>().anishMode && controller.GetComponent<Controller>().currentPlayer == color && !controller.GetComponent<Controller>().IsGameOver() && !controller.GetComponent<Controller>().isPaused)
        {
            DestroyMovePlates();

            cursor = Instantiate(cursorPiece, this.transform.position, Quaternion.identity);
            Activate(cursor);
            cursor.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.5f); // Make it semi-transparent

            InitiateMovePlates();
        }
        else if (controller.GetComponent<Controller>().anishMode && controller.GetComponent<Controller>().currentPlayer != color && !controller.GetComponent<Controller>().IsGameOver() && !controller.GetComponent<Controller>().isPaused)
        {
            if (name.Contains("pawn"))
            {
                DestroyMovePlates(false, true);

                controller.GetComponent<Controller>().anishMovePlate = MovePlateSpawn(position.x, position.y, true, "anish");
            }
        }
    }
    private void OnMouseUp()
    {
        if (GameObject.FindGameObjectWithTag("cursor") != null)
        {
            Debug.Log("Mouse Up");
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null)
            {
                GameObject hitObject = hit.collider.gameObject;
                if (hitObject.name == "MovePlate(Clone)")
                {
                    hitObject.GetComponent<MovePlate>().OnMouseUp();
                }
                Debug.Log("Target Position: " + hitObject.transform.position);
            }
            Destroy(GameObject.FindGameObjectWithTag("cursor"));
        }
    }
    /// <summary>
    /// Destroys all move plates in the scene, optionally destroying move indicators. Move indicators are purely visual, and are not used to move any pieces.
    /// </summary>
    /// <param name="destroyMoveIndicators">If true, move indicators will also be destroyed.</param>
    public static void DestroyMovePlates(bool destroyMoveIndicators = false, bool destroyAnish = false)
    {
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
        foreach (GameObject movePlate in movePlates)
        {
            if ((movePlate.GetComponent<MovePlate>().indicator != "move" || destroyMoveIndicators) && (movePlate.GetComponent<MovePlate>().indicator != "anish" || destroyAnish))
            {
                Destroy(movePlate);
            }
        }
    }
    /// <summary>
    /// Initializes the move plates for the current chess piece.
    /// </summary>
    /// <remarks>
    /// This method spawns move plates based on the current chess piece's position and type.
    /// The move plates are used to determine the valid moves for the piece, and are used to move the piece when clicked.
    /// The method takes no parameters and does not return any values.
    /// </remarks>
    public void InitiateMovePlates()
    {
        MovePlateSpawn(position.x, position.y, false, "selected");
        switch (this.name)
        {
            case "black_queen":
            case "white_queen":
                if (pregnantWith == "") //If the queen is pregnant, it moves like a king
                {
                    LineMovePlate(1, 0);
                    LineMovePlate(0, 1);
                    LineMovePlate(1, 1);
                    LineMovePlate(-1, 0);
                    LineMovePlate(0, -1);
                    LineMovePlate(-1, -1);
                    LineMovePlate(-1, 1);
                    LineMovePlate(1, -1);
                }
                else SurroundMovePlate();
                break;
            case "black_knight":
            case "white_knight":
                LMovePlate();
                break;
            case "black_bishop":
            case "white_bishop":
                if (!controller.GetComponent<Controller>().PositionOnVacation(this.position.x, this.position.y)) // Bishops cannot move if on vacation
                {
                    LineMovePlate(1, 1);
                    LineMovePlate(1, -1);
                    LineMovePlate(-1, 1);
                    LineMovePlate(-1, -1);
                }
                else
                    UIControl.CreateAlert("Rule 14", "Bishops cannot come back from vacation");
                break;
            case "black_king":
            case "white_king":
                SurroundMovePlate();
                break;
            case "black_rook":
            case "white_rook":
                LineMovePlate(1, 0);
                LineMovePlate(0, 1);
                LineMovePlate(-1, 0);
                LineMovePlate(0, -1);
                break;
            case "black_knook":
            case "white_knook":
                LineMovePlate(1, 0);
                LineMovePlate(0, 1);
                LineMovePlate(-1, 0);
                LineMovePlate(0, -1);
                LMovePlate();
                break;
            case "black_pawn":
                PawnMovePlate(position.x, position.y - 1);
                break;
            case "white_pawn":
                PawnMovePlate(position.x, position.y + 1);
                break;
            case "wall":
                break;
        }
    }
    /// <summary>
    /// Generates walls for a rook.
    /// </summary>
    /// <remarks>
    /// This method checks if the name of the current object contains "rook".
    /// If it does, it calls the LineWallGenrator method with different arguments.
    /// </remarks>
    public void WallGenerator()
    {
        if (this.name.Contains("rook"))
        {
            LineWallGenrator(1, 0);
            LineWallGenrator(0, 1);
            LineWallGenrator(-1, 0);
            LineWallGenrator(0, -1);
        }

    }
    /// <summary>
    /// Generates a wall along a line in the chessboard.
    /// </summary>
    /// <param name="xInc">The increment in the x-coordinate of the line.</param>
    /// <param name="yInc">The increment in the y-coordinate of the line.</param>
    /// <remarks>
    /// This method checks if the position in the chessboard is within bounds and if the position is empty.
    /// If it is, it checks if the next position in the line is within bounds and if it contains a rook of the same color.
    /// If it does, it creates a wall at the current position and displays an alert.
    /// </remarks>
    private void LineWallGenrator(int xInc, int yInc)
    {
        Controller sc = controller.GetComponent<Controller>();
        int x = position.x + xInc;
        int y = position.y + yInc;
        if (sc.PositionWithinBounds(x, y) && sc.GetPosition(x, y) == null)
        {
            if (sc.PositionWithinBounds(x + xInc, y + yInc) && sc.GetPosition(x + xInc, y + yInc) != null)
            {
                if (sc.GetPosition(x + xInc, y + yInc).GetComponent<ChessPiece>().name == color + "_rook")
                {
                    sc.SetPosition(sc.Create("wall", x, y));
                    UIControl.DisplayAlert(2);
                }

            }
        }
    }
    /// <summary>
    /// Generates a move plate along a line in the chessboard.
    /// </summary>
    /// <param name="xIncrement">The increment in the x-coordinate of the line.</param>
    /// <param name="yIncrement">The increment in the y-coordinate of the line.</param>
    /// <remarks>
    /// This method checks if the position in the chessboard is within bounds and if the position is empty.
    /// If it is, it creates a move plate at the current position.
    /// It then increments the x and y coordinates by the given increments and repeats the process.
    /// If the position is not empty and contains a piece of a different color, it creates a capturing move plate at the current position.
    /// </remarks>
    public void LineMovePlate(int xIncrement, int yIncrement)
    {
        Controller cs = controller.GetComponent<Controller>();
        int x = position.x + xIncrement;
        int y = position.y + yIncrement;

        while (cs.PositionWithinBounds(x, y) && cs.GetPosition(x, y) == null)
        {
            MovePlateSpawn(x, y);
            x += xIncrement;
            y += yIncrement;
        }
        if (cs.PositionWithinBounds(x, y) && cs.GetPosition(x, y) != null && cs.GetPosition(x, y).GetComponent<ChessPiece>().color != this.color)
        {
            if (cs.GetPosition(x, y).GetComponent<ChessPiece>().color != "wall")
                MovePlateSpawn(x, y, true);
        }
    }
    /// <summary>
    /// Moves the chess piece in an L shape by spawning move plates at the specified coordinates.
    /// </summary>
    /// <remarks>
    /// This method calls the PointMovePlate method eight times to spawn move plates in an L shape.
    /// The move plates are used to determine the valid moves for the piece, and are used to move the piece when clicked.
    /// </remarks>
    public void LMovePlate()
    {
        PointMovePlate(position.x + 1, position.y + 2);
        PointMovePlate(position.x - 1, position.y + 2);
        PointMovePlate(position.x + 2, position.y + 1);
        PointMovePlate(position.x + 2, position.y - 1);
        PointMovePlate(position.x + 1, position.y - 2);
        PointMovePlate(position.x - 1, position.y - 2);
        PointMovePlate(position.x - 2, position.y + 1);
        PointMovePlate(position.x - 2, position.y - 1);
    }
    /// <summary>
    /// Generates move plates around the current chess piece to indicate valid moves.
    /// If the piece is a king and it's the first move, it also checks for the possibility of a castle move.
    /// </summary>
    public void SurroundMovePlate()
    {
        Controller sc = controller.GetComponent<Controller>();
        PointMovePlate(position.x, position.y + 1);
        PointMovePlate(position.x, position.y - 1);
        PointMovePlate(position.x - 1, position.y);
        PointMovePlate(position.x - 1, position.y - 1);
        PointMovePlate(position.x - 1, position.y + 1);
        PointMovePlate(position.x + 1, position.y);
        PointMovePlate(position.x + 1, position.y - 1);
        PointMovePlate(position.x + 1, position.y + 1);
        //kingside castle
        if (sc.PositionWithinBounds(position.x + 3, position.y))
        {
            GameObject kingsideRook = sc.GetPosition(position.x + 3, position.y);
            if (kingsideRook != null)
            {
                if (firstMove && sc.GetPosition(position.x + 1, position.y) == null && sc.GetPosition(position.x + 2, position.y) == null && kingsideRook.GetComponent<ChessPiece>().firstMove && kingsideRook.GetComponent<ChessPiece>().name == color + "_rook")
                {
                    MovePlateSpawn(position.x + 2, position.y, false, "castleKingSide");
                }
            }
        }

        if (sc.PositionWithinBounds(position.x - 4, position.y))
        {
            GameObject queensideRook = sc.GetPosition(position.x - 4, position.y);
            if (queensideRook != null)
            {
                if (firstMove && sc.GetPosition(position.x - 1, position.y) == null && sc.GetPosition(position.x - 2, position.y) == null && sc.GetPosition(position.x - 3, position.y) == null && queensideRook.GetComponent<ChessPiece>().firstMove && queensideRook.GetComponent<ChessPiece>().name == color + "_rook")
                {
                    MovePlateSpawn(position.x - 2, position.y, false, "castleQueenSide");
                }
            }
        }


    }
    /// <summary>
    /// Generates a move plate at the specified coordinates. If the position is within the bounds of the board, it checks if there is a chess piece at that position. If there is no chess piece, it spawns a move plate. If there is a chess piece, it checks if it is of a different color and not a wall. If it is, it spawns a move plate with the option to capture the piece.
    /// </summary>
    /// <param name="x">The x-coordinate of the move plate.</param>
    /// <param name="y">The y-coordinate of the move plate.</param>
    public void PointMovePlate(int x, int y)
    {
        Controller sc = controller.GetComponent<Controller>();
        if (sc.PositionWithinBounds(x, y))
        {
            GameObject cp = sc.GetPosition(x, y);

            if (cp == null)
            {
                MovePlateSpawn(x, y);
            }
            else if (cp.GetComponent<ChessPiece>().color != color)
            {
                if (cp.GetComponent<ChessPiece>().color != "wall")
                    MovePlateSpawn(x, y, true);
            }
        }
    }
    /// <summary>
    /// Generates move plates for a pawn piece based on the specified coordinates.
    /// If it is the first move, it spawns a move plate for a two-square advance. 
    /// It also spawns a move plate for promotion if the pawn reaches the first or eighth rank.
    /// 
    /// If there is a piece next to the pawn, it checks if it is of a different color and not a wall.
    /// If it is, it spawns a move plate with the option to capture the piece.
    /// </summary>
    /// <param name="x">The x-coordinate of the pawn.</param>
    /// <param name="y">The y-coordinate of the pawn.</param>
    public void PawnMovePlate(int x, int y)
    {
        bool promote = false;
        Controller sc = controller.GetComponent<Controller>();
        if (y == 0 || y == 7) promote = true;
        if (sc.PositionWithinBounds(x, y) && sc.GetPosition(x, y) == null)
        {
            // Check double square move for first move
            int oneSquareY = color == "white" ? y + 1 : y - 1;
            if (firstMove && sc.GetPosition(x, oneSquareY) == null && sc.PositionWithinBounds(x, oneSquareY))
            {
                MovePlateSpawn(x, oneSquareY, false, "2squares");
            }
            MovePlateSpawn(x, y, false, promote ? "promote" : null);
        }

        if (sc.PositionWithinBounds(x + 1, y) && sc.GetPosition(x + 1, y) != null && sc.GetPosition(x + 1, y).GetComponent<ChessPiece>().color != color)
        {
            if (sc.GetPosition(x + 1, y).GetComponent<ChessPiece>().color != "wall")
                MovePlateSpawn(x + 1, y, true, promote ? "promote" : null);
        }

        if (sc.PositionWithinBounds(x - 1, y) && sc.GetPosition(x - 1, y) != null && sc.GetPosition(x - 1, y).GetComponent<ChessPiece>().color != color)
        {
            if (sc.GetPosition(x - 1, y).GetComponent<ChessPiece>().color != "wall")
                MovePlateSpawn(x - 1, y, true, promote ? "promote" : null);
        }
    }
    /// <summary>
    /// Promotes the chess piece by setting its name to the specified piece, activating it, deactivating the promotion overlay,
    /// and marking the piece as promoted.
    /// </summary>
    /// <param name="piece">The name of the piece to promote to.</param>
    public void Promote(string piece)
    {
        Debug.Log(piece);
        this.name = piece;
        Activate(gameObject);
        po.SetActive(false);
        promoted = true;
    }
    /// <summary>
    /// Promotes the king to a given chess piece by setting its name to the specified piece,
    /// activating the UI alert, and updating the king's name. This is used when the king eats a piece, as per rule 1.
    /// </summary>
    /// <param name="piece">The name of the piece to promote to.</param>
    public void KingPromote(string piece)
    {
        Debug.Log(piece + " king promote");
        UIControl.DisplayAlert(1);
        this.name = piece;
    }
    /// <summary>
    /// Shows the promotion menu for a pawn, and customized the sprite based on the color of the pawn.
    /// </summary>
    public void ShowPromoteMenu()
    {
        po = GameObject.FindWithTag("Promotion Options").transform.Find("Promotion Menu").gameObject;
        po.SetActive(true);
        if (color == "white")
        {
            po.transform.Find("QueenPromote").GetComponent<UnityEngine.UI.Image>().sprite = white_queen;
            po.transform.Find("QueenPromote").GetComponent<Button>().onClick.AddListener(() => Promote("white_queen"));
            po.transform.Find("RookPromote").GetComponent<UnityEngine.UI.Image>().sprite = white_rook;
            po.transform.Find("RookPromote").GetComponent<Button>().onClick.AddListener(() => Promote("white_rook"));
            po.transform.Find("BishopPromote").GetComponent<UnityEngine.UI.Image>().sprite = white_bishop;
            po.transform.Find("BishopPromote").GetComponent<Button>().onClick.AddListener(() => Promote("white_bishop"));
            po.transform.Find("KnightPromote").GetComponent<UnityEngine.UI.Image>().sprite = white_knight;
            po.transform.Find("KnightPromote").GetComponent<Button>().onClick.AddListener(() => Promote("white_knight"));
        }
        else
        {
            po.transform.Find("QueenPromote").GetComponent<UnityEngine.UI.Image>().sprite = black_queen;
            po.transform.Find("QueenPromote").GetComponent<Button>().onClick.AddListener(() => Promote("black_queen"));
            po.transform.Find("RookPromote").GetComponent<UnityEngine.UI.Image>().sprite = black_rook;
            po.transform.Find("RookPromote").GetComponent<Button>().onClick.AddListener(() => Promote("black_rook"));
            po.transform.Find("BishopPromote").GetComponent<UnityEngine.UI.Image>().sprite = black_bishop;
            po.transform.Find("BishopPromote").GetComponent<Button>().onClick.AddListener(() => Promote("black_bishop"));
            po.transform.Find("KnightPromote").GetComponent<UnityEngine.UI.Image>().sprite = black_knight;
            po.transform.Find("KnightPromote").GetComponent<Button>().onClick.AddListener(() => Promote("black_knight"));
        }
    }
    public (GameObject left, GameObject right) GetDiagonalPieces(string piece)
    {
        Controller sc = controller.GetComponent<Controller>();
        int y = position.y + (color == "white" ? 1 : -1);
        string oppositeColor = color == "white" ? "black" : "white";
        GameObject leftPiece = null;
        GameObject rightPiece = null;
        if (sc.PositionWithinBounds(position.x - 1, y))
        {
            GameObject left = sc.GetPosition(position.x - 1, y);
            if (left != null && left.GetComponent<ChessPiece>().color != color && left.GetComponent<ChessPiece>().name == oppositeColor + piece)
            {
                leftPiece = left;
            }
        }
        if (sc.PositionWithinBounds(position.x + 1, y))
        {
            GameObject right = sc.GetPosition(position.x + 1, y);
            if (right != null && right.GetComponent<ChessPiece>().color != color && right.GetComponent<ChessPiece>().name == oppositeColor + piece)
            {
                rightPiece = right;
            }
        }
        return (leftPiece, rightPiece);
    }
    /// <summary>
    /// Spawns a move plate at the specified coordinates.
    /// </summary>
    /// <param name="x">The x-coordinate of the move plate.</param>
    /// <param name="y">The y-coordinate of the move plate.</param>
    /// <param name="isAttack">Indicates if the move plate represents an attack.</param>
    /// <param name="indicator">The indicator for the move plate.</param>
    public GameObject MovePlateSpawn(int x, int y, bool isAttack = false, string indicator = null)
    {
        Controller sc = controller.GetComponent<Controller>();
        GameObject mp = Instantiate(movePlate, new Vector3(x - 3.5f, y - 3.5f, -0.5f), Quaternion.identity);
        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.Reference = gameObject;
        mpScript.capturing = isAttack;
        mpScript.indicator = indicator;
        mpScript.SetPosition(x, y);
        return mp;
    }
}
