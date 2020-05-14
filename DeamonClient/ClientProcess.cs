using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DeamonClient
{
    public static class ClientProcess
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


        static HttpClient client = new HttpClient();
        

        public static void ShowClient(Clients danyKlient)
        {
            Console.WriteLine($"Name: {danyKlient.Pc_Name}\tID: {danyKlient.Id}\tIpAdresa: {danyKlient.IpAddress}");
        }

        public static async Task<Uri> RegisterClientAsync(Clients klient)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync("api/clients", klient);
            response.EnsureSuccessStatusCode();

            return response.Headers.Location;
        }

        public static async Task<Clients> GetClientsAsync(string Path)
        {
            Clients klient = null;
            HttpResponseMessage response = await client.GetAsync(Path);
            if (response.IsSuccessStatusCode)
            {
                klient = await response.Content.ReadAsAsync<Clients>();
            }
            return klient;
        }

        public static async Task RunAsync()
        {
            client.BaseAddress = new Uri("http://localhost:49497/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.GetAsync("api/clients");

            List<Clients> klienti = await response.Content.ReadAsAsync<List<Clients>>();

            Uri url;

            bool promena = false;
            for (int i = 0; i < klienti.Count; i++)
            {
                if (klienti[i].MacAddress == GetMacAddress())
                {
                    Console.WriteLine("{0}\t{1}\t{2}","ID: " + klienti[i].Id,"Mac: " + klienti[i].MacAddress,"Active: " + klienti[i].Active);

                    url = response.Headers.Location;
                    Console.WriteLine("Vaše URL je: " + url);

                    promena = true;
                    break;
                }
            }

            

            if (promena == false)
            {
                Clients newClient = new Clients
                {
                    Pc_Name = Environment.MachineName,
                    MacAddress = GetMacAddress(),
                    IpAddress = GetLocalIPAddress(),
                    Created = Convert.ToString(string.Format("{0:HH:mm:ss}", DateTime.Now)),
                    Active = true
                };
                
                url = await RegisterClientAsync(newClient);

                if (url == null)
                {
                    Console.WriteLine("URL se nepodařilo načíst");
                }
                else
                {
                    Console.WriteLine("Vytvořeno na " + url);
                }

                Clients Klient = await GetClientsAsync("http://localhost:49497/api/clients/14");
                ShowClient(Klient);
            }
            else
            {
                Console.WriteLine("Klient je již v databázi");
            }
            

            

            
        }
    }
}
