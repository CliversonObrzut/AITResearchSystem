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
                    //Get Ip Host Entry
                    //System.Net.IPHostEntry ipHostEntries = System.Net.Dns.GetHostEntry(stringHostName);
                    //Get Ip Address From The Dns Host Address List
                    IPAddress[] arrIpAddress = Dns.GetHostAddresses(stringHostName);

                    try
                    {
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
        //protected string GetIpAddress()
        //{
        //    //get IP through PROXY
        //    //====================
        //    System.Web.HttpContext context = System.Web.HttpContext.Current;
        //    string ipAddress = _context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

        //    //should break ipAddress down, but here is what it looks like:
        //    // return ipAddress;
        //    if (!string.IsNullOrEmpty(ipAddress))
        //    {
        //        string[] address = ipAddress.Split(',');
        //        if (address.Length != 0)
        //        {
        //            return address[0];
        //        }
        //    }
        //    //if not proxy, get nice ip, give that back :(
        //    //ACROSS WEB HTTP REQUEST
        //    //=======================
        //    ipAddress = context.Request.UserHostAddress;//ServerVariables["REMOTE_ADDR"];

        //    if (ipAddress.Trim() == "::1")//ITS LOCAL(either lan or on same machine), CHECK LAN IP INSTEAD
        //    {
        //        //This is for Local(LAN) Connected ID Address
        //        string stringHostName = System.Net.Dns.GetHostName();
        //        //Get Ip Host Entry
        //        System.Net.IPHostEntry ipHostEntries = System.Net.Dns.GetHostEntry(stringHostName);
        //        //Get Ip Address From The Ip Host Entry Address List
        //        System.Net.IPAddress[] arrIpAddress = ipHostEntries.AddressList;

        //        try
        //        {
        //            ipAddress = arrIpAddress[1].ToString();
        //        }
        //        catch
        //        {
        //            try
        //            {
        //                ipAddress = arrIpAddress[0].ToString();
        //            }
        //            catch
        //            {
        //                try
        //                {
        //                    arrIpAddress = System.Net.Dns.GetHostAddresses(stringHostName);
        //                    ipAddress = arrIpAddress[0].ToString();
        //                }
        //                catch
        //                {
        //                    ipAddress = "127.0.0.1";
        //                }
        //            }
        //        }
        //    }
        //    return ipAddress;
        //}
    }
}
