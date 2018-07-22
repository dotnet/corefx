' Copyright (c) Microsoft Corporation.  All rights reserved.

Imports System
Imports System.Globalization
Imports System.Security
Imports System.Security.Permissions

Imports Microsoft.VisualBasic.CompilerServices
Imports Microsoft.VisualBasic.CompilerServices.ExceptionUtils
Imports Microsoft.VisualBasic.CompilerServices.Utils

<Assembly: System.Reflection.AssemblyInformationalVersion("5.0301.0.0")>
<Assembly: System.Reflection.AssemblyFileVersion("5.0301.0.0")>

Namespace Microsoft.VisualBasic

    Public Module Information

#If Not TELESTO Then
        'QBColorTable below consists of :
        '&H0I,       '   0 - black
        '&H800000I,  '   1 - blue
        '&H8000I,    '   2 - green
        '&H808000I,  '   3 - cyan
        '&H80I,      '   4 - red
        '&H800080I,  '   5 - magenta
        '&H8080I,    '   6 - yellow
        '&HC0C0C0I,  '   7 - white
        '&H808080I,  '   8 - gray
        '&HFF0000I,  '   9 - light blue
        '&HFF00I,    '  10 - light green
        '&HFFFF00I,  '  11 - light cyan
        '&HFFI,      '  12 - light red
        '&HFF00FFI,  '  13 - light magenta
        '&HFFFFI,    '  14 - light yellow
        '&HFFFFFFI,  '  15 - bright white
        Private ReadOnly QBColorTable() As Integer = {&H0I, &H800000I, &H8000I, &H808000I,
                                                        &H80I, &H800080I, &H8080I,
                                                        &HC0C0C0I, &H808080I, &HFF0000I,
                                                        &HFF00I, &HFFFF00I, &HFFI,
                                                        &HFF00FFI, &HFFFFI, &HFFFFFFI}
        Friend Const COMObjectName As String = "__ComObject"
#End If

        '============================================================================
        ' Error functions.
        '============================================================================
        Public Function Err() As ErrObject

            Dim oProj As ProjectData
            oProj = ProjectData.GetProjectData()

            If oProj.m_Err Is Nothing Then
                oProj.m_Err = New ErrObject
            End If
            Err = oProj.m_Err

        End Function

#If Not TELESTO Then
        ' UNDONE This should be hidden
        <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)>
        Public Function Erl() As Integer
#Else
        '<System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)> _
        Public Function Erl() As Integer
#End If
            'UNDONE: THREADING REVIEW
            Dim oProj As ProjectData
            oProj = ProjectData.GetProjectData()
            Erl = oProj.m_Err.Erl
        End Function

        '============================================================================
        ' Is... functions.
        '============================================================================
        Public Function IsArray(ByVal VarName As Object) As Boolean

            If VarName Is Nothing Then
                Return False
            End If

            Return (TypeOf VarName Is System.Array)

        End Function

        Public Function IsDate(ByVal Expression As Object) As Boolean

            If Expression Is Nothing Then
                Return False
            End If

            If TypeOf Expression Is Date Then

                Return True

            Else
                Dim StringExpression As String = TryCast(Expression, String)

                If StringExpression IsNot Nothing Then
                    Dim ConvertedDate As DateTime

                    Return Conversions.TryParseDate(StringExpression, ConvertedDate)
                End If
            End If

            Return False

        End Function

        Public Function IsDBNull(ByVal Expression As Object) As Boolean

            If Expression Is Nothing Then
                Return False

            ElseIf TypeOf Expression Is System.DBNull Then
                Return True

            Else
                Return False

            End If

        End Function

        Public Function IsNothing(ByVal Expression As Object) As Boolean

            Return (Expression Is Nothing)

        End Function

        Public Function IsError(ByVal Expression As Object) As Boolean

            If Expression Is Nothing Then
                Return False
            End If

            Return (TypeOf Expression Is Exception)

        End Function

        Public Function IsReference(ByVal Expression As Object) As Boolean

            Return Not (TypeOf Expression Is System.ValueType)

        End Function

        Public Function LBound(ByVal Array As System.Array, Optional ByVal Rank As Integer = 1) As Integer

            If (Array Is Nothing) Then
                Throw VbMakeException(New ArgumentNullException(GetResourceString(ResId.Argument_InvalidNullValue1, "Array")), vbErrors.OutOfBounds)

            ElseIf (Rank < 1) OrElse (Rank > Array.Rank) Then
                Throw New RankException(GetResourceString(ResId.Argument_InvalidRank1, "Rank"))

            End If

            Return Array.GetLowerBound(Rank - 1)

        End Function

        Public Function UBound(ByVal Array As System.Array, Optional ByVal Rank As Integer = 1) As Integer

            If (Array Is Nothing) Then
                Throw VbMakeException(New ArgumentNullException(GetResourceString(ResId.Argument_InvalidNullValue1, "Array")), vbErrors.OutOfBounds)

            ElseIf (Rank < 1) OrElse (Rank > Array.Rank) Then
                Throw New RankException(GetResourceString(ResId.Argument_InvalidRank1, "Rank"))

            End If

            Return Array.GetUpperBound(Rank - 1)

        End Function

