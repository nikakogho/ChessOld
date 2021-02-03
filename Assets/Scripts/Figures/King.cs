using UnityEngine;

public class King : Figure
{
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
            return 100;
        }
    }

    public GameObject checkEffect;

    public override FigureIndex Index
    {
        get
        {
            return FigureIndex.King;
        }
    }

    public override float OffsetY
    {
        get
        {
            return 0.815f;//3.37f;
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
        get { return Vector3.zero; }
    }

    public Castle leftCastle, rightCastle;

    public King(int x, int y, GameSide side, Castle leftCastle, Castle rightCastle) : base(x, y, side)
    {
        this.leftCastle = leftCastle;
        this.rightCastle = rightCastle;
    }

    public override bool CanMove(int x, int y)
    {
        if (!moved && this.y == y && !side.check)
        {
            if(this.x - x == 3)
            {
                if (board.tiles[1, y].figure != null || board.tiles[2, y].figure != null) return false;

                return leftCastle != null && !leftCastle.moved;
            }
            else if(x - this.x == 2)
            {
                if (board.tiles[5, y].figure != null || board.tiles[6, y].figure != null) return false;

                return rightCastle != null && !rightCastle.moved;
            }
        }

        return Mathf.Abs(this.x - x) <= 1 && Mathf.Abs(this.y - y) <= 1;
    }

    public override void Move(int x, int y)
    {
        if(this.x - x == 3)
        {
            board.tiles[2, leftCastle.y].MoveFigure(leftCastle);
        }
        else if (x - this.x == 2)
        {
            board.tiles[5, rightCastle.y].MoveFigure(rightCastle);
        }

        base.Move(x, y);
    }
}
