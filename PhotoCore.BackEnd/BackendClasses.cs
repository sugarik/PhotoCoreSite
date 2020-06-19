using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoCore
{
    public class SearchFacesRequestFrontEnd
    {
        /// <summary> Назначенный FrontEnd идентификатор транзакции поиска лиц на фотографии </summary>
        public string requestId { get; set; }

        /// <summary> Байтовый массив с фотографией </summary>
        public string selfieImage { get; set; }
    }

    /// <summary> Структура данных результата поиска лиц на фотографии/селфи </summary>
    /// результат метода контроллера SearchFaces
    public class SearchFacesResponseBackEnd
    {
        /// <summary> Назначенный FrontEnd идентификатор транзакции поиска лиц на фотографии. </summary>
        public string requestId;

        public class Face
        {
            /// <summary> Изображение с одним из лиц фотографии/селфи </summary>
            public string faceImage { get; set; }

            /// <summary> Назначенный идентификатор лицу. Назначает сервер/BackEnd </summary>
            public string faceId { get; set; }
        }

        /// <summary> Список извлечённых лиц с идентификаторами </summary>
        public List<Face> faces { get; set; }
    }



    [Serializable]
    public class SearchPhotosRequestFrontEnd
    {
        /// <summary> Назначенный FrontEnd идентификатор транзакции поиска фотографий. Будет использоваться как ключ поиска в базе </summary>
        public string requestId { get; set; }

        public string locationId { get; set; }

        public List<string> faces { get; set; }

        /// <summary> Список дат поиска фотографий </summary>
        public List<string> searchDates { get; set; }
    }


    public class Photo
    {
        /// <summary> Идентификатор фотографии в базе. Эти идентификаторы потом надо вернуть в OrderRequest. Они соответствуют выбранным фотографиям </summary>
        public string photoProcessId { get; set; }

        /// <summary> Ссылка на хранилище миниатюры фотографии. Их надо закачать по ссылке и отобразить пользователю </summary>
        public string thumbnailLink { get; set; }

        /// <summary> Дата и время фотографии. Доп инфа для размещения под отображаемой миниатюрой фотографии. Отображается под высотой и шириной в виде "02.02.2020г" </summary>
        public string dateTime { get; set; }
    }

    /// <summary> структура данных результата поиска фотографий </summary>
    /// результат метода контроллера SearchPhotos
    [Serializable]
    public class SearchPhotosResponseBackEnd
    {
        /// <summary> назначенный FrontEnd идентификатор транзакции поиска фотографий. Подтверждаемое значение </summary>
        public string requestId { get; set; }

        /// <summary> Список извлечённых лиц с идентификаторами </summary>
        public List<Photo> photos { get; set; }

        /// <summary> Массив стоимостей в зависимости от количества фотографий </summary>
        public string[] prices { get; set; }

        /// <summary> Массив дискаунтов в зависимости от количества фотографий </summary>
        public string[] discounts { get; set; }
    }



    public class RestrictedFaceArea
    {
        public float x { get; set; }
        public float y { get; set; }
        public float w { get; set; }
        public float h { get; set; }
        public Int16 a { get; set; }
    }

    public class SearchedThumbnails
    {
        public Guid order_id { get; set; }
        public DateTime shooting_datetime { get; set; }
        public List<RestrictedFaceArea> rect { get; set; }
        public string thumbnail { get; set; }
        public string thumbnail_cloud_id { get; set; }
        public string thumbnail_local_path { get; set; }
    }

    public enum Source : byte { Local = 0, Cloud = 1, }



    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //                                                                                                              //
    //                                            РЕЗЕРВИРОВАНИЕ ЗАКАЗА                                             //   
    //                                                                                                              //
    //  1. Пользователь выбирает миниатюры фотографий, ему отображается стоимость, нажимает кнопку заказа.          //
    //  2. Система сохраняет заказ пользователя в базу                                                              //
    //  3. Пользователю отправляется номер сформированного заказа, предложение ввести почту доставки и оплатить     //
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    [Serializable]
    public class OrderReservationRequestFrontEnd
    {
        /// <summary> Назначенный FrontEnd идентификатор транзакции резервирования покупки </summary>
        public string requestId { get; set; }

        /// <summary> Идентификаторы выбранных фотографий. Из SearchPhotosResponse </summary>
        public List<string> photoProcessId { get; set; }

        /// Если данные можно получить на стороне FrontEnd, то можно закомментировать или указывать NULL
        /// <summary> Конкатенированная строка из данных пользовательской сессии: IP, Port, UserAgent, MAC </summary>
        public string sessionId { get; set; }
    }

    /// <summary> Структура данных ответа на резервирование заказа </summary>
    /// результат метода контроллера OrderReservation
    [Serializable]
    public class OrderReservationResponseBackEnd
    {
        /// <summary> назначенный FrontEnd идентификатор резервирования покупки. Подтверждаемое значение </summary>
        public string requestId { get; set; }

        /// <summary> Назначенный BackEnd идентификатор заказа. Отображается пользователю </summary>
        public string orderNumber { get; set; }

        /// <summary> Стоимость заказа. Отображается пользователю /// </summary>
        public string amount { get; set; }

        /// <summary> Описание заказа. Отображается пользователю /// </summary>
        public string description { get; set; }
    }




    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //                                                                                                              //
    //                                               ОПЛАТА ЗАКАЗА                                                  //                                                                                                             //
    //                                                                                                              //
    //  1. В систему поступает запрос с номером заказа и e-mail его доставки                                        //
    //  2. Система делает запрос в банк на резервирование транзакции оплаты                                         //
    //  3. Система отправляет пользователю строку редиректа на оплату                                               //
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    [Serializable]
    public class OrderPaymentRequestFrontEnd
    {
        /// <summary> Назначенный FrontEnd идентификатор транзакции оплаты покупки </summary>
        public string requestId { get; set; }

        /// <summary> Подтверждаемый идентификатор заказа </summary>
        public string orderNumber { get; set; }

        /// <summary> Введённый пользователем основной email доставки заказа </summary>
        public string mainEmail { get; set; }

        /// <summary> Введённый пользователем дополнительный email доставки заказа </summary>
        public string backupEmail { get; set; }

        /// <summary> Введённый пользователем номер телефона доставки заказа через мэссанджеры </summary>
        public string phone { get; set; }

        /// <summary> Матрица из активированных мэссанджеров для доставки заказа </summary>
        public Int16 messangers { get; set; }
    }

    /// <summary> Структура данных ответа на оплату заказа </summary>
    /// результат метода контроллера OrderPayment
    [Serializable]
    public class OrderPaymentResponseBackEnd
    {
        /// <summary> назначенный FrontEnd идентификатор оплаты покупки. Подтверждаемое значение </summary>
        public string requestId { get; set; }

        /// <summary> Назначенный BackEnd идентификатор заказа. Отображается пользователю </summary> //////////////////////////////////// А нужен ли этот параметр ???????????????????????? Разве что для логирования
        public string orderId { get; set; }

        /// <summary> Код успешности готовности осуществления оплаты со стороны банка </summary>
        public byte errorCode { get; set; } // цифровой логично использовать для мультиязычной поддержки

        /// <summary> Страница переадресации пользователя для осуществления оплаты по банковской карточке. Не null если банк принял заказ и ждёт оплаты </summary>
        public string formUrl { get; set; }
    }


    public class PriceCalculator
    {
        float[] pricesPolicy = new float[] { 0, 6, 6, 6, 5, 5, 5, 4, 4, 4, 3, 3, 3, 2, 2, 2, 1, 1, 1 };
        float[] price;
        float[] discountValue;
        float[] discountPercent;

        string[] priceValues;
        string[] discountValues;

        //public float[] Price { get { return price; } }
        //public float[] DiscountValue { get { return discountValue; } }
        //public float[] DiscountPercent { get { return discountPercent; } }
        public string[] PriceValues { get { return priceValues; } }
        public string[] DiscountValues { get { return discountValues; } }

        public PriceCalculator(int totalPhotos)
        {
            price = new float[totalPhotos + 1];
            discountValue = new float[totalPhotos + 1];
            discountPercent = new float[totalPhotos + 1];
            priceValues = new string[totalPhotos + 1];
            discountValues = new string[totalPhotos + 1];
            priceValues[0] = "0.00 BYN";
            discountValues[0] = "0.00 BYN ( 0.0 % )"; 

            float fullPrice = 0;

            if (pricesPolicy.Length > totalPhotos)
            {
                for (int i=1; i < price.Length; i++)
                {
                    fullPrice = pricesPolicy[1] * i;
                    price[i] = price[i - 1] + pricesPolicy[i];
                    discountValue[i] = fullPrice - price[i];
                    discountPercent[i] = (float)Math.Round((fullPrice - price[i]) * 100 / (fullPrice), 1);
                    priceValues[i] = String.Format("{0:F2} BYN", price[i]);
                    discountValues[i] = String.Format("{0:F2} BYN ( {1:F1} % )", discountValue[i], discountPercent[i]);
                }
            }
            else 
            {
                for (int i = 1; i < pricesPolicy.Length; i++)
                {
                    fullPrice = pricesPolicy[1] * i;
                    price[i] = price[i - 1] + pricesPolicy[i];
                    discountValue[i] = fullPrice - price[i];
                    discountPercent[i] = (float)Math.Round((fullPrice - price[i]) * 100 / (fullPrice), 1);
                    priceValues[i] = String.Format("{0:F2} BYN", price[i]);
                    discountValues[i] = String.Format("{0:F2} BYN ( {1:F1} % )", discountValue[i], discountPercent[i]);
                }
                for (int i = pricesPolicy.Length - 1; i < price.Length; i++)
                {
                    fullPrice = pricesPolicy[1] * i;
                    price[i] = price[i - 1] + pricesPolicy[pricesPolicy.Length - 1];
                    discountValue[i] = fullPrice - price[i];
                    discountPercent[i] = (float)Math.Round((fullPrice - price[i]) * 100 / (fullPrice), 1);
                    priceValues[i] = String.Format("{0:F2} BYN", price[i]);
                    discountValues[i] = String.Format("{0:F2} BYN ( {1:F1} % )", discountValue[i], discountPercent[i]);
                }
            }
        }
    }



}
