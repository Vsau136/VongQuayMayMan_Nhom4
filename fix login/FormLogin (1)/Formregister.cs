using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace FormLogin
{
    public partial class Formregister : Form
    {
        private string connectionString = "Data Source=MINHTIENVICTUS1;Initial Catalog=VongQuayDB;Integrated Security=True";
        public Formregister()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Formregister_Load(object sender, EventArgs e)
        {

        }

        private void textBox2_Click(object sender, EventArgs e)
        {
            txtName.Text = " ";
        }

        private void textBox3_Click(object sender, EventArgs e)
        {
            txtPassword.Text = " ";
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string name = txtName.Text;
            string password = txtPassword.Text;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO [Users] (Name, Password) VALUES (@Name, @Password)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", name);
                        command.Parameters.AddWithValue("@Password", password);

                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Đăng ký thành công!");
                            frmlogin loginForm = new frmlogin();  // Tạo instance của Form Đăng nhập
                            loginForm.Show();  // Hiển thị lại Form Đăng nhập
                            this.Hide();  // Đóng Form hiện tại (Form Đăng ký)
                        }
                        else
                        {
                            MessageBox.Show("Đăng ký thất bại.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }
    }
}
