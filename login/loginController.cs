using Microsoft.AspNetCore.Http;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace ITools.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class loginController : ControllerBase
    {
        /*
        private static List<login> users = new List<login>
            {
                new login{
                    Id = 1,
                    Username = "user",
                    Password = "123456"
                },
                new login{
                    Id = 2,
                    Username = "admin",
                    Password="pas1234"
                }
        };
        [HttpGet]
        public async Task<ActionResult<List<login>>> Get() //here is specify what it should return for example a List/Array is returned like that
        {
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<login>> Get(int id) //here is speficied to get only a single User by Id, if it is not found respond with bad request
        {
            var user = users.Find(u => u.Id == id);
            if(user == null) { 
                return BadRequest("User not found!");
            }
            return Ok(user);
        }

        [HttpPut]
        public async Task<ActionResult<login>> UpdateUser(login request) //here is speficied to update the User with the same Id as the requested User
        {
            var user = users.Find(u => u.Id == request.Id);
            if (user == null)
            {
                return BadRequest("User not found!");
            }
            user.Username = request.Username;
            user.Password = request.Password;
            return Ok(user);
        }
        
         
        [HttpDelete("{id}")]
        public async Task<ActionResult<login>> Delete(int id) //here is speficied to get only a single User by Id, if it is not found respond with bad request
        {
            var user = users.Find(u => u.Id == id);
            if (user == null)
            {
                return BadRequest("User not found!");
            }
            users.Remove(user);
            return Ok(users);
        }*/

        [HttpPost]
        public async Task<ActionResult<List<login>>> AddUser(login user) //here is specify what it should add and return for example a List/Array is returned like that
        {
            /*
            var exists = users.Find(u => u.Id == user.Id);
            if (exists != null)
            {
                return BadRequest("User with this Id already exists!");
            }
            users.Add(user);*/
            //Console.WriteLine(user);
            string server = "server";
            string database = "database";
            string username = "username";
            string password = "password";
            string port = "port";
            MySqlConnection conn = new($"server={server};database={database};username={username};password={password};port={port}");
            await conn.OpenAsync();
            try
            {
                if (conn.State == ConnectionState.Open)
                {
                    //insert salt here
                    string pas = user.Password;
                    //string hash = ComputeHash(pas, "SHA256", salt);
                    //string Query = "INSERT INTO table ( UserId, username, password ) VALUES (" + user.Id.ToString() + ",'" + user.Username.ToString() + "','" + hash.ToString() + "');";
                    //Console.WriteLine(user.Username);
                    string Query = "SELECT password FROM users WHERE username = '" + user.Username.ToString() + "';";
                    MySqlCommand cmd = new(Query, conn);
                    var MyReader = await cmd.ExecuteScalarAsync();
                    if (MyReader == null)
                    {
                        conn.Close();
                        return Unauthorized();
                    }
                    //Console.WriteLine(MyReader.ToString());
                    //NEED FIXING HERE
                    string hashed = MyReader.ToString();
                    //bool pwdHashed = VerifyHash(hash, hashed);
                    bool pwdHashed = VerifyHash(pas, "SHA256", hashed);
                    //RETURNS ALWAYS FALSE
                    //Console.WriteLine(pwdHashed);
                    if (pwdHashed == false)
                    {
                        await conn.CloseAsync();
                        return Unauthorized();
                    }
                    await conn.CloseAsync();
                    return Ok();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        /* 
        public static bool VerifyHash(string hash, string hashed)
        {
            if (hash.Equals(hashed)) { return true; } else { return false; }
        }
        //generates a hash with the salt
        */
        public static string ComputeHash(string plainText, string hashAlgorithm, byte[] saltBytes)
        {
            // Salt size
            saltBytes = new byte[8];

            // Convert Text to Array
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            // Create Array
            byte[] plainTextSaltBytes =
                    new byte[plainTextBytes.Length + saltBytes.Length];

            // Copy Text into Array
            for (int i = 0; i < plainTextBytes.Length; i++)
                plainTextSaltBytes[i] = plainTextBytes[i];

            // Append Salt
            for (int i = 0; i < saltBytes.Length; i++)
                plainTextSaltBytes[plainTextBytes.Length + i] = saltBytes[i];

            // Algorithm
            HashAlgorithm hash;

            // Initialize Class
            switch (hashAlgorithm.ToUpper())
            {
                case "SHA1":
                    hash = new SHA1Managed();
                    break;

                case "SHA256":
                    hash = new SHA256Managed();
                    break;

                case "SHA384":
                    hash = new SHA384Managed();
                    break;

                case "SHA512":
                    hash = new SHA512Managed();
                    break;

                default:
                    hash = new MD5CryptoServiceProvider();
                    break;
            }

            byte[] hashBytes = hash.ComputeHash(plainTextSaltBytes);

            byte[] hashSaltBytes = new byte[hashBytes.Length +
                                                saltBytes.Length];

            // Hash to Array
            for (int i = 0; i < hashBytes.Length; i++)
                hashSaltBytes[i] = hashBytes[i];

            // Append Salt
            for (int i = 0; i < saltBytes.Length; i++)
                hashSaltBytes[hashBytes.Length + i] = saltBytes[i];

            // Convert result into a base64-encoded string.
            string hashValue = Convert.ToBase64String(hashSaltBytes);

            // Return Result
            return hashValue;
        }
         
        public static bool VerifyHash(string plainText, string hashAlgorithm, string hashValue)
        {
            // Base64-encoded hash
            byte[] hashWithSaltBytes = Convert.FromBase64String(hashValue);

            // Hash without Salt
            int hashSizeInBits, hashSizeInBytes;

            // Size of hash is based on the specified algorithm.
            switch (hashAlgorithm.ToUpper())
            {
                case "SHA1":
                    hashSizeInBits = 160;
                    break;

                case "SHA256":
                    hashSizeInBits = 256;
                    break;

                case "SHA384":
                    hashSizeInBits = 384;
                    break;

                case "SHA512":
                    hashSizeInBits = 512;
                    break;

                default: // MD5
                    hashSizeInBits = 128;
                    break;
            }

            // Convert to bytes.
            hashSizeInBytes = hashSizeInBits / 8;

            // Verify Hash Length
            if (hashWithSaltBytes.Length < hashSizeInBytes)
                return false;

            // Array to hold Salt
            byte[] saltBytes = new byte[hashWithSaltBytes.Length -
                                        hashSizeInBytes];

            // Salt to New Array
            for (int i = 0; i < saltBytes.Length; i++)
                saltBytes[i] = hashWithSaltBytes[hashSizeInBytes + i];

            string expectedHashString = ComputeHash(plainText, hashAlgorithm, saltBytes);
            return (hashValue == expectedHashString);
        }
        
    }
}
