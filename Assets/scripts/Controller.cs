using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    public GameObject chessPiece;
    private GameObject[,] board = new GameObject[10, 8];
    private GameObject[] playerBlack = new GameObject[16];
    private GameObject[] playerWhite = new GameObject[16];
    public GameObject gameOverText;
    public GameObject canvas;
    private UIControl uiControl;
    public Dictionary<string, string> settings = new Dictionary<string, string> { { "flip_board", "false" }, { "showMovePlates", "true" } };

    public string currentPlayer = "white";
    private bool gameOver = false;
    public bool isPaused;
    public int whiteEnjoyment = 0;
    public int blackEnjoyment = 0;
    public void ToggleFlipBoard()
    {
        settings["flip_board"] = settings["flip_board"] == "false" ? "true" : "false";
        FlipBoard();
    }
    public void ToggleShowMovePlates()
    {
        settings["showMovePlates"] = settings["showMovePlates"] == "false" ? "true" : "false";
    }
    public void FlipBoard()
    {
        foreach (GameObject piece in board)
        {
            if (piece != null && currentPlayer == "black")
                StartCoroutine(MoveObject(piece, ChessPiece.RotateBoard(piece.transform.position), 0.3f));
        }
        foreach (GameObject movePlate in GameObject.FindGameObjectsWithTag("MovePlate"))
        {
            if (currentPlayer == "black")
                StartCoroutine(MoveObject(movePlate, ChessPiece.RotateBoard(movePlate.transform.position), 0.3f));
        }
    }

    public void Start()
    {
        uiControl = canvas.GetComponent<UIControl>();
        playerWhite = new GameObject[] { Create("white_rook",  0, 0), Create("white_knight",1, 0),
            Create("white_bishop",2, 0), Create("white_queen", 3, 0), Create("white_king",  4, 0),
            Create("white_bishop",5, 0), Create("white_knight",6, 0), Create("white_rook",  7, 0),
            Create("white_pawn",  0, 1), Create("white_pawn",  1, 1), Create("white_pawn",  2, 1),
            Create("white_pawn",  3, 1), Create("white_pawn",  4, 1), Create("white_pawn",  5, 1),
            Create("white_pawn",  6, 1), Create("white_pawn",  7, 1) };

        playerBlack = new GameObject[] { Create("black_rook",   0, 7), Create("black_knight", 1, 7),
            Create("black_bishop",2, 7), Create("black_queen", 3, 7), Create("black_king",   4, 7),
            Create("black_bishop",5, 7), Create("black_knight",6, 7), Create("black_rook",   7, 7),
            Create("black_pawn",  0, 6), Create("black_pawn",  1, 6), Create("black_pawn",   2, 6),
            Create("black_pawn",  3, 6), Create("black_pawn",  4, 6), Create("black_pawn",   5, 6),
            Create("black_pawn",  6, 6), Create("black_pawn",  7, 6) };

        //Set all piece positions on the positions board
        for (int i = 0; i < playerBlack.Length; i++)
        {
            SetPosition(playerBlack[i]);
            SetPosition(playerWhite[i]);
        }
    }
    public GameObject Create(string name, int x, int y)
    {
        GameObject obj = Instantiate(chessPiece, new Vector3(x - 3.5f, y - 3.5f, -1.0f), Quaternion.identity);
        ChessPiece cp = obj.GetComponent<ChessPiece>();
        cp.name = name;
        cp.Position = new Vector2Int(x, y);
        cp.Activate(obj);
        return obj;
    }

    public void SetPosition(GameObject obj)
    {
        board[obj.GetComponent<ChessPiece>().Position.x, obj.GetComponent<ChessPiece>().Position.y] = obj;
    }

    public void SetPositionEmpty(int x, int y)
    {
        Debug.Log(board[x, y]);
        board[x, y] = null;
        Debug.Log(board[x, y]);
    }

    public GameObject GetPosition(int x, int y)
    {
        return board[x, y];
    }
    public bool PositionWithinBounds(int x, int y)
    {
        return (x >= 0 && x < 8 && y >= 0 && y < 8) || (x == 9 && y == 6) || PositionOnVacation(x, y);
    }
    public bool PositionOnVacation(int x, int y)
    {
        return (x == 8 && y == 3) || (x == 8 && y == 4);
    }

    public string GetCurrentPlayer()
    {
        return this.currentPlayer;
    }

    public bool IsGameOver()
    {
        return this.gameOver;
    }

    public void NextTurn()
    {
        this.currentPlayer = (this.currentPlayer == "white") ? "black" : "white";
        ChessPiece.DestroyMovePlates(true);
        foreach (GameObject piece in board)
        {
            if (piece != null)
            {
                ChessPiece cp = piece.GetComponent<ChessPiece>();
                if (settings["flip_board"] == "true")
                    StartCoroutine(MoveObject(piece, ChessPiece.RotateBoard(piece.transform.position), 0.3f));
                if (piece.name == currentPlayer + "_enPassant")
                {
                    Destroy(piece);
                    SetPositionEmpty(cp.Position.x, cp.Position.y);
                }
                if (piece.name == "wall")
                    cp.CheckWalls();
                if (PositionOnVacation(cp.Position.x, cp.Position.y))
                {

                    if (currentPlayer == "black" && cp.color == "white")
                    {
                        whiteEnjoyment += 2;
                        cp.daysOnVacation++;
                    }
                    else if (currentPlayer == "white" && cp.color == "black")
                    {
                        blackEnjoyment += 2;
                        cp.daysOnVacation++;
                    }
                    if (cp.name.Contains("king") && cp.daysOnVacation == 3)
                    {
                        uiControl.DisplayAlert(14);
                        GameOver(cp.color == "black" ? "white" : "black");
                    }
                    if (cp.name.Contains("knight"))
                    {
                        if (cp.Position.y == 3 && cp.CheckRook(8, 4))
                        {
                            Destroy(board[8, 3]);
                            Destroy(board[8, 4]);
                            SetPositionEmpty(8, 4);
                            SetPositionEmpty(8, 3);
                            SetPosition(Create(cp.color + "_knook", 8, 3));
                        }
                        else if (cp.Position.y == 4 && cp.CheckRook(8, 3))
                        {
                            Destroy(board[8, 3]);
                            Destroy(board[8, 4]);
                            SetPositionEmpty(8, 4);
                            SetPositionEmpty(8, 3);
                            SetPosition(Create(cp.color + "_knook", 8, 4));
                        }

                    }
                }
            }
        }
        uiControl.UpdateEnjoymentCounter();
    }
    public IEnumerator MoveObject(GameObject piece, Vector3 newPosition, float duration)
    {
        // Get the current position of the piece
        Vector3 currentPosition = piece.transform.position;
        // Calculate the time elapsed
        float elapsedTime = 0.0f;
        // Start the movement
        while (elapsedTime < duration && piece != null)
        {
            // Calculate the interpolation factor
            float t = elapsedTime / duration;
            // Interpolate the position of the piece
            piece.transform.position = Vector3.Lerp(currentPosition, newPosition, t);
            // Increment the elapsed time
            elapsedTime += Time.deltaTime;
            // Wait for the next frame
            yield return null;
        }
        // Set the final position of the piece
        if (piece != null) piece.transform.position = newPosition;
    }

    private void Update()
    {
        if (gameOver && Input.GetMouseButtonDown(1))
        {
            Debug.Log("Restart");
            gameOver = false;
            SceneManager.LoadScene("Game");
        }
    }

    public void GameOver(string winner)
    {
        gameOver = true;
        Debug.Log("Winner: " + winner);
        gameOverText.SetActive(true);
        gameOverText.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = (winner == "white" ? "White" : "Black") + " Wins!";
        string quote = "Did it feel good actually having to take the king before winning? Don't you agree it should have always been this way?";
        switch (UnityEngine.Random.Range(0, 10))
        {
            case 0:
                quote = "Google En Passant";
                break;
            case 1:
                quote = "Holy Hell!";
                break;
            case 2:
                quote = "New response just dropped!";
                break;
            case 3:
                quote = "Actual Zombie!";
                break;
            case 4:
                quote = "Call the exorcist!";
                break;
            case 5:
                quote = "New response just dropped!";
                break;
            case 6:
                quote = "Bishop goes on vacation, never comes back";
                break;
            case 7:
                quote = "Pawn storm incoming!";
                break;
            case 8:
                quote = "King sacrifice anyone?";
                break;
            case 9:
                quote = "Rook in the corner, plotting world domination";
                break;
            case 10:
                quote = "Ignite the Chessboard!";
                break;
        }
        gameOverText.transform.Find("Quote").GetComponent<TextMeshProUGUI>().text = "\"" + quote + "\"";
    }
}
