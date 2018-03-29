using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using first_project.ViewModels;
using first_project.Models;

namespace first_project
{
    public class Content
    {
        public string id { get; set; }
        public ListItemViewModels result { get; set; }
        public Content()
        {
            id = "";
        }
        public Content(string id,ListItemViewModels input)
        {
            this.id = id;
            this.result = input;
        }
    }
}
