using System.Runtime.InteropServices;

namespace System.Numerics.Matrices
{
    /// <summary>
    /// Represents a matrix of double precision floating-point values defined by its number of columns and rows
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix2x2: IEquatable<Matrix2x2>, IMatrix
    {
        public const int ColumnCount = 2;
        public const int RowCount = 2;

        static Matrix2x2()
        {
            Zero = new Matrix2x2(0);
			Identitiy = new Matrix2x2(1, 0, 
                                      0, 1);
        }

        /// <summary>
        /// Constant Matrix2x2 with all values initialized to zero
        /// </summary>
        public static readonly Matrix2x2 Zero;
        
        /// <summary>
        /// Constant Matrix2x2 with value intialized to the identity of a 2 x 2 matrix
        /// </summary>
        public static readonly Matrix2x2 Identitiy;

        /// <summary>
        /// Initializes a Matrix2x2 with all of it values specifically set
        /// </summary>
        /// <param name="m11">The column 1, row 1 value</param>
        /// <param name="m21">The column 2, row 1 value</param>
        /// <param name="m12">The column 1, row 2 value</param>
        /// <param name="m22">The column 2, row 2 value</param>
        public Matrix2x2(double m11, double m21, 
                         double m12, double m22)
        {
			M11 = m11; M21 = m21; 
			M12 = m12; M22 = m22; 
        }

        /// <summary>
        /// Initialized a Matrix2x2 with all values set to the same value
        /// </summary>
        /// <param name="value">The value to set all values to</param>
        public Matrix2x2(double value)
        {
			M11 = M21 = 
			M12 = M22 = value;
        }

		public double M11;
		public double M21;
		public double M12;
		public double M22;

        public unsafe double this[int col, int row]
        {
            get
            {
                if (col < 0 || col >= ColumnCount)
                    throw new ArgumentOutOfRangeException("col");
                if (row < 0 || row >= RowCount)
                    throw new ArgumentOutOfRangeException("col");

                fixed (Matrix2x2* p = &this)
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

                fixed (Matrix2x2* p = &this)
                {
                    double* d = (double*)p;
                    d[row * ColumnCount + col] = value;
                }
            }
        }

        /// <summary>
        /// Gets the number of columns in the matrix
        /// </summary>
        public int Columns { get { return Matrix2x2.ColumnCount; } }
        /// <summary>
        /// Get the number of rows in the matrix
        /// </summary>
        public int Rows { get { return Matrix2x2.RowCount; } }

        /// <summary>
        /// Gets a new Matrix1x2 containing the values of column 1
        /// </summary>
        public Matrix1x2 Column1 { get { return new Matrix1x2(M11, M12); } }
        /// <summary>
        /// Gets a new Matrix1x2 containing the values of column 2
        /// </summary>
        public Matrix1x2 Column2 { get { return new Matrix1x2(M21, M22); } }
        /// <summary>
        /// Gets a new Matrix2x1 containing the values of column 1
        /// </summary>
        public Matrix2x1 Row1 { get { return new Matrix2x1(M11, M21); } }
        /// <summary>
        /// Gets a new Matrix2x1 containing the values of column 2
        /// </summary>
        public Matrix2x1 Row2 { get { return new Matrix2x1(M12, M22); } }

        public override bool Equals(object obj)
        {
            if (obj is Matrix2x2)
                return this == (Matrix2x2)obj;

            return false;
        }

        public bool Equals(Matrix2x2 other)
        {
            return this == other;
        }

        public unsafe override int GetHashCode()
        {
            fixed (Matrix2x2* p = &this)
            {
                int* x = (int*)p;
                unchecked
                {
                    return (x[00] ^ x[01]) + (x[02] ^ x[03])
                         + (x[02] ^ x[03]) + (x[04] ^ x[05]);
                }
            }
        }

        public override string ToString()
        {
            return String.Format("Matrix2x2: "
                               + "{{|{00}|{01}|}}"
                               + "{{|{02}|{03}|}}"
                               , M11, M21
                               , M12, M22); 
        }

        /// <summary>
        /// Creates and returns a transposed matrix
        /// </summary>
        /// <returns>Matrix with transposed values</returns>
        public Matrix2x2 Transpose()
        {
            return new Matrix2x2(M11, M12, 
                                 M21, M22);
        }

        public static bool operator ==(Matrix2x2 matrix1, Matrix2x2 matrix2)
        {
            return (matrix1 == matrix2 || Math.Abs(matrix1.M11 - matrix2.M11) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M21 - matrix2.M21) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M12 - matrix2.M12) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M22 - matrix2.M22) <= Double.Epsilon);
        }

        public static bool operator !=(Matrix2x2 matrix1, Matrix2x2 matrix2)
        {
            return Math.Abs(matrix1.M11 - matrix2.M11) > Double.Epsilon
                || Math.Abs(matrix1.M21 - matrix2.M21) > Double.Epsilon
                || Math.Abs(matrix1.M12 - matrix2.M12) > Double.Epsilon
                || Math.Abs(matrix1.M22 - matrix2.M22) > Double.Epsilon;
        }

        public static Matrix2x2 operator +(Matrix2x2 matrix1, Matrix2x2 matrix2)
        {
            double m11 = matrix1.M11 + matrix2.M11;
            double m21 = matrix1.M21 + matrix2.M21;
            double m12 = matrix1.M12 + matrix2.M12;
            double m22 = matrix1.M22 + matrix2.M22;

            return new Matrix2x2(m11, m21, 
                                 m12, m22);
        }

        public static Matrix2x2 operator -(Matrix2x2 matrix1, Matrix2x2 matrix2)
        {
            double m11 = matrix1.M11 - matrix2.M11;
            double m21 = matrix1.M21 - matrix2.M21;
            double m12 = matrix1.M12 - matrix2.M12;
            double m22 = matrix1.M22 - matrix2.M22;

            return new Matrix2x2(m11, m21, 
                                 m12, m22);
        }

        public static Matrix2x2 operator *(Matrix2x2 matrix, double scalar)
        {
            double m11 = matrix.M11 * scalar;
            double m21 = matrix.M21 * scalar;
            double m12 = matrix.M12 * scalar;
            double m22 = matrix.M22 * scalar;

            return new Matrix2x2(m11, m21, 
                                 m12, m22);
        }

        public static Matrix2x2 operator *(double scalar, Matrix2x2 matrix)
        {
            double m11 = scalar * matrix.M11;
            double m21 = scalar * matrix.M21;
            double m12 = scalar * matrix.M12;
            double m22 = scalar * matrix.M22;

            return new Matrix2x2(m11, m21, 
                                 m12, m22);
        }

        public static Matrix1x2 operator *(Matrix2x2 matrix1, Matrix1x2 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12;
            double m12 = matrix1.M12 * matrix2.M11 + matrix1.M22 * matrix2.M12;

            return new Matrix1x2(m11, 
                                 m12);
        }
        public static Matrix2x2 operator *(Matrix2x2 matrix1, Matrix2x2 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22;
            double m12 = matrix1.M12 * matrix2.M11 + matrix1.M22 * matrix2.M12;
            double m22 = matrix1.M12 * matrix2.M21 + matrix1.M22 * matrix2.M22;

            return new Matrix2x2(m11, m21, 
                                 m12, m22);
        }
        public static Matrix3x2 operator *(Matrix2x2 matrix1, Matrix3x2 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22;
            double m31 = matrix1.M11 * matrix2.M31 + matrix1.M21 * matrix2.M32;
            double m12 = matrix1.M12 * matrix2.M11 + matrix1.M22 * matrix2.M12;
            double m22 = matrix1.M12 * matrix2.M21 + matrix1.M22 * matrix2.M22;
            double m32 = matrix1.M12 * matrix2.M31 + matrix1.M22 * matrix2.M32;

            return new Matrix3x2(m11, m21, m31, 
                                 m12, m22, m32);
        }
        public static Matrix4x2 operator *(Matrix2x2 matrix1, Matrix4x2 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22;
            double m31 = matrix1.M11 * matrix2.M31 + matrix1.M21 * matrix2.M32;
            double m41 = matrix1.M11 * matrix2.M41 + matrix1.M21 * matrix2.M42;
            double m12 = matrix1.M12 * matrix2.M11 + matrix1.M22 * matrix2.M12;
            double m22 = matrix1.M12 * matrix2.M21 + matrix1.M22 * matrix2.M22;
            double m32 = matrix1.M12 * matrix2.M31 + matrix1.M22 * matrix2.M32;
            double m42 = matrix1.M12 * matrix2.M41 + matrix1.M22 * matrix2.M42;

            return new Matrix4x2(m11, m21, m31, m41, 
                                 m12, m22, m32, m42);
        }
        public static Matrix5x2 operator *(Matrix2x2 matrix1, Matrix5x2 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22;
            double m31 = matrix1.M11 * matrix2.M31 + matrix1.M21 * matrix2.M32;
            double m41 = matrix1.M11 * matrix2.M41 + matrix1.M21 * matrix2.M42;
            double m51 = matrix1.M11 * matrix2.M51 + matrix1.M21 * matrix2.M52;
            double m12 = matrix1.M12 * matrix2.M11 + matrix1.M22 * matrix2.M12;
            double m22 = matrix1.M12 * matrix2.M21 + matrix1.M22 * matrix2.M22;
            double m32 = matrix1.M12 * matrix2.M31 + matrix1.M22 * matrix2.M32;
            double m42 = matrix1.M12 * matrix2.M41 + matrix1.M22 * matrix2.M42;
            double m52 = matrix1.M12 * matrix2.M51 + matrix1.M22 * matrix2.M52;

            return new Matrix5x2(m11, m21, m31, m41, m51, 
                                 m12, m22, m32, m42, m52);
        }
        public static Matrix6x2 operator *(Matrix2x2 matrix1, Matrix6x2 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22;
            double m31 = matrix1.M11 * matrix2.M31 + matrix1.M21 * matrix2.M32;
            double m41 = matrix1.M11 * matrix2.M41 + matrix1.M21 * matrix2.M42;
            double m51 = matrix1.M11 * matrix2.M51 + matrix1.M21 * matrix2.M52;
            double m61 = matrix1.M11 * matrix2.M61 + matrix1.M21 * matrix2.M62;
            double m12 = matrix1.M12 * matrix2.M11 + matrix1.M22 * matrix2.M12;
            double m22 = matrix1.M12 * matrix2.M21 + matrix1.M22 * matrix2.M22;
            double m32 = matrix1.M12 * matrix2.M31 + matrix1.M22 * matrix2.M32;
            double m42 = matrix1.M12 * matrix2.M41 + matrix1.M22 * matrix2.M42;
            double m52 = matrix1.M12 * matrix2.M51 + matrix1.M22 * matrix2.M52;
            double m62 = matrix1.M12 * matrix2.M61 + matrix1.M22 * matrix2.M62;

            return new Matrix6x2(m11, m21, m31, m41, m51, m61, 
                                 m12, m22, m32, m42, m52, m62);
        }
        public static Matrix7x2 operator *(Matrix2x2 matrix1, Matrix7x2 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22;
            double m31 = matrix1.M11 * matrix2.M31 + matrix1.M21 * matrix2.M32;
            double m41 = matrix1.M11 * matrix2.M41 + matrix1.M21 * matrix2.M42;
            double m51 = matrix1.M11 * matrix2.M51 + matrix1.M21 * matrix2.M52;
            double m61 = matrix1.M11 * matrix2.M61 + matrix1.M21 * matrix2.M62;
            double m71 = matrix1.M11 * matrix2.M71 + matrix1.M21 * matrix2.M72;
            double m12 = matrix1.M12 * matrix2.M11 + matrix1.M22 * matrix2.M12;
            double m22 = matrix1.M12 * matrix2.M21 + matrix1.M22 * matrix2.M22;
            double m32 = matrix1.M12 * matrix2.M31 + matrix1.M22 * matrix2.M32;
            double m42 = matrix1.M12 * matrix2.M41 + matrix1.M22 * matrix2.M42;
            double m52 = matrix1.M12 * matrix2.M51 + matrix1.M22 * matrix2.M52;
            double m62 = matrix1.M12 * matrix2.M61 + matrix1.M22 * matrix2.M62;
            double m72 = matrix1.M12 * matrix2.M71 + matrix1.M22 * matrix2.M72;

            return new Matrix7x2(m11, m21, m31, m41, m51, m61, m71, 
                                 m12, m22, m32, m42, m52, m62, m72);
        }
        public static Matrix8x2 operator *(Matrix2x2 matrix1, Matrix8x2 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22;
            double m31 = matrix1.M11 * matrix2.M31 + matrix1.M21 * matrix2.M32;
            double m41 = matrix1.M11 * matrix2.M41 + matrix1.M21 * matrix2.M42;
            double m51 = matrix1.M11 * matrix2.M51 + matrix1.M21 * matrix2.M52;
            double m61 = matrix1.M11 * matrix2.M61 + matrix1.M21 * matrix2.M62;
            double m71 = matrix1.M11 * matrix2.M71 + matrix1.M21 * matrix2.M72;
            double m81 = matrix1.M11 * matrix2.M81 + matrix1.M21 * matrix2.M82;
            double m12 = matrix1.M12 * matrix2.M11 + matrix1.M22 * matrix2.M12;
            double m22 = matrix1.M12 * matrix2.M21 + matrix1.M22 * matrix2.M22;
            double m32 = matrix1.M12 * matrix2.M31 + matrix1.M22 * matrix2.M32;
            double m42 = matrix1.M12 * matrix2.M41 + matrix1.M22 * matrix2.M42;
            double m52 = matrix1.M12 * matrix2.M51 + matrix1.M22 * matrix2.M52;
            double m62 = matrix1.M12 * matrix2.M61 + matrix1.M22 * matrix2.M62;
            double m72 = matrix1.M12 * matrix2.M71 + matrix1.M22 * matrix2.M72;
            double m82 = matrix1.M12 * matrix2.M81 + matrix1.M22 * matrix2.M82;

            return new Matrix8x2(m11, m21, m31, m41, m51, m61, m71, m81, 
                                 m12, m22, m32, m42, m52, m62, m72, m82);
        }
    }
}
