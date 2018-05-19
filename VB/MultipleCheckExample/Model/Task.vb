Imports System
Imports System.ComponentModel
Imports System.Linq

Namespace MultipleCheckExample.Model
	Public Class Task
		Implements INotifyPropertyChanged

		Private _Name As String
		Private _Date As Date
		Private _IsCompleted As Boolean
		Private _IsReviewed As Boolean

		Public Property Name() As String
			Get
				Return _Name
			End Get
			Set(ByVal value As String)
				If _Name <> value Then
					_Name = value
					OnPropertyChanged("Name")
				End If
			End Set
		End Property
		Public Property [Date]() As Date
			Get
				Return _Date
			End Get
			Set(ByVal value As Date)
				If _Date <> value Then
					_Date = value
					OnPropertyChanged("Date")
				End If
			End Set
		End Property
		Public Property IsCompleted() As Boolean
			Get
				Return _IsCompleted
			End Get
			Set(ByVal value As Boolean)
				If _IsCompleted <> value Then
					_IsCompleted = value
					OnPropertyChanged("IsCompleted")
				End If
			End Set
		End Property
		Public Property IsReviewed() As Boolean
			Get
				Return _IsReviewed
			End Get
			Set(ByVal value As Boolean)
				If _IsReviewed <> value Then
					_IsReviewed = value
					OnPropertyChanged("IsReviewed")
				End If
			End Set
		End Property
		Public Shared Function NewTask(Optional ByVal seed As Integer = 100) As Task
			Dim rnd As New Random(seed)
			Return New Task() With {
				.Name = "Name #" & rnd.Next(1, 100),
				.Date = New Date(2018, 3, rnd.Next(1, 30)),
				.IsCompleted = rnd.Next(0, 2) <> 0,
				.IsReviewed = rnd.Next(0, 2) <> 0
			}
		End Function
		Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
		Private Sub OnPropertyChanged(ByVal propertyName As String)
			RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
		End Sub
	End Class
End Namespace
