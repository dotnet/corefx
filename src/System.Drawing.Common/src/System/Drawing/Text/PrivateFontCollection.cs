// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Globalization;

namespace System.Drawing.Text
{
    /// <summary>
    /// Encapsulates a collection of <see cref='System.Drawing.Font'/> objecs.
    /// </summary>
    public sealed class PrivateFontCollection : FontCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref='System.Drawing.Text.PrivateFontCollection'/> class.
        /// </summary>
        public PrivateFontCollection() : base()
        {
            int status = SafeNativeMethods.Gdip.GdipNewPrivateFontCollection(out _nativeFontCollection);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }

        /// <summary>
        /// Cleans up Windows resources for this <see cref='System.Drawing.Text.PrivateFontCollection'/>.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (_nativeFontCollection != IntPtr.Zero)
            {
                try
                {
#if DEBUG
                    int status =
#endif
                    SafeNativeMethods.Gdip.GdipDeletePrivateFontCollection(out _nativeFontCollection);
#if DEBUG
                    Debug.Assert(status == SafeNativeMethods.Gdip.Ok, "GDI+ returned an error status: " + status.ToString(CultureInfo.InvariantCulture));
#endif        
                }
                catch (Exception ex) when (!ClientUtils.IsSecurityOrCriticalException(ex))
                {
                }
                finally
                {
                    _nativeFontCollection = IntPtr.Zero;
                }
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Adds a font from the specified file to this <see cref='System.Drawing.Text.PrivateFontCollection'/>.
        /// </summary>
        public void AddFontFile(string filename)
        {
            int status = SafeNativeMethods.Gdip.GdipPrivateAddFontFile(new HandleRef(this, _nativeFontCollection), filename);
            SafeNativeMethods.Gdip.CheckStatus(status);

            // Register private font with GDI as well so pure GDI-based controls (TextBox, Button for instance) can access it.
            SafeNativeMethods.AddFontFile(filename);
        }

        /// <summary>
        /// Adds a font contained in system memory to this <see cref='System.Drawing.Text.PrivateFontCollection'/>.
        /// </summary>
        public void AddMemoryFont(IntPtr memory, int length)
        {
            int status = SafeNativeMethods.Gdip.GdipPrivateAddMemoryFont(new HandleRef(this, _nativeFontCollection), new HandleRef(null, memory), length);
            SafeNativeMethods.Gdip.CheckStatus(status);
        }
    }
}
