using Du_an_demo.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Du_an_demo.ViewModel
{
    public class AccountService
    {
        private readonly string connectionString;

        public AccountService()
        {
            connectionString = @"Data Source=DESKTOP-L5LPVBI\HOANGNEMAY;Initial Catalog=Restaurant_Managenment_FPTCode;Integrated Security=TRUE";

        }

        public Account Login(string username, string password)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"SELECT * FROM Accounts 
               WHERE Username COLLATE Latin1_General_CS_AS = @username 
                 AND Password = @password";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password); // Note: trong thực tế nên mã hóa

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new Account
                    {
                        Id = (int)reader["Id"],
                        Username = reader["Username"].ToString(),
                        Password = reader["Password"].ToString(),
                        Role = reader["Role"].ToString()
                    };
                }
            }
            return null;
        }
        public bool Register(Account newAccount)
        {
            // Kiểm tra xem tên đăng nhập đã tồn tại chưa
            if (IsUsernameExist(newAccount.Username))
            {
                return false; // Tên đăng nhập đã tồn tại
            }

            // Lưu tài khoản vào cơ sở dữ liệu
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"INSERT INTO Accounts (Username, Password, Role) 
                       VALUES (@username, @password, @role)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@username", newAccount.Username);
                cmd.Parameters.AddWithValue("@password", newAccount.Password); // Lưu mật khẩu dưới dạng plain text

                // Cung cấp giá trị cho @role (mặc định là "Staff")
                cmd.Parameters.AddWithValue("@role", "Staff");  // Hoặc bạn có thể sử dụng một giá trị động, ví dụ: newAccount.Role

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }



        // Kiểm tra nếu tên đăng nhập đã tồn tại
        private bool IsUsernameExist(string username)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"SELECT COUNT(1) FROM Accounts WHERE Username = @username";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@username", username);

                int count = (int)cmd.ExecuteScalar();
                return count > 0; // Trả về true nếu tên đăng nhập đã tồn tại
            }
        }
    }
}
