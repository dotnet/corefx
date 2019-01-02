@echo off

echo.
echo Generating writers..
echo.

TextTransform.exe -a '!!Classname!XmlUtf8RawTextWriter' XmlRawTextWriterGenerator.t4 -out XmlUtf8RawTextWriter.cs
TextTransform.exe -a '!!Classname!XmlEncodedRawTextWriter' XmlRawTextWriterGenerator.t4 -out XmlEncodedRawTextWriter.cs

popd
