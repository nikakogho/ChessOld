using System.Collections.Generic;

public static class AdditionalFunctions
{
    public static void Undo(this List<BoardSet> sets)
    {
        if (sets.Count < 2) return;

        sets.RemoveAt(sets.Count - 1);
        sets[sets.Count - 1].SetToBoard();
    }

    public static T Last<T>(this List<T> list)
    {
        return list[list.Count - 1];
    }
}
