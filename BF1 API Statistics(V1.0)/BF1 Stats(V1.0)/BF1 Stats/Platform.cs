using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF1_Stats
{
    public class Platform
    {
        int id;
        string platform;
        public Platform(int identifier)
        {
            id = identifier;

            switch(id)
            {
                case 1:
                    platform = "xbox";
                        break;
                case 2:
                    platform = "psn";
                        break;
                case 3:
                    platform = "pc";
                    break;
            }
        }
        public int GetPlatformID()
        {
            return id;
        }
        public string GetPlatformString()
        {
            return platform;
        }
    }
}
