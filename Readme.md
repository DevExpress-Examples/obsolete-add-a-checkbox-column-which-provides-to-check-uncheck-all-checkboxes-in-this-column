<!-- default file list -->
*Files to look at*:

* [CheckMultipleBehavior.cs](./CS/MultipleCheckExample/Behavior/CheckMultipleBehavior.cs) (VB: [CheckMultipleBehavior.vb](./VB/MultipleCheckExample/Behavior/CheckMultipleBehavior.vb))
* **[MainWindow.xaml](./CS/MultipleCheckExample/MainWindow.xaml) (VB: [MainWindow.xaml](./VB/MultipleCheckExample/MainWindow.xaml))**
* [MainWindow.xaml.cs](./CS/MultipleCheckExample/MainWindow.xaml.cs) (VB: [MainWindow.xaml.vb](./VB/MultipleCheckExample/MainWindow.xaml.vb))
* [Task.cs](./CS/MultipleCheckExample/Model/Task.cs) (VB: [Task.vb](./VB/MultipleCheckExample/Model/Task.vb))
* [TaskViewModel.cs](./CS/MultipleCheckExample/ViewModel/TaskViewModel.cs) (VB: [TaskViewModel.vb](./VB/MultipleCheckExample/ViewModel/TaskViewModel.vb))
<!-- default file list end -->

Starting with v18.2, GridColumn provides an out-of-the-box option to show a CheckBox in its header - [ShowCheckBoxInHeader](https://documentation.devexpress.com/WPF/DevExpress.Xpf.Grid.ColumnBase.ShowCheckBoxInHeader.property).

# How to add a CheckBox column which provides to check/uncheck all CheckBoxes in this column


<p>This example demonstrates how to add a CheckBox to a column header which allows you to check/uncheck all CheckBoxes in cells under this column.<br><br>This functionality is implemented by means of a behavior - <strong>CheckMultipleBehavior</strong>. It should be attached to TableView. CheckBoxes appear in <strong>boolean</strong> columns for which the <strong>CheckMultipleBehavior.ShowHeaderCheckBox</strong> attached property is enabled.<br><br>To immediately update CheckBoxes' state, the behavior does the following:<br>1) Subscribes the grid's data source to the CollectionChanged event if supported.<br>2) Subscribes every item in the grid's data source to the PropertyChanged event if INotifyPropertyChanged is implemented.<br>3) Subscribes to changing the IsHeaderCheckedProperty attached property. This property can be changed by clicking CheckBoxes in column headers.</p>

<br/>
