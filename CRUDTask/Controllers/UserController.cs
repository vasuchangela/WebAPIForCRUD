using CRUDTask.Data;
using CRUDTask.DataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text.RegularExpressions;

namespace CRUDTask.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : Controller
    {
        private readonly UserCrudContext _dbContext;
        private IConfiguration _configuration;
        public UserController(UserCrudContext userCrudContext, IConfiguration configuration)
        {
            _dbContext = userCrudContext;
            _configuration = configuration;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllUsers(int pg, int pageSize, string searchText)
        {
            var users = new List<User>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("defaultConnection")))
            {
                await connection.OpenAsync();

                var command = new SqlCommand("GetAllUsers", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@pg", pg);
                command.Parameters.AddWithValue("@pageSize", pageSize);
                command.Parameters.AddWithValue("@searchText", searchText);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var user = new User
                        {
                            Id = (int)reader["Id"],
                            FirstName = (string)reader["FirstName"],
                            LastName = (string)reader["LastName"],
                            Email = (string)reader["Email"],
                            StreetAddress = (string)reader["StreetAddress"],
                            City = (string)reader["City"],
                            State = (string)reader["State"],
                            UserName = (string)reader["UserName"],
                            Password = (string)reader["Password"],
                            Phone = (string)reader["Phone"],
                        };
                        users.Add(user);
                    }

                    await reader.NextResultAsync();
                    if (await reader.ReadAsync())
                    {
                        var totalPage = (int)reader["TotalPage"];
                        var response = new
                        {
                            TotalEntries = users.Count(),
                            user = users,
                            totalPage = totalPage
                        };
                        return Ok(response);
                    }
                }
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> addUser([FromBody] User userRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                var existingUserName = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserName == userRequest.UserName);
                var existingEmail = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == userRequest.Email);
                if (existingEmail != null)
                {
                    return BadRequest("Email ID is already exist");
                }
                else if (existingUserName != null)
                {
                    return BadRequest("User name is already exist");
                }
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else if(existingEmail!= null)
            {
                return BadRequest("Email ID is already exist");
            }
            else if(existingUserName!= null)
            {
                return BadRequest("User name is already exist");
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
            _dbContext.SaveChanges();
            return Ok();
        }
    }
}
