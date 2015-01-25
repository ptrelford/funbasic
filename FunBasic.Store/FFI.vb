Imports FunBasic.Interpreter
Imports System.Reflection

Public Class FFI
    Implements IFFI

    Dim ass As Assembly

    Sub New()
        ass = GetType(FunBasic.Library.TextWindow).GetTypeInfo().Assembly
    End Sub

    Public Function MethodInvoke(ns As String, name As String, args() As Object) As Object _
        Implements IFFI.MethodInvoke
        Dim ty = ass.GetType("FunBasic.Library." + ns)
        Dim ps(args.Length - 1) As Type
        For i = 0 To args.Length - 1
            ps(i) = GetType(Object)
        Next
        Dim mi = ty.GetRuntimeMethod(name, ps)
        Return mi.Invoke(Nothing, args)
    End Function

    Public Function PropertyGet(ns As String, name As String) As Object _
        Implements IFFI.PropertyGet
        Dim ty = ass.GetType("FunBasic.Library." + ns)
        Dim pi = ty.GetRuntimeProperty(name)
        Return pi.GetMethod().Invoke(Nothing, New Object() {})
    End Function

    Public Sub PropertySet(ns As String, name As String, value As Object) _
        Implements IFFI.PropertySet
        Dim ty = ass.GetType("FunBasic.Library." + ns)
        Dim pi = ty.GetRuntimeProperty(name)
        pi.SetMethod().Invoke(Nothing, New Object() {value})
    End Sub
End Class
