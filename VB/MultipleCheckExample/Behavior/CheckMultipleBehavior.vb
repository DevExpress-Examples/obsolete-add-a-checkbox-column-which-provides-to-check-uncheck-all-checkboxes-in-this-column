Imports DevExpress.Mvvm.UI.Interactivity
Imports DevExpress.Xpf.Grid
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports System.ComponentModel
Imports System.Linq
Imports System.Windows
Imports System.Windows.Markup

Namespace MultipleCheckExample
	Public Class CheckMultipleBehavior
		Inherits Behavior(Of TableView)

		Private view As TableView
		Private checkedCount As New Dictionary(Of GridColumn, Integer)()
		Private changeCellsInCode As Boolean
		Private setCheckColumnValueActive As Boolean

		Protected Overrides Sub OnAttached()
			MyBase.OnAttached()
			view = TryCast(AssociatedObject, TableView)
			AddHandler view.Loaded, AddressOf View_Loaded
			AddHandler view.CellValueChanging, AddressOf View_CellValueChanging
			AddHandler view.Grid.FilterChanged, AddressOf Grid_FilterChanged
		End Sub

		Protected Overrides Sub OnDetaching()
			MyBase.OnDetaching()
			RemoveHandler view.Loaded, AddressOf View_Loaded
			RemoveHandler view.CellValueChanging, AddressOf View_CellValueChanging
			RemoveHandler view.Grid.FilterChanged, AddressOf Grid_FilterChanged
		End Sub

		Private Sub Grid_FilterChanged(ByVal sender As Object, ByVal e As RoutedEventArgs)
			For Each col As GridColumn In view.Grid.Columns
				If GetShowHeaderCheckBox(col) Then
					UpdateCheckedCount(col)
					SetCheckColumnValue(col)
				End If
			Next col
		End Sub
		Private Sub View_CellValueChanging(ByVal sender As Object, ByVal e As CellValueChangedEventArgs)
			If GetShowHeaderCheckBox(e.Column) Then
				view.CloseEditor()
			End If
		End Sub
		Private Sub View_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
			For Each col As GridColumn In view.Grid.Columns
				If GetShowHeaderCheckBox(col) AndAlso col.FieldType Is GetType(Boolean) Then
					checkedCount.Add(col, 0)
					col.HeaderTemplate = CreateColumnHeaderTemplate(col)
					UpdateCheckedCount(col)
					Dim dpd As DependencyPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(IsHeaderCheckedProperty, GetType(CheckMultipleBehavior))
					dpd.AddValueChanged(col, AddressOf IsHeaderChecked_Changed)
				End If
			Next col
			Dim dataSource As IList = TryCast(view.Grid.ItemsSource, IList)
			For Each item As Object In dataSource
				Dim iCurrentItem As INotifyPropertyChanged = TryCast(item, INotifyPropertyChanged)
				If iCurrentItem IsNot Nothing Then
					AddHandler iCurrentItem.PropertyChanged, AddressOf iItem_PropertyChanged
				End If
			Next item
			Dim iCollection As INotifyCollectionChanged = TryCast(dataSource, INotifyCollectionChanged)
			If iCollection IsNot Nothing Then
				AddHandler iCollection.CollectionChanged, AddressOf iCollection_CollectionChanged
			End If
		End Sub

		Private Function CreateColumnHeaderTemplate(ByVal column As GridColumn) As DataTemplate
			Dim xamlTemplate As String = "<DataTemplate><dxe:CheckEdit Content = """ & column.HeaderCaption & """ EditValue=""{Binding Path=DataContext.(local:CheckMultipleBehavior.IsHeaderChecked), RelativeSource={RelativeSource Mode=FindAncestor, AncestorType=dxg:GridColumnHeader}}""/></DataTemplate>"
			Dim context = New ParserContext()
			context.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation")
			context.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml")
			context.XmlnsDictionary.Add("dxe", "http://schemas.devexpress.com/winfx/2008/xaml/editors")
			context.XmlnsDictionary.Add("dxg", "http://schemas.devexpress.com/winfx/2008/xaml/grid")
			context.XmlnsDictionary.Add("local", "clr-namespace:MultipleCheckExample;assembly=MultipleCheckExample")
			Dim template = DirectCast(XamlReader.Parse(xamlTemplate, context), DataTemplate)
			Return template
		End Function

		Private Sub UpdateCheckCells(ByVal column As GridColumn, ByVal check As Boolean)
			For i As Integer = 0 To view.Grid.DataController.VisibleListSourceRowCount - 1
				view.Grid.SetCellValue(i, column, check)
			Next i
		End Sub

		Private Sub UpdateCheckedCount(ByVal column As GridColumn)
			checkedCount(column) = 0
			For i As Integer = 0 To view.Grid.DataController.VisibleListSourceRowCount - 1
				If DirectCast(view.Grid.GetCellValue(i, column), Boolean) Then
					checkedCount(column) += 1
				End If
			Next i
		End Sub

		Private Sub IsHeaderChecked_Changed(ByVal sender As Object, ByVal e As EventArgs)
			If Not setCheckColumnValueActive Then
				Dim column As GridColumn = TryCast(sender, GridColumn)
				changeCellsInCode = True
				Dim value? As Boolean = CType(column.GetValue(IsHeaderCheckedProperty), Boolean?)
				If value.HasValue Then
					checkedCount(column) = If(value.Value, view.Grid.DataController.VisibleListSourceRowCount, 0)
					UpdateCheckCells(column, value.Value)
				End If
				changeCellsInCode = False
			End If
		End Sub

		Private Sub iCollection_CollectionChanged(ByVal sender As Object, ByVal e As NotifyCollectionChangedEventArgs)
			Dim changedItem As Object = Nothing
			If e.NewItems IsNot Nothing Then
				changedItem = e.NewItems(0)
			ElseIf e.OldItems IsNot Nothing Then
				changedItem = e.OldItems(0)
			End If
			If changedItem IsNot Nothing Then
				Dim iChangedItem As INotifyPropertyChanged = TryCast(changedItem, INotifyPropertyChanged)
				If iChangedItem IsNot Nothing Then
					If e.Action = NotifyCollectionChangedAction.Add Then
						AddHandler iChangedItem.PropertyChanged, AddressOf iItem_PropertyChanged
					End If
					If e.Action = NotifyCollectionChangedAction.Remove Then
						RemoveHandler iChangedItem.PropertyChanged, AddressOf iItem_PropertyChanged
					End If
				End If
				For Each col As GridColumn In view.Grid.Columns
					If GetShowHeaderCheckBox(col) Then
						Dim checkValue As Boolean = DirectCast(TypeDescriptor.GetProperties(changedItem)(col.FieldName).GetValue(changedItem), Boolean)
						If checkValue AndAlso e.Action = NotifyCollectionChangedAction.Add Then
							checkedCount(col) += 1
						End If
						If checkValue AndAlso e.Action = NotifyCollectionChangedAction.Remove Then
							checkedCount(col) -= 1
						End If
						SetCheckColumnValue(col)
					End If
				Next col
			End If
		End Sub

		Private Sub iItem_PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs)
			If Not changeCellsInCode Then
				For Each col As GridColumn In view.Grid.Columns
					If GetShowHeaderCheckBox(col) Then
						If Not changeCellsInCode AndAlso col.FieldName = e.PropertyName Then
							Dim isChecked As Boolean = DirectCast(TypeDescriptor.GetProperties(sender)(e.PropertyName).GetValue(sender), Boolean)
							checkedCount(col) += If(isChecked, 1, -1)
							SetCheckColumnValue(col)
						End If
					End If
				Next col
			End If
		End Sub

		Public Sub SetCheckColumnValue(ByVal column As GridColumn)
			setCheckColumnValueActive = True
			Dim value? As Boolean = Nothing
			If checkedCount(column) = view.Grid.DataController.VisibleListSourceRowCount Then
				value = True
			ElseIf checkedCount(column) = 0 Then
				value = False
			End If
			column.SetValue(IsHeaderCheckedProperty, value)
			setCheckColumnValueActive = False
		End Sub

		Public Shared ReadOnly IsHeaderCheckedProperty As DependencyProperty = DependencyProperty.RegisterAttached("IsHeaderChecked", GetType(Boolean?), GetType(CheckMultipleBehavior))
		Public Shared ReadOnly ShowHeaderCheckBoxProperty As DependencyProperty = DependencyProperty.RegisterAttached("ShowHeaderCheckBox", GetType(Boolean), GetType(CheckMultipleBehavior))
		Public Shared Sub SetIsHeaderChecked(ByVal element As UIElement, ByVal value? As Boolean)
			element.SetValue(IsHeaderCheckedProperty, value)
		End Sub
		Public Shared Function GetIsHeaderChecked(ByVal element As UIElement) As Boolean?
			Return DirectCast(element.GetValue(IsHeaderCheckedProperty), Boolean?)
		End Function
		Public Shared Sub SetShowHeaderCheckBox(ByVal element As GridColumn, ByVal value As Boolean)
			element.SetValue(ShowHeaderCheckBoxProperty, value)
		End Sub
		Public Shared Function GetShowHeaderCheckBox(ByVal element As GridColumn) As Boolean
			Return DirectCast(element.GetValue(ShowHeaderCheckBoxProperty), Boolean)
		End Function
	End Class
End Namespace