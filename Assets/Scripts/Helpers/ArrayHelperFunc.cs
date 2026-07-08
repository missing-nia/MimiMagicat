namespace Magicat.Helpers
{
    public static class ArrayHelperFunc
    {
        public static bool TryGetElement<T>(this T[] array, int index, out T element)
        {
            if (index < array.Length)
            {
                element = array[index];
                return true;
            }
            element = default(T);
            return false;
        }

        public static bool TryGetElement2D<T>(this T[,] array, int x, int y, out T element)
        {
            if (x < array.GetLength(0) && y < array.GetLength(1) && x >= 0 && y >= 0)
            {
                element = array[x,y];
                return true;
            }
            element = default(T);
            return false;
        }
    }
}
