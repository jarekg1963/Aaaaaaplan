using System;
using System.Data.SqlClient;
using System.IO;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace prservice
{
    class Program
    {
        static void Main(string[] args)
        {

            Generuj g = new Generuj();
            g.GenerujPlik();
            SendFileToServer s = new SendFileToServer();
            s.Send();

        }

        class Generuj
        {

            public void GenerujPlik()
            {
                string katalogRoboczy = @"c:\testy\SQL\Anaplan\";
                string dzien = (DateTime.Today.Day < 10) ? "0" + DateTime.Today.Day.ToString() : DateTime.Today.Day.ToString();
                string mc = (DateTime.Today.Month < 10) ? "0" + DateTime.Today.Month.ToString() : DateTime.Today.Month.ToString();
                string StrData = dzien + mc + DateTime.Today.Year.ToString();
                string pathPlik = katalogRoboczy + "AnaplanToEflow" + StrData + ".csv";

                string plikSql = katalogRoboczy + "sqlcomenda.txt";


                string polecenieSQL = System.IO.File.ReadAllText(plikSql);
                // Display the file contents to the console. Variable text is a string.
                System.Console.WriteLine("Contents of WriteText.txt = {0}", polecenieSQL);

                // var connectionString = @"Server=DESKTOP-M9PRPPC\MSSQLSERVER01;Database=Derogation_System;Trusted_Connection=True;";
                var connectionString = @"Data Source=172.26.60.102;Initial Catalog=MRO;User =webdb.admin; Password = Webdb.Admin ";
                using (var client = new SqlConnection(connectionString))
                {
                    client.Open();
                    SqlCommand cmd = new SqlCommand(polecenieSQL, client);
                    SqlDataReader reader = cmd.ExecuteReader();

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

        class SendFileToServer
        {
            // Enter your host name or IP here
            private static string host = "37.74.15.90";
            // Enter your sftp username here
            private static string username = "sap.anaplan";
            // Enter your sftp password here
            private static string password = "Kgqd1z6K";

            static string katalogRoboczy = @"c:\testy\SQL\Anaplan\";
            static string dzien = (DateTime.Today.Day < 10) ? "0" + DateTime.Today.Day.ToString() : DateTime.Today.Day.ToString();
            static string mc = (DateTime.Today.Month < 10) ? "0" + DateTime.Today.Month.ToString() : DateTime.Today.Month.ToString();
            static string StrData = dzien + mc + DateTime.Today.Year.ToString();
            string pathPlik = katalogRoboczy + "AnaplanToEflow" + StrData + ".csv";
            string plik = "AnaplanToEflow" + StrData + ".csv";
            public void Send()
            {
                var connectionInfo = new ConnectionInfo(host, "sftp", new PasswordAuthenticationMethod(username, password));
                // Upload File
                using (var sftp = new SftpClient(connectionInfo))
                {
                    sftp.Connect();
                    sftp.ChangeDirectory("PR Report");
                    using (var uplfileStream = System.IO.File.OpenRead(pathPlik))
                    {
                        sftp.UploadFile(uplfileStream, plik);
                    }
                    sftp.Disconnect();
                }

            }
        }
    }
}
