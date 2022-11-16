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
        public static void InitTeamPanels(Control.ControlCollection Controls, List<Panel> teamPanels)
        {
            for (int i = 1; i < 1000; i++)
            {
                try
                {
                    Panel panel = (Panel)Controls.Find("panel" + i.ToString(), true)[0];

                    teamPanels.Add(panel);
                }
                catch
                {
                    break;
                }
            }
        }

        public static void InitLocationPointerPanels(Control.ControlCollection Controls, List<Panel> locationPointerPanels)
        {
            for (int i = 1; i < 1000; i++)
            {
                try
                {
                    Panel panel = (Panel)Controls.Find("locationPanel" + i.ToString(), true)[0];

                    locationPointerPanels.Add(panel);
                }
                catch
                {
                    break;
                }
            }
        }

        public static void InitTeamNameTextBoxes(Control.ControlCollection Controls, List<TextBox> teamNameTextBoxes)
        {
            for (int i = 1; i < 1000; i += 2)
            {
                try
                {
                    TextBox textBox = (TextBox)Controls.Find("textBox" + i.ToString(), true)[0];

                    teamNameTextBoxes.Add(textBox);
                }
                catch
                {
                    break;
                }
            }
        }

        public static void InitTeamPointTextBoxes(Control.ControlCollection Controls, List<TextBox> teamPointTextBoxes)
        {
            for (int i = 2; i < 1000; i += 2)
            {
                try
                {
                    TextBox textBox = (TextBox)Controls.Find("textBox" + i.ToString(), true)[0];

                    teamPointTextBoxes.Add(textBox);
                }
                catch
                {
                    break;
                }
            }
        }

        public static void InitTeamCountryTextBoxes(Control.ControlCollection Controls, List<TextBox> teamCountryTextBoxes)
        {
            for (int i = 1; i < 1000; i++)
            {
                try
                {
                    TextBox textBox = (TextBox)Controls.Find("countryBox" + i.ToString(), true)[0];

                    teamCountryTextBoxes.Add(textBox);
                }
                catch
                {
                    break;
                }
            }
        }

        public static void InitTeamPictureBoxes(Control.ControlCollection Controls, List<PictureBox> teamPictureBoxes)
        {
            for (int i = 1; i < 1000; i++)
            {
                try
                {
                    PictureBox pictureBox = (PictureBox)Controls.Find("pictureBox" + i.ToString(), true)[0];

                    teamPictureBoxes.Add(pictureBox);
                }
                catch
                {
                    break;
                }
            }
        }
    }
}
