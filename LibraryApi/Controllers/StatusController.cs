using LibraryApi.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryApi.Controllers
{
    public class StatusController : ControllerBase
    {
        private readonly ILookupServerStatus _statusLookup;

        public StatusController(ILookupServerStatus statusLookup)
        {
            _statusLookup = statusLookup;
        }

        [HttpGet("status")]
        public StatusResponse GetTheStatuts()
        {
            StatusResponse status = _statusLookup.GetStatusFor();
            return status; 
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

        [HttpPost("employees")]
        public ActionResult Hire([FromBody] PostEmployeeRequest request)
        {
            return Ok($"Hiring {request.Name} in {request.Department} for {request.StartingSalary:c}");
        }
    }
    public class PostEmployeeRequest
    {
        public string Name { get; set; }
        public string Department { get; set; }
        public int StartingSalary { get; set; }
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
