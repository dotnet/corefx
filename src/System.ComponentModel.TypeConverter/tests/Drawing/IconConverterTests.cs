// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using Xunit;

namespace System.ComponentModel.TypeConverterTests
{
    public class IconConverterTest
    {
        Icon icon = null;
        IconConverter icoConv = null;
        IconConverter icoConvFrmTD = null;
        string iconStr = null;
        byte[] iconBytes = null;

        public IconConverterTest()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            icon = new Icon(assembly.GetManifestResourceStream("Resources.VisualPng.ico"));
            iconStr = icon.ToString();

            icoConv = new IconConverter();
            icoConvFrmTD = (IconConverter)TypeDescriptor.GetConverter(icon);

            using (Stream stream = assembly.GetManifestResourceStream("Resources.VisualPng1.ico"))
            {
                int length = (int)stream.Length;
                iconBytes = new byte[length];
                if (stream.Read(iconBytes, 0, length) != length)
                {
                    throw new InvalidOperationException("Failed to load resource image.");
                }
            }
        }

        [Fact]
        public void TestCanConvertFrom()
        {
            Assert.True(icoConv.CanConvertFrom(typeof(byte[])), "CCF#1");
            Assert.True(icoConv.CanConvertFrom(null, typeof(byte[])), "CCF#1a");
            Assert.True(icoConv.CanConvertFrom(null, iconBytes.GetType()), "CCF#1b");
            Assert.True(!icoConv.CanConvertFrom(null, typeof(string)), "CCF#2");
            Assert.True(!icoConv.CanConvertFrom(null, typeof(Rectangle)), "CCF#3");
            Assert.True(!icoConv.CanConvertFrom(null, typeof(Point)), "CCF#4");
            Assert.True(!icoConv.CanConvertFrom(null, typeof(PointF)), "CCF#5");
            Assert.True(!icoConv.CanConvertFrom(null, typeof(Size)), "CCF#6");
            Assert.True(!icoConv.CanConvertFrom(null, typeof(SizeF)), "CCF#7");
            Assert.True(!icoConv.CanConvertFrom(null, typeof(object)), "CCF#8");
            Assert.True(!icoConv.CanConvertFrom(null, typeof(int)), "CCF#9");
            Assert.True(!icoConv.CanConvertFrom(null, typeof(Metafile)), "CCF#10");

            Assert.True(icoConvFrmTD.CanConvertFrom(typeof(byte[])), "CCF#1A");
            Assert.True(icoConvFrmTD.CanConvertFrom(null, typeof(byte[])), "CCF#1aA");
            Assert.True(icoConvFrmTD.CanConvertFrom(null, iconBytes.GetType()), "CCF#1bA");
            Assert.True(!icoConvFrmTD.CanConvertFrom(null, typeof(string)), "CCF#2A");
            Assert.True(!icoConvFrmTD.CanConvertFrom(null, typeof(Rectangle)), "CCF#3A");
            Assert.True(!icoConvFrmTD.CanConvertFrom(null, typeof(Point)), "CCF#4A");
            Assert.True(!icoConvFrmTD.CanConvertFrom(null, typeof(PointF)), "CCF#5A");
            Assert.True(!icoConvFrmTD.CanConvertFrom(null, typeof(Size)), "CCF#6A");
            Assert.True(!icoConvFrmTD.CanConvertFrom(null, typeof(SizeF)), "CCF#7A");
            Assert.True(!icoConvFrmTD.CanConvertFrom(null, typeof(object)), "CCF#8A");
            Assert.True(!icoConvFrmTD.CanConvertFrom(null, typeof(int)), "CCF#9A");
            Assert.True(!icoConvFrmTD.CanConvertFrom(null, typeof(Metafile)), "CCF#10A");
        }

