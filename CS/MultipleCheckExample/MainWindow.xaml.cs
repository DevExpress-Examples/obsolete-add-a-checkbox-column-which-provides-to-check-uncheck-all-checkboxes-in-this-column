using DevExpress.Xpf.Grid;
using MultipleCheckExample.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace MultipleCheckExample
{
    public partial class MainWindow : Window
    {
        public MainWindow() {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            ObservableCollection<Task> collection = grid.ItemsSource as ObservableCollection<Task>;
            collection.Add(Task.NewTask(new Random().Next(100)));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) {
            TableView view = grid.View as TableView;
            view.DeleteRow(view.FocusedRowHandle);
        }
    }
}
