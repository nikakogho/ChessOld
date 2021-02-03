using UnityEngine;

public class BoardSet
{
    struct FigureData
    {
        public Figure.FigureIndex index;
        public GameSide side;
        public bool isAscendedPawn;
        public bool moved;

        public FigureData(Figure figure)
        {
            isAscendedPawn = false;

            if (figure == null)
            {
                index = Figure.FigureIndex.None;
                side = null;
                moved = false;
                return;
            }

            foreach (Figure f in figure.side.pawns)
            {
                if (f == figure)
                {
                    isAscendedPawn = true;
                    break;
                }
            }

            moved = figure.moved;
            index = figure.Index;
            side = figure.side;
        }
    }

    FigureData[,] figures = new FigureData[8,8];
    public Vector2? lastMovedPawnTile = null;
    
    public BoardSet()
    {
        Board board = Board.instance;

        for (int x = 0; x < 8; x++)
        {
            for(int y = 0; y < 8; y++)
            {
                Figure figure = board.tiles[x, y].figure;

                figures[x, y] = new FigureData(figure);
            }
        }

        lastMovedPawnTile = Pawn.lastMovedPawn == null ? null : (Vector2?)new Vector2(Pawn.lastMovedPawn.x, Pawn.lastMovedPawn.y);
    }

    public void SetToBoard()
    {
        Board board = Board.instance;

        board.Clear();

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                BoardTile tile = board.tiles[x, y];
                FigureData data = figures[x, y];

                switch(data.index)
                {
                    case Figure.FigureIndex.Pawn:
                        tile.SpawnFigure(new Pawn(x, y, figures[x, y].side));
                        break;
                    case Figure.FigureIndex.Horse:
                        tile.SpawnFigure(new Horse(x, y, figures[x, y].side));
                        break;
                    case Figure.FigureIndex.Bishop:
                        tile.SpawnFigure(new Bishop(x, y, figures[x, y].side));
                        break;
                    case Figure.FigureIndex.Castle:
                        tile.SpawnFigure(new Castle(x, y, figures[x, y].side));
                        break;
                    case Figure.FigureIndex.Queen:
                        tile.SpawnFigure(new Queen(x, y, figures[x, y].side));
                        break;
                    case Figure.FigureIndex.King:
                        tile.SpawnFigure(new King(x, y, figures[x, y].side, null, null));
                        break;
                }

                if(tile.figure != null)
                {
                    tile.figure.moved = data.moved;

                    if (data.isAscendedPawn)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            if (data.side.pawns[i] == null) data.side.pawns[i] = tile.figure;
                        }
                    }
                    else
                    {
                        int index = 0;
                        switch (data.index)
                        {
                            case Figure.FigureIndex.Pawn:
                                for(int i = 0; i < 8; i++)
                                {
                                    if (data.side.pawns[i] == null) data.side.pawns[i] = tile.figure;
                                }
                                break;
                            case Figure.FigureIndex.Horse:
                                index = data.side.horses[0] == null ? 0 : 1;
                                data.side.horses[index] = (Horse)tile.figure;
                                break;
                            case Figure.FigureIndex.Bishop:
                                index = data.side.bishops[0] == null ? 0 : 1;
                                data.side.bishops[index] = (Bishop)tile.figure;
                                break;
                            case Figure.FigureIndex.Castle:
                                index = data.side.castles[0] == null ? 0 : 1;
                                data.side.castles[index] = (Castle)tile.figure;
                                break;
                            case Figure.FigureIndex.Queen:
                                tile.figure.side.queen = (Queen)tile.figure;
                                break;
                            case Figure.FigureIndex.King:
                                tile.figure.side.king = (King)tile.figure;
                                break;
                            default:
                                Debug.Log(data.index.ToString());
                                break;
                        }
                    }
                }
            }
        }

        GameSide.white.king.leftCastle = GameSide.white.castles[0];
        GameSide.white.king.rightCastle = GameSide.white.castles[1];

        GameSide.black.king.leftCastle = GameSide.black.castles[0];
        GameSide.black.king.rightCastle = GameSide.black.castles[1];

        GameSide.white.king.checkEffect.SetActive(GameSide.white.CheckForCheck());
        GameSide.black.king.checkEffect.SetActive(GameSide.black.CheckForCheck());

        if (lastMovedPawnTile != null)
        {
            Figure figureAtTile = board.tiles[(int)lastMovedPawnTile.Value.x, (int)lastMovedPawnTile.Value.y].figure;

            if (figureAtTile.Index == Figure.FigureIndex.Pawn)
            {
                Pawn.lastMovedPawn = (Pawn)figureAtTile;
                return;
            }
        }

        Pawn.lastMovedPawn = null;
    }
}
