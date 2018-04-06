using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Windows.UI.Xaml.Media.Imaging;

namespace first_project.Models
{
    public class ListItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string id;

        public string title { get; set; }

        public string description { get; set; }

        public DateTimeOffset date { get; set; }

        public bool completed { get; set; }

        public BitmapImage src { get; set; }

        public bool Completed
        {
            get { return this.completed; }
            set
            {
                this.completed = value;
                NotifyPropertyChanged("Completed");
            }
        }
        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ListItem()
        {

        }
        public ListItem(string title,string description ,DateTimeOffset date,BitmapImage input)
        {
            this.id = Guid.NewGuid().ToString();
            this.title = title;
            this.description = description;
            this.completed = false;
            this.date = date;
            this.src = input;
        }
    }
}
