using UnityEngine;

public class Castle : Figure
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
            return 5;
        }
    }

    public override FigureIndex Index
    {
        get
        {
            return FigureIndex.Castle;
        }
    }

    public override float OffsetY
    {
        get
        {
            return 0;//2.63f;
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

    public Castle(int x, int y, GameSide side) : base(x, y, side)
    {
    }

    public override bool CanMove(int x, int y)
    {
        if ((this.x == x) == (this.y == y)) return false;

        if(this.x == x)
        {
            int startY = y > this.y ? this.y : y;
            int endY = y < this.y ? this.y : y;

            for(int checkY = startY + 1; checkY < endY; checkY++)
            {
                if (board.tiles[x, checkY].figure != null) return false;
            }
        }
        else
        {
            int startX = x > this.x ? this.x : x;
            int endX = x < this.x ? this.x : x;

            for (int checkX = startX + 1; checkX < endX; checkX++)
            {
                if (board.tiles[checkX, y].figure != null) return false;
            }
        }

        return true;
    }
}
