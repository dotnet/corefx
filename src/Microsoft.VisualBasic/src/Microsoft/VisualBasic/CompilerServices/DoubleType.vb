' Copyright (c) Microsoft Corporation.  All rights reserved.

Imports System
Imports System.Globalization

Imports Microsoft.VisualBasic.CompilerServices.DecimalType
Imports Microsoft.VisualBasic.CompilerServices.Utils

Namespace Microsoft.VisualBasic.CompilerServices

#Region " BACKWARDS COMPATIBILITY "

    'WARNING WARNING WARNING WARNING WARNING
    'This code exists to support Everett compiled applications.  Make sure you understand
    'the backwards compatibility ramifications of any edit you make in this region.
    'WARNING WARNING WARNING WARNING WARNING

    <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)>
    Public NotInheritable Class DoubleType
        ' Prevent creation.
        Private Sub New()
        End Sub

        Public Shared Function FromString(ByVal Value As String) As Double
            Return FromString(Value, Nothing)
        End Function

        Public Shared Function FromString(ByVal Value As String, ByVal NumberFormat As NumberFormatInfo) As Double

            If Value Is Nothing Then
                Return 0
            End If

            Try
                Dim i64Value As Int64

                If IsHexOrOctValue(Value, i64Value) Then
                    Return CDbl(i64Value)
                End If
                Return DoubleType.Parse(Value, NumberFormat)

            Catch e As FormatException
                Throw New InvalidCastException(GetResourceString(ResId.InvalidCast_FromStringTo, Left(Value, 32), "Double"), e)
            End Try

        End Function

        Public Shared Function FromObject(ByVal Value As Object) As Double
            Return FromObject(Value, Nothing)
        End Function

        Public Shared Function FromObject(ByVal Value As Object, ByVal NumberFormat As NumberFormatInfo) As Double

            If Value Is Nothing Then
                Return 0
            End If

            Dim ValueInterface As IConvertible
            Dim ValueTypeCode As TypeCode

            ValueInterface = TryCast(Value, IConvertible)

            If ValueInterface Is Nothing Then
                GoTo ThrowInvalidCast
            End If

            ValueTypeCode = ValueInterface.GetTypeCode()

            Select Case ValueTypeCode

                Case TypeCode.Boolean
                    Return CDbl(ValueInterface.ToBoolean(Nothing))

                Case TypeCode.Byte
                    If TypeOf Value Is System.Byte Then
                        Return CDbl(DirectCast(Value, Byte))
                    Else
                        Return CDbl(ValueInterface.ToByte(Nothing))
                    End If

                Case TypeCode.Int16
                    If TypeOf Value Is System.Int16 Then
                        Return CDbl(DirectCast(Value, Int16))
                    Else
                        Return CDbl(ValueInterface.ToInt16(Nothing))
                    End If

                Case TypeCode.Int32
                    If TypeOf Value Is System.Int32 Then
                        Return CDbl(DirectCast(Value, Int32))
                    Else
                        Return CDbl(ValueInterface.ToInt32(Nothing))
                    End If

                Case TypeCode.Int64
                    If TypeOf Value Is System.Int64 Then
                        Return CDbl(DirectCast(Value, Int64))
                    Else
                        Return CDbl(ValueInterface.ToInt64(Nothing))
                    End If

                Case TypeCode.Single
                    If TypeOf Value Is System.Single Then
                        Return DirectCast(Value, Single)
                    Else
                        Return CDbl(ValueInterface.ToSingle(Nothing))
                    End If

                Case TypeCode.Double
                    If TypeOf Value Is System.Double Then
                        Return CDbl(DirectCast(Value, Double))
                    Else
                        Return CDbl(ValueInterface.ToDouble(Nothing))
                    End If

                Case TypeCode.Decimal
                    'Do not use .ToDecimal because of jit temp issue effects all perf
                    Return DecimalToDouble(ValueInterface)

                Case TypeCode.String
                    Return DoubleType.FromString(ValueInterface.ToString(Nothing), NumberFormat)

                Case TypeCode.Char,
                     TypeCode.DateTime
                    ' Fall through to error

                Case Else
                    ' Fall through to error
            End Select
ThrowInvalidCast:
            Throw New InvalidCastException(GetResourceString(ResId.InvalidCast_FromTo, VBFriendlyName(Value), "Double"))

        End Function

        Private Shared Function DecimalToDouble(ByVal ValueInterface As IConvertible) As Double
            Return CDbl(ValueInterface.ToDecimal(Nothing))
        End Function

        Public Shared Function Parse(ByVal Value As String) As Double
            Return Parse(Value, Nothing)
        End Function

        Friend Shared Function TryParse(ByVal Value As String, ByRef Result As Double) As Boolean
            Dim NumberFormat As NumberFormatInfo
            Dim NormalizedNumberFormat As NumberFormatInfo
            Dim culture As CultureInfo = GetCultureInfo()

            NumberFormat = culture.NumberFormat
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

            ' The below code handles the 80% case efficiently and is inefficient only when the numeric and currency settings
            ' are different

            If NumberFormat Is NormalizedNumberFormat Then
                Return System.Double.TryParse(Value, flags, NormalizedNumberFormat, Result)
            Else
                Try
                    ' Use numeric settings to parse
                    ' Note that we use Parse instead of TryParse in order to distinguish whether the conversion failed
                    ' due to FormatException or other exception like OverFlowException, etc.
                    Result = System.Double.Parse(Value, flags, NormalizedNumberFormat)
                    Return True
                Catch FormatEx As FormatException
                    ' Use currency settings to parse
                    Try
                        Return System.Double.TryParse(Value, flags, NumberFormat, Result)
                    Catch ex As ArgumentException
                        Return False
                    End Try
                Catch ex As StackOverflowException
                    Throw ex
                Catch ex As OutOfMemoryException
                    Throw ex
                Catch ex As System.Threading.ThreadAbortException
                    Throw ex
                Catch Ex As Exception
                    Return False
                End Try
            End If

        End Function

        Public Shared Function Parse(ByVal Value As String, ByVal NumberFormat As NumberFormatInfo) As Double
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
                Return System.Double.Parse(Value, flags, NormalizedNumberFormat)
            Catch FormatEx As FormatException When Not (NumberFormat Is NormalizedNumberFormat)
                ' Use currency settings to parse
                Return System.Double.Parse(Value, flags, NumberFormat)
            Catch Ex As Exception
                Throw Ex
            End Try

        End Function

    End Class

#End Region

End Namespace


