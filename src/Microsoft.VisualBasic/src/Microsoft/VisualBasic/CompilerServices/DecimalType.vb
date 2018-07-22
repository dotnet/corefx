' Copyright (c) Microsoft Corporation.  All rights reserved.

Imports System
Imports System.Globalization

Imports Microsoft.VisualBasic.CompilerServices.ExceptionUtils
Imports Microsoft.VisualBasic.CompilerServices.Utils

Namespace Microsoft.VisualBasic.CompilerServices

#Region " BACKWARDS COMPATIBILITY "

    'WARNING WARNING WARNING WARNING WARNING
    'This code exists to support Everett compiled applications.  Make sure you understand
    'the backwards compatibility ramifications of any edit you make in this region.
    'WARNING WARNING WARNING WARNING WARNING

    <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)>
    Public NotInheritable Class DecimalType
        ' Prevent creation.
        Private Sub New()
        End Sub

        Public Shared Function FromBoolean(ByVal Value As Boolean) As Decimal
            If Value Then
                Return -1D
            Else
                Return 0D
            End If
        End Function

        Public Shared Function FromString(ByVal Value As String) As Decimal
            Return FromString(Value, Nothing)
        End Function

        Public Shared Function FromString(ByVal Value As String, ByVal NumberFormat As NumberFormatInfo) As Decimal
            If Value Is Nothing Then
                Return 0D
            End If

            Try
                Dim i64Value As Int64

                If IsHexOrOctValue(Value, i64Value) Then
                    Return CDec(i64Value)
                End If

                Return Parse(Value, NumberFormat)

            Catch e1 As OverflowException
                Throw VbMakeException(vbErrors.Overflow)
            Catch e2 As FormatException
                Throw New InvalidCastException(GetResourceString(ResId.InvalidCast_FromStringTo, Left(Value, 32), "Decimal"))
            End Try
        End Function

        Public Shared Function FromObject(ByVal Value As Object) As Decimal
            Return FromObject(Value, Nothing)
        End Function

        Public Shared Function FromObject(ByVal Value As Object, ByVal NumberFormat As NumberFormatInfo) As Decimal

            If Value Is Nothing Then
                Return 0D
            End If

            Dim ValueInterface As IConvertible
            Dim ValueTypeCode As TypeCode

            ValueInterface = TryCast(Value, IConvertible)

            If Not ValueInterface Is Nothing Then

                ValueTypeCode = ValueInterface.GetTypeCode()

                Select Case ValueTypeCode

                    Case TypeCode.Boolean
                        Return DecimalType.FromBoolean(ValueInterface.ToBoolean(Nothing))

                    Case TypeCode.Byte
                        Return CDec(ValueInterface.ToByte(Nothing))

                    Case TypeCode.Int16
                        Return CDec(ValueInterface.ToInt16(Nothing))

                    Case TypeCode.Int32
                        Return CDec(ValueInterface.ToInt32(Nothing))

                    Case TypeCode.Int64
                        Return CDec(ValueInterface.ToInt64(Nothing))

                    Case TypeCode.Single
                        Return CDec(ValueInterface.ToSingle(Nothing))

                    Case TypeCode.Double
                        Return CDec(ValueInterface.ToDouble(Nothing))

                    Case TypeCode.Decimal
                        Return ValueInterface.ToDecimal(Nothing)

                    Case TypeCode.String
                        Return DecimalType.FromString(ValueInterface.ToString(Nothing), NumberFormat)

                    Case TypeCode.Char,
                         TypeCode.DateTime
                        ' Fall through to error                        
                    Case Else
                        ' Fall through to error
                End Select
            End If

            Throw New InvalidCastException(GetResourceString(ResId.InvalidCast_FromTo, VBFriendlyName(Value), "Decimal"))
        End Function

        Public Shared Function Parse(ByVal Value As String, ByVal NumberFormat As NumberFormatInfo) As Decimal
            Dim NormalizedNumberFormat As NumberFormatInfo
            Dim culture As CultureInfo = GetCultureInfo()

            If NumberFormat Is Nothing Then
                NumberFormat = culture.NumberFormat
            End If

            ' Normalize number format settings to enable us to first use the numeric settings for both currency and number parsing
            ' compatible with VB6
            NormalizedNumberFormat = GetNormalizedNumberFormat(NumberFormat)

            Const flags As NumberStyles =
                    NumberStyles.AllowDecimalPoint Or
                    NumberStyles.AllowExponent Or
                    NumberStyles.AllowLeadingSign Or
                    NumberStyles.AllowLeadingWhite Or
                    NumberStyles.AllowThousands Or
                    NumberStyles.AllowTrailingSign Or
                    NumberStyles.AllowParentheses Or
                    NumberStyles.AllowTrailingWhite Or
                    NumberStyles.AllowCurrencySymbol

            Value = ToHalfwidthNumbers(Value, culture)

            Try
                ' Use numeric settings to parse
                Return System.Decimal.Parse(Value, flags, NormalizedNumberFormat)
            Catch FormatEx As FormatException When Not (NumberFormat Is NormalizedNumberFormat)
                ' Use currency settings to parse
                Return System.Decimal.Parse(Value, flags, NumberFormat)
            Catch Ex As Exception
                Throw Ex
            End Try

        End Function

        '  This method returns a NumberFormat with the relevant Currency Settings to be the same as the Number Settings
        '  In - NumberFormat to be normalized - this is not changed by this Method
        '  Return - Normalized NumberFormat
        Friend Shared Function GetNormalizedNumberFormat(ByVal InNumberFormat As NumberFormatInfo) As NumberFormatInfo

            Dim OutNumberFormat As NumberFormatInfo

            With InNumberFormat
                If (Not .CurrencyDecimalSeparator Is Nothing) AndAlso
                   (Not .NumberDecimalSeparator Is Nothing) AndAlso
                   (Not .CurrencyGroupSeparator Is Nothing) AndAlso
                   (Not .NumberGroupSeparator Is Nothing) AndAlso
                   (.CurrencyDecimalSeparator.Length = 1) AndAlso
                   (.NumberDecimalSeparator.Length = 1) AndAlso
                   (.CurrencyGroupSeparator.Length = 1) AndAlso
                   (.NumberGroupSeparator.Length = 1) AndAlso
                   (.CurrencyDecimalSeparator.Chars(0) = .NumberDecimalSeparator.Chars(0)) AndAlso
                   (.CurrencyGroupSeparator.Chars(0) = .NumberGroupSeparator.Chars(0)) AndAlso
                   (.CurrencyDecimalDigits = .NumberDecimalDigits) Then
                    Return InNumberFormat
                End If
            End With


            With InNumberFormat
                If (Not .CurrencyDecimalSeparator Is Nothing) AndAlso
                   (Not .NumberDecimalSeparator Is Nothing) AndAlso
                   (.CurrencyDecimalSeparator.Length = .NumberDecimalSeparator.Length) AndAlso
                   (Not .CurrencyGroupSeparator Is Nothing) AndAlso
                   (Not .NumberGroupSeparator Is Nothing) AndAlso
                   (.CurrencyGroupSeparator.Length = .NumberGroupSeparator.Length) Then

                    Dim i As Integer
                    For i = 0 To .CurrencyDecimalSeparator.Length - 1
                        If (.CurrencyDecimalSeparator.Chars(i) <> .NumberDecimalSeparator.Chars(i)) Then GoTo MisMatch
                    Next

                    For i = 0 To .CurrencyGroupSeparator.Length - 1
                        If (.CurrencyGroupSeparator.Chars(i) <> .NumberGroupSeparator.Chars(i)) Then GoTo MisMatch
                    Next

                    Return InNumberFormat
                End If
            End With

MisMatch:

            OutNumberFormat = DirectCast(InNumberFormat.Clone, NumberFormatInfo)

            ' Set the Currency Settings to be the Same as the Numeric Settings
            With OutNumberFormat
                .CurrencyDecimalSeparator = .NumberDecimalSeparator
                .CurrencyGroupSeparator = .NumberGroupSeparator
                .CurrencyDecimalDigits = .NumberDecimalDigits
            End With

            Return OutNumberFormat
        End Function

    End Class

#End Region

End Namespace