#If Not TELESTO Then
        '============================================================================
        ' Object type functions.
        '============================================================================
        <SecuritySafeCritical()>
        Friend Function TypeNameOfCOMObject(ByVal VarName As Object, ByVal bThrowException As Boolean) As String

            Dim Result As String = COMObjectName

            Try
                Call (New SecurityPermission(SecurityPermissionFlag.UnmanagedCode)).Demand()
            Catch ex As StackOverflowException
                Throw ex
            Catch ex As OutOfMemoryException
                Throw ex
            Catch ex As System.Threading.ThreadAbortException
                Throw ex
            Catch e As Exception
                If bThrowException Then
                    Throw e
                Else
                    GoTo CleanupTypeName
                End If
            End Try

            Dim pTypeInfo As UnsafeNativeMethods.ITypeInfo = Nothing
            Dim hr As Integer
            Dim ClassName As String = Nothing
            Dim DocString As String = Nothing
            Dim HelpContext As Integer
            Dim HelpFile As String = Nothing


            Do
                Dim pProvideClassInfo As UnsafeNativeMethods.IProvideClassInfo = TryCast(VarName, UnsafeNativeMethods.IProvideClassInfo)

                If pProvideClassInfo IsNot Nothing Then
                    Try
                        pTypeInfo = pProvideClassInfo.GetClassInfo()
                        hr = pTypeInfo.GetDocumentation(-1, ClassName, DocString, HelpContext, HelpFile)
                        If hr >= 0 Then
                            Result = ClassName
                            Exit Do
                        End If
                        pTypeInfo = Nothing
                    Catch ex As StackOverflowException
                        Throw ex
                    Catch ex As OutOfMemoryException
                        Throw ex
                    Catch ex As System.Threading.ThreadAbortException
                        Throw ex
                    Catch
                        'Ignore the error
                    End Try
                End If

                Dim pDispatch As UnsafeNativeMethods.IDispatch = TryCast(VarName, UnsafeNativeMethods.IDispatch)

                If pDispatch IsNot Nothing Then
                    ' Try using IDispatch 
                    hr = pDispatch.GetTypeInfo(0, UnsafeNativeMethods.LCID_US_ENGLISH, pTypeInfo)
                    If hr >= 0 Then
                        hr = pTypeInfo.GetDocumentation(-1, ClassName, DocString, HelpContext, HelpFile)
                        If hr >= 0 Then
                            Result = ClassName
                            Exit Do
                        End If
                    End If
                End If

            Loop While (False)


