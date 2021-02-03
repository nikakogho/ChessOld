using UnityEngine;

public class BoardTile : MonoBehaviour
{
    public Vector3 whiteRotation, blackRotation;
    public Figure figure;
    public int x, y;
    public Vector3 spawnDelta;
    Renderer rend;

    public Color targetColor = Color.yellow;
    public Color cantColor = Color.red;
    public Color pickColor = Color.green;
    public Color defaultColor = Color.white;

    Board board;

    void Start()
    {
        board = Board.instance;

        rend = board.pickTileRend;
    }
    
    bool canPick = false;

    void OnMouseEnter()
    {
        rend.transform.position = new Vector3(transform.position.x, rend.transform.position.y, transform.position.z);

        if (figure != null && figure.side == board.AISide)
        {
            rend.material.color = cantColor;
            canPick = false;
            return;
        }

        if (board.selectedTile == null)
        {
            canPick = figure != null && figure.side == board.turn;

            if (canPick)
            {
                canPick = false;

                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        Figure checkFigure = board.tiles[x, y].figure;

                        if (checkFigure == null || checkFigure.side != figure.side)
                        {
                            if (figure.CanMove(x, y))
                            {
                                board.testing = true;
                                BoardTile targetTile = board.tiles[x, y];

                                targetTile.MoveFigure(figure);
                                
                                canPick = !targetTile.figure.side.CheckForCheck();

                                board.sets[board.sets.Count - 1].SetToBoard();

                                board.testing = false;

                                if(canPick)
                                break;
                            }
                        }
                    }

                    if (canPick) break;
                }
            }

            rend.material.color = canPick ? pickColor : cantColor;
        }
        else
        {
            if(board.selectedTile != this && (figure == null || figure.side != board.selectedTile.figure.side) && board.selectedTile.figure.CanMove(x, y))
            {
                board.testing = true;
                MoveFigure(board.selectedTile.figure);

                canPick = !figure.side.CheckForCheck();

                board.sets[board.sets.Count - 1].SetToBoard();

                board.testing = false;

                rend.material.color = canPick ? targetColor : cantColor;
            }
            else
            {
                rend.material.color = cantColor;
                canPick = false;
            }
        }
    }

    void OnMouseExit()
    {
        rend.material.color = defaultColor;
    }

    void OnMouseDown()
    {
        if (!canPick)
        {
            board.source.clip = board.cantSound;
            board.source.Play();
            return;
        }

        if (board.selectedTile == null)
        {
            board.selectedTile = this;

            board.source.clip = board.pickSound;
            board.source.Play();
        }
        else
        {
            board.source.clip = board.moveSound;
            board.source.Play();

            MoveFigure(board.selectedTile.figure);

            figure.side.king.checkEffect.SetActive(false);
            figure.figObject.transform.position = transform.position + spawnDelta + Vector3.up * figure.OffsetY;
            figure.side.EnemySide.king.checkEffect.SetActive(figure.side.EnemySide.CheckForCheck());

            if (figure.side.EnemySide.check)
            {
                board.source.clip = board.checkSound;
                board.source.Play();
            }

            board.EndMove();
        }

        canPick = false;
        rend.material.color = Color.red;
    }

    public void MoveFigure(Figure figure)
    {
        board.tiles[figure.x, figure.y].figure = null;
        figure.Move(x, y);

        Kill(figure, !board.testing);
    }

    /*
    bool PreCheckForCheck(Figure figure, int moveX, int moveY)
    {
        board.tiles[moveX, moveY].MoveFigure(figure);
        
        return false;
    }*/

    public void Undo()
    {
        board.sets.Undo();
    }

    public void Kill(Figure replaceWith = null, bool makeSound = false)
    {
        if (figure != null)
        {
            Destroy(figure.figObject);
            figure.Destroy();

            if (makeSound)
            {
                board.source.clip = board.turn == board.AISide ? board.getKilledSound : board.killSound;
                board.source.Play();
            }
        }

        figure = replaceWith;
    }

    public void SpawnFigure(Figure fig)
    {
        Kill(fig);

        Vector3 spawnPos = transform.position + spawnDelta + Vector3.up * fig.OffsetY;
        Quaternion spawnRotation = Quaternion.Euler((fig.side == GameSide.white ? whiteRotation : blackRotation) + fig.Rotation);

        fig.figObject = Instantiate(fig.Prefab(), spawnPos, spawnRotation);
        fig.figObject.transform.parent = transform;

        if (fig.GetType() == typeof(King))
        {
            King king = (King)fig;
            king.checkEffect = fig.figObject.transform.GetChild(0).gameObject;
        }
    }
}