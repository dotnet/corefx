using System.Runtime.InteropServices;

namespace System.Numerics.Matrices
{
    /// <summary>
    /// Represents a matrix of double precision floating-point values defined by its number of columns and rows
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix1x8: IEquatable<Matrix1x8>, IMatrix
    {
        public const int ColumnCount = 1;
        public const int RowCount = 8;

        static Matrix1x8()
        {
            Zero = new Matrix1x8(0);
        }

        /// <summary>
        /// Constant Matrix1x8 with all values initialized to zero
        /// </summary>
        public static readonly Matrix1x8 Zero;

        /// <summary>
        /// Initializes a Matrix1x8 with all of it values specifically set
        /// </summary>
        /// <param name="m11">The column 1, row 1 value</param>
        /// <param name="m12">The column 1, row 2 value</param>
        /// <param name="m13">The column 1, row 3 value</param>
        /// <param name="m14">The column 1, row 4 value</param>
        /// <param name="m15">The column 1, row 5 value</param>
        /// <param name="m16">The column 1, row 6 value</param>
        /// <param name="m17">The column 1, row 7 value</param>
        /// <param name="m18">The column 1, row 8 value</param>
        public Matrix1x8(double m11, 
                         double m12, 
                         double m13, 
                         double m14, 
                         double m15, 
                         double m16, 
                         double m17, 
                         double m18)
        {
			M11 = m11; 
			M12 = m12; 
			M13 = m13; 
			M14 = m14; 
			M15 = m15; 
			M16 = m16; 
			M17 = m17; 
			M18 = m18; 
        }

        /// <summary>
        /// Initialized a Matrix1x8 with all values set to the same value
        /// </summary>
        /// <param name="value">The value to set all values to</param>
        public Matrix1x8(double value)
        {
			M11 = 
			M12 = 
			M13 = 
			M14 = 
			M15 = 
			M16 = 
			M17 = 
			M18 = value;
        }

		public double M11;
		public double M12;
		public double M13;
		public double M14;
		public double M15;
		public double M16;
		public double M17;
		public double M18;

        public unsafe double this[int col, int row]
        {
            get
            {
                if (col < 0 || col >= ColumnCount)
                    throw new ArgumentOutOfRangeException("col");
                if (row < 0 || row >= RowCount)
                    throw new ArgumentOutOfRangeException("col");

                fixed (Matrix1x8* p = &this)
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

                fixed (Matrix1x8* p = &this)
                {
                    double* d = (double*)p;
                    d[row * ColumnCount + col] = value;
                }
            }
        }

        /// <summary>
        /// Gets the number of columns in the matrix
        /// </summary>
        public int Columns { get { return Matrix1x8.ColumnCount; } }
        /// <summary>
        /// Get the number of rows in the matrix
        /// </summary>
        public int Rows { get { return Matrix1x8.RowCount; } }


        public override bool Equals(object obj)
        {
            if (obj is Matrix1x8)
                return this == (Matrix1x8)obj;

            return false;
        }

        public bool Equals(Matrix1x8 other)
        {
            return this == other;
        }

        public unsafe override int GetHashCode()
        {
            fixed (Matrix1x8* p = &this)
            {
                int* x = (int*)p;
                unchecked
                {
                    return (x[00] ^ x[01])
                         + (x[01] ^ x[02])
                         + (x[02] ^ x[03])
                         + (x[03] ^ x[04])
                         + (x[04] ^ x[05])
                         + (x[05] ^ x[06])
                         + (x[06] ^ x[07])
                         + (x[07] ^ x[08]);
                }
            }
        }

        public override string ToString()
        {
            return String.Format("Matrix1x8: "
                               + "{{|{00}|}}"
                               + "{{|{01}|}}"
                               + "{{|{02}|}}"
                               + "{{|{03}|}}"
                               + "{{|{04}|}}"
                               + "{{|{05}|}}"
                               + "{{|{06}|}}"
                               + "{{|{07}|}}"
                               , M11
                               , M12
                               , M13
                               , M14
                               , M15
                               , M16
                               , M17
                               , M18); 
        }

        /// <summary>
        /// Creates and returns a transposed matrix
        /// </summary>
        /// <returns>Matrix with transposed values</returns>
        public Matrix8x1 Transpose()
        {
            return new Matrix8x1(M11, M12, M13, M14, M15, M16, M17, M18);
        }

        public static bool operator ==(Matrix1x8 matrix1, Matrix1x8 matrix2)
        {
            return (matrix1 == matrix2 || Math.Abs(matrix1.M11 - matrix2.M11) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M12 - matrix2.M12) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M13 - matrix2.M13) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M14 - matrix2.M14) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M15 - matrix2.M15) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M16 - matrix2.M16) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M17 - matrix2.M17) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M18 - matrix2.M18) <= Double.Epsilon);
        }

        public static bool operator !=(Matrix1x8 matrix1, Matrix1x8 matrix2)
        {
            return Math.Abs(matrix1.M11 - matrix2.M11) > Double.Epsilon
                || Math.Abs(matrix1.M12 - matrix2.M12) > Double.Epsilon
                || Math.Abs(matrix1.M13 - matrix2.M13) > Double.Epsilon
                || Math.Abs(matrix1.M14 - matrix2.M14) > Double.Epsilon
                || Math.Abs(matrix1.M15 - matrix2.M15) > Double.Epsilon
                || Math.Abs(matrix1.M16 - matrix2.M16) > Double.Epsilon
                || Math.Abs(matrix1.M17 - matrix2.M17) > Double.Epsilon
                || Math.Abs(matrix1.M18 - matrix2.M18) > Double.Epsilon;
        }

        public static Matrix1x8 operator +(Matrix1x8 matrix1, Matrix1x8 matrix2)
        {
            double m11 = matrix1.M11 + matrix2.M11;
            double m12 = matrix1.M12 + matrix2.M12;
            double m13 = matrix1.M13 + matrix2.M13;
            double m14 = matrix1.M14 + matrix2.M14;
            double m15 = matrix1.M15 + matrix2.M15;
            double m16 = matrix1.M16 + matrix2.M16;
            double m17 = matrix1.M17 + matrix2.M17;
            double m18 = matrix1.M18 + matrix2.M18;

            return new Matrix1x8(m11, 
                                 m12, 
                                 m13, 
                                 m14, 
                                 m15, 
                                 m16, 
                                 m17, 
                                 m18);
        }

        public static Matrix1x8 operator -(Matrix1x8 matrix1, Matrix1x8 matrix2)
        {
            double m11 = matrix1.M11 - matrix2.M11;
            double m12 = matrix1.M12 - matrix2.M12;
            double m13 = matrix1.M13 - matrix2.M13;
            double m14 = matrix1.M14 - matrix2.M14;
            double m15 = matrix1.M15 - matrix2.M15;
            double m16 = matrix1.M16 - matrix2.M16;
            double m17 = matrix1.M17 - matrix2.M17;
            double m18 = matrix1.M18 - matrix2.M18;

            return new Matrix1x8(m11, 
                                 m12, 
                                 m13, 
                                 m14, 
                                 m15, 
                                 m16, 
                                 m17, 
                                 m18);
        }

        public static Matrix1x8 operator *(Matrix1x8 matrix, double scalar)
        {
            double m11 = matrix.M11 * scalar;
            double m12 = matrix.M12 * scalar;
            double m13 = matrix.M13 * scalar;
            double m14 = matrix.M14 * scalar;
            double m15 = matrix.M15 * scalar;
            double m16 = matrix.M16 * scalar;
            double m17 = matrix.M17 * scalar;
            double m18 = matrix.M18 * scalar;

            return new Matrix1x8(m11, 
                                 m12, 
                                 m13, 
                                 m14, 
                                 m15, 
                                 m16, 
                                 m17, 
                                 m18);
        }

        public static Matrix1x8 operator *(double scalar, Matrix1x8 matrix)
        {
            double m11 = scalar * matrix.M11;
            double m12 = scalar * matrix.M12;
            double m13 = scalar * matrix.M13;
            double m14 = scalar * matrix.M14;
            double m15 = scalar * matrix.M15;
            double m16 = scalar * matrix.M16;
            double m17 = scalar * matrix.M17;
            double m18 = scalar * matrix.M18;

            return new Matrix1x8(m11, 
                                 m12, 
                                 m13, 
                                 m14, 
                                 m15, 
                                 m16, 
                                 m17, 
                                 m18);
        }

        public static Matrix2x8 operator *(Matrix1x8 matrix1, Matrix2x1 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11;
            double m21 = matrix1.M11 * matrix2.M21;
            double m12 = matrix1.M12 * matrix2.M11;
            double m22 = matrix1.M12 * matrix2.M21;
            double m13 = matrix1.M13 * matrix2.M11;
            double m23 = matrix1.M13 * matrix2.M21;
            double m14 = matrix1.M14 * matrix2.M11;
            double m24 = matrix1.M14 * matrix2.M21;
            double m15 = matrix1.M15 * matrix2.M11;
            double m25 = matrix1.M15 * matrix2.M21;
            double m16 = matrix1.M16 * matrix2.M11;
            double m26 = matrix1.M16 * matrix2.M21;
            double m17 = matrix1.M17 * matrix2.M11;
            double m27 = matrix1.M17 * matrix2.M21;
            double m18 = matrix1.M18 * matrix2.M11;
            double m28 = matrix1.M18 * matrix2.M21;

            return new Matrix2x8(m11, m21, 
                                 m12, m22, 
                                 m13, m23, 
                                 m14, m24, 
                                 m15, m25, 
                                 m16, m26, 
                                 m17, m27, 
                                 m18, m28);
        }
        public static Matrix3x8 operator *(Matrix1x8 matrix1, Matrix3x1 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11;
            double m21 = matrix1.M11 * matrix2.M21;
            double m31 = matrix1.M11 * matrix2.M31;
            double m12 = matrix1.M12 * matrix2.M11;
            double m22 = matrix1.M12 * matrix2.M21;
            double m32 = matrix1.M12 * matrix2.M31;
            double m13 = matrix1.M13 * matrix2.M11;
            double m23 = matrix1.M13 * matrix2.M21;
            double m33 = matrix1.M13 * matrix2.M31;
            double m14 = matrix1.M14 * matrix2.M11;
            double m24 = matrix1.M14 * matrix2.M21;
            double m34 = matrix1.M14 * matrix2.M31;
            double m15 = matrix1.M15 * matrix2.M11;
            double m25 = matrix1.M15 * matrix2.M21;
            double m35 = matrix1.M15 * matrix2.M31;
            double m16 = matrix1.M16 * matrix2.M11;
            double m26 = matrix1.M16 * matrix2.M21;
            double m36 = matrix1.M16 * matrix2.M31;
            double m17 = matrix1.M17 * matrix2.M11;
            double m27 = matrix1.M17 * matrix2.M21;
            double m37 = matrix1.M17 * matrix2.M31;
            double m18 = matrix1.M18 * matrix2.M11;
            double m28 = matrix1.M18 * matrix2.M21;
            double m38 = matrix1.M18 * matrix2.M31;

            return new Matrix3x8(m11, m21, m31, 
                                 m12, m22, m32, 
                                 m13, m23, m33, 
                                 m14, m24, m34, 
                                 m15, m25, m35, 
                                 m16, m26, m36, 
                                 m17, m27, m37, 
                                 m18, m28, m38);
        }
        public static Matrix4x8 operator *(Matrix1x8 matrix1, Matrix4x1 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11;
            double m21 = matrix1.M11 * matrix2.M21;
            double m31 = matrix1.M11 * matrix2.M31;
            double m41 = matrix1.M11 * matrix2.M41;
            double m12 = matrix1.M12 * matrix2.M11;
            double m22 = matrix1.M12 * matrix2.M21;
            double m32 = matrix1.M12 * matrix2.M31;
            double m42 = matrix1.M12 * matrix2.M41;
            double m13 = matrix1.M13 * matrix2.M11;
            double m23 = matrix1.M13 * matrix2.M21;
            double m33 = matrix1.M13 * matrix2.M31;
            double m43 = matrix1.M13 * matrix2.M41;
            double m14 = matrix1.M14 * matrix2.M11;
            double m24 = matrix1.M14 * matrix2.M21;
            double m34 = matrix1.M14 * matrix2.M31;
            double m44 = matrix1.M14 * matrix2.M41;
            double m15 = matrix1.M15 * matrix2.M11;
            double m25 = matrix1.M15 * matrix2.M21;
            double m35 = matrix1.M15 * matrix2.M31;
            double m45 = matrix1.M15 * matrix2.M41;
            double m16 = matrix1.M16 * matrix2.M11;
            double m26 = matrix1.M16 * matrix2.M21;
            double m36 = matrix1.M16 * matrix2.M31;
            double m46 = matrix1.M16 * matrix2.M41;
            double m17 = matrix1.M17 * matrix2.M11;
            double m27 = matrix1.M17 * matrix2.M21;
            double m37 = matrix1.M17 * matrix2.M31;
            double m47 = matrix1.M17 * matrix2.M41;
            double m18 = matrix1.M18 * matrix2.M11;
            double m28 = matrix1.M18 * matrix2.M21;
            double m38 = matrix1.M18 * matrix2.M31;
            double m48 = matrix1.M18 * matrix2.M41;

            return new Matrix4x8(m11, m21, m31, m41, 
                                 m12, m22, m32, m42, 
                                 m13, m23, m33, m43, 
                                 m14, m24, m34, m44, 
                                 m15, m25, m35, m45, 
                                 m16, m26, m36, m46, 
                                 m17, m27, m37, m47, 
                                 m18, m28, m38, m48);
        }
        public static Matrix5x8 operator *(Matrix1x8 matrix1, Matrix5x1 matrix2)
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
            double m13 = matrix1.M13 * matrix2.M11;
            double m23 = matrix1.M13 * matrix2.M21;
            double m33 = matrix1.M13 * matrix2.M31;
            double m43 = matrix1.M13 * matrix2.M41;
            double m53 = matrix1.M13 * matrix2.M51;
            double m14 = matrix1.M14 * matrix2.M11;
            double m24 = matrix1.M14 * matrix2.M21;
            double m34 = matrix1.M14 * matrix2.M31;
            double m44 = matrix1.M14 * matrix2.M41;
            double m54 = matrix1.M14 * matrix2.M51;
            double m15 = matrix1.M15 * matrix2.M11;
            double m25 = matrix1.M15 * matrix2.M21;
            double m35 = matrix1.M15 * matrix2.M31;
            double m45 = matrix1.M15 * matrix2.M41;
            double m55 = matrix1.M15 * matrix2.M51;
            double m16 = matrix1.M16 * matrix2.M11;
            double m26 = matrix1.M16 * matrix2.M21;
            double m36 = matrix1.M16 * matrix2.M31;
            double m46 = matrix1.M16 * matrix2.M41;
            double m56 = matrix1.M16 * matrix2.M51;
            double m17 = matrix1.M17 * matrix2.M11;
            double m27 = matrix1.M17 * matrix2.M21;
            double m37 = matrix1.M17 * matrix2.M31;
            double m47 = matrix1.M17 * matrix2.M41;
            double m57 = matrix1.M17 * matrix2.M51;
            double m18 = matrix1.M18 * matrix2.M11;
            double m28 = matrix1.M18 * matrix2.M21;
            double m38 = matrix1.M18 * matrix2.M31;
            double m48 = matrix1.M18 * matrix2.M41;
            double m58 = matrix1.M18 * matrix2.M51;

            return new Matrix5x8(m11, m21, m31, m41, m51, 
                                 m12, m22, m32, m42, m52, 
                                 m13, m23, m33, m43, m53, 
                                 m14, m24, m34, m44, m54, 
                                 m15, m25, m35, m45, m55, 
                                 m16, m26, m36, m46, m56, 
                                 m17, m27, m37, m47, m57, 
                                 m18, m28, m38, m48, m58);
        }
        public static Matrix6x8 operator *(Matrix1x8 matrix1, Matrix6x1 matrix2)
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
            double m13 = matrix1.M13 * matrix2.M11;
            double m23 = matrix1.M13 * matrix2.M21;
            double m33 = matrix1.M13 * matrix2.M31;
            double m43 = matrix1.M13 * matrix2.M41;
            double m53 = matrix1.M13 * matrix2.M51;
            double m63 = matrix1.M13 * matrix2.M61;
            double m14 = matrix1.M14 * matrix2.M11;
            double m24 = matrix1.M14 * matrix2.M21;
            double m34 = matrix1.M14 * matrix2.M31;
            double m44 = matrix1.M14 * matrix2.M41;
            double m54 = matrix1.M14 * matrix2.M51;
            double m64 = matrix1.M14 * matrix2.M61;
            double m15 = matrix1.M15 * matrix2.M11;
            double m25 = matrix1.M15 * matrix2.M21;
            double m35 = matrix1.M15 * matrix2.M31;
            double m45 = matrix1.M15 * matrix2.M41;
            double m55 = matrix1.M15 * matrix2.M51;
            double m65 = matrix1.M15 * matrix2.M61;
            double m16 = matrix1.M16 * matrix2.M11;
            double m26 = matrix1.M16 * matrix2.M21;
            double m36 = matrix1.M16 * matrix2.M31;
            double m46 = matrix1.M16 * matrix2.M41;
            double m56 = matrix1.M16 * matrix2.M51;
            double m66 = matrix1.M16 * matrix2.M61;
            double m17 = matrix1.M17 * matrix2.M11;
            double m27 = matrix1.M17 * matrix2.M21;
            double m37 = matrix1.M17 * matrix2.M31;
            double m47 = matrix1.M17 * matrix2.M41;
            double m57 = matrix1.M17 * matrix2.M51;
            double m67 = matrix1.M17 * matrix2.M61;
            double m18 = matrix1.M18 * matrix2.M11;
            double m28 = matrix1.M18 * matrix2.M21;
            double m38 = matrix1.M18 * matrix2.M31;
            double m48 = matrix1.M18 * matrix2.M41;
            double m58 = matrix1.M18 * matrix2.M51;
            double m68 = matrix1.M18 * matrix2.M61;

            return new Matrix6x8(m11, m21, m31, m41, m51, m61, 
                                 m12, m22, m32, m42, m52, m62, 
                                 m13, m23, m33, m43, m53, m63, 
                                 m14, m24, m34, m44, m54, m64, 
                                 m15, m25, m35, m45, m55, m65, 
                                 m16, m26, m36, m46, m56, m66, 
                                 m17, m27, m37, m47, m57, m67, 
                                 m18, m28, m38, m48, m58, m68);
        }
        public static Matrix7x8 operator *(Matrix1x8 matrix1, Matrix7x1 matrix2)
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
            double m13 = matrix1.M13 * matrix2.M11;
            double m23 = matrix1.M13 * matrix2.M21;
            double m33 = matrix1.M13 * matrix2.M31;
            double m43 = matrix1.M13 * matrix2.M41;
            double m53 = matrix1.M13 * matrix2.M51;
            double m63 = matrix1.M13 * matrix2.M61;
            double m73 = matrix1.M13 * matrix2.M71;
            double m14 = matrix1.M14 * matrix2.M11;
            double m24 = matrix1.M14 * matrix2.M21;
            double m34 = matrix1.M14 * matrix2.M31;
            double m44 = matrix1.M14 * matrix2.M41;
            double m54 = matrix1.M14 * matrix2.M51;
            double m64 = matrix1.M14 * matrix2.M61;
            double m74 = matrix1.M14 * matrix2.M71;
            double m15 = matrix1.M15 * matrix2.M11;
            double m25 = matrix1.M15 * matrix2.M21;
            double m35 = matrix1.M15 * matrix2.M31;
            double m45 = matrix1.M15 * matrix2.M41;
            double m55 = matrix1.M15 * matrix2.M51;
            double m65 = matrix1.M15 * matrix2.M61;
            double m75 = matrix1.M15 * matrix2.M71;
            double m16 = matrix1.M16 * matrix2.M11;
            double m26 = matrix1.M16 * matrix2.M21;
            double m36 = matrix1.M16 * matrix2.M31;
            double m46 = matrix1.M16 * matrix2.M41;
            double m56 = matrix1.M16 * matrix2.M51;
            double m66 = matrix1.M16 * matrix2.M61;
            double m76 = matrix1.M16 * matrix2.M71;
            double m17 = matrix1.M17 * matrix2.M11;
            double m27 = matrix1.M17 * matrix2.M21;
            double m37 = matrix1.M17 * matrix2.M31;
            double m47 = matrix1.M17 * matrix2.M41;
            double m57 = matrix1.M17 * matrix2.M51;
            double m67 = matrix1.M17 * matrix2.M61;
            double m77 = matrix1.M17 * matrix2.M71;
            double m18 = matrix1.M18 * matrix2.M11;
            double m28 = matrix1.M18 * matrix2.M21;
            double m38 = matrix1.M18 * matrix2.M31;
            double m48 = matrix1.M18 * matrix2.M41;
            double m58 = matrix1.M18 * matrix2.M51;
            double m68 = matrix1.M18 * matrix2.M61;
            double m78 = matrix1.M18 * matrix2.M71;

            return new Matrix7x8(m11, m21, m31, m41, m51, m61, m71, 
                                 m12, m22, m32, m42, m52, m62, m72, 
                                 m13, m23, m33, m43, m53, m63, m73, 
                                 m14, m24, m34, m44, m54, m64, m74, 
                                 m15, m25, m35, m45, m55, m65, m75, 
                                 m16, m26, m36, m46, m56, m66, m76, 
                                 m17, m27, m37, m47, m57, m67, m77, 
                                 m18, m28, m38, m48, m58, m68, m78);
        }
        public static Matrix8x8 operator *(Matrix1x8 matrix1, Matrix8x1 matrix2)
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
            double m13 = matrix1.M13 * matrix2.M11;
            double m23 = matrix1.M13 * matrix2.M21;
            double m33 = matrix1.M13 * matrix2.M31;
            double m43 = matrix1.M13 * matrix2.M41;
            double m53 = matrix1.M13 * matrix2.M51;
            double m63 = matrix1.M13 * matrix2.M61;
            double m73 = matrix1.M13 * matrix2.M71;
            double m83 = matrix1.M13 * matrix2.M81;
            double m14 = matrix1.M14 * matrix2.M11;
            double m24 = matrix1.M14 * matrix2.M21;
            double m34 = matrix1.M14 * matrix2.M31;
            double m44 = matrix1.M14 * matrix2.M41;
            double m54 = matrix1.M14 * matrix2.M51;
            double m64 = matrix1.M14 * matrix2.M61;
            double m74 = matrix1.M14 * matrix2.M71;
            double m84 = matrix1.M14 * matrix2.M81;
            double m15 = matrix1.M15 * matrix2.M11;
            double m25 = matrix1.M15 * matrix2.M21;
            double m35 = matrix1.M15 * matrix2.M31;
            double m45 = matrix1.M15 * matrix2.M41;
            double m55 = matrix1.M15 * matrix2.M51;
            double m65 = matrix1.M15 * matrix2.M61;
            double m75 = matrix1.M15 * matrix2.M71;
            double m85 = matrix1.M15 * matrix2.M81;
            double m16 = matrix1.M16 * matrix2.M11;
            double m26 = matrix1.M16 * matrix2.M21;
            double m36 = matrix1.M16 * matrix2.M31;
            double m46 = matrix1.M16 * matrix2.M41;
            double m56 = matrix1.M16 * matrix2.M51;
            double m66 = matrix1.M16 * matrix2.M61;
            double m76 = matrix1.M16 * matrix2.M71;
            double m86 = matrix1.M16 * matrix2.M81;
            double m17 = matrix1.M17 * matrix2.M11;
            double m27 = matrix1.M17 * matrix2.M21;
            double m37 = matrix1.M17 * matrix2.M31;
            double m47 = matrix1.M17 * matrix2.M41;
            double m57 = matrix1.M17 * matrix2.M51;
            double m67 = matrix1.M17 * matrix2.M61;
            double m77 = matrix1.M17 * matrix2.M71;
            double m87 = matrix1.M17 * matrix2.M81;
            double m18 = matrix1.M18 * matrix2.M11;
            double m28 = matrix1.M18 * matrix2.M21;
            double m38 = matrix1.M18 * matrix2.M31;
            double m48 = matrix1.M18 * matrix2.M41;
            double m58 = matrix1.M18 * matrix2.M51;
            double m68 = matrix1.M18 * matrix2.M61;
            double m78 = matrix1.M18 * matrix2.M71;
            double m88 = matrix1.M18 * matrix2.M81;

            return new Matrix8x8(m11, m21, m31, m41, m51, m61, m71, m81, 
                                 m12, m22, m32, m42, m52, m62, m72, m82, 
                                 m13, m23, m33, m43, m53, m63, m73, m83, 
                                 m14, m24, m34, m44, m54, m64, m74, m84, 
                                 m15, m25, m35, m45, m55, m65, m75, m85, 
                                 m16, m26, m36, m46, m56, m66, m76, m86, 
                                 m17, m27, m37, m47, m57, m67, m77, m87, 
                                 m18, m28, m38, m48, m58, m68, m78, m88);
        }
    }
}
