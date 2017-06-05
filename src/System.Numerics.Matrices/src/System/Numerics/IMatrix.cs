namespace System.Numerics.Matrices
{
    interface IMatrix
    {
        double this[int col, int row] { get; set; }
        int Columns { get; }
        int Rows { get; }
    }
}
