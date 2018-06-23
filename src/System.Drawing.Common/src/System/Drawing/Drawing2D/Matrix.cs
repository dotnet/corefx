// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Gdip = System.Drawing.SafeNativeMethods.Gdip;

namespace System.Drawing.Drawing2D
{
    public sealed class Matrix : MarshalByRefObject, IDisposable
    {
        internal IntPtr nativeMatrix;

        public Matrix()
        {
            int status = Gdip.GdipCreateMatrix(out IntPtr nativeMatrix);

            if (status != Gdip.Ok)
                throw Gdip.StatusException(status);

            this.nativeMatrix = nativeMatrix;
        }

        public Matrix(float m11, float m12, float m21, float m22, float dx, float dy)
        {
            int status = Gdip.GdipCreateMatrix2(m11, m12, m21, m22, dx, dy, out IntPtr nativeMatrix);

            if (status != Gdip.Ok)
                throw Gdip.StatusException(status);

            this.nativeMatrix = nativeMatrix;
        }

        public unsafe Matrix(RectangleF rect, PointF[] plgpts)
        {
            if (plgpts == null)
                throw new ArgumentNullException(nameof(plgpts));
            if (plgpts.Length != 3)
                throw Gdip.StatusException(Gdip.InvalidParameter);

            fixed (PointF* p = plgpts)
            {
                Gdip.CheckStatus(Gdip.GdipCreateMatrix3(ref rect, p, out IntPtr nativeMatrix));
                this.nativeMatrix = nativeMatrix;
            }
        }

