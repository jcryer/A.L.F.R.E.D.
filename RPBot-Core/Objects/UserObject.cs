using DSharpPlus.Entities;
using System;
using System.Collections.Generic;

namespace RPBot
{
    class UserObject
    {
        public class RootObject
        {
            public RootObject(ulong userId)
            {
                UserData = new UserData(userId);
                Stats = new StatData();
                ModData = new ModData();
            }

            public UserData UserData { get; set; }
            public StatData Stats { get; set; }
            public ModData ModData { get; set; }

        }

        public class UserData
        {
            public UserData(ulong userID)
            {
                this.UserID = userID;
                FactionID = 0;
                Money = 0;
                Xp = 0;
            }

            public int Money { get; set; }
            public int Xp { get; set; }
            public ulong UserID { get; set; }
            public int FactionID { get; set; }

            public string GetRank()
            {
                int rank = Xp;
                string UserRank = "Tier 10";
                if (rank < 20000) UserRank = "Tier 9";
                if (rank < 17000) UserRank = "Tier 8";
                if (rank < 14000) UserRank = "Tier 7";
                if (rank < 11000) UserRank = "Tier 6";
                if (rank < 8000) UserRank = "Tier 5";
                if (rank < 6000) UserRank = "Tier 4";
                if (rank < 4000) UserRank = "Tier 3";
                if (rank < 2000) UserRank = "Tier 2";
                if (rank < 1000) UserRank = "Tier 1";
                return UserRank;
            }
        }

        public class StatData
        {
            public int Melee;
            public int Ranged;
            public int Mobility;
            public int Dodge;
            public int Durability;
            public int Utility;
            public int Healing;
            public int Influence;
            public int Potential;

            public StatData()
            {
                Melee = 0;
                Ranged = 0;
                Mobility = 0;
                Dodge = 0;
                Durability = 0;
                Utility = 0;
                Healing = 0;
                Influence = 0;
                Potential = 0;
            }

            public int[] GetList()
            {
                return new int[] { Melee, Ranged, Mobility, Dodge, Durability, Utility, Healing, Influence, Potential };
            }

            public void SetList(int[] stats)
            {
                Melee = stats[0];
                Ranged = stats[1];
                Mobility = stats[2];
                Dodge = stats[3];
                Durability = stats[4];
                Utility = stats[5];
                Healing = stats[6];
                Influence = stats[7];
                Potential = stats[8];
            }
        }

        public class InvData
        {
            public InvData()
            {
                Items = new List<int>();
            }

            public List<int> Items { get; set; }
        }

        public class ModData
        {
            public ModData()
            {
                this.IsMuted = 0;
            }
            public int IsMuted { get; set; }
            public TimeSpan MuteDuration { get; set; }
            public List<DiscordRole> Roles { get; set; }
        }
    }
}
