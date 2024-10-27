using System;
using System.Drawing;
using System.Windows.Forms;

namespace FormLogin
{
    public partial class FormCLG : Form
    {
        private string player1Name;
        private string player2Name;
        private int player1Score = 0;        // Điểm người chơi 1
        private int player2Score = 0;        // Điểm người chơi 2
        private readonly Timer spinTimer;    // Bộ đếm thời gian cho vòng quay

        private int player1Spins = 5;        // Số lượt quay của người chơi 1
        private int player2Spins = 5;        // Số lượt quay của người chơi 2
        private int currentPlayer = 1;       // Biến lưu người chơi hiện tại (1 hoặc 2)
        private readonly Random random;      // Đối tượng Random để tạo số ngẫu nhiên
        private float spinAngle = 0f;        // Góc quay hiện tại của vòng quay
        private float currentSpeed = 5f;     // Tốc độ quay hiện tại
        private float maxSpeed = 25f;        // Tốc độ quay tối đa
        private float minSpeed = 5f;         // Tốc độ quay tối thiểu
        private int remainingRounds;         // Số vòng còn lại trong quá trình quay
        private int maxRounds;               // Tổng số vòng mục tiêu
        private Image wheelImage;            // Hình ảnh của vòng quay


        public FormCLG(string player1Name, string player2Name)
        {
            InitializeComponent();
            this.player1Name = player1Name;
            this.player2Name = player2Name;

            // Hiển thị tên người chơi trong các label tương ứng
            label2.Text = player1Name;
            label4.Text = player2Name;

            // Cập nhật label cho lượt quay
            label14.Text = player1Name; // Hiển thị người chơi 1 là người bắt đầu

            // Khởi tạo các biến cần thiết
            random = new Random();
            spinTimer = new Timer
            {
                Interval = 30 // Khoảng thời gian mỗi lần cập nhật quay
            };
            spinTimer.Tick += spinTimer_Tick;

            // Tải hình ảnh của vòng quay
            wheelImage = Properties.Resources.luckywheel1;

            // Hiển thị thông tin người chơi ban đầu
            label11.Text = player1Spins.ToString(); // Lượt quay của người chơi 1
        }


        // Sự kiện khi nhấn nút quay
        private void button1_Click(object sender, EventArgs e)
        {
            if ((currentPlayer == 1 && player1Spins > 0) || (currentPlayer == 2 && player2Spins > 0))
            {
                StartSpin();  // Bắt đầu quay khi còn lượt
            }
            else
            {
                MessageBox.Show("Người chơi này đã hết lượt quay!");
              
            }
        }

        // Hàm khởi động vòng quay
        private void StartSpin()
        {
            spinAngle = 0f;  // Đặt lại góc quay về 0
            currentSpeed = 5f;  // Đặt tốc độ quay ban đầu
            remainingRounds = random.Next(100, 150); // Xác định số vòng quay mục tiêu (từ 100 đến 150)
            maxRounds = remainingRounds;  // Lưu lại số vòng tối đa để điều chỉnh tốc độ
            // Giảm số lượt quay dựa vào người chơi hiện tại
            if (currentPlayer == 1)
            {
                player1Spins--;
                label11.Text = player1Spins.ToString();
            }
            else
            {
                player2Spins--;
                label11.Text = player2Spins.ToString();
            }

            spinTimer.Start(); // Bắt đầu timer để xoay từ từ
        }

        // Sự kiện Tick của spinTimer để cập nhật góc quay
        private void spinTimer_Tick(object sender, EventArgs e)
        {
            // Tăng tốc độ quay trong 1/3 số vòng đầu tiên
            if (remainingRounds > (maxRounds / 3))
            {
                if (currentSpeed < maxSpeed)
                {
                    currentSpeed += 0.5f; // Tăng tốc độ quay
                }
            }
            // Giảm tốc trong 1/3 số vòng cuối cùng
            else if (remainingRounds < (maxRounds / 3))
            {
                if (currentSpeed > minSpeed)
                {
                    currentSpeed -= 0.5f; // Giảm tốc độ quay
                }
            }

            // Cập nhật góc quay dựa vào tốc độ hiện tại
            spinAngle += currentSpeed;

            // Đảm bảo góc quay không vượt quá 360 độ
            if (spinAngle >= 360)
            {
                spinAngle -= 360; // Đặt lại góc quay khi vượt quá 360 độ
            }

            RotateWheel(spinAngle); // Xoay hình ảnh vòng quay

            // Giảm số vòng quay còn lại
            remainingRounds--;

            // Kiểm tra nếu số vòng quay đã hết hoặc tốc độ bằng tối thiểu
            if (remainingRounds <= 0 || currentSpeed <= minSpeed)
            {
                spinTimer.Stop();
                CalculateScore(); // Tính điểm sau khi dừng quay
            }
        }

