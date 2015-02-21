Imports System.Globalization, System.Reflection
Imports Windows.UI

Module ColorLookup

    Private ColorLookup As New Dictionary(Of String, Color)()

    Sub New()
        Dim ti = GetType(Colors)
        Dim colors = ti.GetRuntimeProperties()
        For Each pi In colors
            Dim color = pi.GetMethod().Invoke(Nothing, New Object() {})
            ColorLookup.Add(pi.Name.ToLower(), color)
        Next
        ColorLookup.Add("grey", ColorLookup("gray"))
    End Sub

    Function GetColor(name As String) As Color
        If name.StartsWith("#") Then
            Dim n = Integer.Parse(name.Substring(1), NumberStyles.HexNumber)
            Dim r = (n And &HFF0000) >> 16
            Dim g = (n And &HFF00) >> 8
            Dim b = (n And &HFF)
            Return Color.FromArgb(255, r, g, b)
        Else
            Return ColorLookup(name.ToLower())
        End If
    End Function

End Module
