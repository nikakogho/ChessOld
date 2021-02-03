using UnityEngine;

public abstract class Figure : System.IDisposable
{
    public int x, y;
    public GameObject figObject;
    protected Board board;
    protected GameSide enemySide;
    public GameSide side;
    public abstract GameObject WhitePrefab { get; }
    public abstract GameObject BlackPrefab { get; }
    public abstract float OffsetY { get; }
    public abstract FigureIndex Index { get; }
    public abstract int Value { get; }
    public abstract Vector3 WhiteRotation { get; }
    public abstract Vector3 BlackRotation { get; }

    public Vector3 Rotation { get { return side == GameSide.white ? WhiteRotation : BlackRotation; } }

    public GameObject Prefab()
    {
        return side == GameSide.white ? WhitePrefab : BlackPrefab;
    }

    public abstract bool CanMove(int x, int y);
    public virtual void Move(int x, int y)
    {
        this.x = x;
        this.y = y;

        moved = true;
    }

    public void Dispose()
    {
        System.GC.SuppressFinalize(this);
    }

    public void Destroy()
    {
        side.Remove(this);

        Dispose();
    }

    public bool moved = false;

    public Figure(int x, int y, GameSide side)
    {
        this.x = x;
        this.y = y;
        this.side = side;
        board = Board.instance;
        enemySide = side == GameSide.white ? GameSide.black : GameSide.white;
    }

    public enum FigureIndex { None = 0, Pawn = 1, Horse = 2, Bishop = 3, Castle = 4, Queen = 5, King = 6 }
}