using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class GameSide
{
    public static GameSide white, black;
    public Figure[] pawns = new Figure[8];
    public Horse[] horses = new Horse[2];
    public Bishop[] bishops = new Bishop[2];
    public Castle[] castles = new Castle[2];
    public Queen queen;
    public King king;
    public bool check = false;
    Board board;

    public GameSide()
    {
        board = Board.instance;
    }

    public GameSide EnemySide { get { return this == white ? black : white; } }

    public struct MoveCombo
    {
        public struct Move
        {
            public Figure figure;
            public int x, y;

            public Move(Figure figure, int x, int y)
            {
                this.x = x;
                this.y = y;
                this.figure = figure;
            }
        }
        
        public int reward;
        public List<Move> steps;

        public MoveCombo(int reward, List<Move> steps)
        {
            this.reward = reward;
            this.steps = new List<Move>(steps.Count);

            for (int i = 0; i < steps.Count; i++) this.steps[i] = steps[i];
        }
    }

    void GetCombos(List<MoveCombo> combos, MoveCombo combo, int stepsRemaining)
    {
        stepsRemaining--;

        var oldSet = new BoardSet();

        foreach(Figure figure in AllFigures())
        {
            for(int x = 0; x < 8; x++)
            {
                for(int y = 0; y < 8; y++)
                {
                    BoardTile tile = board.tiles[x, y];

                    if((tile.figure == null || tile.figure.side != figure.side) && figure.CanMove(x, y))
                    {
                        BoardSet set = new BoardSet();
                        tile.MoveFigure(figure);

                        bool completed = !figure.side.CheckForCheck();

                        if (completed)
                        {
                            MoveCombo newCombo = combo;
                            newCombo.steps = new List<MoveCombo.Move>(combo.steps.Count);
                            newCombo.reward += tile.figure == null ? (figure.Index == Figure.FigureIndex.Pawn ? (8 - Mathf.Abs(figure.y - (EnemySide == white ? 0 : 7))) : 0) : tile.figure.Value;

                            for(int i = 0; i < combo.steps.Count; i++)
                            {
                                newCombo.steps[i] = combo.steps[i];
                            }

                            newCombo.steps.Add(new MoveCombo.Move(figure, x, y));
                            combos.Add(newCombo);

                            if(stepsRemaining > 0)
                            GetCombos(combos, combo, stepsRemaining);
                            
                        }

                        set.SetToBoard();
                    }
                }
            }
        }

        oldSet.SetToBoard();
    }

    public void AIMove()
    {
        List<MoveCombo> goodCombos = new List<MoveCombo>();
        int steps = 1;
        
        board.testing = true;
        GetCombos(goodCombos, new MoveCombo(0, new List<MoveCombo.Move>()), steps);
        board.testing = false;

        MoveCombo bestCombo = goodCombos[0];

        for (int i = 1; i < goodCombos.Count; i++)
        {
            if (goodCombos[i].reward * bestCombo.steps.Count > bestCombo.reward * goodCombos[i].steps.Count)
            {
                bestCombo = goodCombos[i];
            }
        }

        board.source.clip = board.moveSound;
        board.source.Play();

        Debug.Log(bestCombo.steps);

        Figure figure = bestCombo.steps[0].figure;
        BoardTile tile = board.tiles[bestCombo.steps[0].x, bestCombo.steps[0].y];

        tile.MoveFigure(figure);

        figure.side.king.checkEffect.SetActive(false);
        Vector3 pos = tile.transform.position + tile.spawnDelta + Vector3.up * figure.OffsetY;
        figure.figObject.transform.position = pos;
        figure.side.EnemySide.king.checkEffect.SetActive(figure.side.EnemySide.CheckForCheck());

        if (figure.side.EnemySide.check)
        {
            board.source.clip = board.checkSound;
            board.source.Play();
        }
        
        board.EndMove();
    }

    public bool CheckForCheck()
    {
        check = false;

        foreach(Figure figure in EnemySide.AllFigures())
        {
            if(figure.CanMove(king.x, king.y))
            {
                check = true;
                return true;
            }
        }

        return false;
    }

    public void SetupFigures(Figure[] pawns, Horse[] horses, Bishop[] bishops, Castle[] castles, Queen queen, King king)
    {
        for (int i = 0; i < 8; i++) this.pawns[i] = pawns[i];

        for(int i = 0; i < 2; i++)
        {
            this.horses[i] = horses[i];
            this.bishops[i] = bishops[i];
            this.castles[i] = castles[i];
        }

        this.queen = queen;
        this.king = king;

        king.leftCastle = castles[0];
        king.rightCastle = castles[1];
    }

    public void Remove(Figure figure)
    {
        if(queen == figure)
        {
            queen = null;
            return;
        }

        for(int i = 0; i < 8; i++)
        {
            if(pawns[i] == figure)
            {
                pawns[i] = null;
                return;
            }
        }

        for(int i = 0; i < 2; i++)
        {
            if (horses[i] == figure)
            {
                horses[i] = null;
                return;
            }

            if (bishops[i] == figure)
            {
                bishops[i] = null;
                return;
            }

            if (castles[i] == figure)
            {
                castles[i] = null;
                return;
            }
        }
    }

    public Figure[] AllFigures()
    {
        List<Figure> figures = new List<Figure>() { king };

        for(int i = 0; i < 8; i++)
        {
            if (pawns[i] != null) figures.Add(pawns[i]);
        }

        for(int i = 0; i < 2; i++)
        {
            if (horses[i] != null) figures.Add(horses[i]);
            if (bishops[i] != null) figures.Add(bishops[i]);
            if (castles[i] != null) figures.Add(castles[i]);
        }

        if (queen != null) figures.Add(queen);

        return figures.ToArray();
    }
}
