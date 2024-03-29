﻿using TransportTask.Models;

namespace TransportTask;

public class TransportTaskSolver
{
    private readonly IPrimalBasisSearcher _primalBasisSearcher;

    public TransportTaskSolver(IPrimalBasisSearcher primalBasisSearcher) => _primalBasisSearcher = primalBasisSearcher;

    public decimal Solve(TransportTaskTable table)
    {
        _primalBasisSearcher.SearchBasis(table);
        EnsureIsValidPrimalBasis(table);

        while (true)
        {
            FillWeights(table);

            if (table.Cells.Any(cell => cell.Mark > 0))
            {
                ToNewBasis(table);

                // var consoleView = string.Join("\n", table.Rows
                //     .Select(row => string.Join("\t", row.Cells.Select(cell => cell.Q switch
                //     {
                //         QMark.Add => "Q+",
                //         QMark.Subtract => "Q-",
                //         QMark.Empty => cell.IsBases ? "X" : "_",
                //         _ => throw new ArgumentOutOfRangeException()
                //     }))));
                // Console.WriteLine(consoleView + "\n\n\n\n" );

                table.ClearWeights();
            }
            else
                break;
        }

        var result = table.Cells.Where(cell => cell.IsBases).Sum(cell => cell.Cost * cell.X!.Value);

        return result;
    }

    private static void ToNewBasis(TransportTaskTable table)
    {
        static void Fill(TransportTaskCell cell)
        {
            var crossCells = cell.Row.Cells.Concat(cell.Column.Cells).ToArray();

            cell.Q = crossCells.Any(cell => cell.Q is QMark.Add) ?
                QMark.Subtract :
                QMark.Add;
        }

        var newBasisCell = table.Cells.MaxBy(cell => cell.Mark);
        newBasisCell!.Q = QMark.Add;

        bool CanUpdate(TransportTaskCell cell) => cell.Q is QMark.Empty &&
                                                  (QMarksCount(QMark.Add, cell), QMarksCount(QMark.Subtract, cell)) is
                                                      ((0, 0), (0, 1)) or
                                                      ((0, 0), (1, 0)) or
                                                      ((0, 0), (1, 1)) or
                                                      ((0, 1), (0, 0)) or
                                                      ((1, 0), (0, 0)) or
                                                      ((1, 1), (0, 0));

        (int InRow, int InColumn) QMarksCount(QMark qValue, TransportTaskCell cell) =>
            (cell.Row.Cells.Count(rowCell => rowCell.Q == qValue),
                cell.Column.Cells.Count(columnCell => columnCell.Q == qValue));

        var basisCells = table.Cells
            .Where(cell => cell.IsBases &&
                           cell.Row.Cells.Count(rowCell => rowCell.IsBases || rowCell == newBasisCell) > 1 &&
                           cell.Column.Cells.Count(columnCell => columnCell.IsBases || columnCell == newBasisCell) > 1)
            .Append(newBasisCell)
            .ToArray();
        ;
        while (basisCells.Any(CanUpdate))
            foreach (var cell in basisCells
                         .Where(CanUpdate)
                         .OrderByDescending(cell => QMarksCount(QMark.Add, cell) is (1, 1) ||
                                                    QMarksCount(QMark.Subtract, cell) is (1, 1))
                         .ToArray())

                if (CanUpdate(cell))
                    Fill(cell);

        var excludedCell = basisCells.Where(cell => cell.Q is QMark.Subtract).MinBy(cell => cell.X);
        var qValue = excludedCell!.X!.Value;

        excludedCell.X = null;

        foreach (var cell in table.Cells.Where(cell => cell.Q is not QMark.Empty))
            if (cell.Q is QMark.Add)
                cell.X = cell.X is null ?
                    qValue :
                    cell.X + qValue;
            else
                cell.X -= qValue;
    }

    private static void FillWeights(TransportTaskTable table)
    {
        var basisCells = table.Cells.Where(cell => cell.IsBases).ToArray();

        basisCells.First().Row.SetU(0);

        while (table.Cells.Any(Extentions.CanFillWeight))
            if (basisCells.Any(Extentions.CanFillWeight))
                foreach (var cell in basisCells.Where(Extentions.CanFillWeight))
                    cell.FillWeight();
            else
                basisCells.First(cell => cell.U is null && cell.V is null).Row.SetU(0);
    }

    private static void EnsureIsValidPrimalBasis(TransportTaskTable table)
    {
        if (table.Cells.Count(cell => cell.IsBases) != table.N + table.M - 1)
            throw new Exception("Incorrect basis cells count.");
    }
}
