using RankingApp.Models;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RankingApp
{
    class CalculatingMethods
    {
        public static void OrderCalculator(List<TextBox> teamPointTextBoxes, Dictionary<PictureBox, Panel> teamPictureBoxAndTeamPanelRelations, 
            Dictionary<TextBox, Panel> teamPointTextBoxAndTeamPanelRelations, List<Panel> locationPointerPanels, int defaultPanelLocationX,
            ref bool isCurrentSession, List<Structure> currentStructures, List<Panel> teamPanels, Dictionary<string, string> teamLastCountry)
        {
            List<int> biggestOrder = new List<int>();
            Dictionary<int, TextBox> pointsTextBoxesOrderedByPoints = new Dictionary<int, TextBox>();

            foreach (TextBox points in teamPointTextBoxes)
            {
                biggestOrder.Add(int.Parse(points.Text));
            }

            biggestOrder.Sort((a, b) => b.CompareTo(a));

            pointsTextBoxesOrderedByPoints = OrderingAndPlacingMethods.OrderByPoints(biggestOrder, teamPointTextBoxes);

            for (int i = 1; i < pointsTextBoxesOrderedByPoints.Count + 1; i++)
            {
                OrderingAndPlacingMethods.PlacePanelWithLocationPanels(i, teamPointTextBoxAndTeamPanelRelations[pointsTextBoxesOrderedByPoints[i]], locationPointerPanels,
                    defaultPanelLocationX);
            }

            CalculateDefaultPoints(teamPointTextBoxes, 10000, 100);

            SavingAndLoadingMethods.SaveDataAndChangePositionChangeImages(currentStructures, teamPanels, teamPictureBoxAndTeamPanelRelations,
                teamPointTextBoxAndTeamPanelRelations, teamLastCountry, pointsTextBoxesOrderedByPoints);

            isCurrentSession = true;
        }

        public static int LoadPreviousRank(string teamName, List<Structure> loadedStructures)
        {
            foreach (Structure structure in loadedStructures)
            {
                if (structure.Team == teamName)
                {
                    return structure.Rank;
                }
            }

            return -1;
        }

        public static void CalculateDefaultPoints(List<TextBox> teamPointTextBoxes, int maxPoints, int gapDifference)
        {
            List<int> biggestOrder = new List<int>();

            Dictionary<int, TextBox> pointsTextBoxesOrderedByPoints = new Dictionary<int, TextBox>();

            foreach (TextBox points in teamPointTextBoxes)
            {
                biggestOrder.Add(int.Parse(points.Text));
            }

            biggestOrder.Sort((a, b) => b.CompareTo(a));

            pointsTextBoxesOrderedByPoints = OrderingAndPlacingMethods.OrderByPoints(biggestOrder, teamPointTextBoxes);

            int pts = maxPoints + gapDifference;
            for (int i = 1; i < pointsTextBoxesOrderedByPoints.Count + 1; i++)
            {
                pts -= gapDifference;
                pointsTextBoxesOrderedByPoints[i].Text = pts.ToString();
            }
        }
    }
}
