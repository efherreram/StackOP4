using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackOverflow.Web.Models;

namespace StackOverflow.PhoneApp
{
    public class QList
    {
        private static IEnumerable<QuestionListModel> ques= null;
        public static IEnumerable<QuestionListModel> Ques
        {
            get { return ques; }
            set { ques = value; }
        }

        public static void Update()
        {
            var api = new StackOverflowApiConnection();
            ques = api.GetQuestionList();
        }
    }
}
