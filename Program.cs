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

            // Deklaracja zmiennych globalnych 

            string katalogusLokalny = @"c:\testy\SQL\Anaplan\";
            string dzien = (DateTime.Today.Day < 10) ? "0" + DateTime.Today.Day.ToString() : DateTime.Today.Day.ToString();
            string mc = (DateTime.Today.Month < 10) ? "0" + DateTime.Today.Month.ToString() : DateTime.Today.Month.ToString();
            string StrData = dzien + mc + DateTime.Today.Year.ToString();
            string pathPlik = katalogusLokalny + "AnaplanToEflow" + StrData + ".csv";
            string plik = "AnaplanToEflow" + StrData + ".csv";

            // Generowanie pliku tekstowego z MSSQL - a

            zapiszDoLogu z = new zapiszDoLogu();
            z.startProgramu(katalogusLokalny);

            Generuj g = new Generuj();
            g.GenerujPlik(katalogusLokalny, pathPlik);
            // Kopiowanie na FTP 
            SendFileToServer s = new SendFileToServer();
        
            s.Send(katalogusLokalny, StrData, pathPlik, plik);


            z.KoniecProgramu(katalogusLokalny);
        }

        class Generuj
        {
            public void GenerujPlik(string kat, string pathPlik)
            {
                string plikSql = kat + "sqlcomenda.txt";
                string polecenieSQL = System.IO.File.ReadAllText(plikSql);

                System.Console.WriteLine("Contents of WriteText.txt = {0}", polecenieSQL);

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

            public void Send(string kat, string StrData, string pathPlik, string plik)
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

        class zapiszDoLogu
        {
            public void startProgramu(string katalogusLokalny)
            {
              
                StreamWriter writetext = File.AppendText(katalogusLokalny + "log.txt");
                writetext.WriteLine("program wystartowal " + DateTime.Now.ToLongDateString() + "    " + DateTime.Now.ToLongTimeString());
                writetext.Dispose();
            }


            public void KoniecProgramu(string katalogusLokalny)
            {             
                StreamWriter writetext = File.AppendText(katalogusLokalny + "log.txt");
              writetext.WriteLine("zakonczyl dzialanie  " + DateTime.Now.ToLongDateString() + "  " + DateTime.Now.ToLongTimeString());
                writetext.Dispose();
            }

        }

    }
}

