using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF1_Stats
{
    public class InDepthInformation
    {
        
        public Platform platform;
        public int count = 0;
        public int[] killCount = new int[9];
        public int[] deathCount = new int[9];
        public int[] winCount = new int[9];
        public int[] lossCount = new int[9];
        public int[] scoreCount = new int[9];
        public int[] timePlayed = new int[9];
        public int[] gameModeCount = new int[9];
        public int maxGames;
        public string name;
        public int typeID;
        public bool[] modeInclusion = new bool[9];

        public InDepthInformation(int userPlatform, int userMaxGames, string playerName, int idType)
        {
            platform = new Platform(userPlatform);
            maxGames = userMaxGames;
            name = playerName;
            typeID = idType;
        }
    }
}
