using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AspNetMvc.Classes
{
    public class OrderRequestFromClient
    {
        public string sessionId { get; set; }

        /// <summary> Идентификаторы выбранных фотографий. Из SearchPhotosResponse </summary>
        public List<string> photoProcessId { get; set; }
    }


    //[Serializable]
    //public class OrderRequest
    //{
    //    /// <summary> назначенный FrontEnd идентификатор платежа заказа. Не более 32 буквенно-цифровых символов. </summary>
    //    public string orderId;

    //    /// <summary> Идентификатор пользователя в системе </summary>
    //    public string userId { get; set; }

    //    /// <summary> Конкатенированная строка из данных пользовательской сессии: IP, Port, UserAgent, MAC </summary>
    //    public string sessionId { get; set; }

    //    /// <summary> Идентификаторы выбранных фотографий. Из SearchPhotosResponse </summary>
    //    public List<string> photoProcessId { get; set; }

    //    /// <summary> Стоимость заказа в валюте платежа </summary>
    //    public float amount { get; set; }

    //    /// <summary> Валюта платежа (933 - BYN) </summary>
    //    public ushort currency { get; set; }

    //    /// <summary> URL куда возвращать успешную обработку платежа </summary>
    //    public string returnUrl { get; set; }

    //    /// <summary> URL куда возвращать ошибку обработки платежа </summary>
    //    public string failUrl { get; set; }

    //    /// <summary> Описание заказа </summary>
    //    public string description { get; set; }
    //}
}