        // Xoay hình ảnh vòng quay
        private void RotateWheel(float angle)
        {
            if (wheelImage == null) return;

            // Tạo một Bitmap mới để vẽ vòng quay xoay
            Bitmap rotatedImage = new Bitmap(wheelImage.Width, wheelImage.Height);
            using (Graphics g = Graphics.FromImage(rotatedImage))
            {
                // Thiết lập chất lượng hình ảnh
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.TranslateTransform(wheelImage.Width / 2, wheelImage.Height / 2); // Dịch tâm ra giữa
                g.RotateTransform(angle); // Xoay theo góc
                g.TranslateTransform(-wheelImage.Width / 2, -wheelImage.Height / 2); // Đặt lại vị trí vẽ
                g.DrawImage(wheelImage, new Point(0, 0)); // Vẽ hình ảnh đã xoay
            }

            // Cập nhật hình ảnh đã xoay vào PictureBox
            pictureBox1.Image?.Dispose(); // Xóa hình ảnh cũ để giải phóng bộ nhớ
            pictureBox1.Image = rotatedImage;

            // Bắt buộc PictureBox vẽ lại ngay lập tức
            pictureBox1.Invalidate();
            pictureBox1.Refresh();
        }

        // Tính toán điểm dựa vào kết quả quay
        // Tính toán điểm dựa vào kết quả quay
        private void CalculateScore()
        {
            // Các giá trị trên vòng quay
            string[] scores = { "500", "Chia đôi", "2000", "900", "Thêm lượt", "-400", "Nhân đôi", "200", "-700", "Mất lượt", "350", "1000" };

            // Xác định vị trí dừng dựa vào góc quay (spinAngle)
            int totalSections = scores.Length; // Số lượng phần trên vòng quay (12 phần)
            float anglePerSection = 360f / totalSections; // Mỗi phần có bao nhiêu độ

            // Tính vị trí dừng dựa vào góc quay hiện tại
            int stopPosition = (int)(spinAngle / anglePerSection) % totalSections;

            // Xử lý giá trị dựa trên vị trí dừng
            string result = scores[stopPosition];
            int earnedScore = 0;

            // Kiểm tra xem giá trị dừng là số hay hành động đặc biệt
            switch (result)
            {
                case "Chia đôi":
                    if (currentPlayer == 1)
                        player1Score /= 2;
                    else
                        player2Score /= 2;
                    break;
                case "Nhân đôi":
                    if (currentPlayer == 1)
                        player1Score *= 2;
                    else
                        player2Score *= 2;
                    break;
                case "Thêm lượt":
                    if (currentPlayer == 1)
                        player1Spins += 2;
                    else
                        player2Spins += 2;
                    break;
                case "Mất lượt":
                    MessageBox.Show("Bạn đã mất lượt!");
                    break;
                default:
                    earnedScore = int.Parse(result);
                    if (currentPlayer == 1)
                    {
                        player1Score += earnedScore;
                    }
                    else
                    {
                        player2Score += earnedScore;
                    }
                    break;
            }

            // Cập nhật điểm số
            labelPlayer1Score.Text = player1Score.ToString();
            labelPlayer2Score.Text = player2Score.ToString();
            if (currentPlayer == 1)
            {
                // Cập nhật giao diện
                label8.Text = player1Score.ToString();
                label11.Text = player1Spins.ToString();
            }
            else
            {
                // Cập nhật giao diện
                label8.Text = player2Score.ToString();
                label11.Text = player2Spins.ToString();
            }

            // Chuyển lượt hoặc kết thúc trò chơi
            if (currentPlayer == 1 && player1Spins == 0)
            {
                MessageBox.Show("Người chơi 1 đã hết lượt, đến lượt người chơi 2!");
                // Hiển thị dòng chữ "nc2"
                label14.Text = "NC2";

                currentPlayer = 2;
                player2Score = 0;
                player2Spins = 5;

                // Cập nhật lại giao diện
                label8.Text = player2Score.ToString();
                label11.Text = player2Spins.ToString();
            }
            else if (currentPlayer == 2 && player2Spins == 0)
            {
                MessageBox.Show("Người chơi 2 đã hết lượt, trò chơi kết thúc!");
                // Khi cả hai người chơi đã hết lượt, so sánh điểm số
                if (player1Score > player2Score)
                {
                    MessageBox.Show("Người chơi 1 giành chiến thắng với " + player1Score + " điểm!");
                }
                else if (player2Score > player1Score)
                {
                    MessageBox.Show("Người chơi 2 giành chiến thắng với " + player2Score + " điểm!");
                }
                else
                {
                    MessageBox.Show("Hòa! Cả hai người chơi đều có số điểm bằng nhau.");
                }
               
                spinTimer.Stop();
            }
        }

        // Nút chơi lại trò chơi
        private void button2_Click(object sender, EventArgs e)
        {
            ResetGame(); // Đặt lại trò chơi
        }

        // Hàm đặt lại trò chơi
        private void ResetGame()
        {
            player1Score = 0;
            player2Score = 0;
            player1Spins = 5;
            player2Spins = 5;
            currentPlayer = 1;

            // Cập nhật lại giao diện
            labelPlayer1Score.Text = player1Score.ToString();
            labelPlayer2Score.Text = player2Score.ToString();
            label8.Text = player1Score.ToString();
            label11.Text = player1Spins.ToString();
            // Nếu có thông báo "NC2" đang hiển thị, ẩn nó đi
            label14.Text = "NC1";

            // Bạn có thể thêm thông báo hoặc logic khác nếu cần
            MessageBox.Show("Trò chơi đã được đặt lại, người chơi 1 bắt đầu!");
        }

        // Sự kiện đóng form
        private void btnClose_Click(object sender, EventArgs e)
        {
            
        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void btnClose_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void labelPlayer1Score_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}