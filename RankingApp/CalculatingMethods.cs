using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace RankingApp
{
    class CalculatingMethods
    {
        public static void OrderCalculator(List<TextBox> pointBoxes, Dictionary<TextBox, Panel> panelRelations, List<Panel> locationPanels, int DefaultPanelLocationX, string configPath, ref bool isCurrentSession,
            ref List<Structure> structures, List<Structure> loadedStructures, List<Panel> panels, Dictionary<Panel, PictureBox> pictureBoxRelationsV, Dictionary<string, string> teamLastCountry,
            Control.ControlCollection Controls)
        {
            List<int> biggestOrder = new List<int>();
            Dictionary<int, TextBox> orderByPoints = new Dictionary<int, TextBox>();

            foreach (TextBox points in pointBoxes)
            {
                biggestOrder.Add(int.Parse(points.Text));
            }

            biggestOrder.Sort((a, b) => b.CompareTo(a));

            orderByPoints = OrderingAndPlacingMethods.OrderByPoints(biggestOrder, pointBoxes); // Correct (ascending) order of the point boxes sorted by the highest points

            for (int i = 1; i < orderByPoints.Count + 1; i++)
            {
                OrderingAndPlacingMethods.PlacePanelWithLocationPanels(i, panelRelations[orderByPoints[i]], locationPanels, DefaultPanelLocationX);
            }

            CalculateDefaultPoints(pointBoxes, 10000, 100);

            SavingAndLoadingMethods.SaveDataAndChangePositionChangeImages(ref structures, loadedStructures, panels, pictureBoxRelationsV, panelRelations, teamLastCountry, pointBoxes,
            configPath, orderByPoints, Controls, isCurrentSession);

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

        public static void CalculateDefaultPoints(List<TextBox> pointBoxes, int maxPoints, int gapDifference)
        {
            List<int> biggestOrder = new List<int>();

            Dictionary<int, TextBox> orderByPoints = new Dictionary<int, TextBox>();

            foreach (TextBox points in pointBoxes)
            {
                biggestOrder.Add(int.Parse(points.Text));
            }

            biggestOrder.Sort((a, b) => b.CompareTo(a));

            orderByPoints = OrderingAndPlacingMethods.OrderByPoints(biggestOrder, pointBoxes); // Correct (ascending) order of the point boxes sorted by the highest points

            int pts = maxPoints + gapDifference;
            for (int i = 1; i < orderByPoints.Count + 1; i++)
            {
                pts -= gapDifference;
                orderByPoints[i].Text = pts.ToString();
            }
        }
    }
}
