using System.Runtime.InteropServices;

namespace System.Numerics.Matrices
{
    /// <summary>
    /// Represents a matrix of double precision floating-point values defined by its number of columns and rows
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix3x8: IEquatable<Matrix3x8>, IMatrix
    {
        public const int ColumnCount = 3;
        public const int RowCount = 8;

        static Matrix3x8()
        {
            Zero = new Matrix3x8(0);
        }

        /// <summary>
        /// Constant Matrix3x8 with all values initialized to zero
        /// </summary>
        public static readonly Matrix3x8 Zero;

        /// <summary>
        /// Initializes a Matrix3x8 with all of it values specifically set
        /// </summary>
        /// <param name="m11">The column 1, row 1 value</param>
        /// <param name="m21">The column 2, row 1 value</param>
        /// <param name="m31">The column 3, row 1 value</param>
        /// <param name="m12">The column 1, row 2 value</param>
        /// <param name="m22">The column 2, row 2 value</param>
        /// <param name="m32">The column 3, row 2 value</param>
        /// <param name="m13">The column 1, row 3 value</param>
        /// <param name="m23">The column 2, row 3 value</param>
        /// <param name="m33">The column 3, row 3 value</param>
        /// <param name="m14">The column 1, row 4 value</param>
        /// <param name="m24">The column 2, row 4 value</param>
        /// <param name="m34">The column 3, row 4 value</param>
        /// <param name="m15">The column 1, row 5 value</param>
        /// <param name="m25">The column 2, row 5 value</param>
        /// <param name="m35">The column 3, row 5 value</param>
        /// <param name="m16">The column 1, row 6 value</param>
        /// <param name="m26">The column 2, row 6 value</param>
        /// <param name="m36">The column 3, row 6 value</param>
        /// <param name="m17">The column 1, row 7 value</param>
        /// <param name="m27">The column 2, row 7 value</param>
        /// <param name="m37">The column 3, row 7 value</param>
        /// <param name="m18">The column 1, row 8 value</param>
        /// <param name="m28">The column 2, row 8 value</param>
        /// <param name="m38">The column 3, row 8 value</param>
        public Matrix3x8(double m11, double m21, double m31, 
                         double m12, double m22, double m32, 
                         double m13, double m23, double m33, 
                         double m14, double m24, double m34, 
                         double m15, double m25, double m35, 
                         double m16, double m26, double m36, 
                         double m17, double m27, double m37, 
                         double m18, double m28, double m38)
        {
			M11 = m11; M21 = m21; M31 = m31; 
			M12 = m12; M22 = m22; M32 = m32; 
			M13 = m13; M23 = m23; M33 = m33; 
			M14 = m14; M24 = m24; M34 = m34; 
			M15 = m15; M25 = m25; M35 = m35; 
			M16 = m16; M26 = m26; M36 = m36; 
			M17 = m17; M27 = m27; M37 = m37; 
			M18 = m18; M28 = m28; M38 = m38; 
        }

        /// <summary>
        /// Initialized a Matrix3x8 with all values set to the same value
        /// </summary>
        /// <param name="value">The value to set all values to</param>
        public Matrix3x8(double value)
        {
			M11 = M21 = M31 = 
			M12 = M22 = M32 = 
			M13 = M23 = M33 = 
			M14 = M24 = M34 = 
			M15 = M25 = M35 = 
			M16 = M26 = M36 = 
			M17 = M27 = M37 = 
			M18 = M28 = M38 = value;
        }

		public double M11;
		public double M21;
		public double M31;
		public double M12;
		public double M22;
		public double M32;
		public double M13;
		public double M23;
		public double M33;
		public double M14;
		public double M24;
		public double M34;
		public double M15;
		public double M25;
		public double M35;
		public double M16;
		public double M26;
		public double M36;
		public double M17;
		public double M27;
		public double M37;
		public double M18;
		public double M28;
		public double M38;

        public unsafe double this[int col, int row]
        {
            get
            {
                if (col < 0 || col >= ColumnCount)
                    throw new ArgumentOutOfRangeException("col");
                if (row < 0 || row >= RowCount)
                    throw new ArgumentOutOfRangeException("col");

                fixed (Matrix3x8* p = &this)
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

                fixed (Matrix3x8* p = &this)
                {
                    double* d = (double*)p;
                    d[row * ColumnCount + col] = value;
                }
            }
        }

        /// <summary>
        /// Gets the number of columns in the matrix
        /// </summary>
        public int Columns { get { return Matrix3x8.ColumnCount; } }
        /// <summary>
        /// Get the number of rows in the matrix
        /// </summary>
        public int Rows { get { return Matrix3x8.RowCount; } }

        /// <summary>
        /// Gets a new Matrix1x8 containing the values of column 1
        /// </summary>
        public Matrix1x8 Column1 { get { return new Matrix1x8(M11, M12, M13, M14, M15, M16, M17, M18); } }
        /// <summary>
        /// Gets a new Matrix1x8 containing the values of column 2
        /// </summary>
        public Matrix1x8 Column2 { get { return new Matrix1x8(M21, M22, M23, M24, M25, M26, M27, M28); } }
        /// <summary>
        /// Gets a new Matrix1x8 containing the values of column 3
        /// </summary>
        public Matrix1x8 Column3 { get { return new Matrix1x8(M31, M32, M33, M34, M35, M36, M37, M38); } }
        /// <summary>
        /// Gets a new Matrix3x1 containing the values of column 1
        /// </summary>
        public Matrix3x1 Row1 { get { return new Matrix3x1(M11, M21, M31); } }
        /// <summary>
        /// Gets a new Matrix3x1 containing the values of column 2
        /// </summary>
        public Matrix3x1 Row2 { get { return new Matrix3x1(M12, M22, M32); } }
        /// <summary>
        /// Gets a new Matrix3x1 containing the values of column 3
        /// </summary>
        public Matrix3x1 Row3 { get { return new Matrix3x1(M13, M23, M33); } }
        /// <summary>
        /// Gets a new Matrix3x1 containing the values of column 4
        /// </summary>
        public Matrix3x1 Row4 { get { return new Matrix3x1(M14, M24, M34); } }
        /// <summary>
        /// Gets a new Matrix3x1 containing the values of column 5
        /// </summary>
        public Matrix3x1 Row5 { get { return new Matrix3x1(M15, M25, M35); } }
        /// <summary>
        /// Gets a new Matrix3x1 containing the values of column 6
        /// </summary>
        public Matrix3x1 Row6 { get { return new Matrix3x1(M16, M26, M36); } }
        /// <summary>
        /// Gets a new Matrix3x1 containing the values of column 7
        /// </summary>
        public Matrix3x1 Row7 { get { return new Matrix3x1(M17, M27, M37); } }
        /// <summary>
        /// Gets a new Matrix3x1 containing the values of column 8
        /// </summary>
        public Matrix3x1 Row8 { get { return new Matrix3x1(M18, M28, M38); } }

        public override bool Equals(object obj)
        {
            if (obj is Matrix3x8)
                return this == (Matrix3x8)obj;

            return false;
        }

        public bool Equals(Matrix3x8 other)
        {
            return this == other;
        }

        public unsafe override int GetHashCode()
        {
            fixed (Matrix3x8* p = &this)
            {
                int* x = (int*)p;
                unchecked
                {
                    return (x[00] ^ x[01]) + (x[02] ^ x[03]) + (x[04] ^ x[05])
                         + (x[03] ^ x[04]) + (x[05] ^ x[06]) + (x[07] ^ x[08])
                         + (x[06] ^ x[07]) + (x[08] ^ x[09]) + (x[10] ^ x[11])
                         + (x[09] ^ x[10]) + (x[11] ^ x[12]) + (x[13] ^ x[14])
                         + (x[12] ^ x[13]) + (x[14] ^ x[15]) + (x[16] ^ x[17])
                         + (x[15] ^ x[16]) + (x[17] ^ x[18]) + (x[19] ^ x[20])
                         + (x[18] ^ x[19]) + (x[20] ^ x[21]) + (x[22] ^ x[23])
                         + (x[21] ^ x[22]) + (x[23] ^ x[24]) + (x[25] ^ x[26]);
                }
            }
        }

        public override string ToString()
        {
            return String.Format("Matrix3x8: "
                               + "{{|{00}|{01}|{02}|}}"
                               + "{{|{03}|{04}|{05}|}}"
                               + "{{|{06}|{07}|{08}|}}"
                               + "{{|{09}|{10}|{11}|}}"
                               + "{{|{12}|{13}|{14}|}}"
                               + "{{|{15}|{16}|{17}|}}"
                               + "{{|{18}|{19}|{20}|}}"
                               + "{{|{21}|{22}|{23}|}}"
                               , M11, M21, M31
                               , M12, M22, M32
                               , M13, M23, M33
                               , M14, M24, M34
                               , M15, M25, M35
                               , M16, M26, M36
                               , M17, M27, M37
                               , M18, M28, M38); 
        }

        /// <summary>
        /// Creates and returns a transposed matrix
        /// </summary>
        /// <returns>Matrix with transposed values</returns>
        public Matrix8x3 Transpose()
        {
            return new Matrix8x3(M11, M12, M13, M14, M15, M16, M17, M18, 
                                 M21, M22, M23, M24, M25, M26, M27, M28, 
                                 M31, M32, M33, M34, M35, M36, M37, M38);
        }

        public static bool operator ==(Matrix3x8 matrix1, Matrix3x8 matrix2)
        {
            return (matrix1 == matrix2 || Math.Abs(matrix1.M11 - matrix2.M11) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M21 - matrix2.M21) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M31 - matrix2.M31) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M12 - matrix2.M12) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M22 - matrix2.M22) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M32 - matrix2.M32) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M13 - matrix2.M13) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M23 - matrix2.M23) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M33 - matrix2.M33) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M14 - matrix2.M14) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M24 - matrix2.M24) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M34 - matrix2.M34) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M15 - matrix2.M15) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M25 - matrix2.M25) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M35 - matrix2.M35) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M16 - matrix2.M16) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M26 - matrix2.M26) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M36 - matrix2.M36) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M17 - matrix2.M17) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M27 - matrix2.M27) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M37 - matrix2.M37) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M18 - matrix2.M18) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M28 - matrix2.M28) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M38 - matrix2.M38) <= Double.Epsilon);
        }

        public static bool operator !=(Matrix3x8 matrix1, Matrix3x8 matrix2)
        {
            return Math.Abs(matrix1.M11 - matrix2.M11) > Double.Epsilon
                || Math.Abs(matrix1.M21 - matrix2.M21) > Double.Epsilon
                || Math.Abs(matrix1.M31 - matrix2.M31) > Double.Epsilon
                || Math.Abs(matrix1.M12 - matrix2.M12) > Double.Epsilon
                || Math.Abs(matrix1.M22 - matrix2.M22) > Double.Epsilon
                || Math.Abs(matrix1.M32 - matrix2.M32) > Double.Epsilon
                || Math.Abs(matrix1.M13 - matrix2.M13) > Double.Epsilon
                || Math.Abs(matrix1.M23 - matrix2.M23) > Double.Epsilon
                || Math.Abs(matrix1.M33 - matrix2.M33) > Double.Epsilon
                || Math.Abs(matrix1.M14 - matrix2.M14) > Double.Epsilon
                || Math.Abs(matrix1.M24 - matrix2.M24) > Double.Epsilon
                || Math.Abs(matrix1.M34 - matrix2.M34) > Double.Epsilon
                || Math.Abs(matrix1.M15 - matrix2.M15) > Double.Epsilon
                || Math.Abs(matrix1.M25 - matrix2.M25) > Double.Epsilon
                || Math.Abs(matrix1.M35 - matrix2.M35) > Double.Epsilon
                || Math.Abs(matrix1.M16 - matrix2.M16) > Double.Epsilon
                || Math.Abs(matrix1.M26 - matrix2.M26) > Double.Epsilon
                || Math.Abs(matrix1.M36 - matrix2.M36) > Double.Epsilon
                || Math.Abs(matrix1.M17 - matrix2.M17) > Double.Epsilon
                || Math.Abs(matrix1.M27 - matrix2.M27) > Double.Epsilon
                || Math.Abs(matrix1.M37 - matrix2.M37) > Double.Epsilon
                || Math.Abs(matrix1.M18 - matrix2.M18) > Double.Epsilon
                || Math.Abs(matrix1.M28 - matrix2.M28) > Double.Epsilon
                || Math.Abs(matrix1.M38 - matrix2.M38) > Double.Epsilon;
        }

        public static Matrix3x8 operator +(Matrix3x8 matrix1, Matrix3x8 matrix2)
        {
            double m11 = matrix1.M11 + matrix2.M11;
            double m21 = matrix1.M21 + matrix2.M21;
            double m31 = matrix1.M31 + matrix2.M31;
            double m12 = matrix1.M12 + matrix2.M12;
            double m22 = matrix1.M22 + matrix2.M22;
            double m32 = matrix1.M32 + matrix2.M32;
            double m13 = matrix1.M13 + matrix2.M13;
            double m23 = matrix1.M23 + matrix2.M23;
            double m33 = matrix1.M33 + matrix2.M33;
            double m14 = matrix1.M14 + matrix2.M14;
            double m24 = matrix1.M24 + matrix2.M24;
            double m34 = matrix1.M34 + matrix2.M34;
            double m15 = matrix1.M15 + matrix2.M15;
            double m25 = matrix1.M25 + matrix2.M25;
            double m35 = matrix1.M35 + matrix2.M35;
            double m16 = matrix1.M16 + matrix2.M16;
            double m26 = matrix1.M26 + matrix2.M26;
            double m36 = matrix1.M36 + matrix2.M36;
            double m17 = matrix1.M17 + matrix2.M17;
            double m27 = matrix1.M27 + matrix2.M27;
            double m37 = matrix1.M37 + matrix2.M37;
            double m18 = matrix1.M18 + matrix2.M18;
            double m28 = matrix1.M28 + matrix2.M28;
            double m38 = matrix1.M38 + matrix2.M38;

            return new Matrix3x8(m11, m21, m31, 
                                 m12, m22, m32, 
                                 m13, m23, m33, 
                                 m14, m24, m34, 
                                 m15, m25, m35, 
                                 m16, m26, m36, 
                                 m17, m27, m37, 
                                 m18, m28, m38);
        }

        public static Matrix3x8 operator -(Matrix3x8 matrix1, Matrix3x8 matrix2)
        {
            double m11 = matrix1.M11 - matrix2.M11;
            double m21 = matrix1.M21 - matrix2.M21;
            double m31 = matrix1.M31 - matrix2.M31;
            double m12 = matrix1.M12 - matrix2.M12;
            double m22 = matrix1.M22 - matrix2.M22;
            double m32 = matrix1.M32 - matrix2.M32;
            double m13 = matrix1.M13 - matrix2.M13;
            double m23 = matrix1.M23 - matrix2.M23;
            double m33 = matrix1.M33 - matrix2.M33;
            double m14 = matrix1.M14 - matrix2.M14;
            double m24 = matrix1.M24 - matrix2.M24;
            double m34 = matrix1.M34 - matrix2.M34;
            double m15 = matrix1.M15 - matrix2.M15;
            double m25 = matrix1.M25 - matrix2.M25;
            double m35 = matrix1.M35 - matrix2.M35;
            double m16 = matrix1.M16 - matrix2.M16;
            double m26 = matrix1.M26 - matrix2.M26;
            double m36 = matrix1.M36 - matrix2.M36;
            double m17 = matrix1.M17 - matrix2.M17;
            double m27 = matrix1.M27 - matrix2.M27;
            double m37 = matrix1.M37 - matrix2.M37;
            double m18 = matrix1.M18 - matrix2.M18;
            double m28 = matrix1.M28 - matrix2.M28;
            double m38 = matrix1.M38 - matrix2.M38;

            return new Matrix3x8(m11, m21, m31, 
                                 m12, m22, m32, 
                                 m13, m23, m33, 
                                 m14, m24, m34, 
                                 m15, m25, m35, 
                                 m16, m26, m36, 
                                 m17, m27, m37, 
                                 m18, m28, m38);
        }

        public static Matrix3x8 operator *(Matrix3x8 matrix, double scalar)
        {
            double m11 = matrix.M11 * scalar;
            double m21 = matrix.M21 * scalar;
            double m31 = matrix.M31 * scalar;
            double m12 = matrix.M12 * scalar;
            double m22 = matrix.M22 * scalar;
            double m32 = matrix.M32 * scalar;
            double m13 = matrix.M13 * scalar;
            double m23 = matrix.M23 * scalar;
            double m33 = matrix.M33 * scalar;
            double m14 = matrix.M14 * scalar;
            double m24 = matrix.M24 * scalar;
            double m34 = matrix.M34 * scalar;
            double m15 = matrix.M15 * scalar;
            double m25 = matrix.M25 * scalar;
            double m35 = matrix.M35 * scalar;
            double m16 = matrix.M16 * scalar;
            double m26 = matrix.M26 * scalar;
            double m36 = matrix.M36 * scalar;
            double m17 = matrix.M17 * scalar;
            double m27 = matrix.M27 * scalar;
            double m37 = matrix.M37 * scalar;
            double m18 = matrix.M18 * scalar;
            double m28 = matrix.M28 * scalar;
            double m38 = matrix.M38 * scalar;

            return new Matrix3x8(m11, m21, m31, 
                                 m12, m22, m32, 
                                 m13, m23, m33, 
                                 m14, m24, m34, 
                                 m15, m25, m35, 
                                 m16, m26, m36, 
                                 m17, m27, m37, 
                                 m18, m28, m38);
        }

        public static Matrix3x8 operator *(double scalar, Matrix3x8 matrix)
        {
            double m11 = scalar * matrix.M11;
            double m21 = scalar * matrix.M21;
            double m31 = scalar * matrix.M31;
            double m12 = scalar * matrix.M12;
            double m22 = scalar * matrix.M22;
            double m32 = scalar * matrix.M32;
            double m13 = scalar * matrix.M13;
            double m23 = scalar * matrix.M23;
            double m33 = scalar * matrix.M33;
            double m14 = scalar * matrix.M14;
            double m24 = scalar * matrix.M24;
            double m34 = scalar * matrix.M34;
            double m15 = scalar * matrix.M15;
            double m25 = scalar * matrix.M25;
            double m35 = scalar * matrix.M35;
            double m16 = scalar * matrix.M16;
            double m26 = scalar * matrix.M26;
            double m36 = scalar * matrix.M36;
            double m17 = scalar * matrix.M17;
            double m27 = scalar * matrix.M27;
            double m37 = scalar * matrix.M37;
            double m18 = scalar * matrix.M18;
            double m28 = scalar * matrix.M28;
            double m38 = scalar * matrix.M38;

            return new Matrix3x8(m11, m21, m31, 
                                 m12, m22, m32, 
                                 m13, m23, m33, 
                                 m14, m24, m34, 
                                 m15, m25, m35, 
                                 m16, m26, m36, 
                                 m17, m27, m37, 
                                 m18, m28, m38);
        }

        public static Matrix1x8 operator *(Matrix3x8 matrix1, Matrix1x3 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12 + matrix1.M31 * matrix2.M13;
            double m12 = matrix1.M12 * matrix2.M11 + matrix1.M22 * matrix2.M12 + matrix1.M32 * matrix2.M13;
            double m13 = matrix1.M13 * matrix2.M11 + matrix1.M23 * matrix2.M12 + matrix1.M33 * matrix2.M13;
            double m14 = matrix1.M14 * matrix2.M11 + matrix1.M24 * matrix2.M12 + matrix1.M34 * matrix2.M13;
            double m15 = matrix1.M15 * matrix2.M11 + matrix1.M25 * matrix2.M12 + matrix1.M35 * matrix2.M13;
            double m16 = matrix1.M16 * matrix2.M11 + matrix1.M26 * matrix2.M12 + matrix1.M36 * matrix2.M13;
            double m17 = matrix1.M17 * matrix2.M11 + matrix1.M27 * matrix2.M12 + matrix1.M37 * matrix2.M13;
            double m18 = matrix1.M18 * matrix2.M11 + matrix1.M28 * matrix2.M12 + matrix1.M38 * matrix2.M13;

            return new Matrix1x8(m11, 
                                 m12, 
                                 m13, 
                                 m14, 
                                 m15, 
                                 m16, 
                                 m17, 
                                 m18);
        }
        public static Matrix2x8 operator *(Matrix3x8 matrix1, Matrix2x3 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12 + matrix1.M31 * matrix2.M13;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22 + matrix1.M31 * matrix2.M23;
            double m12 = matrix1.M12 * matrix2.M11 + matrix1.M22 * matrix2.M12 + matrix1.M32 * matrix2.M13;
            double m22 = matrix1.M12 * matrix2.M21 + matrix1.M22 * matrix2.M22 + matrix1.M32 * matrix2.M23;
            double m13 = matrix1.M13 * matrix2.M11 + matrix1.M23 * matrix2.M12 + matrix1.M33 * matrix2.M13;
            double m23 = matrix1.M13 * matrix2.M21 + matrix1.M23 * matrix2.M22 + matrix1.M33 * matrix2.M23;
            double m14 = matrix1.M14 * matrix2.M11 + matrix1.M24 * matrix2.M12 + matrix1.M34 * matrix2.M13;
            double m24 = matrix1.M14 * matrix2.M21 + matrix1.M24 * matrix2.M22 + matrix1.M34 * matrix2.M23;
            double m15 = matrix1.M15 * matrix2.M11 + matrix1.M25 * matrix2.M12 + matrix1.M35 * matrix2.M13;
            double m25 = matrix1.M15 * matrix2.M21 + matrix1.M25 * matrix2.M22 + matrix1.M35 * matrix2.M23;
            double m16 = matrix1.M16 * matrix2.M11 + matrix1.M26 * matrix2.M12 + matrix1.M36 * matrix2.M13;
            double m26 = matrix1.M16 * matrix2.M21 + matrix1.M26 * matrix2.M22 + matrix1.M36 * matrix2.M23;
            double m17 = matrix1.M17 * matrix2.M11 + matrix1.M27 * matrix2.M12 + matrix1.M37 * matrix2.M13;
            double m27 = matrix1.M17 * matrix2.M21 + matrix1.M27 * matrix2.M22 + matrix1.M37 * matrix2.M23;
            double m18 = matrix1.M18 * matrix2.M11 + matrix1.M28 * matrix2.M12 + matrix1.M38 * matrix2.M13;
            double m28 = matrix1.M18 * matrix2.M21 + matrix1.M28 * matrix2.M22 + matrix1.M38 * matrix2.M23;

            return new Matrix2x8(m11, m21, 
                                 m12, m22, 
                                 m13, m23, 
                                 m14, m24, 
                                 m15, m25, 
                                 m16, m26, 
                                 m17, m27, 
                                 m18, m28);
        }
        public static Matrix3x8 operator *(Matrix3x8 matrix1, Matrix3x3 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12 + matrix1.M31 * matrix2.M13;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22 + matrix1.M31 * matrix2.M23;
            double m31 = matrix1.M11 * matrix2.M31 + matrix1.M21 * matrix2.M32 + matrix1.M31 * matrix2.M33;
            double m12 = matrix1.M12 * matrix2.M11 + matrix1.M22 * matrix2.M12 + matrix1.M32 * matrix2.M13;
            double m22 = matrix1.M12 * matrix2.M21 + matrix1.M22 * matrix2.M22 + matrix1.M32 * matrix2.M23;
            double m32 = matrix1.M12 * matrix2.M31 + matrix1.M22 * matrix2.M32 + matrix1.M32 * matrix2.M33;
            double m13 = matrix1.M13 * matrix2.M11 + matrix1.M23 * matrix2.M12 + matrix1.M33 * matrix2.M13;
            double m23 = matrix1.M13 * matrix2.M21 + matrix1.M23 * matrix2.M22 + matrix1.M33 * matrix2.M23;
            double m33 = matrix1.M13 * matrix2.M31 + matrix1.M23 * matrix2.M32 + matrix1.M33 * matrix2.M33;
            double m14 = matrix1.M14 * matrix2.M11 + matrix1.M24 * matrix2.M12 + matrix1.M34 * matrix2.M13;
            double m24 = matrix1.M14 * matrix2.M21 + matrix1.M24 * matrix2.M22 + matrix1.M34 * matrix2.M23;
            double m34 = matrix1.M14 * matrix2.M31 + matrix1.M24 * matrix2.M32 + matrix1.M34 * matrix2.M33;
            double m15 = matrix1.M15 * matrix2.M11 + matrix1.M25 * matrix2.M12 + matrix1.M35 * matrix2.M13;
            double m25 = matrix1.M15 * matrix2.M21 + matrix1.M25 * matrix2.M22 + matrix1.M35 * matrix2.M23;
            double m35 = matrix1.M15 * matrix2.M31 + matrix1.M25 * matrix2.M32 + matrix1.M35 * matrix2.M33;
            double m16 = matrix1.M16 * matrix2.M11 + matrix1.M26 * matrix2.M12 + matrix1.M36 * matrix2.M13;
            double m26 = matrix1.M16 * matrix2.M21 + matrix1.M26 * matrix2.M22 + matrix1.M36 * matrix2.M23;
            double m36 = matrix1.M16 * matrix2.M31 + matrix1.M26 * matrix2.M32 + matrix1.M36 * matrix2.M33;
            double m17 = matrix1.M17 * matrix2.M11 + matrix1.M27 * matrix2.M12 + matrix1.M37 * matrix2.M13;
            double m27 = matrix1.M17 * matrix2.M21 + matrix1.M27 * matrix2.M22 + matrix1.M37 * matrix2.M23;
            double m37 = matrix1.M17 * matrix2.M31 + matrix1.M27 * matrix2.M32 + matrix1.M37 * matrix2.M33;
            double m18 = matrix1.M18 * matrix2.M11 + matrix1.M28 * matrix2.M12 + matrix1.M38 * matrix2.M13;
            double m28 = matrix1.M18 * matrix2.M21 + matrix1.M28 * matrix2.M22 + matrix1.M38 * matrix2.M23;
            double m38 = matrix1.M18 * matrix2.M31 + matrix1.M28 * matrix2.M32 + matrix1.M38 * matrix2.M33;

            return new Matrix3x8(m11, m21, m31, 
                                 m12, m22, m32, 
                                 m13, m23, m33, 
                                 m14, m24, m34, 
                                 m15, m25, m35, 
                                 m16, m26, m36, 
                                 m17, m27, m37, 
                                 m18, m28, m38);
        }
        public static Matrix4x8 operator *(Matrix3x8 matrix1, Matrix4x3 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12 + matrix1.M31 * matrix2.M13;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22 + matrix1.M31 * matrix2.M23;
            double m31 = matrix1.M11 * matrix2.M31 + matrix1.M21 * matrix2.M32 + matrix1.M31 * matrix2.M33;
            double m41 = matrix1.M11 * matrix2.M41 + matrix1.M21 * matrix2.M42 + matrix1.M31 * matrix2.M43;
            double m12 = matrix1.M12 * matrix2.M11 + matrix1.M22 * matrix2.M12 + matrix1.M32 * matrix2.M13;
            double m22 = matrix1.M12 * matrix2.M21 + matrix1.M22 * matrix2.M22 + matrix1.M32 * matrix2.M23;
            double m32 = matrix1.M12 * matrix2.M31 + matrix1.M22 * matrix2.M32 + matrix1.M32 * matrix2.M33;
            double m42 = matrix1.M12 * matrix2.M41 + matrix1.M22 * matrix2.M42 + matrix1.M32 * matrix2.M43;
            double m13 = matrix1.M13 * matrix2.M11 + matrix1.M23 * matrix2.M12 + matrix1.M33 * matrix2.M13;
            double m23 = matrix1.M13 * matrix2.M21 + matrix1.M23 * matrix2.M22 + matrix1.M33 * matrix2.M23;
            double m33 = matrix1.M13 * matrix2.M31 + matrix1.M23 * matrix2.M32 + matrix1.M33 * matrix2.M33;
            double m43 = matrix1.M13 * matrix2.M41 + matrix1.M23 * matrix2.M42 + matrix1.M33 * matrix2.M43;
            double m14 = matrix1.M14 * matrix2.M11 + matrix1.M24 * matrix2.M12 + matrix1.M34 * matrix2.M13;
            double m24 = matrix1.M14 * matrix2.M21 + matrix1.M24 * matrix2.M22 + matrix1.M34 * matrix2.M23;
            double m34 = matrix1.M14 * matrix2.M31 + matrix1.M24 * matrix2.M32 + matrix1.M34 * matrix2.M33;
            double m44 = matrix1.M14 * matrix2.M41 + matrix1.M24 * matrix2.M42 + matrix1.M34 * matrix2.M43;
            double m15 = matrix1.M15 * matrix2.M11 + matrix1.M25 * matrix2.M12 + matrix1.M35 * matrix2.M13;
            double m25 = matrix1.M15 * matrix2.M21 + matrix1.M25 * matrix2.M22 + matrix1.M35 * matrix2.M23;
            double m35 = matrix1.M15 * matrix2.M31 + matrix1.M25 * matrix2.M32 + matrix1.M35 * matrix2.M33;
            double m45 = matrix1.M15 * matrix2.M41 + matrix1.M25 * matrix2.M42 + matrix1.M35 * matrix2.M43;
            double m16 = matrix1.M16 * matrix2.M11 + matrix1.M26 * matrix2.M12 + matrix1.M36 * matrix2.M13;
            double m26 = matrix1.M16 * matrix2.M21 + matrix1.M26 * matrix2.M22 + matrix1.M36 * matrix2.M23;
            double m36 = matrix1.M16 * matrix2.M31 + matrix1.M26 * matrix2.M32 + matrix1.M36 * matrix2.M33;
            double m46 = matrix1.M16 * matrix2.M41 + matrix1.M26 * matrix2.M42 + matrix1.M36 * matrix2.M43;
            double m17 = matrix1.M17 * matrix2.M11 + matrix1.M27 * matrix2.M12 + matrix1.M37 * matrix2.M13;
            double m27 = matrix1.M17 * matrix2.M21 + matrix1.M27 * matrix2.M22 + matrix1.M37 * matrix2.M23;
            double m37 = matrix1.M17 * matrix2.M31 + matrix1.M27 * matrix2.M32 + matrix1.M37 * matrix2.M33;
            double m47 = matrix1.M17 * matrix2.M41 + matrix1.M27 * matrix2.M42 + matrix1.M37 * matrix2.M43;
            double m18 = matrix1.M18 * matrix2.M11 + matrix1.M28 * matrix2.M12 + matrix1.M38 * matrix2.M13;
            double m28 = matrix1.M18 * matrix2.M21 + matrix1.M28 * matrix2.M22 + matrix1.M38 * matrix2.M23;
            double m38 = matrix1.M18 * matrix2.M31 + matrix1.M28 * matrix2.M32 + matrix1.M38 * matrix2.M33;
            double m48 = matrix1.M18 * matrix2.M41 + matrix1.M28 * matrix2.M42 + matrix1.M38 * matrix2.M43;

            return new Matrix4x8(m11, m21, m31, m41, 
                                 m12, m22, m32, m42, 
                                 m13, m23, m33, m43, 
                                 m14, m24, m34, m44, 
                                 m15, m25, m35, m45, 
                                 m16, m26, m36, m46, 
                                 m17, m27, m37, m47, 
                                 m18, m28, m38, m48);
        }
        public static Matrix5x8 operator *(Matrix3x8 matrix1, Matrix5x3 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12 + matrix1.M31 * matrix2.M13;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22 + matrix1.M31 * matrix2.M23;
            double m31 = matrix1.M11 * matrix2.M31 + matrix1.M21 * matrix2.M32 + matrix1.M31 * matrix2.M33;
            double m41 = matrix1.M11 * matrix2.M41 + matrix1.M21 * matrix2.M42 + matrix1.M31 * matrix2.M43;
            double m51 = matrix1.M11 * matrix2.M51 + matrix1.M21 * matrix2.M52 + matrix1.M31 * matrix2.M53;
            double m12 = matrix1.M12 * matrix2.M11 + matrix1.M22 * matrix2.M12 + matrix1.M32 * matrix2.M13;
            double m22 = matrix1.M12 * matrix2.M21 + matrix1.M22 * matrix2.M22 + matrix1.M32 * matrix2.M23;
            double m32 = matrix1.M12 * matrix2.M31 + matrix1.M22 * matrix2.M32 + matrix1.M32 * matrix2.M33;
            double m42 = matrix1.M12 * matrix2.M41 + matrix1.M22 * matrix2.M42 + matrix1.M32 * matrix2.M43;
            double m52 = matrix1.M12 * matrix2.M51 + matrix1.M22 * matrix2.M52 + matrix1.M32 * matrix2.M53;
            double m13 = matrix1.M13 * matrix2.M11 + matrix1.M23 * matrix2.M12 + matrix1.M33 * matrix2.M13;
            double m23 = matrix1.M13 * matrix2.M21 + matrix1.M23 * matrix2.M22 + matrix1.M33 * matrix2.M23;
            double m33 = matrix1.M13 * matrix2.M31 + matrix1.M23 * matrix2.M32 + matrix1.M33 * matrix2.M33;
            double m43 = matrix1.M13 * matrix2.M41 + matrix1.M23 * matrix2.M42 + matrix1.M33 * matrix2.M43;
            double m53 = matrix1.M13 * matrix2.M51 + matrix1.M23 * matrix2.M52 + matrix1.M33 * matrix2.M53;
            double m14 = matrix1.M14 * matrix2.M11 + matrix1.M24 * matrix2.M12 + matrix1.M34 * matrix2.M13;
            double m24 = matrix1.M14 * matrix2.M21 + matrix1.M24 * matrix2.M22 + matrix1.M34 * matrix2.M23;
            double m34 = matrix1.M14 * matrix2.M31 + matrix1.M24 * matrix2.M32 + matrix1.M34 * matrix2.M33;
            double m44 = matrix1.M14 * matrix2.M41 + matrix1.M24 * matrix2.M42 + matrix1.M34 * matrix2.M43;
            double m54 = matrix1.M14 * matrix2.M51 + matrix1.M24 * matrix2.M52 + matrix1.M34 * matrix2.M53;
            double m15 = matrix1.M15 * matrix2.M11 + matrix1.M25 * matrix2.M12 + matrix1.M35 * matrix2.M13;
            double m25 = matrix1.M15 * matrix2.M21 + matrix1.M25 * matrix2.M22 + matrix1.M35 * matrix2.M23;
            double m35 = matrix1.M15 * matrix2.M31 + matrix1.M25 * matrix2.M32 + matrix1.M35 * matrix2.M33;
            double m45 = matrix1.M15 * matrix2.M41 + matrix1.M25 * matrix2.M42 + matrix1.M35 * matrix2.M43;
            double m55 = matrix1.M15 * matrix2.M51 + matrix1.M25 * matrix2.M52 + matrix1.M35 * matrix2.M53;
            double m16 = matrix1.M16 * matrix2.M11 + matrix1.M26 * matrix2.M12 + matrix1.M36 * matrix2.M13;
            double m26 = matrix1.M16 * matrix2.M21 + matrix1.M26 * matrix2.M22 + matrix1.M36 * matrix2.M23;
            double m36 = matrix1.M16 * matrix2.M31 + matrix1.M26 * matrix2.M32 + matrix1.M36 * matrix2.M33;
            double m46 = matrix1.M16 * matrix2.M41 + matrix1.M26 * matrix2.M42 + matrix1.M36 * matrix2.M43;
            double m56 = matrix1.M16 * matrix2.M51 + matrix1.M26 * matrix2.M52 + matrix1.M36 * matrix2.M53;
            double m17 = matrix1.M17 * matrix2.M11 + matrix1.M27 * matrix2.M12 + matrix1.M37 * matrix2.M13;
            double m27 = matrix1.M17 * matrix2.M21 + matrix1.M27 * matrix2.M22 + matrix1.M37 * matrix2.M23;
            double m37 = matrix1.M17 * matrix2.M31 + matrix1.M27 * matrix2.M32 + matrix1.M37 * matrix2.M33;
            double m47 = matrix1.M17 * matrix2.M41 + matrix1.M27 * matrix2.M42 + matrix1.M37 * matrix2.M43;
            double m57 = matrix1.M17 * matrix2.M51 + matrix1.M27 * matrix2.M52 + matrix1.M37 * matrix2.M53;
            double m18 = matrix1.M18 * matrix2.M11 + matrix1.M28 * matrix2.M12 + matrix1.M38 * matrix2.M13;
            double m28 = matrix1.M18 * matrix2.M21 + matrix1.M28 * matrix2.M22 + matrix1.M38 * matrix2.M23;
            double m38 = matrix1.M18 * matrix2.M31 + matrix1.M28 * matrix2.M32 + matrix1.M38 * matrix2.M33;
            double m48 = matrix1.M18 * matrix2.M41 + matrix1.M28 * matrix2.M42 + matrix1.M38 * matrix2.M43;
            double m58 = matrix1.M18 * matrix2.M51 + matrix1.M28 * matrix2.M52 + matrix1.M38 * matrix2.M53;

            return new Matrix5x8(m11, m21, m31, m41, m51, 
                                 m12, m22, m32, m42, m52, 
                                 m13, m23, m33, m43, m53, 
                                 m14, m24, m34, m44, m54, 
                                 m15, m25, m35, m45, m55, 
                                 m16, m26, m36, m46, m56, 
                                 m17, m27, m37, m47, m57, 
                                 m18, m28, m38, m48, m58);
        }
        public static Matrix6x8 operator *(Matrix3x8 matrix1, Matrix6x3 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12 + matrix1.M31 * matrix2.M13;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22 + matrix1.M31 * matrix2.M23;
            double m31 = matrix1.M11 * matrix2.M31 + matrix1.M21 * matrix2.M32 + matrix1.M31 * matrix2.M33;
            double m41 = matrix1.M11 * matrix2.M41 + matrix1.M21 * matrix2.M42 + matrix1.M31 * matrix2.M43;
            double m51 = matrix1.M11 * matrix2.M51 + matrix1.M21 * matrix2.M52 + matrix1.M31 * matrix2.M53;
            double m61 = matrix1.M11 * matrix2.M61 + matrix1.M21 * matrix2.M62 + matrix1.M31 * matrix2.M63;
            double m12 = matrix1.M12 * matrix2.M11 + matrix1.M22 * matrix2.M12 + matrix1.M32 * matrix2.M13;
            double m22 = matrix1.M12 * matrix2.M21 + matrix1.M22 * matrix2.M22 + matrix1.M32 * matrix2.M23;
            double m32 = matrix1.M12 * matrix2.M31 + matrix1.M22 * matrix2.M32 + matrix1.M32 * matrix2.M33;
            double m42 = matrix1.M12 * matrix2.M41 + matrix1.M22 * matrix2.M42 + matrix1.M32 * matrix2.M43;
            double m52 = matrix1.M12 * matrix2.M51 + matrix1.M22 * matrix2.M52 + matrix1.M32 * matrix2.M53;
            double m62 = matrix1.M12 * matrix2.M61 + matrix1.M22 * matrix2.M62 + matrix1.M32 * matrix2.M63;
            double m13 = matrix1.M13 * matrix2.M11 + matrix1.M23 * matrix2.M12 + matrix1.M33 * matrix2.M13;
            double m23 = matrix1.M13 * matrix2.M21 + matrix1.M23 * matrix2.M22 + matrix1.M33 * matrix2.M23;
            double m33 = matrix1.M13 * matrix2.M31 + matrix1.M23 * matrix2.M32 + matrix1.M33 * matrix2.M33;
            double m43 = matrix1.M13 * matrix2.M41 + matrix1.M23 * matrix2.M42 + matrix1.M33 * matrix2.M43;
            double m53 = matrix1.M13 * matrix2.M51 + matrix1.M23 * matrix2.M52 + matrix1.M33 * matrix2.M53;
            double m63 = matrix1.M13 * matrix2.M61 + matrix1.M23 * matrix2.M62 + matrix1.M33 * matrix2.M63;
            double m14 = matrix1.M14 * matrix2.M11 + matrix1.M24 * matrix2.M12 + matrix1.M34 * matrix2.M13;
            double m24 = matrix1.M14 * matrix2.M21 + matrix1.M24 * matrix2.M22 + matrix1.M34 * matrix2.M23;
            double m34 = matrix1.M14 * matrix2.M31 + matrix1.M24 * matrix2.M32 + matrix1.M34 * matrix2.M33;
            double m44 = matrix1.M14 * matrix2.M41 + matrix1.M24 * matrix2.M42 + matrix1.M34 * matrix2.M43;
            double m54 = matrix1.M14 * matrix2.M51 + matrix1.M24 * matrix2.M52 + matrix1.M34 * matrix2.M53;
            double m64 = matrix1.M14 * matrix2.M61 + matrix1.M24 * matrix2.M62 + matrix1.M34 * matrix2.M63;
            double m15 = matrix1.M15 * matrix2.M11 + matrix1.M25 * matrix2.M12 + matrix1.M35 * matrix2.M13;
            double m25 = matrix1.M15 * matrix2.M21 + matrix1.M25 * matrix2.M22 + matrix1.M35 * matrix2.M23;
            double m35 = matrix1.M15 * matrix2.M31 + matrix1.M25 * matrix2.M32 + matrix1.M35 * matrix2.M33;
            double m45 = matrix1.M15 * matrix2.M41 + matrix1.M25 * matrix2.M42 + matrix1.M35 * matrix2.M43;
            double m55 = matrix1.M15 * matrix2.M51 + matrix1.M25 * matrix2.M52 + matrix1.M35 * matrix2.M53;
            double m65 = matrix1.M15 * matrix2.M61 + matrix1.M25 * matrix2.M62 + matrix1.M35 * matrix2.M63;
            double m16 = matrix1.M16 * matrix2.M11 + matrix1.M26 * matrix2.M12 + matrix1.M36 * matrix2.M13;
            double m26 = matrix1.M16 * matrix2.M21 + matrix1.M26 * matrix2.M22 + matrix1.M36 * matrix2.M23;
            double m36 = matrix1.M16 * matrix2.M31 + matrix1.M26 * matrix2.M32 + matrix1.M36 * matrix2.M33;
            double m46 = matrix1.M16 * matrix2.M41 + matrix1.M26 * matrix2.M42 + matrix1.M36 * matrix2.M43;
            double m56 = matrix1.M16 * matrix2.M51 + matrix1.M26 * matrix2.M52 + matrix1.M36 * matrix2.M53;
            double m66 = matrix1.M16 * matrix2.M61 + matrix1.M26 * matrix2.M62 + matrix1.M36 * matrix2.M63;
            double m17 = matrix1.M17 * matrix2.M11 + matrix1.M27 * matrix2.M12 + matrix1.M37 * matrix2.M13;
            double m27 = matrix1.M17 * matrix2.M21 + matrix1.M27 * matrix2.M22 + matrix1.M37 * matrix2.M23;
            double m37 = matrix1.M17 * matrix2.M31 + matrix1.M27 * matrix2.M32 + matrix1.M37 * matrix2.M33;
            double m47 = matrix1.M17 * matrix2.M41 + matrix1.M27 * matrix2.M42 + matrix1.M37 * matrix2.M43;
            double m57 = matrix1.M17 * matrix2.M51 + matrix1.M27 * matrix2.M52 + matrix1.M37 * matrix2.M53;
            double m67 = matrix1.M17 * matrix2.M61 + matrix1.M27 * matrix2.M62 + matrix1.M37 * matrix2.M63;
            double m18 = matrix1.M18 * matrix2.M11 + matrix1.M28 * matrix2.M12 + matrix1.M38 * matrix2.M13;
            double m28 = matrix1.M18 * matrix2.M21 + matrix1.M28 * matrix2.M22 + matrix1.M38 * matrix2.M23;
            double m38 = matrix1.M18 * matrix2.M31 + matrix1.M28 * matrix2.M32 + matrix1.M38 * matrix2.M33;
            double m48 = matrix1.M18 * matrix2.M41 + matrix1.M28 * matrix2.M42 + matrix1.M38 * matrix2.M43;
            double m58 = matrix1.M18 * matrix2.M51 + matrix1.M28 * matrix2.M52 + matrix1.M38 * matrix2.M53;
            double m68 = matrix1.M18 * matrix2.M61 + matrix1.M28 * matrix2.M62 + matrix1.M38 * matrix2.M63;

            return new Matrix6x8(m11, m21, m31, m41, m51, m61, 
                                 m12, m22, m32, m42, m52, m62, 
                                 m13, m23, m33, m43, m53, m63, 
                                 m14, m24, m34, m44, m54, m64, 
                                 m15, m25, m35, m45, m55, m65, 
                                 m16, m26, m36, m46, m56, m66, 
                                 m17, m27, m37, m47, m57, m67, 
                                 m18, m28, m38, m48, m58, m68);
        }
        public static Matrix7x8 operator *(Matrix3x8 matrix1, Matrix7x3 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12 + matrix1.M31 * matrix2.M13;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22 + matrix1.M31 * matrix2.M23;
            double m31 = matrix1.M11 * matrix2.M31 + matrix1.M21 * matrix2.M32 + matrix1.M31 * matrix2.M33;
            double m41 = matrix1.M11 * matrix2.M41 + matrix1.M21 * matrix2.M42 + matrix1.M31 * matrix2.M43;
            double m51 = matrix1.M11 * matrix2.M51 + matrix1.M21 * matrix2.M52 + matrix1.M31 * matrix2.M53;
            double m61 = matrix1.M11 * matrix2.M61 + matrix1.M21 * matrix2.M62 + matrix1.M31 * matrix2.M63;
            double m71 = matrix1.M11 * matrix2.M71 + matrix1.M21 * matrix2.M72 + matrix1.M31 * matrix2.M73;
            double m12 = matrix1.M12 * matrix2.M11 + matrix1.M22 * matrix2.M12 + matrix1.M32 * matrix2.M13;
            double m22 = matrix1.M12 * matrix2.M21 + matrix1.M22 * matrix2.M22 + matrix1.M32 * matrix2.M23;
            double m32 = matrix1.M12 * matrix2.M31 + matrix1.M22 * matrix2.M32 + matrix1.M32 * matrix2.M33;
            double m42 = matrix1.M12 * matrix2.M41 + matrix1.M22 * matrix2.M42 + matrix1.M32 * matrix2.M43;
            double m52 = matrix1.M12 * matrix2.M51 + matrix1.M22 * matrix2.M52 + matrix1.M32 * matrix2.M53;
            double m62 = matrix1.M12 * matrix2.M61 + matrix1.M22 * matrix2.M62 + matrix1.M32 * matrix2.M63;
            double m72 = matrix1.M12 * matrix2.M71 + matrix1.M22 * matrix2.M72 + matrix1.M32 * matrix2.M73;
            double m13 = matrix1.M13 * matrix2.M11 + matrix1.M23 * matrix2.M12 + matrix1.M33 * matrix2.M13;
            double m23 = matrix1.M13 * matrix2.M21 + matrix1.M23 * matrix2.M22 + matrix1.M33 * matrix2.M23;
            double m33 = matrix1.M13 * matrix2.M31 + matrix1.M23 * matrix2.M32 + matrix1.M33 * matrix2.M33;
            double m43 = matrix1.M13 * matrix2.M41 + matrix1.M23 * matrix2.M42 + matrix1.M33 * matrix2.M43;
            double m53 = matrix1.M13 * matrix2.M51 + matrix1.M23 * matrix2.M52 + matrix1.M33 * matrix2.M53;
            double m63 = matrix1.M13 * matrix2.M61 + matrix1.M23 * matrix2.M62 + matrix1.M33 * matrix2.M63;
            double m73 = matrix1.M13 * matrix2.M71 + matrix1.M23 * matrix2.M72 + matrix1.M33 * matrix2.M73;
            double m14 = matrix1.M14 * matrix2.M11 + matrix1.M24 * matrix2.M12 + matrix1.M34 * matrix2.M13;
            double m24 = matrix1.M14 * matrix2.M21 + matrix1.M24 * matrix2.M22 + matrix1.M34 * matrix2.M23;
            double m34 = matrix1.M14 * matrix2.M31 + matrix1.M24 * matrix2.M32 + matrix1.M34 * matrix2.M33;
            double m44 = matrix1.M14 * matrix2.M41 + matrix1.M24 * matrix2.M42 + matrix1.M34 * matrix2.M43;
            double m54 = matrix1.M14 * matrix2.M51 + matrix1.M24 * matrix2.M52 + matrix1.M34 * matrix2.M53;
            double m64 = matrix1.M14 * matrix2.M61 + matrix1.M24 * matrix2.M62 + matrix1.M34 * matrix2.M63;
            double m74 = matrix1.M14 * matrix2.M71 + matrix1.M24 * matrix2.M72 + matrix1.M34 * matrix2.M73;
            double m15 = matrix1.M15 * matrix2.M11 + matrix1.M25 * matrix2.M12 + matrix1.M35 * matrix2.M13;
            double m25 = matrix1.M15 * matrix2.M21 + matrix1.M25 * matrix2.M22 + matrix1.M35 * matrix2.M23;
            double m35 = matrix1.M15 * matrix2.M31 + matrix1.M25 * matrix2.M32 + matrix1.M35 * matrix2.M33;
            double m45 = matrix1.M15 * matrix2.M41 + matrix1.M25 * matrix2.M42 + matrix1.M35 * matrix2.M43;
            double m55 = matrix1.M15 * matrix2.M51 + matrix1.M25 * matrix2.M52 + matrix1.M35 * matrix2.M53;
            double m65 = matrix1.M15 * matrix2.M61 + matrix1.M25 * matrix2.M62 + matrix1.M35 * matrix2.M63;
            double m75 = matrix1.M15 * matrix2.M71 + matrix1.M25 * matrix2.M72 + matrix1.M35 * matrix2.M73;
            double m16 = matrix1.M16 * matrix2.M11 + matrix1.M26 * matrix2.M12 + matrix1.M36 * matrix2.M13;
            double m26 = matrix1.M16 * matrix2.M21 + matrix1.M26 * matrix2.M22 + matrix1.M36 * matrix2.M23;
            double m36 = matrix1.M16 * matrix2.M31 + matrix1.M26 * matrix2.M32 + matrix1.M36 * matrix2.M33;
            double m46 = matrix1.M16 * matrix2.M41 + matrix1.M26 * matrix2.M42 + matrix1.M36 * matrix2.M43;
            double m56 = matrix1.M16 * matrix2.M51 + matrix1.M26 * matrix2.M52 + matrix1.M36 * matrix2.M53;
            double m66 = matrix1.M16 * matrix2.M61 + matrix1.M26 * matrix2.M62 + matrix1.M36 * matrix2.M63;
            double m76 = matrix1.M16 * matrix2.M71 + matrix1.M26 * matrix2.M72 + matrix1.M36 * matrix2.M73;
            double m17 = matrix1.M17 * matrix2.M11 + matrix1.M27 * matrix2.M12 + matrix1.M37 * matrix2.M13;
            double m27 = matrix1.M17 * matrix2.M21 + matrix1.M27 * matrix2.M22 + matrix1.M37 * matrix2.M23;
            double m37 = matrix1.M17 * matrix2.M31 + matrix1.M27 * matrix2.M32 + matrix1.M37 * matrix2.M33;
            double m47 = matrix1.M17 * matrix2.M41 + matrix1.M27 * matrix2.M42 + matrix1.M37 * matrix2.M43;
            double m57 = matrix1.M17 * matrix2.M51 + matrix1.M27 * matrix2.M52 + matrix1.M37 * matrix2.M53;
            double m67 = matrix1.M17 * matrix2.M61 + matrix1.M27 * matrix2.M62 + matrix1.M37 * matrix2.M63;
            double m77 = matrix1.M17 * matrix2.M71 + matrix1.M27 * matrix2.M72 + matrix1.M37 * matrix2.M73;
            double m18 = matrix1.M18 * matrix2.M11 + matrix1.M28 * matrix2.M12 + matrix1.M38 * matrix2.M13;
            double m28 = matrix1.M18 * matrix2.M21 + matrix1.M28 * matrix2.M22 + matrix1.M38 * matrix2.M23;
            double m38 = matrix1.M18 * matrix2.M31 + matrix1.M28 * matrix2.M32 + matrix1.M38 * matrix2.M33;
            double m48 = matrix1.M18 * matrix2.M41 + matrix1.M28 * matrix2.M42 + matrix1.M38 * matrix2.M43;
            double m58 = matrix1.M18 * matrix2.M51 + matrix1.M28 * matrix2.M52 + matrix1.M38 * matrix2.M53;
            double m68 = matrix1.M18 * matrix2.M61 + matrix1.M28 * matrix2.M62 + matrix1.M38 * matrix2.M63;
            double m78 = matrix1.M18 * matrix2.M71 + matrix1.M28 * matrix2.M72 + matrix1.M38 * matrix2.M73;

            return new Matrix7x8(m11, m21, m31, m41, m51, m61, m71, 
                                 m12, m22, m32, m42, m52, m62, m72, 
                                 m13, m23, m33, m43, m53, m63, m73, 
                                 m14, m24, m34, m44, m54, m64, m74, 
                                 m15, m25, m35, m45, m55, m65, m75, 
                                 m16, m26, m36, m46, m56, m66, m76, 
                                 m17, m27, m37, m47, m57, m67, m77, 
                                 m18, m28, m38, m48, m58, m68, m78);
        }
        public static Matrix8x8 operator *(Matrix3x8 matrix1, Matrix8x3 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12 + matrix1.M31 * matrix2.M13;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22 + matrix1.M31 * matrix2.M23;
            double m31 = matrix1.M11 * matrix2.M31 + matrix1.M21 * matrix2.M32 + matrix1.M31 * matrix2.M33;
            double m41 = matrix1.M11 * matrix2.M41 + matrix1.M21 * matrix2.M42 + matrix1.M31 * matrix2.M43;
            double m51 = matrix1.M11 * matrix2.M51 + matrix1.M21 * matrix2.M52 + matrix1.M31 * matrix2.M53;
            double m61 = matrix1.M11 * matrix2.M61 + matrix1.M21 * matrix2.M62 + matrix1.M31 * matrix2.M63;
            double m71 = matrix1.M11 * matrix2.M71 + matrix1.M21 * matrix2.M72 + matrix1.M31 * matrix2.M73;
            double m81 = matrix1.M11 * matrix2.M81 + matrix1.M21 * matrix2.M82 + matrix1.M31 * matrix2.M83;
            double m12 = matrix1.M12 * matrix2.M11 + matrix1.M22 * matrix2.M12 + matrix1.M32 * matrix2.M13;
            double m22 = matrix1.M12 * matrix2.M21 + matrix1.M22 * matrix2.M22 + matrix1.M32 * matrix2.M23;
            double m32 = matrix1.M12 * matrix2.M31 + matrix1.M22 * matrix2.M32 + matrix1.M32 * matrix2.M33;
            double m42 = matrix1.M12 * matrix2.M41 + matrix1.M22 * matrix2.M42 + matrix1.M32 * matrix2.M43;
            double m52 = matrix1.M12 * matrix2.M51 + matrix1.M22 * matrix2.M52 + matrix1.M32 * matrix2.M53;
            double m62 = matrix1.M12 * matrix2.M61 + matrix1.M22 * matrix2.M62 + matrix1.M32 * matrix2.M63;
            double m72 = matrix1.M12 * matrix2.M71 + matrix1.M22 * matrix2.M72 + matrix1.M32 * matrix2.M73;
            double m82 = matrix1.M12 * matrix2.M81 + matrix1.M22 * matrix2.M82 + matrix1.M32 * matrix2.M83;
            double m13 = matrix1.M13 * matrix2.M11 + matrix1.M23 * matrix2.M12 + matrix1.M33 * matrix2.M13;
            double m23 = matrix1.M13 * matrix2.M21 + matrix1.M23 * matrix2.M22 + matrix1.M33 * matrix2.M23;
            double m33 = matrix1.M13 * matrix2.M31 + matrix1.M23 * matrix2.M32 + matrix1.M33 * matrix2.M33;
            double m43 = matrix1.M13 * matrix2.M41 + matrix1.M23 * matrix2.M42 + matrix1.M33 * matrix2.M43;
            double m53 = matrix1.M13 * matrix2.M51 + matrix1.M23 * matrix2.M52 + matrix1.M33 * matrix2.M53;
            double m63 = matrix1.M13 * matrix2.M61 + matrix1.M23 * matrix2.M62 + matrix1.M33 * matrix2.M63;
            double m73 = matrix1.M13 * matrix2.M71 + matrix1.M23 * matrix2.M72 + matrix1.M33 * matrix2.M73;
            double m83 = matrix1.M13 * matrix2.M81 + matrix1.M23 * matrix2.M82 + matrix1.M33 * matrix2.M83;
            double m14 = matrix1.M14 * matrix2.M11 + matrix1.M24 * matrix2.M12 + matrix1.M34 * matrix2.M13;
            double m24 = matrix1.M14 * matrix2.M21 + matrix1.M24 * matrix2.M22 + matrix1.M34 * matrix2.M23;
            double m34 = matrix1.M14 * matrix2.M31 + matrix1.M24 * matrix2.M32 + matrix1.M34 * matrix2.M33;
            double m44 = matrix1.M14 * matrix2.M41 + matrix1.M24 * matrix2.M42 + matrix1.M34 * matrix2.M43;
            double m54 = matrix1.M14 * matrix2.M51 + matrix1.M24 * matrix2.M52 + matrix1.M34 * matrix2.M53;
            double m64 = matrix1.M14 * matrix2.M61 + matrix1.M24 * matrix2.M62 + matrix1.M34 * matrix2.M63;
            double m74 = matrix1.M14 * matrix2.M71 + matrix1.M24 * matrix2.M72 + matrix1.M34 * matrix2.M73;
            double m84 = matrix1.M14 * matrix2.M81 + matrix1.M24 * matrix2.M82 + matrix1.M34 * matrix2.M83;
            double m15 = matrix1.M15 * matrix2.M11 + matrix1.M25 * matrix2.M12 + matrix1.M35 * matrix2.M13;
            double m25 = matrix1.M15 * matrix2.M21 + matrix1.M25 * matrix2.M22 + matrix1.M35 * matrix2.M23;
            double m35 = matrix1.M15 * matrix2.M31 + matrix1.M25 * matrix2.M32 + matrix1.M35 * matrix2.M33;
            double m45 = matrix1.M15 * matrix2.M41 + matrix1.M25 * matrix2.M42 + matrix1.M35 * matrix2.M43;
            double m55 = matrix1.M15 * matrix2.M51 + matrix1.M25 * matrix2.M52 + matrix1.M35 * matrix2.M53;
            double m65 = matrix1.M15 * matrix2.M61 + matrix1.M25 * matrix2.M62 + matrix1.M35 * matrix2.M63;
            double m75 = matrix1.M15 * matrix2.M71 + matrix1.M25 * matrix2.M72 + matrix1.M35 * matrix2.M73;
            double m85 = matrix1.M15 * matrix2.M81 + matrix1.M25 * matrix2.M82 + matrix1.M35 * matrix2.M83;
            double m16 = matrix1.M16 * matrix2.M11 + matrix1.M26 * matrix2.M12 + matrix1.M36 * matrix2.M13;
            double m26 = matrix1.M16 * matrix2.M21 + matrix1.M26 * matrix2.M22 + matrix1.M36 * matrix2.M23;
            double m36 = matrix1.M16 * matrix2.M31 + matrix1.M26 * matrix2.M32 + matrix1.M36 * matrix2.M33;
            double m46 = matrix1.M16 * matrix2.M41 + matrix1.M26 * matrix2.M42 + matrix1.M36 * matrix2.M43;
            double m56 = matrix1.M16 * matrix2.M51 + matrix1.M26 * matrix2.M52 + matrix1.M36 * matrix2.M53;
            double m66 = matrix1.M16 * matrix2.M61 + matrix1.M26 * matrix2.M62 + matrix1.M36 * matrix2.M63;
            double m76 = matrix1.M16 * matrix2.M71 + matrix1.M26 * matrix2.M72 + matrix1.M36 * matrix2.M73;
            double m86 = matrix1.M16 * matrix2.M81 + matrix1.M26 * matrix2.M82 + matrix1.M36 * matrix2.M83;
            double m17 = matrix1.M17 * matrix2.M11 + matrix1.M27 * matrix2.M12 + matrix1.M37 * matrix2.M13;
            double m27 = matrix1.M17 * matrix2.M21 + matrix1.M27 * matrix2.M22 + matrix1.M37 * matrix2.M23;
            double m37 = matrix1.M17 * matrix2.M31 + matrix1.M27 * matrix2.M32 + matrix1.M37 * matrix2.M33;
            double m47 = matrix1.M17 * matrix2.M41 + matrix1.M27 * matrix2.M42 + matrix1.M37 * matrix2.M43;
            double m57 = matrix1.M17 * matrix2.M51 + matrix1.M27 * matrix2.M52 + matrix1.M37 * matrix2.M53;
            double m67 = matrix1.M17 * matrix2.M61 + matrix1.M27 * matrix2.M62 + matrix1.M37 * matrix2.M63;
            double m77 = matrix1.M17 * matrix2.M71 + matrix1.M27 * matrix2.M72 + matrix1.M37 * matrix2.M73;
            double m87 = matrix1.M17 * matrix2.M81 + matrix1.M27 * matrix2.M82 + matrix1.M37 * matrix2.M83;
            double m18 = matrix1.M18 * matrix2.M11 + matrix1.M28 * matrix2.M12 + matrix1.M38 * matrix2.M13;
            double m28 = matrix1.M18 * matrix2.M21 + matrix1.M28 * matrix2.M22 + matrix1.M38 * matrix2.M23;
            double m38 = matrix1.M18 * matrix2.M31 + matrix1.M28 * matrix2.M32 + matrix1.M38 * matrix2.M33;
            double m48 = matrix1.M18 * matrix2.M41 + matrix1.M28 * matrix2.M42 + matrix1.M38 * matrix2.M43;
            double m58 = matrix1.M18 * matrix2.M51 + matrix1.M28 * matrix2.M52 + matrix1.M38 * matrix2.M53;
            double m68 = matrix1.M18 * matrix2.M61 + matrix1.M28 * matrix2.M62 + matrix1.M38 * matrix2.M63;
            double m78 = matrix1.M18 * matrix2.M71 + matrix1.M28 * matrix2.M72 + matrix1.M38 * matrix2.M73;
            double m88 = matrix1.M18 * matrix2.M81 + matrix1.M28 * matrix2.M82 + matrix1.M38 * matrix2.M83;

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
