using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;

namespace DeamonClient
{
    class Program
    {
        static void Main(string[] args)
        {
            
            //Crud crud = new Crud();
            Console.WriteLine("Pro manuální přidání stanice do zálohovacího systému stiskněte +");
            string klavesa = Console.ReadLine();
            if (klavesa == "+")
            {
                RunAsync().Wait();
                //crud.Post();
            }
            Console.ReadLine();

            /*
            Backup b = new Backup();
            Console.WriteLine("Nyní se provede Backup");
            b.CopyFile();
            Console.ReadLine();
            

            b.CopyFolder(@"Z:\TestSource\Ahoj\Tomáš", @"Z:\TestDestination\Ahoj", true);
            Console.WriteLine("Zkopírováno");
            Console.ReadLine();*/
        }

        static async Task RunAsync()
        {
            using (var client = new HttpClient())
            {               
                static string GetMacAddress()
                {
                    string macAddresses = "";
                    foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
                    {
                        // Only consider Ethernet network interfaces, thereby ignoring any
                        // loopback devices etc.
                        if (nic.NetworkInterfaceType != NetworkInterfaceType.Ethernet) continue;
                        if (nic.OperationalStatus == OperationalStatus.Up)
                        {
                            macAddresses += nic.GetPhysicalAddress().ToString();
                            break;
                        }
                    }
                    return macAddresses;
                }

                static string GetLocalIPAddress()
                {
                    var host = Dns.GetHostEntry(Dns.GetHostName());
                    foreach (var ip in host.AddressList)
                    {
                        if (ip.AddressFamily == AddressFamily.InterNetwork)
                        {
                            return ip.ToString();
                        }
                    }
                    throw new Exception("No network adapters with an IPv4 address in the system!");
                }


                //Go get the data
                client.BaseAddress = new Uri("http://localhost:49497/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response;
                


                //GET ALL COMPUTER
                Console.WriteLine("GET ALL COMPUTER");
                response = await client.GetAsync("/api/clients");
                
                if (response.IsSuccessStatusCode)
                {
                    List<Clients> clients = await response.Content.ReadAsAsync<List<Clients>>();

                    bool promena = false;
                    for (int i = 0; i < clients.Count; i++)
                    {                        
                        if (clients[i].MacAddress == GetMacAddress())
                        {
                            Console.WriteLine("ID :" + clients[i].Id);
                            //int ClientId = clients[i].Id;
                            promena = true;
                            break;
                        }                                             
                    }

                    if (promena == false)
                    {
                        Console.WriteLine("POST");
                        Clients newClient = new Clients();
                        newClient.Pc_Name = Environment.MachineName;
                        newClient.MacAddress = GetMacAddress();
                        newClient.IpAddress = GetLocalIPAddress();
                        newClient.Created = Convert.ToString(string.Format("{0:HH:mm:ss}", DateTime.Now));
                        newClient.Active = true;

                        response = await client.PostAsJsonAsync("api/clients", newClient);
                        if (response.IsSuccessStatusCode)
                        {
                            Uri personUrl = response.Headers.Location;
                            Console.WriteLine(personUrl);

                            /*PUT*/
                            //newPerson.Name = "DASGDFSGLDS";
                            //response = await client.PostAsJsonAsync(personUrl, newPerson);

                            /*DELETE*/
                            //response = await client.DeleteAsync(personUrl);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Klient již existuje");
                    }
                    
                }




                Console.WriteLine("GET ALL PATH");
                response = await client.GetAsync("api/path");

                if (response.IsSuccessStatusCode)
                {
                    List<Path> paths = await response.Content.ReadAsAsync<List<Path>>();

                    //bool promena = false;
                    for (int i = 0; i < paths.Count; i++)
                    {
                        Console.WriteLine("{0}\t{1}",paths[i].Source, paths[i].Destination);
                    }                   
                }
                Console.ReadLine();



                /*POST COMPUTER TO API*/
                //Console.WriteLine("POST");
                //Clients newClient = new Clients();
                //newClient.Pc_Name = Environment.MachineName;
                //newClient.MacAddress = GetMacAddress();
                //newClient.IpAddress = GetLocalIPAddress();
                //newClient.Created = Convert.ToString(string.Format("{0:HH:mm:ss}", DateTime.Now));
                //newClient.Active = true;

                //response = await client.PostAsJsonAsync("api/clients", newClient);
                //if (response.IsSuccessStatusCode)
                //{
                //    Uri personUrl = response.Headers.Location;
                //    Console.WriteLine(personUrl);

                //    /*PUT*/
                //    //newPerson.Name = "DASGDFSGLDS";
                //    //response = await client.PostAsJsonAsync(personUrl, newPerson);

                //    /*DELETE*/
                //    //response = await client.DeleteAsync(personUrl);
                //}



                /*
                //GET SOME PEOPLE ON WRITED INDEX
                Console.WriteLine("GET");
                Console.Write("Type index which one you want:  ");
                string index = Console.ReadLine();
                 response = await client.GetAsync("/api/clients/" + index);

                if (response.IsSuccessStatusCode)
                {
                    Clients clientos = await response.Content.ReadAsAsync<Clients>();
                    //Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{5}\t{6}\t{7}", clientos.Id, clientos.CompName, clientos.MacAdress, clientos.IpAdress, clientos.UserName, clientos.SendDate, clientos.Active);
                    Console.WriteLine("Active: " + clientos.Active);
                }

                Console.ReadLine();
                */

            }

            //Console.ReadLine();
        }
    }
}
