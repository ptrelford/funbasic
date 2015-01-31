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
        If ty Is Nothing Then Throw New InvalidOperationException(ns + " not defined")
        Dim ps(args.Length - 1) As Type
        For i = 0 To args.Length - 1
            ps(i) = GetType(Object)
        Next
        Dim mi = ty.GetRuntimeMethod(name, ps)
        If mi Is Nothing Then Throw New InvalidOperationException(name + " not defined")
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
        If ty Is Nothing Then Throw New InvalidOperationException(ns + " not defined")
        Dim pi = ty.GetRuntimeProperty(name)
        If pi Is Nothing Then Throw New InvalidOperationException(name + " not defined")
        pi.SetMethod().Invoke(Nothing, New Object() {value})
    End Sub

    Public Sub EventAdd(ns As String, name As String, handler As EventHandler) _
        Implements IFFI.EventAdd
        Dim ty = ass.GetType("FunBasic.Library." + ns)
        Dim ev = ty.GetRuntimeEvent(name)
        ev.AddMethod.Invoke(Nothing, New Object() {handler})
    End Sub

End Class
