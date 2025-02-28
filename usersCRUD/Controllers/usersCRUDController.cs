using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Data;
using System.Data.SqlClient;


namespace usersCRUD.Controllers;

[Route("[controller]")]
[ApiController]
public class usersCRUDController : ControllerBase
{


    private readonly string _connectionString;
    public usersCRUDController(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("usersCRUD");
    }


    //-------------------------------------------------------------------
    //ADD NEW USERS INTO DATABASE(POST)
    //-------------------------------------------------------------------


    [HttpPost(Name = "sp_AddUserNew")]
    public IActionResult sp_AddUserNew([FromBody] UsersDetails user)
    {
        if (user == null)
        {
            return BadRequest("User data is required.");
        }

        using (SqlConnection conn = new(_connectionString))
        {
            SqlCommand cmd = new("sp_AddUserNew", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@userID", user.userID);
            cmd.Parameters.AddWithValue("@userName", user.userName);
            cmd.Parameters.AddWithValue("@userEmail", user.userEmail);
            cmd.Parameters.AddWithValue("@userPassword", user.userPassword);

            conn.Open();
            cmd.ExecuteNonQuery();

        }

        return Ok("User added successfully!");
    }

    //------------------------------------------------------------------------
    //FETCH ALL USERS FROM DATABASE(GET)
    //------------------------------------------------------------------------


    [HttpGet(Name = "sp_getAllUsers")]
    public IActionResult sp_getAllUsers()
    {
        List<UsersDetails> users = new();

        
            using (SqlConnection conn = new(_connectionString))
            {
                SqlCommand cmd = new("sp_getAllUsers", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new UsersDetails
                        {
                            userID = reader.GetInt32(0),
                            userName = reader.GetString(1),
                            userEmail = reader.GetString(2),
                            userPassword = reader.GetString(3)
                        });
                    }
                }
            }

            if (users.Count == 0)
                return NotFound("No users found.");

            return Ok(users);
        
       
    }



    //------------------------------------------------------------------------
    //FETCH SINGLE USER(GET BY ID)
    //------------------------------------------------------------------------
   

    [HttpGet("{id}")]
    public IActionResult sp_getUsersbyID(int id)
    {
        UsersDetails user = null;
        using (SqlConnection conn = new(_connectionString))
        {
            SqlCommand cmd = new("sp_getUsersbyID", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@userID", id);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                user = new UsersDetails
                {
                    userID = (int)reader["userID"],
                    userName = reader["userName"].ToString(),
                    userEmail = reader["userEmail"].ToString(),
                    userPassword = reader["userPassword"].ToString()
                };
            }
        }
        return user != null ? Ok(user) : NotFound("User not found");
    }


    //------------------------------------------------------------------------
    //UPDATE EXISTING USERS(PUT)
    //------------------------------------------------------------------------


    [HttpPut("{id}")]
    public IActionResult sp_UpdateUser(int id, [FromBody] UsersDetails user)
    {
        if (user == null)
        {
            return BadRequest("User data is required.");
        }

        using (SqlConnection conn = new(_connectionString))
        {
            SqlCommand cmd = new("sp_UpdateUser", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@userID", id);
            cmd.Parameters.AddWithValue("@userName", user.userName);
            cmd.Parameters.AddWithValue("@userEmail", user.userEmail);
            cmd.Parameters.AddWithValue("@userPassword", user.userPassword);

            conn.Open();
            int rowsAffected = cmd.ExecuteNonQuery();

            if (rowsAffected == 0)
            {
                return NotFound("User not found.");
            }
        }

        return Ok("User updated successfully!");
    }


    //------------------------------------------------------------------------
    //DELETE EXISTING USERS ON DATABASE(DELETE)
    //------------------------------------------------------------------------


    [HttpDelete("{id}")]
    public IActionResult sp_DeleteUser(int id)
    {
        using (SqlConnection conn = new(_connectionString))
        {
            SqlCommand cmd = new("sp_DeleteUser", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@userID", id);
            conn.Open();
            int rowsAffected = cmd.ExecuteNonQuery();

            if (rowsAffected == 0)
            {
                return NotFound("User not found.");
            }
        }

        return Ok("User deleted successfully!");
    }


}




