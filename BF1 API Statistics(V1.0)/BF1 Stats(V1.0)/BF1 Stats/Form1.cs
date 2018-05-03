using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.IO;

namespace BF1_Stats
{
    //https://dotnetrix.co.uk/tabcontrol.htm#tip2
    //https://www.youtube.com/watch?v=6ua-IegyKB4
    //https://www.youtube.com/watch?v=vfWnb6zGXLI
    // DELETE THE PLATFORM??? IT SHOULD AUTO DELETE THO

    public partial class Form1 : Form
    {
        List<Resources.Weapons> listWeapons = new List<Resources.Weapons>();

        bool sideOpened = false;
        bool sideMoving = false;
        bool calculatedWeapons = false;
        int idType = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private async void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtPlayerName.Text) && txtPlayerName.Text != "Enter Player Name To Search")
            {
                //SOLVE ISSUES WITH PLAYER STATS NOT WORKING IF WEBSITE DOESNT WORK
                //FIX NO INTERNET ERROR
                btnSubmit.Enabled = false;
                panelSideIcon.Location = new System.Drawing.Point(panelSideIcon.Location.X, btnGeneralStats.Location.Y);
                panelLoading.Visible = true;
                loadingCircle.Active = true;
                panelSearch.Visible = false;
                panelLoading.Parent = tabPage1;


                Platform platform = new Platform(cbxPlatform.SelectedIndex + 1);
                string link = "Stats/detailedStats?platform=" + platform.GetPlatformID() + "&displayName=" + txtPlayerName.Text;
                //https://battlefieldtracker.com/bf1/profile/pc/Norey/matches
                var baseAddress = new Uri("https://battlefieldtracker.com/bf1/api/");
                try
                {
                    using (var httpClient = new HttpClient { BaseAddress = baseAddress })
                    {

                        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("trn-api-key", "c8dfbf65-8572-4733-adf8-ca6977cbaa6f");
                        using (var response = await httpClient.GetAsync(link))
                        {

                            string responseData = await response.Content.ReadAsStringAsync();
                            determineGeneralStats(ref responseData, platform.GetPlatformID());
                            //txtResults.Text = responseData;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ResetStats();
                }

                tabControlMain.SelectedIndex = 1;
                txtPlayerName.Text = "";
                txtPlayerName_CheckText();
                loadingCircle.Active = false;
                panelLoading.Parent = panelPlayerStats;
                panelLoading.Visible = false;
                panelSearch.Visible = true;
                btnSubmit.Enabled = true;
            }
        }

        

        public void determineGeneralStats(ref string playerStats, int platform) // Goes through the player stats. Sets relevant data to be displayed on GeneralStatsPanel
        {
            if (playerStats.Contains("\"successful\": true"))
            {
                int index = 0;
                int offset = 0;

                index = playerStats.IndexOf("\"flagsCaptured");
                offset = 17;
                int playerCaps = Convert.ToInt32(playerStats.Substring(index+offset, (playerStats.IndexOf(",",index) - (index+offset))));
                lblCaptures.Text = playerCaps.ToString();

                index = playerStats.IndexOf("\"flagsDefended");
                int playerDefends = Convert.ToInt32(playerStats.Substring(index+offset, (playerStats.IndexOf(",", index) - (index + offset))));
                lblDefends.Text = playerDefends.ToString();

                index = playerStats.IndexOf("\"kills");
                offset = 9;
                int playerKills = Convert.ToInt32(playerStats.Substring(index + offset, (playerStats.IndexOf(",", index) - (index + offset))));
                lblKills.Text = playerKills.ToString();

                index = playerStats.IndexOf("\"wins");
                offset = 8;
                int playerWins = Convert.ToInt32(playerStats.Substring(index + offset, (playerStats.IndexOf(",", index) - (index + offset))));
                lblWins.Text = playerWins.ToString();

                index = playerStats.IndexOf("\"losses");
                offset = 10;
                int playerLosses = Convert.ToInt32(playerStats.Substring(index + offset, (playerStats.IndexOf(",", index) - (index + offset))));
                lblLosses.Text = playerLosses.ToString();
                //int playerMVP; // would have to find via internet

                index = playerStats.IndexOf("\"deaths");
                offset = 10;
                int playerDeaths = Convert.ToInt32(playerStats.Substring(index + offset, (playerStats.IndexOf(",", index) - (index + offset))));
                lblDeaths.Text = playerDeaths.ToString();

                double playerKD = Math.Round((double)playerKills/(double)playerDeaths, 2);
                lblKD.Text = playerKD.ToString();

                double playerWL = Math.Round(((double)playerWins / ((double)(playerWins + playerLosses)) * 100),2);
                lblWL.Text = playerWL.ToString()+"%";

                index = playerStats.IndexOf("\"spm");
                offset = 7;
                double playerSPM = Math.Round(Convert.ToDouble(playerStats.Substring(index + offset, (playerStats.IndexOf(",", index) - (index + offset)))), 2);
                lblSPM.Text = playerSPM.ToString();

                index = playerStats.IndexOf("\"kpm");
                offset = 7;
                double playerKPM = Math.Round(Convert.ToDouble(playerStats.Substring(index + offset, (playerStats.IndexOf(",", index) - (index + offset)))), 2);
                lblKPM.Text = playerKPM.ToString();

                index = playerStats.IndexOf("\"skill");
                offset = 9;
                double playerSkill = Convert.ToDouble(playerStats.Substring(index + offset, (playerStats.IndexOf(",", index) - (index + offset))));
                lblSkill.Text = playerSkill.ToString();

                //index = playerStats.IndexOf("\"accuracyRatio");
                //offset = 17;
                //double playerAccuracy = (Math.Round(Convert.ToDouble(playerStats.Substring(index + offset, (playerStats.IndexOf(",", index) - (index + offset)))), 2)*100);

                index = playerStats.IndexOf("\"favoriteClass");
                offset = 18;
                string playerClass = playerStats.Substring(index + offset, ((playerStats.IndexOf(",", index)-1) - (index + offset)));
                lblBestClass.Text = playerClass.ToUpper();
                lblBestClass.Location = new System.Drawing.Point((lblBestClassSTATIC.Location.X + (lblBestClassSTATIC.Width/2)) - (lblBestClass.Width / 2), lblBestClass.Location.Y);
                switch(playerClass)
                {
                    case "Scout":
                        pbxClass.Image = Properties.Resources.Scout;
                        pbxClass.Refresh();
                        break;
                    case "Assault":
                        pbxClass.Image = Properties.Resources.Assault;
                        pbxClass.Refresh();
                        break;
                    case "Medic":
                        pbxClass.Image = Properties.Resources.Medic;
                        pbxClass.Refresh();
                        break;
                    case "Support":
                        pbxClass.Image = Properties.Resources.Support;
                        pbxClass.Refresh();
                        break;
                    default:
                        pbxClass.Image = Properties.Resources.Assault;
                        pbxClass.Refresh();
                        break;
                }

                index = playerStats.IndexOf("\"displayName");
                offset = 16;
                string playerName = playerStats.Substring(index + offset, ((playerStats.IndexOf(",", index)-1) - (index + offset)));
                lblNameSTATIC.Text = playerName;
                lblNameSTATIC.Location = new System.Drawing.Point((tabControlMain.Width/2)-(lblNameSTATIC.Width/2), 40-(lblNameSTATIC.Height/2));

                index = playerStats.IndexOf("\"gameModeStats");
                string subString = playerStats.Substring(index, (playerStats.IndexOf("\"headShots")-index));
                //txtResults.Text = subString;

                index = subString.IndexOf("\"WAR PIGEONS");
                offset = 10;
                int warPigeonsLosses = Convert.ToInt32(subString.Substring(subString.IndexOf("\"losses", index) + offset, (subString.IndexOf(",", subString.IndexOf("\"losses", index)) - (subString.IndexOf("\"losses", index) + offset))));
                index = subString.IndexOf("\"CONQUEST");
                int conquestLosses = Convert.ToInt32(subString.Substring(subString.IndexOf("\"losses", index) + offset, (subString.IndexOf(",", subString.IndexOf("\"losses", index)) - (subString.IndexOf("\"losses", index) + offset))));
                index = subString.IndexOf("\"TEAM DEATHMATCH");
                int tdmLosses = Convert.ToInt32(subString.Substring(subString.IndexOf("\"losses", index) + offset, (subString.IndexOf(",", subString.IndexOf("\"losses", index)) - (subString.IndexOf("\"losses", index) + offset))));
                index = subString.IndexOf("\"DOMINATION");
                int dominationLosses = Convert.ToInt32(subString.Substring(subString.IndexOf("\"losses", index) + offset, (subString.IndexOf(",", subString.IndexOf("\"losses", index)) - (subString.IndexOf("\"losses", index) + offset))));
                index = subString.IndexOf("\"RUSH");
                int rushLosses = Convert.ToInt32(subString.Substring(subString.IndexOf("\"losses", index) + offset, (subString.IndexOf(",", subString.IndexOf("\"losses", index)) - (subString.IndexOf("\"losses", index) + offset))));
                index = subString.IndexOf("\"OPERATIONS SMALL");
                int operationsLosses = Convert.ToInt32(subString.Substring(subString.IndexOf("\"losses", index) + offset, (subString.IndexOf(",", subString.IndexOf("\"losses", index)) - (subString.IndexOf("\"losses", index) + offset))));

                index = subString.IndexOf("\"WAR PIGEONS");
                offset = 8;
                int warPigeonsWins = Convert.ToInt32(subString.Substring(subString.IndexOf("\"wins", index) + offset, (subString.IndexOf(",", subString.IndexOf("\"wins", index)) - (subString.IndexOf("\"wins", index) + offset))));
                index = subString.IndexOf("\"CONQUEST");
                int conquestWins = Convert.ToInt32(subString.Substring(subString.IndexOf("\"wins", index) + offset, (subString.IndexOf(",", subString.IndexOf("\"wins", index)) - (subString.IndexOf("\"wins", index) + offset))));
                index = subString.IndexOf("\"TEAM DEATHMATCH");
                int tdmWins = Convert.ToInt32(subString.Substring(subString.IndexOf("\"wins", index) + offset, (subString.IndexOf(",", subString.IndexOf("\"wins", index)) - (subString.IndexOf("\"wins", index) + offset))));
                index = subString.IndexOf("\"DOMINATION");
                int dominationWins = Convert.ToInt32(subString.Substring(subString.IndexOf("\"wins", index) + offset, (subString.IndexOf(",", subString.IndexOf("\"wins", index)) - (subString.IndexOf("\"wins", index) + offset))));
                index = subString.IndexOf("\"RUSH");
                int rushWins = Convert.ToInt32(subString.Substring(subString.IndexOf("\"wins", index) + offset, (subString.IndexOf(",", subString.IndexOf("\"wins", index)) - (subString.IndexOf("\"wins", index) + offset))));
                index = subString.IndexOf("\"OPERATIONS SMALL");
                int operationsWins = Convert.ToInt32(subString.Substring(subString.IndexOf("\"wins", index) + offset, (subString.IndexOf(",", subString.IndexOf("\"wins", index)) - (subString.IndexOf("\"wins", index) + offset))));

                labelGMConqWins.Text = conquestWins.ToString();
                labelGMConqLosses.Text = conquestLosses.ToString();
                labelGMConqWL.Text = Math.Round((((float)conquestWins / ((float)conquestWins + (float)conquestLosses))*100),2).ToString()+"%";
                labelGMConqPlayed.Text = (conquestWins + conquestLosses).ToString();

                labelGMOpsWins.Text = operationsWins.ToString();
                labelGMOpsLosses.Text = operationsLosses.ToString();
                labelGMOpsWL.Text = Math.Round((((float)operationsWins / ((float)operationsWins + (float)operationsLosses)) * 100),2).ToString() + "%";
                labelGMOpsPlayed.Text = (operationsWins + operationsLosses).ToString();

                labelGMTDMWins.Text = tdmWins.ToString();
                labelGMTDMLosses.Text = tdmLosses.ToString();
                labelGMTDMWL.Text = Math.Round((((float)tdmWins / ((float)tdmWins + (float)tdmLosses)) * 100),2).ToString() + "%";
                labelGMTDMPlayed.Text = (tdmWins + tdmLosses).ToString();

                labelGMDomWins.Text = dominationWins.ToString();
                labelGMDomLosses.Text = dominationLosses.ToString();
                labelGMDomWL.Text = Math.Round((((float)dominationWins / ((float)dominationWins + (float)dominationLosses)) * 100),2).ToString() + "%";
                labelGMDomPlayed.Text = (dominationWins + dominationLosses).ToString();

                labelGMRushWins.Text = rushWins.ToString();
                labelGMRushLosses.Text = rushLosses.ToString();
                labelGMRushWL.Text = Math.Round((((float)rushWins / ((float)rushWins + (float)rushLosses)) * 100),2).ToString() + "%";
                labelGMRushPlayed.Text = (rushWins + rushLosses).ToString();

                labelGMWPWins.Text = warPigeonsWins.ToString();
                labelGMWPLosses.Text = warPigeonsLosses.ToString();
                labelGMWPWL.Text = Math.Round((((float)warPigeonsWins / ((float)warPigeonsWins + (float)warPigeonsLosses)) * 100),2).ToString() + "%";
                labelGMWPPlayed.Text = (warPigeonsWins + warPigeonsLosses).ToString();

                DrawPieChart(warPigeonsLosses+warPigeonsWins, conquestLosses+conquestWins, tdmLosses+tdmWins, dominationLosses+dominationWins, rushLosses+rushWins, operationsLosses+operationsWins);
                DrawBarGraph(warPigeonsWins, warPigeonsLosses, conquestWins, conquestLosses, tdmWins, tdmLosses, dominationWins, dominationLosses, rushWins, rushLosses, operationsWins, operationsLosses);

                try
                {
                    // Check if the player file already exists. If yes, check the retrieved stats against previous stats. If no, create the file and populate data.
                    if (File.Exists(playerName + platform + ".txt"))
                    {
                        using (StreamReader sr = File.OpenText(playerName + platform + ".txt"))
                        {
                            double val;
                            if (Double.TryParse(File.ReadLines(playerName + platform + ".txt").Skip(0).Take(1).First(), out val))
                            {
                                if (val<playerKD)
                                {
                                    pbxKD.Visible = true;
                                    pbxKD.Image = Properties.Resources.GreenArrow;
                                    pbxKD.Refresh();
                                }
                                else if(val == playerKD)
                                {
                                    pbxKD.Visible = false;
                                }
                                else
                                {
                                    pbxKD.Visible = true;
                                    pbxKD.Image = Properties.Resources.RedArrow;
                                    pbxKD.Refresh();
                                }
                            }
                            if (Double.TryParse(File.ReadLines(playerName + platform + ".txt").Skip(1).Take(1).First(), out val))
                            {
                                if (val < playerWL)
                                {
                                    pbxWL.Visible = true;
                                    pbxWL.Image = Properties.Resources.GreenArrow;
                                    pbxWL.Refresh();
                                }
                                else if (val == playerWL)
                                {
                                    pbxWL.Visible = false;
                                }
                                else
                                {
                                    pbxWL.Visible = true;
                                    pbxWL.Image = Properties.Resources.RedArrow;
                                    pbxWL.Refresh();
                                }
                            }
                            if (Double.TryParse(File.ReadLines(playerName + platform + ".txt").Skip(2).Take(1).First(), out val))
                            {
                                if (val < playerSPM)
                                {
                                    pbxSPM.Visible = true;
                                    pbxSPM.Image = Properties.Resources.GreenArrow;
                                    pbxSPM.Refresh();
                                }
                                else if (val == playerSPM)
                                {
                                    pbxSPM.Visible = false;
                                }
                                else
                                {
                                    pbxSPM.Visible = true;
                                    pbxSPM.Image = Properties.Resources.RedArrow;
                                    pbxSPM.Refresh();
                                }
                            }
                            if (Double.TryParse(File.ReadLines(playerName + platform + ".txt").Skip(3).Take(1).First(), out val))
                            {
                                if (val < playerKPM)
                                {
                                    pbxKPM.Visible = true;
                                    pbxKPM.Image = Properties.Resources.GreenArrow;
                                    pbxKPM.Refresh();
                                }
                                else if (val == playerKPM)
                                {
                                    pbxKPM.Visible = false;
                                }
                                else
                                {
                                    pbxKPM.Visible = true;
                                    pbxKPM.Image = Properties.Resources.RedArrow;
                                    pbxKPM.Refresh();
                                }
                            }
                            if (Double.TryParse(File.ReadLines(playerName + platform + ".txt").Skip(4).Take(1).First(), out val))
                            {
                                if (val < playerSkill)
                                {
                                    pbxSkill.Visible = true; 
                                    pbxSkill.Image = Properties.Resources.GreenArrow;
                                    pbxSkill.Refresh();
                                }
                                else if (val == playerSkill)
                                {
                                    pbxSkill.Visible = false;
                                }
                                else
                                {
                                    pbxSkill.Visible = true;
                                    pbxSkill.Image = Properties.Resources.RedArrow;
                                    pbxSkill.Refresh();
                                }
                            }
                        }
                        using (StreamWriter sw = File.CreateText(playerName + platform + ".txt"))
                        {
                            sw.WriteLine(playerKD);
                            sw.WriteLine(playerWL);
                            sw.WriteLine(playerSPM);
                            sw.WriteLine(playerKPM);
                            sw.WriteLine(playerSkill);
                            sw.Close();
                        }
                    }
                    else
                    {
                        //create text file and write stats
                        using (StreamWriter sw = File.CreateText(playerName + platform + ".txt"))
                        {
                            sw.WriteLine(playerKD);
                            sw.WriteLine(playerWL);
                            sw.WriteLine(playerSPM);
                            sw.WriteLine(playerKPM);
                            sw.WriteLine(playerSkill);
                            sw.Close();
                        }
                    }
                }
                catch (Exception Ex)
                {
                    Console.WriteLine(Ex.ToString());
                }
            }
            else
            {
                ResetStats();
            }

        }

        private void DrawPieChart(int warPigeonsPlayed, int conquestPlayed, int tdmPlayed, int dominationPlayed, int rushPlayed, int operationsPlayed)
        {
            //reset your chart series and legends
            chartModesPlayed.Series.Clear();
            chartModesPlayed.Legends.Clear();

            //Add a new Legend(if needed) and do some formating
            chartModesPlayed.Legends.Add("Legend");
            chartModesPlayed.Legends[0].LegendStyle = System.Windows.Forms.DataVisualization.Charting.LegendStyle.Table;
            chartModesPlayed.Legends[0].Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
            chartModesPlayed.Legends[0].Alignment = StringAlignment.Center;
            chartModesPlayed.Legends[0].Title = "Game Modes";
            chartModesPlayed.Legends[0].BorderColor = Color.Black;
            chartModesPlayed.Legends[0].BackColor = Color.DimGray;
            chartModesPlayed.Legends[0].ForeColor = Color.White;

            //Add a new chart-series
            string seriesname = "MySeriesName";
            chartModesPlayed.Series.Add(seriesname);
            //set the chart-type to "Pie"
            chartModesPlayed.Series[seriesname].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;
            chartModesPlayed.Series[seriesname]["PieLabelStyle"] = "Outside";

            // Set border width so that labels are shown on the outside
            chartModesPlayed.Series[seriesname].BorderWidth = 1;
            chartModesPlayed.Series[seriesname].BorderColor = System.Drawing.Color.FromArgb(26, 59, 105);
            chartModesPlayed.Series[seriesname].LabelForeColor = System.Drawing.Color.White;

            //Add some datapoints so the series. in this case you can pass the values to this method
            chartModesPlayed.Series[seriesname].Points.AddXY("War Pigeons", warPigeonsPlayed);
            chartModesPlayed.Series[seriesname].Points.AddXY("Conquest", conquestPlayed);
            chartModesPlayed.Series[seriesname].Points.AddXY("TDM", tdmPlayed);
            chartModesPlayed.Series[seriesname].Points.AddXY("Domination", dominationPlayed);
            chartModesPlayed.Series[seriesname].Points.AddXY("Rush", rushPlayed);
            chartModesPlayed.Series[seriesname].Points.AddXY("Operations", operationsPlayed);
        }

        private void DrawBarGraph(int warPigeonsWin, int warPigeonsLoss,int conquestWin,int conquestLoss, int tdmWin, int tdmLoss, int dominationWin, int dominationLoss, int rushWin, int rushLoss, int operationsWin, int operationsLoss)
        {
            //reset your chart series and legends
            chartModesWL.Series.Clear();
            chartModesWL.Legends.Clear();
            
            //Add a new Legend(if needed) and do some formating
            chartModesWL.Legends.Add("Legend");
            chartModesWL.Legends[0].LegendStyle = System.Windows.Forms.DataVisualization.Charting.LegendStyle.Table;
            chartModesWL.Legends[0].Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Right;
            chartModesWL.Legends[0].Alignment = StringAlignment.Near;
            chartModesWL.Legends[0].Title = "";
            chartModesWL.Legends[0].BorderColor = Color.Transparent;
            chartModesWL.Legends[0].BackColor = Color.Transparent;
            chartModesWL.Legends[0].ForeColor = Color.White;

            //Add a new chart-series
            string wins = "Wins";
            string losses = "Losses";
            chartModesWL.Series.Add(wins);
            chartModesWL.Series.Add(losses);
            //set the chart-type to "Column"
            chartModesWL.Series[wins].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
            chartModesWL.Series[losses].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
            chartModesWL.BackColor = System.Drawing.Color.Transparent;

            ////Add some datapoints so the series. in this case you can pass the values to this method
            chartModesWL.Series[wins].IsValueShownAsLabel = true;
            chartModesWL.Series[wins].LabelForeColor = Color.White;
            chartModesWL.Series[losses].IsValueShownAsLabel = true;
            chartModesWL.Series[losses].LabelForeColor = Color.White;
            chartModesWL.ChartAreas[0].BackColor = Color.Transparent;
            chartModesWL.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chartModesWL.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            chartModesWL.ChartAreas[0].AxisY.Maximum = 100;
            chartModesWL.ChartAreas[0].AxisY.Minimum = 0;
            chartModesWL.ChartAreas[0].AxisY2.Maximum = 100;
            chartModesWL.ChartAreas[0].AxisY2.Minimum = 0;
            chartModesWL.ChartAreas[0].AxisX.LabelStyle.ForeColor = System.Drawing.Color.White;
            chartModesWL.ChartAreas[0].AxisY.LabelStyle.ForeColor = System.Drawing.Color.White;

            chartModesWL.Series[wins].Points.AddXY("Conquest", Math.Round((((float)conquestWin/((float)conquestWin+(float)conquestLoss))*100),2));
            chartModesWL.Series[losses].Points.AddXY("", Math.Round((((float)conquestLoss / ((float)conquestWin + (float)conquestLoss))*100),2));
            chartModesWL.Series[wins].Points.AddXY("Operations", Math.Round((((float)operationsWin / ((float)operationsWin + (float)operationsLoss)) * 100), 2));
            chartModesWL.Series[losses].Points.AddXY("", Math.Round((((float)operationsLoss / ((float)operationsWin + (float)operationsLoss)) * 100), 2));
            chartModesWL.Series[wins].Points.AddXY("Domination", Math.Round((((float)dominationWin / ((float)dominationWin + (float)dominationLoss)) * 100), 2));
            chartModesWL.Series[losses].Points.AddXY("", Math.Round((((float)dominationLoss / ((float)dominationWin + (float)dominationLoss)) * 100), 2));
            chartModesWL.Series[wins].Points.AddXY("Team Deathmatch", Math.Round((((float)tdmWin / ((float)tdmWin + (float)tdmLoss)) * 100), 2));
            chartModesWL.Series[losses].Points.AddXY("", Math.Round((((float)tdmLoss / ((float)tdmWin + (float)tdmLoss)) * 100), 2));
            chartModesWL.Series[wins].Points.AddXY("Rush", Math.Round((((float)rushWin / ((float)rushWin + (float)rushLoss)) * 100), 2));
            chartModesWL.Series[losses].Points.AddXY("", Math.Round((((float)rushLoss / ((float)rushWin + (float)rushLoss)) * 100), 2));
            chartModesWL.Series[wins].Points.AddXY("War Pigeons", Math.Round((((float)warPigeonsWin / ((float)warPigeonsWin + (float)warPigeonsLoss)) * 100), 2));
            chartModesWL.Series[losses].Points.AddXY("", Math.Round((((float)warPigeonsLoss / ((float)warPigeonsWin + (float)warPigeonsLoss)) * 100), 2));
        }
       
        private void Form1_Load(object sender, EventArgs e) //On load of the program, add all weapons to the listWeapons
        {
            cbxPlatform.SelectedIndex = 2;
            cbxIDMatches.SelectedIndex = 0;
            cbxIDStat.SelectedIndex = 0;
            cbxIDWeapon.SelectedIndex = 0;
            //listWeapons.Add(weapons1);
            //listWeapons.Add(weapons2);
            //listWeapons.Add(weapons3);
        }

        private void txtPlayerName_Leave(object sender, EventArgs e) // Used when the user clicks off the search text
        {
            txtPlayerName_CheckText();
        }

        private void txtPlayerName_Enter(object sender, EventArgs e) // Upon clicking the search text, if the default message is there, clear the text to allow user input
        {
            if (txtPlayerName.Text == "Enter Player Name To Search")
            {
                txtPlayerName.Text = "";
                txtPlayerName.ForeColor = Color.Black;
            }
        }

        private void txtPlayerName_CheckText() // This function check to see if the text entered in the search bar was left empty or full of white space characters. Sets text to the default message
        {
            if (string.IsNullOrWhiteSpace(txtPlayerName.Text))
            {
                txtPlayerName.ForeColor = Color.DimGray;
                txtPlayerName.Text = "Enter Player Name To Search";
            }
        }

        private void btnBackToSearch_Click(object sender, EventArgs e)
        {
            tabControlMain.SelectedIndex = 0;
            pbxKD.Visible = false;
            pbxKPM.Visible = false;
            pbxSkill.Visible = false;
            pbxSPM.Visible = false;
            pbxWL.Visible = false;
            btnSide.Location = new System.Drawing.Point(0, -10);
            panelSide.Location = new System.Drawing.Point(-197, -10);
            sideMoving = false;
            sideOpened = false;
            panelGeneralStats.Visible = true;
            panelInDepth.Visible = false;
            panelWeapons.Visible = false;
            calculatedWeapons = false;
            for (int i = listWeapons.Count - 1; i >= 0; i--)
            {
                panelWeaponsBody.Controls.Remove(listWeapons[i]);
            }
            listWeapons.Clear();
        }

        private void btnSide_Click(object sender, EventArgs e)
        {
            tmrSideMover.Start();
            sideOpened =! sideOpened;
            sideMoving = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (sideOpened)
            {
                if (sideMoving == true)
                {
                    btnSide.Location = new System.Drawing.Point(btnSide.Location.X + 40, -10);
                    panelSide.Location = new System.Drawing.Point(panelSide.Location.X + 40, -10);
                    if (btnSide.Location.X >= 150)
                    {
                        sideMoving = false;
                        sideOpened = true;
                        tmrSideMover.Stop();
                    }
                }
            }
            else
            {
                if (sideMoving == true)
                {
                    btnSide.Location = new System.Drawing.Point(btnSide.Location.X - 40, -10);
                    panelSide.Location = new System.Drawing.Point(panelSide.Location.X - 40, -10);
                    if (btnSide.Location.X <= 0)
                    {
                        btnSide.Location = new System.Drawing.Point(0, -10);
                        sideMoving = false;
                        sideOpened = false;
                        tmrSideMover.Stop();
                    }
                }
            }
        }

        private void btnGeneralStats_Click(object sender, EventArgs e)
        {
            tmrSideMover.Start();
            sideOpened = !sideOpened;
            sideMoving = true;
            panelSideIcon.Location = new System.Drawing.Point(panelSideIcon.Location.X, btnGeneralStats.Location.Y);
            panelGeneralStats.Visible = true;
            panelInDepth.Visible = false;
            panelWeapons.Visible = false;
        }

        private async void btnInDepth_Click(object sender, EventArgs e)
        {
            tmrSideMover.Start();
            sideOpened = !sideOpened;
            sideMoving = true;
            panelSideIcon.Location = new System.Drawing.Point(panelSideIcon.Location.X, btnInDepth.Location.Y);
            panelGeneralStats.Visible = false;
            panelInDepth.Visible = true;
            cbxIDMatches.SelectedIndex = 0;
            btnIDWL.PerformClick();
            
        }

        private async void btnWeapons_Click(object sender, EventArgs e)
        {
            tmrSideMover.Start();
            sideOpened = !sideOpened;
            sideMoving = true;
            panelSideIcon.Location = new System.Drawing.Point(panelSideIcon.Location.X, btnWeapons.Location.Y);
            panelGeneralStats.Visible = false;
            panelInDepth.Visible = false;
            panelWeapons.Visible = false;
            panelWeaponSortSlider.Location = new System.Drawing.Point(btnWeaponName.Location.X, panelWeaponSortSlider.Location.Y);
            if (calculatedWeapons != true)
            {
                loadingCircle.Active = true;
                panelLoading.Visible = true;
                Platform platform = new Platform(cbxPlatform.SelectedIndex + 1);
                string link = "Progression/GetWeapons?platform=" + platform.GetPlatformID() + "&displayName=" + lblNameSTATIC.Text;
                var baseAddress = new Uri("https://battlefieldtracker.com/bf1/api/");
                try
                {
                    using (var httpClient = new HttpClient { BaseAddress = baseAddress })
                    {

                        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("trn-api-key", "c8dfbf65-8572-4733-adf8-ca6977cbaa6f");
                        using (var response = await httpClient.GetAsync(link))
                        {

                            string responseData = await response.Content.ReadAsStringAsync();
                            displayWeapons(ref responseData);
                        }
                    }
                    calculatedWeapons = true;
                }
                catch (Exception ex)
                {
                    for (int i = listWeapons.Count - 1; i >= 0; i--)
                    {
                        panelWeaponsBody.Controls.Remove(listWeapons[i]);
                    }
                    listWeapons.Clear();
                }
            }
            if (panelSideIcon.Location.Y == btnWeapons.Location.Y)
            {
                panelWeapons.Visible = true;
                loadingCircle.Active = false;
                panelLoading.Visible = false;
                btnWeaponKills.PerformClick();
            }
        }

        private void displayWeapons(ref string responseData)
        {
            if (responseData.Contains("\"successful\": true"))
            {
                MakeWeaponList(ref responseData);
            }
            else
            {
                panelWeaponsBody.Size = new Size(panelWeaponsBody.Size.Width, 566);
            }
        }

        public class SemiNumericComparer : IComparer<string> // code provided by https://stackoverflow.com/questions/6396378/c-sharp-linq-orderby-numbers-that-are-string-and-you-cannot-convert-them-to-int with slight modification
        {
            public int Compare(string s1, string s2)
            {
                if (IsNumeric(s1) && IsNumeric(s2))
                {
                    if (Convert.ToInt32(s1) > Convert.ToInt32(s2)) return 1;
                    if (Convert.ToInt32(s1) < Convert.ToInt32(s2)) return -1;
                    if (Convert.ToInt32(s1) == Convert.ToInt32(s2)) return 0;
                }

                if (IsNumeric(s1) && !IsNumeric(s2))
                    return 1;

                if (!IsNumeric(s1) && IsNumeric(s2))
                    return -1;

                return string.Compare(s1, s2, true);
            }

            public static bool IsNumeric(object value)
            {
                try
                {
                    int i = Convert.ToInt32(value.ToString());
                    return true;
                }
                catch (FormatException)
                {
                    return false;
                }
            }
        }

        private void UpdateWeaponList()
        {
            int i = 0;
            int space = 110;

            foreach (Resources.Weapons weapon in listWeapons)
            {
                    weapon.Location = new System.Drawing.Point(82, (165 + (i * space)));
                    i++;
            }
        }

        private void MakeWeaponList(ref string responseData)
        { 
            //split retrieved weapon stats string up into multiple lines for easier searching
            string[] lines = responseData.Split(
             new[] { "\r\n", "\r", "\n" },
            StringSplitOptions.None
            );

            int counter = 0;
            string kills="NA";
            string accuracy="NA";
            string seconds="NA";
            string name="NA";
            int switchcase = 0;
            foreach (string element in lines) //First looks for the number of kills, then accuracy, then time, and finally gets the name. This method ensures you do not get the garbage values of the weapon type. (EX: "name": "FIELD KIT")
            {
                switch (switchcase)
                {
                    case 0: //kills
                        if (element.Contains("\"kills\""))
                        {
                            kills = element.Substring((element.IndexOf("\"") + 9), ((element.LastIndexOf(",") - 2) - (element.IndexOf("\"") + 9)));
                            switchcase++;
                        }
                        break;
                    case 1: //accuracy
                        if (element.Contains("\"accuracy\""))
                        {
                            accuracy = element.Substring((element.IndexOf("\"") + 12), (element.LastIndexOf(",") - (element.IndexOf("\"") + 12)));
                            switchcase++;
                        }
                        break;
                    case 2: //seconds
                        if (element.Contains("\"seconds\""))
                        {
                            seconds = element.Substring((element.IndexOf("\"") + 11), (element.LastIndexOf(",") - (element.IndexOf("\"") + 13)));
                            switchcase++;
                        }
                        break;
                    case 3: //name
                        if (element.Contains("\"name\""))
                        {
                            name = (element.Substring((element.IndexOf("\"") + 9), (element.LastIndexOf("\"") - (element.IndexOf("\"") + 9))));
                            switchcase = 0;
                            listWeapons.Add(new Resources.Weapons());
                            listWeapons[counter].Location = new System.Drawing.Point(82, 130);
                            listWeapons[counter].Size = new System.Drawing.Size(514, 110);
                            listWeapons[counter].WeaponAccuracy = accuracy;
                            listWeapons[counter].WeaponKills = kills;
                            this.Controls.Add(listWeapons[counter]);
                            listWeapons[counter].Parent = panelWeaponsBody;
                            listWeapons[counter].WeaponKPM = Math.Round((Convert.ToDouble(kills) / (Convert.ToDouble(seconds) / 60)), 2).ToString();
                            listWeapons[counter].WeaponName = name;
                            counter++;
                        }
                        break;
                }
            }
                
            panelWeaponsBody.Size = new Size(panelWeaponsBody.Size.Width,(listWeapons.Count*110)+200);
            // OLD METHOD. READS FROM FILE AND THEN OUTPUTS FILE. ONLY DOES NAME. DOES NOT ENSURE SAFE REMOVAL OF GARBAGE NAMES FROM WEAPON TYPE
            //using (StreamWriter sw = File.CreateText("Weapons.txt"))
            //{
            //    sw.WriteLine(txtResults.Text);
            //    sw.Close();
            //}
            //int counter = 0;
            //using (StreamWriter sw = File.CreateText("Weapons2.txt"))
            //{
            //    string line;
            //    System.IO.StreamReader file =
            //    new System.IO.StreamReader("Weapons.txt");
            //    while ((line = file.ReadLine()) != null)
            //    {
            //        if (line.Contains("\"name\""))
            //        {
            //            sw.WriteLine(line.Substring((line.IndexOf("\"")+ 9), (line.LastIndexOf("\"")- (line.IndexOf("\"") + 9))));
            //            counter++;
            //        }
            //    }
            //    sw.WriteLine(counter.ToString());
            //
            //    file.Close();
            //    sw.Close();
            //}
        }

        private void btnWeaponName_Click(object sender, EventArgs e)
        {
            int switchCase = 0;
            if (panelWeaponSortSlider.Location.X != btnWeaponName.Location.X)
            {
                switchCase = 0;
            }
            else
            {
                if (panelWeaponSortSlider.BackColor == Color.Black)
                {
                    switchCase = 1;
                }
                else
                {
                    switchCase = 2;
                }
            }
            switch(switchCase)
            {
                case 0:
                    panelWeaponSortSlider.BackColor = Color.Black;
                    panelWeaponSortSlider.Location = new System.Drawing.Point(btnWeaponName.Location.X, panelWeaponSortSlider.Location.Y);
                    listWeapons = listWeapons.OrderByDescending(w => (w.WeaponName)).ToList();
                    UpdateWeaponList();
                    break;
                case 1:
                    panelWeaponSortSlider.BackColor = Color.White;
                    listWeapons = listWeapons.OrderBy(w => (w.WeaponName)).ToList();
                    UpdateWeaponList();
                    break;
                case 2:
                    panelWeaponSortSlider.BackColor = Color.Black;
                    listWeapons = listWeapons.OrderByDescending(w => (w.WeaponName)).ToList();
                    UpdateWeaponList();
                    break;
            }
        }

        private void btnWeaponKills_Click(object sender, EventArgs e)
        {
            int switchCase = 0;
            if (panelWeaponSortSlider.Location.X != btnWeaponKills.Location.X)
            {
                switchCase = 0;
            }
            else
            {
                if (panelWeaponSortSlider.BackColor == Color.Black)
                {
                    switchCase = 1;
                }
                else
                {
                    switchCase = 2;
                }
            }
            switch (switchCase)
            {

                case 0:
                    panelWeaponSortSlider.BackColor = Color.Black;
                    panelWeaponSortSlider.Location = new System.Drawing.Point(btnWeaponKills.Location.X, panelWeaponSortSlider.Location.Y);
                    listWeapons = listWeapons.OrderByDescending(w => w.WeaponKills, new SemiNumericComparer()).ToList();
                    UpdateWeaponList();
                    break;
                case 1:
                    panelWeaponSortSlider.BackColor = Color.White;
                    listWeapons = listWeapons.OrderBy(w => w.WeaponKills, new SemiNumericComparer()).ToList();
                    UpdateWeaponList();
                    break;
                case 2:
                    panelWeaponSortSlider.BackColor = Color.Black;
                    listWeapons = listWeapons.OrderByDescending(w => w.WeaponKills, new SemiNumericComparer()).ToList();
                    UpdateWeaponList();
                    break;
            }
        }

        private void btnWeaponKPM_Click(object sender, EventArgs e)
        {
            int switchCase = 0;
            if (panelWeaponSortSlider.Location.X != btnWeaponKPM.Location.X)
            {
                switchCase = 0;
            }
            else
            {
                if (panelWeaponSortSlider.BackColor == Color.Black)
                {
                    switchCase = 1;
                }
                else
                {
                    switchCase = 2;
                }
            }
            switch (switchCase)
            {
                case 0:
                    panelWeaponSortSlider.BackColor = Color.Black;
                    panelWeaponSortSlider.Location = new System.Drawing.Point(btnWeaponKPM.Location.X, panelWeaponSortSlider.Location.Y);
                    listWeapons = listWeapons.OrderByDescending(w => w.WeaponKPMDouble).ToList();
                    UpdateWeaponList();
                    break;
                case 1:
                    panelWeaponSortSlider.BackColor = Color.White;
                    listWeapons = listWeapons.OrderBy(w => w.WeaponKPMDouble).ToList();
                    UpdateWeaponList();
                    break;
                case 2:
                    panelWeaponSortSlider.BackColor = Color.Black;
                    listWeapons = listWeapons.OrderByDescending(w => w.WeaponKPMDouble).ToList();
                    UpdateWeaponList();
                    break;
            }
        }

        private void btnWeaponAccuracy_Click(object sender, EventArgs e)
        {
            int switchCase = 0;
            if (panelWeaponSortSlider.Location.X != btnWeaponAccuracy.Location.X)
            {
                switchCase = 0;
            }
            else
            {
                if (panelWeaponSortSlider.BackColor == Color.Black)
                {
                    switchCase = 1;
                }
                else
                {
                    switchCase = 2;
                }
            }
            switch (switchCase)
            {
                case 0:
                    panelWeaponSortSlider.BackColor = Color.Black;
                    panelWeaponSortSlider.Location = new System.Drawing.Point(btnWeaponAccuracy.Location.X, panelWeaponSortSlider.Location.Y);
                    listWeapons = listWeapons.OrderByDescending(w => w.WeaponAccuracyDouble).ToList();
                    UpdateWeaponList();
                    break;
                case 1:
                    panelWeaponSortSlider.BackColor = Color.White;
                    listWeapons = listWeapons.OrderBy(w => w.WeaponAccuracyDouble).ToList();
                    UpdateWeaponList();
                    break;
                case 2:
                    panelWeaponSortSlider.BackColor = Color.Black;
                    listWeapons = listWeapons.OrderByDescending(w => w.WeaponAccuracyDouble).ToList();
                    UpdateWeaponList();
                    break;
            }
        }

        public void ResetStats()
        {
            lblCaptures.Text = "N/A";
            lblDefends.Text = "N/A";
            lblKills.Text = "N/A";
            lblWins.Text = "N/A";
            lblLosses.Text = "N/A";
            lblDeaths.Text = "N/A";
            lblKD.Text = "N/A";
            lblWL.Text = "N/A";
            lblSPM.Text = "N/A";
            lblKPM.Text = "N/A";
            lblSkill.Text = "N/A";
            lblBestClass.Text = "N/A";
            lblBestClass.Location = new System.Drawing.Point((lblBestClassSTATIC.Location.X + (lblBestClassSTATIC.Width / 2)) - (lblBestClass.Width / 2), lblBestClass.Location.Y);

            pbxClass.Image = Properties.Resources.Assault;
            pbxClass.Refresh();

            lblNameSTATIC.Text = "Player Not Found";
            lblNameSTATIC.Location = new System.Drawing.Point((tabControlMain.Width / 2) - (lblNameSTATIC.Width / 2), 40 - (lblNameSTATIC.Height / 2));

            labelGMConqWins.Text = "N/A";
            labelGMConqLosses.Text = "N/A";
            labelGMConqWL.Text = "N/A";
            labelGMConqPlayed.Text = "N/A";

            labelGMOpsWins.Text = "N/A";
            labelGMOpsLosses.Text = "N/A";
            labelGMOpsWL.Text = "N/A";
            labelGMOpsPlayed.Text = "N/A";

            labelGMTDMWins.Text = "N/A";
            labelGMTDMLosses.Text = "N/A";
            labelGMTDMWL.Text = "N/A";
            labelGMTDMPlayed.Text = "N/A";

            labelGMDomWins.Text = "N/A";
            labelGMDomLosses.Text = "N/A";
            labelGMDomWL.Text = "N/A";
            labelGMDomPlayed.Text = "N/A";

            labelGMRushWins.Text = "N/A";
            labelGMRushLosses.Text = "N/A";
            labelGMRushWL.Text = "N/A";
            labelGMRushPlayed.Text = "N/A";

            labelGMWPWins.Text = "N/A";
            labelGMWPLosses.Text = "N/A";
            labelGMWPWL.Text = "N/A";
            labelGMWPPlayed.Text = "N/A";

            DrawPieChart(0,0,0,0,0,0);
            DrawBarGraph(0,0,0,0,0,0,0,0,0,0,0,0);

            pbxKD.Visible = false;
            pbxWL.Visible = false;
            pbxSPM.Visible = false;
            pbxKPM.Visible = false;
            pbxSkill.Visible = false;
        
        }

        private void txtPlayerName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                btnSubmit.PerformClick();
            }
        }

        public void searchMatches(int platformID, string name, string link, ref int count, int maxGames, ref int[] killCount, ref int[] deathCount, ref int[] winCount, ref int[] lossCount, ref int[] scoreCount, ref int[] timePlayed, ref int[] gameModeCount)
        {
            if (count >= maxGames)
            {
                return;
            }
            Platform platform = new Platform(platformID);
            try
            {
                var result = new System.Net.WebClient().DownloadString(link);
                if (!result.Contains("We could not find your stats, please ensure your platform and name are correct")) //LOOK AT JUST MAKING IT NOT SEARCH IF THE PLAYER WAS NOT FOUND
                {
                    result = result.Substring(result.IndexOf("class=\"profile-main"), result.IndexOf("class=\"last last-page") - result.IndexOf("class=\"profile-main"));
                    //txtResults.Text += result;
                    int indexLast = result.LastIndexOf("class=\"first first-page");
                    int index1 = 0;
                    while (index1 != -1)
                    {
                        if (count >= maxGames)
                        {
                            return;
                        }
                        index1 = result.IndexOf("href=", index1) + 6;
                        if (index1 >= indexLast)
                        {
                            indexLast = result.LastIndexOf("class=\"next next-page") + 23;
                            if (result.IndexOf("disabled", indexLast, 8) == -1)
                            {
                                indexLast += 5;
                                //txtResults.Text += "NEXT PAGE LINK: " + result.Substring(indexLast, result.IndexOf(">", indexLast) - indexLast) + Environment.NewLine;
                                index1 = -1;
                                searchMatches(platformID, name, "https://battlefieldtracker.com" + result.Substring(indexLast, result.IndexOf(">", indexLast) - indexLast), ref count, maxGames, ref killCount, ref deathCount, ref winCount, ref lossCount, ref scoreCount, ref timePlayed, ref gameModeCount);
                            }
                            else
                            {
                                //txtResults.Text += "NEXT PAGE LINK: UNAVAILABLE. END OF MATCH HISTORY";
                                index1 = -1;
                            }
                        }
                        else
                        {
                            searchGame(name, result.Substring(index1, result.IndexOf("\"", index1) - index1), platform.GetPlatformString(), ref killCount, ref deathCount, ref winCount, ref lossCount, ref scoreCount, ref timePlayed, ref gameModeCount, ref count);
                            //txtResults.Text += "Game Number: " + count + " LINK: " + result.Substring(index1, result.IndexOf("\"", index1) - index1) + "     ";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }

        public void searchGame(string name, string link, string platform, ref int[] killCount, ref int[] deathCount, ref int[] winCount, ref int[] lossCount, ref int[] scoreCount, ref int[] timePlayed, ref int[] gameModeCount, ref int count)
        {
            //substring - "window.trnads" + x offset to first "script" from there. find where instance of href player name is: go until "href=" 
            try
            {
                var result = new System.Net.WebClient().DownloadString("https://battlefieldtracker.com" + link);
                
                result = result.Substring(result.IndexOf("class=\"match-page\""));

                int index = result.IndexOf("class=\"name\">Duration")+73;
                string tempString = result.Substring(index, result.IndexOf("<",index)-index);

                string wonGame;
                if(result.IndexOf("href=\"/bf1/profile/" + platform + "/" + lblNameSTATIC.Text) > result.IndexOf("class=\"card-title\">Team 2")) // if player index is greater than the position of Team 2 members (either part of team 2 or no team)
                {
                    if (tempString == "Team 2")
                    {
                        if (result.IndexOf("href=\"/bf1/profile/" + platform + "/" + lblNameSTATIC.Text) < result.IndexOf("class=\"card-title\">No Team"))
                        {
                            wonGame = "true";
                        }
                        else
                        {
                            wonGame = "check";
                        }
                    }
                    else
                    {
                        wonGame = "false";
                    }
                }
                else
                {
                    if (tempString == "Team 1")
                    {
                        wonGame = "true";
                    }
                    else
                    {
                        wonGame = "false";
                    }
                }
                
                index = (result.IndexOf("<span class="))+ 19;
                tempString = result.Substring(index, (result.IndexOf("</span>", index) - index)); //Gamemode

                index = result.IndexOf("href=\"/bf1/profile/" + platform + "/" + lblNameSTATIC.Text); //Player stats
                int val;
                if (result.LastIndexOf("href=") == index)
                {
                    result = result.Substring(index, (result.Length-1)-index);
                    switch (tempString)
                    {
                        case "Conquest":
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Kills<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                killCount[0] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Deaths<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                deathCount[0] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Score<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                scoreCount[0] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Time Played<")) - 7);
                            tempString = result.Substring(index, result.IndexOf("<", index) - index);

                            val = FindTotalTime(tempString);
                            timePlayed[0] += val;
                            switch (wonGame)
                            {
                                case "true":
                                    winCount[0]++;
                                    break;
                                case "false":
                                    lossCount[0]++;
                                    break;
                                case "check":
                                    if (val > 120) //Time played is greater than 2 minutes count as loss
                                    {
                                        lossCount[0]++;
                                    }
                                    break;
                            }
                            gameModeCount[0]++;
                            count++;
                            break;
                        case "Operations":
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Kills<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                killCount[1] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Deaths<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                deathCount[1] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Score<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                scoreCount[1] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Time Played<")) - 7);
                            tempString = result.Substring(index, result.IndexOf("<", index) - index);

                            val = FindTotalTime(tempString);
                            timePlayed[1] += val;
                            switch (wonGame)
                            {
                                case "true":
                                    winCount[1]++;
                                    break;
                                case "false":
                                    lossCount[1]++;
                                    break;
                                case "check":
                                    if (val > 120) //Time played is greater than 2 minutes count as loss
                                    {
                                        lossCount[1]++;
                                    }
                                    break;
                            }
                            gameModeCount[1]++;
                            count++;
                            break;
                        case "Team Deathmatch":
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Kills<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                killCount[2] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Deaths<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                deathCount[2] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Score<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                scoreCount[2] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Time Played<")) - 7);
                            tempString = result.Substring(index, result.IndexOf("<", index) - index);

                            val = FindTotalTime(tempString);
                            timePlayed[2] += val;
                            switch (wonGame)
                            {
                                case "true":
                                    winCount[2]++;
                                    break;
                                case "false":
                                    lossCount[2]++;
                                    break;
                                case "check":
                                    if (val > 120) //Time played is greater than 2 minutes count as loss
                                    {
                                        lossCount[2]++;
                                    }
                                    break;
                            }
                            gameModeCount[2]++;
                            count++;
                            break;
                        case "Domination":
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Kills<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                killCount[3] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Deaths<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                deathCount[3] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Score<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                scoreCount[3] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Time Played<")) - 7);
                            tempString = result.Substring(index, result.IndexOf("<", index) - index);

                            val = FindTotalTime(tempString);
                            timePlayed[3] += val;
                            switch (wonGame)
                            {
                                case "true":
                                    winCount[3]++;
                                    break;
                                case "false":
                                    lossCount[3]++;
                                    break;
                                case "check":
                                    if (val > 120) //Time played is greater than 2 minutes count as loss
                                    {
                                        lossCount[3]++;
                                    }
                                    break;
                            }
                            gameModeCount[3]++;
                            count++;
                            break;
                        case "Rush":
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Kills<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                killCount[4] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Deaths<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                deathCount[4] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Score<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                scoreCount[4] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Time Played<")) - 7);
                            tempString = result.Substring(index, result.IndexOf("<", index) - index);

                            val = FindTotalTime(tempString);
                            timePlayed[4] += val;
                            switch (wonGame)
                            {
                                case "true":
                                    winCount[4]++;
                                    break;
                                case "false":
                                    lossCount[4]++;
                                    break;
                                case "check":
                                    if (val > 120) //Time played is greater than 2 minutes count as loss
                                    {
                                        lossCount[4]++;
                                    }
                                    break;
                            }
                            gameModeCount[4]++;
                            count++;
                            break;
                        case "War Pigeons":
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Kills<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                killCount[5] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Deaths<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                deathCount[5] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Score<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                scoreCount[5] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Time Played<")) - 7);
                            tempString = result.Substring(index, result.IndexOf("<", index) - index);

                            val = FindTotalTime(tempString);
                            timePlayed[5] += val;
                            switch (wonGame)
                            {
                                case "true":
                                    winCount[5]++;
                                    break;
                                case "false":
                                    lossCount[5]++;
                                    break;
                                case "check":
                                    if (val > 120) //Time played is greater than 2 minutes count as loss
                                    {
                                        lossCount[5]++;
                                    }
                                    break;
                            }
                            gameModeCount[5]++;
                            count++;
                            break;
                        case "Frontlines":
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Kills<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                killCount[6] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Deaths<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                deathCount[6] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Score<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                scoreCount[6] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Time Played<")) - 7);
                            tempString = result.Substring(index, result.IndexOf("<", index) - index);

                            val = FindTotalTime(tempString);
                            timePlayed[6] += val;
                            switch (wonGame)
                            {
                                case "true":
                                    winCount[6]++;
                                    break;
                                case "false":
                                    lossCount[6]++;
                                    break;
                                case "check":
                                    if (val > 120) //Time played is greater than 2 minutes count as loss
                                    {
                                        lossCount[6]++;
                                    }
                                    break;
                            }
                            gameModeCount[6]++;
                            count++;
                            break;
                        case "Supply Drop":
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Kills<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                killCount[7] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Deaths<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                deathCount[7] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Score<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                scoreCount[7] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Time Played<")) - 7);
                            tempString = result.Substring(index, result.IndexOf("<", index) - index);

                            val = FindTotalTime(tempString);
                            timePlayed[7] += val;
                            switch (wonGame)
                            {
                                case "true":
                                    winCount[7]++;
                                    break;
                                case "false":
                                    lossCount[7]++;
                                    break;
                                case "check":
                                    if (val > 120) //Time played is greater than 2 minutes count as loss
                                    {
                                        lossCount[7]++;
                                    }
                                    break;
                            }
                            gameModeCount[7]++;
                            count++;
                            break;
                        case "Air Assault":
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Kills<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                killCount[8] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Deaths<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                deathCount[8] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Score<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                scoreCount[8] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Time Played<")) - 7);
                            tempString = result.Substring(index, result.IndexOf("<", index) - index);

                            val = FindTotalTime(tempString);
                            timePlayed[8] += val;
                            switch (wonGame)
                            {
                                case "true":
                                    winCount[8]++;
                                    break;
                                case "false":
                                    lossCount[8]++;
                                    break;
                                case "check":
                                    if (val > 120) //Time played is greater than 2 minutes count as loss
                                    {
                                        lossCount[8]++;
                                    }
                                    break;
                            }
                            gameModeCount[8]++;
                            count++;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    result = result.Substring(index, result.IndexOf("href=", index + 10) - index);
                    switch (tempString) //FIGURE SOMETHING OUT FOR PARSE
                    {
                        
                        case "Conquest":
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Kills<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                killCount[0] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Deaths<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                deathCount[0] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Score<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                scoreCount[0] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Time Played<")) - 7);
                            tempString = result.Substring(index, result.IndexOf("<", index) - index);

                            val = FindTotalTime(tempString);
                            timePlayed[0] += val;
                            switch (wonGame)
                            {
                                case "true":
                                    winCount[0]++;
                                    break;
                                case "false":
                                    lossCount[0]++;
                                    break;
                                case "check":
                                    if (val > 120) //Time played is greater than 2 minutes count as loss
                                    {
                                        lossCount[0]++;
                                    }
                                    break;
                            }
                            gameModeCount[0]++;
                            count++;
                            break;
                        case "Operations":
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Kills<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                killCount[1] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Deaths<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                deathCount[1] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Score<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                scoreCount[1] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Time Played<")) - 7);
                            tempString = result.Substring(index, result.IndexOf("<", index) - index);

                            val = FindTotalTime(tempString);
                            timePlayed[1] += val;
                            switch (wonGame)
                            {
                                case "true":
                                    winCount[1]++;
                                    break;
                                case "false":
                                    lossCount[1]++;
                                    break;
                                case "check":
                                    if (val > 120) //Time played is greater than 2 minutes count as loss
                                    {
                                        lossCount[1]++;
                                    }
                                    break;
                            }
                            gameModeCount[1]++;
                            count++;
                            break;
                        case "Team Deathmatch":
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Kills<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                killCount[2] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Deaths<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                deathCount[2] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Score<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                scoreCount[2] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Time Played<")) - 7);
                            tempString = result.Substring(index, result.IndexOf("<", index) - index);

                            val = FindTotalTime(tempString);
                            timePlayed[2] += val;
                            switch (wonGame)
                            {
                                case "true":
                                    winCount[2]++;
                                    break;
                                case "false":
                                    lossCount[2]++;
                                    break;
                                case "check":
                                    if (val > 120) //Time played is greater than 2 minutes count as loss
                                    {
                                        lossCount[2]++;
                                    }
                                    break;
                            }
                            gameModeCount[2]++;
                            count++;
                            break;
                        case "Domination":
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Kills<"))-7) + 1;
                            if(Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                killCount[3] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Deaths<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                deathCount[3] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Score<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                scoreCount[3] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Time Played<")) - 7);
                            tempString = result.Substring(index, result.IndexOf("<", index) - index);

                            val = FindTotalTime(tempString);
                            timePlayed[3] += val;
                            switch (wonGame)
                            {
                                case "true":
                                    winCount[3]++;
                                    break;
                                case "false":
                                    lossCount[3]++;
                                    break;
                                case "check":
                                    if (val>120) //Time played is greater than 2 minutes count as loss
                                    {
                                        lossCount[3]++;
                                    }
                                    break;
                            }
                            gameModeCount[3]++;
                            count++;
                            break;
                        case "Rush":
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Kills<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                killCount[4] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Deaths<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                deathCount[4] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Score<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                scoreCount[4] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Time Played<")) - 7);
                            tempString = result.Substring(index, result.IndexOf("<", index) - index);

                            val = FindTotalTime(tempString);
                            timePlayed[4] += val;
                            switch (wonGame)
                            {
                                case "true":
                                    winCount[4]++;
                                    break;
                                case "false":
                                    lossCount[4]++;
                                    break;
                                case "check":
                                    if (val > 120) //Time played is greater than 2 minutes count as loss
                                    {
                                        lossCount[4]++;
                                    }
                                    break;
                            }
                            gameModeCount[4]++;
                            count++;
                            break;
                        case "War Pigeons":
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Kills<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                killCount[5] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Deaths<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                deathCount[5] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Score<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                scoreCount[5] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Time Played<")) - 7);
                            tempString = result.Substring(index, result.IndexOf("<", index) - index);

                            val = FindTotalTime(tempString);
                            timePlayed[5] += val;
                            switch (wonGame)
                            {
                                case "true":
                                    winCount[5]++;
                                    break;
                                case "false":
                                    lossCount[5]++;
                                    break;
                                case "check":
                                    if (val > 120) //Time played is greater than 2 minutes count as loss
                                    {
                                        lossCount[5]++;
                                    }
                                    break;
                            }
                            gameModeCount[5]++;
                            count++;
                            break;
                        case "Frontlines":
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Kills<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                killCount[6] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Deaths<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                deathCount[6] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Score<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                scoreCount[6] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Time Played<")) - 7);
                            tempString = result.Substring(index, result.IndexOf("<", index) - index);

                            val = FindTotalTime(tempString);
                            timePlayed[6] += val;
                            switch (wonGame)
                            {
                                case "true":
                                    winCount[6]++;
                                    break;
                                case "false":
                                    lossCount[6]++;
                                    break;
                                case "check":
                                    if (val > 120) //Time played is greater than 2 minutes count as loss
                                    {
                                        lossCount[6]++;
                                    }
                                    break;
                            }
                            gameModeCount[6]++;
                            count++;
                            break;
                        case "Supply Drop":
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Kills<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                killCount[7] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Deaths<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                deathCount[7] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Score<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                scoreCount[7] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Time Played<")) - 7);
                            tempString = result.Substring(index, result.IndexOf("<", index) - index);

                            val = FindTotalTime(tempString);
                            timePlayed[7] += val;
                            switch (wonGame)
                            {
                                case "true":
                                    winCount[7]++;
                                    break;
                                case "false":
                                    lossCount[7]++;
                                    break;
                                case "check":
                                    if (val > 120) //Time played is greater than 2 minutes count as loss
                                    {
                                        lossCount[7]++;
                                    }
                                    break;
                            }
                            gameModeCount[7]++;
                            count++;
                            break;
                        case "Air Assault":
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Kills<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                killCount[8] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Deaths<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                deathCount[8] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Score<")) - 7) + 1;
                            if (Int32.TryParse(result.Substring(index, result.IndexOf("<", index) - index), System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out val))
                            {
                                scoreCount[8] += val;
                            }
                            index = result.LastIndexOf(">", (result.IndexOf("<div class=\"name\">Time Played<")) - 7);
                            tempString = result.Substring(index, result.IndexOf("<", index) - index);

                            val = FindTotalTime(tempString);
                            timePlayed[8] += val;
                            switch (wonGame)
                            {
                                case "true":
                                    winCount[8]++;
                                    break;
                                case "false":
                                    lossCount[8]++;
                                    break;
                                case "check":
                                    if (val > 120) //Time played is greater than 2 minutes count as loss
                                    {
                                        lossCount[8]++;
                                    }
                                    break;
                            }
                            gameModeCount[8]++;
                            count++;
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }

        private int FindTotalTime(string s)
        {
            int index = 0;
            int val = 0;
            int tempInt = 0;
            if (s.Contains("h"))
            {
                for (index = s.IndexOf("h") - 1; index >= 0; index--)
                {
                    char c = s[index];
                    if (!Char.IsDigit(c))
                    {
                        if (Int32.TryParse(s.Substring(index + 1, ((s.IndexOf("h")) - (index + 1))), out tempInt))
                        {
                            val += ((60 * 60) * tempInt);
                            break;
                        }
                    }
                }
            }
            if (s.Contains("m"))
            {
                for (index = s.IndexOf("m") - 1; index >= 0; index--)
                {
                    char c = s[index];
                    if (!Char.IsDigit(c))
                    {
                        if (Int32.TryParse(s.Substring(index + 1, ((s.IndexOf("m")) - (index + 1))), out tempInt))
                        {
                            val += 60 * tempInt;
                            break;
                        }
                    }
                }
            }
            if (s.Contains("s"))
            {
                for (index = s.IndexOf("s") - 1; index >= 0; index--)
                {
                    char c = s[index];
                    if (!Char.IsDigit(c))
                    {
                        if (Int32.TryParse(s.Substring(index + 1, ((s.IndexOf("s")) - (index + 1))), out tempInt))
                        {
                            val += tempInt;
                            break;
                        }
                    }
                }
            }
            return val;
        }

        private void DrawInDepthGraph(int[] killCount, int[] deathCount, int[] winCount, int[] lossCount, int[] scoreCount, int[] timePlayed, int[] gameModeCount, int count, bool[] modeInclusion, int idType)
        {
            //reset your chart series and legends
            chartInDepth.Series.Clear();
            chartInDepth.Legends.Clear();

            //Add a new Legend(if needed) and do some formating
            chartInDepth.Legends.Add("Legend");
            chartInDepth.Legends[0].LegendStyle = System.Windows.Forms.DataVisualization.Charting.LegendStyle.Table;
            chartInDepth.Legends[0].Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Right;
            chartInDepth.Legends[0].Alignment = StringAlignment.Near;
            chartInDepth.Legends[0].Title = "";
            chartInDepth.Legends[0].BorderColor = Color.Transparent;
            chartInDepth.Legends[0].BackColor = Color.Transparent;
            chartInDepth.Legends[0].ForeColor = Color.White;
            switch (idType)
            {
                case 0:
                    //Add a new chart-series
                    string KD = "K/D";
                    chartInDepth.Series.Add(KD);
                    //set the chart-type to "Column"
                    chartInDepth.Series[KD].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
                    chartInDepth.BackColor = System.Drawing.Color.Transparent;

                    ////Add some datapoints so the series. in this case you can pass the values to this method
                    chartInDepth.Series[KD].IsValueShownAsLabel = true;
                    chartInDepth.Series[KD].LabelForeColor = Color.White;
                    chartInDepth.ChartAreas[0].BackColor = Color.Transparent;
                    chartInDepth.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                    chartInDepth.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
                    chartInDepth.ChartAreas[0].AxisY.Minimum = 0;
                    chartInDepth.ChartAreas[0].AxisY2.Minimum = 0;
                    chartInDepth.ChartAreas[0].AxisY.Maximum = 1000;
                    chartInDepth.ChartAreas[0].AxisY2.Maximum = 1000;
                    chartInDepth.ChartAreas[0].AxisX.LabelStyle.ForeColor = System.Drawing.Color.White;
                    chartInDepth.ChartAreas[0].AxisY.LabelStyle.ForeColor = System.Drawing.Color.White;

                    chartInDepth.Series[KD].Points.AddXY("Conquest", Math.Round((((float)killCount[0] / (float)deathCount[0])), 2));
                    chartInDepth.Series[KD].Points.AddXY("Operations", Math.Round((((float)killCount[1] / (float)deathCount[1])), 2));
                    chartInDepth.Series[KD].Points.AddXY("Domination", Math.Round((((float)killCount[3] / (float)deathCount[3])), 2));
                    chartInDepth.Series[KD].Points.AddXY("Team Deathmatch", Math.Round((((float)killCount[2] / (float)deathCount[2])), 2));
                    chartInDepth.Series[KD].Points.AddXY("Rush", Math.Round((((float)killCount[4] / (float)deathCount[4])), 2));
                    chartInDepth.Series[KD].Points.AddXY("War Pigeons", Math.Round((((float)killCount[5] / (float)deathCount[5])), 2));
                    chartInDepth.Series[KD].Points.AddXY("Frontlines", Math.Round((((float)killCount[6] / (float)deathCount[6])), 2));
                    chartInDepth.Series[KD].Points.AddXY("Supply Drop", Math.Round((((float)killCount[7] / (float)deathCount[7])), 2));
                    chartInDepth.Series[KD].Points.AddXY("Air Assault", Math.Round((((float)killCount[8] / (float)deathCount[8])), 2));
                    
                    labelIDGMSTAT1STATIC.Text = "Total Kills";
                    labelIDGMSTAT2STATIC.Text = "Total Deaths";
                    labelIDGMSTAT3STATIC.Text = "Kills/Deaths";

                    labelIDConqSTAT1.Text = killCount[0].ToString();
                    labelIDConqSTAT2.Text = deathCount[0].ToString();
                    labelIDConqSTAT3.Text = Math.Round(((float)killCount[0]/(float)deathCount[0]),2).ToString();
                    labelIDConqPlayed.Text = gameModeCount[0].ToString();

                    labelIDOpsSTAT1.Text = killCount[1].ToString();
                    labelIDOpsSTAT2.Text = deathCount[1].ToString();
                    labelIDOpsSTAT3.Text = Math.Round(((float)killCount[1] / (float)deathCount[1]), 2).ToString();
                    labelIDOpsPlayed.Text = gameModeCount[1].ToString();

                    labelIDTDMSTAT1.Text = killCount[2].ToString();
                    labelIDTDMSTAT2.Text = deathCount[2].ToString();
                    labelIDTDMSTAT3.Text = Math.Round(((float)killCount[2] / (float)deathCount[2]), 2).ToString();
                    labelIDTDMPlayed.Text = gameModeCount[2].ToString();

                    labelIDDomSTAT1.Text = killCount[3].ToString();
                    labelIDDomSTAT2.Text = deathCount[3].ToString();
                    labelIDDomSTAT3.Text = Math.Round(((float)killCount[3] / (float)deathCount[3]), 2).ToString();
                    labelIDDomPlayed.Text = gameModeCount[3].ToString();

                    labelIDRushSTAT1.Text = killCount[4].ToString();
                    labelIDRushSTAT2.Text = deathCount[4].ToString();
                    labelIDRushSTAT3.Text = Math.Round(((float)killCount[4] / (float)deathCount[4]), 2).ToString();
                    labelIDRushPlayed.Text = gameModeCount[4].ToString();

                    labelIDWPSTAT1.Text = killCount[5].ToString();
                    labelIDWPSTAT2.Text = deathCount[5].ToString();
                    labelIDWPSTAT3.Text = Math.Round(((float)killCount[5] / (float)deathCount[5]), 2).ToString();
                    labelIDWPPlayed.Text = gameModeCount[5].ToString();

                    labelIDFLSTAT1.Text = killCount[6].ToString();
                    labelIDFLSTAT2.Text = deathCount[6].ToString();
                    labelIDFLSTAT3.Text = Math.Round(((float)killCount[6] / (float)deathCount[6]), 2).ToString();
                    labelIDFLPlayed.Text = gameModeCount[6].ToString();

                    labelIDSDSTAT1.Text = killCount[7].ToString();
                    labelIDSDSTAT2.Text = deathCount[7].ToString();
                    labelIDSDSTAT3.Text = Math.Round(((float)killCount[7] / (float)deathCount[7]), 2).ToString();
                    labelIDSDPlayed.Text = gameModeCount[7].ToString();

                    labelIDAASTAT1.Text = killCount[8].ToString();
                    labelIDAASTAT2.Text = deathCount[8].ToString();
                    labelIDAASTAT3.Text = Math.Round(((float)killCount[8] / (float)deathCount[8]), 2).ToString();
                    labelIDAAPlayed.Text = gameModeCount[8].ToString();

                    System.Windows.Forms.DataVisualization.Charting.DataPoint maxDP = chartInDepth.Series[0].Points.FindMaxByValue("Y1", 0);
                    chartInDepth.ChartAreas[0].AxisY.Maximum = Math.Ceiling(maxDP.YValues[0]);
                    chartInDepth.ChartAreas[0].AxisY2.Maximum = chartInDepth.ChartAreas[0].AxisY.Maximum;
                    break;
                case 1:
                    //Add a new chart-series
                    string wins = "Wins";
                    string losses = "Losses";
                    chartInDepth.Series.Add(wins);
                    chartInDepth.Series.Add(losses);
                    //set the chart-type to "Column"
                    chartInDepth.Series[wins].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
                    chartInDepth.Series[losses].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
                    chartInDepth.BackColor = System.Drawing.Color.Transparent;

                    ////Add some datapoints so the series. in this case you can pass the values to this method
                    chartInDepth.Series[wins].IsValueShownAsLabel = true;
                    chartInDepth.Series[wins].LabelForeColor = Color.White;
                    chartInDepth.Series[losses].IsValueShownAsLabel = true;
                    chartInDepth.Series[losses].LabelForeColor = Color.White;
                    chartInDepth.ChartAreas[0].BackColor = Color.Transparent;
                    chartInDepth.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                    chartInDepth.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
                    chartInDepth.ChartAreas[0].AxisY.Maximum = 100;
                    chartInDepth.ChartAreas[0].AxisY.Minimum = 0;
                    chartInDepth.ChartAreas[0].AxisY2.Maximum = 100;
                    chartInDepth.ChartAreas[0].AxisY2.Minimum = 0;
                    chartInDepth.ChartAreas[0].AxisX.LabelStyle.ForeColor = System.Drawing.Color.White;
                    chartInDepth.ChartAreas[0].AxisY.LabelStyle.ForeColor = System.Drawing.Color.White;

                    chartInDepth.Series[wins].Points.AddXY("Conquest", Math.Round((((float)winCount[0] / ((float)winCount[0] + (float)lossCount[0])) * 100), 2));
                    chartInDepth.Series[losses].Points.AddXY("", Math.Round((((float)lossCount[0] / ((float)winCount[0] + (float)lossCount[0])) * 100), 2));
                    chartInDepth.Series[wins].Points.AddXY("Operations", Math.Round((((float)winCount[1] / ((float)winCount[1] + (float)lossCount[1])) * 100), 2));
                    chartInDepth.Series[losses].Points.AddXY("", Math.Round((((float)lossCount[1] / ((float)winCount[1] + (float)lossCount[1])) * 100), 2));
                    chartInDepth.Series[wins].Points.AddXY("Domination", Math.Round((((float)winCount[3] / ((float)winCount[3] + (float)lossCount[3])) * 100), 2));
                    chartInDepth.Series[losses].Points.AddXY("", Math.Round((((float)lossCount[3] / ((float)winCount[3] + (float)lossCount[3])) * 100), 2));
                    chartInDepth.Series[wins].Points.AddXY("Team Deathmatch", Math.Round((((float)winCount[2] / ((float)winCount[2] + (float)lossCount[2])) * 100), 2));
                    chartInDepth.Series[losses].Points.AddXY("", Math.Round((((float)lossCount[2] / ((float)winCount[2] + (float)lossCount[2])) * 100), 2));
                    chartInDepth.Series[wins].Points.AddXY("Rush", Math.Round((((float)winCount[4] / ((float)winCount[4] + (float)lossCount[4])) * 100), 2));
                    chartInDepth.Series[losses].Points.AddXY("", Math.Round((((float)lossCount[4] / ((float)winCount[4] + (float)lossCount[4])) * 100), 2));
                    chartInDepth.Series[wins].Points.AddXY("War Pigeons", Math.Round((((float)winCount[5] / ((float)winCount[5] + (float)lossCount[5])) * 100), 2));
                    chartInDepth.Series[losses].Points.AddXY("", Math.Round((((float)lossCount[5] / ((float)winCount[5] + (float)lossCount[5])) * 100), 2));
                    chartInDepth.Series[wins].Points.AddXY("Frontlines", Math.Round((((float)winCount[6] / ((float)winCount[6] + (float)lossCount[6])) * 100), 2));
                    chartInDepth.Series[losses].Points.AddXY("", Math.Round((((float)lossCount[6] / ((float)winCount[6] + (float)lossCount[6])) * 100), 2));
                    chartInDepth.Series[wins].Points.AddXY("Supply Drop", Math.Round((((float)winCount[7] / ((float)winCount[7] + (float)lossCount[7])) * 100), 2));
                    chartInDepth.Series[losses].Points.AddXY("", Math.Round((((float)lossCount[7] / ((float)winCount[7] + (float)lossCount[7])) * 100), 2));
                    chartInDepth.Series[wins].Points.AddXY("Air Assault", Math.Round((((float)winCount[8] / ((float)winCount[8] + (float)lossCount[8])) * 100), 2));
                    chartInDepth.Series[losses].Points.AddXY("", Math.Round((((float)lossCount[8] / ((float)winCount[8] + (float)lossCount[8])) * 100), 2));


                    labelIDGMSTAT1STATIC.Text = "Total Wins";
                    labelIDGMSTAT2STATIC.Text = "Total Losses";
                    labelIDGMSTAT3STATIC.Text = "Win Ratio";

                    labelIDConqSTAT1.Text = winCount[0].ToString();
                    labelIDConqSTAT2.Text = lossCount[0].ToString();
                    labelIDConqSTAT3.Text = Math.Round((((float)winCount[0] / ((float)winCount[0] + (float)lossCount[0])) * 100), 2).ToString() + "%";
                    labelIDConqPlayed.Text = gameModeCount[0].ToString();

                    labelIDOpsSTAT1.Text = winCount[1].ToString();
                    labelIDOpsSTAT2.Text = lossCount[1].ToString();
                    labelIDOpsSTAT3.Text = Math.Round((((float)winCount[1] / ((float)winCount[1] + (float)lossCount[1])) * 100), 2).ToString() + "%";
                    labelIDOpsPlayed.Text = gameModeCount[1].ToString();

                    labelIDTDMSTAT1.Text = winCount[2].ToString();
                    labelIDTDMSTAT2.Text = lossCount[2].ToString();
                    labelIDTDMSTAT3.Text = Math.Round((((float)winCount[2] / ((float)winCount[2] + (float)lossCount[2])) * 100), 2).ToString() + "%";
                    labelIDTDMPlayed.Text = gameModeCount[2].ToString();

                    labelIDDomSTAT1.Text = winCount[3].ToString();
                    labelIDDomSTAT2.Text = lossCount[3].ToString();
                    labelIDDomSTAT3.Text = Math.Round((((float)winCount[3] / ((float)winCount[3] + (float)lossCount[3])) * 100), 2).ToString() + "%";
                    labelIDDomPlayed.Text = gameModeCount[3].ToString();

                    labelIDRushSTAT1.Text = winCount[4].ToString();
                    labelIDRushSTAT2.Text = lossCount[4].ToString();
                    labelIDRushSTAT3.Text = Math.Round((((float)winCount[4] / ((float)winCount[4] + (float)lossCount[4])) * 100), 2).ToString() + "%";
                    labelIDRushPlayed.Text = gameModeCount[4].ToString();

                    labelIDWPSTAT1.Text = winCount[5].ToString();
                    labelIDWPSTAT2.Text = lossCount[5].ToString();
                    labelIDWPSTAT3.Text = Math.Round((((float)winCount[5] / ((float)winCount[5] + (float)lossCount[5])) * 100), 2).ToString() + "%";
                    labelIDWPPlayed.Text = gameModeCount[5].ToString();

                    labelIDFLSTAT1.Text = winCount[6].ToString();
                    labelIDFLSTAT2.Text = lossCount[6].ToString();
                    labelIDFLSTAT3.Text = Math.Round((((float)winCount[6] / ((float)winCount[6] + (float)lossCount[6])) * 100), 2).ToString() + "%";
                    labelIDFLPlayed.Text = gameModeCount[6].ToString();

                    labelIDSDSTAT1.Text = winCount[7].ToString();
                    labelIDSDSTAT2.Text = lossCount[7].ToString();
                    labelIDSDSTAT3.Text = Math.Round((((float)winCount[7] / ((float)winCount[7] + (float)lossCount[7])) * 100), 2).ToString() + "%";
                    labelIDSDPlayed.Text = gameModeCount[7].ToString();

                    labelIDAASTAT1.Text = winCount[8].ToString();
                    labelIDAASTAT2.Text = lossCount[8].ToString();
                    labelIDAASTAT3.Text = Math.Round((((float)winCount[8] / ((float)winCount[8] + (float)lossCount[8])) * 100), 2).ToString() + "%";
                    labelIDAAPlayed.Text = gameModeCount[8].ToString();
                    break;
                case 2:
                    //Add a new chart-series
                    string score = "SPM";
                    chartInDepth.Series.Add(score);
                    //set the chart-type to "Column"
                    chartInDepth.Series[score].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
                    chartInDepth.BackColor = System.Drawing.Color.Transparent;

                    ////Add some datapoints so the series. in this case you can pass the values to this method
                    chartInDepth.Series[score].IsValueShownAsLabel = true;
                    chartInDepth.Series[score].LabelForeColor = Color.White;
                    chartInDepth.ChartAreas[0].BackColor = Color.Transparent;
                    chartInDepth.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                    chartInDepth.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
                    chartInDepth.ChartAreas[0].AxisY.Maximum = 10000;
                    chartInDepth.ChartAreas[0].AxisY.Minimum = 0;
                    chartInDepth.ChartAreas[0].AxisY2.Maximum = 10000;
                    chartInDepth.ChartAreas[0].AxisY2.Minimum = 0;
                    chartInDepth.ChartAreas[0].AxisX.LabelStyle.ForeColor = System.Drawing.Color.White;
                    chartInDepth.ChartAreas[0].AxisY.LabelStyle.ForeColor = System.Drawing.Color.White;

                    chartInDepth.Series[score].Points.AddXY("Conquest", Math.Round((((float)scoreCount[0] / ((float)timePlayed[0]/60))), 2));
                    chartInDepth.Series[score].Points.AddXY("Operations", Math.Round((((float)scoreCount[1] / ((float)timePlayed[1] / 60))), 2));
                    chartInDepth.Series[score].Points.AddXY("Domination", Math.Round((((float)scoreCount[3] / ((float)timePlayed[3] / 60))), 2));
                    chartInDepth.Series[score].Points.AddXY("Team Deathmatch", Math.Round((((float)scoreCount[2] / ((float)timePlayed[2] / 60))), 2));
                    chartInDepth.Series[score].Points.AddXY("Rush", Math.Round((((float)scoreCount[4] / ((float)timePlayed[4] / 60))), 2));
                    chartInDepth.Series[score].Points.AddXY("War Pigeons", Math.Round((((float)scoreCount[5] / ((float)timePlayed[5] / 60))), 2));
                    chartInDepth.Series[score].Points.AddXY("Frontlines", Math.Round((((float)scoreCount[6] / ((float)timePlayed[6] / 60))), 2));
                    chartInDepth.Series[score].Points.AddXY("Supply Drop", Math.Round((((float)scoreCount[7] / ((float)timePlayed[7] / 60))), 2));
                    chartInDepth.Series[score].Points.AddXY("Air Assault", Math.Round((((float)scoreCount[8] / ((float)timePlayed[8] / 60))), 2));

                    labelIDGMSTAT1STATIC.Text = "Total Score";
                    labelIDGMSTAT2STATIC.Text = "Time Played";
                    labelIDGMSTAT3STATIC.Text = "Score/Minute";


                    labelIDConqSTAT1.Text = scoreCount[0].ToString();
                    labelIDConqSTAT2.Text = timePlayed[0].ToString();
                    labelIDConqSTAT3.Text = Math.Round((((float)scoreCount[0] / ((float)timePlayed[0] / 60))), 2).ToString();
                    labelIDConqPlayed.Text = gameModeCount[0].ToString();

                    labelIDOpsSTAT1.Text = scoreCount[1].ToString();
                    labelIDOpsSTAT2.Text = timePlayed[1].ToString();
                    labelIDOpsSTAT3.Text = Math.Round((((float)scoreCount[1] / ((float)timePlayed[1] / 60))), 2).ToString();
                    labelIDOpsPlayed.Text = gameModeCount[1].ToString();

                    labelIDTDMSTAT1.Text = scoreCount[2].ToString();
                    labelIDTDMSTAT2.Text = timePlayed[2].ToString();
                    labelIDTDMSTAT3.Text = Math.Round((((float)scoreCount[2] / ((float)timePlayed[2] / 60))), 2).ToString();
                    labelIDTDMPlayed.Text = gameModeCount[2].ToString();

                    labelIDDomSTAT1.Text = scoreCount[3].ToString();
                    labelIDDomSTAT2.Text = timePlayed[3].ToString();
                    labelIDDomSTAT3.Text = Math.Round((((float)scoreCount[3] / ((float)timePlayed[3] / 60))), 2).ToString();
                    labelIDDomPlayed.Text = gameModeCount[3].ToString();

                    labelIDRushSTAT1.Text = scoreCount[4].ToString();
                    labelIDRushSTAT2.Text = timePlayed[4].ToString();
                    labelIDRushSTAT3.Text = Math.Round((((float)scoreCount[4] / ((float)timePlayed[4] / 60))), 2).ToString();
                    labelIDRushPlayed.Text = gameModeCount[4].ToString();

                    labelIDWPSTAT1.Text = scoreCount[5].ToString();
                    labelIDWPSTAT2.Text = timePlayed[5].ToString();
                    labelIDWPSTAT3.Text = Math.Round((((float)scoreCount[5] / ((float)timePlayed[5] / 60))), 2).ToString();
                    labelIDWPPlayed.Text = gameModeCount[5].ToString();

                    labelIDFLSTAT1.Text = scoreCount[6].ToString();
                    labelIDFLSTAT2.Text = timePlayed[6].ToString();
                    labelIDFLSTAT3.Text = Math.Round((((float)scoreCount[6] / ((float)timePlayed[6] / 60))), 2).ToString();
                    labelIDFLPlayed.Text = gameModeCount[6].ToString();

                    labelIDSDSTAT1.Text = scoreCount[7].ToString();
                    labelIDSDSTAT2.Text = timePlayed[7].ToString();
                    labelIDSDSTAT3.Text = Math.Round((((float)scoreCount[7] / ((float)timePlayed[7] / 60))), 2).ToString();
                    labelIDSDPlayed.Text = gameModeCount[7].ToString();

                    labelIDAASTAT1.Text = scoreCount[8].ToString();
                    labelIDAASTAT2.Text = timePlayed[8].ToString();
                    labelIDAASTAT3.Text = Math.Round((((float)scoreCount[8] / ((float)timePlayed[8] / 60))), 2).ToString();
                    labelIDAAPlayed.Text = gameModeCount[8].ToString();
                    break;
                default:
                    break;
            }

            chartInDepth.Update();
            
        }

        private void btnSelectAllclb_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clbIDGameModes.Items.Count; i++)
            {
                clbIDGameModes.SetItemChecked(i, true);
            }
        }

        private void btnDeselectAllclb_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clbIDGameModes.Items.Count; i++)
            {
                clbIDGameModes.SetItemChecked(i, false);
            }
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            InDepthInformation idInfo = e.Argument as InDepthInformation;
            searchMatches(idInfo.platform.GetPlatformID(), idInfo.name, "https://battlefieldtracker.com/bf1/profile/" + idInfo.platform.GetPlatformString() + "/" + idInfo.name + "/matches", ref idInfo.count, idInfo.maxGames, ref idInfo.killCount, ref idInfo.deathCount, ref idInfo.winCount, ref idInfo.lossCount, ref idInfo.scoreCount, ref idInfo.timePlayed, ref idInfo.gameModeCount);
            e.Result = idInfo;
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            InDepthInformation idInfo = e.Result as InDepthInformation;
            if (e.Error != null)
            {

            }
            else if (e.Cancelled)
            {

            }
            else
            {
                if (panelSideIcon.Location.Y == btnInDepth.Location.Y)
                {
                    panelInDepth.Visible = true;
                    loadingCircle.Active = false;
                    panelLoading.Visible = false;

                    btnSelectAllclb.PerformClick();

                    DrawInDepthGraph(idInfo.killCount, idInfo.deathCount, idInfo.winCount, idInfo.lossCount, idInfo.scoreCount, idInfo.timePlayed, idInfo.gameModeCount, idInfo.count, idInfo.modeInclusion, idInfo.typeID);
                }
            }

        }

        private void btnIDKD_Click(object sender, EventArgs e)
        {
            idType = 0;
            btnIDUpdate.PerformClick();
        }

        private void btnIDWL_Click(object sender, EventArgs e)
        {
            idType = 1;
            btnIDUpdate.PerformClick();
        }

        private void btnIDSPM_Click(object sender, EventArgs e)
        {
            idType = 2;
            btnIDUpdate.PerformClick();
        }

        private void btnIDUpdate_Click(object sender, EventArgs e) //FIX IF PLAYER IS NOT FOUND OR NO GAMES AT ALL. ALSO WHEN YOU GO TO SEARCH IT CANCELS BACKGROUND WORKER
        {

            panelInDepth.Visible = false;
            panelWeapons.Visible = false;
            loadingCircle.Active = true;
            panelLoading.Visible = true;

            if (idType > 10)
            {

            }
            else
            {
                if (!backgroundWorker.IsBusy)
                {
                    InDepthInformation idInfo = new InDepthInformation(cbxPlatform.SelectedIndex + 1, (cbxIDMatches.SelectedIndex + 1) * 10, lblNameSTATIC.Text,idType);
                    backgroundWorker.RunWorkerAsync(idInfo);
                    idType = 11;
                }
            }

            cbxIDStat.SelectedIndex = 0;
            cbxIDWeapon.SelectedIndex = 0;
        }
        
    }
}
