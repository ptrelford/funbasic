Imports FunBasic.Interpreter
Imports System.Reflection

Public Class FFI
    Implements IFFI

    Dim ass As Assembly
    Dim typeLookup As New Dictionary(Of String, Dictionary(Of String, MethodInfo))()
    Dim unhooks As New List(Of Action)()

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

    Function GetMethodInfo(ns As String, name As String) As MethodInfo
        Dim methodLookup As Dictionary(Of String, MethodInfo) = Nothing
        If Not typeLookup.TryGetValue(ns, methodLookup) Then
            Dim ty = ass.GetType("FunBasic.Library." + ns)
            If ty Is Nothing Then Throw New InvalidOperationException(ns + " not defined")
            methodLookup = ty.GetRuntimeMethods().ToDictionary(Function(m) m.Name)
            typeLookup.Add(ns, methodLookup)
        End If
        Dim mi As MethodInfo = Nothing
        If Not methodLookup.TryGetValue(name, mi) Then
            Throw New InvalidOperationException(name + " not defined")
        End If
        Return mi
    End Function

    Function ConvertArg(arg As Object, ty As Type) As Object
        If ty Is GetType(String) Then
            Return Convert.ToString(arg)
        ElseIf ty Is GetType(Integer) Then
            Return Convert.ToInt32(arg)
        ElseIf ty Is GetType(Double) Then
            Return Convert.ToDouble(arg)
        Else
            Return arg
        End If
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
        unhooks.Add(Sub()
                        ev.RemoveMethod.Invoke(Nothing, New Object() {handler})
                    End Sub)
    End Sub

    Public Sub Unhook()
        For Each action In unhooks
            action.Invoke()
        Next
        unhooks.Clear()
    End Sub

End Class
