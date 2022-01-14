using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RankingApp
{
    class InitializeObjects
    {
        public static void InitPanels(Control.ControlCollection Controls, List<Panel> panels)
        {
            for (int i = 1; i < 1000; i++)
            {
                try
                {
                    Panel panel = (Panel)Controls.Find("panel" + i.ToString(), true)[0];

                    panels.Add(panel);
                }
                catch
                {
                    break;
                }
            }
        }

        public static void InitLocationPanels(Control.ControlCollection Controls, List<Panel> locationPanels)
        {
            for (int i = 1; i < 1000; i++)
            {
                try
                {
                    Panel panel = (Panel)Controls.Find("locationPanel" + i.ToString(), true)[0];

                    locationPanels.Add(panel);
                }
                catch
                {
                    break;
                }
            }
        }

        public static void InitTeamNameBoxes(Control.ControlCollection Controls, List<TextBox> teamNameBoxes)
        {
            for (int i = 1; i < 1000; i += 2)
            {
                try
                {
                    TextBox textBox = (TextBox)Controls.Find("textBox" + i.ToString(), true)[0];

                    teamNameBoxes.Add(textBox);
                }
                catch
                {
                    break;
                }
            }
        }

        public static void InitPointBoxes(Control.ControlCollection Controls, List<TextBox> pointBoxes)
        {
            for (int i = 2; i < 1000; i += 2)
            {
                try
                {
                    TextBox textBox = (TextBox)Controls.Find("textBox" + i.ToString(), true)[0];

                    pointBoxes.Add(textBox);
                }
                catch
                {
                    break;
                }
            }
        }

        public static void InitCountryBoxes(Control.ControlCollection Controls, List<TextBox> countryBoxes)
        {
            for (int i = 1; i < 1000; i++)
            {
                try
                {
                    TextBox textBox = (TextBox)Controls.Find("countryBox" + i.ToString(), true)[0];

                    countryBoxes.Add(textBox);
                }
                catch
                {
                    break;
                }
            }
        }

        public static void InitPictureBoxes(Control.ControlCollection Controls, List<PictureBox> pictureBoxes)
        {
            for (int i = 1; i < 1000; i++)
            {
                try
                {
                    PictureBox pictureBox = (PictureBox)Controls.Find("pictureBox" + i.ToString(), true)[0];

                    pictureBoxes.Add(pictureBox);
                }
                catch
                {
                    break;
                }
            }
        }
    }
}
