using System.Runtime.InteropServices;

namespace System.Numerics.Matrices
{
    /// <summary>
    /// Represents a matrix of double precision floating-point values defined by its number of columns and rows
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix2x1: IEquatable<Matrix2x1>, IMatrix
    {
        public const int ColumnCount = 2;
        public const int RowCount = 1;

        static Matrix2x1()
        {
            Zero = new Matrix2x1(0);
        }

        /// <summary>
        /// Constant Matrix2x1 with all values initialized to zero
        /// </summary>
        public static readonly Matrix2x1 Zero;

        /// <summary>
        /// Initializes a Matrix2x1 with all of it values specifically set
        /// </summary>
        /// <param name="m11">The column 1, row 1 value</param>
        /// <param name="m21">The column 2, row 1 value</param>
        public Matrix2x1(double m11, double m21)
        {
			M11 = m11; M21 = m21; 
        }

        /// <summary>
        /// Initialized a Matrix2x1 with all values set to the same value
        /// </summary>
        /// <param name="value">The value to set all values to</param>
        public Matrix2x1(double value)
        {
			M11 = M21 = value;
        }

		public double M11;
		public double M21;

        public unsafe double this[int col, int row]
        {
            get
            {
                if (col < 0 || col >= ColumnCount)
                    throw new ArgumentOutOfRangeException("col");
                if (row < 0 || row >= RowCount)
                    throw new ArgumentOutOfRangeException("col");

                fixed (Matrix2x1* p = &this)
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

                fixed (Matrix2x1* p = &this)
                {
                    double* d = (double*)p;
                    d[row * ColumnCount + col] = value;
                }
            }
        }

        /// <summary>
        /// Gets the number of columns in the matrix
        /// </summary>
        public int Columns { get { return Matrix2x1.ColumnCount; } }
        /// <summary>
        /// Get the number of rows in the matrix
        /// </summary>
        public int Rows { get { return Matrix2x1.RowCount; } }


        public override bool Equals(object obj)
        {
            if (obj is Matrix2x1)
                return this == (Matrix2x1)obj;

            return false;
        }

        public bool Equals(Matrix2x1 other)
        {
            return this == other;
        }

        public unsafe override int GetHashCode()
        {
            fixed (Matrix2x1* p = &this)
            {
                int* x = (int*)p;
                unchecked
                {
                    return (x[00] ^ x[01]) + (x[02] ^ x[03]);
                }
            }
        }

        public override string ToString()
        {
            return String.Format("Matrix2x1: "
                               + "{{|{00}|{01}|}}"
                               , M11, M21); 
        }

        /// <summary>
        /// Creates and returns a transposed matrix
        /// </summary>
        /// <returns>Matrix with transposed values</returns>
        public Matrix1x2 Transpose()
        {
            return new Matrix1x2(M11, 
                                 M21);
        }

        public static bool operator ==(Matrix2x1 matrix1, Matrix2x1 matrix2)
        {
            return (matrix1 == matrix2 || Math.Abs(matrix1.M11 - matrix2.M11) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M21 - matrix2.M21) <= Double.Epsilon);
        }

        public static bool operator !=(Matrix2x1 matrix1, Matrix2x1 matrix2)
        {
            return Math.Abs(matrix1.M11 - matrix2.M11) > Double.Epsilon
                || Math.Abs(matrix1.M21 - matrix2.M21) > Double.Epsilon;
        }

        public static Matrix2x1 operator +(Matrix2x1 matrix1, Matrix2x1 matrix2)
        {
            double m11 = matrix1.M11 + matrix2.M11;
            double m21 = matrix1.M21 + matrix2.M21;

            return new Matrix2x1(m11, m21);
        }

        public static Matrix2x1 operator -(Matrix2x1 matrix1, Matrix2x1 matrix2)
        {
            double m11 = matrix1.M11 - matrix2.M11;
            double m21 = matrix1.M21 - matrix2.M21;

            return new Matrix2x1(m11, m21);
        }