CleanupTypeName:

            If Result.Chars(0) = "_"c Then
                Result = Result.Substring(1)
            End If

            Return Result
        End Function

        '============================================================================
        ' Color functions.
        '============================================================================
        Public Function QBColor(ByVal Color As Integer) As Integer
            If (Color And &HFFF0I) <> 0 Then
                Throw New ArgumentException(GetResourceString(ResId.Argument_InvalidValue1, "Color"))
            End If

            QBColor = QBColorTable(Color)
        End Function

        Public Function RGB(ByVal Red As Integer, ByVal Green As Integer, ByVal Blue As Integer) As Integer
            If (Red And &H80000000I) <> 0 Then
                Throw New ArgumentException(GetResourceString(ResId.Argument_InvalidValue1, "Red"))
            ElseIf (Green And &H80000000I) <> 0 Then
                Throw New ArgumentException(GetResourceString(ResId.Argument_InvalidValue1, "Green"))
            ElseIf (Blue And &H80000000I) <> 0 Then
                Throw New ArgumentException(GetResourceString(ResId.Argument_InvalidValue1, "Blue"))
            End If

            ' VB2 treats any value > 255 as 255

            If (Red > 255) Then
                Red = &HFFI
            End If

            If (Green > 255) Then
                Green = &HFFI
            End If

            If (Blue > 255) Then
                Blue = &HFFI
            End If

            Return ((Blue * &H10000I) + (Green * &H100I) + Red)
        End Function
#End If 'NOT TELESTO

        Public Function VarType(ByVal VarName As Object) As VariantType
            If VarName Is Nothing Then
                Return VariantType.Object
            End If

            Return VarTypeFromComType(VarName.GetType())
        End Function

        Friend Function VarTypeFromComType(ByVal typ As System.Type) As VariantType
            If typ Is Nothing Then
                Return VariantType.Object
            End If

            If typ.IsArray() Then

                typ = typ.GetElementType()
                If typ.IsArray Then
                    Return CType(VariantType.Array Or VariantType.Object, VariantType)
                End If

                Dim Result As VariantType = VarTypeFromComType(typ)
                If (Result And VariantType.Array) <> 0 Then
                    'Element type is also an array, so just return "array of objects"
                    Return CType(VariantType.Array Or VariantType.Object, VariantType)
                End If
                Return CType(Result Or VariantType.Array, VariantType)

            ElseIf typ.IsEnum() Then
                typ = System.Enum.GetUnderlyingType(typ)
            End If

            If typ Is Nothing Then
                Return VariantType.Empty
            End If

            Select Case Type.GetTypeCode(typ)

                Case TypeCode.String
                    Return VariantType.String
                Case TypeCode.Int32
                    Return VariantType.Integer
                Case TypeCode.Int16
                    Return VariantType.Short
                Case TypeCode.Int64
                    Return VariantType.Long
                Case TypeCode.Single
                    Return VariantType.Single
                Case TypeCode.Double
                    Return VariantType.Double
                Case TypeCode.DateTime
                    Return VariantType.Date
                Case TypeCode.Boolean
                    Return VariantType.Boolean
                Case TypeCode.Decimal
                    Return VariantType.Decimal
                Case TypeCode.Byte
                    Return VariantType.Byte
                Case TypeCode.Char
                    Return VariantType.Char
                Case TypeCode.DBNull
                    Return VariantType.Null

            End Select

            If (typ Is GetType(System.Reflection.Missing)) OrElse
               (typ Is GetType(System.Exception)) OrElse
               (typ.IsSubclassOf(GetType(System.Exception))) Then
                Return VariantType.Error
            ElseIf typ.IsValueType() Then
                Return VariantType.UserDefinedType
            Else
                Return VariantType.Object
            End If

        End Function

