using System.Runtime.InteropServices;

namespace System.Numerics.Matrices
{
    /// <summary>
    /// Represents a matrix of double precision floating-point values defined by its number of columns and rows
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix1x2: IEquatable<Matrix1x2>, IMatrix
    {
        public const int ColumnCount = 1;
        public const int RowCount = 2;

        static Matrix1x2()
        {
            Zero = new Matrix1x2(0);
        }

        /// <summary>
        /// Constant Matrix1x2 with all values initialized to zero
        /// </summary>
        public static readonly Matrix1x2 Zero;

        /// <summary>
        /// Initializes a Matrix1x2 with all of it values specifically set
        /// </summary>
        /// <param name="m11">The column 1, row 1 value</param>
        /// <param name="m12">The column 1, row 2 value</param>
        public Matrix1x2(double m11, 
                         double m12)
        {
			M11 = m11; 
			M12 = m12; 
        }

        /// <summary>
        /// Initialized a Matrix1x2 with all values set to the same value
        /// </summary>
        /// <param name="value">The value to set all values to</param>
        public Matrix1x2(double value)
        {
			M11 = 
			M12 = value;
        }

		public double M11;
		public double M12;

        public unsafe double this[int col, int row]
        {
            get
            {
                if (col < 0 || col >= ColumnCount)
                    throw new ArgumentOutOfRangeException("col");
                if (row < 0 || row >= RowCount)
                    throw new ArgumentOutOfRangeException("col");

                fixed (Matrix1x2* p = &this)
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

                fixed (Matrix1x2* p = &this)
                {
                    double* d = (double*)p;
                    d[row * ColumnCount + col] = value;
                }
            }
        }

        /// <summary>
        /// Gets the number of columns in the matrix
        /// </summary>
        public int Columns { get { return Matrix1x2.ColumnCount; } }
        /// <summary>
        /// Get the number of rows in the matrix
        /// </summary>
        public int Rows { get { return Matrix1x2.RowCount; } }


        public override bool Equals(object obj)
        {
            if (obj is Matrix1x2)
                return this == (Matrix1x2)obj;

            return false;
        }

        public bool Equals(Matrix1x2 other)
        {
            return this == other;
        }

        public unsafe override int GetHashCode()
        {
            fixed (Matrix1x2* p = &this)
            {
                int* x = (int*)p;
                unchecked
                {
                    return (x[00] ^ x[01])
                         + (x[01] ^ x[02]);
                }
            }
        }

        public override string ToString()
        {
            return String.Format("Matrix1x2: "
                               + "{{|{00}|}}"
                               + "{{|{01}|}}"
                               , M11
                               , M12); 
        }

        /// <summary>
        /// Creates and returns a transposed matrix
        /// </summary>
        /// <returns>Matrix with transposed values</returns>
        public Matrix2x1 Transpose()
        {
            return new Matrix2x1(M11, M12);
        }

        public static bool operator ==(Matrix1x2 matrix1, Matrix1x2 matrix2)
        {
            return (matrix1 == matrix2 || Math.Abs(matrix1.M11 - matrix2.M11) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M12 - matrix2.M12) <= Double.Epsilon);
        }

        public static bool operator !=(Matrix1x2 matrix1, Matrix1x2 matrix2)
        {
            return Math.Abs(matrix1.M11 - matrix2.M11) > Double.Epsilon
                || Math.Abs(matrix1.M12 - matrix2.M12) > Double.Epsilon;
        }

        public static Matrix1x2 operator +(Matrix1x2 matrix1, Matrix1x2 matrix2)
        {
            double m11 = matrix1.M11 + matrix2.M11;
            double m12 = matrix1.M12 + matrix2.M12;

            return new Matrix1x2(m11, 
                                 m12);
        }

        public static Matrix1x2 operator -(Matrix1x2 matrix1, Matrix1x2 matrix2)
        {
            double m11 = matrix1.M11 - matrix2.M11;
            double m12 = matrix1.M12 - matrix2.M12;

            return new Matrix1x2(m11, 
                                 m12);
        }

        public static Matrix1x2 operator *(Matrix1x2 matrix, double scalar)
        {
            double m11 = matrix.M11 * scalar;
            double m12 = matrix.M12 * scalar;

            return new Matrix1x2(m11, 
                                 m12);
        }

        public static Matrix1x2 operator *(double scalar, Matrix1x2 matrix)
        {
            double m11 = scalar * matrix.M11;
            double m12 = scalar * matrix.M12;

            return new Matrix1x2(m11, 
                                 m12);
        }

