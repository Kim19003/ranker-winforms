using RankingApp.Models;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace RankingApp
{
    class RuleMethods
    {
        public static void TryToUpdate(string appName, List<TextBox> teamPointTextBoxes, List<TextBox> teamNameTextBoxes,
            Dictionary<PictureBox, Panel> teamPictureBoxAndTeamPanelRelations, Dictionary<TextBox, Panel> teamPointTextBoxAndTeamPanelRelations,
            List<Panel> locationPointerPanels, int defaultPanelLocationX, string configPath, string settingsPath, ref bool isCurrentSession,
            List<Structure> currentStructures, List<Panel> teamPanels, Dictionary<string, string> teamLastCountry, Label updatedLabel)
        {
            if (!IsThereTeamDuplicates(teamPointTextBoxes, teamNameTextBoxes))
            {
                CalculatingMethods.OrderCalculator(teamPointTextBoxes, teamPictureBoxAndTeamPanelRelations, teamPointTextBoxAndTeamPanelRelations, locationPointerPanels,
                    defaultPanelLocationX, ref isCurrentSession, currentStructures, teamPanels, teamLastCountry);

                SavingAndLoadingMethods.SetSettings(settingsPath, configPath, updatedLabel, false);
            }
            else
            {
                MessageBox.Show("Duplicates detected!", appName, MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        public static bool IsThereTeamDuplicates(List<TextBox> teamPointTextBoxes, List<TextBox> teamNameTextBoxes)
        {
            foreach (TextBox pointBox in teamPointTextBoxes)
            {
                foreach (TextBox otherPointBox in teamPointTextBoxes)
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
            foreach (TextBox teamNameBox in teamNameTextBoxes)
            {
                foreach (TextBox otherTeamNameBox in teamNameTextBoxes)
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
