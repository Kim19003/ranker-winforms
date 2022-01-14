using Newtonsoft.Json;
using RankingApp.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RankingApp
{
    public partial class Form1 : Form
    {
        public string AppName { get; }

        Dictionary<TextBox, Panel> panelRelations; // Panel relations to points boxes
        Dictionary<PictureBox, Panel> pictureBoxRelations; // Picture box relations to panels
        Dictionary<Panel, PictureBox> pictureBoxRelationsV; // Picture box relations to panels (vice versa)
        Dictionary<TextBox, PictureBox> countryBoxRelations; // country box relations to picture boxes
        Dictionary<TextBox, TextBox> teamPointRelations; // team box relations to point boxes
        Dictionary<string, Change> teamLastPositionChange; // Used in position change imaging
        Dictionary<string, string> teamLastCountry; // Used in country saving
        Dictionary<string, string> countryAbbreviations; // Used in search queries
        Dictionary<Panel, int> oldRanks; // Used ignoring change picture updating, if no rank changes have been made

        List<Panel> panels, locationPanels;
        List<TextBox> teamNameBoxes, pointBoxes, countryBoxes;
        List<PictureBox> pictureBoxes;
        List<Structure> structures; // Updated structures just before saving

        Image arrowUpImage, arrowDownImage, minusSymbolImage, newEntryImage;

        string configPath, settingsPath, abbreviationsPath;

        int DefaultPanelLocationX { get; }

        bool isCurrentSession; // Used to change the change images based by position changes since last session

        public Form1()
        {
            InitializeComponent();

            AppName = "Ranker";

            panelRelations = new Dictionary<TextBox, Panel>();
            pictureBoxRelations = new Dictionary<PictureBox, Panel>();
            pictureBoxRelationsV = new Dictionary<Panel, PictureBox>();
            countryBoxRelations = new Dictionary<TextBox, PictureBox>();
            teamPointRelations = new Dictionary<TextBox, TextBox>();
            teamLastPositionChange = new Dictionary<string, Change>();
            teamLastCountry = new Dictionary<string, string>();
            countryAbbreviations = new Dictionary<string, string>();
            oldRanks = new Dictionary<Panel, int>();

            panels = new List<Panel>();
            locationPanels = new List<Panel>();
            teamNameBoxes = new List<TextBox>();
            pointBoxes = new List<TextBox>();
            countryBoxes = new List<TextBox>();
            pictureBoxes = new List<PictureBox>();

            arrowUpImage = Resources.arrow_up; // Change arrow up image here
            arrowDownImage = Resources.arrow_down; // Change arrow down image here
            minusSymbolImage = Resources.minus_symbol; // Change minus symbol image here
            newEntryImage = Resources.new_entry_image; // Change new entry image here

            configPath = AppDomain.CurrentDomain.BaseDirectory + "Config.json";
            settingsPath = AppDomain.CurrentDomain.BaseDirectory + "Settings.json";
            abbreviationsPath = AppDomain.CurrentDomain.BaseDirectory + "CountryAbbreviations.txt";

            InitPanels();
            InitLocationPanels();
            InitTeamNameBoxes();
            InitPointBoxes();
            InitCountryBoxes();
            InitPictureBoxes();

            CreatePanelRelation(panels, pointBoxes);
            CreatePictureBoxRelation(pictureBoxes, panels);
            CreatePictureBoxRelationV(panels, pictureBoxes);
            CreateCountryBoxRelation(countryBoxes, pictureBoxes);
            CreatePointBoxRelation(teamNameBoxes, pointBoxes);

            DefaultPanelLocationX = panel1.Location.X;

            foreach (TextBox teamNameBox in teamNameBoxes)
            {
                teamNameBox.KeyDown += new KeyEventHandler(TeamNameBox_KeyDown);
            }
            foreach (TextBox pointsBox in pointBoxes)
            {
                pointsBox.KeyPress += new KeyPressEventHandler(PointBox_KeyPress);
            }
            foreach (TextBox countryBox in countryBoxes)
            {
                countryBox.KeyPress += new KeyPressEventHandler(CountryTextBoxSubmit);
                countryBox.Leave += new EventHandler(CountryBox_Leave);
                countryBox.Visible = false;
            }
            foreach (PictureBox pictureBox in pictureBoxes)
            {
                pictureBox.Click += new EventHandler(PictureBox_Click);
            }

            if (!GetSettings(settingsPath))
            {
                // Error
            }
            LoadData(configPath, teamNameBoxes, pointBoxes); // Load config

            countryAbbreviations = GetCountryAbbreviations(abbreviationsPath);
            isCurrentSession = false;
        }

        #region Initialize Objects
        private void InitPanels()
        {
            for (int i = 1; i < 1000; i++)
            {
                try
                {
                    Panel panel = (Panel)this.Controls.Find("panel" + i.ToString(), true)[0];

                    panels.Add(panel);
                }
                catch
                {
                    break;
                }
            }
        }

        private void InitLocationPanels()
        {
            for (int i = 1; i < 1000; i++)
            {
                try
                {
                    Panel panel = (Panel)this.Controls.Find("locationPanel" + i.ToString(), true)[0];

                    locationPanels.Add(panel);
                }
                catch
                {
                    break;
                }
            }
        }

        private void InitTeamNameBoxes()
        {
            for (int i = 1; i < 1000; i+=2)
            {
                try
                {
                    TextBox textBox = (TextBox)this.Controls.Find("textBox" + i.ToString(), true)[0];

                    teamNameBoxes.Add(textBox);
                }
                catch
                {
                    break;
                }
            }
        }

        private void InitPointBoxes()
        {
            for (int i = 2; i < 1000; i += 2)
            {
                try
                {
                    TextBox textBox = (TextBox)this.Controls.Find("textBox" + i.ToString(), true)[0];

                    pointBoxes.Add(textBox);
                }
                catch
                {
                    break;
                }
            }
        }

        private void InitCountryBoxes()
        {
            for (int i = 1; i < 1000; i++)
            {
                try
                {
                    TextBox textBox = (TextBox)this.Controls.Find("countryBox" + i.ToString(), true)[0];

                    countryBoxes.Add(textBox);
                }
                catch
                {
                    break;
                }
            }
        }

        private void InitPictureBoxes()
        {
            for (int i = 1; i < 1000; i++)
            {
                try
                {
                    PictureBox pictureBox = (PictureBox)this.Controls.Find("pictureBox" + i.ToString(), true)[0];

                    pictureBoxes.Add(pictureBox);
                }
                catch
                {
                    break;
                }
            }
        }
        #endregion

        #region Rule Methods
        private void TryToUpdate()
        {
            if (!IsThereDuplicates()) // Do nothing if there's duplicates
            {
                OrderCalculator(pointBoxes);
                SetSettings(settingsPath, false);
            }
            else
            {
                MessageBox.Show("Duplicates detected!", AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            TryToUpdate();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                TryToUpdate();
            }
        }

        private void changeConfigPathButton_Click(object sender, EventArgs e)
        {
            if (!SetSettings(settingsPath, true))
            {
                // Error
            }
        }

        private void TeamNameBox_KeyDown(object sender, KeyEventArgs e) // Disable line breaks
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                TryToUpdate();

                e.SuppressKeyPress = true;
            }
        }


        private void PointBox_KeyPress(object sender, KeyPressEventArgs e) // Allow only numbers to points boxes
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                TryToUpdate();

                e.Handled = true;
            }

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void PictureBox_Click(object sender, EventArgs e)
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

        private void CountryTextBoxSubmit(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (e.KeyChar == (char)Keys.Enter)
            {
                PictureBox pictureBox = countryBoxRelations[textBox];

                pictureBox.ImageLocation = TryGetImageLocationWithSearchQuery(textBox.Text, ".png");
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

        private void CountryBox_Leave(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            textBox.Visible = false;
        }

        private string TryGetImageLocationWithSearchQuery(string searchQuery, string imageFormat)
        {
            string fullQuery = string.Empty;

            if (searchQuery.Length == 2)
            {
                try
                {
                    fullQuery = countryAbbreviations[searchQuery.ToLower()] + imageFormat;
                }
                catch
                {
                }
            }
            else
            {
                fullQuery = searchQuery + imageFormat;
            }

            return Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, "Resources", fullQuery);
        }

        private bool IsThereDuplicates() // Check for team name and point duplicates
        {
            foreach (TextBox pointBox in pointBoxes)
            {
                foreach (TextBox otherPointBox in pointBoxes)
                {
                    if (pointBox != otherPointBox)
                    {
                        if (int.Parse(pointBox.Text) == int.Parse(otherPointBox.Text))
                        {
                            return true;
                        }
                    }
                }
            }
            foreach (TextBox teamNameBox in teamNameBoxes)
            {
                foreach (TextBox otherTeamNameBox in teamNameBoxes)
                {
                    if (teamNameBox != otherTeamNameBox)
                    {
                        if (teamNameBox.Text == otherTeamNameBox.Text)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private Dictionary<string, string> GetCountryAbbreviations(string filePath)
        {
            Dictionary<string, string> countryAbbreviations = new Dictionary<string, string>();

            if (File.Exists(filePath))
            {
                string[] fullAbbreviations = File.ReadAllLines(filePath);

                for (int i = 0; i < fullAbbreviations.Length; i++)
                {
                    countryAbbreviations.Add(fullAbbreviations[i].ToLower(), fullAbbreviations[i += 1]);
                }
            }

            return countryAbbreviations;
        }
        #endregion

        #region Calculating Methods
        private void OrderCalculator(List<TextBox> pointBoxes)
        {
            List<int> biggestOrder = new List<int>();
            Dictionary<int, TextBox> orderByPoints = new Dictionary<int, TextBox>();

            foreach (TextBox points in pointBoxes)
            {
                biggestOrder.Add(int.Parse(points.Text));
            }

            biggestOrder.Sort((a, b) => b.CompareTo(a));

            orderByPoints = OrderByPoints(biggestOrder, pointBoxes); // Correct (ascending) order of the point boxes sorted by the highest points

            for (int i = 1; i < orderByPoints.Count + 1; i++)
            {
                PlacePanelWithLocationPanels(i, panelRelations[orderByPoints[i]]);
            }

            CalculateDefaultPoints(pointBoxes, 10000, 100);

            SaveDataAndChangePositionChangeImages(configPath, orderByPoints);

            isCurrentSession = true;
        }

        private void ChangePositionChangeImageByPositionChange(bool areSameRanks)
        {
            if (structures != null && structures.Count > 0)
            {
                foreach (Structure currentStructure in structures)
                {
                    if (!areSameRanks) // If not same ranks as before, update change picture box images
                    {
                        PictureBox changePictureBox = GetPictureBoxByRank(currentStructure.Rank);

                        int teamPreviousRank = LoadPreviousRank(currentStructure.Team);

                        if (teamPreviousRank == -1) // If team is a new entry
                        {
                            changePictureBox.Image = newEntryImage;
                            currentStructure.Change = Change.New;

                            if (!teamLastPositionChange.ContainsKey(currentStructure.Team))
                            {
                                teamLastPositionChange.Add(currentStructure.Team, Change.New);
                            }
                            else
                            {
                                teamLastPositionChange[currentStructure.Team] = Change.New;
                            }

                            continue;
                        }

                        if (currentStructure.Rank < teamPreviousRank) // If team is placed higher
                        {
                            changePictureBox.Image = arrowUpImage;
                            currentStructure.Change = Change.Up;

                            if (!teamLastPositionChange.ContainsKey(currentStructure.Team))
                            {
                                teamLastPositionChange.Add(currentStructure.Team, Change.Up);
                            }
                            else
                            {
                                teamLastPositionChange[currentStructure.Team] = Change.Up;
                            }
                        }
                        else if (currentStructure.Rank > teamPreviousRank) // If team is placed lower
                        {
                            changePictureBox.Image = arrowDownImage;
                            currentStructure.Change = Change.Down;

                            if (!teamLastPositionChange.ContainsKey(currentStructure.Team))
                            {
                                teamLastPositionChange.Add(currentStructure.Team, Change.Down);
                            }
                            else
                            {
                                teamLastPositionChange[currentStructure.Team] = Change.Down;
                            }
                        }
                        else // If team has same place
                        {
                            if (!isCurrentSession) // Disable imaging with minus symbol if already imaged once in the same session
                            {
                                changePictureBox.Image = minusSymbolImage;
                                currentStructure.Change = Change.None;

                                if (!teamLastPositionChange.ContainsKey(currentStructure.Team))
                                {
                                    teamLastPositionChange.Add(currentStructure.Team, Change.None);
                                }
                                else
                                {
                                    teamLastPositionChange[currentStructure.Team] = Change.None;
                                }
                            }
                            else
                            {
                                currentStructure.Change = teamLastPositionChange[currentStructure.Team];
                            }
                        }
                    }
                    else // If same ranks as before, don't update change picture box images, and update config with previous change data (or if new team, with Change.New)
                    {
                        if (teamLastPositionChange.ContainsKey(currentStructure.Team)) // If not a new team
                        {
                            currentStructure.Change = teamLastPositionChange[currentStructure.Team];
                        }
                        else
                        {
                            currentStructure.Change = Change.New;
                            teamLastPositionChange.Add(currentStructure.Team, Change.New);
                        }
                    }
                }
            }
        }

        private int LoadPreviousRank(string teamName)
        {
            List<Structure> structures = new List<Structure>();

            structures = JsonConvert.DeserializeObject<List<Structure>>(File.ReadAllText(configPath));

            foreach (Structure structure in structures)
            {
                if (structure.Team == teamName)
                {
                    return structure.Rank;
                }
            }

            return -1;
        }

        private void CalculateDefaultPoints(List<TextBox> pointBoxes, int maxPoints, int gapDifference)
        {
            List<int> biggestOrder = new List<int>();

            Dictionary<int, TextBox> orderByPoints = new Dictionary<int, TextBox>();

            foreach (TextBox points in pointBoxes)
            {
                biggestOrder.Add(int.Parse(points.Text));
            }

            biggestOrder.Sort((a, b) => b.CompareTo(a));

            orderByPoints = OrderByPoints(biggestOrder, pointBoxes); // Correct (ascending) order of the point boxes sorted by the highest points

            int pts = maxPoints + gapDifference;
            for (int i = 1; i < orderByPoints.Count + 1; i++)
            {
                pts -= gapDifference;
                orderByPoints[i].Text = pts.ToString();
            }
        }
        #endregion

        #region Relation Creating Methods
        private void CreatePanelRelation(List<Panel> panels, List<TextBox> pointBoxes)
        {
            if (panels.Count == pointBoxes.Count)
            {
                for (int i = 0; i < panels.Count; i++)
                {
                    panelRelations.Add(pointBoxes[i], panels[i]);
                }
            }
            else
            {
                throw new Exception("'panels' and 'pointBoxes' lists are not the same size!");
            }
        }

        private void CreatePictureBoxRelation(List<PictureBox> pictureBoxes, List<Panel> panels)
        {
            if (pictureBoxes.Count == panels.Count)
            {
                for (int i = 0; i < pictureBoxes.Count; i++)
                {
                    pictureBoxRelations.Add(pictureBoxes[i], panels[i]);
                }
            }
            else
            {
                throw new Exception("'pictureBoxes' and 'panels' lists are not the same size!");
            }
        }

        private void CreatePictureBoxRelationV(List<Panel> panels, List<PictureBox> pictureBoxes)
        {
            if (panels.Count == pictureBoxes.Count)
            {
                for (int i = 0; i < panels.Count; i++)
                {
                    pictureBoxRelationsV.Add(panels[i], pictureBoxes[i]);
                }
            }
            else
            {
                throw new Exception("'panels' and 'pictureBoxes' lists are not the same size!");
            }
        }

        private void CreateCountryBoxRelation(List<TextBox> countryBoxes, List<PictureBox> pictureBoxes)
        {
            if (countryBoxes.Count == pictureBoxes.Count)
            {
                for (int i = 0; i < countryBoxes.Count; i++)
                {
                    countryBoxRelations.Add(countryBoxes[i], pictureBoxes[i]);
                }
            }
            else
            {
                throw new Exception("'countryBoxes' and 'pictureBoxes' lists are not the same size!");
            }
        }

        private void CreatePointBoxRelation(List<TextBox> teamNameBoxes, List<TextBox> pointBoxes)
        {
            if (teamNameBoxes.Count == pointBoxes.Count)
            {
                for (int i = 0; i < teamNameBoxes.Count; i++)
                {
                    teamPointRelations.Add(teamNameBoxes[i], pointBoxes[i]);
                }
            }
            else
            {
                throw new Exception("'teamNameBoxes' and 'pointBoxes' lists are not the same size!");
            }
        }
        #endregion

        #region Ordering And Placing Methods
        private Dictionary<int, TextBox> OrderByPoints(List<int> biggestOrder, List<TextBox> pointBoxes)
        {
            Dictionary<int, TextBox> orderByPoints = new Dictionary<int, TextBox>();

            for (int i = 0; i < biggestOrder.Count; i++) // Always starts from the biggest points
            {
                foreach (TextBox points in pointBoxes)
                {
                    if (biggestOrder[i] == int.Parse(points.Text)) // Match the points with the point boxes (places indexes ascending from '1', as it should ALWAYS match with some points box)
                    {
                        orderByPoints.Add(i + 1, points); // Add the matched ones to the dictionary
                    }
                }
            }

            return orderByPoints;
        }

        private void PlacePanelWithLocationPanels(int placement, Panel panel)
        {
            panel.Location = new Point(DefaultPanelLocationX, locationPanels[placement - 1].Location.Y);
        }

        private PictureBox GetPictureBoxByRank(int rank)
        {
            Control[] _changePictureBox = this.Controls.Find("changePicture" + rank.ToString(), true);
            return (PictureBox)_changePictureBox[0];
        }

        private void ChangeChangePictureBoxImageByChangeValue(PictureBox pictureBox, Change changeValue)
        {
            switch (changeValue)
            {
                case Change.Up:
                    pictureBox.Image = arrowUpImage;
                    break;
                case Change.Down:
                    pictureBox.Image = arrowDownImage;
                    break;
                case Change.None:
                    pictureBox.Image = minusSymbolImage;
                    break;
                case Change.New:
                    pictureBox.Image = newEntryImage;
                    break;
            }
        }
        #endregion

        #region Saving And Loading Methods
        private void SaveDataAndChangePositionChangeImages(string configPath, Dictionary<int, TextBox> orderByPoints)
        {
            structures = new List<Structure>();

            int sameRanksAmount = 0, c = 1;
            foreach (Panel panel in panels)
            {
                /*
                 * Team Name TextBoxes (c): 1, 3, 5...
                 * Points TextBoxes (c): 2, 4, 6...
                 * Country Flag Picture Boxes (b): 1, 2, 3...
                 */

                Control[] _teamNameTextBox = panel.Controls.Find("textBox" + c.ToString(), true),
                _pointsTextBox = panel.Controls.Find("textBox" + (c + 1).ToString(), true);

                TextBox teamNameTextBox = (TextBox)_teamNameTextBox[0], pointsTextBox = (TextBox)_pointsTextBox[0];

                bool isSameRank = false; // If same rank as before, add it to the sameRanksAmount count (if as many as teams overall, DON'T update change picture box images)

                Structure structure = new Structure();
                for (int i = 1; i < orderByPoints.Count + 1; i++)
                {
                    if (orderByPoints[i] == pointsTextBox)
                    {
                        structure.Rank = i; // 1

                        break;
                    }
                }

                if (oldRanks != null && oldRanks.Count > 0)
                {
                    if (oldRanks.ContainsKey(panel))
                    {
                        if (structure.Rank != oldRanks[panel])
                        {
                            isSameRank = false;

                            oldRanks[panel] = structure.Rank;
                        }
                        else
                        {
                            isSameRank = true;
                        }
                    }
                    else
                    {
                        oldRanks.Add(panel, structure.Rank);
                    }
                }

                structure.Team = teamNameTextBox.Text; // 2
                if ((string)pictureBoxRelationsV[panelRelations[pointsTextBox]].Tag != null) // 3
                {
                    structure.Country = (string)pictureBoxRelationsV[panelRelations[pointsTextBox]].Tag;
                }
                else
                {
                    try
                    {
                        structure.Country = teamLastCountry[structure.Team];
                    }
                    catch
                    {
                        structure.Country = null;
                    }
                }
                structure.Points = int.Parse(pointsTextBox.Text); // 4
                structure.Panel = panel.Name; // 5

                structures.Add(structure);

                if (isSameRank)
                {
                    sameRanksAmount += 1;
                }

                c += 2;
            }

            // If SOME of the teams DOESN'T have the same rank as before, ONLY THEN update the change picture box images
            if (sameRanksAmount != pointBoxes.Count) // 6 (also updates the structure.Change to the structures list)
            {
                ChangePositionChangeImageByPositionChange(false);
            }
            else
            {
                ChangePositionChangeImageByPositionChange(true);
            }

            // ---------------------------------------------------------------------------------------------
            // NOTE: EVERY STRUCTURE VARIABLE MUST BE OVERWRITTEN, OR THEY ARE DECLARED WITH DEFAULT VALUES!
            //
            // public int Rank { get; set; } // 1
            // public string Team { get; set; } // 2
            // public string Country { get; set; } // 3
            // public int Points { get; set; } // 4
            // public Change Change { get; set; } // 6
            // public string Panel { get; set; } // 5
            // ---------------------------------------------------------------------------------------------

            File.WriteAllText(configPath, JsonConvert.SerializeObject(structures, Formatting.Indented));
        }

        private void LoadData(string configPath, List<TextBox> teamNameBoxes, List<TextBox> pointBoxes)
        {
            if (File.Exists(configPath))
            {
                if (File.ReadAllText(configPath).Length > 0)
                {
                    UpdateStructuresAndPlaceThem(configPath, teamNameBoxes, pointBoxes);
                }
            }
        }

        private void UpdateStructuresAndPlaceThem(string configPath, List<TextBox> teamNameBoxes, List<TextBox> pointBoxes)
        {
            List<Structure> structures = new List<Structure>();

            structures = JsonConvert.DeserializeObject<List<Structure>>(File.ReadAllText(configPath));

            foreach (Structure structure in structures)
            {
                Panel teamPanel = (Panel)this.Controls.Find(structure.Panel, true)[0];

                Control[] _teamNameTextBox, _teamPointsTextBox;
                TextBox teamNameTextBox, teamPointsTextBox;

                foreach (TextBox teamNameBox in teamNameBoxes) // Search for the matching team name box from the teamNameBoxes list
                {
                    _teamNameTextBox = teamPanel.Controls.Find(teamNameBox.Name, true);

                    if (_teamNameTextBox.Length > 0)
                    {
                        teamNameTextBox = (TextBox)_teamNameTextBox[0];

                        foreach (TextBox nameBox in teamNameBoxes) // Update team name box values with the loaded values
                        {
                            if (nameBox == teamNameTextBox)
                            {
                                nameBox.Text = structure.Team;
                            }

                        }

                        break;
                    }
                }
                foreach (TextBox teamPointsBox in pointBoxes) // Search for the matching points box from the pointBoxes list
                {
                    _teamPointsTextBox = teamPanel.Controls.Find(teamPointsBox.Name, true);

                    if (_teamPointsTextBox.Length > 0)
                    {
                        teamPointsTextBox = (TextBox)_teamPointsTextBox[0];

                        foreach (TextBox pointsBox in pointBoxes) // Update point box values with the loaded values
                        {
                            if (pointsBox == teamPointsTextBox)
                            {
                                pointsBox.Text = structure.Points.ToString();
                            }

                        }

                        break;
                    }
                }

                PlacePanelWithLocationPanels(structure.Rank, teamPanel);

                pictureBoxRelationsV[teamPanel].ImageLocation = TryGetImageLocationWithSearchQuery(structure.Country, ".png"); // Change the picture based on country name
                pictureBoxRelationsV[teamPanel].Tag = structure.Country;

                if (!teamLastCountry.ContainsKey(structure.Team))
                {
                    teamLastCountry.Add(structure.Team, structure.Country);
                }
                else
                {
                    teamLastCountry[structure.Team] = structure.Country;
                }

                if (!oldRanks.ContainsKey(teamPanel))
                {
                    oldRanks.Add(teamPanel, structure.Rank);
                }
                else
                {
                    oldRanks[teamPanel] = structure.Rank;
                }

                teamLastPositionChange.Add(structure.Team, structure.Change);

                ChangeChangePictureBoxImageByChangeValue(GetPictureBoxByRank(structure.Rank), structure.Change);
            }
        }

        private bool SetSettings(string settingsPath, bool changingPath)
        {
            if (File.Exists(settingsPath))
            {
                if (File.ReadAllText(settingsPath).Length > 0)
                {
                    Settings settings = new Settings();
                    settings.Date = DateTime.Now.ToString("dd.MM.yyyy (HH:mm)");

                    if (changingPath)
                    {
                        OpenFileDialog openFileDialog = new OpenFileDialog();

                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            this.configPath = openFileDialog.FileName;
                        }
                    }

                    settings.ConfigPath = this.configPath;

                    File.WriteAllText(settingsPath, JsonConvert.SerializeObject(settings, Formatting.Indented));

                    updatedLabel.Text = "Updated: " + settings.Date;

                    return true;
                }
            }
            else
            {
                // Create settings file?
            }

            return false;
        }

        private bool GetSettings(string settingsPath)
        {
            if (File.Exists(settingsPath))
            {
                if (File.ReadAllText(settingsPath).Length > 0)
                {
                    Settings settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(settingsPath));

                    if (!string.IsNullOrEmpty(settings.ConfigPath) && !string.IsNullOrEmpty(settings.Date))
                    {
                        this.configPath = settings.ConfigPath;
                        updatedLabel.Text = "Updated: " + settings.Date;

                        return true;
                    }
                }
            }

            updatedLabel.Text = "Updated: 01.01.1990";

            return false;
        }
        #endregion
    }

    public enum Change { Up, Down, None, New }

    class Structure
    {
        public int Rank { get; set; }
        public string Team { get; set; }
        public string Country { get; set; }
        public int Points { get; set; }
        public Change Change { get; set; }
        public string Panel { get; set; }
    }

    class Settings
    {
        public string Date { get; set; }
        public string ConfigPath { get; set; }
    }
}
