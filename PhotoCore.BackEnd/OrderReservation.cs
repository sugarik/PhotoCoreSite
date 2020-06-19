using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoCore
{
    public class OrderReservation
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(SearchFaces));

        OrderReservationResponseBackEnd response = null;
        public OrderReservationResponseBackEnd Response { get { return response; } }

        public OrderReservation (OrderReservationRequestFrontEnd request, string sessionInfo, ProjectConfigData configData)
        {
            response = new OrderReservationResponseBackEnd();
            if (String.IsNullOrEmpty(request.requestId)) response.requestId = Guid.NewGuid().ToString();
            else response.requestId = request.requestId;

            try
            {
                if (request.photoProcessId != null && request.photoProcessId.Count > 0)
                {
                    int photoCounter = request.photoProcessId.Count;

                    float amountValue = 0;
                    if (photoCounter > configData.billCosts.Length - 1) amountValue = (float)Math.Round(configData.billCosts[configData.billCosts.Length - 1], 2);
                    else amountValue = (float)Math.Round(configData.billCosts[photoCounter]);

                    if (amountValue > 0)
                    {
                        response.amount = String.Format("{0} {1}", amountValue, configData.billCurrency);

                        double delta = TimeSpan.FromTicks(DateTime.UtcNow.Ticks - 636710112000000000).TotalSeconds;
                        response.orderNumber = String.Format("{0}{1}", configData.billOrderPrefix, (((UInt64)(delta)).ToString("X") + ((byte)(delta % 1 * 16)).ToString("X")).Replace("0", "G").Replace("A", "S").Replace("B", "V").Replace("C", "R").Replace("E", "Z"));

                        if (photoCounter == 1 || photoCounter == 21 || photoCounter == 31 || photoCounter == 41 || photoCounter == 51 || photoCounter == 61 || photoCounter == 71 || photoCounter == 81 || photoCounter == 91 || photoCounter == 101) response.description = String.Format("Покупка {0} фотографии.", photoCounter);
                        else response.description = String.Format("Покупка {0} фотографий.", photoCounter);

                        MsSqlDbExplorer mySqlDbExplorer = new MsSqlDbExplorer();
                        mySqlDbExplorer.Orders_AddNewRecord(response.orderNumber, DateTime.Now, request.photoProcessId, (int)(amountValue * 100), 933, response.description, "user", request.sessionId);
                    }
                    else response.description = "Нет фотографий для приобретения.";
                }
                else
                {
                    response.description = "Нет фотографий для приобретения.";
                }
            }
            catch (Exception exc) { }
        }

    }
}
