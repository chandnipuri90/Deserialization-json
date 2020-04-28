using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;
using System.Data.SqlClient;
using System.Configuration;
using Newtonsoft.Json;

using System.Collections.Specialized;

namespace DeserializeJson2ConsoleAppx
{
    class Program
    {
        static void Main(string[] args)
        {
            string jsonstring = File.ReadAllText(@"C:\Users\chandni1\Desktop\test1.json");

            var serializer = new JavaScriptSerializer();
            AllItems allItemsObj = serializer.Deserialize<AllItems>(jsonstring);

            string myConnectionString = ConfigurationManager.ConnectionStrings["MyconnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(myConnectionString))
            {
                con.Open();

                foreach (var item in allItemsObj.Items)
                {

                    if (SaveToDatabase(con, item))
                    {
                        Console.WriteLine("Success : " + item.Description + " Saved into database");
                        Console.Read();
                    }
                    else
                    {
                        Console.WriteLine("Error : " + item.Description + " unable to Saved into database");
                    }
                }
            }
            Console.Read();
        }


        static bool SaveToDatabase(SqlConnection con, Item aItemObj)
        {
            try
            {
                string insertQuery = @"Insert into AllItemsTable(LocalTimestamp,Id,Description,Processed,Position1,Position2) Values(@LocalTimestamp,@Id,@Description,@Processed,@Position1,@Position2)";
                using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                {
                    cmd.Parameters.Add(new SqlParameter("@LocalTimestamp", aItemObj.LocalTimestamp));
                    cmd.Parameters.Add(new SqlParameter("@Id", aItemObj.Id));
                    cmd.Parameters.Add(new SqlParameter("@Description", aItemObj.Description));
                    cmd.Parameters.Add(new SqlParameter("@Processed", aItemObj.Processed));
                    for (int index = 0; index < aItemObj.Position.Count; index++)
                    {
                        if (index == 0)
                            cmd.Parameters.Add(new SqlParameter("@Position1", aItemObj.Position[index].ToString()));
                        else
                            cmd.Parameters.Add(new SqlParameter("@Position2", aItemObj.Position[index].ToString()));
                    }
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception objEx)
            {
                return false;
            }
        }
    }

    public class Item
    {
        public string LocalTimestamp { get; set; }
        public int Id { get; set; }
        public string Description { get; set; }
        public bool Processed { get; set; }
        public List<double> Position { get; set; }
    }

    public class AllItems
    {
        public List<Item> Items { get; set; }
        public bool HasMoreResults { get; set; }
    }
}
