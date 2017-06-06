using System.Collections.Generic;
using UnityEngine;

public struct Trapezoid
{
    public Trapezoid(float x1, float x2, float z1, float z2)
    {
        xMin = x1;
        xMax = x2;
        zMin = z1;
        zMax = z2;
    }

    public float xMin;
    public float xMax;
    public float zMin;
    public float zMax;

    public float Top { get { return 2 * xMax; } }
    public float Bottom { get { return 2 * xMin; } }
    public float Height { get { return zMax - zMin; } }
    public float LegBase { get { return xMax - xMin; } }

    public Vector2 TopLeftCorner { get { return new Vector2(-xMax, zMax); } }
    public Vector2 TopRightCorner { get { return new Vector2(xMax, zMax); } }
    public Vector2 BottomLeftCorner { get { return new Vector2(-xMax, zMax); } }
    public Vector2 BottomRightCorner { get { return new Vector2(xMax, zMax); } }
}

public class SpectatorCell
{
    public SpectatorCell(Vector2 Pos)
    {
        Position = Pos;
        IsOccupied = false;
    }

    public Vector2 Position;
    public bool IsOccupied;
}

public class SpectatorGrid
{
    #region Properties

    private SpectatorCell[][] mCells;
    private List<SpectatorCell> mAvailableCells;
    private Vector2 mCellDimensions;

    #endregion


    #region Public Static Methods

    public static SpectatorGrid CreateGrid(Trapezoid Bounds, Vector2 CellDimensions)
    {
        if(Bounds.Height <= 0)
        {
            return null;
        }

        SpectatorCell[][] Cells;

        int NumRows = Mathf.CeilToInt(Bounds.Height / CellDimensions.y);
        float XZRatio = Bounds.LegBase / Bounds.Height;
        Cells = new SpectatorCell[NumRows][];
        int Row = 0;
        Vector2 CellPos = Bounds.TopRightCorner;

        do
        {
            float RowXMax = Bounds.xMin + (CellPos.y - Bounds.zMin - 1) * XZRatio;
            int Column = 0;
            int NumColumns = Mathf.CeilToInt(2 * RowXMax / CellDimensions.x);
            CellPos = new Vector2(-NumColumns * .5f * CellDimensions.x, CellPos.y);
            Cells[Row] = new SpectatorCell[NumColumns];

            while (Column < NumColumns)
            {
                Cells[Row][Column] = new SpectatorCell(CellPos);
                CellPos += CellDimensions.x * Vector2.right;
                Column++;
            }

            CellPos -= CellDimensions.y * Vector2.up;
            Row++;
        } while (Row < NumRows);
                
        return new SpectatorGrid(Cells, CellDimensions);
    }

    public static void DrawGrid(SpectatorGrid Grid, Vector2 CellDimensions)
    {
        if(Grid == null)
        {
            return;
        }

        for (int i = 0; i < Grid.GetRowCount(); i++)
        {
            Vector3[] Corners = new Vector3[4];
            for (int j = 0; j < Grid.GetRowLength(i); j++)
            {
                Corners[0] = new Vector3(Grid.GetCellPosition(i, j).x, 0, Grid.GetCellPosition(i, j).y);
                Corners[1] = Corners[0] + CellDimensions.x * Vector3.right;
                Corners[2] = Corners[1] - CellDimensions.y * Vector3.forward;
                Corners[3] = Corners[0] - CellDimensions.y * Vector3.forward;

                for(int c = 0; c < 4; c++)
                {
                    Debug.DrawLine(Corners[c % 4], Corners[(c + 1) % 4], new Color(c/4.0f, (c % 4)/4.0f, 1 - (c % 4)/4.0f));
                }               
            }
        }
    }

    #endregion


    #region Public Methods

    public int GetRowCount()
    {
        return mCells.Length;
    }

    public SpectatorGrid(SpectatorCell[][] Cells, Vector2 CellDimensions)
    {
        mCells = Cells;
        mCellDimensions = CellDimensions;
        mAvailableCells = new List<SpectatorCell>();
        for(int i = 0; i < mCells.Length; i++)
        {
            mAvailableCells.AddRange(mCells[i]);
        }
    }
    
    public int GetRowLength(int row)
    {
        return mCells[row].Length;
    }

    public Vector2 GetCellPosition(int row, int column)
    {
        return mCells[row][column].Position;
    }

    public Vector3 GetRandomCellPosition()
    {
        int RandomIndex = Random.Range(0, mAvailableCells.Count);
        Vector2 Pos2D = mAvailableCells[RandomIndex].Position;

        return new Vector3(Pos2D.x, 0, Pos2D.y);
    }

    public bool OccupyCellAtPosition(Vector3 Position)
    {
        SpectatorCell Cell = GetCellAtPosition(Position);
        if (mAvailableCells.Contains(Cell))
        {
            mAvailableCells.Remove(Cell);
            return true;
        }
        return false;       
    }

    public bool FreeCellAtPosition(Vector3 Position)
    {
        SpectatorCell Cell = GetCellAtPosition(Position);
        if (!mAvailableCells.Contains(Cell))
        {
            mAvailableCells.Add(Cell);
            return true;
        }
        return false;
    }

    #endregion


    #region Private Methods

    private SpectatorCell GetCellAtPosition(Vector3 Position)
    {
        int Row = (int)((mCells[0][0].Position.y - Position.z) / mCellDimensions.y);
        int Column = (int)((Position.x - mCells[Row][0].Position.x) / mCellDimensions.x);
        return mCells[Row][Column];
    }

    #endregion
}
