using System;
using System.Drawing;
using System.Windows.Forms;

namespace FormLogin
{
    public partial class FormSL : Form
    {
        private int currentScore = 0;           // Điểm hiện tại
        private int remainingSpins = 5;         // Số lượt quay còn lại
        private readonly Random random;         // Đối tượng Random để tạo số ngẫu nhiên
        private readonly Timer spinTimer;       // Bộ đếm thời gian cho vòng quay
        private float spinAngle = 0f;           // Góc quay hiện tại của vòng quay
        private Image wheelImage;               // Hình ảnh của vòng quay

        private int remainingRounds;            // Số vòng quay còn lại
        private float currentSpeed = 0f;        // Tốc độ quay hiện tại
        private const int maxSpeed = 20;        // Tốc độ quay tối đa

        public FormSL()
        {
            InitializeComponent();
            random = new Random();
            spinTimer = new Timer
            {
                Interval = 30 // Khoảng thời gian mỗi lần cập nhật quay
            };
            spinTimer.Tick += SpinTimer_Tick;

            // Tải hình ảnh của vòng quay (đảm bảo hình ảnh tồn tại trong Resources)
            wheelImage = Properties.Resources.luckywheel1;
        }

        // Sự kiện khi nhấn nút quay
        private void button1_Click(object sender, EventArgs e)
        {
            if (remainingSpins > 0)
            {
                StartSpin();  // Bắt đầu quay khi còn lượt
            }
            else
            {
                MessageBox.Show("Bạn đã hết lượt quay!"); // Thông báo khi hết lượt quay
            }
        }

        // Hàm khởi động vòng quay
        private void StartSpin()
        {
            spinAngle = 0f;
            remainingSpins--;

            // Xác định số vòng quay ngẫu nhiên từ 150 đến 200
            remainingRounds = random.Next(100, 150);
            currentSpeed = 0f; // Bắt đầu với tốc độ bằng 0
            spinTimer.Start();
        }

        // Xử lý quay vòng theo mỗi Tick của Timer
        private void SpinTimer_Tick(object sender, EventArgs e)
        {
            // Tăng tốc độ quay trong khoảng 1/3 số vòng đầu tiên
            if (remainingRounds > (150 / 3))
            {
                if (currentSpeed < maxSpeed)
                {
                    currentSpeed += 0.5f; // Tăng tốc độ quay
                }
            }
            // Giảm tốc khi gần đến số vòng cuối cùng
            else if (remainingRounds < (150 / 3))
            {
                if (currentSpeed > 0)
                {
                    currentSpeed -= 0.5f; // Giảm tốc độ quay
                }
            }

            spinAngle += currentSpeed; // Cập nhật góc quay dựa vào tốc độ hiện tại

            if (spinAngle >= 360)
            {
                spinAngle -= 360; // Đặt lại góc quay khi vượt quá 360 độ
            }

            RotateWheel(spinAngle); // Xoay hình ảnh

            // Giảm số vòng quay còn lại
            remainingRounds--;

            // Kiểm tra nếu số vòng quay đã hết hoặc tốc độ bằng 0
            if (remainingRounds <= 0 || currentSpeed <= 0f)
            {
                spinTimer.Stop();
                CalculateScore(); // Tính điểm sau khi dừng quay
                                  // Kiểm tra nếu đã hết lượt quay
                if (remainingSpins == 0)
                {
                    CheckWinOrLose(); // Kiểm tra xem người chơi thắng hay thua
                }
            }
        }
        // Hàm kiểm tra kết quả thắng hoặc thua
        private void CheckWinOrLose()
        {
            if (currentScore > 1000)
            {
                MessageBox.Show("Chúc mừng! Bạn đã thắng trò chơi với tổng điểm: " + currentScore);
            }
            else
            {
                MessageBox.Show("Rất tiếc! Bạn đã thua. Tổng điểm của bạn là: " + currentScore);
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

            // Kiểm tra xem giá trị dừng là số hay hành động đặc biệt
            int earnedScore = 0;
            switch (result)
            {
                case "Chia đôi":
                    currentScore /= 2;  // Chia đôi điểm hiện tại
                    break;
                case "Nhân đôi":
                    currentScore *= 2;  // Nhân đôi điểm hiện tại
                    break;
                case "Thêm lượt":
                    remainingSpins+=2;   // Thêm một lượt quay
                    break;
                case "Mất lượt":
                    MessageBox.Show("Bạn đã mất lượt!");  // Mất lượt quay
                    break;
                default:
                    earnedScore = int.Parse(result);  // Chuyển đổi chuỗi thành số nguyên
                    currentScore += earnedScore;      // Cập nhật điểm số
                    break;
            }

            // Cập nhật giao diện
            label8.Text = currentScore.ToString();
            label11.Text = remainingSpins.ToString();
        }

        // Nút chơi lại trò chơi
        private void button2_Click(object sender, EventArgs e)
        {
            ResetGame(); // Đặt lại trò chơi
        }

        // Hàm đặt lại trò chơi
        private void ResetGame()
        {
            currentScore = 0;
            remainingSpins = 5;

            // Cập nhật lại giao diện
            label8.Text = currentScore.ToString();
            label11.Text = remainingSpins.ToString();
        }

        // Sự kiện đóng form
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close(); // Đóng form
        }
    }
}
