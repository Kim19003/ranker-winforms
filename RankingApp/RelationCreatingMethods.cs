using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RankingApp
{
    class RelationCreatingMethods
    {
        public static void CreateTeamPanelAndTeamPointTextBoxRelations(List<Panel> teamPanels, List<TextBox> teamPointTextBoxes,
            Dictionary<TextBox, Panel> teamPointTextBoxAndTeamPanelRelations)
        {
            if (teamPanels.Count == teamPointTextBoxes.Count)
            {
                for (int i = 0; i < teamPanels.Count; i++)
                {
                    teamPointTextBoxAndTeamPanelRelations.Add(teamPointTextBoxes[i], teamPanels[i]);
                }
            }
            else
            {
                throw new Exception("'teamPanels' and 'teamPointTextBoxes' lists are not the same size!");
            }
        }

        public static void CreateTeamPictureBoxAndTeamPanelRelations(List<PictureBox> teamPictureBoxes, List<Panel> teamPanels,
            Dictionary<PictureBox, Panel> teamPictureBoxAndTeamPanelRelations)
        {
            if (teamPictureBoxes.Count == teamPanels.Count)
            {
                for (int i = 0; i < teamPictureBoxes.Count; i++)
                {
                    teamPictureBoxAndTeamPanelRelations.Add(teamPictureBoxes[i], teamPanels[i]);
                }
            }
            else
            {
                throw new Exception("'teamPictureBoxes' and 'teamPanels' lists are not the same size!");
            }
        }

        public static void CreateTeamCountryTextBoxAndTeamPictureBoxRelations(List<TextBox> teamCountryTextBoxes, List<PictureBox> teamPictureBoxes,
            Dictionary<TextBox, PictureBox> teamCountryTextBoxAndTeamPictureBoxRelations)
        {
            if (teamCountryTextBoxes.Count == teamPictureBoxes.Count)
            {
                for (int i = 0; i < teamCountryTextBoxes.Count; i++)
                {
                    teamCountryTextBoxAndTeamPictureBoxRelations.Add(teamCountryTextBoxes[i], teamPictureBoxes[i]);
                }
            }
            else
            {
                throw new Exception("'teamCountryTextBoxes' and 'teamPictureBoxes' lists are not the same size!");
            }
        }

        public static void CreateTeamNameTextBoxAndTeamPointTextBoxRelations(List<TextBox> teamNameTextBoxes, List<TextBox> teamPointTextBoxes,
            Dictionary<TextBox, TextBox> teamNameTextBoxAndPointBoxRelations)
        {
            if (teamNameTextBoxes.Count == teamPointTextBoxes.Count)
            {
                for (int i = 0; i < teamNameTextBoxes.Count; i++)
                {
                    teamNameTextBoxAndPointBoxRelations.Add(teamNameTextBoxes[i], teamPointTextBoxes[i]);
                }
            }
            else
            {
                throw new Exception("'teamNameTextBoxes' and 'teamPointTextBoxes' lists are not the same size!");
            }
        }
    }
}
