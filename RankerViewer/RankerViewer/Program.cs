using Newtonsoft.Json;

namespace RankerViewer
{
    internal class Program
    {
        static readonly string localPath = @"C:\RankerViewer\";
        static readonly string localAppSettingsPath = Path.Combine(localPath, "AppSettings.json");

        static void Main(string[] args)
        {
            if (!Directory.Exists(localPath))
            {
                Directory.CreateDirectory(localPath);
            }

            if (!File.Exists(localAppSettingsPath))
            {
                File.WriteAllText(localAppSettingsPath, "{\n\t\n}");
            }

            AppSettings appSettings = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText(localAppSettingsPath)) ?? new();

            List<Structure> structures;
            string siteTemplate;
            string stylesTemplate;
            string structureBlockTemplate;

            try
            {
                structures = JsonConvert.DeserializeObject<List<Structure>>(File.ReadAllText(appSettings.RankingPath)) ?? new();
                siteTemplate = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "SiteTemplate.html"));
                stylesTemplate = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "StylesTemplate.css"));
                structureBlockTemplate = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "StructureTemplate.html"));
            }
            catch (Exception ex)
            {
                PrintErrorMessage($"Error creating the RankerViewer site: {ex.Message}");

                goto End;
            }

            List<string> createdStructureBlocks = new();

            foreach (Structure structure in structures.OrderBy(s => s.Rank))
            {
                string thisStructureBlock = structureBlockTemplate;

                thisStructureBlock = thisStructureBlock.Replace("{TEAM_RANK}", structure.Rank.ToString());

                if (!string.IsNullOrEmpty(structure.Country))
                {
                    thisStructureBlock = thisStructureBlock.Replace("{TEAM_IMAGE}", Path.Combine(appSettings.CountryFlagsPath, $"{structure.Country}.png"));
                }
                else
                {
                    thisStructureBlock = thisStructureBlock.Replace("\"{TEAM_IMAGE}\"", "\"{TEAM_IMAGE}\" style=\"display:none;\"");
                }

                thisStructureBlock = thisStructureBlock.Replace("{TEAM_NAME}", structure.Team);
                thisStructureBlock = thisStructureBlock.Replace("{TEAM_POINTS}", structure.Points.ToString());

                createdStructureBlocks.Add(thisStructureBlock);
            }

            string newSite = siteTemplate.Replace("{UPDATE_TIME}", DateTime.Now.ToString("g"));
            newSite = newSite.Replace("{INSERT_STRUCTURES_HERE}", string.Join(Environment.NewLine, createdStructureBlocks));

            try
            {
                File.WriteAllText(Path.Combine(appSettings.SiteOutputPath, "styles.css"), stylesTemplate);
                File.WriteAllText(Path.Combine(appSettings.SiteOutputPath, "index.html"), newSite);
            }
            catch (Exception ex)
            {
                PrintErrorMessage($"Error creating the RankerViewer site: {ex.Message}");
            }

            End:

            Environment.Exit(0);
        }

        static void PrintErrorMessage(string errorMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(errorMessage);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Read();
        }
    }
}