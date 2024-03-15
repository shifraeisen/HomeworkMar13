using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HomeworkMar13.Data
{
    public class SimpleAdRepository
    {
        private readonly string _connectionString;

        public SimpleAdRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public List<SimpleAd> GetAllAds()
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "select * from SimpleAds order by DateCreated desc";

            List<SimpleAd> ads = new();

            connection.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                ads.Add(new SimpleAd
                {
                    Id = (int)reader["ID"],
                    UserID = (int)reader["UserID"],
                    Name = (string)reader["Name"],
                    Number = (string)reader["Number"],
                    Details = (string)reader["Details"],
                    DateCreated = (DateTime)reader["DateCreated"]
                });
            }
            return ads;
        }
        public void NewAd(SimpleAd ad, int userID)
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"insert into SimpleAds
                                values (@userID, @name, @num, @details, @datecreated)";
            cmd.Parameters.AddWithValue("@userID", userID);
            cmd.Parameters.AddWithValue("@name", ad.Name);           
            cmd.Parameters.AddWithValue("@num", ad.Number);
            cmd.Parameters.AddWithValue("@details", ad.Details);
            cmd.Parameters.AddWithValue("@datecreated", DateTime.Today);

            connection.Open();

            cmd.ExecuteNonQuery();
        }
        public void DeleteAd(int id)
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"delete from SimpleAds
                                where ID = @id";
            cmd.Parameters.AddWithValue("@id", id);

            connection.Open();

            cmd.ExecuteNonQuery();
        }
        public SimpleAd GetByID(int id)
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"select top 1 * from SimpleAds
                                where id = @id";
            cmd.Parameters.AddWithValue("@id", id);

            connection.Open();

            var reader = cmd.ExecuteReader();
            if(!reader.Read())
            {
                return null;
            }
            return new SimpleAd
            {
                Id = (int)reader["ID"],
                UserID = (int)reader["UserID"],
                Name = (string)reader["Name"],
                Number = (string)reader["Number"],
                Details  = (string)reader["Details"],
                DateCreated = (DateTime)reader["DateCreated"]
            };
        }
    }
}
