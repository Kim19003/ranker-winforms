using Newtonsoft.Json;
using RankingApp.Models;
using RankingApp.Other;
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
        public static void SaveDataAndChangePositionChangeImages(List<Structure> currentStructures, List<Panel> teamPanels,
            Dictionary<PictureBox, Panel> teamPictureBoxAndTeamPanelRelations, Dictionary<TextBox, Panel> teamPointTextBoxAndTeamPanelRelations,
            Dictionary<string, string> teamLastCountry, Dictionary<int, TextBox> pointsTextBoxesOrderedByPoints)
        {
            currentStructures.Clear();

            int c = 1;
            foreach (Panel panel in teamPanels)
            {
                Control[] _teamNameTextBox = panel.Controls.Find("textBox" + c.ToString(), true),
                _pointsTextBox = panel.Controls.Find("textBox" + (c + 1).ToString(), true);

                TextBox teamNameTextBox = (TextBox)_teamNameTextBox[0], pointsTextBox = (TextBox)_pointsTextBox[0];

                Structure structure = new Structure();

                for (int i = 1; i < pointsTextBoxesOrderedByPoints.Count + 1; i++)
                {
                    if (pointsTextBoxesOrderedByPoints[i] == pointsTextBox)
                    {
                        structure.Rank = i;

                        break;
                    }
                }

                structure.Team = teamNameTextBox.Text;

                Panel foundPanel = teamPictureBoxAndTeamPanelRelations.First(p => p.Value == teamPointTextBoxAndTeamPanelRelations[pointsTextBox]).Value;

                if ((string)foundPanel.Tag != null)
                {
                    structure.Country = (string)foundPanel.Tag;
                }
                else
                {
                    if (teamLastCountry.ContainsKey(structure.Team))
                    {
                        structure.Country = teamLastCountry[structure.Team];
                    }
                    else
                    {
                        structure.Country = null;
                    }
                }
                structure.Points = int.Parse(pointsTextBox.Text);
                structure.Panel = panel.Name;

                currentStructures.Add(structure);

                c += 2;
            }
        }

        public static void SaveToConfig(string configPath, List<Structure> currentStructures, ref bool hasChanges)
        {
            try
            {
                if (currentStructures.Count > 0)
                {
                    File.WriteAllText(configPath, JsonConvert.SerializeObject(currentStructures, Formatting.Indented));
                    hasChanges = true;
                }
            }
            catch
            {
            }
        }

        public static void LoadData(Control.ControlCollection Controls, List<Structure> loadedStructures, Dictionary<string, string> teamLastCountry,
            string configPath, List<TextBox> teamNameTextBoxes, List<TextBox> teamPointTextBoxes, List<Panel> locationPointerPanels,
            Dictionary<string, string> countryAbbreviations, int defaultPanelLocationX, Dictionary<PictureBox, Panel> teamPictureBoxAndTeamPanelRelations)
        {
            if (File.Exists(configPath))
            {
                if (File.ReadAllText(configPath).Length > 0)
                {
                    UpdateStructuresAndPlaceThem(Controls, loadedStructures, teamLastCountry, configPath, teamNameTextBoxes, teamPointTextBoxes, locationPointerPanels,
                        countryAbbreviations, defaultPanelLocationX, teamPictureBoxAndTeamPanelRelations);
                }
            }
        }

        public static void UpdateStructuresAndPlaceThem(Control.ControlCollection Controls, List<Structure> loadedStructures,
            Dictionary<string, string> teamLastCountry, string configPath, List<TextBox> teamNameTextBoxes, List<TextBox> teamPointTextBoxes,
            List<Panel> locationPointerPanels, Dictionary<string, string> countryAbbreviations, int defaultPanelLocationX,
            Dictionary<PictureBox, Panel> teamPictureBoxAndTeamPanelRelations)
        {
            loadedStructures.Fill(JsonConvert.DeserializeObject<List<Structure>>(File.ReadAllText(configPath)), true);

            foreach (Structure structure in loadedStructures)
            {
                Panel teamPanel = (Panel)Controls.Find(structure.Panel, true)[0];

                Control[] _teamNameTextBox, _teamPointsTextBox;
                TextBox teamNameTextBox, teamPointsTextBox;

                foreach (TextBox teamNameBox in teamNameTextBoxes)
                {
                    _teamNameTextBox = teamPanel.Controls.Find(teamNameBox.Name, true);

                    if (_teamNameTextBox.Length > 0)
                    {
                        teamNameTextBox = (TextBox)_teamNameTextBox[0];

                        foreach (TextBox nameBox in teamNameTextBoxes)
                        {
                            if (nameBox == teamNameTextBox)
                            {
                                nameBox.Text = structure.Team;
                            }

                        }

                        break;
                    }
                }
                foreach (TextBox teamPointsBox in teamPointTextBoxes)
                {
                    _teamPointsTextBox = teamPanel.Controls.Find(teamPointsBox.Name, true);

                    if (_teamPointsTextBox.Length > 0)
                    {
                        teamPointsTextBox = (TextBox)_teamPointsTextBox[0];

                        foreach (TextBox pointsBox in teamPointTextBoxes)
                        {
                            if (pointsBox == teamPointsTextBox)
                            {
                                pointsBox.Text = structure.Points.ToString();
                            }

                        }

                        break;
                    }
                }

                OrderingAndPlacingMethods.PlacePanelWithLocationPanels(structure.Rank, teamPanel, locationPointerPanels, defaultPanelLocationX);

                teamPictureBoxAndTeamPanelRelations.First(p => p.Value == teamPanel).Key.ImageLocation = RuleMethods.TryGetImageLocationWithSearchQuery(structure.Country, ".png", countryAbbreviations);
                teamPictureBoxAndTeamPanelRelations.First(p => p.Value == teamPanel).Key.Tag = structure.Country;

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

        public static bool IsThereUnsavedChanges(string configPath, List<Structure> currentStructures)
        {
            List<Structure> savedStructures = JsonConvert.DeserializeObject<List<Structure>>(File.ReadAllText(configPath));

            string oldStructureJson = JsonConvert.SerializeObject(savedStructures);
            string currentStructureJson = JsonConvert.SerializeObject(currentStructures);

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
                    Settings settings = new Settings
                    {
                        Date = DateTime.Now.ToString("dd.MM.yyyy (HH:mm)")
                    };

                    if (changingPath)
                    {
                        OpenFileDialog openFileDialog = new OpenFileDialog();

                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            configPath = openFileDialog.FileName;
                        }
                    }

                    settings.StructuresPath = configPath;

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

                    if (!string.IsNullOrEmpty(settings.StructuresPath) && !string.IsNullOrEmpty(settings.Date))
                    {
                        configPath = settings.StructuresPath;
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
