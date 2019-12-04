using System;
using System.Data.SqlClient;

namespace prservice
{
    class Program
    {
        static void Main(string[] args)
        {
           

            string polecenieSQL = System.IO.File.ReadAllText(@"c:\testy\SQL\sqlcomenda.txt");

            // Display the file contents to the console. Variable text is a string.
            System.Console.WriteLine("Contents of WriteText.txt = {0}", polecenieSQL);




            // var connectionString = @"Server=DESKTOP-M9PRPPC\MSSQLSERVER01;Database=Derogation_System;Trusted_Connection=True;";
            var connectionString = @"Data Source=172.26.60.102;Initial Catalog=MRO;User =webdb.admin; Password = Webdb.Admin ";
            using (var client = new SqlConnection(connectionString))
            {
                client.Open();
                SqlCommand cmd = new SqlCommand(polecenieSQL , client);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine("\t{0}",
                        reader[0]);
                }
                reader.Close();
            }
        }
    }
}
