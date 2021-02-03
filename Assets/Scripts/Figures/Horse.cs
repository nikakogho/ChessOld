using UnityEngine;

public class Horse : Figure
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
            return FigureIndex.Horse;
        }
    }

    public override Vector3 WhiteRotation
    {
        get
        {
            return new Vector3(0, 90, 0);
        }
    }

    public override Vector3 BlackRotation
    {
        get
        {
            return new Vector3(0, -90, 0);
        }
    }

    public override float OffsetY
    {
        get
        {
            return 0;//2.75f;
        }
    }

    public override bool CanMove(int x, int y)
    {
        return (Mathf.Abs(this.x - x) == 2 && Mathf.Abs(this.y - y) == 1) || (Mathf.Abs(this.x - x) == 1 && Mathf.Abs(this.y - y) == 2);
    }
    
    public Horse(int x, int y, GameSide side) : base(x, y, side)
    {
    }
}
