using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AspNetMvc.Classes
{
    public class BillingPolice
    {
        float[] costs = new float[] { 0, 5, 10, 15, 19, 23, 27, 30, 33, 36, 38, 40, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100 };

        const string currency = "BYN";
        const string orderPrefix = "LEBYAZHIY_";
        float amountValue = 0;

        int photoCounter = 0;

        public string AmountString { get { return String.Format("{0} {1}", amountValue, currency); } }

        public float AmountValue { get { return amountValue; } }

        public string OrderId { get { if (photoCounter > 0 && amountValue > 0) return GetOrderIdByDate(); else return "NULL"; } }

        public bool IsLegal { get { if (photoCounter > 0 && amountValue > 0) return true; else return false; } }

        public string Description
        {
            get
            {
                if (photoCounter == 0) return "Нет фотографий для приобретения.";
                else if (photoCounter == 1 || photoCounter == 21 || photoCounter == 31 || photoCounter == 41 || photoCounter == 51 || photoCounter == 61 || photoCounter == 71 || photoCounter == 81 || photoCounter == 91 || photoCounter == 101) return String.Format("Покупка {0} фотографии.", photoCounter);
                else return String.Format("Покупка {0} фотографий.", photoCounter);
            }
        }

        public BillingPolice(List<string> photoProcessId)
        {
            try
            {
                if (photoProcessId != null && photoProcessId.Count > 0)
                {
                    photoCounter = photoProcessId.Count;
                    if (photoProcessId.Count > costs.Length + 1) amountValue = (float)Math.Round(costs[costs.Length - 1], 2);
                    else amountValue = (float)Math.Round(costs[photoProcessId.Count]);
                }
            }
            catch (Exception exc) { }
        }

        string GetOrderIdByDate()
        {
            double delta = TimeSpan.FromTicks(DateTime.UtcNow.Ticks - 636710112000000000).TotalSeconds;
            return String.Format("{0}{1}", orderPrefix, (((UInt64)(delta)).ToString("X") + ((byte)(delta % 1 * 16)).ToString("X")).Replace("0", "G").Replace("A", "S").Replace("B", "V").Replace("C", "R").Replace("E", "Z"));
        }
    }
}