#If Not TELESTO Then
#Region " BACKWARDS COMPATIBILITY.  These functions (IsNumeric, TypeName, SystemTypeName, VbTypeName) have been superceded by the versions in Versioned.vb "

        'WARNING WARNING WARNING WARNING WARNING
        'This code exists to support Everett compiled applications.  Make sure you understand
        'the backwards compatibility ramifications of any edit you make in this region.
        'The compiler loads references to these functions (see CompilerHost::PrepareRemappingTable()) so
        'they need to exist.  The Orcas compiler will be made not to die if they are missing
        'WARNING WARNING WARNING WARNING WARNING

        Friend Function IsOldNumericTypeCode(ByVal TypCode As System.TypeCode) As Boolean

            Select Case TypCode

                Case TypeCode.Int16,
                     TypeCode.Int32,
                     TypeCode.Int64,
                     TypeCode.Single,
                     TypeCode.Double,
                     TypeCode.Boolean,
                     TypeCode.Decimal,
                     TypeCode.Byte
                    Return True

                Case Else
                    Return False

            End Select

        End Function

        '*
        '*  IsNumeric -
        '* 
        '* 
        '* NOTE: Code changes here MUST BE PERFORMANCE TESTED
        '* 
        Public Function IsNumeric(ByVal Expression As Object) As Boolean

            Dim ValueInterface As IConvertible
            Dim ValueTypeCode As TypeCode

            ValueInterface = TryCast(Expression, IConvertible)

            If ValueInterface Is Nothing Then
                Dim CharArray As Char() = TryCast(Expression, Char())

                If CharArray IsNot Nothing Then
                    Expression = CStr(CharArray)
                Else
                    Return False
                End If
            End If

            ValueTypeCode = ValueInterface.GetTypeCode()

            If (ValueTypeCode = TypeCode.String) OrElse (ValueTypeCode = TypeCode.Char) Then

                'Convert to double, exception thrown if not a number
                Dim dbl As Double

                Dim i64Value As Int64
                Dim Value As String

                Value = ValueInterface.ToString(Nothing)

                Try
                    If IsHexOrOctValue(Value, i64Value) Then
                        Return True
                    End If
                Catch ex As StackOverflowException
                    Throw ex
                Catch ex As OutOfMemoryException
                    Throw ex
                Catch ex As System.Threading.ThreadAbortException
                    Throw ex
                Catch
                    Return False
                End Try

                Return DoubleType.TryParse(Value, dbl)

            End If

            Return IsOldNumericTypeCode(ValueTypeCode)

        End Function

        Friend Function OldVBFriendlyNameOfTypeName(ByVal typename As String) As String
            Dim ArraySuffix As String = Nothing
            Dim Name As String
            Dim LastChar As Integer = typename.Length - 1

            If typename.Chars(LastChar) = "]"c Then
                Dim pos As Integer
                pos = typename.IndexOf("["c)
                If pos + 1 = LastChar Then
                    ArraySuffix = "()"
                Else
                    ArraySuffix = typename.Substring(pos, LastChar - pos + 1).Replace("["c, "("c).Replace("]"c, ")"c)
                End If
                typename = typename.Substring(0, pos)
            End If

            Name = OldVbTypeName(typename)
            If Name Is Nothing Then
                Name = typename
            End If

            If ArraySuffix Is Nothing Then
                Return Name
            End If
            Return Name & AdjustArraySuffix(ArraySuffix)

        End Function

        Public Function TypeName(ByVal VarName As Object) As String

            Dim Result As String
            Dim bIsArray As Boolean
            Dim typ As System.Type
            Dim ArrayType As System.Type

            If VarName Is Nothing Then
                Return "Nothing"
            End If

            typ = VarName.GetType()

            If typ.IsArray Then
                bIsArray = True
                ArrayType = typ
                typ = ArrayType.GetElementType()
            End If

            If typ.IsEnum() Then

                Result = typ.Name
                GoTo UnmangleName

            Else
                Dim tc As TypeCode

                tc = Type.GetTypeCode(typ)

                Select Case tc

                    Case TypeCode.DBNull : Result = "DBNull"
                    Case TypeCode.Int16 : Result = "Short"
                    Case TypeCode.Int32 : Result = "Integer"
                    Case TypeCode.Single : Result = "Single"
                    Case TypeCode.Double : Result = "Double"
                    Case TypeCode.DateTime : Result = "Date"
                    Case TypeCode.String : Result = "String"
                    Case TypeCode.Boolean : Result = "Boolean"
                    Case TypeCode.Decimal : Result = "Decimal"
                    Case TypeCode.Byte : Result = "Byte"
                    Case TypeCode.Char : Result = "Char"
                    Case TypeCode.Int64 : Result = "Long"

                    Case Else

                        Result = typ.Name

                        If (typ.IsCOMObject AndAlso (System.String.CompareOrdinal(Result, COMObjectName) = 0)) Then
                            Result = LegacyTypeNameOfCOMObject(VarName, True)
                        End If

