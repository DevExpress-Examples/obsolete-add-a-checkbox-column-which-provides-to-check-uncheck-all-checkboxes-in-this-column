using System;
using System.ComponentModel;
using System.Linq;

namespace MultipleCheckExample.Model
{
    public class Task : INotifyPropertyChanged
    {
        private string _Name;
        private DateTime _Date;
        private bool _IsCompleted;
        private bool _IsReviewed;
        
        public string Name {
            get { return _Name; }
            set {
                if (_Name != value) {
                    _Name = value;
                    OnPropertyChanged("Name");
                }
            }
        }
        public DateTime Date {
            get { return _Date; }
            set {
                if (_Date != value) {
                    _Date = value;
                    OnPropertyChanged("Date");
                }
            }
        }
        public bool IsCompleted {
            get { return _IsCompleted; }
            set {
                if (_IsCompleted != value) {
                    _IsCompleted = value;
                    OnPropertyChanged("IsCompleted");
                }
            }
        }
        public bool IsReviewed {
            get { return _IsReviewed; }
            set {
                if (_IsReviewed != value) {
                    _IsReviewed = value;
                    OnPropertyChanged("IsReviewed");
                }
            }
        }
        public static Task NewTask(int seed = 100) {
            Random rnd = new Random(seed);
            return new Task() { Name = "Name #" + rnd.Next(1, 100), Date = new DateTime(2018, 3, rnd.Next(1, 30)), IsCompleted = rnd.Next(0, 2) != 0, IsReviewed = rnd.Next(0, 2) != 0 };
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
