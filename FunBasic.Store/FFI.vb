﻿Imports FunBasic.Interpreter
Imports System.Reflection

Public Class FFI
    Implements IFFI

    Dim ass As Assembly
    Dim typeLookup As New Dictionary(Of String, Dictionary(Of String, MethodInfo))(StringComparer.OrdinalIgnoreCase)
    Dim unhooks As New Dictionary(Of EventInfo, Action)()

    Sub New()
        ass = GetType(FunBasic.Library.TextWindow).GetTypeInfo().Assembly
    End Sub

    Public Function MethodInvoke(ns As String, name As String, args() As Object) As Object _
        Implements IFFI.MethodInvoke
        Dim mi = GetMethodInfo(ns, name)
        Dim ps = mi.GetParameters()
        Dim typedArgs(args.Length - 1) As Object
        For i = 0 To args.Length - 1
            Dim ty = ps(i).ParameterType        
            typedArgs(i) = ConvertArg(args(i), ty)
        Next
        Return mi.Invoke(Nothing, typedArgs)
    End Function

    Function GetTypeFromName(ns As String) As Type
        Dim ty = ass.GetType("FunBasic.Library." + ns)
        If ty Is Nothing Then
            Dim ti = ass.DefinedTypes().FirstOrDefault(Function(t) String.Compare(t.Name, ns, StringComparison.OrdinalIgnoreCase) = 0)
            If ti Is Nothing Then
                Throw New InvalidOperationException(ns + " not defined")
            Else
                ty = ass.GetType("FunBasic.Library." + ti.Name)
            End If
        End If
        Return ty
    End Function

    Function GetMethodInfo(ns As String, name As String) As MethodInfo
        Dim methodLookup As Dictionary(Of String, MethodInfo) = Nothing
        If Not typeLookup.TryGetValue(ns, methodLookup) Then
            Dim ty = GetTypeFromName(ns)
            methodLookup = ty.GetRuntimeMethods().ToDictionary(Function(m) m.Name, StringComparer.OrdinalIgnoreCase)
            typeLookup.Add(ns, methodLookup)
        End If

        Dim mi As MethodInfo = Nothing
        If Not methodLookup.TryGetValue(name, mi) Then
            Throw New InvalidOperationException(ns + "." + name + " not defined")
        End If
        Return mi
    End Function

    Function ConvertArg(arg As Object, ty As Type) As Object
        If ty Is GetType(String) Then
            Return Convert.ToString(arg)
        ElseIf ty Is GetType(Integer) Then
            If arg.GetType() Is GetType(String) AndAlso arg = "" Then
                Return 0
            Else
                Return Convert.ToInt32(arg)
            End If
        ElseIf ty Is GetType(Double) Then
            If arg.GetType() Is GetType(String) AndAlso arg = "" Then
                Return 0
            Else
                Return Convert.ToDouble(arg)
            End If
        ElseIf ty Is GetType(Boolean) Then
            Return Convert.ToBoolean(arg)
        Else
            Return arg
        End If
    End Function

    Public Function PropertyGet(ns As String, name As String) As Object _
        Implements IFFI.PropertyGet
        Dim ty = GetTypeFromName(ns)
        If ty Is Nothing Then Throw New InvalidOperationException(ns + " not defined")
        Dim pi = ty.GetRuntimeProperty(name)
        If pi Is Nothing Then Throw New InvalidOperationException(ns + "." + name + " not defined")
        Return pi.GetMethod().Invoke(Nothing, New Object() {})
    End Function

    Public Sub PropertySet(ns As String, name As String, value As Object) _
        Implements IFFI.PropertySet
        Dim ty = GetTypeFromName(ns)
        If ty Is Nothing Then Throw New InvalidOperationException(ns + " not defined")
        Dim pi = ty.GetRuntimeProperty(name)
        If pi Is Nothing Then Throw New InvalidOperationException(ns + "." + name + " not defined")
        Dim typedArg = ConvertArg(value, pi.PropertyType)
        pi.SetMethod().Invoke(Nothing, New Object() {typedArg})
    End Sub

    Public Sub EventAdd(ns As String, name As String, handler As EventHandler) _
        Implements IFFI.EventAdd
        Dim ty = GetTypeFromName(ns)
        Dim ev = ty.GetRuntimeEvent(name)
        Dim action As Action = Nothing
        If unhooks.TryGetValue(ev, action) Then
            action.Invoke()
            unhooks.Remove(ev)
        End If
        ev.AddMethod.Invoke(Nothing, New Object() {handler})
        unhooks.Add(ev, Sub()
                            ev.RemoveMethod.Invoke(Nothing, New Object() {handler})
                        End Sub)
    End Sub

    Public Sub Unhook()
        For Each action In unhooks.Values
            action.Invoke()
        Next
        unhooks.Clear()
    End Sub

    Public ReadOnly Property IsHooked As Boolean
        Get
            Return unhooks.Count > 0
        End Get
    End Property


End Class
