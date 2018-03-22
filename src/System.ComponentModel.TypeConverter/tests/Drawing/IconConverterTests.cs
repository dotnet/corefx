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
        private readonly Icon _icon = null;
        private readonly IconConverter _icoConv = null;
        private readonly IconConverter _icoConvFrmTD = null;
        private readonly string _iconStr = null;
        private readonly byte[] _iconBytes = null;

        public IconConverterTest()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            Stream testIconStream = assembly.GetManifestResourceStream("Resources.VisualPng.ico");

            int length = (int)testIconStream.Length;
            _iconBytes = new byte[length];
            if (testIconStream.Read(_iconBytes, 0, length) != length)
            {
                throw new InvalidOperationException("Failed to load resource image.");
            }

            testIconStream.Position = 0;
            _icon = new Icon(testIconStream);
            _iconStr = _icon.ToString();

            _icoConv = new IconConverter();
            _icoConvFrmTD = (IconConverter)TypeDescriptor.GetConverter(_icon);
        }

        [Fact]
        public void TestCanConvertFrom()
        {
            Assert.True(_icoConv.CanConvertFrom(typeof(byte[])), "CCF#1");
            Assert.True(_icoConv.CanConvertFrom(null, typeof(byte[])), "CCF#1a");
            Assert.True(_icoConv.CanConvertFrom(null, _iconBytes.GetType()), "CCF#1b");
            Assert.True(!_icoConv.CanConvertFrom(null, typeof(string)), "CCF#2");
            Assert.True(!_icoConv.CanConvertFrom(null, typeof(Rectangle)), "CCF#3");
            Assert.True(!_icoConv.CanConvertFrom(null, typeof(Point)), "CCF#4");
            Assert.True(!_icoConv.CanConvertFrom(null, typeof(PointF)), "CCF#5");
            Assert.True(!_icoConv.CanConvertFrom(null, typeof(Size)), "CCF#6");
            Assert.True(!_icoConv.CanConvertFrom(null, typeof(SizeF)), "CCF#7");
            Assert.True(!_icoConv.CanConvertFrom(null, typeof(object)), "CCF#8");
            Assert.True(!_icoConv.CanConvertFrom(null, typeof(int)), "CCF#9");
            Assert.True(!_icoConv.CanConvertFrom(null, typeof(Metafile)), "CCF#10");

            Assert.True(_icoConvFrmTD.CanConvertFrom(typeof(byte[])), "CCF#1A");
            Assert.True(_icoConvFrmTD.CanConvertFrom(null, typeof(byte[])), "CCF#1aA");
            Assert.True(_icoConvFrmTD.CanConvertFrom(null, _iconBytes.GetType()), "CCF#1bA");
            Assert.True(!_icoConvFrmTD.CanConvertFrom(null, typeof(string)), "CCF#2A");
            Assert.True(!_icoConvFrmTD.CanConvertFrom(null, typeof(Rectangle)), "CCF#3A");
            Assert.True(!_icoConvFrmTD.CanConvertFrom(null, typeof(Point)), "CCF#4A");
            Assert.True(!_icoConvFrmTD.CanConvertFrom(null, typeof(PointF)), "CCF#5A");
            Assert.True(!_icoConvFrmTD.CanConvertFrom(null, typeof(Size)), "CCF#6A");
            Assert.True(!_icoConvFrmTD.CanConvertFrom(null, typeof(SizeF)), "CCF#7A");
            Assert.True(!_icoConvFrmTD.CanConvertFrom(null, typeof(object)), "CCF#8A");
            Assert.True(!_icoConvFrmTD.CanConvertFrom(null, typeof(int)), "CCF#9A");
            Assert.True(!_icoConvFrmTD.CanConvertFrom(null, typeof(Metafile)), "CCF#10A");
        }

        [Fact]
        public void TestCanConvertTo()
        {
            Assert.True(_icoConv.CanConvertTo(typeof(string)), "CCT#1");
            Assert.True(_icoConv.CanConvertTo(null, typeof(string)), "CCT#1a");
            Assert.True(_icoConv.CanConvertTo(null, _iconStr.GetType()), "CCT#1b");
            Assert.True(_icoConv.CanConvertTo(typeof(byte[])), "CCT#2");
            Assert.True(_icoConv.CanConvertTo(null, typeof(byte[])), "CCT#2a");
            Assert.True(_icoConv.CanConvertTo(null, _iconBytes.GetType()), "CCT#2b");
            Assert.True(!_icoConv.CanConvertTo(null, typeof(Rectangle)), "CCT#3");
            Assert.True(!_icoConv.CanConvertTo(null, typeof(Point)), "CCT#4");
            Assert.True(!_icoConv.CanConvertTo(null, typeof(PointF)), "CCT#5");
            Assert.True(!_icoConv.CanConvertTo(null, typeof(Size)), "CCT#6");
            Assert.True(!_icoConv.CanConvertTo(null, typeof(SizeF)), "CCT#7");
            Assert.True(!_icoConv.CanConvertTo(null, typeof(object)), "CCT#8");
            Assert.True(!_icoConv.CanConvertTo(null, typeof(int)), "CCT#9");

            Assert.True(_icoConvFrmTD.CanConvertTo(typeof(string)), "CCT#1A");
            Assert.True(_icoConvFrmTD.CanConvertTo(null, typeof(string)), "CCT#1aA");
            Assert.True(_icoConvFrmTD.CanConvertTo(null, _iconStr.GetType()), "CCT#1bA");
            Assert.True(_icoConvFrmTD.CanConvertTo(typeof(byte[])), "CCT#2A");
            Assert.True(_icoConvFrmTD.CanConvertTo(null, typeof(byte[])), "CCT#2aA");
            Assert.True(_icoConvFrmTD.CanConvertTo(null, _iconBytes.GetType()), "CCT#2bA");
            Assert.True(!_icoConvFrmTD.CanConvertTo(null, typeof(Rectangle)), "CCT#3A");
            Assert.True(!_icoConvFrmTD.CanConvertTo(null, typeof(Point)), "CCT#4A");
            Assert.True(!_icoConvFrmTD.CanConvertTo(null, typeof(PointF)), "CCT#5A");
            Assert.True(!_icoConvFrmTD.CanConvertTo(null, typeof(Size)), "CCT#6A");
            Assert.True(!_icoConvFrmTD.CanConvertTo(null, typeof(SizeF)), "CCT#7A");
            Assert.True(!_icoConvFrmTD.CanConvertTo(null, typeof(object)), "CCT#8A");
            Assert.True(!_icoConvFrmTD.CanConvertTo(null, typeof(int)), "CCT#9A");
        }

        [Fact]
        public void TestConvertFrom()
        {
            Icon newIcon = (Icon)_icoConv.ConvertFrom(null, CultureInfo.InvariantCulture, _iconBytes);

            Assert.Equal(_icon.Height, newIcon.Height);
            Assert.Equal(_icon.Width, newIcon.Width);

            Assert.Throws<NotSupportedException>(() => _icoConv.ConvertFrom("System.Drawing.String"));
            Assert.Throws<NotSupportedException>(() => _icoConv.ConvertFrom(null, CultureInfo.InvariantCulture, "System.Drawing.String"));
            Assert.Throws<NotSupportedException>(() => _icoConv.ConvertFrom(null, CultureInfo.InvariantCulture, new Bitmap(20, 20)));
            Assert.Throws<NotSupportedException>(() => _icoConv.ConvertFrom(null, CultureInfo.InvariantCulture, new Point(10, 10)));
            Assert.Throws<NotSupportedException>(() => _icoConv.ConvertFrom(null, CultureInfo.InvariantCulture, new SizeF(10, 10)));
            Assert.Throws<NotSupportedException>(() => _icoConv.ConvertFrom(null, CultureInfo.InvariantCulture, new object()));


            newIcon = (Icon)_icoConvFrmTD.ConvertFrom(null, CultureInfo.InvariantCulture, _iconBytes);

            Assert.Equal(_icon.Height, newIcon.Height);
            Assert.Equal(_icon.Width, newIcon.Width);

            Assert.Throws<NotSupportedException>(() => _icoConvFrmTD.ConvertFrom("System.Drawing.String"));
            Assert.Throws<NotSupportedException>(() => _icoConvFrmTD.ConvertFrom(null, CultureInfo.InvariantCulture, "System.Drawing.String"));
            Assert.Throws<NotSupportedException>(() => _icoConvFrmTD.ConvertFrom(null, CultureInfo.InvariantCulture, new Bitmap(20, 20)));
            Assert.Throws<NotSupportedException>(() => _icoConvFrmTD.ConvertFrom(null, CultureInfo.InvariantCulture, new Point(10, 10)));
            Assert.Throws<NotSupportedException>(() => _icoConvFrmTD.ConvertFrom(null, CultureInfo.InvariantCulture, new SizeF(10, 10)));
            Assert.Throws<NotSupportedException>(() => _icoConvFrmTD.ConvertFrom(null, CultureInfo.InvariantCulture, new object()));
        }

        [Fact]
        public void TestConvertTo()
        {
            Assert.Equal(_iconStr, (string)_icoConv.ConvertTo(null, CultureInfo.InvariantCulture, _icon, typeof(string)));
            Assert.Equal(_iconStr, (string)_icoConv.ConvertTo(_icon, typeof(string)));

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

            Assert.Equal(_iconStr, (string)_icoConvFrmTD.ConvertTo(null, CultureInfo.InvariantCulture, _icon, typeof(string)));
            Assert.Equal(_iconStr, (string)_icoConvFrmTD.ConvertTo(_icon, typeof(string)));

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

            Assert.Equal("(none)", (string)_icoConv.ConvertTo(null, typeof(string)));
        }
    }
}
