// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace System.Drawing
{
    public static partial class SystemFonts
    {
        private unsafe static bool GetNonClientMetrics(out NativeMethods.NONCLIENTMETRICS metrics)
        {
            metrics = new NativeMethods.NONCLIENTMETRICS { cbSize = (uint)sizeof(NativeMethods.NONCLIENTMETRICS) };
            fixed (void* m = &metrics)
            {
                return UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETNONCLIENTMETRICS, metrics.cbSize, m, 0);
            }
        }

        public static Font CaptionFont
        {
            get
            {
                Font captionFont = null;

                if (GetNonClientMetrics(out NativeMethods.NONCLIENTMETRICS metrics))
                {
                    captionFont = GetFontFromData(metrics.lfCaptionFont);
                    captionFont.SetSystemFontName(nameof(CaptionFont));
                }

                return captionFont;
            }
        }

        public static Font SmallCaptionFont
        {
            get
            {
                Font smcaptionFont = null;

                if (GetNonClientMetrics(out NativeMethods.NONCLIENTMETRICS metrics))
                {
                    smcaptionFont = GetFontFromData(metrics.lfSmCaptionFont);
                    smcaptionFont.SetSystemFontName(nameof(SmallCaptionFont));
                }

                return smcaptionFont;
            }
        }

        public static Font MenuFont
        {
            get
            {
                Font menuFont = null;

                if (GetNonClientMetrics(out NativeMethods.NONCLIENTMETRICS metrics))
                {
                    menuFont = GetFontFromData(metrics.lfMenuFont);
                    menuFont.SetSystemFontName(nameof(MenuFont));
                }

                return menuFont;
            }
        }

        public static Font StatusFont
        {
            get
            {
                Font statusFont = null;

                if (GetNonClientMetrics(out NativeMethods.NONCLIENTMETRICS metrics))
                {
                    statusFont = GetFontFromData(metrics.lfStatusFont);
                    statusFont.SetSystemFontName(nameof(StatusFont));
                }

                return statusFont;
            }
        }

        public static Font MessageBoxFont
        {
            get
            {
                Font messageBoxFont = null;

                if (GetNonClientMetrics(out NativeMethods.NONCLIENTMETRICS metrics))
                {
                    messageBoxFont = GetFontFromData(metrics.lfMessageFont);
                    messageBoxFont.SetSystemFontName(nameof(MessageBoxFont));
                }

                return messageBoxFont;
            }
        }

        private static bool IsCriticalFontException(Exception ex)
        {
            return !(
                // In any of these cases we'll handle the exception.
                ex is ExternalException ||
                ex is ArgumentException ||
                ex is OutOfMemoryException || // GDI+ throws this one for many reasons other than actual OOM.
                ex is InvalidOperationException ||
                ex is NotImplementedException ||
                ex is FileNotFoundException);
        }

        public static unsafe Font IconTitleFont
        {
            get
            {
                Font iconTitleFont = null;

                var itfont = new SafeNativeMethods.LOGFONT();
                if (UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETICONTITLELOGFONT, (uint)sizeof(SafeNativeMethods.LOGFONT), &itfont, 0))
                {
                    iconTitleFont = GetFontFromData(itfont);
                    iconTitleFont.SetSystemFontName(nameof(IconTitleFont));
                }

                return iconTitleFont;
            }
        }

        public static Font DefaultFont
        {
            get
            {
                Font defaultFont = null;

                // For Arabic systems, always return Tahoma 8.
                if ((ushort)UnsafeNativeMethods.GetSystemDefaultLCID() == 0x0001)
                {
                    try
                    {
                        defaultFont = new Font("Tahoma", 8);
                    }
                    catch (Exception ex) when (!IsCriticalFontException(ex)) { }
                }
    
                // First try DEFAULT_GUI.
                if (defaultFont == null)
                {
                    IntPtr handle = UnsafeNativeMethods.GetStockObject(NativeMethods.DEFAULT_GUI_FONT);
                    try
                    {
                        using (Font fontInWorldUnits = Font.FromHfont(handle))
                        {
                            defaultFont = FontInPoints(fontInWorldUnits);
                        }
                    }
                    catch (ArgumentException)
                    {
                        // This can happen in theory if we end up pulling a non-TrueType font
                    }
                }

                // If DEFAULT_GUI didn't work, try Tahoma.
                if (defaultFont == null)
                {
                    try
                    {
                        defaultFont = new Font("Tahoma", 8);
                    }
                    catch (ArgumentException)
                    {
                    }
                }

                // Use GenericSansSerif as a last resort - this will always work.
                if (defaultFont == null)
                {
                    defaultFont = new Font(FontFamily.GenericSansSerif, 8);
                }

                if (defaultFont.Unit != GraphicsUnit.Point)
                {
                    defaultFont = FontInPoints(defaultFont);
                }

                Debug.Assert(defaultFont != null, "defaultFont wasn't set.");

                defaultFont.SetSystemFontName(nameof(DefaultFont));
                return defaultFont;
            }
        }

        public static Font DialogFont
        {
            get
            {
                Font dialogFont = null;

                if ((ushort)UnsafeNativeMethods.GetSystemDefaultLCID() == 0x0011)
                {
                    // Always return DefaultFont for Japanese cultures.
                    dialogFont = DefaultFont;
                }
                else
                {
                    try
                    {
                        // Use MS Shell Dlg 2, 8pt for anything other than Japanese.
                        dialogFont = new Font("MS Shell Dlg 2", 8);
                    }
                    catch (ArgumentException)
                    {
                        // This can happen in theory if we end up pulling a non-TrueType font
                    }
                }

                if (dialogFont == null)
                {
                    dialogFont = DefaultFont;
                }
                else if (dialogFont.Unit != GraphicsUnit.Point)
                {
                    dialogFont = FontInPoints(dialogFont);
                }

                // For Japanese cultures, SystemFonts.DefaultFont returns a new Font object every time it is invoked.
                // So for Japanese we return the DefaultFont with its SystemFontName set to DialogFont.
                dialogFont.SetSystemFontName(nameof(DialogFont));
                return dialogFont;
            }
        }

        private static Font FontInPoints(Font font)
        {
            return new Font(font.FontFamily, font.SizeInPoints, font.Style, GraphicsUnit.Point, font.GdiCharSet, font.GdiVerticalFont);
        }

        private static Font GetFontFromData(SafeNativeMethods.LOGFONT logFont)
        {
            Font font = null;
            try
            {
                font = Font.FromLogFont(ref logFont);
            }
            catch (Exception ex) when (!IsCriticalFontException(ex)) { }

            return
                font == null ? DefaultFont :
                font.Unit != GraphicsUnit.Point ? FontInPoints(font) :
                font;
        }
    }
}
