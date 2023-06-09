using CRUDTask.Data;
using CRUDTask.DataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace CRUDTask.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : Controller
    {
        private readonly UserCrudContext _dbContext;
        public UserController(UserCrudContext userCrudContext)
        {
            _dbContext = userCrudContext;
        }


        [HttpGet]
        public async Task<IActionResult> getAllUsers(int pg, int pageSize,string searchText)
        {
            var allUsers = _dbContext.Users.Where(x => x.DeletedAt == null).ToList();
            if (searchText == "undefined" || searchText.Length < 3 || searchText=="")
            {
                allUsers = _dbContext.Users.Where(x => x.DeletedAt == null).ToList();
            }
            else
            {
                allUsers = _dbContext.Users.Where(x=>x.DeletedAt == null && (x.FirstName.ToLower().Contains(searchText.ToLower()) || x.LastName.ToLower().Contains(searchText.ToLower()) || x.Email.ToLower().Contains(searchText.ToLower()))).ToList();
            }
            if (pg < 1)
            {
                pg = 1;
            }
            int recsCount = allUsers.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = allUsers.Skip(recSkip).Take(pager.PageSize).ToList();
            var users = data.ToList();
            var response = new
            {
                TotalEntries = recsCount,
                user = data,
                totalPage = pager.EndPage
            };
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> addUser([FromBody] User userRequest)
        {
            var existingUserName = await _dbContext.Users.FirstOrDefaultAsync(x=>x.UserName == userRequest.UserName);
            var existingEmail = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == userRequest.Email);
            if (existingEmail != null)
            {
                return BadRequest("Email ID is already exist");
            }
            else if (existingUserName != null)
            {
                return BadRequest("User name is already exist");
            }
            else if (userRequest.FirstName == "")
            {
                return BadRequest("First Name is required");
            }
            else if (userRequest.LastName == "")
            {
                return BadRequest("Last Name is required");
            }
            else if (userRequest.Email == "")
            {
                return BadRequest("Email is required");
            }
            else if (userRequest.Phone == "")
            {
                return BadRequest("Phone number is required");
            }
            else if (userRequest.Phone.Length != 10)
            {
                return BadRequest("Length of phone number should be 10");
            }
            else if (userRequest.StreetAddress == "")
            {
                return BadRequest("Street Address is required");
            }
            else if (userRequest.UserName == "")
            {
                return BadRequest("User Name required");
            }
            else if (userRequest.Password.Length < 6)
            {
                return BadRequest("Password shold be 6 characters long");
            }
            else
            {
                await _dbContext.Users.AddAsync(userRequest);
                await _dbContext.SaveChangesAsync();
                return Ok(userRequest);
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> getUser([FromRoute] int id)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x=>x.Id == id);
            
            if(user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> updateUser([FromRoute] int id , User updateUser)
        {
            var user = await _dbContext.Users.FindAsync(id);
            var existingUserName = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserName == updateUser.UserName && x.Id != user.Id);
            var existingEmail = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == updateUser.Email && x.Id != user.Id);
            if (user == null)
            {
                return NotFound();
            }
            if(existingEmail!= null)
            {
                return BadRequest("Email ID is already exist");
            }
            else if(existingUserName!= null)
            {
                return BadRequest("User name is already exist");
            }
            else if (updateUser.FirstName == "")
            {
                return BadRequest("First Name is required");
            }
            else if (updateUser.LastName == "")
            {
                return BadRequest("Last Name is required");
            }
            else if (updateUser.Email == "")
            {
                return BadRequest("Email is required");
            }
            else if (updateUser.Phone == "")
            {
                return BadRequest("Phone number is required");
            }
            else if (updateUser.StreetAddress == "")
            {
                return BadRequest("Street Address is required");
            }
            else if(updateUser.Phone.Length != 10)
            {
                return BadRequest("Length of phone number should be 10");
            }
            else if (updateUser.UserName == "")
            {
                return BadRequest("User Name required");
            }
            else if (updateUser.Password.Length < 6)
            {
                return BadRequest("Password shold be 6 characters long");
            }
            else
            {
                user.FirstName = updateUser.FirstName;
                user.LastName = updateUser.LastName;
                user.Email = updateUser.Email;
                user.Phone = updateUser.Phone;
                user.StreetAddress = updateUser.StreetAddress;
                user.City = updateUser.City;
                user.State = updateUser.State;
                user.Password = updateUser.Password;
                user.UserName = updateUser.UserName;
                await _dbContext.SaveChangesAsync();
                return Ok(user);
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> deleteUser([FromRoute] int id)
        {
            var user = _dbContext.Users.Find(id);

            if(user == null)
            {
                return NotFound();
            }
            user.DeletedAt = DateTime.Now;
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
