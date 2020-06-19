﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AspNetMvc.Classes
{


    /// <summary> Структура данных поиска лиц на фотографии/селфи </summary>
    /// метод контроллера SearchFaces (SearchFacesRequest request)
    [Serializable]
    public class SearchFacesRequest
    {
        /// <summary> Назначенный FrontEnd идентификатор транзакции поиска лиц на фотографии </summary>
        public string requestId { get; set; }

        /// <summary> Байтовый массив с фотографией </summary>
        public byte[] selfieImage { get; set; }

        // ********************************************************************************************************************************

        /// Если данные можно получить на стороне FrontEnd, то можно закомментировать или указывать NULL
        /// <summary> Идентификатор пользователя в системе учёта пользователей FrontEnd. </summary>
        public string userId { get; set; }

        /// Если данные можно получить на стороне FrontEnd, то можно закомментировать или указывать NULL
        /// <summary> Конкатенированная строка из данных пользовательской сессии: IP, Port, UserAgent, MAC </summary>
        public string sessionId { get; set; }
    }


    /// <summary> Структура данных результата поиска лиц на фотографии/селфи </summary>
    /// результат метода контроллера SearchFaces
    [Serializable]
    public class SearchFacesResponse
    {
        /// <summary> Назначенный FrontEnd идентификатор транзакции поиска лиц на фотографии. </summary>
        public string requestId;

        public class Face
        {
            /// <summary> Изображение с одним из лиц фотографии/селфи </summary>
            public byte[] faceImage { get; set; }

            /// <summary> Назначенный идентификатор лицу. Назначает сервер/BackEnd </summary>
            public string faceId { get; set; }
        }

        /// <summary> Список извлечённых лиц с идентификаторами </summary>
        public List<Face> faces { get; set; }
    }


    /// <summary> Структура данных поиска фотографий по лицам </summary>
    /// метод контроллера SearchPhotos (SearchPhotosRequest request)
    [Serializable]
    public class SearchPhotosRequest
    {

        /// <summary> Назначенный FrontEnd идентификатор транзакции поиска фотографий. Будет использоваться как ключ поиска в базе </summary>
        public string requestId { get; set; }


        public class Face
        {
            /// <summary> Изображение с одним из лиц фотографии/селфи </summary>
            public byte[] faceImage { get; set; }

            /// <summary> Назначенный идентификатор лицу. Назначает сервер/BackEnd </summary>
            public string faceId { get; set; }
        }

        /// <summary> Список структур данных лиц </summary>
        public List<Face> faces { get; set; }

        /// <summary> Список дат поиска фотографий </summary>
        public List<DateTime> searchDates { get; set; }

        // ********************************************************************************************************************************

        /// Если данные можно получить на стороне FrontEnd, то можно закомментировать или указывать NULL
        /// <summary> Идентификатор пользователя в системе </summary>
        public string userId { get; set; }

        /// Если данные можно получить на стороне FrontEnd, то можно закомментировать или указывать NULL
        /// <summary> Конкатенированная строка из данных пользовательской сессии: IP, Port, UserAgent, MAC </summary>
        public string sessionId { get; set; }
    }

    /// <summary> структура данных результата поиска фотографий </summary>
    /// результат метода контроллера SearchPhotos
    [Serializable]
    public class SearchPhotosResponse
    {
        /// <summary> назначенный FrontEnd идентификатор транзакции поиска фотографий. Подтверждаемое значение </summary>
        public string requestId { get; set; }

        public class Photo
        {
            /// <summary> Идентификатор фотографии в базе. Эти идентификаторы потом надо вернуть в OrderRequest. Они соответствуют выбранным фотографиям </summary>
            public string photoProcessId { get; set; }

            /// <summary> Ссылка на хранилище миниатюры фотографии. Их надо закачать по ссылке и отобразить пользователю </summary>
            public string thumbnailLink { get; set; }

            /// <summary> Ширина фотографии в пикселях. Доп инфа для размещения под отображаемой миниатюрой фотографии. Отображается совместно ширина и высота в виде "6000х4000 пикселей" </summary>
            public int width { get; set; }

            /// <summary> Высота фотографии в пикселях. Доп инфа для размещения под отображаемой миниатюрой фотографии. Отображается совместно ширина и высота в виде "6000х4000 пикселей" </summary>
            public int height { get; set; }

            /// <summary> Дата и время фотографии. Доп инфа для размещения под отображаемой миниатюрой фотографии. Отображается под высотой и шириной в виде "02.02.2020г" </summary>
            public string dateTime { get; set; }
        }

        /// <summary> Список извлечённых лиц с идентификаторами </summary>
        public List<Photo> photos { get; set; }
    }


    /// <summary> Структура данных предварительного резервирования заказа </summary>
    /// метод контроллера OrderReservation (OrderReservationRequest request)
    [Serializable]
    public class OrderReservationRequest
    {
        /// <summary> Назначенный FrontEnd идентификатор транзакции резервирования покупки </summary>
        public Guid requestId { get; set; }

        /// <summary> Идентификаторы выбранных фотографий. Из SearchPhotosResponse </summary>
        public List<string> photoProcessId { get; set; }

        // ********************************************************************************************************************************

        /// Если данные можно получить на стороне FrontEnd, то можно закомментировать или указывать NULL
        /// <summary> Идентификатор пользователя в системе </summary>
        public string userId { get; set; }

        /// Если данные можно получить на стороне FrontEnd, то можно закомментировать или указывать NULL
        /// <summary> Конкатенированная строка из данных пользовательской сессии: IP, Port, UserAgent, MAC </summary>
        public string sessionId { get; set; }
    }


    /// <summary> Структура данных ответа на резервирование заказа </summary>
    /// результат метода контроллера OrderReservation
    [Serializable]
    public class OrderReservationResponse
    {
        /// <summary> назначенный FrontEnd идентификатор резервирования покупки. Подтверждаемое значение </summary>
        public string requestId { get; set; }

        /// <summary> Назначенный BackEnd идентификатор заказа. Отображается пользователю </summary>
        public string orderId { get; set; }

        /// <summary> Стоимость заказа. Отображается пользователю /// </summary>
        public string amount { get; set; }

        /// <summary> Описание заказа. Отображается пользователю /// </summary>
        public string description { get; set; }
    }


    /// <summary> Структура данных оплаты заказа </summary>
    /// метод контроллера OrderPayment (OrderPaymentRequest request)
    [Serializable]
    public class OrderPaymentRequest
    {
        /// <summary> Назначенный FrontEnd идентификатор транзакции оплаты покупки </summary>
        public string requestId { get; set; }

        /// <summary> Подтверждаемый идентификатор заказа </summary>
        public string orderId { get; set; }

        // ********************************************************************************************************************************

        /// Если данные можно получить на стороне FrontEnd, то можно закомментировать или указывать NULL
        /// <summary> Идентификатор пользователя в системе </summary>
        public string userId { get; set; }

        /// Если данные можно получить на стороне FrontEnd, то можно закомментировать или указывать NULL
        /// <summary> Конкатенированная строка из данных пользовательской сессии: IP, Port, UserAgent, MAC </summary>
        public string sessionId { get; set; }
    }


    /// <summary> Структура данных ответа на оплату заказа </summary>
    /// результат метода контроллера OrderPayment
    [Serializable]
    public class OrderPaymentResponse
    {
        /// <summary> назначенный FrontEnd идентификатор оплаты покупки. Подтверждаемое значение </summary>
        public string requestId { get; set; }

        /// <summary> Назначенный BackEnd идентификатор заказа. Отображается пользователю </summary>
        public string orderId { get; set; }

        /// <summary> Код успешности готовности осуществления оплаты со стороны банка </summary>
        public byte errorCode { get; set; }

        /// <summary> Страница переадресации пользователя для осуществления оплаты по банковской карточке </summary>
        public string formUrl { get; set; }
    }
}