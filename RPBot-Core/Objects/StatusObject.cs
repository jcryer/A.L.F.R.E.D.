using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RPBot
{
    class StatusObject
    {
        public class RootObject
        {
            public RootObject(ulong userid)
            {
                this.UserID = userid;
                this.StatusData = new List<StatusData>();
            }

            public void AddStatus(DateTime datetime, UserStatus status)
            {
                var last = StatusData.LastOrDefault();
                if (last != null)
                {
                    if (last.Status != status) StatusData.Add(new StatusData(datetime, status));
                }
                else
                {
                    StatusData.Add(new StatusData(datetime, status));
                }
            }

            public ulong UserID;
            public List<StatusData> StatusData;
        }
        public class StatusData
        {
            public StatusData(DateTime datetime, UserStatus status)
            {
                this.Datetime = datetime;
                this.Status = status;
            }

            public DateTime Datetime { get; set; }
            public UserStatus Status { get; set; }
        }
    }

   
}
