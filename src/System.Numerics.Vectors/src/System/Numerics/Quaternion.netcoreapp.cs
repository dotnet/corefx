// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using Internal.Runtime.CompilerServices;

namespace System.Numerics
{
    /// <summary>
    /// A structure encapsulating a four-dimensional vector (x,y,z,w), 
    /// which is used to efficiently rotate an object about the (x,y,z) vector by the angle theta, where w = cos(theta/2).
    /// </summary>
    public struct Quaternion : IEquatable<Quaternion>
    {
        /// <summary>
        /// Specifies the X-value of the vector component of the Quaternion.
        /// </summary>
        public float X;
        /// <summary>
        /// Specifies the Y-value of the vector component of the Quaternion.
        /// </summary>
        public float Y;
        /// <summary>
        /// Specifies the Z-value of the vector component of the Quaternion.
        /// </summary>
        public float Z;
        /// <summary>
        /// Specifies the rotation component of the Quaternion.
        /// </summary>
        public float W;

        /// <summary>
        /// Returns a Quaternion representing no rotation. 
        /// </summary>
        public static Quaternion Identity
        {
            get { return new Quaternion(0, 0, 0, 1); }
        }

        /// <summary>
        /// Returns whether the Quaternion is the identity Quaternion.
        /// </summary>
        public bool IsIdentity
        {
            get { return X == 0f && Y == 0f && Z == 0f && W == 1f; }
        }

