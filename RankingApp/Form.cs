using RankingApp.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RankingApp
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        public static string AppName { get { return "Ranker"; } }

        static Dictionary<TextBox, Panel> panelRelations; // Panel relations to points boxes
        static Dictionary<PictureBox, Panel> pictureBoxRelations; // Picture box relations to panels
        static Dictionary<Panel, PictureBox> pictureBoxRelationsV; // Picture box relations to panels (vice versa)
        static Dictionary<TextBox, PictureBox> countryBoxRelations; // country box relations to picture boxes
        static Dictionary<TextBox, TextBox> teamPointRelations; // team box relations to point boxes
        static Dictionary<string, string> teamLastCountry; // Used in country saving
        static Dictionary<string, string> countryAbbreviations; // Used in search queries

        static List<Panel> panels, locationPanels;
        static List<TextBox> teamNameBoxes, pointBoxes, countryBoxes;
        static List<PictureBox> pictureBoxes;

        static List<Structure> structures; // Current structures
        static List<Structure> loadedStructures; // Loaded structures

        static string configPath, settingsPath, abbreviationsPath;

        static int DefaultPanelLocationX { get; set; }

        static bool isCurrentSession; // Used to change the change images based by position changes since last session

        public Form1()
        {
            InitializeComponent();

            MaximizeBox = false;

            // -- Dictionaries --
            panelRelations = new Dictionary<TextBox, Panel>();
            pictureBoxRelations = new Dictionary<PictureBox, Panel>();
            pictureBoxRelationsV = new Dictionary<Panel, PictureBox>();
            countryBoxRelations = new Dictionary<TextBox, PictureBox>();
            teamPointRelations = new Dictionary<TextBox, TextBox>();
            teamLastCountry = new Dictionary<string, string>();
            countryAbbreviations = new Dictionary<string, string>();
            // ---

            // -- Lists --
            panels = new List<Panel>();
            locationPanels = new List<Panel>();
            teamNameBoxes = new List<TextBox>();
            pointBoxes = new List<TextBox>();
            countryBoxes = new List<TextBox>(); 
            pictureBoxes = new List<PictureBox>();
            structures = new List<Structure>();
            loadedStructures = new List<Structure>();
            // ---

            // -- Paths --
            configPath = AppDomain.CurrentDomain.BaseDirectory + "Config.json";
            settingsPath = AppDomain.CurrentDomain.BaseDirectory + "Settings.json";
            abbreviationsPath = AppDomain.CurrentDomain.BaseDirectory + "CountryAbbreviations.txt";
            // ---

            InitializeObjects.InitPanels(Controls, panels);
            InitializeObjects.InitLocationPanels(Controls, locationPanels);
            InitializeObjects.InitTeamNameBoxes(Controls, teamNameBoxes);
            InitializeObjects.InitPointBoxes(Controls, pointBoxes);
            InitializeObjects.InitCountryBoxes(Controls, countryBoxes);
            InitializeObjects.InitPictureBoxes(Controls, pictureBoxes);

            RelationCreatingMethods.CreatePanelRelation(panels, pointBoxes, panelRelations);
            RelationCreatingMethods.CreatePictureBoxRelation(pictureBoxes, panels, pictureBoxRelations);
            RelationCreatingMethods.CreatePictureBoxRelationV(panels, pictureBoxes, pictureBoxRelationsV);
            RelationCreatingMethods.CreateCountryBoxRelation(countryBoxes, pictureBoxes, countryBoxRelations);
            RelationCreatingMethods.CreatePointBoxRelation(teamNameBoxes, pointBoxes, teamPointRelations);

            DefaultPanelLocationX = panel1.Location.X;

            // -- Event Adding --
            foreach (TextBox teamNameBox in teamNameBoxes)
            {
                teamNameBox.KeyDown += new KeyEventHandler(TeamNameBox_KeyDown);
                teamNameBox.AutoSize = false;
                teamNameBox.Size = new Size(324, 31);
            }
            foreach (TextBox pointsBox in pointBoxes)
            {
                pointsBox.KeyPress += new KeyPressEventHandler(PointBox_KeyPress);
            }
            foreach (TextBox countryBox in countryBoxes)
            {
                countryBox.KeyPress += new KeyPressEventHandler(CountryTextBox_Submit);
                countryBox.Leave += new EventHandler(CountryBox_Leave);
                countryBox.Visible = false;
            }
            foreach (PictureBox pictureBox in pictureBoxes)
            {
                pictureBox.Click += new EventHandler(PictureBox_Click);
            }
            // ---

            if (!SavingAndLoadingMethods.GetSettings(settingsPath, ref configPath, updatedLabel))
            {
                // Error
            }
            SavingAndLoadingMethods.LoadData(Controls, ref loadedStructures, pictureBoxRelationsV, teamLastCountry, configPath, teamNameBoxes, pointBoxes, locationPanels,
                countryAbbreviations, DefaultPanelLocationX); // Load config

            countryAbbreviations = RuleMethods.GetCountryAbbreviations(abbreviationsPath);
            isCurrentSession = false;
        }

        private void Form_Load(object sender, EventArgs e)
        {
            Location = new Point(Properties.Settings.Default.Form_Location.X, Properties.Settings.Default.Form_Location.Y);
        }

        private void Form_Closing(object sender, FormClosingEventArgs e)
        {
            isCurrentSession = false;

            if (structures.Count > 0 && SavingAndLoadingMethods.IsThereChanges(configPath, structures))
            {
                if (MessageBox.Show("Unsaved changes detected! Do you want to save before exiting?", AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    SavingAndLoadingMethods.SaveToConfig(configPath, structures);
                }
            }

            Properties.Settings.Default.Form_Location = Location;
            Properties.Settings.Default.Save();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (SavingAndLoadingMethods.IsThereChanges(configPath, structures))
            {
                SavingAndLoadingMethods.SaveToConfig(configPath, structures);
            }
        }

        private void getConfigPathButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"Current config path: {configPath}", AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void updateButton_Click(object sender, EventArgs e)
        {
            RuleMethods.TryToUpdate(AppName, pointBoxes, teamNameBoxes, panelRelations, locationPanels, DefaultPanelLocationX, configPath, settingsPath, ref isCurrentSession, ref structures, loadedStructures,
                panels, pictureBoxRelationsV, teamLastCountry, Controls, updatedLabel);
        }

        public void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5) // F5, refresh
            {
                RuleMethods.TryToUpdate(AppName, pointBoxes, teamNameBoxes, panelRelations, locationPanels, DefaultPanelLocationX, configPath, settingsPath, ref isCurrentSession, ref structures, loadedStructures,
                    panels, pictureBoxRelationsV, teamLastCountry, Controls, updatedLabel);
            }

            if (e.Control && e.KeyCode == Keys.S) // CTRL + S, save
            {
                if (SavingAndLoadingMethods.IsThereChanges(configPath, structures))
                {
                    SavingAndLoadingMethods.SaveToConfig(configPath, structures);
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

        public void TeamNameBox_KeyDown(object sender, KeyEventArgs e) // Disable line breaks
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                RuleMethods.TryToUpdate(AppName, pointBoxes, teamNameBoxes, panelRelations, locationPanels, DefaultPanelLocationX, configPath, settingsPath, ref isCurrentSession, ref structures, loadedStructures,
                    panels, pictureBoxRelationsV, teamLastCountry, Controls, updatedLabel);

                e.SuppressKeyPress = true;
            }
        }


        public void PointBox_KeyPress(object sender, KeyPressEventArgs e) // Allow only numbers to points boxes
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                RuleMethods.TryToUpdate(AppName, pointBoxes, teamNameBoxes, panelRelations, locationPanels, DefaultPanelLocationX, configPath, settingsPath, ref isCurrentSession, ref structures, loadedStructures,
                    panels, pictureBoxRelationsV, teamLastCountry, Controls, updatedLabel);

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

            Panel panel = pictureBoxRelations[pictureBox];
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
                PictureBox pictureBox = countryBoxRelations[textBox];

                pictureBox.ImageLocation = RuleMethods.TryGetImageLocationWithSearchQuery(textBox.Text, ".png", countryAbbreviations);
                if (textBox.Text.Length == 2)
                {
                    try
                    {
                        pictureBox.Tag = countryAbbreviations[textBox.Text];
                    }
                    catch
                    {
                        pictureBox.Tag = string.Empty;
                    }
                }
                else if (textBox.Text.Length > 1)
                {
                    pictureBox.Tag = textBox.Text.Substring(0, 1).ToUpper() + textBox.Text.Substring(1);
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

    public enum Change { Up, Down, None, New }

    class Structure
    {
        public int Rank { get; set; }
        public string Team { get; set; }
        public string Country { get; set; }
        public int Points { get; set; }
        public Change Change { get; set; }
        public int PositionChange { get; set; }
        public string Panel { get; set; }
    }

    class Settings
    {
        public string Date { get; set; }
        public string ConfigPath { get; set; }
    }
}
