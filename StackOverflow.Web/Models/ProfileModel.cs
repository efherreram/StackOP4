﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StackOverflow.Web.Models
{
    public class ProfileModel
    {

        public string UserName { get; set; }
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public int ReputationScore { get; set; }
        public int Views { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastLogTime { get; set; }

    }
}