using UnityEngine;

public class Queen : Figure {
    #region Prefab
    public static GameObject whitePrefab, blackPrefab;
    public override GameObject WhitePrefab
    {
        get
        {
            return whitePrefab;
        }
    }

    public override GameObject BlackPrefab
    {
        get
        {
            return blackPrefab;
        }
    }
    #endregion

    public override int Value
    {
        get
        {
            return 10;
        }
    }

    Castle castle;
    Bishop bishop;

    public override FigureIndex Index
    {
        get
        {
            return FigureIndex.Queen;
        }
    }

    public override float OffsetY
    {
        get
        {
            return 0;//3.1f;
        }
    }

    public override Vector3 WhiteRotation
    {
        get
        {
            return Vector3.zero;
        }
    }

    public override Vector3 BlackRotation
    {
        get
        {
            return Vector3.zero;
        }
    }

    public Queen(int x, int y, GameSide side) : base(x, y, side)
    {
        castle = new Castle(x, y, side);
        bishop = new Bishop(x, y, side);
    }

    public override bool CanMove(int x, int y)
    {
        return castle.CanMove(x, y) || bishop.CanMove(x, y);
    }

    public override void Move(int x, int y)
    {
        base.Move(x, y);

        castle.x = x;
        castle.y = y;
        bishop.x = x;
        bishop.y = y;
    }
}
