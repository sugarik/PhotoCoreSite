using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoCore
{
    public class OrderDeliverer
    {

        // {merchant-url}?mdOrder={mdOrder}&orderNumber={orderNumber}&operation={operation}&status={status}
        // mdOrder       ANS36     Номер заказа в платежной системе
        // orderNumber   AN..32    Номер(идентификатор) заказа в системе магазина, уникален для каждого магазина в пределах системы
        // operation     N..9      Тип операции:
        //                          approved  - операция холдирования суммы (только для двухстадийной системы оплаты);
        //                          deposited - операция завершения;
        //                          reversed  - операция отмены;
        //                          refunded  - операция возврата;
        // status        N1        Индикатор успешности операции, указанной в параметре operation(1 - операция прошла успешно, 0 - операция завершилась ошибкой)

        // Пример URL: https://myshop.ru/callback/?mdOrder=1234567890-098776-234-522&orderNumber=0987&operation=deposited&status=0

        public class BillReport
        {
            /// <summary> Номер заказа в платежной системе </summary>
            public string mdOrder { get; set; }

            /// <summary> Номер(идентификатор) заказа в системе магазина </summary>
            public string orderNumber { get; set; }

            /// <summary> Уведомление какого типа операции </summary>
            public string operation { get; set; }

            /// <summary> Индикатор успешности операции, указанной в параметре operation (1 - операция прошла успешно, 0 - операция завершилась ошибкой) </summary>
            public byte status { get; set; }
        }


        BillReport billReport = null;
        public BillReport Report { get { return billReport; } }

        public OrderDeliverer(string billingReport)
        {
            try
            {
                billReport = JsonConvert.DeserializeObject<BillReport>(billingReport);


                // 1. Извлечь из базы данные заказа


                // 2. Разослать из заказа ссылки на контент


            }
            catch(Exception exc)
            {

            }
        }

    }
}
