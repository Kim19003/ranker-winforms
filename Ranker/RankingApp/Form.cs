using RankingApp.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace RankingApp
{
    public partial class Form1 : Form
    {
        public static string AppName { get; } = "Ranker";

        static readonly Dictionary<TextBox, Panel> teamPointTextBoxAndTeamPanelRelations = new Dictionary<TextBox, Panel>();
        static readonly Dictionary<PictureBox, Panel> teamPictureBoxAndTeamPanelRelations = new Dictionary<PictureBox, Panel>();
        static readonly Dictionary<TextBox, PictureBox> teamCountryTextBoxAndTeamPictureBoxRelations = new Dictionary<TextBox, PictureBox>();
        static readonly Dictionary<TextBox, TextBox> teamNameTextBoxAndPointBoxRelations = new Dictionary<TextBox, TextBox>();

        static readonly Dictionary<string, string> teamLastCountry = new Dictionary<string, string>();
        static Dictionary<string, string> countryAbbreviations = new Dictionary<string, string>();

        static readonly List<Panel> teamPanels = new List<Panel>();
        static readonly List<Panel> locationPointerPanels = new List<Panel>();
        static readonly List<TextBox> teamNameTextBoxes = new List<TextBox>();
        static readonly List<TextBox> teamPointTextBoxes = new List<TextBox>();
        static readonly List<TextBox> teamCountryTextBoxes = new List<TextBox>();
        static readonly List<PictureBox> teamPictureBoxes = new List<PictureBox>();

        static readonly List<Structure> currentStructures = new List<Structure>();
        static readonly List<Structure> loadedStructures = new List<Structure>();

        static string configPath = AppDomain.CurrentDomain.BaseDirectory + "Config.json";
        static readonly string settingsPath = AppDomain.CurrentDomain.BaseDirectory + "Settings.json";
        static readonly string abbreviationsPath = AppDomain.CurrentDomain.BaseDirectory + "CountryAbbreviations.txt";

        static int DefaultPanelLocationX { get; set; }
        static int MixPanelLocationY { get; set; }
        static int MaxPanelLocationY { get; set; }

        static bool isCurrentSession; // Used to change the change images based by position changes since last session

        readonly Timer loopTimer = new Timer()
        {
            Interval = 1,
            Enabled = false
        };

        public Form1()
        {
            InitializeComponent();

            MaximizeBox = false;

            InitializeObjects.InitTeamPanels(Controls, teamPanels);
            InitializeObjects.InitLocationPointerPanels(Controls, locationPointerPanels);
            InitializeObjects.InitTeamNameTextBoxes(Controls, teamNameTextBoxes);
            InitializeObjects.InitTeamPointTextBoxes(Controls, teamPointTextBoxes);
            InitializeObjects.InitTeamCountryTextBoxes(Controls, teamCountryTextBoxes);
            InitializeObjects.InitTeamPictureBoxes(Controls, teamPictureBoxes);

            RelationCreatingMethods.CreateTeamPanelAndTeamPointTextBoxRelations(teamPanels, teamPointTextBoxes, teamPointTextBoxAndTeamPanelRelations);
            RelationCreatingMethods.CreateTeamPictureBoxAndTeamPanelRelations(teamPictureBoxes, teamPanels, teamPictureBoxAndTeamPanelRelations);
            RelationCreatingMethods.CreateTeamCountryTextBoxAndTeamPictureBoxRelations(teamCountryTextBoxes, teamPictureBoxes, teamCountryTextBoxAndTeamPictureBoxRelations);
            RelationCreatingMethods.CreateTeamNameTextBoxAndTeamPointTextBoxRelations(teamNameTextBoxes, teamPointTextBoxes, teamNameTextBoxAndPointBoxRelations);

            DefaultPanelLocationX = panel1.Location.X;
            MixPanelLocationY = panel1.Location.Y;
            MaxPanelLocationY = panel50.Location.Y;

            foreach (TextBox teamNameTextBox in teamNameTextBoxes)
            {
                teamNameTextBox.KeyDown += new KeyEventHandler(TeamNameBox_KeyDown);
                teamNameTextBox.AutoSize = false;
                teamNameTextBox.Size = new Size(324, 31);
            }
            foreach (TextBox pointsTextBox in teamPointTextBoxes)
            {
                pointsTextBox.KeyPress += new KeyPressEventHandler(PointBox_KeyPress);
            }
            foreach (TextBox countryTextBox in teamCountryTextBoxes)
            {
                countryTextBox.KeyPress += new KeyPressEventHandler(CountryTextBox_Submit);
                countryTextBox.Leave += new EventHandler(CountryBox_Leave);
                countryTextBox.Visible = false;
            }
            foreach (PictureBox pictureTextBox in teamPictureBoxes)
            {
                pictureTextBox.Click += new EventHandler(PictureBox_Click);
            }

            if (!SavingAndLoadingMethods.GetSettings(settingsPath, ref configPath, updatedLabel))
            {
                throw new Exception("Can't get settings!");
            }
            SavingAndLoadingMethods.LoadData(Controls, loadedStructures, teamLastCountry, configPath, teamNameTextBoxes, teamPointTextBoxes,
                locationPointerPanels, countryAbbreviations, DefaultPanelLocationX, teamPictureBoxAndTeamPanelRelations);

            countryAbbreviations = RuleMethods.GetCountryAbbreviations(abbreviationsPath);
            isCurrentSession = false;

            //loopTimer.Tick += new EventHandler(loopTimerEvent);
        }

        private void Form_Load(object sender, EventArgs e)
        {
            Location = new Point(Properties.Settings.Default.Form_Location.X, Properties.Settings.Default.Form_Location.Y);
        }

        private void Form_Closing(object sender, FormClosingEventArgs e)
        {
            isCurrentSession = false;

            if (currentStructures.Count > 0 && SavingAndLoadingMethods.IsThereChanges(configPath, currentStructures))
            {
                if (MessageBox.Show("Unsaved changes detected! Do you want to save before exiting?", AppName,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    SavingAndLoadingMethods.SaveToConfig(configPath, currentStructures);
                }
            }

            Properties.Settings.Default.Form_Location = Location;
            Properties.Settings.Default.Save();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (SavingAndLoadingMethods.IsThereChanges(configPath, currentStructures))
            {
                SavingAndLoadingMethods.SaveToConfig(configPath, currentStructures);
            }
        }

        private void getConfigPathButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"Current config path: {configPath}", AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void updateButton_Click(object sender, EventArgs e)
        {
            RuleMethods.TryToUpdate(AppName, teamPointTextBoxes, teamNameTextBoxes, teamPictureBoxAndTeamPanelRelations, teamPointTextBoxAndTeamPanelRelations,
                locationPointerPanels, DefaultPanelLocationX, configPath, settingsPath, ref isCurrentSession, currentStructures, teamPanels, teamLastCountry,
                updatedLabel);
        }

        public void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5) // Refresh
            {
                RuleMethods.TryToUpdate(AppName, teamPointTextBoxes, teamNameTextBoxes, teamPictureBoxAndTeamPanelRelations, teamPointTextBoxAndTeamPanelRelations,
                    locationPointerPanels, DefaultPanelLocationX, configPath, settingsPath, ref isCurrentSession, currentStructures, teamPanels, teamLastCountry,
                    updatedLabel);
            }

            if (e.Control && e.KeyCode == Keys.S) // Save
            {
                if (SavingAndLoadingMethods.IsThereChanges(configPath, currentStructures))
                {
                    SavingAndLoadingMethods.SaveToConfig(configPath, currentStructures);
                }
            }
        }

        public void changeConfigPathButton_Click(object sender, EventArgs e)
        {
            if (!SavingAndLoadingMethods.SetSettings(settingsPath, configPath, updatedLabel, true))
            {
                // Error
            }
        }

        public void TeamNameBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                RuleMethods.TryToUpdate(AppName, teamPointTextBoxes, teamNameTextBoxes, teamPictureBoxAndTeamPanelRelations, teamPointTextBoxAndTeamPanelRelations,
                    locationPointerPanels, DefaultPanelLocationX, configPath, settingsPath, ref isCurrentSession, currentStructures, teamPanels, teamLastCountry,
                    updatedLabel);

                e.SuppressKeyPress = true;
            }
        }


        public void PointBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                RuleMethods.TryToUpdate(AppName, teamPointTextBoxes, teamNameTextBoxes, teamPictureBoxAndTeamPanelRelations, teamPointTextBoxAndTeamPanelRelations,
                    locationPointerPanels, DefaultPanelLocationX, configPath, settingsPath, ref isCurrentSession, currentStructures, teamPanels, teamLastCountry,
                    updatedLabel);

                e.Handled = true;
            }

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        public void PictureBox_Click(object sender, EventArgs e)
        {
            PictureBox pictureBox = (PictureBox)sender;

            Panel panel = teamPictureBoxAndTeamPanelRelations[pictureBox];
            TextBox countryBox = (TextBox)panel.Controls.Find("countryBox" + panel.Name.Substring(5), true)[0];

            if (countryBox.Visible)
            {
                countryBox.Visible = false;
            }
            else
            {
                countryBox.Visible = true;
            }
        }

        public void CountryTextBox_Submit(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (e.KeyChar == (char)Keys.Enter)
            {
                PictureBox pictureBox = teamCountryTextBoxAndTeamPictureBoxRelations[textBox];

                pictureBox.ImageLocation = RuleMethods.TryGetImageLocationWithSearchQuery(textBox.Text, ".png", countryAbbreviations);
                if (textBox.Text.Length == 2)
                {
                    if (countryAbbreviations.ContainsKey(textBox.Text))
                    {
                        pictureBox.Tag = countryAbbreviations[textBox.Text];
                    }
                    else
                    {
                        pictureBox.Tag = string.Empty;
                    }
                }
                else if (textBox.Text.Length > 1)
                {
                    pictureBox.Tag = textBox.Text[0].ToString().ToUpper() + textBox.Text.Substring(1);
                }
                else
                {
                    pictureBox.Tag = string.Empty;
                }

                e.Handled = true;

                textBox.Visible = false;
            }
        }

        public void CountryBox_Leave(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            textBox.Visible = false;
        }
    }
}
