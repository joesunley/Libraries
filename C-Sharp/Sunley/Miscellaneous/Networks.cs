using System;
using System.IO;
using System.Net;

namespace Sunley.Miscellaneous
{
    public static partial class Misc
    {
        public static IPAddress GetPublicIPAddress()
        {
            String address = "";
            WebRequest request = WebRequest.Create("http://checkip.dyndns.org/");
            using (WebResponse response = request.GetResponse())
            using (StreamReader stream = new StreamReader(response.GetResponseStream()))
            {
                address = stream.ReadToEnd();
            }

            int first = address.IndexOf("Address: ") + 9;
            int last = address.LastIndexOf("</body>");
            address = address.Substring(first, last - first);

            return IPAddress.Parse(address);
        }
        
    }
}
