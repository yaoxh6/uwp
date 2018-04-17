using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using first_project.Models;
using Windows.UI.Xaml.Media.Imaging;

namespace first_project.ViewModels
{
    public class ListItemViewModels
    {
        private ObservableCollection<Models.ListItem> allItems = new ObservableCollection<Models.ListItem>();

        public ObservableCollection<Models.ListItem> AllItems { get { return this.allItems; } }
        public void AddTodoItem(ListItem temp)
        {
            this.allItems.Add(temp);
        }

        public void RemoveTodoItem(string id)
        {
            ListItem temp = new ListItem();
            for(int i = 0; i < allItems.Count(); i++)
            {
                if(allItems[i].id == id)
                {
                    temp = allItems[i];
                    break;
                }
            }
            this.allItems.Remove(temp);
            ((App)App.Current).dataBase.delete(id);
        }

        public void UpdateTodoItem(string id,string title,string description,DateTimeOffset date,BitmapImage src,string image)
        {
            ListItem temp = new ListItem(id, title, description, date, src,image);
            this.RemoveTodoItem(id);
            this.AddTodoItem(temp);
            ((App)App.Current).dataBase.insert(id, title, description, date,image);
        }
    }
}
