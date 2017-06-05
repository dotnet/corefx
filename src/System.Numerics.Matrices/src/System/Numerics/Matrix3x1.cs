using System.Runtime.InteropServices;

namespace System.Numerics.Matrices
{
    /// <summary>
    /// Represents a matrix of double precision floating-point values defined by its number of columns and rows
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix3x1: IEquatable<Matrix3x1>, IMatrix
    {
        public const int ColumnCount = 3;
        public const int RowCount = 1;

        static Matrix3x1()
        {
            Zero = new Matrix3x1(0);
        }

        /// <summary>
        /// Constant Matrix3x1 with all values initialized to zero
        /// </summary>
        public static readonly Matrix3x1 Zero;

        /// <summary>
        /// Initializes a Matrix3x1 with all of it values specifically set
        /// </summary>
        /// <param name="m11">The column 1, row 1 value</param>
        /// <param name="m21">The column 2, row 1 value</param>
        /// <param name="m31">The column 3, row 1 value</param>
        public Matrix3x1(double m11, double m21, double m31)
        {
			M11 = m11; M21 = m21; M31 = m31; 
        }

        /// <summary>
        /// Initialized a Matrix3x1 with all values set to the same value
        /// </summary>
        /// <param name="value">The value to set all values to</param>
        public Matrix3x1(double value)
        {
			M11 = M21 = M31 = value;
        }

		public double M11;
		public double M21;
		public double M31;

        public unsafe double this[int col, int row]
        {
            get
            {
                if (col < 0 || col >= ColumnCount)
                    throw new ArgumentOutOfRangeException("col");
                if (row < 0 || row >= RowCount)
                    throw new ArgumentOutOfRangeException("col");

                fixed (Matrix3x1* p = &this)
                {
                    double* d = (double*)p;
                    return d[row * ColumnCount + col];
                }
            }
            set
            {
                if (col < 0 || col >= ColumnCount)
                    throw new ArgumentOutOfRangeException("col");
                if (row < 0 || row >= RowCount)
                    throw new ArgumentOutOfRangeException("col");

                fixed (Matrix3x1* p = &this)
                {
                    double* d = (double*)p;
                    d[row * ColumnCount + col] = value;
                }
            }
        }

        /// <summary>
        /// Gets the number of columns in the matrix
        /// </summary>
        public int Columns { get { return Matrix3x1.ColumnCount; } }
        /// <summary>
        /// Get the number of rows in the matrix
        /// </summary>
        public int Rows { get { return Matrix3x1.RowCount; } }


        public override bool Equals(object obj)
        {
            if (obj is Matrix3x1)
                return this == (Matrix3x1)obj;

            return false;
        }

        public bool Equals(Matrix3x1 other)
        {
            return this == other;
        }

        public unsafe override int GetHashCode()
        {
            fixed (Matrix3x1* p = &this)
            {
                int* x = (int*)p;
                unchecked
                {
                    return (x[00] ^ x[01]) + (x[02] ^ x[03]) + (x[04] ^ x[05]);
                }
            }
        }

        public override string ToString()
        {
            return String.Format("Matrix3x1: "
                               + "{{|{00}|{01}|{02}|}}"
                               , M11, M21, M31); 
        }

        /// <summary>
        /// Creates and returns a transposed matrix
        /// </summary>
        /// <returns>Matrix with transposed values</returns>
        public Matrix1x3 Transpose()
        {
            return new Matrix1x3(M11, 
                                 M21, 
                                 M31);
        }

        public static bool operator ==(Matrix3x1 matrix1, Matrix3x1 matrix2)
        {
            return (matrix1 == matrix2 || Math.Abs(matrix1.M11 - matrix2.M11) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M21 - matrix2.M21) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M31 - matrix2.M31) <= Double.Epsilon);
        }

        public static bool operator !=(Matrix3x1 matrix1, Matrix3x1 matrix2)
        {
            return Math.Abs(matrix1.M11 - matrix2.M11) > Double.Epsilon
                || Math.Abs(matrix1.M21 - matrix2.M21) > Double.Epsilon
                || Math.Abs(matrix1.M31 - matrix2.M31) > Double.Epsilon;
        }

        public static Matrix3x1 operator +(Matrix3x1 matrix1, Matrix3x1 matrix2)
        {
            double m11 = matrix1.M11 + matrix2.M11;
            double m21 = matrix1.M21 + matrix2.M21;
            double m31 = matrix1.M31 + matrix2.M31;

            return new Matrix3x1(m11, m21, m31);
        }

        public static Matrix3x1 operator -(Matrix3x1 matrix1, Matrix3x1 matrix2)
        {
            double m11 = matrix1.M11 - matrix2.M11;
            double m21 = matrix1.M21 - matrix2.M21;
            double m31 = matrix1.M31 - matrix2.M31;

            return new Matrix3x1(m11, m21, m31);
        }

        public static Matrix3x1 operator *(Matrix3x1 matrix, double scalar)
        {
            double m11 = matrix.M11 * scalar;
            double m21 = matrix.M21 * scalar;
            double m31 = matrix.M31 * scalar;

            return new Matrix3x1(m11, m21, m31);
        }

        public static Matrix3x1 operator *(double scalar, Matrix3x1 matrix)
        {
            double m11 = scalar * matrix.M11;
            double m21 = scalar * matrix.M21;
            double m31 = scalar * matrix.M31;

            return new Matrix3x1(m11, m21, m31);
        }

        public static Matrix2x1 operator *(Matrix3x1 matrix1, Matrix2x3 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12 + matrix1.M31 * matrix2.M13;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22 + matrix1.M31 * matrix2.M23;

            return new Matrix2x1(m11, m21);
        }
        public static Matrix3x1 operator *(Matrix3x1 matrix1, Matrix3x3 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12 + matrix1.M31 * matrix2.M13;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22 + matrix1.M31 * matrix2.M23;
            double m31 = matrix1.M11 * matrix2.M31 + matrix1.M21 * matrix2.M32 + matrix1.M31 * matrix2.M33;

            return new Matrix3x1(m11, m21, m31);
        }
        public static Matrix4x1 operator *(Matrix3x1 matrix1, Matrix4x3 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12 + matrix1.M31 * matrix2.M13;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22 + matrix1.M31 * matrix2.M23;
            double m31 = matrix1.M11 * matrix2.M31 + matrix1.M21 * matrix2.M32 + matrix1.M31 * matrix2.M33;
            double m41 = matrix1.M11 * matrix2.M41 + matrix1.M21 * matrix2.M42 + matrix1.M31 * matrix2.M43;

            return new Matrix4x1(m11, m21, m31, m41);
        }
        public static Matrix5x1 operator *(Matrix3x1 matrix1, Matrix5x3 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12 + matrix1.M31 * matrix2.M13;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22 + matrix1.M31 * matrix2.M23;
            double m31 = matrix1.M11 * matrix2.M31 + matrix1.M21 * matrix2.M32 + matrix1.M31 * matrix2.M33;
            double m41 = matrix1.M11 * matrix2.M41 + matrix1.M21 * matrix2.M42 + matrix1.M31 * matrix2.M43;
            double m51 = matrix1.M11 * matrix2.M51 + matrix1.M21 * matrix2.M52 + matrix1.M31 * matrix2.M53;

            return new Matrix5x1(m11, m21, m31, m41, m51);
        }
        public static Matrix6x1 operator *(Matrix3x1 matrix1, Matrix6x3 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12 + matrix1.M31 * matrix2.M13;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22 + matrix1.M31 * matrix2.M23;
            double m31 = matrix1.M11 * matrix2.M31 + matrix1.M21 * matrix2.M32 + matrix1.M31 * matrix2.M33;
            double m41 = matrix1.M11 * matrix2.M41 + matrix1.M21 * matrix2.M42 + matrix1.M31 * matrix2.M43;
            double m51 = matrix1.M11 * matrix2.M51 + matrix1.M21 * matrix2.M52 + matrix1.M31 * matrix2.M53;
            double m61 = matrix1.M11 * matrix2.M61 + matrix1.M21 * matrix2.M62 + matrix1.M31 * matrix2.M63;

            return new Matrix6x1(m11, m21, m31, m41, m51, m61);
        }
        public static Matrix7x1 operator *(Matrix3x1 matrix1, Matrix7x3 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12 + matrix1.M31 * matrix2.M13;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22 + matrix1.M31 * matrix2.M23;
            double m31 = matrix1.M11 * matrix2.M31 + matrix1.M21 * matrix2.M32 + matrix1.M31 * matrix2.M33;
            double m41 = matrix1.M11 * matrix2.M41 + matrix1.M21 * matrix2.M42 + matrix1.M31 * matrix2.M43;
            double m51 = matrix1.M11 * matrix2.M51 + matrix1.M21 * matrix2.M52 + matrix1.M31 * matrix2.M53;
            double m61 = matrix1.M11 * matrix2.M61 + matrix1.M21 * matrix2.M62 + matrix1.M31 * matrix2.M63;
            double m71 = matrix1.M11 * matrix2.M71 + matrix1.M21 * matrix2.M72 + matrix1.M31 * matrix2.M73;

            return new Matrix7x1(m11, m21, m31, m41, m51, m61, m71);
        }
        public static Matrix8x1 operator *(Matrix3x1 matrix1, Matrix8x3 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12 + matrix1.M31 * matrix2.M13;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22 + matrix1.M31 * matrix2.M23;
            double m31 = matrix1.M11 * matrix2.M31 + matrix1.M21 * matrix2.M32 + matrix1.M31 * matrix2.M33;
            double m41 = matrix1.M11 * matrix2.M41 + matrix1.M21 * matrix2.M42 + matrix1.M31 * matrix2.M43;
            double m51 = matrix1.M11 * matrix2.M51 + matrix1.M21 * matrix2.M52 + matrix1.M31 * matrix2.M53;
            double m61 = matrix1.M11 * matrix2.M61 + matrix1.M21 * matrix2.M62 + matrix1.M31 * matrix2.M63;
            double m71 = matrix1.M11 * matrix2.M71 + matrix1.M21 * matrix2.M72 + matrix1.M31 * matrix2.M73;
            double m81 = matrix1.M11 * matrix2.M81 + matrix1.M21 * matrix2.M82 + matrix1.M31 * matrix2.M83;

            return new Matrix8x1(m11, m21, m31, m41, m51, m61, m71, m81);
        }
    }
}