        [Fact]
        public void TestCanConvertTo()
        {
            Assert.True(icoConv.CanConvertTo(typeof(string)), "CCT#1");
            Assert.True(icoConv.CanConvertTo(null, typeof(string)), "CCT#1a");
            Assert.True(icoConv.CanConvertTo(null, iconStr.GetType()), "CCT#1b");
            Assert.True(icoConv.CanConvertTo(typeof(byte[])), "CCT#2");
            Assert.True(icoConv.CanConvertTo(null, typeof(byte[])), "CCT#2a");
            Assert.True(icoConv.CanConvertTo(null, iconBytes.GetType()), "CCT#2b");
            Assert.True(!icoConv.CanConvertTo(null, typeof(Rectangle)), "CCT#3");
            Assert.True(!icoConv.CanConvertTo(null, typeof(Point)), "CCT#4");
            Assert.True(!icoConv.CanConvertTo(null, typeof(PointF)), "CCT#5");
            Assert.True(!icoConv.CanConvertTo(null, typeof(Size)), "CCT#6");
            Assert.True(!icoConv.CanConvertTo(null, typeof(SizeF)), "CCT#7");
            Assert.True(!icoConv.CanConvertTo(null, typeof(object)), "CCT#8");
            Assert.True(!icoConv.CanConvertTo(null, typeof(int)), "CCT#9");

            Assert.True(icoConvFrmTD.CanConvertTo(typeof(string)), "CCT#1A");
            Assert.True(icoConvFrmTD.CanConvertTo(null, typeof(string)), "CCT#1aA");
            Assert.True(icoConvFrmTD.CanConvertTo(null, iconStr.GetType()), "CCT#1bA");
            Assert.True(icoConvFrmTD.CanConvertTo(typeof(byte[])), "CCT#2A");
            Assert.True(icoConvFrmTD.CanConvertTo(null, typeof(byte[])), "CCT#2aA");
            Assert.True(icoConvFrmTD.CanConvertTo(null, iconBytes.GetType()), "CCT#2bA");
            Assert.True(!icoConvFrmTD.CanConvertTo(null, typeof(Rectangle)), "CCT#3A");
            Assert.True(!icoConvFrmTD.CanConvertTo(null, typeof(Point)), "CCT#4A");
            Assert.True(!icoConvFrmTD.CanConvertTo(null, typeof(PointF)), "CCT#5A");
            Assert.True(!icoConvFrmTD.CanConvertTo(null, typeof(Size)), "CCT#6A");
            Assert.True(!icoConvFrmTD.CanConvertTo(null, typeof(SizeF)), "CCT#7A");
            Assert.True(!icoConvFrmTD.CanConvertTo(null, typeof(object)), "CCT#8A");
            Assert.True(!icoConvFrmTD.CanConvertTo(null, typeof(int)), "CCT#9A");
        }

        [Fact]
        public void TestConvertFrom()
        {
            Icon newIcon = (Icon)icoConv.ConvertFrom(null, CultureInfo.InvariantCulture, iconBytes);

            Assert.Equal(icon.Height, newIcon.Height);
            Assert.Equal(icon.Width, newIcon.Width);

            Assert.Throws<NotSupportedException>(() => icoConv.ConvertFrom("System.Drawing.String"));
            Assert.Throws<NotSupportedException>(() => icoConv.ConvertFrom(null, CultureInfo.InvariantCulture, "System.Drawing.String"));
            Assert.Throws<NotSupportedException>(() => icoConv.ConvertFrom(null, CultureInfo.InvariantCulture, new Bitmap(20, 20)));
            Assert.Throws<NotSupportedException>(() => icoConv.ConvertFrom(null, CultureInfo.InvariantCulture, new Point(10, 10)));
            Assert.Throws<NotSupportedException>(() => icoConv.ConvertFrom(null, CultureInfo.InvariantCulture, new SizeF(10, 10)));
            Assert.Throws<NotSupportedException>(() => icoConv.ConvertFrom(null, CultureInfo.InvariantCulture, new object()));


            newIcon = (Icon)icoConvFrmTD.ConvertFrom(null, CultureInfo.InvariantCulture, iconBytes);

            Assert.Equal(icon.Height, newIcon.Height);
            Assert.Equal(icon.Width, newIcon.Width);

            Assert.Throws<NotSupportedException>(() => icoConvFrmTD.ConvertFrom("System.Drawing.String"));
            Assert.Throws<NotSupportedException>(() => icoConvFrmTD.ConvertFrom(null, CultureInfo.InvariantCulture, "System.Drawing.String"));
            Assert.Throws<NotSupportedException>(() => icoConvFrmTD.ConvertFrom(null, CultureInfo.InvariantCulture, new Bitmap(20, 20)));
            Assert.Throws<NotSupportedException>(() => icoConvFrmTD.ConvertFrom(null, CultureInfo.InvariantCulture, new Point(10, 10)));
            Assert.Throws<NotSupportedException>(() => icoConvFrmTD.ConvertFrom(null, CultureInfo.InvariantCulture, new SizeF(10, 10)));
            Assert.Throws<NotSupportedException>(() => icoConvFrmTD.ConvertFrom(null, CultureInfo.InvariantCulture, new object()));
        }

