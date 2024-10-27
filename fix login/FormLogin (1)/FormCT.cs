using System;
using System.Collections.Generic;
using System.Drawing;
using System.Media;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.Text;
using System.Windows.Forms;

namespace FormLogin
{
    public partial class FormCT : Form
    {
        private int currentScore = 0;           // Điểm hiện tại
        private int remainingSpins = 5;         // Số lượt quay còn lại
        private readonly Random random;         // Đối tượng Random để tạo số ngẫu nhiên
        private readonly Timer spinTimer;       // Bộ đếm thời gian cho vòng quay
        private float spinAngle = 0f;           // Góc quay hiện tại của vòng quay
        private Image wheelImage;               // Hình ảnh của vòng quay
        private int currentPlayer = 1;          // Người chơi hiện tại (1 hoặc 2)
        private int player1Score = 0, player2Score = 0; // Điểm của người chơi 1 và 2
        private int player1Spins = 5, player2Spins = 5; // Lượt quay của người chơi
        private TcpListener listener;           // Listener cho chế độ kết nối
        private TcpClient client;      // Client cho chế độ kết nối
        private NetworkStream stream;

        private float currentSpeed = 5f;
        private float maxSpeed = 25f;
        private float minSpeed = 5f;
        private int remainingRounds;
        private int maxRounds;


        public FormCT()
        {
            InitializeComponent();
            random = new Random();
            spinTimer = new Timer
            {
                Interval = 30 // Khoảng thời gian mỗi lần cập nhật quay
            };
            spinTimer.Tick += spinTimer_Tick;

            // Tải hình ảnh của vòng quay
            wheelImage = Properties.Resources.luckywheel1;

            button1.Enabled = false;

            // Hiển thị thông tin người chơi ban đầu
            label11.Text = player1Spins.ToString();
            DisplayIpAddress();
        }


        private void button3_Click(object sender, EventArgs e)
        {
            int port;
            if (int.TryParse(textBox3.Text, out port))
            {
                StartServer(port);
            }
            else
            {
                MessageBox.Show("Cổng không hợp lệ!");
            }
        }

        private void StartServer(int port)
        {
            try
            {
                IPAddress ip = IPAddress.Parse(textBox1.Text); // Chuyển đổi IP từ chuỗi
                listener = new TcpListener(ip, port);
                listener.Start();
                MessageBox.Show("Server đã bắt đầu. Đang chờ client...");
                listener.BeginAcceptTcpClient(new AsyncCallback(AcceptClientCallback), listener);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể bắt đầu server: " + ex.Message);
            }
        }

        private void AcceptClientCallback(IAsyncResult ar)
        {
            try
            {
                TcpListener serverListener = (TcpListener)ar.AsyncState;
                client = serverListener.EndAcceptTcpClient(ar);
                stream = client.GetStream();

                // Kiểm tra nếu là lượt của người chơi 1
                if (currentPlayer == 1)
                {
                    button1.Enabled = true; // Mở khóa nút quay cho người chơi 1 (server)
                }
                else
                {
                    button1.Enabled = true; // Khóa nút quay cho người chơi 2
                }

                BeginReceiveData();
            }
            catch (Exception)
            {
                // Xử lý lỗi nếu cần
            }
        }



        private void button4_Click(object sender, EventArgs e)
        {
            string ip = textBox2.Text;
            int port;
            if (int.TryParse(textBox4.Text, out port))
            {
                ConnectToServer(ip, port);
            }
            else
            {
                MessageBox.Show("Cổng không hợp lệ!");
            }
        }

        private void ConnectToServer(string ip, int port)
        {
            try
            {
                client = new TcpClient(ip, port);
                stream = client.GetStream();
                MessageBox.Show("Đã kết nối tới server!");

                // Mặc định khóa nút quay cho người chơi 2 (client) và đợi tín hiệu từ server để mở khóa
                button1.Enabled = true;

                BeginReceiveData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể kết nối tới server: " + ex.Message);
            }
        }


        private void BeginReceiveData()
        {
            try
            {
                byte[] buffer = new byte[1024];
                stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(ReceiveDataCallback), buffer);
            }
            catch (Exception ex)
            {
            }
        }
        private void UpdateScores(string receivedData)
        {
            // Kiểm tra nếu `receivedData` là thông báo khóa nút quay của client
            if (receivedData == "LOCK_CLIENT_SPIN")
            {
                DisableSpinButton(); // Khóa nút quay của client
                return;
            }

            // Kiểm tra nếu `receivedData` là thông báo khóa nút quay của client
            if (receivedData == "End_Game")
            {
                DisableSpinButton(); // Khóa nút quay của client
                return;
            }

            // Kiểm tra nếu `receivedData` là thông báo chuyển lượt
            if (receivedData == "CHANGE_TURN_TO_PLAYER2")
            {
                currentPlayer = 2;
                UnlockSpinButton();  // Mở khóa nút quay cho người chơi 2
                return;
            }

            // Xử lý nếu `receivedData` là điểm số
            int receivedScore;
            if (int.TryParse(receivedData, out receivedScore))
            {
                if (currentPlayer == 1)
                {
                    player1Score = receivedScore;
                    if (label8.InvokeRequired)
                    {
                        label8.Invoke(new Action(() => label8.Text = player1Score.ToString()));
                    }
                    else
                    {
                        label8.Text = player1Score.ToString(); // Cập nhật điểm lên giao diện cho người chơi 1
                    }
                }
                else if (currentPlayer == 2)
                {
                    player2Score = receivedScore;
                    if (label3.InvokeRequired)
                    {
                        label3.Invoke(new Action(() => label3.Text = player2Score.ToString()));
                    }
                    else
                    {
                        label3.Text = player2Score.ToString(); // Cập nhật điểm lên giao diện cho người chơi 2
                    }
                }
            }
            else
            {
            }
        }