        public static Matrix2x1 operator *(Matrix2x1 matrix, double scalar)
        {
            double m11 = matrix.M11 * scalar;
            double m21 = matrix.M21 * scalar;

            return new Matrix2x1(m11, m21);
        }

        public static Matrix2x1 operator *(double scalar, Matrix2x1 matrix)
        {
            double m11 = scalar * matrix.M11;
            double m21 = scalar * matrix.M21;

            return new Matrix2x1(m11, m21);
        }

        public static Matrix2x1 operator *(Matrix2x1 matrix1, Matrix2x2 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22;

            return new Matrix2x1(m11, m21);
        }
        public static Matrix3x1 operator *(Matrix2x1 matrix1, Matrix3x2 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22;
            double m31 = matrix1.M11 * matrix2.M31 + matrix1.M21 * matrix2.M32;

            return new Matrix3x1(m11, m21, m31);
        }
        public static Matrix4x1 operator *(Matrix2x1 matrix1, Matrix4x2 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22;
            double m31 = matrix1.M11 * matrix2.M31 + matrix1.M21 * matrix2.M32;
            double m41 = matrix1.M11 * matrix2.M41 + matrix1.M21 * matrix2.M42;

            return new Matrix4x1(m11, m21, m31, m41);
        }
        public static Matrix5x1 operator *(Matrix2x1 matrix1, Matrix5x2 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22;
            double m31 = matrix1.M11 * matrix2.M31 + matrix1.M21 * matrix2.M32;
            double m41 = matrix1.M11 * matrix2.M41 + matrix1.M21 * matrix2.M42;
            double m51 = matrix1.M11 * matrix2.M51 + matrix1.M21 * matrix2.M52;

            return new Matrix5x1(m11, m21, m31, m41, m51);
        }
        public static Matrix6x1 operator *(Matrix2x1 matrix1, Matrix6x2 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22;
            double m31 = matrix1.M11 * matrix2.M31 + matrix1.M21 * matrix2.M32;
            double m41 = matrix1.M11 * matrix2.M41 + matrix1.M21 * matrix2.M42;
            double m51 = matrix1.M11 * matrix2.M51 + matrix1.M21 * matrix2.M52;
            double m61 = matrix1.M11 * matrix2.M61 + matrix1.M21 * matrix2.M62;

            return new Matrix6x1(m11, m21, m31, m41, m51, m61);
        }
        public static Matrix7x1 operator *(Matrix2x1 matrix1, Matrix7x2 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22;
            double m31 = matrix1.M11 * matrix2.M31 + matrix1.M21 * matrix2.M32;
            double m41 = matrix1.M11 * matrix2.M41 + matrix1.M21 * matrix2.M42;
            double m51 = matrix1.M11 * matrix2.M51 + matrix1.M21 * matrix2.M52;
            double m61 = matrix1.M11 * matrix2.M61 + matrix1.M21 * matrix2.M62;
            double m71 = matrix1.M11 * matrix2.M71 + matrix1.M21 * matrix2.M72;

            return new Matrix7x1(m11, m21, m31, m41, m51, m61, m71);
        }
        public static Matrix8x1 operator *(Matrix2x1 matrix1, Matrix8x2 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22;
            double m31 = matrix1.M11 * matrix2.M31 + matrix1.M21 * matrix2.M32;
            double m41 = matrix1.M11 * matrix2.M41 + matrix1.M21 * matrix2.M42;
            double m51 = matrix1.M11 * matrix2.M51 + matrix1.M21 * matrix2.M52;
            double m61 = matrix1.M11 * matrix2.M61 + matrix1.M21 * matrix2.M62;
            double m71 = matrix1.M11 * matrix2.M71 + matrix1.M21 * matrix2.M72;
            double m81 = matrix1.M11 * matrix2.M81 + matrix1.M21 * matrix2.M82;

            return new Matrix8x1(m11, m21, m31, m41, m51, m61, m71, m81);
        }
    }
}