        [Fact]
        public void TestConvertTo()
        {
            Assert.Equal(iconStr, (string)icoConv.ConvertTo(null, CultureInfo.InvariantCulture, icon, typeof(string)));
            Assert.Equal(iconStr, (string)icoConv.ConvertTo(icon, typeof(string)));

            /*byte [] newIconBytes = (byte []) icoConv.ConvertTo (null, CultureInfo.InvariantCulture,
											icon, iconBytes.GetType ());

			Assert.AreEqual (iconBytes.Length, newIconBytes.Length, "CT#2");

			newIconBytes = (byte []) icoConv.ConvertTo (icon, iconBytes.GetType ());

			Assert.AreEqual (iconBytes.Length, newIconBytes.Length, "CT#2a");


			try {
				icoConv.ConvertTo (null, CultureInfo.InvariantCulture,
						 icon, typeof (Rectangle));
				Assert.Fail ("CT#3: must throw NotSupportedException");
			} catch (Exception e) {
				Assert.True ( e is NotSupportedException, "CT#3");
			}

			try {
				icoConv.ConvertTo (null, CultureInfo.InvariantCulture,
						 icon, icon.GetType ());
				Assert.Fail ("CT#4: must throw NotSupportedException");
			} catch (Exception e) {
				Assert.True (e is NotSupportedException, "CT#4");
			}

			try {
				icoConv.ConvertTo (null, CultureInfo.InvariantCulture,
						 icon, typeof (Size));
				Assert.Fail ("CT#5: must throw NotSupportedException");
			} catch (Exception e) {
				Assert.True (e is NotSupportedException, "CT#5");
			}

			try {
				icoConv.ConvertTo (null, CultureInfo.InvariantCulture,
						 icon, typeof (Bitmap));
				Assert.Fail ("CT#6: must throw NotSupportedException");
			} catch (Exception e) {
				Assert.True ( e is NotSupportedException, "CT#6");
			}

			try {
				icoConv.ConvertTo (null, CultureInfo.InvariantCulture,
						 icon, typeof (Point));
				Assert.Fail ("CT#7: must throw NotSupportedException");
			} catch (Exception e) {
				Assert.True (e is NotSupportedException, "CT#7");
			}

			try {
				icoConv.ConvertTo (null, CultureInfo.InvariantCulture,
						 icon, typeof (Metafile));
				Assert.Fail ("CT#8: must throw NotSupportedException");
			} catch (Exception e) {
				Assert.True (e is NotSupportedException, "CT#8");
			}

			try {
				icoConv.ConvertTo (null, CultureInfo.InvariantCulture,
						 icon, typeof (object));
				Assert.Fail ("CT#9: must throw NotSupportedException");
			} catch (Exception e) {
				Assert.True (e is NotSupportedException, "CT#9");
			}

			try {
				icoConv.ConvertTo (null, CultureInfo.InvariantCulture,
						 icon, typeof (int));
				Assert.Fail ("CT#10: must throw NotSupportedException");
			} catch (Exception e) {
				Assert.True (e is NotSupportedException, "CT#10");
			}*/

            Assert.Equal(iconStr, (string)icoConvFrmTD.ConvertTo(null, CultureInfo.InvariantCulture, icon, typeof(string)));
            Assert.Equal(iconStr, (string)icoConvFrmTD.ConvertTo(icon, typeof(string)));

            /*newIconBytes = (byte []) icoConvFrmTD.ConvertTo (null, CultureInfo.InvariantCulture,
											icon, iconBytes.GetType ());

			Assert.AreEqual (iconBytes.Length, newIconBytes.Length, "CT#2A");

			newIconBytes = (byte []) icoConvFrmTD.ConvertTo (icon, iconBytes.GetType ());

			Assert.AreEqual (iconBytes.Length, newIconBytes.Length, "CT#2aA");

			try {
				icoConvFrmTD.ConvertTo (null, CultureInfo.InvariantCulture,
						 icon, typeof (Rectangle));
				Assert.Fail ("CT#3A: must throw NotSupportedException");
			} catch (Exception e) {
				Assert.True (e is NotSupportedException, "CT#3A");
			}

			try {
				icoConvFrmTD.ConvertTo (null, CultureInfo.InvariantCulture,
						 icon, icon.GetType ());
				Assert.Fail ("CT#4A: must throw NotSupportedException");
			} catch (Exception e) {
				Assert.True (e is NotSupportedException, "CT#4A");
			}

			try {
				icoConvFrmTD.ConvertTo (null, CultureInfo.InvariantCulture,
						 icon, typeof (Size));
				Assert.Fail ("CT#5A: must throw NotSupportedException");
			} catch (Exception e) {
				Assert.True (e is NotSupportedException, "CT#5A");
			}

			try {
				icoConvFrmTD.ConvertTo (null, CultureInfo.InvariantCulture,
						 icon, typeof (Bitmap));
				Assert.Fail ("CT#6A: must throw NotSupportedException");
			} catch (Exception e) {
				Assert.True (e is NotSupportedException, "CT#6A");
			}

			try {
				icoConvFrmTD.ConvertTo (null, CultureInfo.InvariantCulture,
						 icon, typeof (Point));
				Assert.Fail ("CT#7A: must throw NotSupportedException");
			} catch (Exception e) {
				Assert.True (e is NotSupportedException, "CT#7A");
			}

			try {
				icoConvFrmTD.ConvertTo (null, CultureInfo.InvariantCulture,
						 icon, typeof (Metafile));
				Assert.Fail ("CT#8A: must throw NotSupportedException");
			} catch (Exception e) {
				Assert.True (e is NotSupportedException, "CT#8A");
			}

			try {
				icoConvFrmTD.ConvertTo (null, CultureInfo.InvariantCulture,
						 icon, typeof (object));
				Assert.Fail ("CT#9A: must throw NotSupportedException");
			} catch (Exception e) {
				Assert.True (e is NotSupportedException, "CT#9A");
			}

			try {
				icoConvFrmTD.ConvertTo (null, CultureInfo.InvariantCulture,
						 icon, typeof (int));
				Assert.Fail ("CT#10A: must throw NotSupportedException");
			} catch (Exception e) {
				Assert.True (e is NotSupportedException, "CT#10A");
			}*/

            Assert.Equal("(none)", (string)icoConv.ConvertTo(null, typeof(string)));
        }
    }
}
