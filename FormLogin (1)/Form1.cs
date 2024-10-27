using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormLogin
{
    public partial class frmlogin : Form
    {
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
            if ((txtName.Text == "") || (txtPass.Text == ""))
            {
                MessageBox.Show("Vui lòng điền thông tin!", "Thông báo");
            }
            else
            {
                if((txtName.Text =="Nhom4")&&(txtPass.Text == "12345"))
                {
                    MessageBox.Show("Bạn đã đăng nhập thành công", "Thông báo");
                    this.Hide();
                    FormH home = new FormH();
                    home.Show();
                }
                else
                {
                    MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng, vui lòng nhập lại!", "Thông báo");
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
