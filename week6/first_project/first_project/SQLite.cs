using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLitePCL;
using first_project.Models;
using System.Globalization;
using first_project;
using Windows.UI.Xaml.Media.Imaging;
using System.IO;
using Windows.Storage;
using Windows.Storage.Streams;

public class DataAccess
{
    BitmapImage NewImage;
    string address = "ms-appx:///Assets/";
    CultureInfo provider = CultureInfo.InvariantCulture;
    SQLiteConnection conn;
    public DataAccess()
    {     // Get a reference to the SQLite database
        NewImage = new BitmapImage();
        conn = new SQLiteConnection("sqlitepcldemo.db");
        string sql = @"CREATE TABLE IF NOT EXISTS
                                ListItem (Id      VARCHAR( 140 ) PRIMARY KEY NOT NULL,
                                            Title    VARCHAR( 140 ),
                                            Description    VARCHAR( 140 ),
                                            Date VARCHAR( 140 ),
                                            Image VARCHAR( 140 )
                                            );";
        using (var statement = conn.Prepare(sql))
        {
            statement.Step();
        }
    }

    public void insert(string id, string title, string description, DateTimeOffset date)
    {

        using (var custstmt = conn.Prepare("INSERT INTO ListItem (Id, Title, Description,Date) VALUES (?, ?, ?, ?)"))
        {
            custstmt.Bind(1, id);
            custstmt.Bind(2, title);
            custstmt.Bind(3, description);
            custstmt.Bind(4, date.ToString(provider));
            custstmt.Step();
        }

    }

    public void insert(string id, string title, string description, DateTimeOffset date,string imageName)
    {

        using (var custstmt = conn.Prepare("INSERT INTO ListItem (Id, Title, Description,Date,Image) VALUES (?, ?, ?, ?, ?)"))
        {
            custstmt.Bind(1, id);
            custstmt.Bind(2, title);
            custstmt.Bind(3, description);
            custstmt.Bind(4, date.ToString(provider));
            custstmt.Bind(5, imageName);
            custstmt.Step();
        }


    }

    public void update(string id, string title, string description, DateTimeOffset date, string image)
    {

        using (var custstmt = conn.Prepare("UPDATE ListItem SET Title = ?, Description = ? ,Date = ? Image = ? WHERE Id=?"))
        {
            custstmt.Bind(1, title);
            custstmt.Bind(2, description);
            custstmt.Bind(3, date.ToString(provider));
            custstmt.Bind(4, image);
            custstmt.Bind(5, id);
            custstmt.Step();
        }
    }

    public void delete(string id)
    {

        using (var statement = conn.Prepare("DELETE FROM ListItem WHERE Id = ?"))
        {
            statement.Bind(1, id);
            statement.Step();
        }

    }

    DateTimeOffset stringToDate(string input)
    {
        string format = "MM/dd/yyyy H:mm:ss zzz";
        DateTimeOffset result;

        result = DateTimeOffset.ParseExact(input, format, provider,
                                           DateTimeStyles.AllowWhiteSpaces |
                                           DateTimeStyles.AdjustToUniversal);


        return result;
    }

    public void nameToImage(string name)
    {
        string temp = address + name;
        if (name != null)
        {
            NewImage = new BitmapImage(new Uri(temp));
        }
    }

    public void readDate()
    {
        using (var statement = conn.Prepare("SELECT Id,Title,Description,Date,Image FROM ListItem"))
        {
            while (SQLiteResult.ROW == statement.Step())
            {
                nameToImage((string)statement[4]);
                ListItem temp = new ListItem((string)statement[0], (string)statement[1], (string)statement[2], stringToDate((string)statement[3]), NewImage,(string)statement[4]);
                ((App)App.Current).ViewModel.AddTodoItem(temp);
            }
        }
    }

    public StringBuilder query(string input)
    {
        string input2 = "%" + input + "%";
        StringBuilder result = new StringBuilder();
        using (var statement = conn.Prepare("SELECT Title,Description,Date FROM ListItem WHERE Title LIKE ? OR Description LIKE ? OR Date LIKE ?"))
        {
            statement.Bind(1, input2);
            statement.Bind(2, input2);
            statement.Bind(3, input2);
            while (SQLiteResult.ROW == statement.Step())
            {
                result.Append("Title: ").Append((string)statement[0]).Append(" Description: ").Append((string)statement[1]).Append(" Date: ").Append((string)statement[2]).Append("\n");
            }
        }
        return result;
    }
}
