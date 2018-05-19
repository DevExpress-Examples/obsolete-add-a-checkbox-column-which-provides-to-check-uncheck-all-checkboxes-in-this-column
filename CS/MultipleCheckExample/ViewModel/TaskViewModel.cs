using MultipleCheckExample.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace MultipleCheckExample
{
    public class TaskViewModel
    {
        public ObservableCollection<Task> List { get; set; }
        public TaskViewModel() {
            List = new ObservableCollection<Task>();
            for (int i = 0; i < 5; i++) {
                List.Add(Task.NewTask(i));
            }
        }
    }
}
