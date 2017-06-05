using System.Runtime.InteropServices;

namespace System.Numerics.Matrices
{
    /// <summary>
    /// Represents a matrix of double precision floating-point values defined by its number of columns and rows
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix6x3: IEquatable<Matrix6x3>, IMatrix
    {
        public const int ColumnCount = 6;
        public const int RowCount = 3;

        static Matrix6x3()
        {
            Zero = new Matrix6x3(0);
        }

        /// <summary>
        /// Constant Matrix6x3 with all values initialized to zero
        /// </summary>
        public static readonly Matrix6x3 Zero;

        /// <summary>
        /// Initializes a Matrix6x3 with all of it values specifically set
        /// </summary>
        /// <param name="m11">The column 1, row 1 value</param>
        /// <param name="m21">The column 2, row 1 value</param>
        /// <param name="m31">The column 3, row 1 value</param>
        /// <param name="m41">The column 4, row 1 value</param>
        /// <param name="m51">The column 5, row 1 value</param>
        /// <param name="m61">The column 6, row 1 value</param>
        /// <param name="m12">The column 1, row 2 value</param>
        /// <param name="m22">The column 2, row 2 value</param>
        /// <param name="m32">The column 3, row 2 value</param>
        /// <param name="m42">The column 4, row 2 value</param>
        /// <param name="m52">The column 5, row 2 value</param>
        /// <param name="m62">The column 6, row 2 value</param>
        /// <param name="m13">The column 1, row 3 value</param>
        /// <param name="m23">The column 2, row 3 value</param>
        /// <param name="m33">The column 3, row 3 value</param>
        /// <param name="m43">The column 4, row 3 value</param>
        /// <param name="m53">The column 5, row 3 value</param>
        /// <param name="m63">The column 6, row 3 value</param>
        public Matrix6x3(double m11, double m21, double m31, double m41, double m51, double m61, 
                         double m12, double m22, double m32, double m42, double m52, double m62, 
                         double m13, double m23, double m33, double m43, double m53, double m63)
        {
			M11 = m11; M21 = m21; M31 = m31; M41 = m41; M51 = m51; M61 = m61; 
			M12 = m12; M22 = m22; M32 = m32; M42 = m42; M52 = m52; M62 = m62; 
			M13 = m13; M23 = m23; M33 = m33; M43 = m43; M53 = m53; M63 = m63; 
        }

        /// <summary>
        /// Initialized a Matrix6x3 with all values set to the same value
        /// </summary>
        /// <param name="value">The value to set all values to</param>
        public Matrix6x3(double value)
        {
			M11 = M21 = M31 = M41 = M51 = M61 = 
			M12 = M22 = M32 = M42 = M52 = M62 = 
			M13 = M23 = M33 = M43 = M53 = M63 = value;
        }

		public double M11;
		public double M21;
		public double M31;
		public double M41;
		public double M51;
		public double M61;
		public double M12;
		public double M22;
		public double M32;
		public double M42;
		public double M52;
		public double M62;
		public double M13;
		public double M23;
		public double M33;
		public double M43;
		public double M53;
		public double M63;

        public unsafe double this[int col, int row]
        {
            get
            {
                if (col < 0 || col >= ColumnCount)
                    throw new ArgumentOutOfRangeException("col");
                if (row < 0 || row >= RowCount)
                    throw new ArgumentOutOfRangeException("col");

                fixed (Matrix6x3* p = &this)
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

                fixed (Matrix6x3* p = &this)
                {
                    double* d = (double*)p;
                    d[row * ColumnCount + col] = value;
                }
            }
        }

        /// <summary>
        /// Gets the number of columns in the matrix
        /// </summary>
        public int Columns { get { return Matrix6x3.ColumnCount; } }
        /// <summary>
        /// Get the number of rows in the matrix
        /// </summary>
        public int Rows { get { return Matrix6x3.RowCount; } }

        /// <summary>
        /// Gets a new Matrix1x3 containing the values of column 1
        /// </summary>
        public Matrix1x3 Column1 { get { return new Matrix1x3(M11, M12, M13); } }
        /// <summary>
        /// Gets a new Matrix1x3 containing the values of column 2
        /// </summary>
        public Matrix1x3 Column2 { get { return new Matrix1x3(M21, M22, M23); } }
        /// <summary>
        /// Gets a new Matrix1x3 containing the values of column 3
        /// </summary>
        public Matrix1x3 Column3 { get { return new Matrix1x3(M31, M32, M33); } }
        /// <summary>
        /// Gets a new Matrix1x3 containing the values of column 4
        /// </summary>
        public Matrix1x3 Column4 { get { return new Matrix1x3(M41, M42, M43); } }
        /// <summary>
        /// Gets a new Matrix1x3 containing the values of column 5
        /// </summary>
        public Matrix1x3 Column5 { get { return new Matrix1x3(M51, M52, M53); } }
        /// <summary>
        /// Gets a new Matrix1x3 containing the values of column 6
        /// </summary>
        public Matrix1x3 Column6 { get { return new Matrix1x3(M61, M62, M63); } }
        /// <summary>
        /// Gets a new Matrix6x1 containing the values of column 1
        /// </summary>
        public Matrix6x1 Row1 { get { return new Matrix6x1(M11, M21, M31, M41, M51, M61); } }
        /// <summary>
        /// Gets a new Matrix6x1 containing the values of column 2
        /// </summary>
        public Matrix6x1 Row2 { get { return new Matrix6x1(M12, M22, M32, M42, M52, M62); } }
        /// <summary>
        /// Gets a new Matrix6x1 containing the values of column 3
        /// </summary>
        public Matrix6x1 Row3 { get { return new Matrix6x1(M13, M23, M33, M43, M53, M63); } }

        public override bool Equals(object obj)
        {
            if (obj is Matrix6x3)
                return this == (Matrix6x3)obj;

            return false;
        }

        public bool Equals(Matrix6x3 other)
        {
            return this == other;
        }

        public unsafe override int GetHashCode()
        {
            fixed (Matrix6x3* p = &this)
            {
                int* x = (int*)p;
                unchecked
                {
                    return (x[00] ^ x[01]) + (x[02] ^ x[03]) + (x[04] ^ x[05]) + (x[06] ^ x[07]) + (x[08] ^ x[09]) + (x[10] ^ x[11])
                         + (x[06] ^ x[07]) + (x[08] ^ x[09]) + (x[10] ^ x[11]) + (x[12] ^ x[13]) + (x[14] ^ x[15]) + (x[16] ^ x[17])
                         + (x[12] ^ x[13]) + (x[14] ^ x[15]) + (x[16] ^ x[17]) + (x[18] ^ x[19]) + (x[20] ^ x[21]) + (x[22] ^ x[23]);
                }
            }
        }

        public override string ToString()
        {
            return String.Format("Matrix6x3: "
                               + "{{|{00}|{01}|{02}|{03}|{04}|{05}|}}"
                               + "{{|{06}|{07}|{08}|{09}|{10}|{11}|}}"
                               + "{{|{12}|{13}|{14}|{15}|{16}|{17}|}}"
                               , M11, M21, M31, M41, M51, M61
                               , M12, M22, M32, M42, M52, M62
                               , M13, M23, M33, M43, M53, M63); 
        }

        /// <summary>
        /// Creates and returns a transposed matrix
        /// </summary>
        /// <returns>Matrix with transposed values</returns>
        public Matrix3x6 Transpose()
        {
            return new Matrix3x6(M11, M12, M13, 
                                 M21, M22, M23, 
                                 M31, M32, M33, 
                                 M41, M42, M43, 
                                 M51, M52, M53, 
                                 M61, M62, M63);
        }

        public static bool operator ==(Matrix6x3 matrix1, Matrix6x3 matrix2)
        {
            return (matrix1 == matrix2 || Math.Abs(matrix1.M11 - matrix2.M11) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M21 - matrix2.M21) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M31 - matrix2.M31) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M41 - matrix2.M41) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M51 - matrix2.M51) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M61 - matrix2.M61) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M12 - matrix2.M12) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M22 - matrix2.M22) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M32 - matrix2.M32) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M42 - matrix2.M42) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M52 - matrix2.M52) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M62 - matrix2.M62) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M13 - matrix2.M13) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M23 - matrix2.M23) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M33 - matrix2.M33) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M43 - matrix2.M43) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M53 - matrix2.M53) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M63 - matrix2.M63) <= Double.Epsilon);
        }

        public static bool operator !=(Matrix6x3 matrix1, Matrix6x3 matrix2)
        {
            return Math.Abs(matrix1.M11 - matrix2.M11) > Double.Epsilon
                || Math.Abs(matrix1.M21 - matrix2.M21) > Double.Epsilon
                || Math.Abs(matrix1.M31 - matrix2.M31) > Double.Epsilon
                || Math.Abs(matrix1.M41 - matrix2.M41) > Double.Epsilon
                || Math.Abs(matrix1.M51 - matrix2.M51) > Double.Epsilon
                || Math.Abs(matrix1.M61 - matrix2.M61) > Double.Epsilon
                || Math.Abs(matrix1.M12 - matrix2.M12) > Double.Epsilon
                || Math.Abs(matrix1.M22 - matrix2.M22) > Double.Epsilon
                || Math.Abs(matrix1.M32 - matrix2.M32) > Double.Epsilon
                || Math.Abs(matrix1.M42 - matrix2.M42) > Double.Epsilon
                || Math.Abs(matrix1.M52 - matrix2.M52) > Double.Epsilon
                || Math.Abs(matrix1.M62 - matrix2.M62) > Double.Epsilon
                || Math.Abs(matrix1.M13 - matrix2.M13) > Double.Epsilon
                || Math.Abs(matrix1.M23 - matrix2.M23) > Double.Epsilon
                || Math.Abs(matrix1.M33 - matrix2.M33) > Double.Epsilon
                || Math.Abs(matrix1.M43 - matrix2.M43) > Double.Epsilon
                || Math.Abs(matrix1.M53 - matrix2.M53) > Double.Epsilon
                || Math.Abs(matrix1.M63 - matrix2.M63) > Double.Epsilon;
        }

        public static Matrix6x3 operator +(Matrix6x3 matrix1, Matrix6x3 matrix2)
        {
            double m11 = matrix1.M11 + matrix2.M11;
            double m21 = matrix1.M21 + matrix2.M21;
            double m31 = matrix1.M31 + matrix2.M31;
            double m41 = matrix1.M41 + matrix2.M41;
            double m51 = matrix1.M51 + matrix2.M51;
            double m61 = matrix1.M61 + matrix2.M61;
            double m12 = matrix1.M12 + matrix2.M12;
            double m22 = matrix1.M22 + matrix2.M22;
            double m32 = matrix1.M32 + matrix2.M32;
            double m42 = matrix1.M42 + matrix2.M42;
            double m52 = matrix1.M52 + matrix2.M52;
            double m62 = matrix1.M62 + matrix2.M62;
            double m13 = matrix1.M13 + matrix2.M13;
            double m23 = matrix1.M23 + matrix2.M23;
            double m33 = matrix1.M33 + matrix2.M33;
            double m43 = matrix1.M43 + matrix2.M43;
            double m53 = matrix1.M53 + matrix2.M53;
            double m63 = matrix1.M63 + matrix2.M63;

            return new Matrix6x3(m11, m21, m31, m41, m51, m61, 
                                 m12, m22, m32, m42, m52, m62, 
                                 m13, m23, m33, m43, m53, m63);
        }

        public static Matrix6x3 operator -(Matrix6x3 matrix1, Matrix6x3 matrix2)
        {
            double m11 = matrix1.M11 - matrix2.M11;
            double m21 = matrix1.M21 - matrix2.M21;
            double m31 = matrix1.M31 - matrix2.M31;
            double m41 = matrix1.M41 - matrix2.M41;
            double m51 = matrix1.M51 - matrix2.M51;
            double m61 = matrix1.M61 - matrix2.M61;
            double m12 = matrix1.M12 - matrix2.M12;
            double m22 = matrix1.M22 - matrix2.M22;
            double m32 = matrix1.M32 - matrix2.M32;
            double m42 = matrix1.M42 - matrix2.M42;
            double m52 = matrix1.M52 - matrix2.M52;
            double m62 = matrix1.M62 - matrix2.M62;
            double m13 = matrix1.M13 - matrix2.M13;
            double m23 = matrix1.M23 - matrix2.M23;
            double m33 = matrix1.M33 - matrix2.M33;
            double m43 = matrix1.M43 - matrix2.M43;
            double m53 = matrix1.M53 - matrix2.M53;
            double m63 = matrix1.M63 - matrix2.M63;

            return new Matrix6x3(m11, m21, m31, m41, m51, m61, 
                                 m12, m22, m32, m42, m52, m62, 
                                 m13, m23, m33, m43, m53, m63);
        }

        public static Matrix6x3 operator *(Matrix6x3 matrix, double scalar)
        {
            double m11 = matrix.M11 * scalar;
            double m21 = matrix.M21 * scalar;
            double m31 = matrix.M31 * scalar;
            double m41 = matrix.M41 * scalar;
            double m51 = matrix.M51 * scalar;
            double m61 = matrix.M61 * scalar;
            double m12 = matrix.M12 * scalar;
            double m22 = matrix.M22 * scalar;
            double m32 = matrix.M32 * scalar;
            double m42 = matrix.M42 * scalar;
            double m52 = matrix.M52 * scalar;
            double m62 = matrix.M62 * scalar;
            double m13 = matrix.M13 * scalar;
            double m23 = matrix.M23 * scalar;
            double m33 = matrix.M33 * scalar;
            double m43 = matrix.M43 * scalar;
            double m53 = matrix.M53 * scalar;
            double m63 = matrix.M63 * scalar;

            return new Matrix6x3(m11, m21, m31, m41, m51, m61, 
                                 m12, m22, m32, m42, m52, m62, 
                                 m13, m23, m33, m43, m53, m63);
        }

        public static Matrix6x3 operator *(double scalar, Matrix6x3 matrix)
        {
            double m11 = scalar * matrix.M11;
            double m21 = scalar * matrix.M21;
            double m31 = scalar * matrix.M31;
            double m41 = scalar * matrix.M41;
            double m51 = scalar * matrix.M51;
            double m61 = scalar * matrix.M61;
            double m12 = scalar * matrix.M12;
            double m22 = scalar * matrix.M22;
            double m32 = scalar * matrix.M32;
            double m42 = scalar * matrix.M42;
            double m52 = scalar * matrix.M52;
            double m62 = scalar * matrix.M62;
            double m13 = scalar * matrix.M13;
            double m23 = scalar * matrix.M23;
            double m33 = scalar * matrix.M33;
            double m43 = scalar * matrix.M43;
            double m53 = scalar * matrix.M53;
            double m63 = scalar * matrix.M63;

            return new Matrix6x3(m11, m21, m31, m41, m51, m61, 
                                 m12, m22, m32, m42, m52, m62, 
                                 m13, m23, m33, m43, m53, m63);
        }

        public static Matrix1x3 operator *(Matrix6x3 matrix1, Matrix1x6 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12 + matrix1.M31 * matrix2.M13 + matrix1.M41 * matrix2.M14 + matrix1.M51 * matrix2.M15 + matrix1.M61 * matrix2.M16;
            double m12 = matrix1.M12 * matrix2.M11 + matrix1.M22 * matrix2.M12 + matrix1.M32 * matrix2.M13 + matrix1.M42 * matrix2.M14 + matrix1.M52 * matrix2.M15 + matrix1.M62 * matrix2.M16;
            double m13 = matrix1.M13 * matrix2.M11 + matrix1.M23 * matrix2.M12 + matrix1.M33 * matrix2.M13 + matrix1.M43 * matrix2.M14 + matrix1.M53 * matrix2.M15 + matrix1.M63 * matrix2.M16;

            return new Matrix1x3(m11, 
                                 m12, 
                                 m13);
        }
        public static Matrix2x3 operator *(Matrix6x3 matrix1, Matrix2x6 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12 + matrix1.M31 * matrix2.M13 + matrix1.M41 * matrix2.M14 + matrix1.M51 * matrix2.M15 + matrix1.M61 * matrix2.M16;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22 + matrix1.M31 * matrix2.M23 + matrix1.M41 * matrix2.M24 + matrix1.M51 * matrix2.M25 + matrix1.M61 * matrix2.M26;
            double m12 = matrix1.M12 * matrix2.M11 + matrix1.M22 * matrix2.M12 + matrix1.M32 * matrix2.M13 + matrix1.M42 * matrix2.M14 + matrix1.M52 * matrix2.M15 + matrix1.M62 * matrix2.M16;
            double m22 = matrix1.M12 * matrix2.M21 + matrix1.M22 * matrix2.M22 + matrix1.M32 * matrix2.M23 + matrix1.M42 * matrix2.M24 + matrix1.M52 * matrix2.M25 + matrix1.M62 * matrix2.M26;
            double m13 = matrix1.M13 * matrix2.M11 + matrix1.M23 * matrix2.M12 + matrix1.M33 * matrix2.M13 + matrix1.M43 * matrix2.M14 + matrix1.M53 * matrix2.M15 + matrix1.M63 * matrix2.M16;
            double m23 = matrix1.M13 * matrix2.M21 + matrix1.M23 * matrix2.M22 + matrix1.M33 * matrix2.M23 + matrix1.M43 * matrix2.M24 + matrix1.M53 * matrix2.M25 + matrix1.M63 * matrix2.M26;

            return new Matrix2x3(m11, m21, 
                                 m12, m22, 
                                 m13, m23);
        }
        public static Matrix3x3 operator *(Matrix6x3 matrix1, Matrix3x6 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12 + matrix1.M31 * matrix2.M13 + matrix1.M41 * matrix2.M14 + matrix1.M51 * matrix2.M15 + matrix1.M61 * matrix2.M16;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22 + matrix1.M31 * matrix2.M23 + matrix1.M41 * matrix2.M24 + matrix1.M51 * matrix2.M25 + matrix1.M61 * matrix2.M26;
            double m31 = matrix1.M11 * matrix2.M31 + matrix1.M21 * matrix2.M32 + matrix1.M31 * matrix2.M33 + matrix1.M41 * matrix2.M34 + matrix1.M51 * matrix2.M35 + matrix1.M61 * matrix2.M36;
            double m12 = matrix1.M12 * matrix2.M11 + matrix1.M22 * matrix2.M12 + matrix1.M32 * matrix2.M13 + matrix1.M42 * matrix2.M14 + matrix1.M52 * matrix2.M15 + matrix1.M62 * matrix2.M16;
            double m22 = matrix1.M12 * matrix2.M21 + matrix1.M22 * matrix2.M22 + matrix1.M32 * matrix2.M23 + matrix1.M42 * matrix2.M24 + matrix1.M52 * matrix2.M25 + matrix1.M62 * matrix2.M26;
            double m32 = matrix1.M12 * matrix2.M31 + matrix1.M22 * matrix2.M32 + matrix1.M32 * matrix2.M33 + matrix1.M42 * matrix2.M34 + matrix1.M52 * matrix2.M35 + matrix1.M62 * matrix2.M36;
            double m13 = matrix1.M13 * matrix2.M11 + matrix1.M23 * matrix2.M12 + matrix1.M33 * matrix2.M13 + matrix1.M43 * matrix2.M14 + matrix1.M53 * matrix2.M15 + matrix1.M63 * matrix2.M16;
            double m23 = matrix1.M13 * matrix2.M21 + matrix1.M23 * matrix2.M22 + matrix1.M33 * matrix2.M23 + matrix1.M43 * matrix2.M24 + matrix1.M53 * matrix2.M25 + matrix1.M63 * matrix2.M26;
            double m33 = matrix1.M13 * matrix2.M31 + matrix1.M23 * matrix2.M32 + matrix1.M33 * matrix2.M33 + matrix1.M43 * matrix2.M34 + matrix1.M53 * matrix2.M35 + matrix1.M63 * matrix2.M36;

            return new Matrix3x3(m11, m21, m31, 
                                 m12, m22, m32, 
                                 m13, m23, m33);
        }
        public static Matrix4x3 operator *(Matrix6x3 matrix1, Matrix4x6 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12 + matrix1.M31 * matrix2.M13 + matrix1.M41 * matrix2.M14 + matrix1.M51 * matrix2.M15 + matrix1.M61 * matrix2.M16;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22 + matrix1.M31 * matrix2.M23 + matrix1.M41 * matrix2.M24 + matrix1.M51 * matrix2.M25 + matrix1.M61 * matrix2.M26;
            double m31 = matrix1.M11 * matrix2.M31 + matrix1.M21 * matrix2.M32 + matrix1.M31 * matrix2.M33 + matrix1.M41 * matrix2.M34 + matrix1.M51 * matrix2.M35 + matrix1.M61 * matrix2.M36;
            double m41 = matrix1.M11 * matrix2.M41 + matrix1.M21 * matrix2.M42 + matrix1.M31 * matrix2.M43 + matrix1.M41 * matrix2.M44 + matrix1.M51 * matrix2.M45 + matrix1.M61 * matrix2.M46;
            double m12 = matrix1.M12 * matrix2.M11 + matrix1.M22 * matrix2.M12 + matrix1.M32 * matrix2.M13 + matrix1.M42 * matrix2.M14 + matrix1.M52 * matrix2.M15 + matrix1.M62 * matrix2.M16;
            double m22 = matrix1.M12 * matrix2.M21 + matrix1.M22 * matrix2.M22 + matrix1.M32 * matrix2.M23 + matrix1.M42 * matrix2.M24 + matrix1.M52 * matrix2.M25 + matrix1.M62 * matrix2.M26;
            double m32 = matrix1.M12 * matrix2.M31 + matrix1.M22 * matrix2.M32 + matrix1.M32 * matrix2.M33 + matrix1.M42 * matrix2.M34 + matrix1.M52 * matrix2.M35 + matrix1.M62 * matrix2.M36;
            double m42 = matrix1.M12 * matrix2.M41 + matrix1.M22 * matrix2.M42 + matrix1.M32 * matrix2.M43 + matrix1.M42 * matrix2.M44 + matrix1.M52 * matrix2.M45 + matrix1.M62 * matrix2.M46;
            double m13 = matrix1.M13 * matrix2.M11 + matrix1.M23 * matrix2.M12 + matrix1.M33 * matrix2.M13 + matrix1.M43 * matrix2.M14 + matrix1.M53 * matrix2.M15 + matrix1.M63 * matrix2.M16;
            double m23 = matrix1.M13 * matrix2.M21 + matrix1.M23 * matrix2.M22 + matrix1.M33 * matrix2.M23 + matrix1.M43 * matrix2.M24 + matrix1.M53 * matrix2.M25 + matrix1.M63 * matrix2.M26;
            double m33 = matrix1.M13 * matrix2.M31 + matrix1.M23 * matrix2.M32 + matrix1.M33 * matrix2.M33 + matrix1.M43 * matrix2.M34 + matrix1.M53 * matrix2.M35 + matrix1.M63 * matrix2.M36;
            double m43 = matrix1.M13 * matrix2.M41 + matrix1.M23 * matrix2.M42 + matrix1.M33 * matrix2.M43 + matrix1.M43 * matrix2.M44 + matrix1.M53 * matrix2.M45 + matrix1.M63 * matrix2.M46;

            return new Matrix4x3(m11, m21, m31, m41, 
                                 m12, m22, m32, m42, 
                                 m13, m23, m33, m43);
        }
        public static Matrix5x3 operator *(Matrix6x3 matrix1, Matrix5x6 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12 + matrix1.M31 * matrix2.M13 + matrix1.M41 * matrix2.M14 + matrix1.M51 * matrix2.M15 + matrix1.M61 * matrix2.M16;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22 + matrix1.M31 * matrix2.M23 + matrix1.M41 * matrix2.M24 + matrix1.M51 * matrix2.M25 + matrix1.M61 * matrix2.M26;
            double m31 = matrix1.M11 * matrix2.M31 + matrix1.M21 * matrix2.M32 + matrix1.M31 * matrix2.M33 + matrix1.M41 * matrix2.M34 + matrix1.M51 * matrix2.M35 + matrix1.M61 * matrix2.M36;
            double m41 = matrix1.M11 * matrix2.M41 + matrix1.M21 * matrix2.M42 + matrix1.M31 * matrix2.M43 + matrix1.M41 * matrix2.M44 + matrix1.M51 * matrix2.M45 + matrix1.M61 * matrix2.M46;
            double m51 = matrix1.M11 * matrix2.M51 + matrix1.M21 * matrix2.M52 + matrix1.M31 * matrix2.M53 + matrix1.M41 * matrix2.M54 + matrix1.M51 * matrix2.M55 + matrix1.M61 * matrix2.M56;
            double m12 = matrix1.M12 * matrix2.M11 + matrix1.M22 * matrix2.M12 + matrix1.M32 * matrix2.M13 + matrix1.M42 * matrix2.M14 + matrix1.M52 * matrix2.M15 + matrix1.M62 * matrix2.M16;
            double m22 = matrix1.M12 * matrix2.M21 + matrix1.M22 * matrix2.M22 + matrix1.M32 * matrix2.M23 + matrix1.M42 * matrix2.M24 + matrix1.M52 * matrix2.M25 + matrix1.M62 * matrix2.M26;
            double m32 = matrix1.M12 * matrix2.M31 + matrix1.M22 * matrix2.M32 + matrix1.M32 * matrix2.M33 + matrix1.M42 * matrix2.M34 + matrix1.M52 * matrix2.M35 + matrix1.M62 * matrix2.M36;
            double m42 = matrix1.M12 * matrix2.M41 + matrix1.M22 * matrix2.M42 + matrix1.M32 * matrix2.M43 + matrix1.M42 * matrix2.M44 + matrix1.M52 * matrix2.M45 + matrix1.M62 * matrix2.M46;
            double m52 = matrix1.M12 * matrix2.M51 + matrix1.M22 * matrix2.M52 + matrix1.M32 * matrix2.M53 + matrix1.M42 * matrix2.M54 + matrix1.M52 * matrix2.M55 + matrix1.M62 * matrix2.M56;
            double m13 = matrix1.M13 * matrix2.M11 + matrix1.M23 * matrix2.M12 + matrix1.M33 * matrix2.M13 + matrix1.M43 * matrix2.M14 + matrix1.M53 * matrix2.M15 + matrix1.M63 * matrix2.M16;
            double m23 = matrix1.M13 * matrix2.M21 + matrix1.M23 * matrix2.M22 + matrix1.M33 * matrix2.M23 + matrix1.M43 * matrix2.M24 + matrix1.M53 * matrix2.M25 + matrix1.M63 * matrix2.M26;
            double m33 = matrix1.M13 * matrix2.M31 + matrix1.M23 * matrix2.M32 + matrix1.M33 * matrix2.M33 + matrix1.M43 * matrix2.M34 + matrix1.M53 * matrix2.M35 + matrix1.M63 * matrix2.M36;
            double m43 = matrix1.M13 * matrix2.M41 + matrix1.M23 * matrix2.M42 + matrix1.M33 * matrix2.M43 + matrix1.M43 * matrix2.M44 + matrix1.M53 * matrix2.M45 + matrix1.M63 * matrix2.M46;
            double m53 = matrix1.M13 * matrix2.M51 + matrix1.M23 * matrix2.M52 + matrix1.M33 * matrix2.M53 + matrix1.M43 * matrix2.M54 + matrix1.M53 * matrix2.M55 + matrix1.M63 * matrix2.M56;

            return new Matrix5x3(m11, m21, m31, m41, m51, 
                                 m12, m22, m32, m42, m52, 
                                 m13, m23, m33, m43, m53);
        }
        public static Matrix6x3 operator *(Matrix6x3 matrix1, Matrix6x6 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12 + matrix1.M31 * matrix2.M13 + matrix1.M41 * matrix2.M14 + matrix1.M51 * matrix2.M15 + matrix1.M61 * matrix2.M16;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22 + matrix1.M31 * matrix2.M23 + matrix1.M41 * matrix2.M24 + matrix1.M51 * matrix2.M25 + matrix1.M61 * matrix2.M26;
            double m31 = matrix1.M11 * matrix2.M31 + matrix1.M21 * matrix2.M32 + matrix1.M31 * matrix2.M33 + matrix1.M41 * matrix2.M34 + matrix1.M51 * matrix2.M35 + matrix1.M61 * matrix2.M36;
            double m41 = matrix1.M11 * matrix2.M41 + matrix1.M21 * matrix2.M42 + matrix1.M31 * matrix2.M43 + matrix1.M41 * matrix2.M44 + matrix1.M51 * matrix2.M45 + matrix1.M61 * matrix2.M46;
            double m51 = matrix1.M11 * matrix2.M51 + matrix1.M21 * matrix2.M52 + matrix1.M31 * matrix2.M53 + matrix1.M41 * matrix2.M54 + matrix1.M51 * matrix2.M55 + matrix1.M61 * matrix2.M56;
            double m61 = matrix1.M11 * matrix2.M61 + matrix1.M21 * matrix2.M62 + matrix1.M31 * matrix2.M63 + matrix1.M41 * matrix2.M64 + matrix1.M51 * matrix2.M65 + matrix1.M61 * matrix2.M66;
            double m12 = matrix1.M12 * matrix2.M11 + matrix1.M22 * matrix2.M12 + matrix1.M32 * matrix2.M13 + matrix1.M42 * matrix2.M14 + matrix1.M52 * matrix2.M15 + matrix1.M62 * matrix2.M16;
            double m22 = matrix1.M12 * matrix2.M21 + matrix1.M22 * matrix2.M22 + matrix1.M32 * matrix2.M23 + matrix1.M42 * matrix2.M24 + matrix1.M52 * matrix2.M25 + matrix1.M62 * matrix2.M26;
            double m32 = matrix1.M12 * matrix2.M31 + matrix1.M22 * matrix2.M32 + matrix1.M32 * matrix2.M33 + matrix1.M42 * matrix2.M34 + matrix1.M52 * matrix2.M35 + matrix1.M62 * matrix2.M36;
            double m42 = matrix1.M12 * matrix2.M41 + matrix1.M22 * matrix2.M42 + matrix1.M32 * matrix2.M43 + matrix1.M42 * matrix2.M44 + matrix1.M52 * matrix2.M45 + matrix1.M62 * matrix2.M46;
            double m52 = matrix1.M12 * matrix2.M51 + matrix1.M22 * matrix2.M52 + matrix1.M32 * matrix2.M53 + matrix1.M42 * matrix2.M54 + matrix1.M52 * matrix2.M55 + matrix1.M62 * matrix2.M56;
            double m62 = matrix1.M12 * matrix2.M61 + matrix1.M22 * matrix2.M62 + matrix1.M32 * matrix2.M63 + matrix1.M42 * matrix2.M64 + matrix1.M52 * matrix2.M65 + matrix1.M62 * matrix2.M66;
            double m13 = matrix1.M13 * matrix2.M11 + matrix1.M23 * matrix2.M12 + matrix1.M33 * matrix2.M13 + matrix1.M43 * matrix2.M14 + matrix1.M53 * matrix2.M15 + matrix1.M63 * matrix2.M16;
            double m23 = matrix1.M13 * matrix2.M21 + matrix1.M23 * matrix2.M22 + matrix1.M33 * matrix2.M23 + matrix1.M43 * matrix2.M24 + matrix1.M53 * matrix2.M25 + matrix1.M63 * matrix2.M26;
            double m33 = matrix1.M13 * matrix2.M31 + matrix1.M23 * matrix2.M32 + matrix1.M33 * matrix2.M33 + matrix1.M43 * matrix2.M34 + matrix1.M53 * matrix2.M35 + matrix1.M63 * matrix2.M36;
            double m43 = matrix1.M13 * matrix2.M41 + matrix1.M23 * matrix2.M42 + matrix1.M33 * matrix2.M43 + matrix1.M43 * matrix2.M44 + matrix1.M53 * matrix2.M45 + matrix1.M63 * matrix2.M46;
            double m53 = matrix1.M13 * matrix2.M51 + matrix1.M23 * matrix2.M52 + matrix1.M33 * matrix2.M53 + matrix1.M43 * matrix2.M54 + matrix1.M53 * matrix2.M55 + matrix1.M63 * matrix2.M56;
            double m63 = matrix1.M13 * matrix2.M61 + matrix1.M23 * matrix2.M62 + matrix1.M33 * matrix2.M63 + matrix1.M43 * matrix2.M64 + matrix1.M53 * matrix2.M65 + matrix1.M63 * matrix2.M66;

            return new Matrix6x3(m11, m21, m31, m41, m51, m61, 
                                 m12, m22, m32, m42, m52, m62, 
                                 m13, m23, m33, m43, m53, m63);
        }
        public static Matrix7x3 operator *(Matrix6x3 matrix1, Matrix7x6 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12 + matrix1.M31 * matrix2.M13 + matrix1.M41 * matrix2.M14 + matrix1.M51 * matrix2.M15 + matrix1.M61 * matrix2.M16;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22 + matrix1.M31 * matrix2.M23 + matrix1.M41 * matrix2.M24 + matrix1.M51 * matrix2.M25 + matrix1.M61 * matrix2.M26;
            double m31 = matrix1.M11 * matrix2.M31 + matrix1.M21 * matrix2.M32 + matrix1.M31 * matrix2.M33 + matrix1.M41 * matrix2.M34 + matrix1.M51 * matrix2.M35 + matrix1.M61 * matrix2.M36;
            double m41 = matrix1.M11 * matrix2.M41 + matrix1.M21 * matrix2.M42 + matrix1.M31 * matrix2.M43 + matrix1.M41 * matrix2.M44 + matrix1.M51 * matrix2.M45 + matrix1.M61 * matrix2.M46;
            double m51 = matrix1.M11 * matrix2.M51 + matrix1.M21 * matrix2.M52 + matrix1.M31 * matrix2.M53 + matrix1.M41 * matrix2.M54 + matrix1.M51 * matrix2.M55 + matrix1.M61 * matrix2.M56;
            double m61 = matrix1.M11 * matrix2.M61 + matrix1.M21 * matrix2.M62 + matrix1.M31 * matrix2.M63 + matrix1.M41 * matrix2.M64 + matrix1.M51 * matrix2.M65 + matrix1.M61 * matrix2.M66;
            double m71 = matrix1.M11 * matrix2.M71 + matrix1.M21 * matrix2.M72 + matrix1.M31 * matrix2.M73 + matrix1.M41 * matrix2.M74 + matrix1.M51 * matrix2.M75 + matrix1.M61 * matrix2.M76;
            double m12 = matrix1.M12 * matrix2.M11 + matrix1.M22 * matrix2.M12 + matrix1.M32 * matrix2.M13 + matrix1.M42 * matrix2.M14 + matrix1.M52 * matrix2.M15 + matrix1.M62 * matrix2.M16;
            double m22 = matrix1.M12 * matrix2.M21 + matrix1.M22 * matrix2.M22 + matrix1.M32 * matrix2.M23 + matrix1.M42 * matrix2.M24 + matrix1.M52 * matrix2.M25 + matrix1.M62 * matrix2.M26;
            double m32 = matrix1.M12 * matrix2.M31 + matrix1.M22 * matrix2.M32 + matrix1.M32 * matrix2.M33 + matrix1.M42 * matrix2.M34 + matrix1.M52 * matrix2.M35 + matrix1.M62 * matrix2.M36;
            double m42 = matrix1.M12 * matrix2.M41 + matrix1.M22 * matrix2.M42 + matrix1.M32 * matrix2.M43 + matrix1.M42 * matrix2.M44 + matrix1.M52 * matrix2.M45 + matrix1.M62 * matrix2.M46;
            double m52 = matrix1.M12 * matrix2.M51 + matrix1.M22 * matrix2.M52 + matrix1.M32 * matrix2.M53 + matrix1.M42 * matrix2.M54 + matrix1.M52 * matrix2.M55 + matrix1.M62 * matrix2.M56;
            double m62 = matrix1.M12 * matrix2.M61 + matrix1.M22 * matrix2.M62 + matrix1.M32 * matrix2.M63 + matrix1.M42 * matrix2.M64 + matrix1.M52 * matrix2.M65 + matrix1.M62 * matrix2.M66;
            double m72 = matrix1.M12 * matrix2.M71 + matrix1.M22 * matrix2.M72 + matrix1.M32 * matrix2.M73 + matrix1.M42 * matrix2.M74 + matrix1.M52 * matrix2.M75 + matrix1.M62 * matrix2.M76;
            double m13 = matrix1.M13 * matrix2.M11 + matrix1.M23 * matrix2.M12 + matrix1.M33 * matrix2.M13 + matrix1.M43 * matrix2.M14 + matrix1.M53 * matrix2.M15 + matrix1.M63 * matrix2.M16;
            double m23 = matrix1.M13 * matrix2.M21 + matrix1.M23 * matrix2.M22 + matrix1.M33 * matrix2.M23 + matrix1.M43 * matrix2.M24 + matrix1.M53 * matrix2.M25 + matrix1.M63 * matrix2.M26;
            double m33 = matrix1.M13 * matrix2.M31 + matrix1.M23 * matrix2.M32 + matrix1.M33 * matrix2.M33 + matrix1.M43 * matrix2.M34 + matrix1.M53 * matrix2.M35 + matrix1.M63 * matrix2.M36;
            double m43 = matrix1.M13 * matrix2.M41 + matrix1.M23 * matrix2.M42 + matrix1.M33 * matrix2.M43 + matrix1.M43 * matrix2.M44 + matrix1.M53 * matrix2.M45 + matrix1.M63 * matrix2.M46;
            double m53 = matrix1.M13 * matrix2.M51 + matrix1.M23 * matrix2.M52 + matrix1.M33 * matrix2.M53 + matrix1.M43 * matrix2.M54 + matrix1.M53 * matrix2.M55 + matrix1.M63 * matrix2.M56;
            double m63 = matrix1.M13 * matrix2.M61 + matrix1.M23 * matrix2.M62 + matrix1.M33 * matrix2.M63 + matrix1.M43 * matrix2.M64 + matrix1.M53 * matrix2.M65 + matrix1.M63 * matrix2.M66;
            double m73 = matrix1.M13 * matrix2.M71 + matrix1.M23 * matrix2.M72 + matrix1.M33 * matrix2.M73 + matrix1.M43 * matrix2.M74 + matrix1.M53 * matrix2.M75 + matrix1.M63 * matrix2.M76;

            return new Matrix7x3(m11, m21, m31, m41, m51, m61, m71, 
                                 m12, m22, m32, m42, m52, m62, m72, 
                                 m13, m23, m33, m43, m53, m63, m73);
        }
        public static Matrix8x3 operator *(Matrix6x3 matrix1, Matrix8x6 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12 + matrix1.M31 * matrix2.M13 + matrix1.M41 * matrix2.M14 + matrix1.M51 * matrix2.M15 + matrix1.M61 * matrix2.M16;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22 + matrix1.M31 * matrix2.M23 + matrix1.M41 * matrix2.M24 + matrix1.M51 * matrix2.M25 + matrix1.M61 * matrix2.M26;
            double m31 = matrix1.M11 * matrix2.M31 + matrix1.M21 * matrix2.M32 + matrix1.M31 * matrix2.M33 + matrix1.M41 * matrix2.M34 + matrix1.M51 * matrix2.M35 + matrix1.M61 * matrix2.M36;
            double m41 = matrix1.M11 * matrix2.M41 + matrix1.M21 * matrix2.M42 + matrix1.M31 * matrix2.M43 + matrix1.M41 * matrix2.M44 + matrix1.M51 * matrix2.M45 + matrix1.M61 * matrix2.M46;
            double m51 = matrix1.M11 * matrix2.M51 + matrix1.M21 * matrix2.M52 + matrix1.M31 * matrix2.M53 + matrix1.M41 * matrix2.M54 + matrix1.M51 * matrix2.M55 + matrix1.M61 * matrix2.M56;
            double m61 = matrix1.M11 * matrix2.M61 + matrix1.M21 * matrix2.M62 + matrix1.M31 * matrix2.M63 + matrix1.M41 * matrix2.M64 + matrix1.M51 * matrix2.M65 + matrix1.M61 * matrix2.M66;
            double m71 = matrix1.M11 * matrix2.M71 + matrix1.M21 * matrix2.M72 + matrix1.M31 * matrix2.M73 + matrix1.M41 * matrix2.M74 + matrix1.M51 * matrix2.M75 + matrix1.M61 * matrix2.M76;
            double m81 = matrix1.M11 * matrix2.M81 + matrix1.M21 * matrix2.M82 + matrix1.M31 * matrix2.M83 + matrix1.M41 * matrix2.M84 + matrix1.M51 * matrix2.M85 + matrix1.M61 * matrix2.M86;
            double m12 = matrix1.M12 * matrix2.M11 + matrix1.M22 * matrix2.M12 + matrix1.M32 * matrix2.M13 + matrix1.M42 * matrix2.M14 + matrix1.M52 * matrix2.M15 + matrix1.M62 * matrix2.M16;
            double m22 = matrix1.M12 * matrix2.M21 + matrix1.M22 * matrix2.M22 + matrix1.M32 * matrix2.M23 + matrix1.M42 * matrix2.M24 + matrix1.M52 * matrix2.M25 + matrix1.M62 * matrix2.M26;
            double m32 = matrix1.M12 * matrix2.M31 + matrix1.M22 * matrix2.M32 + matrix1.M32 * matrix2.M33 + matrix1.M42 * matrix2.M34 + matrix1.M52 * matrix2.M35 + matrix1.M62 * matrix2.M36;
            double m42 = matrix1.M12 * matrix2.M41 + matrix1.M22 * matrix2.M42 + matrix1.M32 * matrix2.M43 + matrix1.M42 * matrix2.M44 + matrix1.M52 * matrix2.M45 + matrix1.M62 * matrix2.M46;
            double m52 = matrix1.M12 * matrix2.M51 + matrix1.M22 * matrix2.M52 + matrix1.M32 * matrix2.M53 + matrix1.M42 * matrix2.M54 + matrix1.M52 * matrix2.M55 + matrix1.M62 * matrix2.M56;
            double m62 = matrix1.M12 * matrix2.M61 + matrix1.M22 * matrix2.M62 + matrix1.M32 * matrix2.M63 + matrix1.M42 * matrix2.M64 + matrix1.M52 * matrix2.M65 + matrix1.M62 * matrix2.M66;
            double m72 = matrix1.M12 * matrix2.M71 + matrix1.M22 * matrix2.M72 + matrix1.M32 * matrix2.M73 + matrix1.M42 * matrix2.M74 + matrix1.M52 * matrix2.M75 + matrix1.M62 * matrix2.M76;
            double m82 = matrix1.M12 * matrix2.M81 + matrix1.M22 * matrix2.M82 + matrix1.M32 * matrix2.M83 + matrix1.M42 * matrix2.M84 + matrix1.M52 * matrix2.M85 + matrix1.M62 * matrix2.M86;
            double m13 = matrix1.M13 * matrix2.M11 + matrix1.M23 * matrix2.M12 + matrix1.M33 * matrix2.M13 + matrix1.M43 * matrix2.M14 + matrix1.M53 * matrix2.M15 + matrix1.M63 * matrix2.M16;
            double m23 = matrix1.M13 * matrix2.M21 + matrix1.M23 * matrix2.M22 + matrix1.M33 * matrix2.M23 + matrix1.M43 * matrix2.M24 + matrix1.M53 * matrix2.M25 + matrix1.M63 * matrix2.M26;
            double m33 = matrix1.M13 * matrix2.M31 + matrix1.M23 * matrix2.M32 + matrix1.M33 * matrix2.M33 + matrix1.M43 * matrix2.M34 + matrix1.M53 * matrix2.M35 + matrix1.M63 * matrix2.M36;
            double m43 = matrix1.M13 * matrix2.M41 + matrix1.M23 * matrix2.M42 + matrix1.M33 * matrix2.M43 + matrix1.M43 * matrix2.M44 + matrix1.M53 * matrix2.M45 + matrix1.M63 * matrix2.M46;
            double m53 = matrix1.M13 * matrix2.M51 + matrix1.M23 * matrix2.M52 + matrix1.M33 * matrix2.M53 + matrix1.M43 * matrix2.M54 + matrix1.M53 * matrix2.M55 + matrix1.M63 * matrix2.M56;
            double m63 = matrix1.M13 * matrix2.M61 + matrix1.M23 * matrix2.M62 + matrix1.M33 * matrix2.M63 + matrix1.M43 * matrix2.M64 + matrix1.M53 * matrix2.M65 + matrix1.M63 * matrix2.M66;
            double m73 = matrix1.M13 * matrix2.M71 + matrix1.M23 * matrix2.M72 + matrix1.M33 * matrix2.M73 + matrix1.M43 * matrix2.M74 + matrix1.M53 * matrix2.M75 + matrix1.M63 * matrix2.M76;
            double m83 = matrix1.M13 * matrix2.M81 + matrix1.M23 * matrix2.M82 + matrix1.M33 * matrix2.M83 + matrix1.M43 * matrix2.M84 + matrix1.M53 * matrix2.M85 + matrix1.M63 * matrix2.M86;

            return new Matrix8x3(m11, m21, m31, m41, m51, m61, m71, m81, 
                                 m12, m22, m32, m42, m52, m62, m72, m82, 
                                 m13, m23, m33, m43, m53, m63, m73, m83);
        }
    }
}
