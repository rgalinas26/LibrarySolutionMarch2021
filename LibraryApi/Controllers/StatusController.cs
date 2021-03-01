﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryApi.Controllers
{
    public class StatusController : ControllerBase
    {
        [HttpGet("status")]
        public StatusResponse GetTheStatuts()
        {
            return new StatusResponse
            {
                Message = "Everything is going great. Thanks for asking!",
                LastChecked = DateTime.Now
            };
        }
    }

    public class StatusResponse
    {
        public string Message { get; set; }
        public DateTime LastChecked { get; set; }
    }
}
