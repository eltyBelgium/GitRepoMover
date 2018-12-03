using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GitRepoMover
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = readSettings();
            var gitManager = new GitManager(settings.Username,settings.Password,settings.LocalTempFolder,settings.AllBranches);
            gitManager.GitClone(settings.CurrentUrl);
            gitManager.GitGetAllBranches();
            gitManager.GitMoveRepo(settings.NewUrl);

            Console.ReadLine();
        }


        static Settings readSettings()
        {
            string json = System.IO.File.ReadAllText("./config.json");
            return JsonConvert.DeserializeObject<Settings>(json);
        }
    }
}