        public static Matrix2x2 operator *(Matrix1x2 matrix1, Matrix2x1 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11;
            double m21 = matrix1.M11 * matrix2.M21;
            double m12 = matrix1.M12 * matrix2.M11;
            double m22 = matrix1.M12 * matrix2.M21;

            return new Matrix2x2(m11, m21, 
                                 m12, m22);
        }
        public static Matrix3x2 operator *(Matrix1x2 matrix1, Matrix3x1 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11;
            double m21 = matrix1.M11 * matrix2.M21;
            double m31 = matrix1.M11 * matrix2.M31;
            double m12 = matrix1.M12 * matrix2.M11;
            double m22 = matrix1.M12 * matrix2.M21;
            double m32 = matrix1.M12 * matrix2.M31;

            return new Matrix3x2(m11, m21, m31, 
                                 m12, m22, m32);
        }
        public static Matrix4x2 operator *(Matrix1x2 matrix1, Matrix4x1 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11;
            double m21 = matrix1.M11 * matrix2.M21;
            double m31 = matrix1.M11 * matrix2.M31;
            double m41 = matrix1.M11 * matrix2.M41;
            double m12 = matrix1.M12 * matrix2.M11;
            double m22 = matrix1.M12 * matrix2.M21;
            double m32 = matrix1.M12 * matrix2.M31;
            double m42 = matrix1.M12 * matrix2.M41;

            return new Matrix4x2(m11, m21, m31, m41, 
                                 m12, m22, m32, m42);
        }
        public static Matrix5x2 operator *(Matrix1x2 matrix1, Matrix5x1 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11;
            double m21 = matrix1.M11 * matrix2.M21;
            double m31 = matrix1.M11 * matrix2.M31;
            double m41 = matrix1.M11 * matrix2.M41;
            double m51 = matrix1.M11 * matrix2.M51;
            double m12 = matrix1.M12 * matrix2.M11;
            double m22 = matrix1.M12 * matrix2.M21;
            double m32 = matrix1.M12 * matrix2.M31;
            double m42 = matrix1.M12 * matrix2.M41;
            double m52 = matrix1.M12 * matrix2.M51;

            return new Matrix5x2(m11, m21, m31, m41, m51, 
                                 m12, m22, m32, m42, m52);
        }
        public static Matrix6x2 operator *(Matrix1x2 matrix1, Matrix6x1 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11;
            double m21 = matrix1.M11 * matrix2.M21;
            double m31 = matrix1.M11 * matrix2.M31;
            double m41 = matrix1.M11 * matrix2.M41;
            double m51 = matrix1.M11 * matrix2.M51;
            double m61 = matrix1.M11 * matrix2.M61;
            double m12 = matrix1.M12 * matrix2.M11;
            double m22 = matrix1.M12 * matrix2.M21;
            double m32 = matrix1.M12 * matrix2.M31;
            double m42 = matrix1.M12 * matrix2.M41;
            double m52 = matrix1.M12 * matrix2.M51;
            double m62 = matrix1.M12 * matrix2.M61;

            return new Matrix6x2(m11, m21, m31, m41, m51, m61, 
                                 m12, m22, m32, m42, m52, m62);
        }
        public static Matrix7x2 operator *(Matrix1x2 matrix1, Matrix7x1 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11;
            double m21 = matrix1.M11 * matrix2.M21;
            double m31 = matrix1.M11 * matrix2.M31;
            double m41 = matrix1.M11 * matrix2.M41;
            double m51 = matrix1.M11 * matrix2.M51;
            double m61 = matrix1.M11 * matrix2.M61;
            double m71 = matrix1.M11 * matrix2.M71;
            double m12 = matrix1.M12 * matrix2.M11;
            double m22 = matrix1.M12 * matrix2.M21;
            double m32 = matrix1.M12 * matrix2.M31;
            double m42 = matrix1.M12 * matrix2.M41;
            double m52 = matrix1.M12 * matrix2.M51;
            double m62 = matrix1.M12 * matrix2.M61;
            double m72 = matrix1.M12 * matrix2.M71;

            return new Matrix7x2(m11, m21, m31, m41, m51, m61, m71, 
                                 m12, m22, m32, m42, m52, m62, m72);
        }
        public static Matrix8x2 operator *(Matrix1x2 matrix1, Matrix8x1 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11;
            double m21 = matrix1.M11 * matrix2.M21;
            double m31 = matrix1.M11 * matrix2.M31;
            double m41 = matrix1.M11 * matrix2.M41;
            double m51 = matrix1.M11 * matrix2.M51;
            double m61 = matrix1.M11 * matrix2.M61;
            double m71 = matrix1.M11 * matrix2.M71;
            double m81 = matrix1.M11 * matrix2.M81;
            double m12 = matrix1.M12 * matrix2.M11;
            double m22 = matrix1.M12 * matrix2.M21;
            double m32 = matrix1.M12 * matrix2.M31;
            double m42 = matrix1.M12 * matrix2.M41;
            double m52 = matrix1.M12 * matrix2.M51;
            double m62 = matrix1.M12 * matrix2.M61;
            double m72 = matrix1.M12 * matrix2.M71;
            double m82 = matrix1.M12 * matrix2.M81;

            return new Matrix8x2(m11, m21, m31, m41, m51, m61, m71, m81, 
                                 m12, m22, m32, m42, m52, m62, m72, m82);
        }
    }
}
