using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RankingApp
{
    class OrderingAndPlacingMethods
    {
        public static Dictionary<int, TextBox> OrderByPoints(List<int> biggestOrder, List<TextBox> pointBoxes)
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

        public static void PlacePanelWithLocationPanels(int placement, Panel panel, List<Panel> locationPanels, int DefaultPanelLocationX)
        {
            panel.Location = new Point(DefaultPanelLocationX, locationPanels[placement - 1].Location.Y);
        }

        public static PictureBox GetPictureBoxByRank(int rank, Control.ControlCollection Controls)
        {
            Control[] _changePictureBox = Controls.Find("changePicture" + rank.ToString(), true);
            return (PictureBox)_changePictureBox[0];
        }
    }
}
