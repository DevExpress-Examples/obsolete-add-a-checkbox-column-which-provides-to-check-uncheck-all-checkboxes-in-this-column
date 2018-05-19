Imports MultipleCheckExample.Model
Imports System
Imports System.Collections.ObjectModel
Imports System.Linq

Namespace MultipleCheckExample
	Public Class TaskViewModel
		Public Property List() As ObservableCollection(Of Task)
		Public Sub New()
			List = New ObservableCollection(Of Task)()
			For i As Integer = 0 To 4
				List.Add(Task.NewTask(i))
			Next i
		End Sub
	End Class
End Namespace
