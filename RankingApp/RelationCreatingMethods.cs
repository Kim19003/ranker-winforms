using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RankingApp
{
    class RelationCreatingMethods
    {
        public static void CreatePanelRelation(List<Panel> panels, List<TextBox> pointBoxes, Dictionary<TextBox, Panel> panelRelations)
        {
            if (panels.Count == pointBoxes.Count)
            {
                for (int i = 0; i < panels.Count; i++)
                {
                    panelRelations.Add(pointBoxes[i], panels[i]);
                }
            }
            else
            {
                throw new Exception("'panels' and 'pointBoxes' lists are not the same size!");
            }
        }

        public static void CreatePictureBoxRelation(List<PictureBox> pictureBoxes, List<Panel> panels, Dictionary<PictureBox, Panel> pictureBoxRelations)
        {
            if (pictureBoxes.Count == panels.Count)
            {
                for (int i = 0; i < pictureBoxes.Count; i++)
                {
                    pictureBoxRelations.Add(pictureBoxes[i], panels[i]);
                }
            }
            else
            {
                throw new Exception("'pictureBoxes' and 'panels' lists are not the same size!");
            }
        }

        public static void CreatePictureBoxRelationV(List<Panel> panels, List<PictureBox> pictureBoxes, Dictionary<Panel, PictureBox> pictureBoxRelationsV)
        {
            if (panels.Count == pictureBoxes.Count)
            {
                for (int i = 0; i < panels.Count; i++)
                {
                    pictureBoxRelationsV.Add(panels[i], pictureBoxes[i]);
                }
            }
            else
            {
                throw new Exception("'panels' and 'pictureBoxes' lists are not the same size!");
            }
        }

        public static void CreateCountryBoxRelation(List<TextBox> countryBoxes, List<PictureBox> pictureBoxes, Dictionary<TextBox, PictureBox> countryBoxRelations)
        {
            if (countryBoxes.Count == pictureBoxes.Count)
            {
                for (int i = 0; i < countryBoxes.Count; i++)
                {
                    countryBoxRelations.Add(countryBoxes[i], pictureBoxes[i]);
                }
            }
            else
            {
                throw new Exception("'countryBoxes' and 'pictureBoxes' lists are not the same size!");
            }
        }

        public static void CreatePointBoxRelation(List<TextBox> teamNameBoxes, List<TextBox> pointBoxes, Dictionary<TextBox, TextBox> teamPointRelations)
        {
            if (teamNameBoxes.Count == pointBoxes.Count)
            {
                for (int i = 0; i < teamNameBoxes.Count; i++)
                {
                    teamPointRelations.Add(teamNameBoxes[i], pointBoxes[i]);
                }
            }
            else
            {
                throw new Exception("'teamNameBoxes' and 'pointBoxes' lists are not the same size!");
            }
        }
    }
}
