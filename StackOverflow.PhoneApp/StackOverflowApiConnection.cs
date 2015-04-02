using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp.Portable;
using RestSharp.Portable.Deserializers;
using StackOverflow.Web.Models;

namespace StackOverflow.PhoneApp
{
    class StackOverflowApiConnection
    {
        private readonly RestClient _ApiConnection; 

        public StackOverflowApiConnection()
        {
            _ApiConnection = new RestClient{BaseUrl = new Uri("http://localhost:16470/")};
        }

        public IEnumerable<QuestionListModel> GetQuestionList()
        {
            RestRequest request = new RestRequest{Resource = "/api/QuestionList"};
            var result = _ApiConnection.Execute(request);
            RestSharp.Portable.Deserializers.JsonDeserializer deserial = new JsonDeserializer();
            var list = deserial.Deserialize<IEnumerable<QuestionListModel>>(result.Result);
            return list;
        }

    }
}
