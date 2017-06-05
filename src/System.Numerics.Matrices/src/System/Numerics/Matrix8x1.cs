using System.Runtime.InteropServices;

namespace System.Numerics.Matrices
{
    /// <summary>
    /// Represents a matrix of double precision floating-point values defined by its number of columns and rows
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix8x1: IEquatable<Matrix8x1>, IMatrix
    {
        public const int ColumnCount = 8;
        public const int RowCount = 1;

        static Matrix8x1()
        {
            Zero = new Matrix8x1(0);
        }

        /// <summary>
        /// Constant Matrix8x1 with all values initialized to zero
        /// </summary>
        public static readonly Matrix8x1 Zero;

        /// <summary>
        /// Initializes a Matrix8x1 with all of it values specifically set
        /// </summary>
        /// <param name="m11">The column 1, row 1 value</param>
        /// <param name="m21">The column 2, row 1 value</param>
        /// <param name="m31">The column 3, row 1 value</param>
        /// <param name="m41">The column 4, row 1 value</param>
        /// <param name="m51">The column 5, row 1 value</param>
        /// <param name="m61">The column 6, row 1 value</param>
        /// <param name="m71">The column 7, row 1 value</param>
        /// <param name="m81">The column 8, row 1 value</param>
        public Matrix8x1(double m11, double m21, double m31, double m41, double m51, double m61, double m71, double m81)
        {
			M11 = m11; M21 = m21; M31 = m31; M41 = m41; M51 = m51; M61 = m61; M71 = m71; M81 = m81; 
        }

        /// <summary>
        /// Initialized a Matrix8x1 with all values set to the same value
        /// </summary>
        /// <param name="value">The value to set all values to</param>
        public Matrix8x1(double value)
        {
			M11 = M21 = M31 = M41 = M51 = M61 = M71 = M81 = value;
        }

		public double M11;
		public double M21;
		public double M31;
		public double M41;
		public double M51;
		public double M61;
		public double M71;
		public double M81;

        public unsafe double this[int col, int row]
        {
            get
            {
                if (col < 0 || col >= ColumnCount)
                    throw new ArgumentOutOfRangeException("col");
                if (row < 0 || row >= RowCount)
                    throw new ArgumentOutOfRangeException("col");

                fixed (Matrix8x1* p = &this)
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

                fixed (Matrix8x1* p = &this)
                {
                    double* d = (double*)p;
                    d[row * ColumnCount + col] = value;
                }
            }
        }

        /// <summary>
        /// Gets the number of columns in the matrix
        /// </summary>
        public int Columns { get { return Matrix8x1.ColumnCount; } }
        /// <summary>
        /// Get the number of rows in the matrix
        /// </summary>
        public int Rows { get { return Matrix8x1.RowCount; } }


        public override bool Equals(object obj)
        {
            if (obj is Matrix8x1)
                return this == (Matrix8x1)obj;

            return false;
        }

        public bool Equals(Matrix8x1 other)
        {
            return this == other;
        }

        public unsafe override int GetHashCode()
        {
            fixed (Matrix8x1* p = &this)
            {
                int* x = (int*)p;
                unchecked
                {
                    return (x[00] ^ x[01]) + (x[02] ^ x[03]) + (x[04] ^ x[05]) + (x[06] ^ x[07]) + (x[08] ^ x[09]) + (x[10] ^ x[11]) + (x[12] ^ x[13]) + (x[14] ^ x[15]);
                }
            }
        }

        public override string ToString()
        {
            return String.Format("Matrix8x1: "
                               + "{{|{00}|{01}|{02}|{03}|{04}|{05}|{06}|{07}|}}"
                               , M11, M21, M31, M41, M51, M61, M71, M81); 
        }

        /// <summary>
        /// Creates and returns a transposed matrix
        /// </summary>
        /// <returns>Matrix with transposed values</returns>
        public Matrix1x8 Transpose()
        {
            return new Matrix1x8(M11, 
                                 M21, 
                                 M31, 
                                 M41, 
                                 M51, 
                                 M61, 
                                 M71, 
                                 M81);
        }

        public static bool operator ==(Matrix8x1 matrix1, Matrix8x1 matrix2)
        {
            return (matrix1 == matrix2 || Math.Abs(matrix1.M11 - matrix2.M11) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M21 - matrix2.M21) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M31 - matrix2.M31) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M41 - matrix2.M41) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M51 - matrix2.M51) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M61 - matrix2.M61) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M71 - matrix2.M71) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M81 - matrix2.M81) <= Double.Epsilon);
        }

        public static bool operator !=(Matrix8x1 matrix1, Matrix8x1 matrix2)
        {
            return Math.Abs(matrix1.M11 - matrix2.M11) > Double.Epsilon
                || Math.Abs(matrix1.M21 - matrix2.M21) > Double.Epsilon
                || Math.Abs(matrix1.M31 - matrix2.M31) > Double.Epsilon
                || Math.Abs(matrix1.M41 - matrix2.M41) > Double.Epsilon
                || Math.Abs(matrix1.M51 - matrix2.M51) > Double.Epsilon
                || Math.Abs(matrix1.M61 - matrix2.M61) > Double.Epsilon
                || Math.Abs(matrix1.M71 - matrix2.M71) > Double.Epsilon
                || Math.Abs(matrix1.M81 - matrix2.M81) > Double.Epsilon;
        }

        public static Matrix8x1 operator +(Matrix8x1 matrix1, Matrix8x1 matrix2)
        {
            double m11 = matrix1.M11 + matrix2.M11;
            double m21 = matrix1.M21 + matrix2.M21;
            double m31 = matrix1.M31 + matrix2.M31;
            double m41 = matrix1.M41 + matrix2.M41;
            double m51 = matrix1.M51 + matrix2.M51;
            double m61 = matrix1.M61 + matrix2.M61;
            double m71 = matrix1.M71 + matrix2.M71;
            double m81 = matrix1.M81 + matrix2.M81;

            return new Matrix8x1(m11, m21, m31, m41, m51, m61, m71, m81);
        }

        public static Matrix8x1 operator -(Matrix8x1 matrix1, Matrix8x1 matrix2)
        {
            double m11 = matrix1.M11 - matrix2.M11;
            double m21 = matrix1.M21 - matrix2.M21;
            double m31 = matrix1.M31 - matrix2.M31;
            double m41 = matrix1.M41 - matrix2.M41;
            double m51 = matrix1.M51 - matrix2.M51;
            double m61 = matrix1.M61 - matrix2.M61;
            double m71 = matrix1.M71 - matrix2.M71;
            double m81 = matrix1.M81 - matrix2.M81;

            return new Matrix8x1(m11, m21, m31, m41, m51, m61, m71, m81);
        }

        public static Matrix8x1 operator *(Matrix8x1 matrix, double scalar)
        {
            double m11 = matrix.M11 * scalar;
            double m21 = matrix.M21 * scalar;
            double m31 = matrix.M31 * scalar;
            double m41 = matrix.M41 * scalar;
            double m51 = matrix.M51 * scalar;
            double m61 = matrix.M61 * scalar;
            double m71 = matrix.M71 * scalar;
            double m81 = matrix.M81 * scalar;

            return new Matrix8x1(m11, m21, m31, m41, m51, m61, m71, m81);
        }

        public static Matrix8x1 operator *(double scalar, Matrix8x1 matrix)
        {
            double m11 = scalar * matrix.M11;
            double m21 = scalar * matrix.M21;
            double m31 = scalar * matrix.M31;
            double m41 = scalar * matrix.M41;
            double m51 = scalar * matrix.M51;
            double m61 = scalar * matrix.M61;
            double m71 = scalar * matrix.M71;
            double m81 = scalar * matrix.M81;

            return new Matrix8x1(m11, m21, m31, m41, m51, m61, m71, m81);
        }

        public static Matrix2x1 operator *(Matrix8x1 matrix1, Matrix2x8 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12 + matrix1.M31 * matrix2.M13 + matrix1.M41 * matrix2.M14 + matrix1.M51 * matrix2.M15 + matrix1.M61 * matrix2.M16 + matrix1.M71 * matrix2.M17 + matrix1.M81 * matrix2.M18;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22 + matrix1.M31 * matrix2.M23 + matrix1.M41 * matrix2.M24 + matrix1.M51 * matrix2.M25 + matrix1.M61 * matrix2.M26 + matrix1.M71 * matrix2.M27 + matrix1.M81 * matrix2.M28;

            return new Matrix2x1(m11, m21);
        }
        public static Matrix3x1 operator *(Matrix8x1 matrix1, Matrix3x8 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12 + matrix1.M31 * matrix2.M13 + matrix1.M41 * matrix2.M14 + matrix1.M51 * matrix2.M15 + matrix1.M61 * matrix2.M16 + matrix1.M71 * matrix2.M17 + matrix1.M81 * matrix2.M18;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22 + matrix1.M31 * matrix2.M23 + matrix1.M41 * matrix2.M24 + matrix1.M51 * matrix2.M25 + matrix1.M61 * matrix2.M26 + matrix1.M71 * matrix2.M27 + matrix1.M81 * matrix2.M28;
            double m31 = matrix1.M11 * matrix2.M31 + matrix1.M21 * matrix2.M32 + matrix1.M31 * matrix2.M33 + matrix1.M41 * matrix2.M34 + matrix1.M51 * matrix2.M35 + matrix1.M61 * matrix2.M36 + matrix1.M71 * matrix2.M37 + matrix1.M81 * matrix2.M38;

            return new Matrix3x1(m11, m21, m31);
        }
        public static Matrix4x1 operator *(Matrix8x1 matrix1, Matrix4x8 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12 + matrix1.M31 * matrix2.M13 + matrix1.M41 * matrix2.M14 + matrix1.M51 * matrix2.M15 + matrix1.M61 * matrix2.M16 + matrix1.M71 * matrix2.M17 + matrix1.M81 * matrix2.M18;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22 + matrix1.M31 * matrix2.M23 + matrix1.M41 * matrix2.M24 + matrix1.M51 * matrix2.M25 + matrix1.M61 * matrix2.M26 + matrix1.M71 * matrix2.M27 + matrix1.M81 * matrix2.M28;
            double m31 = matrix1.M11 * matrix2.M31 + matrix1.M21 * matrix2.M32 + matrix1.M31 * matrix2.M33 + matrix1.M41 * matrix2.M34 + matrix1.M51 * matrix2.M35 + matrix1.M61 * matrix2.M36 + matrix1.M71 * matrix2.M37 + matrix1.M81 * matrix2.M38;
            double m41 = matrix1.M11 * matrix2.M41 + matrix1.M21 * matrix2.M42 + matrix1.M31 * matrix2.M43 + matrix1.M41 * matrix2.M44 + matrix1.M51 * matrix2.M45 + matrix1.M61 * matrix2.M46 + matrix1.M71 * matrix2.M47 + matrix1.M81 * matrix2.M48;

            return new Matrix4x1(m11, m21, m31, m41);
        }
        public static Matrix5x1 operator *(Matrix8x1 matrix1, Matrix5x8 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12 + matrix1.M31 * matrix2.M13 + matrix1.M41 * matrix2.M14 + matrix1.M51 * matrix2.M15 + matrix1.M61 * matrix2.M16 + matrix1.M71 * matrix2.M17 + matrix1.M81 * matrix2.M18;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22 + matrix1.M31 * matrix2.M23 + matrix1.M41 * matrix2.M24 + matrix1.M51 * matrix2.M25 + matrix1.M61 * matrix2.M26 + matrix1.M71 * matrix2.M27 + matrix1.M81 * matrix2.M28;
            double m31 = matrix1.M11 * matrix2.M31 + matrix1.M21 * matrix2.M32 + matrix1.M31 * matrix2.M33 + matrix1.M41 * matrix2.M34 + matrix1.M51 * matrix2.M35 + matrix1.M61 * matrix2.M36 + matrix1.M71 * matrix2.M37 + matrix1.M81 * matrix2.M38;
            double m41 = matrix1.M11 * matrix2.M41 + matrix1.M21 * matrix2.M42 + matrix1.M31 * matrix2.M43 + matrix1.M41 * matrix2.M44 + matrix1.M51 * matrix2.M45 + matrix1.M61 * matrix2.M46 + matrix1.M71 * matrix2.M47 + matrix1.M81 * matrix2.M48;
            double m51 = matrix1.M11 * matrix2.M51 + matrix1.M21 * matrix2.M52 + matrix1.M31 * matrix2.M53 + matrix1.M41 * matrix2.M54 + matrix1.M51 * matrix2.M55 + matrix1.M61 * matrix2.M56 + matrix1.M71 * matrix2.M57 + matrix1.M81 * matrix2.M58;

            return new Matrix5x1(m11, m21, m31, m41, m51);
        }
        public static Matrix6x1 operator *(Matrix8x1 matrix1, Matrix6x8 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12 + matrix1.M31 * matrix2.M13 + matrix1.M41 * matrix2.M14 + matrix1.M51 * matrix2.M15 + matrix1.M61 * matrix2.M16 + matrix1.M71 * matrix2.M17 + matrix1.M81 * matrix2.M18;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22 + matrix1.M31 * matrix2.M23 + matrix1.M41 * matrix2.M24 + matrix1.M51 * matrix2.M25 + matrix1.M61 * matrix2.M26 + matrix1.M71 * matrix2.M27 + matrix1.M81 * matrix2.M28;
            double m31 = matrix1.M11 * matrix2.M31 + matrix1.M21 * matrix2.M32 + matrix1.M31 * matrix2.M33 + matrix1.M41 * matrix2.M34 + matrix1.M51 * matrix2.M35 + matrix1.M61 * matrix2.M36 + matrix1.M71 * matrix2.M37 + matrix1.M81 * matrix2.M38;
            double m41 = matrix1.M11 * matrix2.M41 + matrix1.M21 * matrix2.M42 + matrix1.M31 * matrix2.M43 + matrix1.M41 * matrix2.M44 + matrix1.M51 * matrix2.M45 + matrix1.M61 * matrix2.M46 + matrix1.M71 * matrix2.M47 + matrix1.M81 * matrix2.M48;
            double m51 = matrix1.M11 * matrix2.M51 + matrix1.M21 * matrix2.M52 + matrix1.M31 * matrix2.M53 + matrix1.M41 * matrix2.M54 + matrix1.M51 * matrix2.M55 + matrix1.M61 * matrix2.M56 + matrix1.M71 * matrix2.M57 + matrix1.M81 * matrix2.M58;
            double m61 = matrix1.M11 * matrix2.M61 + matrix1.M21 * matrix2.M62 + matrix1.M31 * matrix2.M63 + matrix1.M41 * matrix2.M64 + matrix1.M51 * matrix2.M65 + matrix1.M61 * matrix2.M66 + matrix1.M71 * matrix2.M67 + matrix1.M81 * matrix2.M68;

            return new Matrix6x1(m11, m21, m31, m41, m51, m61);
        }
        public static Matrix7x1 operator *(Matrix8x1 matrix1, Matrix7x8 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12 + matrix1.M31 * matrix2.M13 + matrix1.M41 * matrix2.M14 + matrix1.M51 * matrix2.M15 + matrix1.M61 * matrix2.M16 + matrix1.M71 * matrix2.M17 + matrix1.M81 * matrix2.M18;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22 + matrix1.M31 * matrix2.M23 + matrix1.M41 * matrix2.M24 + matrix1.M51 * matrix2.M25 + matrix1.M61 * matrix2.M26 + matrix1.M71 * matrix2.M27 + matrix1.M81 * matrix2.M28;
            double m31 = matrix1.M11 * matrix2.M31 + matrix1.M21 * matrix2.M32 + matrix1.M31 * matrix2.M33 + matrix1.M41 * matrix2.M34 + matrix1.M51 * matrix2.M35 + matrix1.M61 * matrix2.M36 + matrix1.M71 * matrix2.M37 + matrix1.M81 * matrix2.M38;
            double m41 = matrix1.M11 * matrix2.M41 + matrix1.M21 * matrix2.M42 + matrix1.M31 * matrix2.M43 + matrix1.M41 * matrix2.M44 + matrix1.M51 * matrix2.M45 + matrix1.M61 * matrix2.M46 + matrix1.M71 * matrix2.M47 + matrix1.M81 * matrix2.M48;
            double m51 = matrix1.M11 * matrix2.M51 + matrix1.M21 * matrix2.M52 + matrix1.M31 * matrix2.M53 + matrix1.M41 * matrix2.M54 + matrix1.M51 * matrix2.M55 + matrix1.M61 * matrix2.M56 + matrix1.M71 * matrix2.M57 + matrix1.M81 * matrix2.M58;
            double m61 = matrix1.M11 * matrix2.M61 + matrix1.M21 * matrix2.M62 + matrix1.M31 * matrix2.M63 + matrix1.M41 * matrix2.M64 + matrix1.M51 * matrix2.M65 + matrix1.M61 * matrix2.M66 + matrix1.M71 * matrix2.M67 + matrix1.M81 * matrix2.M68;
            double m71 = matrix1.M11 * matrix2.M71 + matrix1.M21 * matrix2.M72 + matrix1.M31 * matrix2.M73 + matrix1.M41 * matrix2.M74 + matrix1.M51 * matrix2.M75 + matrix1.M61 * matrix2.M76 + matrix1.M71 * matrix2.M77 + matrix1.M81 * matrix2.M78;

            return new Matrix7x1(m11, m21, m31, m41, m51, m61, m71);
        }
        public static Matrix8x1 operator *(Matrix8x1 matrix1, Matrix8x8 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12 + matrix1.M31 * matrix2.M13 + matrix1.M41 * matrix2.M14 + matrix1.M51 * matrix2.M15 + matrix1.M61 * matrix2.M16 + matrix1.M71 * matrix2.M17 + matrix1.M81 * matrix2.M18;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22 + matrix1.M31 * matrix2.M23 + matrix1.M41 * matrix2.M24 + matrix1.M51 * matrix2.M25 + matrix1.M61 * matrix2.M26 + matrix1.M71 * matrix2.M27 + matrix1.M81 * matrix2.M28;
            double m31 = matrix1.M11 * matrix2.M31 + matrix1.M21 * matrix2.M32 + matrix1.M31 * matrix2.M33 + matrix1.M41 * matrix2.M34 + matrix1.M51 * matrix2.M35 + matrix1.M61 * matrix2.M36 + matrix1.M71 * matrix2.M37 + matrix1.M81 * matrix2.M38;
            double m41 = matrix1.M11 * matrix2.M41 + matrix1.M21 * matrix2.M42 + matrix1.M31 * matrix2.M43 + matrix1.M41 * matrix2.M44 + matrix1.M51 * matrix2.M45 + matrix1.M61 * matrix2.M46 + matrix1.M71 * matrix2.M47 + matrix1.M81 * matrix2.M48;
            double m51 = matrix1.M11 * matrix2.M51 + matrix1.M21 * matrix2.M52 + matrix1.M31 * matrix2.M53 + matrix1.M41 * matrix2.M54 + matrix1.M51 * matrix2.M55 + matrix1.M61 * matrix2.M56 + matrix1.M71 * matrix2.M57 + matrix1.M81 * matrix2.M58;
            double m61 = matrix1.M11 * matrix2.M61 + matrix1.M21 * matrix2.M62 + matrix1.M31 * matrix2.M63 + matrix1.M41 * matrix2.M64 + matrix1.M51 * matrix2.M65 + matrix1.M61 * matrix2.M66 + matrix1.M71 * matrix2.M67 + matrix1.M81 * matrix2.M68;
            double m71 = matrix1.M11 * matrix2.M71 + matrix1.M21 * matrix2.M72 + matrix1.M31 * matrix2.M73 + matrix1.M41 * matrix2.M74 + matrix1.M51 * matrix2.M75 + matrix1.M61 * matrix2.M76 + matrix1.M71 * matrix2.M77 + matrix1.M81 * matrix2.M78;
            double m81 = matrix1.M11 * matrix2.M81 + matrix1.M21 * matrix2.M82 + matrix1.M31 * matrix2.M83 + matrix1.M41 * matrix2.M84 + matrix1.M51 * matrix2.M85 + matrix1.M61 * matrix2.M86 + matrix1.M71 * matrix2.M87 + matrix1.M81 * matrix2.M88;

            return new Matrix8x1(m11, m21, m31, m41, m51, m61, m71, m81);
        }
    }
}
