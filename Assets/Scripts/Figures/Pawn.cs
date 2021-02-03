using UnityEngine;

public class Pawn : Figure
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
            return y == (side == GameSide.white ? 6 : 1) ? 8 : 1;
        }
    }

    public static Pawn lastMovedPawn = null;

    public override FigureIndex Index
    {
        get
        {
            return FigureIndex.Pawn;
        }
    }

    public override float OffsetY
    {
        get
        {
            return 0;// 2.23f;
        }
    }

    public override Vector3 WhiteRotation
    {
        get
        {
            return new Vector3(90, 0, 0);
        }
    }

    public override Vector3 BlackRotation
    {
        get
        {
            return new Vector3(90, 0, 0);
        }
    }

    public Pawn(int x, int y, GameSide side) : base(x, y, side)
    {
    }

    public override bool CanMove(int x, int y)
    {
        int yDelta = this.y - y;

        bool weirdWhite = side == GameSide.white && !((yDelta == -2 && !moved) || (yDelta == -1));
        bool weirdBlack = side == GameSide.black && !((yDelta == 2 && !moved) || (yDelta == 1));

        if (weirdWhite || weirdBlack) 
        {
            return false;
        }

        if(Mathf.Abs(yDelta) == 2)
        {
            if (this.x != x || board.tiles[x, (this.y + y) / 2].figure != null) return false;
        }

        if(this.x == x)
        {
            return board.tiles[x, y].figure == null;
        }
        else if (Mathf.Abs(this.x - x) == 1)
        {
            Figure figure = board.tiles[x, y].figure;

            if (figure != null && figure.side != side) return true;

            figure = board.tiles[x, this.y].figure;

            if (figure != null && figure.side != side && figure == lastMovedPawn) return true;
        }
        
        return false;
    }

    public override void Move(int x, int y)
    {
        if (board.tiles[x, this.y].figure == lastMovedPawn && lastMovedPawn != null) board.tiles[x, this.y].Kill();

        if (Mathf.Abs(this.y - y) == 2) lastMovedPawn = this;
        else lastMovedPawn = null;

        base.Move(x, y);

        if(y == 0 || y == 7)
        {
            board.Ascend(this);
        }
    }
}
