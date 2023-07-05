using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Net.NetworkInformation;


namespace PortChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] portsToCheck = { 21, 22, 8083, 8323, 11001 };
            string filePath = "iplist.csv";
            string resultFile = "ipportresult.csv";

            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    using (var writer = new StreamWriter(resultFile))
                    {
                        writer.WriteLine("IP;Ping;Port {0};Port {1};Port {2};Port {3};Port {4}", portsToCheck[0], portsToCheck[1], portsToCheck[2], portsToCheck[3], portsToCheck[4]);
                        while (!reader.EndOfStream)
                        {
                            string[] line = reader.ReadLine().Split(';');
                            string host = line[0];
                            string portStatus = host;

                            try
                            {
                                Ping ping = new Ping();
                                PingReply reply = ping.Send(host);
                                if (reply.Status == IPStatus.Success)
                                {
                                    Console.WriteLine("Host {0} is reachable.", host);
                                    portStatus += ";Reachable";
                                    foreach (int port in portsToCheck)
                                    {
                                        try
                                        {
                                            using (var client = new TcpClient(host, port))
                                            {
                                                Console.WriteLine("Port {0} on IP {1} is open.", port, host);
                                                portStatus += ";Open";
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine("Port {0} on IP {1} is closed.", port, host);
                                            portStatus += ";Closed";
                                        }
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Host {0} is not reachable.", host);
                                    portStatus += ";Unreachable;;;";
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error pinging host: " + ex.Message);
                                portStatus += ";Error;;;";
                            }

                            writer.WriteLine(portStatus);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading file: " + ex.Message);
            }

            Console.WriteLine("\n Fäddisch!");
            Console.ReadLine(); 
        }
    }
}

