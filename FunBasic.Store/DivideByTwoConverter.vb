Public Class DivideByTwoConverter
    Implements IValueConverter


    Public Function Convert(value As Object, targetType As Type, parameter As Object, language As String) As Object Implements IValueConverter.Convert
        If value IsNot Nothing And value.GetType() Is GetType(Double) Then
            Return CType(value, Double) / 2.0
        End If
        Return 0.0
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, language As String) As Object Implements IValueConverter.ConvertBack
        Throw New NotSupportedException()
    End Function
End Class
