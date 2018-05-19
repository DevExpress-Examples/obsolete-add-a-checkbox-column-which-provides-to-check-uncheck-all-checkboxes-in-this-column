using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Grid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Markup;

namespace MultipleCheckExample
{
    public class CheckMultipleBehavior : Behavior<TableView>
    {
        TableView view;
        Dictionary<GridColumn, int> checkedCount = new Dictionary<GridColumn, int>();
        bool changeCellsInCode;
        bool setCheckColumnValueActive;

        protected override void OnAttached() {
            base.OnAttached();
            view = AssociatedObject as TableView;
            view.Loaded += View_Loaded;
            view.CellValueChanging += View_CellValueChanging;
            view.Grid.FilterChanged += Grid_FilterChanged;
        }

        protected override void OnDetaching() {
            base.OnDetaching();
            view.Loaded -= View_Loaded;
            view.CellValueChanging -= View_CellValueChanging;
            view.Grid.FilterChanged -= Grid_FilterChanged;
        }

        private void Grid_FilterChanged(object sender, RoutedEventArgs e) {
            foreach (GridColumn col in view.Grid.Columns) {
                if (GetShowHeaderCheckBox(col)) {
                    UpdateCheckedCount(col);
                    SetCheckColumnValue(col);
                }
            }
        }
        private void View_CellValueChanging(object sender, CellValueChangedEventArgs e) {
            if (GetShowHeaderCheckBox(e.Column))
                view.CloseEditor();
        }
        private void View_Loaded(object sender, RoutedEventArgs e) {
            foreach (GridColumn col in view.Grid.Columns) {
                if (GetShowHeaderCheckBox(col) && col.FieldType == typeof(bool)) {
                    checkedCount.Add(col, 0);
                    col.HeaderTemplate = CreateColumnHeaderTemplate(col);
                    UpdateCheckedCount(col);
                    DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(IsHeaderCheckedProperty, typeof(CheckMultipleBehavior));
                    dpd.AddValueChanged(col, IsHeaderChecked_Changed);
                }
            }
            IList dataSource = view.Grid.ItemsSource as IList;
            foreach (object item in dataSource) {
                INotifyPropertyChanged iCurrentItem = item as INotifyPropertyChanged;
                if (iCurrentItem != null) {
                    iCurrentItem.PropertyChanged += iItem_PropertyChanged;
                }
            }
            INotifyCollectionChanged iCollection = dataSource as INotifyCollectionChanged;
            if (iCollection != null)
                iCollection.CollectionChanged += iCollection_CollectionChanged;
        }

        DataTemplate CreateColumnHeaderTemplate(GridColumn column) {
            string xamlTemplate = "<DataTemplate><dxe:CheckEdit Content = \"" + column.HeaderCaption + "\" EditValue=\"{Binding Path=DataContext.(local:CheckMultipleBehavior.IsHeaderChecked), RelativeSource={RelativeSource Mode=FindAncestor, AncestorType=dxg:GridColumnHeader}}\"/></DataTemplate>";
            var context = new ParserContext();
            context.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            context.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");
            context.XmlnsDictionary.Add("dxe", "http://schemas.devexpress.com/winfx/2008/xaml/editors");
            context.XmlnsDictionary.Add("dxg", "http://schemas.devexpress.com/winfx/2008/xaml/grid");
            context.XmlnsDictionary.Add("local", "clr-namespace:MultipleCheckExample;assembly=MultipleCheckExample");
            var template = (DataTemplate)XamlReader.Parse(xamlTemplate, context);
            return template;
        }

        void UpdateCheckCells(GridColumn column, bool check) {
            for (int i = 0; i < view.Grid.DataController.VisibleListSourceRowCount; i++)
                view.Grid.SetCellValue(i, column, check);
        }

        void UpdateCheckedCount(GridColumn column) {
            checkedCount[column] = 0;
            for (int i = 0; i < view.Grid.DataController.VisibleListSourceRowCount; i++)
                if ((bool)view.Grid.GetCellValue(i, column))
                    checkedCount[column]++;
        }

        void IsHeaderChecked_Changed(object sender, EventArgs e) {
            if (!setCheckColumnValueActive) {
                GridColumn column = sender as GridColumn;
                changeCellsInCode = true;
                bool? value = column.GetValue(IsHeaderCheckedProperty) as bool?;
                if (value.HasValue) {
                    checkedCount[column] = value.Value ? view.Grid.DataController.VisibleListSourceRowCount : 0;
                    UpdateCheckCells(column, value.Value);
                }
                changeCellsInCode = false;
            }
        }

        void iCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            object changedItem = null;
            if (e.NewItems != null)
                changedItem = e.NewItems[0];
            else if (e.OldItems != null)
                changedItem = e.OldItems[0];
            if (changedItem != null) {
                INotifyPropertyChanged iChangedItem = changedItem as INotifyPropertyChanged;
                if (iChangedItem != null) {
                    if (e.Action == NotifyCollectionChangedAction.Add)
                        iChangedItem.PropertyChanged += iItem_PropertyChanged;
                    if (e.Action == NotifyCollectionChangedAction.Remove)
                        iChangedItem.PropertyChanged -= iItem_PropertyChanged;
                }
                foreach (GridColumn col in view.Grid.Columns) {
                    if (GetShowHeaderCheckBox(col)) {
                        bool checkValue = (bool)TypeDescriptor.GetProperties(changedItem)[col.FieldName].GetValue(changedItem);
                        if (checkValue && e.Action == NotifyCollectionChangedAction.Add)
                            checkedCount[col]++;
                        if (checkValue && e.Action == NotifyCollectionChangedAction.Remove)
                            checkedCount[col]--;
                        SetCheckColumnValue(col);
                    }
                }
            }
        }

        void iItem_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (!changeCellsInCode) {
                foreach (GridColumn col in view.Grid.Columns) {
                    if (GetShowHeaderCheckBox(col)) {
                        if (!changeCellsInCode && col.FieldName == e.PropertyName) {
                            bool isChecked = (bool)TypeDescriptor.GetProperties(sender)[e.PropertyName].GetValue(sender);
                            checkedCount[col] += isChecked ? 1 : -1;
                            SetCheckColumnValue(col);
                        }
                    }
                }
            }
        }

        public void SetCheckColumnValue(GridColumn column) {
            setCheckColumnValueActive = true;
            bool? value = null;
            if (checkedCount[column] == view.Grid.DataController.VisibleListSourceRowCount)
                value = true;
            else if (checkedCount[column] == 0)
                value = false;
            column.SetValue(IsHeaderCheckedProperty, value);
            setCheckColumnValueActive = false;
        }

        public static readonly DependencyProperty IsHeaderCheckedProperty = DependencyProperty.RegisterAttached("IsHeaderChecked", typeof(bool?), typeof(CheckMultipleBehavior));
        public static readonly DependencyProperty ShowHeaderCheckBoxProperty = DependencyProperty.RegisterAttached("ShowHeaderCheckBox", typeof(bool), typeof(CheckMultipleBehavior));
        public static void SetIsHeaderChecked(UIElement element, bool? value) {
            element.SetValue(IsHeaderCheckedProperty, value);
        }
        public static bool? GetIsHeaderChecked(UIElement element) {
            return (bool?)element.GetValue(IsHeaderCheckedProperty);
        }
        public static void SetShowHeaderCheckBox(GridColumn element, bool value) {
            element.SetValue(ShowHeaderCheckBoxProperty, value);
        }
        public static bool GetShowHeaderCheckBox(GridColumn element) {
            return (bool)element.GetValue(ShowHeaderCheckBoxProperty);
        }
    }
}