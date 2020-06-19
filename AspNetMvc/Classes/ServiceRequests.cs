using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;




    //[Serializable]
    //public class OrderReservationRequestFrontEnd
    //{
    //    /// <summary> Назначенный FrontEnd идентификатор транзакции резервирования покупки </summary>
    //    public string requestId { get; set; }

    //    /// <summary> Идентификаторы выбранных фотографий. Из SearchPhotosResponse </summary>
    //    public List<string> photoProcessId { get; set; }

    //    /// Если данные можно получить на стороне FrontEnd, то можно закомментировать или указывать NULL
    //    /// <summary> Конкатенированная строка из данных пользовательской сессии: IP, Port, UserAgent, MAC </summary>
    //    public string sessionId { get; set; }
    //}


    ///// <summary> Структура данных ответа на резервирование заказа </summary>
    ///// результат метода контроллера OrderReservation
    //[Serializable]
    //public class OrderReservationResponseBackEnd
    //{
    //    /// <summary> назначенный FrontEnd идентификатор резервирования покупки. Подтверждаемое значение </summary>
    //    public string requestId { get; set; }

    //    /// <summary> Назначенный BackEnd идентификатор заказа. Отображается пользователю </summary>
    //    public string orderId { get; set; }

    //    /// <summary> Стоимость заказа. Отображается пользователю /// </summary>
    //    public string amount { get; set; }

    //    /// <summary> Описание заказа. Отображается пользователю /// </summary>
    //    public string description { get; set; }
    //}





    //[Serializable]
    //public class OrderPaymentRequestFrontEnd
    //{
    //    /// <summary> Назначенный FrontEnd идентификатор транзакции оплаты покупки </summary>
    //    public string requestId { get; set; }

    //    /// <summary> Подтверждаемый идентификатор заказа </summary>
    //    public string orderId { get; set; }

    //    /// <summary> Введённый пользователем email доставки заказа </summary>
    //    public string email { get; set; }
    //}

    ///// <summary> Структура данных ответа на оплату заказа </summary>
    ///// результат метода контроллера OrderPayment
    //[Serializable]
    //public class OrderPaymentResponseBackEnd
    //{
    //    /// <summary> назначенный FrontEnd идентификатор оплаты покупки. Подтверждаемое значение </summary>
    //    public string requestId { get; set; }

    //    /// <summary> Назначенный BackEnd идентификатор заказа. Отображается пользователю </summary>
    //    public string orderId { get; set; }

    //    /// <summary> Код успешности готовности осуществления оплаты со стороны банка </summary>
    //    public byte errorCode { get; set; }

    //    /// <summary> Страница переадресации пользователя для осуществления оплаты по банковской карточке </summary>
    //    public string formUrl { get; set; }
    //}