        /// <summary>
        /// Constructs a Quaternion from the given components.
        /// </summary>
        /// <param name="x">The X component of the Quaternion.</param>
        /// <param name="y">The Y component of the Quaternion.</param>
        /// <param name="z">The Z component of the Quaternion.</param>
        /// <param name="w">The W component of the Quaternion.</param>
        public Quaternion(float x, float y, float z, float w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        /// <summary>
        /// Constructs a Quaternion from the given vector and rotation parts.
        /// </summary>
        /// <param name="vectorPart">The vector part of the Quaternion.</param>
        /// <param name="scalarPart">The rotation part of the Quaternion.</param>
        public Quaternion(Vector3 vectorPart, float scalarPart)
        {
            X = vectorPart.X;
            Y = vectorPart.Y;
            Z = vectorPart.Z;
            W = scalarPart;
        }

        /// <summary>
        /// Calculates the length of the Quaternion.
        /// </summary>
        /// <returns>The computed length of the Quaternion.</returns>
        public float Length()
        {
            Vector4 q = Unsafe.As<Quaternion, Vector4>(ref this);
            return q.Length();
        }

        /// <summary>
        /// Calculates the length squared of the Quaternion. This operation is cheaper than Length().
        /// </summary>
        /// <returns>The length squared of the Quaternion.</returns>
        public float LengthSquared()
        {
            Vector4 q = Unsafe.As<Quaternion, Vector4>(ref this);
            return q.LengthSquared();
        }

        /// <summary>
        /// Divides each component of the Quaternion by the length of the Quaternion.
        /// </summary>
        /// <param name="value">The source Quaternion.</param>
        /// <returns>The normalized Quaternion.</returns>
        public static Quaternion Normalize(Quaternion value)
        {
            Vector4 q = Unsafe.As<Quaternion, Vector4>(ref value);
            q = Vector4.Normalize(q);
            return Unsafe.As<Vector4, Quaternion>(ref q);
        }

        /// <summary>
        /// Creates the conjugate of a specified Quaternion.
        /// </summary>
        /// <param name="value">The Quaternion of which to return the conjugate.</param>
        /// <returns>A new Quaternion that is the conjugate of the specified one.</returns>
        public static Quaternion Conjugate(Quaternion value)
        {
            Vector4 q = -Unsafe.As<Quaternion, Vector4>(ref value);
            q.W = -q.W;
            return Unsafe.As<Vector4, Quaternion>(ref q);
        }

        /// <summary>
        /// Returns the inverse of a Quaternion.
        /// </summary>
        /// <param name="value">The source Quaternion.</param>
        /// <returns>The inverted Quaternion.</returns>
        public static Quaternion Inverse(Quaternion value)
        {
            //  -1   (       a              -v       )
            // q   = ( -------------   ------------- )
            //       (  a^2 + |v|^2  ,  a^2 + |v|^2  )

            Vector4 q = Unsafe.As<Quaternion, Vector4>(ref value);

            float ls = Vector4.Dot(q, q);
            float invNorm = -1.0f / ls;

            q *= invNorm;
            q.W = -q.W;

            return Unsafe.As<Vector4, Quaternion>(ref q);
        }

        /// <summary>
        /// Creates a Quaternion from a normalized vector axis and an angle to rotate about the vector.
        /// </summary>
        /// <param name="axis">The unit vector to rotate around.
        /// This vector must be normalized before calling this function or the resulting Quaternion will be incorrect.</param>
        /// <param name="angle">The angle, in radians, to rotate around the vector.</param>
        /// <returns>The created Quaternion.</returns>
        public static Quaternion CreateFromAxisAngle(Vector3 axis, float angle)
        {
            float halfAngle = angle * 0.5f;
            float s = MathF.Sin(halfAngle);
            float c = MathF.Cos(halfAngle);

            Vector4 q = new Vector4(axis, 0) * s;

            q.W = c;

            return Unsafe.As<Vector4, Quaternion>(ref q);
        }

        /// <summary>
        /// Creates a new Quaternion from the given yaw, pitch, and roll, in radians.
        /// </summary>
        /// <param name="yaw">The yaw angle, in radians, around the Y-axis.</param>
        /// <param name="pitch">The pitch angle, in radians, around the X-axis.</param>
        /// <param name="roll">The roll angle, in radians, around the Z-axis.</param>
        /// <returns></returns>
        public static Quaternion CreateFromYawPitchRoll(float yaw, float pitch, float roll)
        {
            //  Roll first, about axis the object is facing, then
            //  pitch upward, then yaw to face into the new heading
            float sr, cr, sp, cp, sy, cy;

            float halfRoll = roll * 0.5f;
            sr = MathF.Sin(halfRoll);
            cr = MathF.Cos(halfRoll);

            float halfPitch = pitch * 0.5f;
            sp = MathF.Sin(halfPitch);
            cp = MathF.Cos(halfPitch);

            float halfYaw = yaw * 0.5f;
            sy = MathF.Sin(halfYaw);
            cy = MathF.Cos(halfYaw);

            Vector4 result = 
                new Vector4(
                    cy * sp * cr, 
                    sy * cp * cr, 
                    cy * cp * sr, 
                    cy * cp * cr)
                + new Vector4(
                    sy * cp * sr, 
                    -cy * sp * sr, 
                    -sy * sp * cr, 
                    sy * sp * sr);

            return Unsafe.As<Vector4, Quaternion>(ref result);
        }

        /// <summary>
        /// Creates a Quaternion from the given rotation matrix.
        /// </summary>
        /// <param name="matrix">The rotation matrix.</param>
        /// <returns>The created Quaternion.</returns>
        public static Quaternion CreateFromRotationMatrix(Matrix4x4 matrix)
        {
            float trace = matrix.M11 + matrix.M22 + matrix.M33;

            Quaternion q = new Quaternion();

            if (trace > 0.0f)
            {
                float s = MathF.Sqrt(trace + 1.0f);
                q.W = s * 0.5f;
                s = 0.5f / s;
                q.X = (matrix.M23 - matrix.M32) * s;
                q.Y = (matrix.M31 - matrix.M13) * s;
                q.Z = (matrix.M12 - matrix.M21) * s;
            }
            else
            {
                if (matrix.M11 >= matrix.M22 && matrix.M11 >= matrix.M33)
                {
                    float s = MathF.Sqrt(1.0f + matrix.M11 - matrix.M22 - matrix.M33);
                    float invS = 0.5f / s;
                    q.X = 0.5f * s;
                    q.Y = (matrix.M12 + matrix.M21) * invS;
                    q.Z = (matrix.M13 + matrix.M31) * invS;
                    q.W = (matrix.M23 - matrix.M32) * invS;
                }
                else if (matrix.M22 > matrix.M33)
                {
                    float s = MathF.Sqrt(1.0f + matrix.M22 - matrix.M11 - matrix.M33);
                    float invS = 0.5f / s;
                    q.X = (matrix.M21 + matrix.M12) * invS;
                    q.Y = 0.5f * s;
                    q.Z = (matrix.M32 + matrix.M23) * invS;
                    q.W = (matrix.M31 - matrix.M13) * invS;
                }
                else
                {
                    float s = MathF.Sqrt(1.0f + matrix.M33 - matrix.M11 - matrix.M22);
                    float invS = 0.5f / s;
                    q.X = (matrix.M31 + matrix.M13) * invS;
                    q.Y = (matrix.M32 + matrix.M23) * invS;
                    q.Z = 0.5f * s;
                    q.W = (matrix.M12 - matrix.M21) * invS;
                }
            }

            return q;
        }

        /// <summary>
        /// Calculates the dot product of two Quaternions.
        /// </summary>
        /// <param name="quaternion1">The first source Quaternion.</param>
        /// <param name="quaternion2">The second source Quaternion.</param>
        /// <returns>The dot product of the Quaternions.</returns>
        public static float Dot(Quaternion quaternion1, Quaternion quaternion2)
        {
            return Vector4.Dot(
                Unsafe.As<Quaternion, Vector4>(ref quaternion1), 
                Unsafe.As<Quaternion, Vector4>(ref quaternion2));
        }

        /// <summary>
        /// Interpolates between two quaternions, using spherical linear interpolation.
        /// </summary>
        /// <param name="quaternion1">The first source Quaternion.</param>
        /// <param name="quaternion2">The second source Quaternion.</param>
        /// <param name="amount">The relative weight of the second source Quaternion in the interpolation.</param>
        /// <returns>The interpolated Quaternion.</returns>
        public static Quaternion Slerp(Quaternion quaternion1, Quaternion quaternion2, float amount)
        {
            const float epsilon = 1e-6f;

            float t = amount;

            Vector4 q1 = Unsafe.As<Quaternion, Vector4>(ref quaternion1);
            Vector4 q2 = Unsafe.As<Quaternion, Vector4>(ref quaternion2);

            float cosOmega = Vector4.Dot(q1, q2);

            bool flip = false;

            if (cosOmega < 0.0f)
            {
                flip = true;
                cosOmega = -cosOmega;
            }

            float s1, s2;

            if (cosOmega > (1.0f - epsilon))
            {
                // Too close, do straight linear interpolation.
                s1 = 1.0f - t;
                s2 = (flip) ? -t : t;
            }
            else
            {
                float omega = MathF.Acos(cosOmega);
                float invSinOmega = 1 / MathF.Sin(omega);

                s1 = MathF.Sin((1.0f - t) * omega) * invSinOmega;
                s2 = (flip)
                    ? -MathF.Sin(t * omega) * invSinOmega
                    : MathF.Sin(t * omega) * invSinOmega;
            }

            Vector4 result = q1 * s1 + q2 * s2;

            return Unsafe.As<Vector4, Quaternion>(ref result);
        }

        /// <summary>
        ///  Linearly interpolates between two quaternions.
        /// </summary>
        /// <param name="quaternion1">The first source Quaternion.</param>
        /// <param name="quaternion2">The second source Quaternion.</param>
        /// <param name="amount">The relative weight of the second source Quaternion in the interpolation.</param>
        /// <returns>The interpolated Quaternion.</returns>
        public static Quaternion Lerp(Quaternion quaternion1, Quaternion quaternion2, float amount)
        {
            float t = amount;
            float t1 = 1.0f - t;

            Vector4 q1 = Unsafe.As<Quaternion, Vector4>(ref quaternion1);
            Vector4 q2 = Unsafe.As<Quaternion, Vector4>(ref quaternion2);

            float dot = Vector4.Dot(q1, q2);

            q1 *= t1;
            if (dot >= 0.0f)
            {
                q2 *= t;
            }
            else
            {
                q2 *= -t;
            }

            Vector4 result = Vector4.Normalize(q1 + q2);

            return Unsafe.As<Vector4, Quaternion>(ref result);
        }

        /// <summary>
        /// Concatenates two Quaternions; the result represents the value1 rotation followed by the value2 rotation.
        /// </summary>
        /// <param name="value1">The first Quaternion rotation in the series.</param>
        /// <param name="value2">The second Quaternion rotation in the series.</param>
        /// <returns>A new Quaternion representing the concatenation of the value1 rotation followed by the value2 rotation.</returns>
        public static Quaternion Concatenate(Quaternion value1, Quaternion value2)
        {
            float q1w = value1.W;
            float q2w = value2.W;

            Vector3 vec1 = Unsafe.As<Quaternion, Vector3>(ref value1);
            Vector3 vec2 = Unsafe.As<Quaternion, Vector3>(ref value2);

            float dot = Vector3.Dot(vec1, vec2);

            Vector3 vectorPart = Vector3.Cross(vec2, vec1) + vec1 * q2w + vec2 * q1w;
            float scalarPart = q1w * q2w - dot;

            return new Quaternion(vectorPart, scalarPart);
        }

        /// <summary>
        /// Flips the sign of each component of the quaternion.
        /// </summary>
        /// <param name="value">The source Quaternion.</param>
        /// <returns>The negated Quaternion.</returns>
        public static Quaternion Negate(Quaternion value)
        {
            Vector4 q = Unsafe.As<Quaternion, Vector4>(ref value);
            Vector4 result = -q;

            return Unsafe.As<Vector4, Quaternion>(ref result);
        }

        /// <summary>
        /// Adds two Quaternions element-by-element.
        /// </summary>
        /// <param name="value1">The first source Quaternion.</param>
        /// <param name="value2">The second source Quaternion.</param>
        /// <returns>The result of adding the Quaternions.</returns>
        public static Quaternion Add(Quaternion value1, Quaternion value2)
        {
            Vector4 q1 = Unsafe.As<Quaternion, Vector4>(ref value1);
            Vector4 q2 = Unsafe.As<Quaternion, Vector4>(ref value2);

            Vector4 result = q1 + q2;

            return Unsafe.As<Vector4, Quaternion>(ref result);
        }

        /// <summary>
        /// Subtracts one Quaternion from another.
        /// </summary>
        /// <param name="value1">The first source Quaternion.</param>
        /// <param name="value2">The second Quaternion, to be subtracted from the first.</param>
        /// <returns>The result of the subtraction.</returns>
        public static Quaternion Subtract(Quaternion value1, Quaternion value2)
        {
            Vector4 q1 = Unsafe.As<Quaternion, Vector4>(ref value1);
            Vector4 q2 = Unsafe.As<Quaternion, Vector4>(ref value2);

            Vector4 result = q1 - q2;

            return Unsafe.As<Vector4, Quaternion>(ref result);
        }

        /// <summary>
        /// Multiplies two Quaternions together.
        /// </summary>
        /// <param name="value1">The Quaternion on the left side of the multiplication.</param>
        /// <param name="value2">The Quaternion on the right side of the multiplication.</param>
        /// <returns>The result of the multiplication.</returns>
        public static Quaternion Multiply(Quaternion value1, Quaternion value2)
        {
            float q1w = value1.W;
            float q2w = value2.W;

            Vector3 vec1 = Unsafe.As<Quaternion, Vector3>(ref value1);
            Vector3 vec2 = Unsafe.As<Quaternion, Vector3>(ref value2);

            float dot = Vector3.Dot(vec1, vec2);

            Vector3 vectorPart = Vector3.Cross(vec1, vec2) + vec1 * q2w + vec2 * q1w;
            float scalarPart = q1w * q2w - dot;

            return new Quaternion(vectorPart, scalarPart);
        }

        /// <summary>
        /// Multiplies a Quaternion by a scalar value.
        /// </summary>
        /// <param name="value1">The source Quaternion.</param>
        /// <param name="value2">The scalar value.</param>
        /// <returns>The result of the multiplication.</returns>
        public static Quaternion Multiply(Quaternion value1, float value2)
        {
            Vector4 q1 = Unsafe.As<Quaternion, Vector4>(ref value1);

            Vector4 result = q1 * value2;

            return Unsafe.As<Vector4, Quaternion>(ref result);
        }

        /// <summary>
        /// Divides a Quaternion by another Quaternion.
        /// </summary>
        /// <param name="value1">The source Quaternion.</param>
        /// <param name="value2">The divisor.</param>
        /// <returns>The result of the division.</returns>
        public static Quaternion Divide(Quaternion value1, Quaternion value2)
        {
            Vector4 q2 = Unsafe.As<Quaternion, Vector4>(ref value2);

            // Inverse part.
            float ls = Vector4.Dot(q2, q2);
            float invNorm = -1.0f / ls;
            Vector4 q = q2 * invNorm;
            float q2w = -q.W;

            // Multiply part.
            float q1w = value1.W;
            Vector3 vec1 = Unsafe.As<Quaternion, Vector3>(ref value1);
            Vector3 vec2 = Unsafe.As<Vector4, Vector3>(ref q);

            float dot = Vector3.Dot(vec1, vec2);

            Vector3 vectorPart = Vector3.Cross(vec1, vec2) + vec1 * q2w + vec2 * q1w;
            float scalarPart = q1w * q2w - dot;

            return new Quaternion(vectorPart, scalarPart);
        }

        /// <summary>
        /// Flips the sign of each component of the quaternion.
        /// </summary>
        /// <param name="value">The source Quaternion.</param>
        /// <returns>The negated Quaternion.</returns>
        public static Quaternion operator -(Quaternion value)
        {
            Vector4 q = Unsafe.As<Quaternion, Vector4>(ref value);
            Vector4 result = -q;

            return Unsafe.As<Vector4, Quaternion>(ref result);
        }

        /// <summary>
        /// Adds two Quaternions element-by-element.
        /// </summary>
        /// <param name="value1">The first source Quaternion.</param>
        /// <param name="value2">The second source Quaternion.</param>
        /// <returns>The result of adding the Quaternions.</returns>
        public static Quaternion operator +(Quaternion value1, Quaternion value2)
        {
            Vector4 q1 = Unsafe.As<Quaternion, Vector4>(ref value1);
            Vector4 q2 = Unsafe.As<Quaternion, Vector4>(ref value2);

            Vector4 result = q1 + q2;

            return Unsafe.As<Vector4, Quaternion>(ref result);
        }

        /// <summary>
        /// Subtracts one Quaternion from another.
        /// </summary>
        /// <param name="value1">The first source Quaternion.</param>
        /// <param name="value2">The second Quaternion, to be subtracted from the first.</param>
        /// <returns>The result of the subtraction.</returns>
        public static Quaternion operator -(Quaternion value1, Quaternion value2)
        {
            Vector4 q1 = Unsafe.As<Quaternion, Vector4>(ref value1);
            Vector4 q2 = Unsafe.As<Quaternion, Vector4>(ref value2);

            Vector4 result = q1 - q2;

            return Unsafe.As<Vector4, Quaternion>(ref result);
        }

        /// <summary>
        /// Multiplies two Quaternions together.
        /// </summary>
        /// <param name="value1">The Quaternion on the left side of the multiplication.</param>
        /// <param name="value2">The Quaternion on the right side of the multiplication.</param>
        /// <returns>The result of the multiplication.</returns>
        public static Quaternion operator *(Quaternion value1, Quaternion value2)
        {
            float q1w = value1.W;
            float q2w = value2.W;

            Vector3 vec1 = Unsafe.As<Quaternion, Vector3>(ref value1);
            Vector3 vec2 = Unsafe.As<Quaternion, Vector3>(ref value2);

            float dot = Vector3.Dot(vec1, vec2);

            Vector3 vectorPart = Vector3.Cross(vec1, vec2) + vec1 * q2w + vec2 * q1w;
            float scalarPart = q1w * q2w - dot;

            return new Quaternion(vectorPart, scalarPart);
        }

        /// <summary>
        /// Multiplies a Quaternion by a scalar value.
        /// </summary>
        /// <param name="value1">The source Quaternion.</param>
        /// <param name="value2">The scalar value.</param>
        /// <returns>The result of the multiplication.</returns>
        public static Quaternion operator *(Quaternion value1, float value2)
        {
            Vector4 q1 = Unsafe.As<Quaternion, Vector4>(ref value1);

            Vector4 result = q1 * value2;

            return Unsafe.As<Vector4, Quaternion>(ref result);
        }

        /// <summary>
        /// Divides a Quaternion by another Quaternion.
        /// </summary>
        /// <param name="value1">The source Quaternion.</param>
        /// <param name="value2">The divisor.</param>
        /// <returns>The result of the division.</returns>
        public static Quaternion operator /(Quaternion value1, Quaternion value2)
        {
            Vector4 q2 = Unsafe.As<Quaternion, Vector4>(ref value2);

            // Inverse part.
            float ls = Vector4.Dot(q2, q2);
            float invNorm = -1.0f / ls;
            Vector4 q = q2 * invNorm;
            float q2w = -q.W;

            // Multiply part.
            float q1w = value1.W;
            Vector3 vec1 = Unsafe.As<Quaternion, Vector3>(ref value1);
            Vector3 vec2 = Unsafe.As<Vector4, Vector3>(ref q);

            float dot = Vector3.Dot(vec1, vec2);

            Vector3 vectorPart = Vector3.Cross(vec1, vec2) + vec1 * q2w + vec2 * q1w;
            float scalarPart = q1w * q2w - dot;

            return new Quaternion(vectorPart, scalarPart);
        }

        /// <summary>
        /// Returns a boolean indicating whether the two given Quaternions are equal.
        /// </summary>
        /// <param name="value1">The first Quaternion to compare.</param>
        /// <param name="value2">The second Quaternion to compare.</param>
        /// <returns>True if the Quaternions are equal; False otherwise.</returns>
        public static bool operator ==(Quaternion value1, Quaternion value2)
        {
            Vector4 q1 = Unsafe.As<Quaternion, Vector4>(ref value1);
            Vector4 q2 = Unsafe.As<Quaternion, Vector4>(ref value2);
            return q1 == q2;
        }

        /// <summary>
        /// Returns a boolean indicating whether the two given Quaternions are not equal.
        /// </summary>
        /// <param name="value1">The first Quaternion to compare.</param>
        /// <param name="value2">The second Quaternion to compare.</param>
        /// <returns>True if the Quaternions are not equal; False if they are equal.</returns>
        public static bool operator !=(Quaternion value1, Quaternion value2)
        {
            Vector4 q1 = Unsafe.As<Quaternion, Vector4>(ref value1);
            Vector4 q2 = Unsafe.As<Quaternion, Vector4>(ref value2);
            return q1 != q2;
        }

        /// <summary>
        /// Returns a boolean indicating whether the given Quaternion is equal to this Quaternion instance.
        /// </summary>
        /// <param name="other">The Quaternion to compare this instance to.</param>
        /// <returns>True if the other Quaternion is equal to this instance; False otherwise.</returns>
        public bool Equals(Quaternion other)
        {
            Vector4 q1 = Unsafe.As<Quaternion, Vector4>(ref this);
            Vector4 q2 = Unsafe.As<Quaternion, Vector4>(ref other);
            return q1 == q2;
        }

        /// <summary>
        /// Returns a boolean indicating whether the given Object is equal to this Quaternion instance.
        /// </summary>
        /// <param name="obj">The Object to compare against.</param>
        /// <returns>True if the Object is equal to this Quaternion; False otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Quaternion)
            {
                return Equals((Quaternion)obj);
            }

            return false;
        }

        /// <summary>
        /// Returns a String representing this Quaternion instance.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            CultureInfo ci = CultureInfo.CurrentCulture;

            return String.Format(ci, "{{X:{0} Y:{1} Z:{2} W:{3}}}", X.ToString(ci), Y.ToString(ci), Z.ToString(ci), W.ToString(ci));
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return unchecked(X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode() + W.GetHashCode());
        }
    }
}
