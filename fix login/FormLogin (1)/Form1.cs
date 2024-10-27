using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.IO;
using System.Data.SQLite;



namespace FormLogin
{
    public partial class frmlogin : Form
    {
        private string connectionString = "Data Source=MINHTIENVICTUS1;Initial Catalog=VongQuayDB;Integrated Security=True";
        //private string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\VongQuayDB.sql;Integrated Security=True;";
        //private string connectionString = @"Data Source=D:\HK5\fix login\FormLogin\bin\Debug\VongQuayDB.sql;Version=3;";
        //private string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\VongQuayDB.sql;Integrated Security=True;";
        public frmlogin()
        {
            InitializeComponent();
            
        }
        public new void Show()
        {
            base.Show();
        }
        private void frmlogin_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string name = txtUser.Text;
            string password = txtPassword.Text;

            using (SqlConnection connection = new SqlConnection(connectionString))
            //using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT COUNT(1) FROM [Users] WHERE Name = @Name AND Password = @Password";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    //using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", name);
                        command.Parameters.AddWithValue("@Password", password);

                        int count = Convert.ToInt32(command.ExecuteScalar());

                        if (count == 1)
                        {
                            MessageBox.Show("Đăng nhập thành công!");
                            // Chuyển sang trang khác (form khác) nếu cần
                            FormH mainForm = new FormH();
                            mainForm.UserName = txtUser.Text;
                            mainForm.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Formregister formregister = new Formregister();
            formregister.Show();
        }
    }
}
