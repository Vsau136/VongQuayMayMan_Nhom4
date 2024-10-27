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
    public partial class FormH : Form
    {
        public FormH()
        {
            InitializeComponent();
        }

        void Intruc()
        {
            string intruction = @"Đối với chế độ solo:
        -Người chơi sẽ được thực hiện quay vòng quay 5 lần.Sau 5 lần quay nếu tổng số điểm của người chơi đạt trên 1000 điểm người chơi sẽ thắng.Ngược lại, người chơi sẽ được tính là thua.
        -Sau khi chơi xong bạn có thể nhấn nút chơi lại để tiếp tục trò chơi.
        Đối với chế độ So tài:
        -Chế độ này sẽ giành cho 2 người chơi.
        -Đầu tiên, hai người chơi sẽ tiến hành nhập tên vào các mục 'Nhập tên người chơi 1:' và 'Nhập tên người chơi 2:'
        Sau đó vòng quay sẽ mở ra và bắt đầu tính lượt quay của người chơi 1.
        - Mỗi người chơi sẽ có 5 lượt quay.
        Sau khi thực hiện xong 5 lượt quay sẽ có thông báo hiện lên đến lượt quay của người chơi 2.
        - Khi người chơi 2 hoàn tất quá trình quay, hệ thống sẽ so sánh số điểm của hai người chơi và công bố tên người chiến thắng.
        Sau khi chơi xong bạn có thể nhấn nút chơi lại để tiếp tục trò chơi. Đối với chế độ Kết nối:
        -Người chơi sẽ thiết lập địa chỉ Ip và port của mình để người khác có thể tìm
        Đối với chế độ Kết nối:
        - Người chơi sẽ thiết lập địa chỉ Ip và port của mình để người khác có thể tìm thấy và kết nối.
        - Sau khi kết nối hai người chơi sẽ được thay phiên nhau quay vòng quay may mắn.
        - Trò chơi sẽ kết thúc khi cả hai người chơi đều đã quay đủ 5 lượt.
        - Người chiến thắng sẽ là người có số điểm cao nhất.
        Đối với mục Xếp hạng:
        - Mục này sẽ hiện lên màng hình Top 5 người có tổng số điểm cao nhất trong chế độ Solo.
        - Đồng thời cũng thể hiện thứ hạn hiện tại của người chơi.
        Lưu ý: Bảng xếp hạng trên chỉ ghi nhận số điểm của người chơi từ chế độ Solo. 
        Đối với chế độ So tài và Kết nối bảng xếp hạng sẽ không ghi nhận kết quả.";
            MessageBox.Show(intruction, "Hướng Dẫn");
        }
        private void btnItt_Click(object sender, EventArgs e)
        {
            Intruc();
        }

        private void btnSL_Click(object sender, EventArgs e)
        {
            FormSL formSL = new FormSL(UserName);
            formSL.ShowDialog();
        }

        private void btnCnt_Click(object sender, EventArgs e)
        {
            FormCT formCT1 = new FormCT();
            FormCT formCT2 = new FormCT();

            formCT1.Show();  // Show the first player's form
            formCT2.Show();  // Show the second player's form
        }


        private void btnClg_Click(object sender, EventArgs e)
        {
            FormEntername formentername = new FormEntername();
            formentername.ShowDialog();
        }

        private void btnrnk_Click(object sender, EventArgs e)
        {
            FormRk formRk = new FormRk(UserName);
            formRk.ShowDialog();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
        public string UserName { get; set; }

        private void FormH_Load(object sender, EventArgs e)
        {
            lblChao.Text = "Xin chào " + UserName;
        }
    }
}