        private void ReceiveDataCallback(IAsyncResult ar)
        {
            try
            {
                byte[] buffer = (byte[])ar.AsyncState;
                int bytesRead = stream.EndRead(ar);
                if (bytesRead > 0)
                {
                    string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    if (receivedData == "REPLAY_REQUEST")
                    {
                        // Hiển thị xác nhận cho người chơi
                        DialogResult result = MessageBox.Show("Bạn có muốn chơi lại không?", "Xác nhận chơi lại", MessageBoxButtons.YesNo);
                        if (result == DialogResult.Yes)
                        {
                            // Gửi phản hồi xác nhận chơi lại
                            SendData("REPLAY_CONFIRM");
                            ResetGame(); // Reset điểm và lượt chơi
                        }
                        else
                        {
                            // Gửi yêu cầu ngắt kết nối đến người chơi khác
                            SendData("DISCONNECT");
                            CloseApplication(); // Đóng cửa sổ hiện tại
                        }
                    }
                    else if (receivedData == "REPLAY_CONFIRM")
                    {
                        // Reset game khi người chơi còn lại đồng ý chơi lại
                        ResetGame();

                    }
                    else if (receivedData == "DISCONNECT")
                    {
                        // Đóng cửa sổ hiện tại khi nhận yêu cầu ngắt kết nối
                        CloseApplication();
                    }
                    if (receivedData != "LOCK_CLIENT_SPIN")
                    {
                        MessageBox.Show("Nhận được: " + receivedData);
                    }
                    UpdateScores(receivedData); // Cập nhật điểm từ server

                    // Kiểm tra thông báo chuyển lượt
                    if (receivedData == "CHANGE_TURN_TO_PLAYER2")
                    {
                        currentPlayer = 2;
                        UnlockSpinButton();
                    }
                    else if (receivedData == "End_Game")
                    {
                        EndGame();
                    }

                    // Tiếp tục nhận dữ liệu
                    BeginReceiveData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi nhận dữ liệu: " + ex.Message);
            }
        }


        private void SendData(string data)
{
    try
    {
        if (stream != null && stream.CanWrite)
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            stream.Write(dataBytes, 0, dataBytes.Length);
        }
        else
        {
            MessageBox.Show("Stream không hợp lệ. Không thể gửi dữ liệu.");
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show("Lỗi khi gửi dữ liệu: " + ex.Message);
    }
}


        // Sự kiện khi nhấn nút quay
        private void button1_Click(object sender, EventArgs e)
        {
            if ((currentPlayer == 1 && player1Spins > 0) || (currentPlayer == 2 && player2Spins > 0))
            {
                StartSpin();
            }
            else
            {
                MessageBox.Show("Người chơi này đã hết lượt quay!");
                DisableSpinButton();
            }
        }


        // Hàm khởi động vòng quay cho người chơi
        private void StartSpin()
        {
            // Đảm bảo người chơi còn lượt trước khi tiến hành quay
            if ((currentPlayer == 1 && player1Spins <= 0) || (currentPlayer == 2 && player2Spins <= 0))
            {
                MessageBox.Show("Người chơi này đã hết lượt quay!");
                DisableSpinButton(); // Vô hiệu hóa nút quay
                return; // Thoát khỏi phương thức
            }

            spinAngle = 0f;
            currentSpeed = 5f;
            remainingRounds = random.Next(200, 250);
            maxRounds = remainingRounds;

            if (currentPlayer == 1)
            {
                player1Spins--;
                label11.Text = player1Spins.ToString();

                // Gửi thông báo khóa nút quay đến client
                SendData("LOCK_CLIENT_SPIN");
            }
            else
            {
                player2Spins--;
                label11.Text = player2Spins.ToString();
            }

            spinTimer.Start();
        }


        // Sự kiện khi nhấn nút quay
        private void DisableSpinButton()
        {
            button1.Enabled = false; // Disable the spin button
        }

        private void spinTimer_Tick(object sender, EventArgs e)
        {
            if (remainingRounds > (maxRounds / 3))
            {
                if (currentSpeed < maxSpeed)
                {
                    currentSpeed += 0.5f;
                }
            }
            else if (remainingRounds < (maxRounds / 3))
            {
                if (currentSpeed > minSpeed)
                {
                    currentSpeed -= 0.5f;
                }
            }

            spinAngle += currentSpeed;

            if (spinAngle >= 360)
            {
                spinAngle -= 360;
            }

            RotateWheel(spinAngle);
            remainingRounds--;

            if (remainingRounds <= 0 || currentSpeed <= minSpeed)
            {
                spinTimer.Stop();


                CalculateScore();
            }
        }

        private void CalculateScore()
        {
            string[] scores = { "500", "Chia đôi", "2000", "900", "Thêm lượt", "-400", "Nhân đôi", "200", "-700", "Mất lượt", "350", "1000" };
            int totalSections = scores.Length;
            float anglePerSection = 360f / totalSections;
            int stopPosition = (int)(spinAngle / anglePerSection) % totalSections;
            string result = scores[stopPosition];
            int earnedScore = 0;

            switch (result)
            {
                case "Chia đôi":
                    if (currentPlayer == 1) player1Score /= 2;
                    else player2Score /= 2;
                    break;
                case "Nhân đôi":
                    if (currentPlayer == 1) player1Score *= 2;
                    else player2Score *= 2;
                    break;
                case "Thêm lượt":
                    if (currentPlayer == 1) player1Spins += 2;
                    else player2Spins += 2;
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

            // Cập nhật điểm lên giao diện
            label8.Text = player1Score.ToString();
            label3.Text = player2Score.ToString();


            // Check if both players have no spins left
            if (player1Spins == 0 && player2Spins == 0)
            {
                EndGame();
            }
            else
            {
                if (currentPlayer == 1)
                {
                    label9.Text = player1Score.ToString();
                    label11.Text = player1Spins.ToString();
                }
                else
                {
                    label9.Text = player2Score.ToString();
                    label11.Text = player2Spins.ToString();
                }

                // Truyền điểm tới người chơi khác và kiểm tra điều kiện chuyển lượt
                SendData(currentPlayer == 1 ? player1Score.ToString() : player2Score.ToString());

                if (currentPlayer == 1 && player1Spins == 0)
                {
                    MessageBox.Show("Người chơi 1 đã hết lượt, đến lượt người chơi 2!");
                    currentPlayer = 2;
                    LockSpinButton();
                    SendData("CHANGE_TURN_TO_PLAYER2"); // Thông báo cho client chuyển lượt sang người chơi 2
                }
                else if (currentPlayer == 2 && player2Spins == 0)
                {
                    MessageBox.Show("Người chơi 2 đã hết lượt");
                    UnlockSpinButton();
                    SendData("End_Game");
                }
            }
        }


        // Khóa nút quay
        private void LockSpinButton()
        {
            button1.Enabled = false;
        }

        // Mở khóa nút quay
        private void UnlockSpinButton()
        {
            button1.Enabled = true;
        }

        private void EndGame()
        {
            spinTimer.Stop(); // Stop the spin timer
            string winnerMessage;

            if (player1Score > player2Score)
            {
                winnerMessage = "Người chơi 1 thắng!";
            }
            else if (player2Score > player1Score)
            {
                winnerMessage = "Người chơi 2 thắng!";
            }
            else
            {
                winnerMessage = "Hòa!";
            }

            MessageBox.Show(winnerMessage);
            // Optionally, you can reset the game or provide options for a new game
        }

        private void RotateWheel(float angle)
        {
            if (wheelImage == null) return;

            Bitmap rotatedImage = new Bitmap(wheelImage.Width, wheelImage.Height);
            using (Graphics g = Graphics.FromImage(rotatedImage))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.TranslateTransform(wheelImage.Width / 2, wheelImage.Height / 2);
                g.RotateTransform(angle);
                g.TranslateTransform(-wheelImage.Width / 2, -wheelImage.Height / 2);
                g.DrawImage(wheelImage, new Point(0, 0));
            }

            pictureBox1.Image?.Dispose();
            pictureBox1.Image = rotatedImage;
            pictureBox1.Invalidate();
            pictureBox1.Refresh();
        }


        private void DisplayIpAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    textBox1.Text = ip.ToString(); // Hiển thị địa chỉ IP lên textbox
                    break;
                }
            }
        }
        private void RequestReplay()
        {
            // Gửi yêu cầu chơi lại đến người chơi còn lại
            SendData("REPLAY_REQUEST");
        }
        private void button2_Click(object sender, EventArgs e)
        {
            // Yêu cầu chơi lại
            RequestReplay();
        }
        private void ResetGame()
        {
            player1Score = 0;
            player2Score = 0;
            player1Spins = 5;
            player2Spins = 5;
            currentPlayer = 1;

            // Cập nhật giao diện
            label8.Text = player1Score.ToString();
            label3.Text = player2Score.ToString();
            label11.Text = player1Spins.ToString();
            label9.Text = player1Score.ToString();

            button1.Enabled = true;
        }

        private void CloseApplication()
        {
            // Ngắt kết nối
            if (client != null) client.Close();
            if (listener != null) listener.Stop();

            // Đóng cửa sổ
            this.Invoke(new Action(() => this.Close()));
        }
        private void FormCT_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseApplication();
        }


    }
}