        public unsafe Matrix(Rectangle rect, Point[] plgpts)
        {
            if (plgpts == null)
                throw new ArgumentNullException(nameof(plgpts));
            if (plgpts.Length != 3)
                throw Gdip.StatusException(Gdip.InvalidParameter);

            fixed (Point* p = plgpts)
            {
                Gdip.CheckStatus(Gdip.GdipCreateMatrix3I(ref rect, p, out IntPtr nativeMatrix));
                this.nativeMatrix = nativeMatrix;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (nativeMatrix != IntPtr.Zero)
            {
                Gdip.GdipDeleteMatrix(new HandleRef(this, nativeMatrix));
                nativeMatrix = IntPtr.Zero;
            }
        }

        ~Matrix() => Dispose(false);

        public Matrix Clone()
        {
            int status = Gdip.GdipCloneMatrix(new HandleRef(this, nativeMatrix), out IntPtr clonedMatrix);

            if (status != Gdip.Ok)
                throw Gdip.StatusException(status);

            return new Matrix(clonedMatrix);
        }

        public float[] Elements
        {
            get
            {
                IntPtr buf = Marshal.AllocHGlobal(6 * 8); // 6 elements x 8 bytes (float)

                try
                {
                    int status = Gdip.GdipGetMatrixElements(new HandleRef(this, nativeMatrix), buf);

                    if (status != Gdip.Ok)
                    {
                        throw Gdip.StatusException(status);
                    }

                    float[] m = new float[6];
                    Marshal.Copy(buf, m, 0, 6);
                    return m;
                }
                finally
                {
                    Marshal.FreeHGlobal(buf);
                }
            }
        }

        public float OffsetX => Elements[4];
        public float OffsetY => Elements[5];

        public void Reset()
        {
            int status = Gdip.GdipSetMatrixElements(new HandleRef(this, nativeMatrix),
                                                       1.0f, 0.0f, 0.0f,
                                                       1.0f, 0.0f, 0.0f);

            if (status != Gdip.Ok)
                throw Gdip.StatusException(status);
        }

        public void Multiply(Matrix matrix) => Multiply(matrix, MatrixOrder.Prepend);

        public void Multiply(Matrix matrix, MatrixOrder order)
        {
            if (matrix == null)
            {
                throw new ArgumentNullException(nameof(matrix));
            }

            if (matrix.nativeMatrix == nativeMatrix)
            {
                throw new InvalidOperationException(SR.GdiplusObjectBusy);
            }

            int status = Gdip.GdipMultiplyMatrix(new HandleRef(this, nativeMatrix), new HandleRef(matrix, matrix.nativeMatrix),
                                                    order);

            if (status != Gdip.Ok)
                throw Gdip.StatusException(status);
        }

        public void Translate(float offsetX, float offsetY) => Translate(offsetX, offsetY, MatrixOrder.Prepend);

        public void Translate(float offsetX, float offsetY, MatrixOrder order)
        {
            int status = Gdip.GdipTranslateMatrix(new HandleRef(this, nativeMatrix),
                                                     offsetX, offsetY, order);

            if (status != Gdip.Ok)
                throw Gdip.StatusException(status);
        }

        public void Scale(float scaleX, float scaleY) => Scale(scaleX, scaleY, MatrixOrder.Prepend);

        public void Scale(float scaleX, float scaleY, MatrixOrder order)
        {
            int status = Gdip.GdipScaleMatrix(new HandleRef(this, nativeMatrix), scaleX, scaleY, order);

            if (status != Gdip.Ok)
                throw Gdip.StatusException(status);
        }

        public void Rotate(float angle) => Rotate(angle, MatrixOrder.Prepend);

        public void Rotate(float angle, MatrixOrder order)
        {
            int status = Gdip.GdipRotateMatrix(new HandleRef(this, nativeMatrix), angle, order);

            if (status != Gdip.Ok)
                throw Gdip.StatusException(status);
        }

        public void RotateAt(float angle, PointF point) => RotateAt(angle, point, MatrixOrder.Prepend);
        public void RotateAt(float angle, PointF point, MatrixOrder order)
        {
            int status;
            if (order == MatrixOrder.Prepend)
            {
                status = Gdip.GdipTranslateMatrix(new HandleRef(this, nativeMatrix), point.X, point.Y, order);
                status |= Gdip.GdipRotateMatrix(new HandleRef(this, nativeMatrix), angle, order);
                status |= Gdip.GdipTranslateMatrix(new HandleRef(this, nativeMatrix), -point.X, -point.Y, order);
            }
            else
            {
                status = Gdip.GdipTranslateMatrix(new HandleRef(this, nativeMatrix), -point.X, -point.Y, order);
                status |= Gdip.GdipRotateMatrix(new HandleRef(this, nativeMatrix), angle, order);
                status |= Gdip.GdipTranslateMatrix(new HandleRef(this, nativeMatrix), point.X, point.Y, order);
            }

            if (status != Gdip.Ok)
                throw Gdip.StatusException(status);
        }

        public void Shear(float shearX, float shearY)
        {
            int status = Gdip.GdipShearMatrix(new HandleRef(this, nativeMatrix), shearX, shearY, MatrixOrder.Prepend);

            if (status != Gdip.Ok)
                throw Gdip.StatusException(status);
        }

        public void Shear(float shearX, float shearY, MatrixOrder order)
        {
            int status = Gdip.GdipShearMatrix(new HandleRef(this, nativeMatrix), shearX, shearY, order);

            if (status != Gdip.Ok)
                throw Gdip.StatusException(status);
        }

        public void Invert()
        {
            int status = Gdip.GdipInvertMatrix(new HandleRef(this, nativeMatrix));

            if (status != Gdip.Ok)
                throw Gdip.StatusException(status);
        }

        public unsafe void TransformPoints(PointF[] pts)
        {
            if (pts == null)
                throw new ArgumentNullException(nameof(pts));

            fixed (PointF* p = pts)
            {
                int status = Gdip.GdipTransformMatrixPoints(
                    new HandleRef(this, nativeMatrix),
                    p,
                    pts.Length);

                if (status != Gdip.Ok)
                {
                    throw Gdip.StatusException(status);
                }
            }
        }

        public unsafe void TransformPoints(Point[] pts)
        {
            if (pts == null)
                throw new ArgumentNullException(nameof(pts));

            fixed (Point* p = pts)
            {
                int status = Gdip.GdipTransformMatrixPointsI(
                    new HandleRef(this, nativeMatrix),
                    p,
                    pts.Length);

                if (status != Gdip.Ok)
                {
                    throw Gdip.StatusException(status);
                }
            }
        }

        public unsafe void TransformVectors(PointF[] pts)
        {
            if (pts == null)
                throw new ArgumentNullException(nameof(pts));

            fixed (PointF* p = pts)
            {
                int status = Gdip.GdipVectorTransformMatrixPoints(
                    new HandleRef(this, nativeMatrix),
                    p,
                    pts.Length);

                if (status != Gdip.Ok)
                {
                    throw Gdip.StatusException(status);
                }
            }
        }

        public void VectorTransformPoints(Point[] pts) => TransformVectors(pts);

        public unsafe void TransformVectors(Point[] pts)
        {
            if (pts == null)
                throw new ArgumentNullException(nameof(pts));

            fixed (Point* p = pts)
            {
                int status = Gdip.GdipVectorTransformMatrixPointsI(
                    new HandleRef(this, nativeMatrix),
                    p,
                    pts.Length);

                if (status != Gdip.Ok)
                {
                    throw Gdip.StatusException(status);
                }
            }
        }

        public bool IsInvertible
        {
            get
            {
                int status = Gdip.GdipIsMatrixInvertible(new HandleRef(this, nativeMatrix), out int isInvertible);

                if (status != Gdip.Ok)
                    throw Gdip.StatusException(status);

                return isInvertible != 0;
            }
        }

        public bool IsIdentity
        {
            get
            {
                int status = Gdip.GdipIsMatrixIdentity(new HandleRef(this, nativeMatrix), out int isIdentity);

                if (status != Gdip.Ok)
                    throw Gdip.StatusException(status);

                return isIdentity != 0;
            }
        }
        public override bool Equals(object obj)
        {
            Matrix matrix2 = obj as Matrix;
            if (matrix2 == null)
                return false;


            int status = Gdip.GdipIsMatrixEqual(new HandleRef(this, nativeMatrix),
                                                   new HandleRef(matrix2, matrix2.nativeMatrix),
                                                   out int isEqual);

            if (status != Gdip.Ok)
                throw Gdip.StatusException(status);

            return isEqual != 0;
        }

        public override int GetHashCode() => base.GetHashCode();

        internal Matrix(IntPtr nativeMatrix) => SetNativeMatrix(nativeMatrix);

        internal void SetNativeMatrix(IntPtr nativeMatrix) => this.nativeMatrix = nativeMatrix;
    }
}
