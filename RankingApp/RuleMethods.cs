using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace RankingApp
{
    class RuleMethods
    {
        public static void TryToUpdate(string AppName, List<TextBox> pointBoxes, List<TextBox> teamNameBoxes, Dictionary<TextBox, Panel> panelRelations, List<Panel> locationPanels, int DefaultPanelLocationX, string configPath, string settingsPath, ref bool isCurrentSession,
            ref List<Structure> structures, List<Structure> loadedStructures, List<Panel> panels, Dictionary<Panel, PictureBox> pictureBoxRelationsV, Dictionary<string, string> teamLastCountry,
            Control.ControlCollection Controls, Label updatedLabel)
        {
            if (!IsThereDuplicates(pointBoxes, teamNameBoxes)) // Do nothing if there's duplicates
            {
                CalculatingMethods.OrderCalculator(pointBoxes, panelRelations, locationPanels, DefaultPanelLocationX, configPath, ref isCurrentSession, ref structures, loadedStructures, panels, 
                   pictureBoxRelationsV, teamLastCountry, Controls);
                SavingAndLoadingMethods.SetSettings(settingsPath, configPath, updatedLabel, false);
            }
            else
            {
                MessageBox.Show("Duplicates detected!", AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public static string TryGetImageLocationWithSearchQuery(string searchQuery, string imageFormat, Dictionary<string, string> countryAbbreviations)
        {
            string fullQuery = string.Empty;

            if (searchQuery.Length == 2 || searchQuery.Length == 3)
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

        public static bool IsThereDuplicates(List<TextBox> pointBoxes, List<TextBox> teamNameBoxes) // Check for team name and point duplicates
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

        public static Dictionary<string, string> GetCountryAbbreviations(string filePath)
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
    }
}
