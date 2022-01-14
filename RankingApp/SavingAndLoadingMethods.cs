using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace RankingApp
{
    class SavingAndLoadingMethods
    {
        public static void SaveDataAndChangePositionChangeImages(ref List<Structure> structures, List<Structure> loadedStructures, List<Panel> panels, Dictionary<Panel, PictureBox> pictureBoxRelationsV, Dictionary<TextBox, Panel> panelRelations, Dictionary<string, string> teamLastCountry, List<TextBox> pointBoxes,
            string configPath, Dictionary<int, TextBox> orderByPoints, Control.ControlCollection Controls, bool isCurrentSession)
        {
            structures.Clear(); // ALWAYS OVERWRITE THE STRUCTURES LIST

            int c = 1;
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

                Structure structure = new Structure();
                for (int i = 1; i < orderByPoints.Count + 1; i++)
                {
                    if (orderByPoints[i] == pointsTextBox)
                    {
                        structure.Rank = i; // 1

                        break;
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

                c += 2;
            }

            // ---------------------------------------------------------------------------------------------
            // NOTE: EVERY STRUCTURE VARIABLE MUST BE OVERWRITTEN, OR THEY ARE DECLARED WITH DEFAULT VALUES!
            //
            // public int Rank { get; set; } // 1
            // public string Team { get; set; } // 2
            // public string Country { get; set; } // 3
            // public int Points { get; set; } // 4
            // public string Panel { get; set; } // 5
            // ---------------------------------------------------------------------------------------------
        }

        public static void SaveToConfig(string configPath, List<Structure> structures)
        {
            try
            {
                if (structures.Count > 0)
                {
                    File.WriteAllText(configPath, JsonConvert.SerializeObject(structures, Formatting.Indented));
                }
                else
                {
                    // Do nothing, since there's no changes
                }
            }
            catch
            {
            }
        }

        public static void LoadData(Control.ControlCollection Controls, ref List<Structure> loadedStructures, Dictionary<Panel, PictureBox> pictureBoxRelationsV, Dictionary<string, string> teamLastCountry, string configPath,
            List<TextBox> teamNameBoxes, List<TextBox> pointBoxes, List<Panel> locationPanels, Dictionary<string, string> countryAbbreviations, int DefaultPanelLocationX)
        {
            if (File.Exists(configPath))
            {
                if (File.ReadAllText(configPath).Length > 0)
                {
                    UpdateStructuresAndPlaceThem(Controls, ref loadedStructures, pictureBoxRelationsV, teamLastCountry, configPath, teamNameBoxes, pointBoxes, locationPanels, countryAbbreviations, DefaultPanelLocationX);
                }
            }
        }

        public static void UpdateStructuresAndPlaceThem(Control.ControlCollection Controls, ref List<Structure> loadedStructures, Dictionary<Panel, PictureBox> pictureBoxRelationsV, Dictionary<string, string> teamLastCountry, string configPath,
            List<TextBox> teamNameBoxes, List<TextBox> pointBoxes, List<Panel> locationPanels, Dictionary<string, string> countryAbbreviations, int DefaultPanelLocationX)
        {
            loadedStructures = JsonConvert.DeserializeObject<List<Structure>>(File.ReadAllText(configPath));

            foreach (Structure structure in loadedStructures)
            {
                Panel teamPanel = (Panel)Controls.Find(structure.Panel, true)[0];

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

                OrderingAndPlacingMethods.PlacePanelWithLocationPanels(structure.Rank, teamPanel, locationPanels, DefaultPanelLocationX);

                pictureBoxRelationsV[teamPanel].ImageLocation = RuleMethods.TryGetImageLocationWithSearchQuery(structure.Country, ".png", countryAbbreviations); // Change the picture based on country name
                pictureBoxRelationsV[teamPanel].Tag = structure.Country;

                if (!teamLastCountry.ContainsKey(structure.Team))
                {
                    teamLastCountry.Add(structure.Team, structure.Country);
                }
                else
                {
                    teamLastCountry[structure.Team] = structure.Country;
                }
            }
        }

        public static bool IsThereChanges(string configPath, List<Structure> currentStructures)
        {
            List<Structure> savedStructures = new List<Structure>();
            savedStructures = JsonConvert.DeserializeObject<List<Structure>>(File.ReadAllText(configPath));

            string oldStructureJson = JsonConvert.SerializeObject(savedStructures, Formatting.Indented);
            string currentStructureJson = JsonConvert.SerializeObject(currentStructures, Formatting.Indented);

            if (currentStructureJson != oldStructureJson)
            {
                return true;
            }

            return false;
        }

        public static bool SetSettings(string settingsPath, string configPath, Label updatedLabel, bool changingPath)
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
                            configPath = openFileDialog.FileName;
                        }
                    }

                    settings.ConfigPath = configPath;

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

        public static bool GetSettings(string settingsPath, ref string configPath, Label updatedLabel)
        {
            if (File.Exists(settingsPath))
            {
                if (File.ReadAllText(settingsPath).Length > 0)
                {
                    Settings settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(settingsPath));

                    if (!string.IsNullOrEmpty(settings.ConfigPath) && !string.IsNullOrEmpty(settings.Date))
                    {
                        configPath = settings.ConfigPath;
                        updatedLabel.Text = "Updated: " + settings.Date;

                        return true;
                    }
                }
            }

            updatedLabel.Text = "Updated: 01.01.1990";

            return false;
        }
    }
}
