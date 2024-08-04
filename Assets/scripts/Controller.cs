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
    private Religion religion;
    private UIControl uiControl;
    public GameObject flash;
    //Settings
    public Dictionary<string, string> settings = new() { { "flip_board", "false" }, { "showMovePlates", "true" }, { "forceEnPassant", "true" } };
    public Dictionary<string, int> piecePrices = new() { { "pawn", 3 }, { "rook", 15 }, { "knight", 9 }, { "bishop", 9 }, { "queen", 27 } };

    public string currentPlayer = "white";
    public bool gameOver = false;
    public bool isPaused;
    public bool anishMode = false;
    public bool catchGukesh = false;
    public bool anishValidWhite = true;
    public bool anishValidBlack = true;
    public bool gukeshValidWhite = true;
    public bool gukeshValidBlack = true;
    public GameObject anishMovePlate;
    public int whiteEnjoyment = 0;
    public int blackEnjoyment = 0;
    public int blackPawnsLost = 0;
    public int whitePawnsLost = 0;
    public bool whiteCapturedPiece = false;
    public bool blackCapturedPiece = false;
    public int move = 1;
    public int halfMove = 0;
    /// <summary>
    /// Toggles the flip board setting.
    /// </summary>
    public void ToggleFlipBoard()
    {
        settings["flip_board"] = settings["flip_board"] == "false" ? "true" : "false";
        FlipBoard();
    }
    /// <summary>
    /// Toggles the "showMovePlates" setting in the settings dictionary.
    /// </summary>
    public void ToggleShowMovePlates()
    {
        settings["showMovePlates"] = settings["showMovePlates"] == "false" ? "true" : "false";
    }
    public void ToggleForceEnPassant()
    {
        settings["forceEnPassant"] = settings["forceEnPassant"] == "false" ? "true" : "false";
    }
    /// <summary>
    /// Flips the transforms of all elements on the board if the flip board setting is true, and the current player is black.
    /// </summary>
    /// <returns>A coroutine that moves each piece and move plate to the flipped position.</returns>
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
        uiControl.ToggleTileMap();
    }
    /// <summary>
    /// Initializes the game by creating the playerWhite and playerBlack arrays, and setting the positions of all pieces on the board.
    /// </summary>
    public void Start()
    {
        religion = GameObject.FindGameObjectWithTag("Religion").GetComponent<Religion>();
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
    /// <summary>
    /// Creates a new game object for a chess piece with the given name, position, and activates it.
    /// </summary>
    /// <param name="name">The name of the chess piece.</param>
    /// <param name="x">The x-coordinate of the chess piece's position.</param>
    /// <param name="y">The y-coordinate of the chess piece's position.</param>
    /// <returns>The newly created game object.</returns>
    public GameObject Create(string name, int x, int y)
    {
        GameObject obj = Instantiate(chessPiece, new Vector3(x - 3.5f, y - 3.5f, -1.0f), Quaternion.identity);
        ChessPiece cp = obj.GetComponent<ChessPiece>();
        cp.name = name;
        cp.Position = new Vector2Int(x, y);
        cp.Activate(obj);
        return obj;
    }
    /// <summary>
    /// Sets the position of a game object in the board based on its ChessPiece component's position.
    /// </summary>
    /// <param name="obj">The game object to set the position for.</param>
    public void SetPosition(GameObject obj)
    {
        board[obj.GetComponent<ChessPiece>().Position.x, obj.GetComponent<ChessPiece>().Position.y] = obj;
    }
    /// <summary>
    /// Sets the position on the board to be empty by setting the value at the specified coordinates to null.
    /// </summary>
    /// <param name="x">The x-coordinate of the position to set to empty.</param>
    /// <param name="y">The y-coordinate of the position to set to empty.</param>
    public void SetPositionEmpty(int x, int y)
    {
        board[x, y] = null;
    }
    /// <summary>
    /// Retrieves the game object at the specified position on the board.
    /// </summary>
    /// <param name="x">The x-coordinate of the position.</param>
    /// <param name="y">The y-coordinate of the position.</param>
    /// <returns>The game object at the specified position, or null if the position is out of bounds.</returns>
    public GameObject GetPosition(int x, int y)
    {
        return board[x, y];
    }
    /// <summary>
    /// Checks if the given position is within the bounds of the chessboard or on vacation.
    /// </summary>
    /// <param name="x">The x-coordinate of the position.</param>
    /// <param name="y">The y-coordinate of the position.</param>
    /// <returns>True if the position is within bounds or on vacation, false otherwise.</returns>
    public bool PositionWithinBounds(int x, int y)
    {
        return ((x >= 0 && x < 8 && y >= 0 && y < 8) || (x == 9 && y == 6) || PositionOnVacation(x, y)) && !(x == 1 && y == 3);
    }
    /// <summary>
    /// Checks if the given position is on vacation.
    /// </summary>
    /// <param name="x">The x-coordinate of the position.</param>
    /// <param name="y">The y-coordinate of the position.</param>
    /// <returns>True if the position is on vacation, false otherwise.</returns>
    public bool PositionOnVacation(int x, int y)
    {
        return (x == 8 && y == 3) || (x == 8 && y == 4);
    }
    /// <summary>
    /// Checks if the given position is in the nursery.
    /// </summary>
    /// <param name="x">The x-coordinate of the position.</param>
    /// <param name="y">The y-coordinate of the position.</param>
    /// <returns>True if the position is in the nursery, false otherwise.</returns>
    public bool PositionInNursery(int x, int y)
    {
        return (x == 9 && y == 3) || (x == 9 && y == 4);
    }

    public string GetCurrentPlayer()
    {
        return this.currentPlayer;
    }

    public bool IsGameOver()
    {
        return this.gameOver;
    }
    /// <summary>
    /// Switches the current player to the other player and performs various actions based on the state of the game.
    /// </summary>
    public void NextTurn()
    {
        catchGukesh = false;
        if (anishMovePlate != null) // Destroy the anish move plate and the stolen pawn
        {
            Debug.Log(anishValidWhite + " " + anishValidBlack + " " + (currentPlayer == "white" ? anishValidWhite : anishValidBlack));
            if (currentPlayer == "white") anishValidWhite = false;
            else anishValidBlack = false;
            RemovePieceAt(anishMovePlate.GetComponent<MovePlate>().position);
            Destroy(anishMovePlate);
            catchGukesh = true;
        }
        this.currentPlayer = (this.currentPlayer == "white") ? "black" : "white"; // Change player
        ChessPiece.DestroyMovePlates(true); // Destroy all move plates, even move indicators
        Debug.Log("white " + gukeshValidWhite + " black " + gukeshValidBlack + " " + (currentPlayer == "white" ? gukeshValidWhite : gukeshValidBlack));
        if (currentPlayer == "white" ? gukeshValidWhite : gukeshValidBlack) uiControl.ChangeGukeshButton(true);
        else uiControl.ChangeGukeshButton(false);
        if (currentPlayer == "white" ? anishValidWhite : anishValidBlack) uiControl.ChangeAnishButton(true, false);
        else uiControl.ChangeAnishButton(false, false);
        if (currentPlayer == "white") move++;
        if ((currentPlayer == "white" ? whitePawnsLost : blackPawnsLost) >= 3) CommunistRevolution();
        religion.NextTurn();
        uiControl.UpdateMoveCounter();
        foreach (GameObject piece in board) //Go through all pieces
        {
            if (piece != null)
            {
                ChessPiece cp = piece.GetComponent<ChessPiece>(); //identifies the piece we are analyzing
                if (settings["flip_board"] == "true") //Flip the position of the piece on the board if the flip_board option is set to true 
                {
                    StartCoroutine(MoveObject(piece, ChessPiece.RotateBoard(piece.transform.position), 0.3f));

                    uiControl.ToggleTileMap();
                }
                if (piece.name == currentPlayer + "_enPassant") //Removes en passants, as they only exist for one round
                {
                    RemovePieceAt(cp.Position);
                }
                if (piece.name == "wall") //makes sure the wall is still supposed to exist
                    cp.CheckWalls();
                if (PositionOnVacation(cp.Position.x, cp.Position.y)) //If the piece is on vacation
                {

                    if (currentPlayer == "black" && cp.color == "white") // add two to the enjoyment of the player who is on vacation
                    {
                        whiteEnjoyment += 2;
                        cp.daysOnVacation++;
                    }
                    else if (currentPlayer == "white" && cp.color == "black")
                    {
                        blackEnjoyment += 2;
                        cp.daysOnVacation++;
                    }
                    if (cp.isKing && cp.daysOnVacation == 3) //If the king is on vacation for 3 days, it dies
                    {
                        uiControl.DisplayAlert(14);
                        GameOver(cp.color == "black" ? "white" : "black");
                    }
                    if (cp.name.Contains("knight")) //If a knight and rook are on vacation, the rook is destroyed and the knight becomes a Knook.
                    {
                        if (cp.Position.y == 3 && cp.CheckRook(8, 4, cp.color))
                        {
                            RemovePieceAt(new Vector2Int(8, 4));
                            RemovePieceAt(new Vector2Int(8, 3));
                            SetPosition(Create(cp.color + "_knook", 8, 3));
                        }
                        else if (cp.Position.y == 4 && cp.CheckRook(8, 3, cp.color))
                        {
                            RemovePieceAt(new Vector2Int(8, 4));
                            RemovePieceAt(new Vector2Int(8, 3));
                            SetPosition(Create(cp.color + "_knook", 8, 4));
                        }
                    }
                }
                if (cp.name == currentPlayer + "_queen") //Enable the pawn shop if the queen of the current player is next to the king and not pregnant
                {
                    if ((cp.CheckKing(cp.Position.x + 1, cp.Position.y, cp.color) || cp.CheckKing(cp.Position.x - 1, cp.Position.y, cp.color) || cp.CheckKing(cp.Position.x, cp.Position.y + 1, cp.color) || cp.CheckKing(cp.Position.x, cp.Position.y - 1, cp.color)) && cp.pregnantWith == "")
                    {
                        uiControl.ChangeShopButton(true);
                    }
                    else uiControl.ChangeShopButton(false);
                    if (cp.pregnantWith != "") cp.daysPregnant++; // add one day to the queen's pregancy
                    if (cp.daysPregnant == 3) //after 3 days, the queen gives birth to a new piece
                    {
                        if (GetPosition(9, 3) == null)
                        {
                            GameObject newPiece = Create(cp.color + "_" + cp.pregnantWith, 9, 3);
                            SetPosition(newPiece);
                            StartCoroutine(FlashCoroutine(newPiece.transform.position.x, newPiece.transform.position.y));
                        }
                        else if (GetPosition(9, 4) == null)
                        {
                            GameObject newPiece = Create(cp.color + "_" + cp.pregnantWith, 9, 4);
                            SetPosition(newPiece);
                            StartCoroutine(FlashCoroutine(newPiece.transform.position.x, newPiece.transform.position.y));
                        }
                        else //if the nursery is full, the game ends
                        {
                            GameOver(cp.color == "black" ? "white" : "black");
                            uiControl.DisplayAlert(15);
                        }
                        cp.pregnantWith = ""; //set the queen's pregnantWith back to nothing
                        cp.daysPregnant = 0; //set the queen's daysPregnant back to 0
                        cp.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f); //set the queen's color back to white
                    }
                }
                if (PositionInNursery(cp.Position.x, cp.Position.y)) //after two days in the nursery, a piece is randomly placed on the board
                {
                    cp.daysAfterBirth++;
                    if (cp.daysAfterBirth == 4)
                    {
                        List<Vector2Int> emptyPositions = new();
                        for (int x = 0; x < 8; x++)
                        {
                            for (int y = 0; y < 8; y++)
                            {
                                if (board[x, y] == null)
                                {
                                    emptyPositions.Add(new Vector2Int(x, y));
                                }
                            }
                        }

                        if (emptyPositions.Count > 0)
                        {
                            Vector2Int randomPosition = emptyPositions[UnityEngine.Random.Range(0, emptyPositions.Count)];
                            cp.Position = randomPosition;
                            SetPosition(piece);
                            cp.daysAfterBirth = 0;
                        }
                    }
                }

            }
        }
        uiControl.UpdateEnjoymentCounter(); //update the enjoyment counter on the screen
        Debug.Log(PositionToFEN());
    }
    public void RemovePieceAt(Vector2Int position)
    {
        if (board[position.x, position.y] != null)
        {
            Debug.Log(board[position.x, position.y].name);
            Destroy(board[position.x, position.y]);
        }
        SetPositionEmpty(position.x, position.y);
    }
    public void RemovePiece(GameObject piece)
    {
        RemovePieceAt(piece.GetComponent<ChessPiece>().Position);
    }
    public void ForceEnPassant(GameObject piece)
    {
        ChessPiece cp = piece.GetComponent<ChessPiece>(); //piece taking the en passant
        ChessPiece rf = null; //en passant
        (GameObject left, GameObject right) = cp.GetDiagonalPieces("_enPassant");
        if (left != null) rf = left.GetComponent<ChessPiece>();
        else if (right != null) rf = right.GetComponent<ChessPiece>();

        SetPositionEmpty(cp.Position.x, cp.Position.y);
        cp.Position = rf.Position;
        cp.FlipBoardFix();
        SetPosition(piece);
        Destroy(rf.gameObject);
        RemovePieceAt(new Vector2Int(cp.Position.x, cp.Position.y + (cp.color == "white" ? -1 : 1)));
        uiControl.ChangeEnPassantModal(true);
        StartCoroutine(WaitForNextTurn(0.3f));
        if (cp.color == "white") blackPawnsLost++;
        else whitePawnsLost++;
    }
    public void DoubleEnPassant(GameObject left, GameObject right, GameObject enPassant)
    {
        ChessPiece enPassantPiece = enPassant.GetComponent<ChessPiece>();
        GameObject rf = GetPosition(enPassantPiece.Position.x, enPassantPiece.Position.y + (enPassantPiece.color == "white" ? 1 : -1));
        ChessPiece antipawn = Create(enPassantPiece.color + "_antipawn", rf.GetComponent<ChessPiece>().Position.x, rf.GetComponent<ChessPiece>().Position.y).GetComponent<ChessPiece>();
        ChessPiece bishop = Create(left.GetComponent<ChessPiece>().color + "_bishop", enPassantPiece.Position.x, enPassantPiece.Position.y).GetComponent<ChessPiece>();
        SetPositionEmpty(left.GetComponent<ChessPiece>().Position.x, left.GetComponent<ChessPiece>().Position.y);
        SetPositionEmpty(right.GetComponent<ChessPiece>().Position.x, right.GetComponent<ChessPiece>().Position.y);
        Destroy(left);
        Destroy(right);
        Destroy(enPassant);
        Destroy(rf);
        antipawn.FlipBoardFix();
        bishop.FlipBoardFix();
        SetPosition(antipawn.gameObject);
        SetPosition(bishop.gameObject);
        StartCoroutine(FlashCoroutine(bishop.gameObject.transform.position.x, bishop.gameObject.transform.position.y));
        uiControl.ChangeEnPassantModal(true);
        uiControl.DisplayAlert(3);
        StartCoroutine(WaitForNextTurn(0.3f));
    }
    IEnumerator WaitForNextTurn(float seconds)
    {
        //Print the time of when the function is first called.
        Debug.Log("Started Coroutine at timestamp : " + Time.time);

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(seconds);

        //After we have waited 5 seconds print the time again.
        Debug.Log("Finished Coroutine at timestamp : " + Time.time);
        NextTurn();
    }
    /// <summary>
    /// Impregnates the queen with a given piece. This will only impregnate one queen at a time.
    /// </summary>
    /// <param name="piece">The name of the piece to impregnate the queen with.</param>
    public void ImpregnateQueen(string piece)
    {
        // Check if the current player has enough funds to impregnate the queen
        if ((currentPlayer == "white" ? whiteEnjoyment : blackEnjoyment) >= piecePrices[piece])
        {
            // Log the price of the piece and the current player's enjoyment
            Debug.Log(piecePrices[piece]);
            Debug.Log(currentPlayer == "white" ? whiteEnjoyment : blackEnjoyment);

            // Disable the shop button and shop UI
            uiControl.ChangeShopButton(false);
            uiControl.ChangeShop(false);

            // Iterate through the board and find the first queen of the current player
            foreach (GameObject chessPiece in board)
            {
                if (chessPiece == null) continue;
                ChessPiece cp = chessPiece.GetComponent<ChessPiece>();
                if (cp.name == currentPlayer + "_queen" && (cp.CheckKing(cp.Position.x + 1, cp.Position.y, cp.color) || cp.CheckKing(cp.Position.x - 1, cp.Position.y, cp.color) || cp.CheckKing(cp.Position.x, cp.Position.y + 1, cp.color) || cp.CheckKing(cp.Position.x, cp.Position.y - 1, cp.color)) && cp.daysPregnant == 0)
                {
                    // Set the queen to be pregnant with the given piece
                    cp.pregnantWith = piece;
                    // Change the color of the queen's sprite to indicate pregnancy
                    chessPiece.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.6f);
                    break;
                }
            }

            // Deduct the price of the piece from the current player's enjoyment
            if (currentPlayer == "white") whiteEnjoyment -= piecePrices[piece];
            else blackEnjoyment -= piecePrices[piece];

            // Update the enjoyment counter UI
            uiControl.UpdateEnjoymentCounter();
        }
        else
        {
            // Log a message indicating that the current player does not have enough funds
            Debug.Log("not enough funds");
        }
    }
    public void CommunistRevolution()
    {
        uiControl.CommunistRevolution(currentPlayer);
        foreach (GameObject piece in board)
        {
            if (piece != null && piece.GetComponent<ChessPiece>().color == currentPlayer)
            {
                if (piece.GetComponent<ChessPiece>().isKing)
                {
                    Debug.Log(name);
                    piece.name = currentPlayer + "_lenin";
                    Debug.Log(piece.name);
                    piece.GetComponent<ChessPiece>().Activate(piece);
                    return;
                }
                else if (piece.name.Contains("queen"))
                {
                    RemovePiece(piece);
                }
            }
        }
    }
    /// <summary>
    /// Moves a game object smoothly from its current position to a new position over a specified duration.
    /// </summary>
    /// <param name="piece">The game object to be moved.</param>
    /// <param name="newPosition">The target position to move the game object to.</param>
    /// <param name="duration">The time it takes for the game object to reach the target position.</param>
    /// <returns>An enumerator that can be used to control the movement of the game object.</returns>
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
    /// <summary>
    /// If the game is over and the right mouse button is clicked, reload the "Game" scene.
    /// </summary>
    private void Update()
    {
        if (gameOver && Input.GetMouseButtonDown(1))
        {
            Restart();
        }
    }
    public void Restart()
    {
        Debug.Log("Restart");
        gameOver = false;
        SceneManager.LoadScene("Game");
    }
    /// <summary>
    /// Creates a flash effect at the specified position.
    /// </summary>
    /// <param name="x">The x-coordinate of the flash effect.</param>
    /// <param name="y">The y-coordinate of the flash effect.</param>
    /// <returns>An IEnumerator object representing the flash effect coroutine.</returns>
    private IEnumerator FlashCoroutine(float x, float y)
    {
        // Instantiate the flash prefab at the given position with zero scale
        GameObject flashInstance = Instantiate(flash, new Vector3(x, y, -3), Quaternion.identity);
        flashInstance.transform.localScale = Vector3.zero;

        float duration = 0.5f;
        float halfDuration = duration / 2f;
        float elapsedTime = 0f;

        // Lerp the scale from 0 to 1 over half the duration
        while (elapsedTime < halfDuration)
        {
            float t = elapsedTime / halfDuration;
            flashInstance.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Lerp the scale from 1 back to 0 over the remaining half duration
        elapsedTime = 0f;
        while (elapsedTime < halfDuration)
        {
            float t = elapsedTime / halfDuration;
            flashInstance.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Destroy the flash instance after the animation is complete
        Destroy(flashInstance);
    }
    public void GameOverChecked() //this is used for rule 7 to work properly
    {
        if (gameOver) GameOver(currentPlayer == "white" ? "black" : "white");
    }
    /// <summary>
    /// Sets the game state to over and displays the winner. Displays a random quote from the Google en passant chainS
    /// </summary>
    /// <param name="winner">The winning player.</param>
    public void GameOver(string winner)
    {
        gameOver = true;
        Debug.Log("Winner: " + winner);
        if (!(winner == "white" ? whiteCapturedPiece : blackCapturedPiece))
        {
            uiControl.TrueEnding();
        }
        else
        {
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

    public string PositionToFEN()
    {
        string result = "";
        int consecutiveNullCount = 0;

        for (int r = 7; r >= 0; r--)
        {
            for (int c = 0; c <= 7; c++)
            {
                if (c > 7) continue;
                string name = "";
                if (board[c, r] != null)
                    name = board[c, r].GetComponent<ChessPiece>().name;
                if (board[c, r] == null || name.Contains("enPassant") || name.Contains("wall") || name.Contains("knook") || name.Contains("antipawn"))
                {
                    consecutiveNullCount++;
                }
                else
                {
                    if (consecutiveNullCount > 0)
                    {
                        result += consecutiveNullCount.ToString();
                        consecutiveNullCount = 0;
                    }

                    switch (board[c, r].name)
                    {
                        case "white_pawn":
                            result += "P";
                            break;
                        case "white_rook":
                            result += "R";
                            break;
                        case "white_knight":
                            result += "N";
                            break;
                        case "white_bishop":
                            result += "B";
                            break;
                        case "white_queen":
                            result += "Q";
                            break;
                        case "white_king":
                            result += "K";
                            break;
                        case "black_pawn":
                            result += "p";
                            break;
                        case "black_rook":
                            result += "r";
                            break;
                        case "black_knight":
                            result += "n";
                            break;
                        case "black_bishop":
                            result += "b";
                            break;
                        case "black_queen":
                            result += "q";
                            break;
                        case "black_king":
                            result += "k";
                            break;
                    }
                }
            }

            if (consecutiveNullCount > 0)
            {
                result += consecutiveNullCount.ToString();
                consecutiveNullCount = 0;
            }

            if (r > 0)
            {
                result += "/";
            }
        }
        result += "_" + (currentPlayer == "white" ? "w" : "b") + "_-_";
        bool enPassant = false;
        foreach (GameObject piece in board)
        {
            if (piece == null) continue;
            ChessPiece cp = piece.GetComponent<ChessPiece>();
            if (cp.name.Contains("enPassant"))
            {
                result += (char)(cp.Position.x + 97);
                result += cp.Position.y + 1;
                enPassant = true;
            }
        }
        if (!enPassant) result += "-";
        result += "_" + halfMove + "_" + move;

        return result;
    }
    private int numRolls = 0;
    public void ConvertPawn(string name)
    {
        foreach (GameObject piece in board)
        {
            if (piece != null && piece.name.Contains("pawn") && piece.GetComponent<ChessPiece>().color == currentPlayer)
            {
                Debug.Log(name);
                piece.name = currentPlayer + "_" + name;
                Debug.Log(piece.name);
                piece.GetComponent<ChessPiece>().Activate(piece);
                numRolls++;
                break;
            }
        }
        if (numRolls == 5) uiControl.ChangeD20UpgradeOK(true);
    }
}
