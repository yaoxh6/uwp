using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace first_project.Models
{
    public class PrimaryTile
    {
        public string time { get; set; } = "ok";
        public string message { get; set; } = "nothing";
        public string message2 { get; set; } = " At vero eos et accusamus et iusto odio dignissimos ducimus qui blanditiis praesentium voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi sint occaecati cupiditate non provident.";
        public string branding { get; set; } = "name";
        public string appName { get; set; } = "HoL";
    
        public PrimaryTile(string input,string input2) {
            time = input;
            message = input2;
        }
    }

}
