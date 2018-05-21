using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace AITResearchSystem.Services
{
    public class IpAddressService
    {
        private readonly IHttpContextAccessor _accessor;

        public IpAddressService(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public string GetIpAddress()
        {
            string ipAddress;
            try
            {
                ipAddress = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
                if (string.IsNullOrEmpty(ipAddress))
                {
                    ipAddress = "127.0.0.1";
                }
                else if (ipAddress == "::1") // it is localhost
                {
                    //This is for Local(LAN) Connected ID Address
                    string stringHostName = Dns.GetHostName();
                    
                    //Get Ip Address From The Dns Host Address List
                    IPAddress[] arrIpAddress = Dns.GetHostAddresses(stringHostName);

                    try
                    {
                        // Identifies the IPV4 IP Address from the DNS Host Address List
                        foreach (IPAddress ip4 in arrIpAddress.Where(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork))
                        {
                            ipAddress = ip4.ToString();
                        }
                    }
                    catch
                    {
                        ipAddress = "127.0.0.1";
                    }
                }
            }
            catch (Exception)
            {
                ipAddress = "127.0.0.1";
            }

            return ipAddress;
        }
    }
}
