using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RankingApp
{
    class OrderingAndPlacingMethods
    {
        public static Dictionary<int, TextBox> OrderByPoints(List<int> biggestOrder, List<TextBox> teamPointTextBoxes)
        {
            Dictionary<int, TextBox> pointsTextBoxesOrderedByPoints = new Dictionary<int, TextBox>();

            for (int i = 0; i < biggestOrder.Count; i++)
            {
                foreach (TextBox points in teamPointTextBoxes)
                {
                    if (biggestOrder[i] == int.Parse(points.Text))
                    {
                        pointsTextBoxesOrderedByPoints.Add(i + 1, points);
                    }
                }
            }

            return pointsTextBoxesOrderedByPoints;
        }

        public static void PlacePanelWithLocationPanels(int placement, Panel panel, List<Panel> locationPointerPanels, int defaultPanelLocationX)
        {
            panel.Location = new Point(defaultPanelLocationX, locationPointerPanels[placement - 1].Location.Y);
        }

        public static PictureBox GetPictureBoxByRank(int rank, Control.ControlCollection Controls)
        {
            Control[] _changePictureBox = Controls.Find("changePicture" + rank.ToString(), true);
            return (PictureBox)_changePictureBox[0];
        }
    }
}