UnmangleName:
                        'REVIEW: Will managled names go away for Beta2?
                        ' They don't seem to be as of 11/6/2000
                        Dim i As Integer
                        i = Result.IndexOf("+"c)
                        If i >= 0 Then
                            Result = Result.Substring(i + 1)
                        End If

                End Select

            End If

            If bIsArray Then

                Dim ary As Array
                ary = CType(VarName, Array)
                If ary.Rank = 1 Then
                    Result = Result & "[]"
                Else
                    Result = Result & "[" & (New String(","c, ary.Rank - 1)) & "]"
                End If

                Result = OldVBFriendlyNameOfTypeName(Result)

            End If

            Return Result
        End Function

        Public Function SystemTypeName(ByVal VbName As String) As String

            Select Case Trim(VbName).ToUpperInvariant()
                Case "OBJECT" : Return "System.Object"
                Case "SHORT" : Return "System.Int16"
                Case "INTEGER" : Return "System.Int32"
                Case "SINGLE" : Return "System.Single"
                Case "DOUBLE" : Return "System.Double"
                Case "DATE" : Return "System.DateTime"
                Case "STRING" : Return "System.String"
                Case "BOOLEAN" : Return "System.Boolean"
                Case "DECIMAL" : Return "System.Decimal"
                Case "BYTE" : Return "System.Byte"
                Case "CHAR" : Return "System.Char"
                Case "LONG" : Return "System.Int64"
                Case Else : Return Nothing
            End Select

        End Function

        Public Function VbTypeName(ByVal UrtName As String) As String
            Return OldVbTypeName(UrtName)
        End Function

        Friend Function OldVbTypeName(ByVal UrtName As String) As String

            UrtName = Trim(UrtName).ToUpperInvariant()
            If Left(UrtName, 7) = "SYSTEM." Then
                UrtName = Mid(UrtName, 8)
            End If

            Select Case UrtName
                Case "OBJECT" : Return "Object"
                Case "INT16" : Return "Short"
                Case "INT32" : Return "Integer"
                Case "SINGLE" : Return "Single"
                Case "DOUBLE" : Return "Double"
                Case "DATETIME" : Return "Date"
                Case "STRING" : Return "String"
                Case "BOOLEAN" : Return "Boolean"
                Case "DECIMAL" : Return "Decimal"
                Case "BYTE" : Return "Byte"
                Case "CHAR" : Return "Char"
                Case "INT64" : Return "Long"
                Case Else
                    Return Nothing
            End Select

        End Function

        '============================================================================
        ' Object type functions.
        '============================================================================
        <SecuritySafeCritical()>
        Friend Function LegacyTypeNameOfCOMObject(ByVal VarName As Object, ByVal bThrowException As Boolean) As String

            Dim Result As String = COMObjectName

            Try
                Call (New SecurityPermission(SecurityPermissionFlag.UnmanagedCode)).Demand()
            Catch ex As StackOverflowException
                Throw ex
            Catch ex As OutOfMemoryException
                Throw ex
            Catch ex As System.Threading.ThreadAbortException
                Throw ex
            Catch e As Exception
                If bThrowException Then
                    Throw e
                Else
                    GoTo CleanupTypeName
                End If
            End Try

            Dim pTypeInfo As UnsafeNativeMethods.ITypeInfo = Nothing
            Dim hr As Integer
            Dim ClassName As String = Nothing
            Dim DocString As String = Nothing
            Dim HelpContext As Integer
            Dim HelpFile As String = Nothing

            Dim pDispatch As UnsafeNativeMethods.IDispatch = TryCast(VarName, UnsafeNativeMethods.IDispatch)

            If pDispatch IsNot Nothing Then
                hr = pDispatch.GetTypeInfo(0, UnsafeNativeMethods.LCID_US_ENGLISH, pTypeInfo)
                If hr >= 0 Then
                    hr = pTypeInfo.GetDocumentation(-1, ClassName, DocString, HelpContext, HelpFile)
                    If hr >= 0 Then
                        Result = ClassName
                    End If
                End If
            End If

CleanupTypeName:

            If Result.Chars(0) = "_"c Then
                Result = Result.Substring(1)
            End If

            Return Result
        End Function

#End Region
#End If 'Not TELESTO

    End Module

End Namespace


