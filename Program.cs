using System;
using System.Data.SqlClient;
using System.IO;

namespace prservice
{
    class Program
    {
        static void Main(string[] args)
        {
            Generuj g = new Generuj();

            g.GenerujPlik();


        }

        class Generuj
        {
            public void GenerujPlik()
            {
                string katalogRoboczy = @"c:\testy\SQL\Anaplan\";

                string polecenieSQL = System.IO.File.ReadAllText(@"c:\testy\SQL\sqlcomenda.txt");
                string dzien = (DateTime.Today.Day < 10) ? "0" + DateTime.Today.Day.ToString() : DateTime.Today.Day.ToString();
                string mc = (DateTime.Today.Month < 10) ? "0" + DateTime.Today.Month.ToString() : DateTime.Today.Month.ToString();

                // Display the file contents to the console. Variable text is a string.
                System.Console.WriteLine("Contents of WriteText.txt = {0}", polecenieSQL);

                // var connectionString = @"Server=DESKTOP-M9PRPPC\MSSQLSERVER01;Database=Derogation_System;Trusted_Connection=True;";
                var connectionString = @"Data Source=172.26.60.102;Initial Catalog=MRO;User =webdb.admin; Password = Webdb.Admin ";
                using (var client = new SqlConnection(connectionString))
                {
                    client.Open();
                    SqlCommand cmd = new SqlCommand(polecenieSQL, client);

                    SqlDataReader reader = cmd.ExecuteReader();
                    string StrData = dzien + mc + DateTime.Today.Year.ToString();
                    string pathPlik = katalogRoboczy + "AnaplanToEflow" + StrData + ".csv";
                    StreamWriter sw = File.CreateText(pathPlik);

                    while (reader.Read())
                    {
                        sw.WriteLine(reader[0].ToString());

                        Console.WriteLine("\t{0}",
                            reader[0]);
                    }
                    reader.Close();
                    sw.Dispose();
                }
            }

        }
    }
}
