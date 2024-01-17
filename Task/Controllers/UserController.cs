using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using Task.DBContext;
using Task.Model;
using Task.Services;

namespace Task.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        IUser u;
        TaskContext db;
        IConfiguration _config;
        public UserController(IUser _u, TaskContext _db, IConfiguration cofig=null) 
        { 
            u= _u;
            db = _db;
            _config = cofig;
        }
        [Authorize]
        [HttpGet]
        [Route("GetAll")]
        public ActionResult GetAll()
        {
            return Ok(new
            {
                Code = 0,
                Message = u.GetAll()
            }) ;
        }
        [Authorize]
        [HttpPost]
        [Route("Save")]
        public ActionResult Save(User o)
        {
            if (ModelState.IsValid)
            {
                var res = u.Save(o);
                return new ObjectResult(res)
                {
                    StatusCode = StatusCodes.Status201Created
                };
                
            }
            else
            {
                return BadRequest();
            }
          
        }
        [Authorize]
        [HttpGet]
        [Route("Delete")]
        public ActionResult Delete(int id)
        {
            ResponseModel res = new ResponseModel();

            var obj = u.Delete(id);

            if (obj.Code == 2)
            {
                res.Code = 2;
                res.Message = "Invalid UserId";
                return BadRequest(res);

            }
            if (obj.Message != null)
            {
                res.Code = 0;
                res.Message = obj;
                return Ok(res);
            }
            else
            {
                res.Code = -1;
                res.Message = "Does Not Exist";
                return NotFound(res);
            }
        }
        [Authorize]
        [HttpGet]
        [Route("Get")]
        public ActionResult Get(int id)
        {
          ResponseModel res=new ResponseModel();

            var obj = u.Get(id);

            if (obj.Code==2)
            {
                res.Code = 2;
                res.Message = "Invalid UserId";
                return BadRequest(res);

            }
            if (obj.Message!=null)
            {
                res.Code = 0; 
                res.Message = obj;  
                return Ok(res);
            }
            else
            {
                res.Code = -1;
                res.Message="Does Not Exist"; 
                return NotFound(res);
            }
        }
        [Authorize]
        [HttpPut]
        [Route("PutUser")]
        public ActionResult Put(User user)
        {
            if (user.Id==0)
                return BadRequest("Not a valid id");

           
                var existingUser = db.TblUsers.Where(s => s.Id == user.Id)
                                                        .FirstOrDefault();

                if (existingUser != null)
                {
                existingUser.Username = user.Username;
                existingUser.Password = user.Password;
                existingUser.IsAdmin=user.IsAdmin;
                existingUser.Age=user.Age;
                existingUser.Hobbies=user.Hobbies;

               
                    db.SaveChanges();
                }
                else
                {
                    return NotFound();
                }
            

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public ActionResult Login(string UserName,string Password)
        {
            try
            {
                string userName = u.ValidateLogin(UserName, Password);
                if (userName!=null)
                {
                    string JWTToken = BuildJWTToken(userName);
                    return Ok(new { Code = 0, Message = JWTToken });
                }
                else
                {
                    return Ok(new { Code = -1, Message = "invalid credentials" });
                }
            }
            catch (Exception er)
            {

                return Ok(new { Code = -1, Message = er.Message });
            }
           

            

        }
        private string BuildJWTToken(string userName)
            {

                List<Claim> ClaimList = new List<Claim>();

               ClaimList.Add(new Claim("Name", userName));
               ClaimList.Add(new Claim("Role", "Admin"));


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtToken:SecretKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var issuer = _config["JwtToken:Issuer"];
                var audience = _config["JwtToken:Audience"];
                var jwtValidity = DateTime.Now.AddMinutes(Convert.ToDouble(_config["JwtToken:TokenExpiry"]));
                var token = new JwtSecurityToken(issuer,
                  audience,
                  ClaimList,
                  expires: jwtValidity,
                  signingCredentials: creds);
                return new JwtSecurityTokenHandler().WriteToken(token);
            }

       
    }
}
