using UnityEngine;

public class Bishop : Figure
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
            return 3;
        }
    }

    public override FigureIndex Index
    {
        get
        {
            return FigureIndex.Bishop;
        }
    }

    public override float OffsetY
    {
        get
        {
            return 2;//2.86f;
        }
    }

    public override Vector3 WhiteRotation
    {
        get
        {
            return new Vector3(90, 0, 45);
        }
    }

    public override Vector3 BlackRotation { get { return new Vector3(90, 0, -135); } }

    public Bishop(int x, int y, GameSide side) : base(x, y, side)
    {
    }

    public override bool CanMove(int x, int y)
    {
        if (Mathf.Abs(this.x - x) != Mathf.Abs(this.y - y)) return false;

        int startX = Mathf.Min(x, this.x);
        int endX = Mathf.Max(x, this.x);
        int startY = x > this.x ? this.y : y;
        int endY = x > this.x ? y : this.y;
        int yChange = startY > endY ? -1 : 1;
        
        for(int checkX = startX + 1, checkY = startY + yChange; checkX < endX; checkX++)
        {
            if (board.tiles[checkX, checkY].figure != null) return false;
            
            checkY += yChange;
        }

        return true;
    }
}
