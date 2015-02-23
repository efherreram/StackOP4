using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using StackOverflow.Domain.Entities;

namespace StackOverflow.Data
{
    public class StackOverflowContext :DbContext
    {

        public StackOverflowContext() :base("StackOverflowContext")
        {
            
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; } 

    }
}
