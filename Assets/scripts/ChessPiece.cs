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

    public Sprite black_pawn, black_rook, black_knight, black_bishop, black_queen, black_king, black_knook;
    public Sprite white_pawn, white_rook, white_knight, white_bishop, white_queen, white_king, white_knook;
    public Sprite wallSprite;
    /*private void Start()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        UIControl = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIControl>();
    }*/

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
            case "black_king":
                sr.sprite = black_king; color = "black";
                isKing = true; break;

            case "white_pawn": sr.sprite = white_pawn; color = "white"; break;
            case "white_rook": sr.sprite = white_rook; color = "white"; break;
            case "white_knight": sr.sprite = white_knight; color = "white"; break;
            case "white_bishop": sr.sprite = white_bishop; color = "white"; break;
            case "white_queen": sr.sprite = white_queen; color = "white"; break;
            case "white_knook": sr.sprite = white_knook; color = "white"; break;
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
        }
    }
    void Update()
    {
        if (cursor != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            cursor.transform.position = new Vector3(mousePos.x, mousePos.y, -2.0f);
        }

    }
    public void CheckWalls()
    {
        Controller sc = controller.GetComponent<Controller>();
        bool horizontalRooks = CheckRook(position.x + 1, position.y) && CheckRook(position.x - 1, position.y);
        bool verticalRooks = CheckRook(position.x, position.y + 1) && CheckRook(position.x, position.y - 1);

        if (!horizontalRooks && !verticalRooks)
        {
            sc.SetPositionEmpty(position.x, position.y);
            Destroy(gameObject);
        }
    }
    public bool CheckRook(int x, int y)
    {
        Controller sc = controller.GetComponent<Controller>();
        if (sc.PositionWithinBounds(x, y) && sc.GetPosition(x, y) != null)
        {
            if (sc.GetPosition(x, y).GetComponent<ChessPiece>().name.Contains("rook"))
                return true;
        }

        return false;
    }
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
                transform.position = new Vector3((position.x - 3.5f), (position.y - 3.5f), -0.1f);

        }
    }

    public static Vector3 RotateBoard(Vector3 position)
    {
        return new Vector3(-position.x, -position.y, position.z);
    }

    private void OnMouseDown()
    {
        if (controller.GetComponent<Controller>().currentPlayer == color && !controller.GetComponent<Controller>().IsGameOver() && !controller.GetComponent<Controller>().isPaused)
        {
            DestroyMovePlates();

            cursor = Instantiate(cursorPiece, this.transform.position, Quaternion.identity);
            Activate(cursor);
            cursor.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.5f); // Make it semi-transparent

            InitiateMovePlates();
        }
    }

    private void OnMouseUp()
    {
        Destroy(GameObject.FindGameObjectWithTag("cursor"));
    }
    public static void DestroyMovePlates(bool destroyMoveIndicators = false)
    {
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
        foreach (GameObject movePlate in movePlates)
        {
            if (movePlate.GetComponent<MovePlate>().indicator != "move" || destroyMoveIndicators)
                Destroy(movePlate);

        }
    }
    public void InitiateMovePlates()
    {
        MovePlateSpawn(position.x, position.y, false, "selected");
        switch (this.name)
        {
            case "black_queen":
            case "white_queen":
                if (pregnantWith == "")
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
                if (this.position.x < 8)
                {
                    LineMovePlate(1, 1);
                    LineMovePlate(1, -1);
                    LineMovePlate(-1, 1);
                    LineMovePlate(-1, -1);
                }
                else
                    UIControl.DisplayAlert(14);
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

    public void PawnMovePlate(int x, int y)
    {
        bool promote = false;
        Controller sc = controller.GetComponent<Controller>();
        if (y == 0 || y == 7) promote = true;
        if (sc.PositionWithinBounds(x, y) && sc.GetPosition(x, y) == null)
        {
            if (sc.GetPosition(x, color == "white" ? y + 1 : y - 1) == null && firstMove)
            {
                MovePlateSpawn(x, color == "white" ? y + 1 : y - 1, false, "2squares");
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

    public void Promote(string piece)
    {
        Debug.Log(piece);
        this.name = piece;
        Activate(gameObject);
        po.SetActive(false);
        promoted = true;
    }
    public void KingPromote(string piece)
    {
        Debug.Log(piece + " king promote");
        UIControl.DisplayAlert(1);
        this.name = piece;
    }

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

    public void MovePlateSpawn(int x, int y, bool isAttack = false, string indicator = null)
    {
        Controller sc = controller.GetComponent<Controller>();
        GameObject mp = Instantiate(movePlate, new Vector3(x - 3.5f, y - 3.5f, -0.5f), Quaternion.identity);
        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.Reference = gameObject;
        mpScript.capturing = isAttack;
        mpScript.indicator = indicator;
        mpScript.SetPosition(x, y);
    }

}
