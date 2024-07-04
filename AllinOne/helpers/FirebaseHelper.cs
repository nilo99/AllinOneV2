using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;

namespace AllinOne.helpers
{
    public class FirebaseHelper
    {
        private IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "KYAucGnJXxK6yUMZJ5LT4kO2yQpNmKK3jXr1DoVe",
            BasePath = "https://allinonev2-e908d-default-rtdb.europe-west1.firebasedatabase.app/"
        };

        private IFirebaseClient client;

        public FirebaseHelper()
        {
            client = new FireSharp.FirebaseClient(config);
            if (client == null)
            {
                throw new Exception("Failed to connect to Firebase.");
            }
        }

        public async Task<bool> UsernameExists(string username)
        {
            FirebaseResponse response = await client.GetAsync("Users");
            var users = response.ResultAs<Dictionary<string, User>>();

            if (users != null && users.Values.Any(user => user.Username == username))
            {
                return true;
            }

            return false;
        }

        public async Task<bool> EmailExists(string email)
        {
            FirebaseResponse response = await client.GetAsync("Users");
            var users = response.ResultAs<Dictionary<string, User>>();

            if (users != null && users.Values.Any(user => user.Email == email))
            {
                return true;
            }

            return false;
        }

        public async Task RegisterUser(string username, string email, string password)
        {
            string userId = Guid.NewGuid().ToString();
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new
            {
                Id = userId,
                Username = username,
                Email = email,
                Password = hashedPassword
            };
            SetResponse response = await client.SetAsync($"Users/{userId}", user);
        }


        public async Task<User> GetUsernameOrEmail(string identifier)
        {
            FirebaseResponse response = await client.GetAsync("Users");
            var users = response.ResultAs<Dictionary<string, User>>();

            if (users != null)
            {
                return users.Values.FirstOrDefault(user => user.Username == identifier || user.Email == identifier);
            }
            return null;
        }

        public bool VerifyPassword(string enteredPassword, string storedHashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(enteredPassword, storedHashedPassword);
        }


    }
    public class User
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

    }
}
