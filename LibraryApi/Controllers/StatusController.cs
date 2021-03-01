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

        // the :int makes the route expect an int
        [HttpGet("customers/{customerId:int}")]
        public ActionResult GetInfoAboutCustomer(int customerId)
        {
            return Ok($"Getting info about customer {customerId}");
        }

        // straight up data in the URL "heirarchical"
        [HttpGet("blogs/{year:int}/{month:int:min(1):max(12)}/{day:int}")]
        public ActionResult GetBlogPosts(int year, int month, int day)
        {
            return Ok($"Getting blogs for {month}-{day}-{year}");
        }

        //query string data entry
        [HttpGet("employees")]
        public ActionResult GetEmployees([FromQuery]string department = "All")
        {
            var response = new GetEmployeesResponse
            {
                Data = new List<string> { "Joe", "Ron" },
                Department = department
            };
            return Ok(response);
        }

        [HttpGet("whoami")]
        public ActionResult WhoAmI([FromHeader(Name ="User-Agent")]string userAgent)
        {
            return Ok($"I have no idea, but you are running {userAgent}");
        }
    }
    public class GetEmployeesResponse
    {
        public List<string> Data { get; set; }
        public string Department { get; set; }
    }

    public class StatusResponse
    {
        public string Message { get; set; }
        public DateTime LastChecked { get; set; }
    }
}
