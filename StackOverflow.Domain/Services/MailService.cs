using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace StackOverflow.Domain.Services
{
    public class MailService
    {
        public IRestResponse SendMail(string destination, string message)
        {
            var client = new RestClient
            {
                BaseUrl = new Uri("https://api.mailgun.net/v2"),
                Authenticator = new HttpBasicAuthenticator("api",
                    "key-8tw489mxfegaqewx93in2xo449q5p3l0")
            };
            var request = new RestRequest();
            request.AddParameter("domain",
                                "app5dcaf6d377cc4ddcb696b827eabcb975.mailgun.org", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "StackOverflowP4@p4.com");
            String email = "<" + destination + ">";
            request.AddParameter("to", email);
            request.AddParameter("subject", "Password");
            String text = message;
            request.AddParameter("text", text);
            request.Method = Method.POST;
            return client.Execute(request);
        }

    }
    
}
