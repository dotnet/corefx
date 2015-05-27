using System.Runtime.InteropServices;

namespace System.Numerics.Matrices
{
    /// <summary>
    /// Represents a matrix of double precision floating-point values defined by its number of columns and rows
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix8x7: IEquatable<Matrix8x7>, IMatrix
    {
        public const int ColumnCount = 8;
        public const int RowCount = 7;

        static Matrix8x7()
        {
            Zero = new Matrix8x7(0);
        }

        /// <summary>
        /// Constant Matrix8x7 with all values initialized to zero
        /// </summary>
        public static readonly Matrix8x7 Zero;

        /// <summary>
        /// Initializes a Matrix8x7 with all of it values specifically set
        /// </summary>
        /// <param name="m11">The column 1, row 1 value</param>
        /// <param name="m21">The column 2, row 1 value</param>
        /// <param name="m31">The column 3, row 1 value</param>
        /// <param name="m41">The column 4, row 1 value</param>
        /// <param name="m51">The column 5, row 1 value</param>
        /// <param name="m61">The column 6, row 1 value</param>
        /// <param name="m71">The column 7, row 1 value</param>
        /// <param name="m81">The column 8, row 1 value</param>
        /// <param name="m12">The column 1, row 2 value</param>
        /// <param name="m22">The column 2, row 2 value</param>
        /// <param name="m32">The column 3, row 2 value</param>
        /// <param name="m42">The column 4, row 2 value</param>
        /// <param name="m52">The column 5, row 2 value</param>
        /// <param name="m62">The column 6, row 2 value</param>
        /// <param name="m72">The column 7, row 2 value</param>
        /// <param name="m82">The column 8, row 2 value</param>
        /// <param name="m13">The column 1, row 3 value</param>
        /// <param name="m23">The column 2, row 3 value</param>
        /// <param name="m33">The column 3, row 3 value</param>
        /// <param name="m43">The column 4, row 3 value</param>
        /// <param name="m53">The column 5, row 3 value</param>
        /// <param name="m63">The column 6, row 3 value</param>
        /// <param name="m73">The column 7, row 3 value</param>
        /// <param name="m83">The column 8, row 3 value</param>
        /// <param name="m14">The column 1, row 4 value</param>
        /// <param name="m24">The column 2, row 4 value</param>
        /// <param name="m34">The column 3, row 4 value</param>
        /// <param name="m44">The column 4, row 4 value</param>
        /// <param name="m54">The column 5, row 4 value</param>
        /// <param name="m64">The column 6, row 4 value</param>
        /// <param name="m74">The column 7, row 4 value</param>
        /// <param name="m84">The column 8, row 4 value</param>
        /// <param name="m15">The column 1, row 5 value</param>
        /// <param name="m25">The column 2, row 5 value</param>
        /// <param name="m35">The column 3, row 5 value</param>
        /// <param name="m45">The column 4, row 5 value</param>
        /// <param name="m55">The column 5, row 5 value</param>
        /// <param name="m65">The column 6, row 5 value</param>
        /// <param name="m75">The column 7, row 5 value</param>
        /// <param name="m85">The column 8, row 5 value</param>
        /// <param name="m16">The column 1, row 6 value</param>
        /// <param name="m26">The column 2, row 6 value</param>
        /// <param name="m36">The column 3, row 6 value</param>
        /// <param name="m46">The column 4, row 6 value</param>
        /// <param name="m56">The column 5, row 6 value</param>
        /// <param name="m66">The column 6, row 6 value</param>
        /// <param name="m76">The column 7, row 6 value</param>
        /// <param name="m86">The column 8, row 6 value</param>
        /// <param name="m17">The column 1, row 7 value</param>
        /// <param name="m27">The column 2, row 7 value</param>
        /// <param name="m37">The column 3, row 7 value</param>
        /// <param name="m47">The column 4, row 7 value</param>
        /// <param name="m57">The column 5, row 7 value</param>
        /// <param name="m67">The column 6, row 7 value</param>
        /// <param name="m77">The column 7, row 7 value</param>
        /// <param name="m87">The column 8, row 7 value</param>
        public Matrix8x7(double m11, double m21, double m31, double m41, double m51, double m61, double m71, double m81, 
                         double m12, double m22, double m32, double m42, double m52, double m62, double m72, double m82, 
                         double m13, double m23, double m33, double m43, double m53, double m63, double m73, double m83, 
                         double m14, double m24, double m34, double m44, double m54, double m64, double m74, double m84, 
                         double m15, double m25, double m35, double m45, double m55, double m65, double m75, double m85, 
                         double m16, double m26, double m36, double m46, double m56, double m66, double m76, double m86, 
                         double m17, double m27, double m37, double m47, double m57, double m67, double m77, double m87)
        {
			M11 = m11; M21 = m21; M31 = m31; M41 = m41; M51 = m51; M61 = m61; M71 = m71; M81 = m81; 
			M12 = m12; M22 = m22; M32 = m32; M42 = m42; M52 = m52; M62 = m62; M72 = m72; M82 = m82; 
			M13 = m13; M23 = m23; M33 = m33; M43 = m43; M53 = m53; M63 = m63; M73 = m73; M83 = m83; 
			M14 = m14; M24 = m24; M34 = m34; M44 = m44; M54 = m54; M64 = m64; M74 = m74; M84 = m84; 
			M15 = m15; M25 = m25; M35 = m35; M45 = m45; M55 = m55; M65 = m65; M75 = m75; M85 = m85; 
			M16 = m16; M26 = m26; M36 = m36; M46 = m46; M56 = m56; M66 = m66; M76 = m76; M86 = m86; 
			M17 = m17; M27 = m27; M37 = m37; M47 = m47; M57 = m57; M67 = m67; M77 = m77; M87 = m87; 
        }

        /// <summary>
        /// Initialized a Matrix8x7 with all values set to the same value
        /// </summary>
        /// <param name="value">The value to set all values to</param>
        public Matrix8x7(double value)
        {
			M11 = M21 = M31 = M41 = M51 = M61 = M71 = M81 = 
			M12 = M22 = M32 = M42 = M52 = M62 = M72 = M82 = 
			M13 = M23 = M33 = M43 = M53 = M63 = M73 = M83 = 
			M14 = M24 = M34 = M44 = M54 = M64 = M74 = M84 = 
			M15 = M25 = M35 = M45 = M55 = M65 = M75 = M85 = 
			M16 = M26 = M36 = M46 = M56 = M66 = M76 = M86 = 
			M17 = M27 = M37 = M47 = M57 = M67 = M77 = M87 = value;
        }

		public double M11;
		public double M21;
		public double M31;
		public double M41;
		public double M51;
		public double M61;
		public double M71;
		public double M81;
		public double M12;
		public double M22;
		public double M32;
		public double M42;
		public double M52;
		public double M62;
		public double M72;
		public double M82;
		public double M13;
		public double M23;
		public double M33;
		public double M43;
		public double M53;
		public double M63;
		public double M73;
		public double M83;
		public double M14;
		public double M24;
		public double M34;
		public double M44;
		public double M54;
		public double M64;
		public double M74;
		public double M84;
		public double M15;
		public double M25;
		public double M35;
		public double M45;
		public double M55;
		public double M65;
		public double M75;
		public double M85;
		public double M16;
		public double M26;
		public double M36;
		public double M46;
		public double M56;
		public double M66;
		public double M76;
		public double M86;
		public double M17;
		public double M27;
		public double M37;
		public double M47;
		public double M57;
		public double M67;
		public double M77;
		public double M87;

        public unsafe double this[int col, int row]
        {
            get
            {
                if (col < 0 || col >= ColumnCount)
                    throw new ArgumentOutOfRangeException("col");
                if (row < 0 || row >= RowCount)
                    throw new ArgumentOutOfRangeException("col");

                fixed (Matrix8x7* p = &this)
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

                fixed (Matrix8x7* p = &this)
                {
                    double* d = (double*)p;
                    d[row * ColumnCount + col] = value;
                }
            }
        }

        /// <summary>
        /// Gets the number of columns in the matrix
        /// </summary>
        public int Columns { get { return Matrix8x7.ColumnCount; } }
        /// <summary>
        /// Get the number of rows in the matrix
        /// </summary>
        public int Rows { get { return Matrix8x7.RowCount; } }

        /// <summary>
        /// Gets a new Matrix1x7 containing the values of column 1
        /// </summary>
        public Matrix1x7 Column1 { get { return new Matrix1x7(M11, M12, M13, M14, M15, M16, M17); } }
        /// <summary>
        /// Gets a new Matrix1x7 containing the values of column 2
        /// </summary>
        public Matrix1x7 Column2 { get { return new Matrix1x7(M21, M22, M23, M24, M25, M26, M27); } }
        /// <summary>
        /// Gets a new Matrix1x7 containing the values of column 3
        /// </summary>
        public Matrix1x7 Column3 { get { return new Matrix1x7(M31, M32, M33, M34, M35, M36, M37); } }
        /// <summary>
        /// Gets a new Matrix1x7 containing the values of column 4
        /// </summary>
        public Matrix1x7 Column4 { get { return new Matrix1x7(M41, M42, M43, M44, M45, M46, M47); } }
        /// <summary>
        /// Gets a new Matrix1x7 containing the values of column 5
        /// </summary>
        public Matrix1x7 Column5 { get { return new Matrix1x7(M51, M52, M53, M54, M55, M56, M57); } }
        /// <summary>
        /// Gets a new Matrix1x7 containing the values of column 6
        /// </summary>
        public Matrix1x7 Column6 { get { return new Matrix1x7(M61, M62, M63, M64, M65, M66, M67); } }
        /// <summary>
        /// Gets a new Matrix1x7 containing the values of column 7
        /// </summary>
        public Matrix1x7 Column7 { get { return new Matrix1x7(M71, M72, M73, M74, M75, M76, M77); } }
        /// <summary>
        /// Gets a new Matrix1x7 containing the values of column 8
        /// </summary>
        public Matrix1x7 Column8 { get { return new Matrix1x7(M81, M82, M83, M84, M85, M86, M87); } }
        /// <summary>
        /// Gets a new Matrix8x1 containing the values of column 1
        /// </summary>
        public Matrix8x1 Row1 { get { return new Matrix8x1(M11, M21, M31, M41, M51, M61, M71, M81); } }
        /// <summary>
        /// Gets a new Matrix8x1 containing the values of column 2
        /// </summary>
        public Matrix8x1 Row2 { get { return new Matrix8x1(M12, M22, M32, M42, M52, M62, M72, M82); } }
        /// <summary>
        /// Gets a new Matrix8x1 containing the values of column 3
        /// </summary>
        public Matrix8x1 Row3 { get { return new Matrix8x1(M13, M23, M33, M43, M53, M63, M73, M83); } }
        /// <summary>
        /// Gets a new Matrix8x1 containing the values of column 4
        /// </summary>
        public Matrix8x1 Row4 { get { return new Matrix8x1(M14, M24, M34, M44, M54, M64, M74, M84); } }
        /// <summary>
        /// Gets a new Matrix8x1 containing the values of column 5
        /// </summary>
        public Matrix8x1 Row5 { get { return new Matrix8x1(M15, M25, M35, M45, M55, M65, M75, M85); } }
        /// <summary>
        /// Gets a new Matrix8x1 containing the values of column 6
        /// </summary>
        public Matrix8x1 Row6 { get { return new Matrix8x1(M16, M26, M36, M46, M56, M66, M76, M86); } }
        /// <summary>
        /// Gets a new Matrix8x1 containing the values of column 7
        /// </summary>
        public Matrix8x1 Row7 { get { return new Matrix8x1(M17, M27, M37, M47, M57, M67, M77, M87); } }

        public override bool Equals(object obj)
        {
            if (obj is Matrix8x7)
                return this == (Matrix8x7)obj;

            return false;
        }

        public bool Equals(Matrix8x7 other)
        {
            return this == other;
        }

        public unsafe override int GetHashCode()
        {
            fixed (Matrix8x7* p = &this)
            {
                int* x = (int*)p;
                unchecked
                {
                    return (x[00] ^ x[01]) + (x[02] ^ x[03]) + (x[04] ^ x[05]) + (x[06] ^ x[07]) + (x[08] ^ x[09]) + (x[10] ^ x[11]) + (x[12] ^ x[13]) + (x[14] ^ x[15])
                         + (x[08] ^ x[09]) + (x[10] ^ x[11]) + (x[12] ^ x[13]) + (x[14] ^ x[15]) + (x[16] ^ x[17]) + (x[18] ^ x[19]) + (x[20] ^ x[21]) + (x[22] ^ x[23])
                         + (x[16] ^ x[17]) + (x[18] ^ x[19]) + (x[20] ^ x[21]) + (x[22] ^ x[23]) + (x[24] ^ x[25]) + (x[26] ^ x[27]) + (x[28] ^ x[29]) + (x[30] ^ x[31])
                         + (x[24] ^ x[25]) + (x[26] ^ x[27]) + (x[28] ^ x[29]) + (x[30] ^ x[31]) + (x[32] ^ x[33]) + (x[34] ^ x[35]) + (x[36] ^ x[37]) + (x[38] ^ x[39])
                         + (x[32] ^ x[33]) + (x[34] ^ x[35]) + (x[36] ^ x[37]) + (x[38] ^ x[39]) + (x[40] ^ x[41]) + (x[42] ^ x[43]) + (x[44] ^ x[45]) + (x[46] ^ x[47])
                         + (x[40] ^ x[41]) + (x[42] ^ x[43]) + (x[44] ^ x[45]) + (x[46] ^ x[47]) + (x[48] ^ x[49]) + (x[50] ^ x[51]) + (x[52] ^ x[53]) + (x[54] ^ x[55])
                         + (x[48] ^ x[49]) + (x[50] ^ x[51]) + (x[52] ^ x[53]) + (x[54] ^ x[55]) + (x[56] ^ x[57]) + (x[58] ^ x[59]) + (x[60] ^ x[61]) + (x[62] ^ x[63]);
                }
            }
        }

        public override string ToString()
        {
            return String.Format("Matrix8x7: "
                               + "{{|{00}|{01}|{02}|{03}|{04}|{05}|{06}|{07}|}}"
                               + "{{|{08}|{09}|{10}|{11}|{12}|{13}|{14}|{15}|}}"
                               + "{{|{16}|{17}|{18}|{19}|{20}|{21}|{22}|{23}|}}"
                               + "{{|{24}|{25}|{26}|{27}|{28}|{29}|{30}|{31}|}}"
                               + "{{|{32}|{33}|{34}|{35}|{36}|{37}|{38}|{39}|}}"
                               + "{{|{40}|{41}|{42}|{43}|{44}|{45}|{46}|{47}|}}"
                               + "{{|{48}|{49}|{50}|{51}|{52}|{53}|{54}|{55}|}}"
                               , M11, M21, M31, M41, M51, M61, M71, M81
                               , M12, M22, M32, M42, M52, M62, M72, M82
                               , M13, M23, M33, M43, M53, M63, M73, M83
                               , M14, M24, M34, M44, M54, M64, M74, M84
                               , M15, M25, M35, M45, M55, M65, M75, M85
                               , M16, M26, M36, M46, M56, M66, M76, M86
                               , M17, M27, M37, M47, M57, M67, M77, M87); 
        }

        /// <summary>
        /// Creates and returns a transposed matrix
        /// </summary>
        /// <returns>Matrix with transposed values</returns>
        public Matrix7x8 Transpose()
        {
            return new Matrix7x8(M11, M12, M13, M14, M15, M16, M17, 
                                 M21, M22, M23, M24, M25, M26, M27, 
                                 M31, M32, M33, M34, M35, M36, M37, 
                                 M41, M42, M43, M44, M45, M46, M47, 
                                 M51, M52, M53, M54, M55, M56, M57, 
                                 M61, M62, M63, M64, M65, M66, M67, 
                                 M71, M72, M73, M74, M75, M76, M77, 
                                 M81, M82, M83, M84, M85, M86, M87);
        }

        public static bool operator ==(Matrix8x7 matrix1, Matrix8x7 matrix2)
        {
            return (matrix1 == matrix2 || Math.Abs(matrix1.M11 - matrix2.M11) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M21 - matrix2.M21) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M31 - matrix2.M31) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M41 - matrix2.M41) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M51 - matrix2.M51) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M61 - matrix2.M61) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M71 - matrix2.M71) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M81 - matrix2.M81) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M12 - matrix2.M12) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M22 - matrix2.M22) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M32 - matrix2.M32) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M42 - matrix2.M42) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M52 - matrix2.M52) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M62 - matrix2.M62) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M72 - matrix2.M72) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M82 - matrix2.M82) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M13 - matrix2.M13) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M23 - matrix2.M23) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M33 - matrix2.M33) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M43 - matrix2.M43) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M53 - matrix2.M53) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M63 - matrix2.M63) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M73 - matrix2.M73) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M83 - matrix2.M83) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M14 - matrix2.M14) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M24 - matrix2.M24) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M34 - matrix2.M34) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M44 - matrix2.M44) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M54 - matrix2.M54) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M64 - matrix2.M64) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M74 - matrix2.M74) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M84 - matrix2.M84) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M15 - matrix2.M15) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M25 - matrix2.M25) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M35 - matrix2.M35) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M45 - matrix2.M45) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M55 - matrix2.M55) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M65 - matrix2.M65) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M75 - matrix2.M75) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M85 - matrix2.M85) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M16 - matrix2.M16) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M26 - matrix2.M26) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M36 - matrix2.M36) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M46 - matrix2.M46) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M56 - matrix2.M56) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M66 - matrix2.M66) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M76 - matrix2.M76) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M86 - matrix2.M86) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M17 - matrix2.M17) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M27 - matrix2.M27) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M37 - matrix2.M37) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M47 - matrix2.M47) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M57 - matrix2.M57) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M67 - matrix2.M67) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M77 - matrix2.M77) <= Double.Epsilon)
                && (matrix1 == matrix2 || Math.Abs(matrix1.M87 - matrix2.M87) <= Double.Epsilon);
        }

        public static bool operator !=(Matrix8x7 matrix1, Matrix8x7 matrix2)
        {
            return Math.Abs(matrix1.M11 - matrix2.M11) > Double.Epsilon
                || Math.Abs(matrix1.M21 - matrix2.M21) > Double.Epsilon
                || Math.Abs(matrix1.M31 - matrix2.M31) > Double.Epsilon
                || Math.Abs(matrix1.M41 - matrix2.M41) > Double.Epsilon
                || Math.Abs(matrix1.M51 - matrix2.M51) > Double.Epsilon
                || Math.Abs(matrix1.M61 - matrix2.M61) > Double.Epsilon
                || Math.Abs(matrix1.M71 - matrix2.M71) > Double.Epsilon
                || Math.Abs(matrix1.M81 - matrix2.M81) > Double.Epsilon
                || Math.Abs(matrix1.M12 - matrix2.M12) > Double.Epsilon
                || Math.Abs(matrix1.M22 - matrix2.M22) > Double.Epsilon
                || Math.Abs(matrix1.M32 - matrix2.M32) > Double.Epsilon
                || Math.Abs(matrix1.M42 - matrix2.M42) > Double.Epsilon
                || Math.Abs(matrix1.M52 - matrix2.M52) > Double.Epsilon
                || Math.Abs(matrix1.M62 - matrix2.M62) > Double.Epsilon
                || Math.Abs(matrix1.M72 - matrix2.M72) > Double.Epsilon
                || Math.Abs(matrix1.M82 - matrix2.M82) > Double.Epsilon
                || Math.Abs(matrix1.M13 - matrix2.M13) > Double.Epsilon
                || Math.Abs(matrix1.M23 - matrix2.M23) > Double.Epsilon
                || Math.Abs(matrix1.M33 - matrix2.M33) > Double.Epsilon
                || Math.Abs(matrix1.M43 - matrix2.M43) > Double.Epsilon
                || Math.Abs(matrix1.M53 - matrix2.M53) > Double.Epsilon
                || Math.Abs(matrix1.M63 - matrix2.M63) > Double.Epsilon
                || Math.Abs(matrix1.M73 - matrix2.M73) > Double.Epsilon
                || Math.Abs(matrix1.M83 - matrix2.M83) > Double.Epsilon
                || Math.Abs(matrix1.M14 - matrix2.M14) > Double.Epsilon
                || Math.Abs(matrix1.M24 - matrix2.M24) > Double.Epsilon
                || Math.Abs(matrix1.M34 - matrix2.M34) > Double.Epsilon
                || Math.Abs(matrix1.M44 - matrix2.M44) > Double.Epsilon
                || Math.Abs(matrix1.M54 - matrix2.M54) > Double.Epsilon
                || Math.Abs(matrix1.M64 - matrix2.M64) > Double.Epsilon
                || Math.Abs(matrix1.M74 - matrix2.M74) > Double.Epsilon
                || Math.Abs(matrix1.M84 - matrix2.M84) > Double.Epsilon
                || Math.Abs(matrix1.M15 - matrix2.M15) > Double.Epsilon
                || Math.Abs(matrix1.M25 - matrix2.M25) > Double.Epsilon
                || Math.Abs(matrix1.M35 - matrix2.M35) > Double.Epsilon
                || Math.Abs(matrix1.M45 - matrix2.M45) > Double.Epsilon
                || Math.Abs(matrix1.M55 - matrix2.M55) > Double.Epsilon
                || Math.Abs(matrix1.M65 - matrix2.M65) > Double.Epsilon
                || Math.Abs(matrix1.M75 - matrix2.M75) > Double.Epsilon
                || Math.Abs(matrix1.M85 - matrix2.M85) > Double.Epsilon
                || Math.Abs(matrix1.M16 - matrix2.M16) > Double.Epsilon
                || Math.Abs(matrix1.M26 - matrix2.M26) > Double.Epsilon
                || Math.Abs(matrix1.M36 - matrix2.M36) > Double.Epsilon
                || Math.Abs(matrix1.M46 - matrix2.M46) > Double.Epsilon
                || Math.Abs(matrix1.M56 - matrix2.M56) > Double.Epsilon
                || Math.Abs(matrix1.M66 - matrix2.M66) > Double.Epsilon
                || Math.Abs(matrix1.M76 - matrix2.M76) > Double.Epsilon
                || Math.Abs(matrix1.M86 - matrix2.M86) > Double.Epsilon
                || Math.Abs(matrix1.M17 - matrix2.M17) > Double.Epsilon
                || Math.Abs(matrix1.M27 - matrix2.M27) > Double.Epsilon
                || Math.Abs(matrix1.M37 - matrix2.M37) > Double.Epsilon
                || Math.Abs(matrix1.M47 - matrix2.M47) > Double.Epsilon
                || Math.Abs(matrix1.M57 - matrix2.M57) > Double.Epsilon
                || Math.Abs(matrix1.M67 - matrix2.M67) > Double.Epsilon
                || Math.Abs(matrix1.M77 - matrix2.M77) > Double.Epsilon
                || Math.Abs(matrix1.M87 - matrix2.M87) > Double.Epsilon;
        }

        public static Matrix8x7 operator +(Matrix8x7 matrix1, Matrix8x7 matrix2)
        {
            double m11 = matrix1.M11 + matrix2.M11;
            double m21 = matrix1.M21 + matrix2.M21;
            double m31 = matrix1.M31 + matrix2.M31;
            double m41 = matrix1.M41 + matrix2.M41;
            double m51 = matrix1.M51 + matrix2.M51;
            double m61 = matrix1.M61 + matrix2.M61;
            double m71 = matrix1.M71 + matrix2.M71;
            double m81 = matrix1.M81 + matrix2.M81;
            double m12 = matrix1.M12 + matrix2.M12;
            double m22 = matrix1.M22 + matrix2.M22;
            double m32 = matrix1.M32 + matrix2.M32;
            double m42 = matrix1.M42 + matrix2.M42;
            double m52 = matrix1.M52 + matrix2.M52;
            double m62 = matrix1.M62 + matrix2.M62;
            double m72 = matrix1.M72 + matrix2.M72;
            double m82 = matrix1.M82 + matrix2.M82;
            double m13 = matrix1.M13 + matrix2.M13;
            double m23 = matrix1.M23 + matrix2.M23;
            double m33 = matrix1.M33 + matrix2.M33;
            double m43 = matrix1.M43 + matrix2.M43;
            double m53 = matrix1.M53 + matrix2.M53;
            double m63 = matrix1.M63 + matrix2.M63;
            double m73 = matrix1.M73 + matrix2.M73;
            double m83 = matrix1.M83 + matrix2.M83;
            double m14 = matrix1.M14 + matrix2.M14;
            double m24 = matrix1.M24 + matrix2.M24;
            double m34 = matrix1.M34 + matrix2.M34;
            double m44 = matrix1.M44 + matrix2.M44;
            double m54 = matrix1.M54 + matrix2.M54;
            double m64 = matrix1.M64 + matrix2.M64;
            double m74 = matrix1.M74 + matrix2.M74;
            double m84 = matrix1.M84 + matrix2.M84;
            double m15 = matrix1.M15 + matrix2.M15;
            double m25 = matrix1.M25 + matrix2.M25;
            double m35 = matrix1.M35 + matrix2.M35;
            double m45 = matrix1.M45 + matrix2.M45;
            double m55 = matrix1.M55 + matrix2.M55;
            double m65 = matrix1.M65 + matrix2.M65;
            double m75 = matrix1.M75 + matrix2.M75;
            double m85 = matrix1.M85 + matrix2.M85;
            double m16 = matrix1.M16 + matrix2.M16;
            double m26 = matrix1.M26 + matrix2.M26;
            double m36 = matrix1.M36 + matrix2.M36;
            double m46 = matrix1.M46 + matrix2.M46;
            double m56 = matrix1.M56 + matrix2.M56;
            double m66 = matrix1.M66 + matrix2.M66;
            double m76 = matrix1.M76 + matrix2.M76;
            double m86 = matrix1.M86 + matrix2.M86;
            double m17 = matrix1.M17 + matrix2.M17;
            double m27 = matrix1.M27 + matrix2.M27;
            double m37 = matrix1.M37 + matrix2.M37;
            double m47 = matrix1.M47 + matrix2.M47;
            double m57 = matrix1.M57 + matrix2.M57;
            double m67 = matrix1.M67 + matrix2.M67;
            double m77 = matrix1.M77 + matrix2.M77;
            double m87 = matrix1.M87 + matrix2.M87;

            return new Matrix8x7(m11, m21, m31, m41, m51, m61, m71, m81, 
                                 m12, m22, m32, m42, m52, m62, m72, m82, 
                                 m13, m23, m33, m43, m53, m63, m73, m83, 
                                 m14, m24, m34, m44, m54, m64, m74, m84, 
                                 m15, m25, m35, m45, m55, m65, m75, m85, 
                                 m16, m26, m36, m46, m56, m66, m76, m86, 
                                 m17, m27, m37, m47, m57, m67, m77, m87);
        }

        public static Matrix8x7 operator -(Matrix8x7 matrix1, Matrix8x7 matrix2)
        {
            double m11 = matrix1.M11 - matrix2.M11;
            double m21 = matrix1.M21 - matrix2.M21;
            double m31 = matrix1.M31 - matrix2.M31;
            double m41 = matrix1.M41 - matrix2.M41;
            double m51 = matrix1.M51 - matrix2.M51;
            double m61 = matrix1.M61 - matrix2.M61;
            double m71 = matrix1.M71 - matrix2.M71;
            double m81 = matrix1.M81 - matrix2.M81;
            double m12 = matrix1.M12 - matrix2.M12;
            double m22 = matrix1.M22 - matrix2.M22;
            double m32 = matrix1.M32 - matrix2.M32;
            double m42 = matrix1.M42 - matrix2.M42;
            double m52 = matrix1.M52 - matrix2.M52;
            double m62 = matrix1.M62 - matrix2.M62;
            double m72 = matrix1.M72 - matrix2.M72;
            double m82 = matrix1.M82 - matrix2.M82;
            double m13 = matrix1.M13 - matrix2.M13;
            double m23 = matrix1.M23 - matrix2.M23;
            double m33 = matrix1.M33 - matrix2.M33;
            double m43 = matrix1.M43 - matrix2.M43;
            double m53 = matrix1.M53 - matrix2.M53;
            double m63 = matrix1.M63 - matrix2.M63;
            double m73 = matrix1.M73 - matrix2.M73;
            double m83 = matrix1.M83 - matrix2.M83;
            double m14 = matrix1.M14 - matrix2.M14;
            double m24 = matrix1.M24 - matrix2.M24;
            double m34 = matrix1.M34 - matrix2.M34;
            double m44 = matrix1.M44 - matrix2.M44;
            double m54 = matrix1.M54 - matrix2.M54;
            double m64 = matrix1.M64 - matrix2.M64;
            double m74 = matrix1.M74 - matrix2.M74;
            double m84 = matrix1.M84 - matrix2.M84;
            double m15 = matrix1.M15 - matrix2.M15;
            double m25 = matrix1.M25 - matrix2.M25;
            double m35 = matrix1.M35 - matrix2.M35;
            double m45 = matrix1.M45 - matrix2.M45;
            double m55 = matrix1.M55 - matrix2.M55;
            double m65 = matrix1.M65 - matrix2.M65;
            double m75 = matrix1.M75 - matrix2.M75;
            double m85 = matrix1.M85 - matrix2.M85;
            double m16 = matrix1.M16 - matrix2.M16;
            double m26 = matrix1.M26 - matrix2.M26;
            double m36 = matrix1.M36 - matrix2.M36;
            double m46 = matrix1.M46 - matrix2.M46;
            double m56 = matrix1.M56 - matrix2.M56;
            double m66 = matrix1.M66 - matrix2.M66;
            double m76 = matrix1.M76 - matrix2.M76;
            double m86 = matrix1.M86 - matrix2.M86;
            double m17 = matrix1.M17 - matrix2.M17;
            double m27 = matrix1.M27 - matrix2.M27;
            double m37 = matrix1.M37 - matrix2.M37;
            double m47 = matrix1.M47 - matrix2.M47;
            double m57 = matrix1.M57 - matrix2.M57;
            double m67 = matrix1.M67 - matrix2.M67;
            double m77 = matrix1.M77 - matrix2.M77;
            double m87 = matrix1.M87 - matrix2.M87;

            return new Matrix8x7(m11, m21, m31, m41, m51, m61, m71, m81, 
                                 m12, m22, m32, m42, m52, m62, m72, m82, 
                                 m13, m23, m33, m43, m53, m63, m73, m83, 
                                 m14, m24, m34, m44, m54, m64, m74, m84, 
                                 m15, m25, m35, m45, m55, m65, m75, m85, 
                                 m16, m26, m36, m46, m56, m66, m76, m86, 
                                 m17, m27, m37, m47, m57, m67, m77, m87);
        }

        public static Matrix8x7 operator *(Matrix8x7 matrix, double scalar)
        {
            double m11 = matrix.M11 * scalar;
            double m21 = matrix.M21 * scalar;
            double m31 = matrix.M31 * scalar;
            double m41 = matrix.M41 * scalar;
            double m51 = matrix.M51 * scalar;
            double m61 = matrix.M61 * scalar;
            double m71 = matrix.M71 * scalar;
            double m81 = matrix.M81 * scalar;
            double m12 = matrix.M12 * scalar;
            double m22 = matrix.M22 * scalar;
            double m32 = matrix.M32 * scalar;
            double m42 = matrix.M42 * scalar;
            double m52 = matrix.M52 * scalar;
            double m62 = matrix.M62 * scalar;
            double m72 = matrix.M72 * scalar;
            double m82 = matrix.M82 * scalar;
            double m13 = matrix.M13 * scalar;
            double m23 = matrix.M23 * scalar;
            double m33 = matrix.M33 * scalar;
            double m43 = matrix.M43 * scalar;
            double m53 = matrix.M53 * scalar;
            double m63 = matrix.M63 * scalar;
            double m73 = matrix.M73 * scalar;
            double m83 = matrix.M83 * scalar;
            double m14 = matrix.M14 * scalar;
            double m24 = matrix.M24 * scalar;
            double m34 = matrix.M34 * scalar;
            double m44 = matrix.M44 * scalar;
            double m54 = matrix.M54 * scalar;
            double m64 = matrix.M64 * scalar;
            double m74 = matrix.M74 * scalar;
            double m84 = matrix.M84 * scalar;
            double m15 = matrix.M15 * scalar;
            double m25 = matrix.M25 * scalar;
            double m35 = matrix.M35 * scalar;
            double m45 = matrix.M45 * scalar;
            double m55 = matrix.M55 * scalar;
            double m65 = matrix.M65 * scalar;
            double m75 = matrix.M75 * scalar;
            double m85 = matrix.M85 * scalar;
            double m16 = matrix.M16 * scalar;
            double m26 = matrix.M26 * scalar;
            double m36 = matrix.M36 * scalar;
            double m46 = matrix.M46 * scalar;
            double m56 = matrix.M56 * scalar;
            double m66 = matrix.M66 * scalar;
            double m76 = matrix.M76 * scalar;
            double m86 = matrix.M86 * scalar;
            double m17 = matrix.M17 * scalar;
            double m27 = matrix.M27 * scalar;
            double m37 = matrix.M37 * scalar;
            double m47 = matrix.M47 * scalar;
            double m57 = matrix.M57 * scalar;
            double m67 = matrix.M67 * scalar;
            double m77 = matrix.M77 * scalar;
            double m87 = matrix.M87 * scalar;

            return new Matrix8x7(m11, m21, m31, m41, m51, m61, m71, m81, 
                                 m12, m22, m32, m42, m52, m62, m72, m82, 
                                 m13, m23, m33, m43, m53, m63, m73, m83, 
                                 m14, m24, m34, m44, m54, m64, m74, m84, 
                                 m15, m25, m35, m45, m55, m65, m75, m85, 
                                 m16, m26, m36, m46, m56, m66, m76, m86, 
                                 m17, m27, m37, m47, m57, m67, m77, m87);
        }

        public static Matrix8x7 operator *(double scalar, Matrix8x7 matrix)
        {
            double m11 = scalar * matrix.M11;
            double m21 = scalar * matrix.M21;
            double m31 = scalar * matrix.M31;
            double m41 = scalar * matrix.M41;
            double m51 = scalar * matrix.M51;
            double m61 = scalar * matrix.M61;
            double m71 = scalar * matrix.M71;
            double m81 = scalar * matrix.M81;
            double m12 = scalar * matrix.M12;
            double m22 = scalar * matrix.M22;
            double m32 = scalar * matrix.M32;
            double m42 = scalar * matrix.M42;
            double m52 = scalar * matrix.M52;
            double m62 = scalar * matrix.M62;
            double m72 = scalar * matrix.M72;
            double m82 = scalar * matrix.M82;
            double m13 = scalar * matrix.M13;
            double m23 = scalar * matrix.M23;
            double m33 = scalar * matrix.M33;
            double m43 = scalar * matrix.M43;
            double m53 = scalar * matrix.M53;
            double m63 = scalar * matrix.M63;
            double m73 = scalar * matrix.M73;
            double m83 = scalar * matrix.M83;
            double m14 = scalar * matrix.M14;
            double m24 = scalar * matrix.M24;
            double m34 = scalar * matrix.M34;
            double m44 = scalar * matrix.M44;
            double m54 = scalar * matrix.M54;
            double m64 = scalar * matrix.M64;
            double m74 = scalar * matrix.M74;
            double m84 = scalar * matrix.M84;
            double m15 = scalar * matrix.M15;
            double m25 = scalar * matrix.M25;
            double m35 = scalar * matrix.M35;
            double m45 = scalar * matrix.M45;
            double m55 = scalar * matrix.M55;
            double m65 = scalar * matrix.M65;
            double m75 = scalar * matrix.M75;
            double m85 = scalar * matrix.M85;
            double m16 = scalar * matrix.M16;
            double m26 = scalar * matrix.M26;
            double m36 = scalar * matrix.M36;
            double m46 = scalar * matrix.M46;
            double m56 = scalar * matrix.M56;
            double m66 = scalar * matrix.M66;
            double m76 = scalar * matrix.M76;
            double m86 = scalar * matrix.M86;
            double m17 = scalar * matrix.M17;
            double m27 = scalar * matrix.M27;
            double m37 = scalar * matrix.M37;
            double m47 = scalar * matrix.M47;
            double m57 = scalar * matrix.M57;
            double m67 = scalar * matrix.M67;
            double m77 = scalar * matrix.M77;
            double m87 = scalar * matrix.M87;

            return new Matrix8x7(m11, m21, m31, m41, m51, m61, m71, m81, 
                                 m12, m22, m32, m42, m52, m62, m72, m82, 
                                 m13, m23, m33, m43, m53, m63, m73, m83, 
                                 m14, m24, m34, m44, m54, m64, m74, m84, 
                                 m15, m25, m35, m45, m55, m65, m75, m85, 
                                 m16, m26, m36, m46, m56, m66, m76, m86, 
                                 m17, m27, m37, m47, m57, m67, m77, m87);
        }

        public static Matrix1x7 operator *(Matrix8x7 matrix1, Matrix1x8 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12 + matrix1.M31 * matrix2.M13 + matrix1.M41 * matrix2.M14 + matrix1.M51 * matrix2.M15 + matrix1.M61 * matrix2.M16 + matrix1.M71 * matrix2.M17 + matrix1.M81 * matrix2.M18;
            double m12 = matrix1.M12 * matrix2.M11 + matrix1.M22 * matrix2.M12 + matrix1.M32 * matrix2.M13 + matrix1.M42 * matrix2.M14 + matrix1.M52 * matrix2.M15 + matrix1.M62 * matrix2.M16 + matrix1.M72 * matrix2.M17 + matrix1.M82 * matrix2.M18;
            double m13 = matrix1.M13 * matrix2.M11 + matrix1.M23 * matrix2.M12 + matrix1.M33 * matrix2.M13 + matrix1.M43 * matrix2.M14 + matrix1.M53 * matrix2.M15 + matrix1.M63 * matrix2.M16 + matrix1.M73 * matrix2.M17 + matrix1.M83 * matrix2.M18;
            double m14 = matrix1.M14 * matrix2.M11 + matrix1.M24 * matrix2.M12 + matrix1.M34 * matrix2.M13 + matrix1.M44 * matrix2.M14 + matrix1.M54 * matrix2.M15 + matrix1.M64 * matrix2.M16 + matrix1.M74 * matrix2.M17 + matrix1.M84 * matrix2.M18;
            double m15 = matrix1.M15 * matrix2.M11 + matrix1.M25 * matrix2.M12 + matrix1.M35 * matrix2.M13 + matrix1.M45 * matrix2.M14 + matrix1.M55 * matrix2.M15 + matrix1.M65 * matrix2.M16 + matrix1.M75 * matrix2.M17 + matrix1.M85 * matrix2.M18;
            double m16 = matrix1.M16 * matrix2.M11 + matrix1.M26 * matrix2.M12 + matrix1.M36 * matrix2.M13 + matrix1.M46 * matrix2.M14 + matrix1.M56 * matrix2.M15 + matrix1.M66 * matrix2.M16 + matrix1.M76 * matrix2.M17 + matrix1.M86 * matrix2.M18;
            double m17 = matrix1.M17 * matrix2.M11 + matrix1.M27 * matrix2.M12 + matrix1.M37 * matrix2.M13 + matrix1.M47 * matrix2.M14 + matrix1.M57 * matrix2.M15 + matrix1.M67 * matrix2.M16 + matrix1.M77 * matrix2.M17 + matrix1.M87 * matrix2.M18;

            return new Matrix1x7(m11, 
                                 m12, 
                                 m13, 
                                 m14, 
                                 m15, 
                                 m16, 
                                 m17);
        }
        public static Matrix2x7 operator *(Matrix8x7 matrix1, Matrix2x8 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12 + matrix1.M31 * matrix2.M13 + matrix1.M41 * matrix2.M14 + matrix1.M51 * matrix2.M15 + matrix1.M61 * matrix2.M16 + matrix1.M71 * matrix2.M17 + matrix1.M81 * matrix2.M18;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22 + matrix1.M31 * matrix2.M23 + matrix1.M41 * matrix2.M24 + matrix1.M51 * matrix2.M25 + matrix1.M61 * matrix2.M26 + matrix1.M71 * matrix2.M27 + matrix1.M81 * matrix2.M28;
            double m12 = matrix1.M12 * matrix2.M11 + matrix1.M22 * matrix2.M12 + matrix1.M32 * matrix2.M13 + matrix1.M42 * matrix2.M14 + matrix1.M52 * matrix2.M15 + matrix1.M62 * matrix2.M16 + matrix1.M72 * matrix2.M17 + matrix1.M82 * matrix2.M18;
            double m22 = matrix1.M12 * matrix2.M21 + matrix1.M22 * matrix2.M22 + matrix1.M32 * matrix2.M23 + matrix1.M42 * matrix2.M24 + matrix1.M52 * matrix2.M25 + matrix1.M62 * matrix2.M26 + matrix1.M72 * matrix2.M27 + matrix1.M82 * matrix2.M28;
            double m13 = matrix1.M13 * matrix2.M11 + matrix1.M23 * matrix2.M12 + matrix1.M33 * matrix2.M13 + matrix1.M43 * matrix2.M14 + matrix1.M53 * matrix2.M15 + matrix1.M63 * matrix2.M16 + matrix1.M73 * matrix2.M17 + matrix1.M83 * matrix2.M18;
            double m23 = matrix1.M13 * matrix2.M21 + matrix1.M23 * matrix2.M22 + matrix1.M33 * matrix2.M23 + matrix1.M43 * matrix2.M24 + matrix1.M53 * matrix2.M25 + matrix1.M63 * matrix2.M26 + matrix1.M73 * matrix2.M27 + matrix1.M83 * matrix2.M28;
            double m14 = matrix1.M14 * matrix2.M11 + matrix1.M24 * matrix2.M12 + matrix1.M34 * matrix2.M13 + matrix1.M44 * matrix2.M14 + matrix1.M54 * matrix2.M15 + matrix1.M64 * matrix2.M16 + matrix1.M74 * matrix2.M17 + matrix1.M84 * matrix2.M18;
            double m24 = matrix1.M14 * matrix2.M21 + matrix1.M24 * matrix2.M22 + matrix1.M34 * matrix2.M23 + matrix1.M44 * matrix2.M24 + matrix1.M54 * matrix2.M25 + matrix1.M64 * matrix2.M26 + matrix1.M74 * matrix2.M27 + matrix1.M84 * matrix2.M28;
            double m15 = matrix1.M15 * matrix2.M11 + matrix1.M25 * matrix2.M12 + matrix1.M35 * matrix2.M13 + matrix1.M45 * matrix2.M14 + matrix1.M55 * matrix2.M15 + matrix1.M65 * matrix2.M16 + matrix1.M75 * matrix2.M17 + matrix1.M85 * matrix2.M18;
            double m25 = matrix1.M15 * matrix2.M21 + matrix1.M25 * matrix2.M22 + matrix1.M35 * matrix2.M23 + matrix1.M45 * matrix2.M24 + matrix1.M55 * matrix2.M25 + matrix1.M65 * matrix2.M26 + matrix1.M75 * matrix2.M27 + matrix1.M85 * matrix2.M28;
            double m16 = matrix1.M16 * matrix2.M11 + matrix1.M26 * matrix2.M12 + matrix1.M36 * matrix2.M13 + matrix1.M46 * matrix2.M14 + matrix1.M56 * matrix2.M15 + matrix1.M66 * matrix2.M16 + matrix1.M76 * matrix2.M17 + matrix1.M86 * matrix2.M18;
            double m26 = matrix1.M16 * matrix2.M21 + matrix1.M26 * matrix2.M22 + matrix1.M36 * matrix2.M23 + matrix1.M46 * matrix2.M24 + matrix1.M56 * matrix2.M25 + matrix1.M66 * matrix2.M26 + matrix1.M76 * matrix2.M27 + matrix1.M86 * matrix2.M28;
            double m17 = matrix1.M17 * matrix2.M11 + matrix1.M27 * matrix2.M12 + matrix1.M37 * matrix2.M13 + matrix1.M47 * matrix2.M14 + matrix1.M57 * matrix2.M15 + matrix1.M67 * matrix2.M16 + matrix1.M77 * matrix2.M17 + matrix1.M87 * matrix2.M18;
            double m27 = matrix1.M17 * matrix2.M21 + matrix1.M27 * matrix2.M22 + matrix1.M37 * matrix2.M23 + matrix1.M47 * matrix2.M24 + matrix1.M57 * matrix2.M25 + matrix1.M67 * matrix2.M26 + matrix1.M77 * matrix2.M27 + matrix1.M87 * matrix2.M28;

            return new Matrix2x7(m11, m21, 
                                 m12, m22, 
                                 m13, m23, 
                                 m14, m24, 
                                 m15, m25, 
                                 m16, m26, 
                                 m17, m27);
        }
        public static Matrix3x7 operator *(Matrix8x7 matrix1, Matrix3x8 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12 + matrix1.M31 * matrix2.M13 + matrix1.M41 * matrix2.M14 + matrix1.M51 * matrix2.M15 + matrix1.M61 * matrix2.M16 + matrix1.M71 * matrix2.M17 + matrix1.M81 * matrix2.M18;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22 + matrix1.M31 * matrix2.M23 + matrix1.M41 * matrix2.M24 + matrix1.M51 * matrix2.M25 + matrix1.M61 * matrix2.M26 + matrix1.M71 * matrix2.M27 + matrix1.M81 * matrix2.M28;
            double m31 = matrix1.M11 * matrix2.M31 + matrix1.M21 * matrix2.M32 + matrix1.M31 * matrix2.M33 + matrix1.M41 * matrix2.M34 + matrix1.M51 * matrix2.M35 + matrix1.M61 * matrix2.M36 + matrix1.M71 * matrix2.M37 + matrix1.M81 * matrix2.M38;
            double m12 = matrix1.M12 * matrix2.M11 + matrix1.M22 * matrix2.M12 + matrix1.M32 * matrix2.M13 + matrix1.M42 * matrix2.M14 + matrix1.M52 * matrix2.M15 + matrix1.M62 * matrix2.M16 + matrix1.M72 * matrix2.M17 + matrix1.M82 * matrix2.M18;
            double m22 = matrix1.M12 * matrix2.M21 + matrix1.M22 * matrix2.M22 + matrix1.M32 * matrix2.M23 + matrix1.M42 * matrix2.M24 + matrix1.M52 * matrix2.M25 + matrix1.M62 * matrix2.M26 + matrix1.M72 * matrix2.M27 + matrix1.M82 * matrix2.M28;
            double m32 = matrix1.M12 * matrix2.M31 + matrix1.M22 * matrix2.M32 + matrix1.M32 * matrix2.M33 + matrix1.M42 * matrix2.M34 + matrix1.M52 * matrix2.M35 + matrix1.M62 * matrix2.M36 + matrix1.M72 * matrix2.M37 + matrix1.M82 * matrix2.M38;
            double m13 = matrix1.M13 * matrix2.M11 + matrix1.M23 * matrix2.M12 + matrix1.M33 * matrix2.M13 + matrix1.M43 * matrix2.M14 + matrix1.M53 * matrix2.M15 + matrix1.M63 * matrix2.M16 + matrix1.M73 * matrix2.M17 + matrix1.M83 * matrix2.M18;
            double m23 = matrix1.M13 * matrix2.M21 + matrix1.M23 * matrix2.M22 + matrix1.M33 * matrix2.M23 + matrix1.M43 * matrix2.M24 + matrix1.M53 * matrix2.M25 + matrix1.M63 * matrix2.M26 + matrix1.M73 * matrix2.M27 + matrix1.M83 * matrix2.M28;
            double m33 = matrix1.M13 * matrix2.M31 + matrix1.M23 * matrix2.M32 + matrix1.M33 * matrix2.M33 + matrix1.M43 * matrix2.M34 + matrix1.M53 * matrix2.M35 + matrix1.M63 * matrix2.M36 + matrix1.M73 * matrix2.M37 + matrix1.M83 * matrix2.M38;
            double m14 = matrix1.M14 * matrix2.M11 + matrix1.M24 * matrix2.M12 + matrix1.M34 * matrix2.M13 + matrix1.M44 * matrix2.M14 + matrix1.M54 * matrix2.M15 + matrix1.M64 * matrix2.M16 + matrix1.M74 * matrix2.M17 + matrix1.M84 * matrix2.M18;
            double m24 = matrix1.M14 * matrix2.M21 + matrix1.M24 * matrix2.M22 + matrix1.M34 * matrix2.M23 + matrix1.M44 * matrix2.M24 + matrix1.M54 * matrix2.M25 + matrix1.M64 * matrix2.M26 + matrix1.M74 * matrix2.M27 + matrix1.M84 * matrix2.M28;
            double m34 = matrix1.M14 * matrix2.M31 + matrix1.M24 * matrix2.M32 + matrix1.M34 * matrix2.M33 + matrix1.M44 * matrix2.M34 + matrix1.M54 * matrix2.M35 + matrix1.M64 * matrix2.M36 + matrix1.M74 * matrix2.M37 + matrix1.M84 * matrix2.M38;
            double m15 = matrix1.M15 * matrix2.M11 + matrix1.M25 * matrix2.M12 + matrix1.M35 * matrix2.M13 + matrix1.M45 * matrix2.M14 + matrix1.M55 * matrix2.M15 + matrix1.M65 * matrix2.M16 + matrix1.M75 * matrix2.M17 + matrix1.M85 * matrix2.M18;
            double m25 = matrix1.M15 * matrix2.M21 + matrix1.M25 * matrix2.M22 + matrix1.M35 * matrix2.M23 + matrix1.M45 * matrix2.M24 + matrix1.M55 * matrix2.M25 + matrix1.M65 * matrix2.M26 + matrix1.M75 * matrix2.M27 + matrix1.M85 * matrix2.M28;
            double m35 = matrix1.M15 * matrix2.M31 + matrix1.M25 * matrix2.M32 + matrix1.M35 * matrix2.M33 + matrix1.M45 * matrix2.M34 + matrix1.M55 * matrix2.M35 + matrix1.M65 * matrix2.M36 + matrix1.M75 * matrix2.M37 + matrix1.M85 * matrix2.M38;
            double m16 = matrix1.M16 * matrix2.M11 + matrix1.M26 * matrix2.M12 + matrix1.M36 * matrix2.M13 + matrix1.M46 * matrix2.M14 + matrix1.M56 * matrix2.M15 + matrix1.M66 * matrix2.M16 + matrix1.M76 * matrix2.M17 + matrix1.M86 * matrix2.M18;
            double m26 = matrix1.M16 * matrix2.M21 + matrix1.M26 * matrix2.M22 + matrix1.M36 * matrix2.M23 + matrix1.M46 * matrix2.M24 + matrix1.M56 * matrix2.M25 + matrix1.M66 * matrix2.M26 + matrix1.M76 * matrix2.M27 + matrix1.M86 * matrix2.M28;
            double m36 = matrix1.M16 * matrix2.M31 + matrix1.M26 * matrix2.M32 + matrix1.M36 * matrix2.M33 + matrix1.M46 * matrix2.M34 + matrix1.M56 * matrix2.M35 + matrix1.M66 * matrix2.M36 + matrix1.M76 * matrix2.M37 + matrix1.M86 * matrix2.M38;
            double m17 = matrix1.M17 * matrix2.M11 + matrix1.M27 * matrix2.M12 + matrix1.M37 * matrix2.M13 + matrix1.M47 * matrix2.M14 + matrix1.M57 * matrix2.M15 + matrix1.M67 * matrix2.M16 + matrix1.M77 * matrix2.M17 + matrix1.M87 * matrix2.M18;
            double m27 = matrix1.M17 * matrix2.M21 + matrix1.M27 * matrix2.M22 + matrix1.M37 * matrix2.M23 + matrix1.M47 * matrix2.M24 + matrix1.M57 * matrix2.M25 + matrix1.M67 * matrix2.M26 + matrix1.M77 * matrix2.M27 + matrix1.M87 * matrix2.M28;
            double m37 = matrix1.M17 * matrix2.M31 + matrix1.M27 * matrix2.M32 + matrix1.M37 * matrix2.M33 + matrix1.M47 * matrix2.M34 + matrix1.M57 * matrix2.M35 + matrix1.M67 * matrix2.M36 + matrix1.M77 * matrix2.M37 + matrix1.M87 * matrix2.M38;

            return new Matrix3x7(m11, m21, m31, 
                                 m12, m22, m32, 
                                 m13, m23, m33, 
                                 m14, m24, m34, 
                                 m15, m25, m35, 
                                 m16, m26, m36, 
                                 m17, m27, m37);
        }
        public static Matrix4x7 operator *(Matrix8x7 matrix1, Matrix4x8 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12 + matrix1.M31 * matrix2.M13 + matrix1.M41 * matrix2.M14 + matrix1.M51 * matrix2.M15 + matrix1.M61 * matrix2.M16 + matrix1.M71 * matrix2.M17 + matrix1.M81 * matrix2.M18;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22 + matrix1.M31 * matrix2.M23 + matrix1.M41 * matrix2.M24 + matrix1.M51 * matrix2.M25 + matrix1.M61 * matrix2.M26 + matrix1.M71 * matrix2.M27 + matrix1.M81 * matrix2.M28;
            double m31 = matrix1.M11 * matrix2.M31 + matrix1.M21 * matrix2.M32 + matrix1.M31 * matrix2.M33 + matrix1.M41 * matrix2.M34 + matrix1.M51 * matrix2.M35 + matrix1.M61 * matrix2.M36 + matrix1.M71 * matrix2.M37 + matrix1.M81 * matrix2.M38;
            double m41 = matrix1.M11 * matrix2.M41 + matrix1.M21 * matrix2.M42 + matrix1.M31 * matrix2.M43 + matrix1.M41 * matrix2.M44 + matrix1.M51 * matrix2.M45 + matrix1.M61 * matrix2.M46 + matrix1.M71 * matrix2.M47 + matrix1.M81 * matrix2.M48;
            double m12 = matrix1.M12 * matrix2.M11 + matrix1.M22 * matrix2.M12 + matrix1.M32 * matrix2.M13 + matrix1.M42 * matrix2.M14 + matrix1.M52 * matrix2.M15 + matrix1.M62 * matrix2.M16 + matrix1.M72 * matrix2.M17 + matrix1.M82 * matrix2.M18;
            double m22 = matrix1.M12 * matrix2.M21 + matrix1.M22 * matrix2.M22 + matrix1.M32 * matrix2.M23 + matrix1.M42 * matrix2.M24 + matrix1.M52 * matrix2.M25 + matrix1.M62 * matrix2.M26 + matrix1.M72 * matrix2.M27 + matrix1.M82 * matrix2.M28;
            double m32 = matrix1.M12 * matrix2.M31 + matrix1.M22 * matrix2.M32 + matrix1.M32 * matrix2.M33 + matrix1.M42 * matrix2.M34 + matrix1.M52 * matrix2.M35 + matrix1.M62 * matrix2.M36 + matrix1.M72 * matrix2.M37 + matrix1.M82 * matrix2.M38;
            double m42 = matrix1.M12 * matrix2.M41 + matrix1.M22 * matrix2.M42 + matrix1.M32 * matrix2.M43 + matrix1.M42 * matrix2.M44 + matrix1.M52 * matrix2.M45 + matrix1.M62 * matrix2.M46 + matrix1.M72 * matrix2.M47 + matrix1.M82 * matrix2.M48;
            double m13 = matrix1.M13 * matrix2.M11 + matrix1.M23 * matrix2.M12 + matrix1.M33 * matrix2.M13 + matrix1.M43 * matrix2.M14 + matrix1.M53 * matrix2.M15 + matrix1.M63 * matrix2.M16 + matrix1.M73 * matrix2.M17 + matrix1.M83 * matrix2.M18;
            double m23 = matrix1.M13 * matrix2.M21 + matrix1.M23 * matrix2.M22 + matrix1.M33 * matrix2.M23 + matrix1.M43 * matrix2.M24 + matrix1.M53 * matrix2.M25 + matrix1.M63 * matrix2.M26 + matrix1.M73 * matrix2.M27 + matrix1.M83 * matrix2.M28;
            double m33 = matrix1.M13 * matrix2.M31 + matrix1.M23 * matrix2.M32 + matrix1.M33 * matrix2.M33 + matrix1.M43 * matrix2.M34 + matrix1.M53 * matrix2.M35 + matrix1.M63 * matrix2.M36 + matrix1.M73 * matrix2.M37 + matrix1.M83 * matrix2.M38;
            double m43 = matrix1.M13 * matrix2.M41 + matrix1.M23 * matrix2.M42 + matrix1.M33 * matrix2.M43 + matrix1.M43 * matrix2.M44 + matrix1.M53 * matrix2.M45 + matrix1.M63 * matrix2.M46 + matrix1.M73 * matrix2.M47 + matrix1.M83 * matrix2.M48;
            double m14 = matrix1.M14 * matrix2.M11 + matrix1.M24 * matrix2.M12 + matrix1.M34 * matrix2.M13 + matrix1.M44 * matrix2.M14 + matrix1.M54 * matrix2.M15 + matrix1.M64 * matrix2.M16 + matrix1.M74 * matrix2.M17 + matrix1.M84 * matrix2.M18;
            double m24 = matrix1.M14 * matrix2.M21 + matrix1.M24 * matrix2.M22 + matrix1.M34 * matrix2.M23 + matrix1.M44 * matrix2.M24 + matrix1.M54 * matrix2.M25 + matrix1.M64 * matrix2.M26 + matrix1.M74 * matrix2.M27 + matrix1.M84 * matrix2.M28;
            double m34 = matrix1.M14 * matrix2.M31 + matrix1.M24 * matrix2.M32 + matrix1.M34 * matrix2.M33 + matrix1.M44 * matrix2.M34 + matrix1.M54 * matrix2.M35 + matrix1.M64 * matrix2.M36 + matrix1.M74 * matrix2.M37 + matrix1.M84 * matrix2.M38;
            double m44 = matrix1.M14 * matrix2.M41 + matrix1.M24 * matrix2.M42 + matrix1.M34 * matrix2.M43 + matrix1.M44 * matrix2.M44 + matrix1.M54 * matrix2.M45 + matrix1.M64 * matrix2.M46 + matrix1.M74 * matrix2.M47 + matrix1.M84 * matrix2.M48;
            double m15 = matrix1.M15 * matrix2.M11 + matrix1.M25 * matrix2.M12 + matrix1.M35 * matrix2.M13 + matrix1.M45 * matrix2.M14 + matrix1.M55 * matrix2.M15 + matrix1.M65 * matrix2.M16 + matrix1.M75 * matrix2.M17 + matrix1.M85 * matrix2.M18;
            double m25 = matrix1.M15 * matrix2.M21 + matrix1.M25 * matrix2.M22 + matrix1.M35 * matrix2.M23 + matrix1.M45 * matrix2.M24 + matrix1.M55 * matrix2.M25 + matrix1.M65 * matrix2.M26 + matrix1.M75 * matrix2.M27 + matrix1.M85 * matrix2.M28;
            double m35 = matrix1.M15 * matrix2.M31 + matrix1.M25 * matrix2.M32 + matrix1.M35 * matrix2.M33 + matrix1.M45 * matrix2.M34 + matrix1.M55 * matrix2.M35 + matrix1.M65 * matrix2.M36 + matrix1.M75 * matrix2.M37 + matrix1.M85 * matrix2.M38;
            double m45 = matrix1.M15 * matrix2.M41 + matrix1.M25 * matrix2.M42 + matrix1.M35 * matrix2.M43 + matrix1.M45 * matrix2.M44 + matrix1.M55 * matrix2.M45 + matrix1.M65 * matrix2.M46 + matrix1.M75 * matrix2.M47 + matrix1.M85 * matrix2.M48;
            double m16 = matrix1.M16 * matrix2.M11 + matrix1.M26 * matrix2.M12 + matrix1.M36 * matrix2.M13 + matrix1.M46 * matrix2.M14 + matrix1.M56 * matrix2.M15 + matrix1.M66 * matrix2.M16 + matrix1.M76 * matrix2.M17 + matrix1.M86 * matrix2.M18;
            double m26 = matrix1.M16 * matrix2.M21 + matrix1.M26 * matrix2.M22 + matrix1.M36 * matrix2.M23 + matrix1.M46 * matrix2.M24 + matrix1.M56 * matrix2.M25 + matrix1.M66 * matrix2.M26 + matrix1.M76 * matrix2.M27 + matrix1.M86 * matrix2.M28;
            double m36 = matrix1.M16 * matrix2.M31 + matrix1.M26 * matrix2.M32 + matrix1.M36 * matrix2.M33 + matrix1.M46 * matrix2.M34 + matrix1.M56 * matrix2.M35 + matrix1.M66 * matrix2.M36 + matrix1.M76 * matrix2.M37 + matrix1.M86 * matrix2.M38;
            double m46 = matrix1.M16 * matrix2.M41 + matrix1.M26 * matrix2.M42 + matrix1.M36 * matrix2.M43 + matrix1.M46 * matrix2.M44 + matrix1.M56 * matrix2.M45 + matrix1.M66 * matrix2.M46 + matrix1.M76 * matrix2.M47 + matrix1.M86 * matrix2.M48;
            double m17 = matrix1.M17 * matrix2.M11 + matrix1.M27 * matrix2.M12 + matrix1.M37 * matrix2.M13 + matrix1.M47 * matrix2.M14 + matrix1.M57 * matrix2.M15 + matrix1.M67 * matrix2.M16 + matrix1.M77 * matrix2.M17 + matrix1.M87 * matrix2.M18;
            double m27 = matrix1.M17 * matrix2.M21 + matrix1.M27 * matrix2.M22 + matrix1.M37 * matrix2.M23 + matrix1.M47 * matrix2.M24 + matrix1.M57 * matrix2.M25 + matrix1.M67 * matrix2.M26 + matrix1.M77 * matrix2.M27 + matrix1.M87 * matrix2.M28;
            double m37 = matrix1.M17 * matrix2.M31 + matrix1.M27 * matrix2.M32 + matrix1.M37 * matrix2.M33 + matrix1.M47 * matrix2.M34 + matrix1.M57 * matrix2.M35 + matrix1.M67 * matrix2.M36 + matrix1.M77 * matrix2.M37 + matrix1.M87 * matrix2.M38;
            double m47 = matrix1.M17 * matrix2.M41 + matrix1.M27 * matrix2.M42 + matrix1.M37 * matrix2.M43 + matrix1.M47 * matrix2.M44 + matrix1.M57 * matrix2.M45 + matrix1.M67 * matrix2.M46 + matrix1.M77 * matrix2.M47 + matrix1.M87 * matrix2.M48;

            return new Matrix4x7(m11, m21, m31, m41, 
                                 m12, m22, m32, m42, 
                                 m13, m23, m33, m43, 
                                 m14, m24, m34, m44, 
                                 m15, m25, m35, m45, 
                                 m16, m26, m36, m46, 
                                 m17, m27, m37, m47);
        }
        public static Matrix5x7 operator *(Matrix8x7 matrix1, Matrix5x8 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12 + matrix1.M31 * matrix2.M13 + matrix1.M41 * matrix2.M14 + matrix1.M51 * matrix2.M15 + matrix1.M61 * matrix2.M16 + matrix1.M71 * matrix2.M17 + matrix1.M81 * matrix2.M18;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22 + matrix1.M31 * matrix2.M23 + matrix1.M41 * matrix2.M24 + matrix1.M51 * matrix2.M25 + matrix1.M61 * matrix2.M26 + matrix1.M71 * matrix2.M27 + matrix1.M81 * matrix2.M28;
            double m31 = matrix1.M11 * matrix2.M31 + matrix1.M21 * matrix2.M32 + matrix1.M31 * matrix2.M33 + matrix1.M41 * matrix2.M34 + matrix1.M51 * matrix2.M35 + matrix1.M61 * matrix2.M36 + matrix1.M71 * matrix2.M37 + matrix1.M81 * matrix2.M38;
            double m41 = matrix1.M11 * matrix2.M41 + matrix1.M21 * matrix2.M42 + matrix1.M31 * matrix2.M43 + matrix1.M41 * matrix2.M44 + matrix1.M51 * matrix2.M45 + matrix1.M61 * matrix2.M46 + matrix1.M71 * matrix2.M47 + matrix1.M81 * matrix2.M48;
            double m51 = matrix1.M11 * matrix2.M51 + matrix1.M21 * matrix2.M52 + matrix1.M31 * matrix2.M53 + matrix1.M41 * matrix2.M54 + matrix1.M51 * matrix2.M55 + matrix1.M61 * matrix2.M56 + matrix1.M71 * matrix2.M57 + matrix1.M81 * matrix2.M58;
            double m12 = matrix1.M12 * matrix2.M11 + matrix1.M22 * matrix2.M12 + matrix1.M32 * matrix2.M13 + matrix1.M42 * matrix2.M14 + matrix1.M52 * matrix2.M15 + matrix1.M62 * matrix2.M16 + matrix1.M72 * matrix2.M17 + matrix1.M82 * matrix2.M18;
            double m22 = matrix1.M12 * matrix2.M21 + matrix1.M22 * matrix2.M22 + matrix1.M32 * matrix2.M23 + matrix1.M42 * matrix2.M24 + matrix1.M52 * matrix2.M25 + matrix1.M62 * matrix2.M26 + matrix1.M72 * matrix2.M27 + matrix1.M82 * matrix2.M28;
            double m32 = matrix1.M12 * matrix2.M31 + matrix1.M22 * matrix2.M32 + matrix1.M32 * matrix2.M33 + matrix1.M42 * matrix2.M34 + matrix1.M52 * matrix2.M35 + matrix1.M62 * matrix2.M36 + matrix1.M72 * matrix2.M37 + matrix1.M82 * matrix2.M38;
            double m42 = matrix1.M12 * matrix2.M41 + matrix1.M22 * matrix2.M42 + matrix1.M32 * matrix2.M43 + matrix1.M42 * matrix2.M44 + matrix1.M52 * matrix2.M45 + matrix1.M62 * matrix2.M46 + matrix1.M72 * matrix2.M47 + matrix1.M82 * matrix2.M48;
            double m52 = matrix1.M12 * matrix2.M51 + matrix1.M22 * matrix2.M52 + matrix1.M32 * matrix2.M53 + matrix1.M42 * matrix2.M54 + matrix1.M52 * matrix2.M55 + matrix1.M62 * matrix2.M56 + matrix1.M72 * matrix2.M57 + matrix1.M82 * matrix2.M58;
            double m13 = matrix1.M13 * matrix2.M11 + matrix1.M23 * matrix2.M12 + matrix1.M33 * matrix2.M13 + matrix1.M43 * matrix2.M14 + matrix1.M53 * matrix2.M15 + matrix1.M63 * matrix2.M16 + matrix1.M73 * matrix2.M17 + matrix1.M83 * matrix2.M18;
            double m23 = matrix1.M13 * matrix2.M21 + matrix1.M23 * matrix2.M22 + matrix1.M33 * matrix2.M23 + matrix1.M43 * matrix2.M24 + matrix1.M53 * matrix2.M25 + matrix1.M63 * matrix2.M26 + matrix1.M73 * matrix2.M27 + matrix1.M83 * matrix2.M28;
            double m33 = matrix1.M13 * matrix2.M31 + matrix1.M23 * matrix2.M32 + matrix1.M33 * matrix2.M33 + matrix1.M43 * matrix2.M34 + matrix1.M53 * matrix2.M35 + matrix1.M63 * matrix2.M36 + matrix1.M73 * matrix2.M37 + matrix1.M83 * matrix2.M38;
            double m43 = matrix1.M13 * matrix2.M41 + matrix1.M23 * matrix2.M42 + matrix1.M33 * matrix2.M43 + matrix1.M43 * matrix2.M44 + matrix1.M53 * matrix2.M45 + matrix1.M63 * matrix2.M46 + matrix1.M73 * matrix2.M47 + matrix1.M83 * matrix2.M48;
            double m53 = matrix1.M13 * matrix2.M51 + matrix1.M23 * matrix2.M52 + matrix1.M33 * matrix2.M53 + matrix1.M43 * matrix2.M54 + matrix1.M53 * matrix2.M55 + matrix1.M63 * matrix2.M56 + matrix1.M73 * matrix2.M57 + matrix1.M83 * matrix2.M58;
            double m14 = matrix1.M14 * matrix2.M11 + matrix1.M24 * matrix2.M12 + matrix1.M34 * matrix2.M13 + matrix1.M44 * matrix2.M14 + matrix1.M54 * matrix2.M15 + matrix1.M64 * matrix2.M16 + matrix1.M74 * matrix2.M17 + matrix1.M84 * matrix2.M18;
            double m24 = matrix1.M14 * matrix2.M21 + matrix1.M24 * matrix2.M22 + matrix1.M34 * matrix2.M23 + matrix1.M44 * matrix2.M24 + matrix1.M54 * matrix2.M25 + matrix1.M64 * matrix2.M26 + matrix1.M74 * matrix2.M27 + matrix1.M84 * matrix2.M28;
            double m34 = matrix1.M14 * matrix2.M31 + matrix1.M24 * matrix2.M32 + matrix1.M34 * matrix2.M33 + matrix1.M44 * matrix2.M34 + matrix1.M54 * matrix2.M35 + matrix1.M64 * matrix2.M36 + matrix1.M74 * matrix2.M37 + matrix1.M84 * matrix2.M38;
            double m44 = matrix1.M14 * matrix2.M41 + matrix1.M24 * matrix2.M42 + matrix1.M34 * matrix2.M43 + matrix1.M44 * matrix2.M44 + matrix1.M54 * matrix2.M45 + matrix1.M64 * matrix2.M46 + matrix1.M74 * matrix2.M47 + matrix1.M84 * matrix2.M48;
            double m54 = matrix1.M14 * matrix2.M51 + matrix1.M24 * matrix2.M52 + matrix1.M34 * matrix2.M53 + matrix1.M44 * matrix2.M54 + matrix1.M54 * matrix2.M55 + matrix1.M64 * matrix2.M56 + matrix1.M74 * matrix2.M57 + matrix1.M84 * matrix2.M58;
            double m15 = matrix1.M15 * matrix2.M11 + matrix1.M25 * matrix2.M12 + matrix1.M35 * matrix2.M13 + matrix1.M45 * matrix2.M14 + matrix1.M55 * matrix2.M15 + matrix1.M65 * matrix2.M16 + matrix1.M75 * matrix2.M17 + matrix1.M85 * matrix2.M18;
            double m25 = matrix1.M15 * matrix2.M21 + matrix1.M25 * matrix2.M22 + matrix1.M35 * matrix2.M23 + matrix1.M45 * matrix2.M24 + matrix1.M55 * matrix2.M25 + matrix1.M65 * matrix2.M26 + matrix1.M75 * matrix2.M27 + matrix1.M85 * matrix2.M28;
            double m35 = matrix1.M15 * matrix2.M31 + matrix1.M25 * matrix2.M32 + matrix1.M35 * matrix2.M33 + matrix1.M45 * matrix2.M34 + matrix1.M55 * matrix2.M35 + matrix1.M65 * matrix2.M36 + matrix1.M75 * matrix2.M37 + matrix1.M85 * matrix2.M38;
            double m45 = matrix1.M15 * matrix2.M41 + matrix1.M25 * matrix2.M42 + matrix1.M35 * matrix2.M43 + matrix1.M45 * matrix2.M44 + matrix1.M55 * matrix2.M45 + matrix1.M65 * matrix2.M46 + matrix1.M75 * matrix2.M47 + matrix1.M85 * matrix2.M48;
            double m55 = matrix1.M15 * matrix2.M51 + matrix1.M25 * matrix2.M52 + matrix1.M35 * matrix2.M53 + matrix1.M45 * matrix2.M54 + matrix1.M55 * matrix2.M55 + matrix1.M65 * matrix2.M56 + matrix1.M75 * matrix2.M57 + matrix1.M85 * matrix2.M58;
            double m16 = matrix1.M16 * matrix2.M11 + matrix1.M26 * matrix2.M12 + matrix1.M36 * matrix2.M13 + matrix1.M46 * matrix2.M14 + matrix1.M56 * matrix2.M15 + matrix1.M66 * matrix2.M16 + matrix1.M76 * matrix2.M17 + matrix1.M86 * matrix2.M18;
            double m26 = matrix1.M16 * matrix2.M21 + matrix1.M26 * matrix2.M22 + matrix1.M36 * matrix2.M23 + matrix1.M46 * matrix2.M24 + matrix1.M56 * matrix2.M25 + matrix1.M66 * matrix2.M26 + matrix1.M76 * matrix2.M27 + matrix1.M86 * matrix2.M28;
            double m36 = matrix1.M16 * matrix2.M31 + matrix1.M26 * matrix2.M32 + matrix1.M36 * matrix2.M33 + matrix1.M46 * matrix2.M34 + matrix1.M56 * matrix2.M35 + matrix1.M66 * matrix2.M36 + matrix1.M76 * matrix2.M37 + matrix1.M86 * matrix2.M38;
            double m46 = matrix1.M16 * matrix2.M41 + matrix1.M26 * matrix2.M42 + matrix1.M36 * matrix2.M43 + matrix1.M46 * matrix2.M44 + matrix1.M56 * matrix2.M45 + matrix1.M66 * matrix2.M46 + matrix1.M76 * matrix2.M47 + matrix1.M86 * matrix2.M48;
            double m56 = matrix1.M16 * matrix2.M51 + matrix1.M26 * matrix2.M52 + matrix1.M36 * matrix2.M53 + matrix1.M46 * matrix2.M54 + matrix1.M56 * matrix2.M55 + matrix1.M66 * matrix2.M56 + matrix1.M76 * matrix2.M57 + matrix1.M86 * matrix2.M58;
            double m17 = matrix1.M17 * matrix2.M11 + matrix1.M27 * matrix2.M12 + matrix1.M37 * matrix2.M13 + matrix1.M47 * matrix2.M14 + matrix1.M57 * matrix2.M15 + matrix1.M67 * matrix2.M16 + matrix1.M77 * matrix2.M17 + matrix1.M87 * matrix2.M18;
            double m27 = matrix1.M17 * matrix2.M21 + matrix1.M27 * matrix2.M22 + matrix1.M37 * matrix2.M23 + matrix1.M47 * matrix2.M24 + matrix1.M57 * matrix2.M25 + matrix1.M67 * matrix2.M26 + matrix1.M77 * matrix2.M27 + matrix1.M87 * matrix2.M28;
            double m37 = matrix1.M17 * matrix2.M31 + matrix1.M27 * matrix2.M32 + matrix1.M37 * matrix2.M33 + matrix1.M47 * matrix2.M34 + matrix1.M57 * matrix2.M35 + matrix1.M67 * matrix2.M36 + matrix1.M77 * matrix2.M37 + matrix1.M87 * matrix2.M38;
            double m47 = matrix1.M17 * matrix2.M41 + matrix1.M27 * matrix2.M42 + matrix1.M37 * matrix2.M43 + matrix1.M47 * matrix2.M44 + matrix1.M57 * matrix2.M45 + matrix1.M67 * matrix2.M46 + matrix1.M77 * matrix2.M47 + matrix1.M87 * matrix2.M48;
            double m57 = matrix1.M17 * matrix2.M51 + matrix1.M27 * matrix2.M52 + matrix1.M37 * matrix2.M53 + matrix1.M47 * matrix2.M54 + matrix1.M57 * matrix2.M55 + matrix1.M67 * matrix2.M56 + matrix1.M77 * matrix2.M57 + matrix1.M87 * matrix2.M58;

            return new Matrix5x7(m11, m21, m31, m41, m51, 
                                 m12, m22, m32, m42, m52, 
                                 m13, m23, m33, m43, m53, 
                                 m14, m24, m34, m44, m54, 
                                 m15, m25, m35, m45, m55, 
                                 m16, m26, m36, m46, m56, 
                                 m17, m27, m37, m47, m57);
        }
        public static Matrix6x7 operator *(Matrix8x7 matrix1, Matrix6x8 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12 + matrix1.M31 * matrix2.M13 + matrix1.M41 * matrix2.M14 + matrix1.M51 * matrix2.M15 + matrix1.M61 * matrix2.M16 + matrix1.M71 * matrix2.M17 + matrix1.M81 * matrix2.M18;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22 + matrix1.M31 * matrix2.M23 + matrix1.M41 * matrix2.M24 + matrix1.M51 * matrix2.M25 + matrix1.M61 * matrix2.M26 + matrix1.M71 * matrix2.M27 + matrix1.M81 * matrix2.M28;
            double m31 = matrix1.M11 * matrix2.M31 + matrix1.M21 * matrix2.M32 + matrix1.M31 * matrix2.M33 + matrix1.M41 * matrix2.M34 + matrix1.M51 * matrix2.M35 + matrix1.M61 * matrix2.M36 + matrix1.M71 * matrix2.M37 + matrix1.M81 * matrix2.M38;
            double m41 = matrix1.M11 * matrix2.M41 + matrix1.M21 * matrix2.M42 + matrix1.M31 * matrix2.M43 + matrix1.M41 * matrix2.M44 + matrix1.M51 * matrix2.M45 + matrix1.M61 * matrix2.M46 + matrix1.M71 * matrix2.M47 + matrix1.M81 * matrix2.M48;
            double m51 = matrix1.M11 * matrix2.M51 + matrix1.M21 * matrix2.M52 + matrix1.M31 * matrix2.M53 + matrix1.M41 * matrix2.M54 + matrix1.M51 * matrix2.M55 + matrix1.M61 * matrix2.M56 + matrix1.M71 * matrix2.M57 + matrix1.M81 * matrix2.M58;
            double m61 = matrix1.M11 * matrix2.M61 + matrix1.M21 * matrix2.M62 + matrix1.M31 * matrix2.M63 + matrix1.M41 * matrix2.M64 + matrix1.M51 * matrix2.M65 + matrix1.M61 * matrix2.M66 + matrix1.M71 * matrix2.M67 + matrix1.M81 * matrix2.M68;
            double m12 = matrix1.M12 * matrix2.M11 + matrix1.M22 * matrix2.M12 + matrix1.M32 * matrix2.M13 + matrix1.M42 * matrix2.M14 + matrix1.M52 * matrix2.M15 + matrix1.M62 * matrix2.M16 + matrix1.M72 * matrix2.M17 + matrix1.M82 * matrix2.M18;
            double m22 = matrix1.M12 * matrix2.M21 + matrix1.M22 * matrix2.M22 + matrix1.M32 * matrix2.M23 + matrix1.M42 * matrix2.M24 + matrix1.M52 * matrix2.M25 + matrix1.M62 * matrix2.M26 + matrix1.M72 * matrix2.M27 + matrix1.M82 * matrix2.M28;
            double m32 = matrix1.M12 * matrix2.M31 + matrix1.M22 * matrix2.M32 + matrix1.M32 * matrix2.M33 + matrix1.M42 * matrix2.M34 + matrix1.M52 * matrix2.M35 + matrix1.M62 * matrix2.M36 + matrix1.M72 * matrix2.M37 + matrix1.M82 * matrix2.M38;
            double m42 = matrix1.M12 * matrix2.M41 + matrix1.M22 * matrix2.M42 + matrix1.M32 * matrix2.M43 + matrix1.M42 * matrix2.M44 + matrix1.M52 * matrix2.M45 + matrix1.M62 * matrix2.M46 + matrix1.M72 * matrix2.M47 + matrix1.M82 * matrix2.M48;
            double m52 = matrix1.M12 * matrix2.M51 + matrix1.M22 * matrix2.M52 + matrix1.M32 * matrix2.M53 + matrix1.M42 * matrix2.M54 + matrix1.M52 * matrix2.M55 + matrix1.M62 * matrix2.M56 + matrix1.M72 * matrix2.M57 + matrix1.M82 * matrix2.M58;
            double m62 = matrix1.M12 * matrix2.M61 + matrix1.M22 * matrix2.M62 + matrix1.M32 * matrix2.M63 + matrix1.M42 * matrix2.M64 + matrix1.M52 * matrix2.M65 + matrix1.M62 * matrix2.M66 + matrix1.M72 * matrix2.M67 + matrix1.M82 * matrix2.M68;
            double m13 = matrix1.M13 * matrix2.M11 + matrix1.M23 * matrix2.M12 + matrix1.M33 * matrix2.M13 + matrix1.M43 * matrix2.M14 + matrix1.M53 * matrix2.M15 + matrix1.M63 * matrix2.M16 + matrix1.M73 * matrix2.M17 + matrix1.M83 * matrix2.M18;
            double m23 = matrix1.M13 * matrix2.M21 + matrix1.M23 * matrix2.M22 + matrix1.M33 * matrix2.M23 + matrix1.M43 * matrix2.M24 + matrix1.M53 * matrix2.M25 + matrix1.M63 * matrix2.M26 + matrix1.M73 * matrix2.M27 + matrix1.M83 * matrix2.M28;
            double m33 = matrix1.M13 * matrix2.M31 + matrix1.M23 * matrix2.M32 + matrix1.M33 * matrix2.M33 + matrix1.M43 * matrix2.M34 + matrix1.M53 * matrix2.M35 + matrix1.M63 * matrix2.M36 + matrix1.M73 * matrix2.M37 + matrix1.M83 * matrix2.M38;
            double m43 = matrix1.M13 * matrix2.M41 + matrix1.M23 * matrix2.M42 + matrix1.M33 * matrix2.M43 + matrix1.M43 * matrix2.M44 + matrix1.M53 * matrix2.M45 + matrix1.M63 * matrix2.M46 + matrix1.M73 * matrix2.M47 + matrix1.M83 * matrix2.M48;
            double m53 = matrix1.M13 * matrix2.M51 + matrix1.M23 * matrix2.M52 + matrix1.M33 * matrix2.M53 + matrix1.M43 * matrix2.M54 + matrix1.M53 * matrix2.M55 + matrix1.M63 * matrix2.M56 + matrix1.M73 * matrix2.M57 + matrix1.M83 * matrix2.M58;
            double m63 = matrix1.M13 * matrix2.M61 + matrix1.M23 * matrix2.M62 + matrix1.M33 * matrix2.M63 + matrix1.M43 * matrix2.M64 + matrix1.M53 * matrix2.M65 + matrix1.M63 * matrix2.M66 + matrix1.M73 * matrix2.M67 + matrix1.M83 * matrix2.M68;
            double m14 = matrix1.M14 * matrix2.M11 + matrix1.M24 * matrix2.M12 + matrix1.M34 * matrix2.M13 + matrix1.M44 * matrix2.M14 + matrix1.M54 * matrix2.M15 + matrix1.M64 * matrix2.M16 + matrix1.M74 * matrix2.M17 + matrix1.M84 * matrix2.M18;
            double m24 = matrix1.M14 * matrix2.M21 + matrix1.M24 * matrix2.M22 + matrix1.M34 * matrix2.M23 + matrix1.M44 * matrix2.M24 + matrix1.M54 * matrix2.M25 + matrix1.M64 * matrix2.M26 + matrix1.M74 * matrix2.M27 + matrix1.M84 * matrix2.M28;
            double m34 = matrix1.M14 * matrix2.M31 + matrix1.M24 * matrix2.M32 + matrix1.M34 * matrix2.M33 + matrix1.M44 * matrix2.M34 + matrix1.M54 * matrix2.M35 + matrix1.M64 * matrix2.M36 + matrix1.M74 * matrix2.M37 + matrix1.M84 * matrix2.M38;
            double m44 = matrix1.M14 * matrix2.M41 + matrix1.M24 * matrix2.M42 + matrix1.M34 * matrix2.M43 + matrix1.M44 * matrix2.M44 + matrix1.M54 * matrix2.M45 + matrix1.M64 * matrix2.M46 + matrix1.M74 * matrix2.M47 + matrix1.M84 * matrix2.M48;
            double m54 = matrix1.M14 * matrix2.M51 + matrix1.M24 * matrix2.M52 + matrix1.M34 * matrix2.M53 + matrix1.M44 * matrix2.M54 + matrix1.M54 * matrix2.M55 + matrix1.M64 * matrix2.M56 + matrix1.M74 * matrix2.M57 + matrix1.M84 * matrix2.M58;
            double m64 = matrix1.M14 * matrix2.M61 + matrix1.M24 * matrix2.M62 + matrix1.M34 * matrix2.M63 + matrix1.M44 * matrix2.M64 + matrix1.M54 * matrix2.M65 + matrix1.M64 * matrix2.M66 + matrix1.M74 * matrix2.M67 + matrix1.M84 * matrix2.M68;
            double m15 = matrix1.M15 * matrix2.M11 + matrix1.M25 * matrix2.M12 + matrix1.M35 * matrix2.M13 + matrix1.M45 * matrix2.M14 + matrix1.M55 * matrix2.M15 + matrix1.M65 * matrix2.M16 + matrix1.M75 * matrix2.M17 + matrix1.M85 * matrix2.M18;
            double m25 = matrix1.M15 * matrix2.M21 + matrix1.M25 * matrix2.M22 + matrix1.M35 * matrix2.M23 + matrix1.M45 * matrix2.M24 + matrix1.M55 * matrix2.M25 + matrix1.M65 * matrix2.M26 + matrix1.M75 * matrix2.M27 + matrix1.M85 * matrix2.M28;
            double m35 = matrix1.M15 * matrix2.M31 + matrix1.M25 * matrix2.M32 + matrix1.M35 * matrix2.M33 + matrix1.M45 * matrix2.M34 + matrix1.M55 * matrix2.M35 + matrix1.M65 * matrix2.M36 + matrix1.M75 * matrix2.M37 + matrix1.M85 * matrix2.M38;
            double m45 = matrix1.M15 * matrix2.M41 + matrix1.M25 * matrix2.M42 + matrix1.M35 * matrix2.M43 + matrix1.M45 * matrix2.M44 + matrix1.M55 * matrix2.M45 + matrix1.M65 * matrix2.M46 + matrix1.M75 * matrix2.M47 + matrix1.M85 * matrix2.M48;
            double m55 = matrix1.M15 * matrix2.M51 + matrix1.M25 * matrix2.M52 + matrix1.M35 * matrix2.M53 + matrix1.M45 * matrix2.M54 + matrix1.M55 * matrix2.M55 + matrix1.M65 * matrix2.M56 + matrix1.M75 * matrix2.M57 + matrix1.M85 * matrix2.M58;
            double m65 = matrix1.M15 * matrix2.M61 + matrix1.M25 * matrix2.M62 + matrix1.M35 * matrix2.M63 + matrix1.M45 * matrix2.M64 + matrix1.M55 * matrix2.M65 + matrix1.M65 * matrix2.M66 + matrix1.M75 * matrix2.M67 + matrix1.M85 * matrix2.M68;
            double m16 = matrix1.M16 * matrix2.M11 + matrix1.M26 * matrix2.M12 + matrix1.M36 * matrix2.M13 + matrix1.M46 * matrix2.M14 + matrix1.M56 * matrix2.M15 + matrix1.M66 * matrix2.M16 + matrix1.M76 * matrix2.M17 + matrix1.M86 * matrix2.M18;
            double m26 = matrix1.M16 * matrix2.M21 + matrix1.M26 * matrix2.M22 + matrix1.M36 * matrix2.M23 + matrix1.M46 * matrix2.M24 + matrix1.M56 * matrix2.M25 + matrix1.M66 * matrix2.M26 + matrix1.M76 * matrix2.M27 + matrix1.M86 * matrix2.M28;
            double m36 = matrix1.M16 * matrix2.M31 + matrix1.M26 * matrix2.M32 + matrix1.M36 * matrix2.M33 + matrix1.M46 * matrix2.M34 + matrix1.M56 * matrix2.M35 + matrix1.M66 * matrix2.M36 + matrix1.M76 * matrix2.M37 + matrix1.M86 * matrix2.M38;
            double m46 = matrix1.M16 * matrix2.M41 + matrix1.M26 * matrix2.M42 + matrix1.M36 * matrix2.M43 + matrix1.M46 * matrix2.M44 + matrix1.M56 * matrix2.M45 + matrix1.M66 * matrix2.M46 + matrix1.M76 * matrix2.M47 + matrix1.M86 * matrix2.M48;
            double m56 = matrix1.M16 * matrix2.M51 + matrix1.M26 * matrix2.M52 + matrix1.M36 * matrix2.M53 + matrix1.M46 * matrix2.M54 + matrix1.M56 * matrix2.M55 + matrix1.M66 * matrix2.M56 + matrix1.M76 * matrix2.M57 + matrix1.M86 * matrix2.M58;
            double m66 = matrix1.M16 * matrix2.M61 + matrix1.M26 * matrix2.M62 + matrix1.M36 * matrix2.M63 + matrix1.M46 * matrix2.M64 + matrix1.M56 * matrix2.M65 + matrix1.M66 * matrix2.M66 + matrix1.M76 * matrix2.M67 + matrix1.M86 * matrix2.M68;
            double m17 = matrix1.M17 * matrix2.M11 + matrix1.M27 * matrix2.M12 + matrix1.M37 * matrix2.M13 + matrix1.M47 * matrix2.M14 + matrix1.M57 * matrix2.M15 + matrix1.M67 * matrix2.M16 + matrix1.M77 * matrix2.M17 + matrix1.M87 * matrix2.M18;
            double m27 = matrix1.M17 * matrix2.M21 + matrix1.M27 * matrix2.M22 + matrix1.M37 * matrix2.M23 + matrix1.M47 * matrix2.M24 + matrix1.M57 * matrix2.M25 + matrix1.M67 * matrix2.M26 + matrix1.M77 * matrix2.M27 + matrix1.M87 * matrix2.M28;
            double m37 = matrix1.M17 * matrix2.M31 + matrix1.M27 * matrix2.M32 + matrix1.M37 * matrix2.M33 + matrix1.M47 * matrix2.M34 + matrix1.M57 * matrix2.M35 + matrix1.M67 * matrix2.M36 + matrix1.M77 * matrix2.M37 + matrix1.M87 * matrix2.M38;
            double m47 = matrix1.M17 * matrix2.M41 + matrix1.M27 * matrix2.M42 + matrix1.M37 * matrix2.M43 + matrix1.M47 * matrix2.M44 + matrix1.M57 * matrix2.M45 + matrix1.M67 * matrix2.M46 + matrix1.M77 * matrix2.M47 + matrix1.M87 * matrix2.M48;
            double m57 = matrix1.M17 * matrix2.M51 + matrix1.M27 * matrix2.M52 + matrix1.M37 * matrix2.M53 + matrix1.M47 * matrix2.M54 + matrix1.M57 * matrix2.M55 + matrix1.M67 * matrix2.M56 + matrix1.M77 * matrix2.M57 + matrix1.M87 * matrix2.M58;
            double m67 = matrix1.M17 * matrix2.M61 + matrix1.M27 * matrix2.M62 + matrix1.M37 * matrix2.M63 + matrix1.M47 * matrix2.M64 + matrix1.M57 * matrix2.M65 + matrix1.M67 * matrix2.M66 + matrix1.M77 * matrix2.M67 + matrix1.M87 * matrix2.M68;

            return new Matrix6x7(m11, m21, m31, m41, m51, m61, 
                                 m12, m22, m32, m42, m52, m62, 
                                 m13, m23, m33, m43, m53, m63, 
                                 m14, m24, m34, m44, m54, m64, 
                                 m15, m25, m35, m45, m55, m65, 
                                 m16, m26, m36, m46, m56, m66, 
                                 m17, m27, m37, m47, m57, m67);
        }
        public static Matrix7x7 operator *(Matrix8x7 matrix1, Matrix7x8 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12 + matrix1.M31 * matrix2.M13 + matrix1.M41 * matrix2.M14 + matrix1.M51 * matrix2.M15 + matrix1.M61 * matrix2.M16 + matrix1.M71 * matrix2.M17 + matrix1.M81 * matrix2.M18;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22 + matrix1.M31 * matrix2.M23 + matrix1.M41 * matrix2.M24 + matrix1.M51 * matrix2.M25 + matrix1.M61 * matrix2.M26 + matrix1.M71 * matrix2.M27 + matrix1.M81 * matrix2.M28;
            double m31 = matrix1.M11 * matrix2.M31 + matrix1.M21 * matrix2.M32 + matrix1.M31 * matrix2.M33 + matrix1.M41 * matrix2.M34 + matrix1.M51 * matrix2.M35 + matrix1.M61 * matrix2.M36 + matrix1.M71 * matrix2.M37 + matrix1.M81 * matrix2.M38;
            double m41 = matrix1.M11 * matrix2.M41 + matrix1.M21 * matrix2.M42 + matrix1.M31 * matrix2.M43 + matrix1.M41 * matrix2.M44 + matrix1.M51 * matrix2.M45 + matrix1.M61 * matrix2.M46 + matrix1.M71 * matrix2.M47 + matrix1.M81 * matrix2.M48;
            double m51 = matrix1.M11 * matrix2.M51 + matrix1.M21 * matrix2.M52 + matrix1.M31 * matrix2.M53 + matrix1.M41 * matrix2.M54 + matrix1.M51 * matrix2.M55 + matrix1.M61 * matrix2.M56 + matrix1.M71 * matrix2.M57 + matrix1.M81 * matrix2.M58;
            double m61 = matrix1.M11 * matrix2.M61 + matrix1.M21 * matrix2.M62 + matrix1.M31 * matrix2.M63 + matrix1.M41 * matrix2.M64 + matrix1.M51 * matrix2.M65 + matrix1.M61 * matrix2.M66 + matrix1.M71 * matrix2.M67 + matrix1.M81 * matrix2.M68;
            double m71 = matrix1.M11 * matrix2.M71 + matrix1.M21 * matrix2.M72 + matrix1.M31 * matrix2.M73 + matrix1.M41 * matrix2.M74 + matrix1.M51 * matrix2.M75 + matrix1.M61 * matrix2.M76 + matrix1.M71 * matrix2.M77 + matrix1.M81 * matrix2.M78;
            double m12 = matrix1.M12 * matrix2.M11 + matrix1.M22 * matrix2.M12 + matrix1.M32 * matrix2.M13 + matrix1.M42 * matrix2.M14 + matrix1.M52 * matrix2.M15 + matrix1.M62 * matrix2.M16 + matrix1.M72 * matrix2.M17 + matrix1.M82 * matrix2.M18;
            double m22 = matrix1.M12 * matrix2.M21 + matrix1.M22 * matrix2.M22 + matrix1.M32 * matrix2.M23 + matrix1.M42 * matrix2.M24 + matrix1.M52 * matrix2.M25 + matrix1.M62 * matrix2.M26 + matrix1.M72 * matrix2.M27 + matrix1.M82 * matrix2.M28;
            double m32 = matrix1.M12 * matrix2.M31 + matrix1.M22 * matrix2.M32 + matrix1.M32 * matrix2.M33 + matrix1.M42 * matrix2.M34 + matrix1.M52 * matrix2.M35 + matrix1.M62 * matrix2.M36 + matrix1.M72 * matrix2.M37 + matrix1.M82 * matrix2.M38;
            double m42 = matrix1.M12 * matrix2.M41 + matrix1.M22 * matrix2.M42 + matrix1.M32 * matrix2.M43 + matrix1.M42 * matrix2.M44 + matrix1.M52 * matrix2.M45 + matrix1.M62 * matrix2.M46 + matrix1.M72 * matrix2.M47 + matrix1.M82 * matrix2.M48;
            double m52 = matrix1.M12 * matrix2.M51 + matrix1.M22 * matrix2.M52 + matrix1.M32 * matrix2.M53 + matrix1.M42 * matrix2.M54 + matrix1.M52 * matrix2.M55 + matrix1.M62 * matrix2.M56 + matrix1.M72 * matrix2.M57 + matrix1.M82 * matrix2.M58;
            double m62 = matrix1.M12 * matrix2.M61 + matrix1.M22 * matrix2.M62 + matrix1.M32 * matrix2.M63 + matrix1.M42 * matrix2.M64 + matrix1.M52 * matrix2.M65 + matrix1.M62 * matrix2.M66 + matrix1.M72 * matrix2.M67 + matrix1.M82 * matrix2.M68;
            double m72 = matrix1.M12 * matrix2.M71 + matrix1.M22 * matrix2.M72 + matrix1.M32 * matrix2.M73 + matrix1.M42 * matrix2.M74 + matrix1.M52 * matrix2.M75 + matrix1.M62 * matrix2.M76 + matrix1.M72 * matrix2.M77 + matrix1.M82 * matrix2.M78;
            double m13 = matrix1.M13 * matrix2.M11 + matrix1.M23 * matrix2.M12 + matrix1.M33 * matrix2.M13 + matrix1.M43 * matrix2.M14 + matrix1.M53 * matrix2.M15 + matrix1.M63 * matrix2.M16 + matrix1.M73 * matrix2.M17 + matrix1.M83 * matrix2.M18;
            double m23 = matrix1.M13 * matrix2.M21 + matrix1.M23 * matrix2.M22 + matrix1.M33 * matrix2.M23 + matrix1.M43 * matrix2.M24 + matrix1.M53 * matrix2.M25 + matrix1.M63 * matrix2.M26 + matrix1.M73 * matrix2.M27 + matrix1.M83 * matrix2.M28;
            double m33 = matrix1.M13 * matrix2.M31 + matrix1.M23 * matrix2.M32 + matrix1.M33 * matrix2.M33 + matrix1.M43 * matrix2.M34 + matrix1.M53 * matrix2.M35 + matrix1.M63 * matrix2.M36 + matrix1.M73 * matrix2.M37 + matrix1.M83 * matrix2.M38;
            double m43 = matrix1.M13 * matrix2.M41 + matrix1.M23 * matrix2.M42 + matrix1.M33 * matrix2.M43 + matrix1.M43 * matrix2.M44 + matrix1.M53 * matrix2.M45 + matrix1.M63 * matrix2.M46 + matrix1.M73 * matrix2.M47 + matrix1.M83 * matrix2.M48;
            double m53 = matrix1.M13 * matrix2.M51 + matrix1.M23 * matrix2.M52 + matrix1.M33 * matrix2.M53 + matrix1.M43 * matrix2.M54 + matrix1.M53 * matrix2.M55 + matrix1.M63 * matrix2.M56 + matrix1.M73 * matrix2.M57 + matrix1.M83 * matrix2.M58;
            double m63 = matrix1.M13 * matrix2.M61 + matrix1.M23 * matrix2.M62 + matrix1.M33 * matrix2.M63 + matrix1.M43 * matrix2.M64 + matrix1.M53 * matrix2.M65 + matrix1.M63 * matrix2.M66 + matrix1.M73 * matrix2.M67 + matrix1.M83 * matrix2.M68;
            double m73 = matrix1.M13 * matrix2.M71 + matrix1.M23 * matrix2.M72 + matrix1.M33 * matrix2.M73 + matrix1.M43 * matrix2.M74 + matrix1.M53 * matrix2.M75 + matrix1.M63 * matrix2.M76 + matrix1.M73 * matrix2.M77 + matrix1.M83 * matrix2.M78;
            double m14 = matrix1.M14 * matrix2.M11 + matrix1.M24 * matrix2.M12 + matrix1.M34 * matrix2.M13 + matrix1.M44 * matrix2.M14 + matrix1.M54 * matrix2.M15 + matrix1.M64 * matrix2.M16 + matrix1.M74 * matrix2.M17 + matrix1.M84 * matrix2.M18;
            double m24 = matrix1.M14 * matrix2.M21 + matrix1.M24 * matrix2.M22 + matrix1.M34 * matrix2.M23 + matrix1.M44 * matrix2.M24 + matrix1.M54 * matrix2.M25 + matrix1.M64 * matrix2.M26 + matrix1.M74 * matrix2.M27 + matrix1.M84 * matrix2.M28;
            double m34 = matrix1.M14 * matrix2.M31 + matrix1.M24 * matrix2.M32 + matrix1.M34 * matrix2.M33 + matrix1.M44 * matrix2.M34 + matrix1.M54 * matrix2.M35 + matrix1.M64 * matrix2.M36 + matrix1.M74 * matrix2.M37 + matrix1.M84 * matrix2.M38;
            double m44 = matrix1.M14 * matrix2.M41 + matrix1.M24 * matrix2.M42 + matrix1.M34 * matrix2.M43 + matrix1.M44 * matrix2.M44 + matrix1.M54 * matrix2.M45 + matrix1.M64 * matrix2.M46 + matrix1.M74 * matrix2.M47 + matrix1.M84 * matrix2.M48;
            double m54 = matrix1.M14 * matrix2.M51 + matrix1.M24 * matrix2.M52 + matrix1.M34 * matrix2.M53 + matrix1.M44 * matrix2.M54 + matrix1.M54 * matrix2.M55 + matrix1.M64 * matrix2.M56 + matrix1.M74 * matrix2.M57 + matrix1.M84 * matrix2.M58;
            double m64 = matrix1.M14 * matrix2.M61 + matrix1.M24 * matrix2.M62 + matrix1.M34 * matrix2.M63 + matrix1.M44 * matrix2.M64 + matrix1.M54 * matrix2.M65 + matrix1.M64 * matrix2.M66 + matrix1.M74 * matrix2.M67 + matrix1.M84 * matrix2.M68;
            double m74 = matrix1.M14 * matrix2.M71 + matrix1.M24 * matrix2.M72 + matrix1.M34 * matrix2.M73 + matrix1.M44 * matrix2.M74 + matrix1.M54 * matrix2.M75 + matrix1.M64 * matrix2.M76 + matrix1.M74 * matrix2.M77 + matrix1.M84 * matrix2.M78;
            double m15 = matrix1.M15 * matrix2.M11 + matrix1.M25 * matrix2.M12 + matrix1.M35 * matrix2.M13 + matrix1.M45 * matrix2.M14 + matrix1.M55 * matrix2.M15 + matrix1.M65 * matrix2.M16 + matrix1.M75 * matrix2.M17 + matrix1.M85 * matrix2.M18;
            double m25 = matrix1.M15 * matrix2.M21 + matrix1.M25 * matrix2.M22 + matrix1.M35 * matrix2.M23 + matrix1.M45 * matrix2.M24 + matrix1.M55 * matrix2.M25 + matrix1.M65 * matrix2.M26 + matrix1.M75 * matrix2.M27 + matrix1.M85 * matrix2.M28;
            double m35 = matrix1.M15 * matrix2.M31 + matrix1.M25 * matrix2.M32 + matrix1.M35 * matrix2.M33 + matrix1.M45 * matrix2.M34 + matrix1.M55 * matrix2.M35 + matrix1.M65 * matrix2.M36 + matrix1.M75 * matrix2.M37 + matrix1.M85 * matrix2.M38;
            double m45 = matrix1.M15 * matrix2.M41 + matrix1.M25 * matrix2.M42 + matrix1.M35 * matrix2.M43 + matrix1.M45 * matrix2.M44 + matrix1.M55 * matrix2.M45 + matrix1.M65 * matrix2.M46 + matrix1.M75 * matrix2.M47 + matrix1.M85 * matrix2.M48;
            double m55 = matrix1.M15 * matrix2.M51 + matrix1.M25 * matrix2.M52 + matrix1.M35 * matrix2.M53 + matrix1.M45 * matrix2.M54 + matrix1.M55 * matrix2.M55 + matrix1.M65 * matrix2.M56 + matrix1.M75 * matrix2.M57 + matrix1.M85 * matrix2.M58;
            double m65 = matrix1.M15 * matrix2.M61 + matrix1.M25 * matrix2.M62 + matrix1.M35 * matrix2.M63 + matrix1.M45 * matrix2.M64 + matrix1.M55 * matrix2.M65 + matrix1.M65 * matrix2.M66 + matrix1.M75 * matrix2.M67 + matrix1.M85 * matrix2.M68;
            double m75 = matrix1.M15 * matrix2.M71 + matrix1.M25 * matrix2.M72 + matrix1.M35 * matrix2.M73 + matrix1.M45 * matrix2.M74 + matrix1.M55 * matrix2.M75 + matrix1.M65 * matrix2.M76 + matrix1.M75 * matrix2.M77 + matrix1.M85 * matrix2.M78;
            double m16 = matrix1.M16 * matrix2.M11 + matrix1.M26 * matrix2.M12 + matrix1.M36 * matrix2.M13 + matrix1.M46 * matrix2.M14 + matrix1.M56 * matrix2.M15 + matrix1.M66 * matrix2.M16 + matrix1.M76 * matrix2.M17 + matrix1.M86 * matrix2.M18;
            double m26 = matrix1.M16 * matrix2.M21 + matrix1.M26 * matrix2.M22 + matrix1.M36 * matrix2.M23 + matrix1.M46 * matrix2.M24 + matrix1.M56 * matrix2.M25 + matrix1.M66 * matrix2.M26 + matrix1.M76 * matrix2.M27 + matrix1.M86 * matrix2.M28;
            double m36 = matrix1.M16 * matrix2.M31 + matrix1.M26 * matrix2.M32 + matrix1.M36 * matrix2.M33 + matrix1.M46 * matrix2.M34 + matrix1.M56 * matrix2.M35 + matrix1.M66 * matrix2.M36 + matrix1.M76 * matrix2.M37 + matrix1.M86 * matrix2.M38;
            double m46 = matrix1.M16 * matrix2.M41 + matrix1.M26 * matrix2.M42 + matrix1.M36 * matrix2.M43 + matrix1.M46 * matrix2.M44 + matrix1.M56 * matrix2.M45 + matrix1.M66 * matrix2.M46 + matrix1.M76 * matrix2.M47 + matrix1.M86 * matrix2.M48;
            double m56 = matrix1.M16 * matrix2.M51 + matrix1.M26 * matrix2.M52 + matrix1.M36 * matrix2.M53 + matrix1.M46 * matrix2.M54 + matrix1.M56 * matrix2.M55 + matrix1.M66 * matrix2.M56 + matrix1.M76 * matrix2.M57 + matrix1.M86 * matrix2.M58;
            double m66 = matrix1.M16 * matrix2.M61 + matrix1.M26 * matrix2.M62 + matrix1.M36 * matrix2.M63 + matrix1.M46 * matrix2.M64 + matrix1.M56 * matrix2.M65 + matrix1.M66 * matrix2.M66 + matrix1.M76 * matrix2.M67 + matrix1.M86 * matrix2.M68;
            double m76 = matrix1.M16 * matrix2.M71 + matrix1.M26 * matrix2.M72 + matrix1.M36 * matrix2.M73 + matrix1.M46 * matrix2.M74 + matrix1.M56 * matrix2.M75 + matrix1.M66 * matrix2.M76 + matrix1.M76 * matrix2.M77 + matrix1.M86 * matrix2.M78;
            double m17 = matrix1.M17 * matrix2.M11 + matrix1.M27 * matrix2.M12 + matrix1.M37 * matrix2.M13 + matrix1.M47 * matrix2.M14 + matrix1.M57 * matrix2.M15 + matrix1.M67 * matrix2.M16 + matrix1.M77 * matrix2.M17 + matrix1.M87 * matrix2.M18;
            double m27 = matrix1.M17 * matrix2.M21 + matrix1.M27 * matrix2.M22 + matrix1.M37 * matrix2.M23 + matrix1.M47 * matrix2.M24 + matrix1.M57 * matrix2.M25 + matrix1.M67 * matrix2.M26 + matrix1.M77 * matrix2.M27 + matrix1.M87 * matrix2.M28;
            double m37 = matrix1.M17 * matrix2.M31 + matrix1.M27 * matrix2.M32 + matrix1.M37 * matrix2.M33 + matrix1.M47 * matrix2.M34 + matrix1.M57 * matrix2.M35 + matrix1.M67 * matrix2.M36 + matrix1.M77 * matrix2.M37 + matrix1.M87 * matrix2.M38;
            double m47 = matrix1.M17 * matrix2.M41 + matrix1.M27 * matrix2.M42 + matrix1.M37 * matrix2.M43 + matrix1.M47 * matrix2.M44 + matrix1.M57 * matrix2.M45 + matrix1.M67 * matrix2.M46 + matrix1.M77 * matrix2.M47 + matrix1.M87 * matrix2.M48;
            double m57 = matrix1.M17 * matrix2.M51 + matrix1.M27 * matrix2.M52 + matrix1.M37 * matrix2.M53 + matrix1.M47 * matrix2.M54 + matrix1.M57 * matrix2.M55 + matrix1.M67 * matrix2.M56 + matrix1.M77 * matrix2.M57 + matrix1.M87 * matrix2.M58;
            double m67 = matrix1.M17 * matrix2.M61 + matrix1.M27 * matrix2.M62 + matrix1.M37 * matrix2.M63 + matrix1.M47 * matrix2.M64 + matrix1.M57 * matrix2.M65 + matrix1.M67 * matrix2.M66 + matrix1.M77 * matrix2.M67 + matrix1.M87 * matrix2.M68;
            double m77 = matrix1.M17 * matrix2.M71 + matrix1.M27 * matrix2.M72 + matrix1.M37 * matrix2.M73 + matrix1.M47 * matrix2.M74 + matrix1.M57 * matrix2.M75 + matrix1.M67 * matrix2.M76 + matrix1.M77 * matrix2.M77 + matrix1.M87 * matrix2.M78;

            return new Matrix7x7(m11, m21, m31, m41, m51, m61, m71, 
                                 m12, m22, m32, m42, m52, m62, m72, 
                                 m13, m23, m33, m43, m53, m63, m73, 
                                 m14, m24, m34, m44, m54, m64, m74, 
                                 m15, m25, m35, m45, m55, m65, m75, 
                                 m16, m26, m36, m46, m56, m66, m76, 
                                 m17, m27, m37, m47, m57, m67, m77);
        }
        public static Matrix8x7 operator *(Matrix8x7 matrix1, Matrix8x8 matrix2)
        {
            double m11 = matrix1.M11 * matrix2.M11 + matrix1.M21 * matrix2.M12 + matrix1.M31 * matrix2.M13 + matrix1.M41 * matrix2.M14 + matrix1.M51 * matrix2.M15 + matrix1.M61 * matrix2.M16 + matrix1.M71 * matrix2.M17 + matrix1.M81 * matrix2.M18;
            double m21 = matrix1.M11 * matrix2.M21 + matrix1.M21 * matrix2.M22 + matrix1.M31 * matrix2.M23 + matrix1.M41 * matrix2.M24 + matrix1.M51 * matrix2.M25 + matrix1.M61 * matrix2.M26 + matrix1.M71 * matrix2.M27 + matrix1.M81 * matrix2.M28;
            double m31 = matrix1.M11 * matrix2.M31 + matrix1.M21 * matrix2.M32 + matrix1.M31 * matrix2.M33 + matrix1.M41 * matrix2.M34 + matrix1.M51 * matrix2.M35 + matrix1.M61 * matrix2.M36 + matrix1.M71 * matrix2.M37 + matrix1.M81 * matrix2.M38;
            double m41 = matrix1.M11 * matrix2.M41 + matrix1.M21 * matrix2.M42 + matrix1.M31 * matrix2.M43 + matrix1.M41 * matrix2.M44 + matrix1.M51 * matrix2.M45 + matrix1.M61 * matrix2.M46 + matrix1.M71 * matrix2.M47 + matrix1.M81 * matrix2.M48;
            double m51 = matrix1.M11 * matrix2.M51 + matrix1.M21 * matrix2.M52 + matrix1.M31 * matrix2.M53 + matrix1.M41 * matrix2.M54 + matrix1.M51 * matrix2.M55 + matrix1.M61 * matrix2.M56 + matrix1.M71 * matrix2.M57 + matrix1.M81 * matrix2.M58;
            double m61 = matrix1.M11 * matrix2.M61 + matrix1.M21 * matrix2.M62 + matrix1.M31 * matrix2.M63 + matrix1.M41 * matrix2.M64 + matrix1.M51 * matrix2.M65 + matrix1.M61 * matrix2.M66 + matrix1.M71 * matrix2.M67 + matrix1.M81 * matrix2.M68;
            double m71 = matrix1.M11 * matrix2.M71 + matrix1.M21 * matrix2.M72 + matrix1.M31 * matrix2.M73 + matrix1.M41 * matrix2.M74 + matrix1.M51 * matrix2.M75 + matrix1.M61 * matrix2.M76 + matrix1.M71 * matrix2.M77 + matrix1.M81 * matrix2.M78;
            double m81 = matrix1.M11 * matrix2.M81 + matrix1.M21 * matrix2.M82 + matrix1.M31 * matrix2.M83 + matrix1.M41 * matrix2.M84 + matrix1.M51 * matrix2.M85 + matrix1.M61 * matrix2.M86 + matrix1.M71 * matrix2.M87 + matrix1.M81 * matrix2.M88;
            double m12 = matrix1.M12 * matrix2.M11 + matrix1.M22 * matrix2.M12 + matrix1.M32 * matrix2.M13 + matrix1.M42 * matrix2.M14 + matrix1.M52 * matrix2.M15 + matrix1.M62 * matrix2.M16 + matrix1.M72 * matrix2.M17 + matrix1.M82 * matrix2.M18;
            double m22 = matrix1.M12 * matrix2.M21 + matrix1.M22 * matrix2.M22 + matrix1.M32 * matrix2.M23 + matrix1.M42 * matrix2.M24 + matrix1.M52 * matrix2.M25 + matrix1.M62 * matrix2.M26 + matrix1.M72 * matrix2.M27 + matrix1.M82 * matrix2.M28;
            double m32 = matrix1.M12 * matrix2.M31 + matrix1.M22 * matrix2.M32 + matrix1.M32 * matrix2.M33 + matrix1.M42 * matrix2.M34 + matrix1.M52 * matrix2.M35 + matrix1.M62 * matrix2.M36 + matrix1.M72 * matrix2.M37 + matrix1.M82 * matrix2.M38;
            double m42 = matrix1.M12 * matrix2.M41 + matrix1.M22 * matrix2.M42 + matrix1.M32 * matrix2.M43 + matrix1.M42 * matrix2.M44 + matrix1.M52 * matrix2.M45 + matrix1.M62 * matrix2.M46 + matrix1.M72 * matrix2.M47 + matrix1.M82 * matrix2.M48;
            double m52 = matrix1.M12 * matrix2.M51 + matrix1.M22 * matrix2.M52 + matrix1.M32 * matrix2.M53 + matrix1.M42 * matrix2.M54 + matrix1.M52 * matrix2.M55 + matrix1.M62 * matrix2.M56 + matrix1.M72 * matrix2.M57 + matrix1.M82 * matrix2.M58;
            double m62 = matrix1.M12 * matrix2.M61 + matrix1.M22 * matrix2.M62 + matrix1.M32 * matrix2.M63 + matrix1.M42 * matrix2.M64 + matrix1.M52 * matrix2.M65 + matrix1.M62 * matrix2.M66 + matrix1.M72 * matrix2.M67 + matrix1.M82 * matrix2.M68;
            double m72 = matrix1.M12 * matrix2.M71 + matrix1.M22 * matrix2.M72 + matrix1.M32 * matrix2.M73 + matrix1.M42 * matrix2.M74 + matrix1.M52 * matrix2.M75 + matrix1.M62 * matrix2.M76 + matrix1.M72 * matrix2.M77 + matrix1.M82 * matrix2.M78;
            double m82 = matrix1.M12 * matrix2.M81 + matrix1.M22 * matrix2.M82 + matrix1.M32 * matrix2.M83 + matrix1.M42 * matrix2.M84 + matrix1.M52 * matrix2.M85 + matrix1.M62 * matrix2.M86 + matrix1.M72 * matrix2.M87 + matrix1.M82 * matrix2.M88;
            double m13 = matrix1.M13 * matrix2.M11 + matrix1.M23 * matrix2.M12 + matrix1.M33 * matrix2.M13 + matrix1.M43 * matrix2.M14 + matrix1.M53 * matrix2.M15 + matrix1.M63 * matrix2.M16 + matrix1.M73 * matrix2.M17 + matrix1.M83 * matrix2.M18;
            double m23 = matrix1.M13 * matrix2.M21 + matrix1.M23 * matrix2.M22 + matrix1.M33 * matrix2.M23 + matrix1.M43 * matrix2.M24 + matrix1.M53 * matrix2.M25 + matrix1.M63 * matrix2.M26 + matrix1.M73 * matrix2.M27 + matrix1.M83 * matrix2.M28;
            double m33 = matrix1.M13 * matrix2.M31 + matrix1.M23 * matrix2.M32 + matrix1.M33 * matrix2.M33 + matrix1.M43 * matrix2.M34 + matrix1.M53 * matrix2.M35 + matrix1.M63 * matrix2.M36 + matrix1.M73 * matrix2.M37 + matrix1.M83 * matrix2.M38;
            double m43 = matrix1.M13 * matrix2.M41 + matrix1.M23 * matrix2.M42 + matrix1.M33 * matrix2.M43 + matrix1.M43 * matrix2.M44 + matrix1.M53 * matrix2.M45 + matrix1.M63 * matrix2.M46 + matrix1.M73 * matrix2.M47 + matrix1.M83 * matrix2.M48;
            double m53 = matrix1.M13 * matrix2.M51 + matrix1.M23 * matrix2.M52 + matrix1.M33 * matrix2.M53 + matrix1.M43 * matrix2.M54 + matrix1.M53 * matrix2.M55 + matrix1.M63 * matrix2.M56 + matrix1.M73 * matrix2.M57 + matrix1.M83 * matrix2.M58;
            double m63 = matrix1.M13 * matrix2.M61 + matrix1.M23 * matrix2.M62 + matrix1.M33 * matrix2.M63 + matrix1.M43 * matrix2.M64 + matrix1.M53 * matrix2.M65 + matrix1.M63 * matrix2.M66 + matrix1.M73 * matrix2.M67 + matrix1.M83 * matrix2.M68;
            double m73 = matrix1.M13 * matrix2.M71 + matrix1.M23 * matrix2.M72 + matrix1.M33 * matrix2.M73 + matrix1.M43 * matrix2.M74 + matrix1.M53 * matrix2.M75 + matrix1.M63 * matrix2.M76 + matrix1.M73 * matrix2.M77 + matrix1.M83 * matrix2.M78;
            double m83 = matrix1.M13 * matrix2.M81 + matrix1.M23 * matrix2.M82 + matrix1.M33 * matrix2.M83 + matrix1.M43 * matrix2.M84 + matrix1.M53 * matrix2.M85 + matrix1.M63 * matrix2.M86 + matrix1.M73 * matrix2.M87 + matrix1.M83 * matrix2.M88;
            double m14 = matrix1.M14 * matrix2.M11 + matrix1.M24 * matrix2.M12 + matrix1.M34 * matrix2.M13 + matrix1.M44 * matrix2.M14 + matrix1.M54 * matrix2.M15 + matrix1.M64 * matrix2.M16 + matrix1.M74 * matrix2.M17 + matrix1.M84 * matrix2.M18;
            double m24 = matrix1.M14 * matrix2.M21 + matrix1.M24 * matrix2.M22 + matrix1.M34 * matrix2.M23 + matrix1.M44 * matrix2.M24 + matrix1.M54 * matrix2.M25 + matrix1.M64 * matrix2.M26 + matrix1.M74 * matrix2.M27 + matrix1.M84 * matrix2.M28;
            double m34 = matrix1.M14 * matrix2.M31 + matrix1.M24 * matrix2.M32 + matrix1.M34 * matrix2.M33 + matrix1.M44 * matrix2.M34 + matrix1.M54 * matrix2.M35 + matrix1.M64 * matrix2.M36 + matrix1.M74 * matrix2.M37 + matrix1.M84 * matrix2.M38;
            double m44 = matrix1.M14 * matrix2.M41 + matrix1.M24 * matrix2.M42 + matrix1.M34 * matrix2.M43 + matrix1.M44 * matrix2.M44 + matrix1.M54 * matrix2.M45 + matrix1.M64 * matrix2.M46 + matrix1.M74 * matrix2.M47 + matrix1.M84 * matrix2.M48;
            double m54 = matrix1.M14 * matrix2.M51 + matrix1.M24 * matrix2.M52 + matrix1.M34 * matrix2.M53 + matrix1.M44 * matrix2.M54 + matrix1.M54 * matrix2.M55 + matrix1.M64 * matrix2.M56 + matrix1.M74 * matrix2.M57 + matrix1.M84 * matrix2.M58;
            double m64 = matrix1.M14 * matrix2.M61 + matrix1.M24 * matrix2.M62 + matrix1.M34 * matrix2.M63 + matrix1.M44 * matrix2.M64 + matrix1.M54 * matrix2.M65 + matrix1.M64 * matrix2.M66 + matrix1.M74 * matrix2.M67 + matrix1.M84 * matrix2.M68;
            double m74 = matrix1.M14 * matrix2.M71 + matrix1.M24 * matrix2.M72 + matrix1.M34 * matrix2.M73 + matrix1.M44 * matrix2.M74 + matrix1.M54 * matrix2.M75 + matrix1.M64 * matrix2.M76 + matrix1.M74 * matrix2.M77 + matrix1.M84 * matrix2.M78;
            double m84 = matrix1.M14 * matrix2.M81 + matrix1.M24 * matrix2.M82 + matrix1.M34 * matrix2.M83 + matrix1.M44 * matrix2.M84 + matrix1.M54 * matrix2.M85 + matrix1.M64 * matrix2.M86 + matrix1.M74 * matrix2.M87 + matrix1.M84 * matrix2.M88;
            double m15 = matrix1.M15 * matrix2.M11 + matrix1.M25 * matrix2.M12 + matrix1.M35 * matrix2.M13 + matrix1.M45 * matrix2.M14 + matrix1.M55 * matrix2.M15 + matrix1.M65 * matrix2.M16 + matrix1.M75 * matrix2.M17 + matrix1.M85 * matrix2.M18;
            double m25 = matrix1.M15 * matrix2.M21 + matrix1.M25 * matrix2.M22 + matrix1.M35 * matrix2.M23 + matrix1.M45 * matrix2.M24 + matrix1.M55 * matrix2.M25 + matrix1.M65 * matrix2.M26 + matrix1.M75 * matrix2.M27 + matrix1.M85 * matrix2.M28;
            double m35 = matrix1.M15 * matrix2.M31 + matrix1.M25 * matrix2.M32 + matrix1.M35 * matrix2.M33 + matrix1.M45 * matrix2.M34 + matrix1.M55 * matrix2.M35 + matrix1.M65 * matrix2.M36 + matrix1.M75 * matrix2.M37 + matrix1.M85 * matrix2.M38;
            double m45 = matrix1.M15 * matrix2.M41 + matrix1.M25 * matrix2.M42 + matrix1.M35 * matrix2.M43 + matrix1.M45 * matrix2.M44 + matrix1.M55 * matrix2.M45 + matrix1.M65 * matrix2.M46 + matrix1.M75 * matrix2.M47 + matrix1.M85 * matrix2.M48;
            double m55 = matrix1.M15 * matrix2.M51 + matrix1.M25 * matrix2.M52 + matrix1.M35 * matrix2.M53 + matrix1.M45 * matrix2.M54 + matrix1.M55 * matrix2.M55 + matrix1.M65 * matrix2.M56 + matrix1.M75 * matrix2.M57 + matrix1.M85 * matrix2.M58;
            double m65 = matrix1.M15 * matrix2.M61 + matrix1.M25 * matrix2.M62 + matrix1.M35 * matrix2.M63 + matrix1.M45 * matrix2.M64 + matrix1.M55 * matrix2.M65 + matrix1.M65 * matrix2.M66 + matrix1.M75 * matrix2.M67 + matrix1.M85 * matrix2.M68;
            double m75 = matrix1.M15 * matrix2.M71 + matrix1.M25 * matrix2.M72 + matrix1.M35 * matrix2.M73 + matrix1.M45 * matrix2.M74 + matrix1.M55 * matrix2.M75 + matrix1.M65 * matrix2.M76 + matrix1.M75 * matrix2.M77 + matrix1.M85 * matrix2.M78;
            double m85 = matrix1.M15 * matrix2.M81 + matrix1.M25 * matrix2.M82 + matrix1.M35 * matrix2.M83 + matrix1.M45 * matrix2.M84 + matrix1.M55 * matrix2.M85 + matrix1.M65 * matrix2.M86 + matrix1.M75 * matrix2.M87 + matrix1.M85 * matrix2.M88;
            double m16 = matrix1.M16 * matrix2.M11 + matrix1.M26 * matrix2.M12 + matrix1.M36 * matrix2.M13 + matrix1.M46 * matrix2.M14 + matrix1.M56 * matrix2.M15 + matrix1.M66 * matrix2.M16 + matrix1.M76 * matrix2.M17 + matrix1.M86 * matrix2.M18;
            double m26 = matrix1.M16 * matrix2.M21 + matrix1.M26 * matrix2.M22 + matrix1.M36 * matrix2.M23 + matrix1.M46 * matrix2.M24 + matrix1.M56 * matrix2.M25 + matrix1.M66 * matrix2.M26 + matrix1.M76 * matrix2.M27 + matrix1.M86 * matrix2.M28;
            double m36 = matrix1.M16 * matrix2.M31 + matrix1.M26 * matrix2.M32 + matrix1.M36 * matrix2.M33 + matrix1.M46 * matrix2.M34 + matrix1.M56 * matrix2.M35 + matrix1.M66 * matrix2.M36 + matrix1.M76 * matrix2.M37 + matrix1.M86 * matrix2.M38;
            double m46 = matrix1.M16 * matrix2.M41 + matrix1.M26 * matrix2.M42 + matrix1.M36 * matrix2.M43 + matrix1.M46 * matrix2.M44 + matrix1.M56 * matrix2.M45 + matrix1.M66 * matrix2.M46 + matrix1.M76 * matrix2.M47 + matrix1.M86 * matrix2.M48;
            double m56 = matrix1.M16 * matrix2.M51 + matrix1.M26 * matrix2.M52 + matrix1.M36 * matrix2.M53 + matrix1.M46 * matrix2.M54 + matrix1.M56 * matrix2.M55 + matrix1.M66 * matrix2.M56 + matrix1.M76 * matrix2.M57 + matrix1.M86 * matrix2.M58;
            double m66 = matrix1.M16 * matrix2.M61 + matrix1.M26 * matrix2.M62 + matrix1.M36 * matrix2.M63 + matrix1.M46 * matrix2.M64 + matrix1.M56 * matrix2.M65 + matrix1.M66 * matrix2.M66 + matrix1.M76 * matrix2.M67 + matrix1.M86 * matrix2.M68;
            double m76 = matrix1.M16 * matrix2.M71 + matrix1.M26 * matrix2.M72 + matrix1.M36 * matrix2.M73 + matrix1.M46 * matrix2.M74 + matrix1.M56 * matrix2.M75 + matrix1.M66 * matrix2.M76 + matrix1.M76 * matrix2.M77 + matrix1.M86 * matrix2.M78;
            double m86 = matrix1.M16 * matrix2.M81 + matrix1.M26 * matrix2.M82 + matrix1.M36 * matrix2.M83 + matrix1.M46 * matrix2.M84 + matrix1.M56 * matrix2.M85 + matrix1.M66 * matrix2.M86 + matrix1.M76 * matrix2.M87 + matrix1.M86 * matrix2.M88;
            double m17 = matrix1.M17 * matrix2.M11 + matrix1.M27 * matrix2.M12 + matrix1.M37 * matrix2.M13 + matrix1.M47 * matrix2.M14 + matrix1.M57 * matrix2.M15 + matrix1.M67 * matrix2.M16 + matrix1.M77 * matrix2.M17 + matrix1.M87 * matrix2.M18;
            double m27 = matrix1.M17 * matrix2.M21 + matrix1.M27 * matrix2.M22 + matrix1.M37 * matrix2.M23 + matrix1.M47 * matrix2.M24 + matrix1.M57 * matrix2.M25 + matrix1.M67 * matrix2.M26 + matrix1.M77 * matrix2.M27 + matrix1.M87 * matrix2.M28;
            double m37 = matrix1.M17 * matrix2.M31 + matrix1.M27 * matrix2.M32 + matrix1.M37 * matrix2.M33 + matrix1.M47 * matrix2.M34 + matrix1.M57 * matrix2.M35 + matrix1.M67 * matrix2.M36 + matrix1.M77 * matrix2.M37 + matrix1.M87 * matrix2.M38;
            double m47 = matrix1.M17 * matrix2.M41 + matrix1.M27 * matrix2.M42 + matrix1.M37 * matrix2.M43 + matrix1.M47 * matrix2.M44 + matrix1.M57 * matrix2.M45 + matrix1.M67 * matrix2.M46 + matrix1.M77 * matrix2.M47 + matrix1.M87 * matrix2.M48;
            double m57 = matrix1.M17 * matrix2.M51 + matrix1.M27 * matrix2.M52 + matrix1.M37 * matrix2.M53 + matrix1.M47 * matrix2.M54 + matrix1.M57 * matrix2.M55 + matrix1.M67 * matrix2.M56 + matrix1.M77 * matrix2.M57 + matrix1.M87 * matrix2.M58;
            double m67 = matrix1.M17 * matrix2.M61 + matrix1.M27 * matrix2.M62 + matrix1.M37 * matrix2.M63 + matrix1.M47 * matrix2.M64 + matrix1.M57 * matrix2.M65 + matrix1.M67 * matrix2.M66 + matrix1.M77 * matrix2.M67 + matrix1.M87 * matrix2.M68;
            double m77 = matrix1.M17 * matrix2.M71 + matrix1.M27 * matrix2.M72 + matrix1.M37 * matrix2.M73 + matrix1.M47 * matrix2.M74 + matrix1.M57 * matrix2.M75 + matrix1.M67 * matrix2.M76 + matrix1.M77 * matrix2.M77 + matrix1.M87 * matrix2.M78;
            double m87 = matrix1.M17 * matrix2.M81 + matrix1.M27 * matrix2.M82 + matrix1.M37 * matrix2.M83 + matrix1.M47 * matrix2.M84 + matrix1.M57 * matrix2.M85 + matrix1.M67 * matrix2.M86 + matrix1.M77 * matrix2.M87 + matrix1.M87 * matrix2.M88;

            return new Matrix8x7(m11, m21, m31, m41, m51, m61, m71, m81, 
                                 m12, m22, m32, m42, m52, m62, m72, m82, 
                                 m13, m23, m33, m43, m53, m63, m73, m83, 
                                 m14, m24, m34, m44, m54, m64, m74, m84, 
                                 m15, m25, m35, m45, m55, m65, m75, m85, 
                                 m16, m26, m36, m46, m56, m66, m76, m86, 
                                 m17, m27, m37, m47, m57, m67, m77, m87);
        }
    }
}
