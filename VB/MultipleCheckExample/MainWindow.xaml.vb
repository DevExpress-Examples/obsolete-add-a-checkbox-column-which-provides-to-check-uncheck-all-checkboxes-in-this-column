Imports DevExpress.Xpf.Grid
Imports MultipleCheckExample.Model
Imports System
Imports System.Collections.ObjectModel
Imports System.Linq
Imports System.Windows

Namespace MultipleCheckExample
	Partial Public Class MainWindow
		Inherits Window

		Public Sub New()
			InitializeComponent()
		End Sub

		Private Sub Button_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
			Dim collection As ObservableCollection(Of Task) = TryCast(grid.ItemsSource, ObservableCollection(Of Task))
			collection.Add(Task.NewTask((New Random()).Next(100)))
		End Sub

		Private Sub Button_Click_1(ByVal sender As Object, ByVal e As RoutedEventArgs)
			Dim view As TableView = TryCast(grid.View, TableView)
			view.DeleteRow(view.FocusedRowHandle)
		End Sub
	End Class
End Namespace
