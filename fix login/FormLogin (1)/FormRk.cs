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

namespace FormLogin
{
    public partial class FormRk : Form
    {
        private string connectionString = "Data Source=MINHTIENVICTUS1;Initial Catalog=VongQuayDB;Integrated Security=True";
        private string userName;
        public FormRk( string userName)
        {
            InitializeComponent();
            this.userName = userName;
        }

        private void FormRk_Load(object sender, EventArgs e)
        {
            LoadLeaderboard(userName);
        }
        private void LoadLeaderboard(string userName)
        {
            // Truy vấn để lấy 5 người đứng đầu
            string topPlayersQuery = @"SELECT TOP 5 UserName, FinalScore
                                        FROM Score
                                        ORDER BY FinalScore DESC;";

            // Truy vấn để tìm hạng của người chơi
            string userRankQuery = @"SELECT COUNT(*) + 1 AS Rank
                                      FROM Score
                                      WHERE FinalScore > (SELECT FinalScore FROM Score WHERE UserName = @UserName);";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand topPlayersCommand = new SqlCommand(topPlayersQuery, connection);
                SqlCommand userRankCommand = new SqlCommand(userRankQuery, connection);
                userRankCommand.Parameters.AddWithValue("@UserName", userName);

                try
                {
                    connection.Open();

                    // Lấy 5 người đứng đầu
                    SqlDataAdapter adapter = new SqlDataAdapter(topPlayersCommand);
                    DataTable leaderboard = new DataTable();
                    adapter.Fill(leaderboard);

                    // Gán tên và điểm vào các label
                    if (leaderboard.Rows.Count > 0)
                    {
                        lblRank1.Text = $"Hạng 1: {leaderboard.Rows[0]["UserName"]} - {leaderboard.Rows[0]["FinalScore"]}";
                        if (leaderboard.Rows.Count > 1)
                            lblRank2.Text = $"Hạng 2: {leaderboard.Rows[1]["UserName"]} - {leaderboard.Rows[1]["FinalScore"]}";
                        if (leaderboard.Rows.Count > 2)
                            lblRank3.Text = $"Hạng 3: {leaderboard.Rows[2]["UserName"]} - {leaderboard.Rows[2]["FinalScore"]}";
                        if (leaderboard.Rows.Count > 3)
                            lblRank4.Text = $"Hạng 4: {leaderboard.Rows[3]["UserName"]} - {leaderboard.Rows[3]["FinalScore"]}";
                        if (leaderboard.Rows.Count > 4)
                            lblRank5.Text = $"Hạng 5: {leaderboard.Rows[4]["UserName"]} - {leaderboard.Rows[4]["FinalScore"]}";
                    }

                    // Lấy hạng của người chơi
                    int myRank = (int)userRankCommand.ExecuteScalar();
                    lblmyRank.Text = $"Hạng của bạn là: {myRank}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }
        private void label19_Click(object sender, EventArgs e)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            
            this.Hide();
        }
    }
}
