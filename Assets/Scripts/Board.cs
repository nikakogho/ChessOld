using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Board : MonoBehaviour
{
    Camera cam;
    public Transform camPoint1, camPoint2;
    public AudioSource source;
    public AudioClip victorySound, defeatSound, tieSound, moveSound, killSound, getKilledSound, pickSound, checkSound, cantSound;
    public static GameMode gameMode;
    public static Board instance;
    public GameObject tilePrefab;
    public Vector3 startPos;
    public BoardTile[,] tiles = new BoardTile[8, 8];
    public BoardTile selectedTile = null;
    public Transform tileParent;
    public Renderer pickTileRend;
    public GameObject ascendUI, pickSideUI;
    public GameSide turn;
    public List<BoardSet> sets = new List<BoardSet>();
    public GameObject loadingAnim;
    public bool testing = false;

    public GameSide AISide = null;

    public float cameraSwitchDuration = 2f;

    #region Prefabs
    public GameObject whitePawn, whiteHorse, whiteBishop, whiteCastle, whiteQueen, whiteKing;
    public GameObject blackPawn, blackHorse, blackBishop, blackCastle, blackQueen, blackKing;

    void ApplyPrefabs()
    {
        Pawn.whitePrefab = whitePawn;
        Pawn.blackPrefab = blackPawn;

        Horse.whitePrefab = whiteHorse;
        Horse.blackPrefab = blackHorse;

        Bishop.whitePrefab = whiteBishop;
        Bishop.blackPrefab = blackBishop;

        Castle.whitePrefab = whiteCastle;
        Castle.blackPrefab = blackCastle;

        Queen.whitePrefab = whiteQueen;
        Queen.blackPrefab = blackQueen;

        King.whitePrefab = whiteKing;
        King.blackPrefab = blackKing;
    }
    #endregion

    #region Setup
    void SpawnTiles()
    {
        for (int x = 0; x < 8; x++)
        {
            for(int y = 0; y < 8; y++)
            {
                Vector3 spawnPos = startPos + new Vector3(x * 4, 0, y * 4);

                BoardTile tile = Instantiate(tilePrefab, spawnPos, Quaternion.identity, tileParent).GetComponent<BoardTile>();

                tile.x = x;
                tile.y = y;

                tiles[x, y] = tile;
            }
        }
    }

    void SetupFigures()
    {
        GameSide white = new GameSide();
        GameSide black = new GameSide();

        GameSide.white = white;
        GameSide.black = black;

        for(int x = 0; x < 8; x++)
        {
            Pawn whitePawn = new Pawn(x, 1, white);
            Pawn blackPawn = new Pawn(x, 6, black);

            tiles[x, 1].SpawnFigure(whitePawn);
            tiles[x, 6].SpawnFigure(blackPawn);

            white.pawns[x] = whitePawn;
            black.pawns[x] = blackPawn;
        }

        Castle blackCastleLeft = new Castle(0, 7, black), blackCastleRight = new Castle(7, 7, black);
        Horse blackHorseLeft = new Horse(1, 7, black), blackHorseRight = new Horse(6, 7, black);
        Bishop blackBishopLeft = new Bishop(2, 7, black), blackBishopRight = new Bishop(5, 7, black);
        Queen blackQueen = new Queen(3, 7, black);
        King blackKing = new King(4, 7, black, blackCastleLeft, blackCastleRight);

        tiles[0, 7].SpawnFigure(blackCastleLeft);
        tiles[1, 7].SpawnFigure(blackHorseLeft);
        tiles[2, 7].SpawnFigure(blackBishopLeft);
        tiles[3, 7].SpawnFigure(blackQueen);
        tiles[4, 7].SpawnFigure(blackKing);
        tiles[5, 7].SpawnFigure(blackBishopRight);
        tiles[6, 7].SpawnFigure(blackHorseRight);
        tiles[7, 7].SpawnFigure(blackCastleRight);

        black.castles[0] = blackCastleLeft;
        black.castles[1] = blackCastleRight;
        black.horses[0] = blackHorseLeft;
        black.horses[1] = blackHorseRight;
        black.bishops[0] = blackBishopLeft;
        black.bishops[1] = blackBishopRight;
        black.queen = blackQueen;
        black.king = blackKing;

        Castle whiteCastleLeft = new Castle(0, 0, white), whiteCastleRight = new Castle(7, 0, white);
        Horse whiteHorseLeft = new Horse(1, 0, white), whiteHorseRight = new Horse(6, 0, white);
        Bishop whiteBishopLeft = new Bishop(2, 0, white), whiteBishopRight = new Bishop(5, 0, white);
        Queen whiteQueen = new Queen(3, 0, white);
        King whiteKing = new King(4, 0, white, whiteCastleLeft, whiteCastleRight);

        tiles[0, 0].SpawnFigure(whiteCastleLeft);
        tiles[1, 0].SpawnFigure(whiteHorseLeft);
        tiles[2, 0].SpawnFigure(whiteBishopLeft);
        tiles[3, 0].SpawnFigure(whiteQueen);
        tiles[4, 0].SpawnFigure(whiteKing);
        tiles[5, 0].SpawnFigure(whiteBishopRight);
        tiles[6, 0].SpawnFigure(whiteHorseRight);
        tiles[7, 0].SpawnFigure(whiteCastleRight);

        white.castles[0] = whiteCastleLeft;
        white.castles[1] = whiteCastleRight;
        white.horses[0] = whiteHorseLeft;
        white.horses[1] = whiteHorseRight;
        white.bishops[0] = whiteBishopLeft;
        white.bishops[1] = whiteBishopRight;
        white.queen = whiteQueen;
        white.king = whiteKing;
    }
    #endregion

    void ToggleLoadingAnim()
    {
        loadingAnim.SetActive(!loadingAnim.activeSelf);
    }

    IEnumerator AIMove()
    {
        yield return new WaitForSeconds(1);
        ToggleLoadingAnim();
        turn.AIMove();
        //EndMove();
        ToggleLoadingAnim();
        yield return new WaitForSeconds(1);
    }

    IEnumerator CameraSwitch(Transform camPoint)
    {
        Vector3 midPoint = (cam.transform.position + camPoint.transform.position) / 2;
        midPoint.y = cam.transform.position.y;

        float yieldDelta = 0.02f;

        float angleDelta = 180f * yieldDelta / cameraSwitchDuration;
        //float angle = 0;

        Debug.Log("started");

        while(Vector3.Distance(cam.transform.position, camPoint.transform.position) > 0.1f)
        {
            cam.transform.RotateAround(midPoint, Vector3.up, angleDelta);
            yield return new WaitForSeconds(yieldDelta);
        }

        cam.transform.position = camPoint.transform.position;
        cam.transform.rotation = camPoint.transform.rotation;

        Debug.Log("Done");
    }

    public void EndMove()
    {
        selectedTile = null;
        turn = turn == GameSide.white ? GameSide.black : GameSide.white;

        if(AISide == null)
        {
            if(turn == GameSide.black)
            {
                cam.transform.position = camPoint1.transform.position;
                cam.transform.rotation = camPoint1.transform.rotation;
                StopAllCoroutines();
                StartCoroutine(CameraSwitch(camPoint2));
            }
            else
            {
                cam.transform.position = camPoint2.transform.position;
                cam.transform.rotation = camPoint2.transform.rotation;
                StopAllCoroutines();
                StartCoroutine(CameraSwitch(camPoint1));
            }
        }
        
        if (turn != AISide)
        {
            sets.Add(new BoardSet());
        }

        bool canMove = false;

        foreach(Figure figure in turn.AllFigures())
        {
            for(int x = 0; x < 8; x++)
            {
                for(int y = 0; y < 8; y++)
                {
                    if((tiles[x, y].figure == null || tiles[x,y].figure.side != figure.side) && figure.CanMove(x, y))
                    {
                        testing = true;

                        tiles[x, y].MoveFigure(figure);
                        canMove = !figure.side.CheckForCheck();
                        sets.Last().SetToBoard();

                        testing = false;

                        if (canMove) break;
                    }
                }
                if (canMove) break;
            }
            if (canMove) break;
        }

        if (canMove)
        {
            if(turn == AISide)
            {
                StartCoroutine(AIMove());
            }
        }
        else
        {
            if (turn.check)
            {
                //lose game
                source.clip = victorySound;
            }
            else
            {
                //draw
                source.clip = tieSound;
            }

            source.Play();
        }
    }

    public void Clear()
    {
        foreach(BoardTile tile in tiles)
        {
            tile.Kill();
        }
    }

    public void PickWhiteSide()
    {
        AISide = GameSide.black;
    }

    public void PickBlackSide()
    {
        AISide = GameSide.white;
    }

    void Awake()
    {
        instance = this;
        cam = Camera.main;
        ApplyPrefabs();

        SpawnTiles();
        SetupFigures();

        turn = GameSide.white;

        sets.Add(new BoardSet());

        if(gameMode == GameMode.OnePlayer)
        {
            pickSideUI.SetActive(true);
        }
    }

    #region Ascension
    int ascendingPawnIndex;
    Pawn ascendingPawn;

    public void Ascend(Pawn pawn)
    {
        if (testing) return;

        for (ascendingPawnIndex = 0; ascendingPawnIndex < 8; ascendingPawnIndex++) if (pawn.side.pawns[ascendingPawnIndex] == pawn) break;
        ascendingPawn = pawn;

        ascendUI.SetActive(true);
    }

    public void PickAscension(int choice)
    {
        Figure toReplaceWith = null;

        switch ((AscensionChoice)choice)
        {
            case AscensionChoice.Bishop:
                toReplaceWith = new Bishop(ascendingPawn.x, ascendingPawn.y, ascendingPawn.side);
                break;
            case AscensionChoice.Knight:
                toReplaceWith = new Horse(ascendingPawn.x, ascendingPawn.y, ascendingPawn.side);
                break;
            case AscensionChoice.Castle:
                toReplaceWith = new Castle(ascendingPawn.x, ascendingPawn.y, ascendingPawn.side);
                break;
            case AscensionChoice.Queen:
                toReplaceWith = new Queen(ascendingPawn.x, ascendingPawn.y, ascendingPawn.side);
                break;
        }

        ascendingPawn.side.pawns[ascendingPawnIndex] = toReplaceWith;
        tiles[toReplaceWith.x, toReplaceWith.y].SpawnFigure(toReplaceWith);

        ascendUI.SetActive(false);
    }

    public enum AscensionChoice { Knight, Bishop, Castle, Queen }
    #endregion
}

public enum GameMode { OnePlayer, TwoPlayers }