using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeworkMar13.Data
{
    public class UserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public void AddUser(User user, string password)
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(password);
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"insert into Users
                                values (@name, @email, @hash)
                                select SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@name", user.Name);
            cmd.Parameters.AddWithValue("@email", user.Email);
            cmd.Parameters.AddWithValue("@hash", hash);

            conn.Open();

            user.Id = (int)(decimal)cmd.ExecuteScalar();
        }
        public User Login(string email, string password)
        {
            var user = GetByEmail(email);
            if (user == null)
            {
                return null;
            }

            var isMatch = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!isMatch)
            {
                return null;
            }

            return user;
        }
        public User GetByEmail(string email)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"select top 1 * from Users
                                where email = @email";
            cmd.Parameters.AddWithValue("@email", email);

            conn.Open();

            var reader = cmd.ExecuteReader();

            if(!reader.Read())
            {
                return null;
            }
            return new User
            {
                Id = (int)reader["ID"],
                Name = (string)reader["Name"],
                Email = email,
                PasswordHash = (string)reader["PasswordHash"]
            };
        }
    }
}
