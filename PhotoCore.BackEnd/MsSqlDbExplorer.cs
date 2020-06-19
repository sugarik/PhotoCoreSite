using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Threading;
using SkiaSharp;

namespace PhotoCore
{
    public class MsSqlDbExplorer
    {
        string connectionString = "Data Source=mssql.by1880.hb.by;Initial Catalog=photocore;Persist Security Info=True;User ID=photocoreuser;Password=!Q@WAZSX3e4rdcfv;Pooling=True";

        public class ContentData
        {
            public Guid amazon_id { get; set; }
            public short location_id { get; set; }
            public short partner_id { get; set; }
            public Guid order_id { get; set; }
            public DateTime shooting_datetime { get; set; }
            public DateTime ready_datetime { get; set; }
            public DateTime expiration_date { get; set; }
            public bool locked { get; set; }

            /// <summary> относительная координата левой стороны квадрата лица по ширине </summary>
            public float? x { get; set; }
            /// <summary> относительная координата верхней стороны квадрата лица по высоте </summary>
            public float? y { get; set; }
            /// <summary> относительная ширина квадрата лица от ширины снимка </summary>
            public float? w { get; set; }
            /// <summary> относительная высота квадрата лица от высоты снимка </summary>
            public float? h { get; set; }
            /// <summary> угол наклона лица </summary>
            public Int16? a { get; set; }
            /// <summary> ширина фотографии в пикселях </summary>
            public short? width { get; set; }
            /// <summary> высота фотографии в байтах </summary>
            public short? height { get; set; }
            /// <summary> размер фотографии в байтах </summary>
            public int? size { get; set; }
            public string face_cloud_id { get; set; }
            public string thumbnail_cloud_id { get; set; }
            public string photo_cloud_id { get; set; }
            public string face_local_path { get; set; }
            public string thumbnail_local_path { get; set; }
            public string photo_local_path { get; set; }
        }


        public class OrderReservation
        {
            public string orderNumber { get; set; } // генерит бэкэнд
            public DateTime orderDatetime { get; set; } // генерит бэкэнд
            public string contentIdList { get; set; } // приходит от фронтэнда
            public int amount { get; set; } // генерит бэкэнд
            public Int16 currency { get; set; } // генерит бэкэнд
            public string description { get; set; }// генерит бэкэнд
            public string userId { get; set; } // приходит от фронтэнда
            public string userSession { get; set; } // генериться на стыке фронтэнда и бэкэнда
        }

        public class OrderAddressUpdate
        {
            public string orderNumber { get; set; } // генерит бэкэнд
            public DateTime addressUpdateDatetime { get; set; } // генерит бэкэнд
            public string email1 { get; set; }
            public string email2 { get; set; }
            public string phone { get; set; }
            public Int16 messagers { get; set; }
            public string orderId { get; set; }
        }

        public class OrderPaymentUpdate
        {
            public string orderNumber { get; set; } // генерит бэкэнд
            public DateTime paymentUpdateDatetime { get; set; } // генерит бэкэнд // По значению этого параметра определяется состояние заказа в платежной системе.Список возможных значений приведен в таблице ниже.Отсутствует, если заказ не был найден.
            public Int16 errorCode { get; set; } // Код ошибки.
            public string errorMessage { get; set; } // Описание ошибки на языке, переданном в параметре Language в запросе.
            public byte orderStatus { get; set; } // По значению этого параметра определяется состояние заказа в платежной системе.Список возможных значений приведен в таблице ниже.Отсутствует, если заказ не был найден.
            public string pan { get; set; } // Маскированный номер карты, которая использовалась для оплаты.Указан только после оплаты заказа.
            public string expiration { get; set; } // Срок истечения действия карты в формате YYYYMM. Указан только после оплаты заказа.
            public string cardholderName { get; set; } // Имя держателя карты. Указан только после оплаты заказа.
            public string approvalCode { get; set; } // Код авторизации МПС. Поле фиксированной длины(6 символов), может содержать цифры и латинские буквы.
            public Int16 authCode { get; set; } // Это поле является устаревшим.Его значение всегда равно "2", независимо от состояния заказа и кода авторизации процессинговой системы.
            public string ip { get; set; } // IP адрес пользователя, который оплачивал заказ
        }

        public class OrderDeliveryUpdate
        {
            public string orderNumber { get; set; } // генерит бэкэнд
            public DateTime deliveryUpdateDatetime { get; set; }
            public bool deliveryStatus {get;set;}
        }

        public MsSqlDbExplorer() { }

        public MsSqlDbExplorer(string connectionString) { this.connectionString = connectionString; }

        public MsSqlDbExplorer(ProjectConfigData projectConfigData) { this.connectionString = String.Format("Data Source={0};Initial Catalog={1};Persist Security Info={2};User ID={3};Password={4};Pooling={5}", projectConfigData.dbDataSource, projectConfigData.dbInitialCatalog, projectConfigData.dbPersistSecurityInfo, projectConfigData.dbUserId, projectConfigData.dbPassword, projectConfigData.dbPooling); ; }

        public enum Table : byte { Content = 1, SearchFaces = 2, SearchPhotos = 3, Orders = 4, OrderReservations = 5, OrderPayments = 6 }

        /// <summary> Метод создания требуемой таблицы </summary>
        /// <returns> Результат создания таблицы: -1 - не создана;  0 - уже имеется, нет надобности создавать;  1 - вновь создана  /// </returns>
        public int CreateTable(Table table)
        {
            int result = -1; // пока устанавливаем не "НЕ СОЗДАНА"
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    bool tableExist = false;
                    cmd.CommandText = String.Format("select object_id('{0}')", table);
                    if (connection.State != ConnectionState.Open) connection.Open();
                    object id = cmd.ExecuteScalar();
                    if (id is System.Int32) tableExist = true;  else if (id is System.DBNull) tableExist = false;

                    if (!tableExist)
                    {
                        switch (table)
                        {
                            case Table.Content:
                                cmd.CommandText = "CREATE TABLE Content ("
                                    + "[amazon_id] UNIQUEIDENTIFIER NOT NULL," // идентификатор лица в Amazon
                                    + "[location_id] SMALLINT NOT NULL," // идентификатор места съёмки
                                    + "[partner_id] SMALLINT NULL," // идентификатор фотографа или устройства
                                    + "[order_id] UNIQUEIDENTIFIER NOT NULL," // идентификатор обработчика фотографии
                                    + "[shooting_datetime] DATETIME NOT NULL," // дата и время съёмки
                                    + "[ready_datetime] DATETIME NOT NULL," // дата и время готовности контента
                                    + "[expiration_date] DATE NOT NULL," // конечная дата хранения контента
                                    + "[locked] BIT NOT NULL," // указатель блокировки отображения лица

                                    + "[x] FLOAT(24) NULL," // относительная координата левой стороны квадрата лица по ширине
                                    + "[y] FLOAT(24) NULL," // относительная координата верхней стороны квадрата лица по высоте
                                    + "[w] FLOAT(24) NULL," // относительная ширина квадрата лица от ширины снимка
                                    + "[h] FLOAT(24) NULL," // относительная высота квадрата лица от высоты снимка
                                    + "[a] SMALLINT NULL,"  // угол поворота лица на фотографии

                                    + "[width] SMALLINT NULL," // ширина фотографии в пикселях
                                    + "[height] SMALLINT NULL," // высота фотографии в байтах
                                    + "[size] INT NULL," // размер фотографии в байтах

                                    + "[face_cloud_id] VARCHAR(127) NULL," // идентификатор лица в системе провайдера облачного хранения 
                                    + "[thumbnail_cloud_id] VARCHAR(127) NULL," // идентификатор миниатюры в системе провайдера облачного хранения 
                                    + "[photo_cloud_id] VARCHAR(127) NULL," // идентификатор фотографии в системе провайдера облачного хранения 

                                    + "[face_local_path] VARCHAR(255) NULL," // путь к изображению лица в локальной файловой системе хранения
                                    + "[thumbnail_local_path] VARCHAR(255) NULL," // путь к изображению миниатюры в локальной файловой системе хранения
                                    + "[photo_local_path] VARCHAR(255) NULL," // путь к изображению фотографии в локальной файловой системе хранения

                                    + "PRIMARY KEY CLUSTERED([amazon_id] ASC));"
                                    + "CREATE INDEX[IX_Content_amazon_id] ON[Content]([amazon_id])"
                                    + "CREATE INDEX[IX_Content_location_id] ON[Content]([location_id])"
                                    + "CREATE INDEX[IX_Content_shooting_datetime] ON[Content]([shooting_datetime])"
                                    + "CREATE INDEX[IX_Content_expiration_date] ON[Content]([expiration_date])";
                                break;
                            case Table.SearchFaces:
                                cmd.CommandText = "CREATE TABLE SearchFaces (" // на каждое поисковое лицо одна запись
                                    + "[face_id] UNIQUEIDENTIFIER NOT NULL," // системный идентификатор изображения лица поиска. Равен аналогичной переменной в таблице Search_results
                                    + "[datetime] DATETIME NOT NULL," // дата и время поиска
                                    + "[search_dates] VARCHAR(127) NOT NULL," // даты поиска контента, в формате YYYYMMDD разделённые через ";"
                                    + "[face_local_path] VARCHAR(255) NULL," // путь к изображению лица в локальной файловой системе хранения
                                    + "[face_cloud_id] VARCHAR(127) NULL," // идентификатор изображения лица в облачной системе хранения
                                    + "[user_id] VARCHAR(8000) NOT NULL," // идентификатор пользователя
                                    + "PRIMARY KEY CLUSTERED([face_id] ASC));"
                                    + "CREATE INDEX[IX_Content_face_id] ON SearchFaces([face_id])";
                                break;
                            case Table.SearchPhotos:
                                cmd.CommandText = "CREATE TABLE SearchPhotos (" // на каждое поисковое лицо одна запись
                                   + "[face_id] UNIQUEIDENTIFIER NOT NULL," // идентификатор изображения лица поиска. . Равен аналогичной переменной в таблице Search_faces
                                   + "[datetime] DATETIME NOT NULL," // дата и время поиска
                                   + "[content_amazon_id_list] VARCHAR(8000) NOT NULL," // идентификатор пользователя
                                   + "PRIMARY KEY CLUSTERED([face_id] ASC));"
                                   + "CREATE INDEX[IX_Content_face_id] ON SearchPhotos([face_id])";
                                break;
                            //case Table.OrderReservations:
                            //    cmd.CommandText = "CREATE TABLE OrderReservations ("
                            //        + "[order_id] UNIQUEIDENTIFIER NOT NULL," // идентификатор резервирования заказа
                            //        + "[datetime] DATETIME NOT NULL," // дата и время резервирования
                            //        + "[content_order_id_list] VARCHAR(8000) NOT NULL," // список идентификаторов обработчиков фотографий (включающих данные фото, лица и миниатюры)
                            //        + "[currency_id] CHAR(3) NULL," // валюта платежа. Буквенный трехзначный код валюты согласно ISO4271.
                            //        + "[amount] SMALLMONEY NOT NULL," // стоимость предзаказа
                            //        + "[rate] NVARCHAR(1024) NOT NULL," // тариф, описание заказа
                            //        + "[session_id] UNIQUEIDENTIFIER NOT NULL," // идентификатор сессии предзаказа
                            //        + "[email] VARCHAR(127) NULL," // email доставки заказа
                            //        + "[phone] VARCHAR(127) NULL," // телефон доставки заказа
                            //        + "[messenger_list] VARCHAR(127) NULL," // мессанджеры доставки заказа через ";"
                            //        + "[user_id] VARCHAR(8000) NOT NULL," // идентификатор пользователя
                            //        + "PRIMARY KEY CLUSTERED([order_id] ASC));"
                            //        + "CREATE INDEX[IX_Content_order_id] ON OrderReservations([order_id])";
                            //    break;
                            //case Table.OrderPayments:
                            //    cmd.CommandText = "CREATE TABLE OrderPayments ("
                            //        + "[order_id] UNIQUEIDENTIFIER NOT NULL," // зарезервированный идентификатор  заказа
                            //        + "[datetime] DATETIME NOT NULL," // дата и время оплаты
                            //        + "[currency_id] CHAR(3) NULL," // валюта платежа. Буквенный трехзначный код валюты согласно ISO4271.
                            //        + "[amount] MONEY NULL," // стоимость предзаказа (amount - Сумма транзакции)
                            //        + "[payment_method] VARCHAR(127) NULL,"
                            //        + "[payment_order_id] VARCHAR(127) NULL," // Номер заказа в платёжной системе
                            //        + "[transaction_id] VARCHAR(127) NULL," // Номер транзакции платёжной системы
                            //        + "[payment_type] VARCHAR(127) NULL," // Номер транзакции платёжной системы
                            //        + "[rrn] VARCHAR(127) NULL," // Номер транзакции в системе Visa/ MasterCard
                            //        + "[wsb_signature] VARCHAR(127) NULL," // Номер транзакции в системе Visa/ MasterCard

                            //        + "[price] MONEY NOT NULL," // стоимость предзаказа
                            //        + "[rate] NVARCHAR(1024) NOT NULL," // тариф, описание заказа
                            //        + "PRIMARY KEY CLUSTERED([order_id] ASC));"
                            //        + "CREATE INDEX[IX_Content_order_id] ON OrderPayments([order_id])";
                            //    break;

                            case Table.Orders:
                                cmd.CommandText = "CREATE TABLE Orders ("
                                    + "[order_number] VARCHAR(32) NOT NULL," // Номер(идентификатор) заказа в системе магазина
                                    + "[order_datetime] DATETIME NOT NULL," // Дата и время оформления заказа
                                    + "[content_id_list] VARCHAR(8000) NOT NULL," // список приобретаемых идентификаторов фотографий
                                    + "[amount] INT NOT NULL," // Сумма платежа в копейках(или центах)
                                    + "[currency] SMALLINT NOT NULL," // Код валюты платежа ISO 4217.Если не указан, считается равным коду валюты по умолчанию.
                                    + "[description] NVARCHAR(1024)	NULL," //Описание заказа в свободной форме
                                    + "[user_id] VARCHAR(127) NOT NULL," // Идентификатор пользователя
                                    + "[user_session] VARCHAR(8000)	NOT NULL," // Информация по сессии пользователя

                                    + "[address_update_datetime] DATETIME NULL," // Дата и время оформления заказа
                                    + "[email_1] VARCHAR(127) NULL," // Почта доставки контента
                                    + "[email_2] VARCHAR(127) NULL," // Почта доставки контента
                                    + "[phone] VARCHAR(127) NULL," // Телефон пользователя
                                    + "[messagers] SMALLINT NULL," // Битовая маска запрошенных мэссанджеров
                                    + "[order_id] VARCHAR(36) NULL," // Номер заказа в платежной системе.Уникален в пределах системы. Отсутствует если регистрация заказа на удалась по причине ошибки, детализированной в ErrorCode.

                                    + "[payment_update_datetime] DATETIME NULL," // По значению этого параметра определяется состояние заказа в платежной системе.Список возможных значений приведен в таблице ниже.Отсутствует, если заказ не был найден.
                                    + "[error_code] SMALLINT NULL," // Код ошибки.
                                    + "[error_message] NVARCHAR(512) NULL," // Описание ошибки на языке, переданном в параметре Language в запросе.
                                    + "[order_status] TINYINT NULL," // По значению этого параметра определяется состояние заказа в платежной системе.Список возможных значений приведен в таблице ниже.Отсутствует, если заказ не был найден.
                                    + "[pan] VARCHAR(19) NULL," // Маскированный номер карты, которая использовалась для оплаты.Указан только после оплаты заказа.
                                    + "[expiration] CHAR(6)	NULL," // Срок истечения действия карты в формате YYYYMM. Указан только после оплаты заказа.
                                    + "[cardholder_name] VARCHAR(64) NULL," // Имя держателя карты. Указан только после оплаты заказа.
                                    + "[approval_code] CHAR(6) NULL," // Код авторизации МПС. Поле фиксированной длины(6 символов), может содержать цифры и латинские буквы.
                                    + "[auth_code] SMALLINT NULL," // Это поле является устаревшим.Его значение всегда равно "2", независимо от состояния заказа и кода авторизации процессинговой системы.
                                    + "[ip] VARCHAR(20)	NULL," // IP адрес пользователя, который оплачивал заказ

                                    + "[delivery_update_datetime] DATETIME NULL," //
                                    + "[delivery_status] BIT NULL," // Статус доставки

                                    + "PRIMARY KEY CLUSTERED([order_number] ASC));"
                                    + "CREATE INDEX[IX_Content_order_number] ON Orders([order_number])"
                                    + "CREATE INDEX[IX_Content_order_id] ON Orders([order_id])";
                                break;
                        }
                        cmd.ExecuteNonQuery();
                        result = 1;
                    }
                    else result = 0;
                    if (connection.State == ConnectionState.Open) connection.Close();
                }
            }
            catch (Exception exc)
            {

            }
            return result;
        }

        /// <summary> Метод удаления требуемой таблицы </summary>
        /// <returns> Результат удаления таблицы:
        /// -1 - не удалена;
        ///  0 - уже отсутствовала
        ///  1 - удалена
        /// </returns>
        public int DropTable(Table table)
        {
            int result = -1; // пока устанавливаем не "НЕ УДАЛЕНА"
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    bool tableExist = false;
                    cmd.CommandText = String.Format("select object_id('{0}')", table);
                    if (connection.State != ConnectionState.Open) connection.Open();
                    object id = cmd.ExecuteScalar();
                    if (id is System.Int32) tableExist = true; else if (id is System.DBNull) tableExist = false;

                    if (tableExist)
                    {
                        cmd.CommandText = String.Format("DROP TABLE IF EXISTS {0};", table);
                        int rows = cmd.ExecuteNonQuery();
                        result = 1;
                    }
                    else result = 0;
                    if (connection.State == ConnectionState.Open) connection.Close();
                }
            }
            catch (Exception exc)
            {

            }
            return result;
        }

        /// <summary> Метод подсчёта количества строк интересуемой таблицы </summary>
        /// <param name="table"> Запрашиваемая для подсчёта таблица базы данных </param>
        /// <returns> -1 если запрос не успешени или таблица отсутствует, иначе - кол-во сторок в таблице </returns>
        public int SelectCountFrom(Table table)
        {
            int result = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = String.Format("SELECT count(*) FROM {0};", table);
                    if (connection.State != ConnectionState.Open) connection.Open();
                    result = (int)cmd.ExecuteScalar();                    
                    if (connection.State == ConnectionState.Open) connection.Close();
                }
            }
            catch (Exception exc)
            {

            }
            return result;
        }

        // здесь заменили тип locationId с Int16 на string. Учесть в структуре базы
        public List<Photo> SelectFromContent(List<Guid>[] amazomIds, string locationId, List<string> dateMasks, Source source = Source.Cloud)
        {
            List<Guid> allFacesIds = new List<Guid>(); // все идентификаторы лиц похожих на искомые
            Dictionary<Guid, SearchedThumbnails> legalPhotos = new Dictionary<Guid, SearchedThumbnails>();
            List<Guid> sortedOrderIds = new List<Guid>();

            #region формируем общий список уникальных лиц allFacesIds, избавляемся от возможных дублей
            if (amazomIds != null && amazomIds.Length > 0)
            {
                for (int f = 0; f < amazomIds.Length; f++) // перебираем коллекции
                {
                    if (amazomIds[f] != null && amazomIds[f].Count > 0)
                    {
                        for (int i = 0; i < amazomIds[f].Count; i++) if (!allFacesIds.Contains(amazomIds[f][i])) allFacesIds.Add(amazomIds[f][i]);
                    }
                }
            }
            #endregion формируем общий список уникальных лиц allFacesIds, избавляемся от возможных дублей

            #region выявляем некорректные форматы дат
            if (dateMasks != null && dateMasks.Count > 0)
            {
                // избавляемся от потенциально неверных дат
                for (int i = dateMasks.Count - 1; i > -1; i--) if (dateMasks[i] == null && dateMasks[i].Length != 8) dateMasks.RemoveAt(i);
            }
            #endregion выявляем некорректные форматы дат

            if (allFacesIds.Count > 0 && dateMasks != null && dateMasks.Count > 0)
            {
                // ШАГ первый - проверить поисковые лица на предмет присутствия отписанных лиц и изъять фотки с запретами.

                string queryString = "";
                #region ФОРМИРУЕМ СТРОКУ ЗАПРОСА 1
                StringBuilder sb1 = new StringBuilder();
                sb1.Append(String.Format("SELECT order_id, shooting_datetime, locked, thumbnail_cloud_id, thumbnail_local_path FROM Content WHERE location_id = {0} and (", locationId));
                for (int i = 0; i < dateMasks.Count; i++)
                {
                    sb1.Append(String.Format(" ( shooting_datetime between '{0}-{1}-{2}' and '{0}-{1}-{2} 23:59:59.999' )", dateMasks[i].Substring(0, 4), dateMasks[i].Substring(4, 2), dateMasks[i].Substring(6, 2)));
                    if (i < dateMasks.Count - 1) sb1.Append(" or"); else sb1.Append(" ) and (");
                }
                for (int i = 0; i < allFacesIds.Count; i++) // перебираем лица в коллекции
                {
                    sb1.Append(String.Format(" amazon_id='{0}'", allFacesIds[i]));
                    if (i < allFacesIds.Count - 1) sb1.Append(" or"); else sb1.Append(" ) ORDER BY shooting_datetime ;");
                }
                queryString = sb1.ToString(); sb1.Clear();
                allFacesIds = null;
                #endregion ФОРМИРУЕМ СТРОКУ ЗАПРОСА 1

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(queryString, connection))
                    {
                        if (connection.State != ConnectionState.Open) connection.Open();
                        var reader = command.ExecuteReader();
                        if (reader.HasRows) // если есть данные
                        {
                            while (reader.Read()) // построчно считываем данные
                            {
                                SearchedThumbnails searchedThumbnails = new SearchedThumbnails();
                                searchedThumbnails.order_id = (Guid)reader.GetValue(0);
                                searchedThumbnails.shooting_datetime = (DateTime)reader.GetValue(1);
                                bool isLocked = (bool)reader.GetValue(2);
                                searchedThumbnails.thumbnail_cloud_id = (string)reader.GetValue(3);
                                searchedThumbnails.thumbnail_local_path = (string)reader.GetValue(4);

                                if (!isLocked)
                                {
                                    if (!legalPhotos.ContainsKey(searchedThumbnails.order_id)) legalPhotos.Add(searchedThumbnails.order_id, searchedThumbnails);
                                    if (!sortedOrderIds.Contains(searchedThumbnails.order_id)) sortedOrderIds.Add(searchedThumbnails.order_id);
                                }
                                searchedThumbnails = null;
                            }
                        }
                        reader.Close();
                    }

                    queryString = "";

                    if (legalPhotos.Count > 0)
                    {
                        // ШАГ второй - проверить наличие на легальных фото наличия персон с отпиской

                        #region ФОРМИРУЕМ СТРОКУ ЗАПРОСА 2
                        StringBuilder sb2 = new StringBuilder();
                        sb2.Append("SELECT order_id, x, y, w, h, a FROM Content WHERE locked = 0 and (");

                        for (int i = 0; i < sortedOrderIds.Count; i++) // перебираем лица в коллекции
                        {
                            sb2.Append(String.Format(" order_id='{0}'", sortedOrderIds[i]));
                            if (i < sortedOrderIds.Count - 1) sb2.Append(" or"); else sb2.Append(" );");
                        }
                        queryString = sb2.ToString(); sb2.Clear();
                        #endregion ФОРМИРУЕМ СТРОКУ ЗАПРОСА 2

                        using (SqlCommand command = new SqlCommand(queryString, connection))
                        {
                            if (connection.State != ConnectionState.Open) connection.Open();
                            var reader = command.ExecuteReader();

                            if (reader.HasRows) // если есть данные
                            {
                                while (reader.Read()) // построчно считываем данные
                                {
                                    Guid order_id = (Guid)reader.GetValue(0);
                                    float x = -1; object objX = reader.GetValue(1); if (objX != null) x = (float)objX;
                                    float y = -1; object objY = reader.GetValue(2); if (objY != null) y = (float)objY;
                                    float w = -1; object objW = reader.GetValue(3); if (objW != null) w = (float)objW;
                                    float h = -1; object objH = reader.GetValue(4); if (objH != null) h = (float)objH;
                                    Int16 a = 0; object objA = reader.GetValue(5); if (objA != null) a = (Int16)objA;
                                    if (x >= 0 && y >= 0 && w >= 0 && h >= 0) // признак наличия отписавшегося лица
                                    {
                                        if (legalPhotos.ContainsKey(order_id))
                                        {
                                            if (legalPhotos[order_id].rect == null) legalPhotos[order_id].rect = new List<RestrictedFaceArea>();
                                            legalPhotos[order_id].rect.Add(new RestrictedFaceArea() { x = x, y = y, w = w, h = h, a = a });
                                        }
                                    }
                                }
                            }
                            reader.Close();
                        }
                        queryString = null;
                    }
                }
            }

            if (legalPhotos.Count > 0)
            {
                ThumbnailsSearchGenerator thumbnailsSearchGenerator = new ThumbnailsSearchGenerator(legalPhotos, sortedOrderIds, Source.Cloud);
                allFacesIds = null;
                sortedOrderIds = null;
                legalPhotos = null;
                return thumbnailsSearchGenerator.Result;
            }
            else
            {
                allFacesIds = null;
                sortedOrderIds = null;
                legalPhotos = null;
                return null;
            }
        }


        public List<ContentData> InsertIntoContent(List<ContentData> data, int packetSize = 20)
        {
            DateTime dt1 = DateTime.Now;
            int requested = 0;
            int executed = 0;
            List<ContentData> lost = null;
            List<List<ContentData>> packet = new List<List<ContentData>>();

            for (int i = 0; i < data.Count; i++)
            {
                if (packet.Count == 0) packet.Add(new List<ContentData>());
                if (packet[packet.Count - 1].Count < packetSize) packet[packet.Count - 1].Add(data[i]);
                else { packet.Add(new List<ContentData>()); packet[packet.Count - 1].Add(data[i]); }
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                for (int p = 0; p < packet.Count; p++)
                {
                    StringBuilder sb = new StringBuilder("INSERT INTO Content (amazon_id, location_id, partner_id, order_id, shooting_datetime, ready_datetime, expiration_date, locked, x, y, w, h, a, width, height, size, face_cloud_id, thumbnail_cloud_id, photo_cloud_id, face_local_path, thumbnail_local_path, photo_local_path) VALUES ");
                    using (SqlCommand command = new SqlCommand())
                    {
                        for (int i = 0; i < packet[p].Count; i++)
                        {
                            sb.Append(String.Format("( @amazon_id{0}, @location_id{0}, @partner_id{0}, @order_id{0}, @shooting_datetime{0}, @ready_datetime{0}, @expiration_date{0}, @locked{0}, @x{0}, @y{0}, @w{0}, @h{0}, @a{0}," +
                                " @width{0}, @height{0}, @size{0}, @face_cloud_id{0}, @thumbnail_cloud_id{0}, @photo_cloud_id{0}, @face_local_path{0}, @thumbnail_local_path{0}, @photo_local_path{0} ),\r", i));
                            command.Parameters.Add("@amazon_id" + i.ToString(), SqlDbType.UniqueIdentifier).Value = packet[p][i].amazon_id;
                            command.Parameters.Add("@location_id" + i.ToString(), SqlDbType.SmallInt).Value = packet[p][i].location_id;
                            command.Parameters.Add("@partner_id" + i.ToString(), SqlDbType.SmallInt).Value = packet[p][i].partner_id;
                            command.Parameters.Add("@order_id" + i.ToString(), SqlDbType.UniqueIdentifier).Value = packet[p][i].order_id;
                            command.Parameters.Add("@shooting_datetime" + i.ToString(), SqlDbType.DateTimeOffset).Value = packet[p][i].shooting_datetime;
                            command.Parameters.Add("@ready_datetime" + i.ToString(), SqlDbType.DateTimeOffset).Value = packet[p][i].ready_datetime;
                            command.Parameters.Add("@expiration_date" + i.ToString(), SqlDbType.Date).Value = packet[p][i].expiration_date;
                            command.Parameters.Add("@locked" + i.ToString(), SqlDbType.Bit).Value = packet[p][i].locked;

                            command.Parameters.Add("@x" + i.ToString(), SqlDbType.Float).Value = (object)packet[p][i].x ?? DBNull.Value;
                            command.Parameters.Add("@y" + i.ToString(), SqlDbType.Float).Value = (object)packet[p][i].y ?? DBNull.Value;
                            command.Parameters.Add("@w" + i.ToString(), SqlDbType.Float).Value = (object)packet[p][i].w ?? DBNull.Value;
                            command.Parameters.Add("@h" + i.ToString(), SqlDbType.Float).Value = (object)packet[p][i].h ?? DBNull.Value;
                            command.Parameters.Add("@a" + i.ToString(), SqlDbType.SmallInt).Value = (object)packet[p][i].a ?? DBNull.Value;

                            command.Parameters.Add("@width" + i.ToString(), SqlDbType.SmallInt).Value = (object)packet[p][i].width ?? DBNull.Value;
                            command.Parameters.Add("@height" + i.ToString(), SqlDbType.SmallInt).Value = (object)packet[p][i].height ?? DBNull.Value;
                            command.Parameters.Add("@size" + i.ToString(), SqlDbType.Int).Value = (object)packet[p][i].size ?? DBNull.Value;

                            command.Parameters.Add("@face_cloud_id" + i.ToString(), SqlDbType.VarChar).Value = (object)packet[p][i].face_cloud_id ?? DBNull.Value;
                            command.Parameters.Add("@thumbnail_cloud_id" + i.ToString(), SqlDbType.VarChar).Value = (object)packet[p][i].thumbnail_cloud_id ?? DBNull.Value;
                            command.Parameters.Add("@photo_cloud_id" + i.ToString(), SqlDbType.VarChar).Value = (object)packet[p][i].photo_cloud_id ?? DBNull.Value;
                            command.Parameters.Add("@face_local_path" + i.ToString(), SqlDbType.VarChar).Value = (object)packet[p][i].face_local_path ?? DBNull.Value;
                            command.Parameters.Add("@thumbnail_local_path" + i.ToString(), SqlDbType.VarChar).Value = (object)packet[p][i].thumbnail_local_path ?? DBNull.Value;
                            command.Parameters.Add("@photo_local_path" + i.ToString(), SqlDbType.VarChar).Value = (object)packet[p][i].photo_local_path ?? DBNull.Value;
                            requested++;
                        }
                        string query = sb.Remove(sb.Length - 2, 2).ToString() + ";";
                        command.Connection = connection;
                        command.CommandText = query;

                        sb = null;

                        try
                        {
                            if (connection.State != ConnectionState.Open) connection.Open();
                            int result = command.ExecuteNonQuery();
                            executed = executed + packet[p].Count;
                        }
                        catch (Exception exc)
                        {
                            if (lost == null) lost = new List<ContentData>();
                            for (int i = 0; i < packet[p].Count; i++) lost.Add(packet[p][i]);

                            

                            // можно добавить попытки одиночной записи строк

                        }
                    }
                }
            }
            packet = null;
            DateTime dt2 = DateTime.Now;
            return lost;
        }


        /// <summary> Метод добавления нового заказа. Предварительная запись. Фактическое бронирование. </summary>
        /// <param name="orderNumber"> Назначенный системой уникальный идентификатор заказа. </param>
        /// <param name="orderDatetime"> Дата и время бронирования заказа. </param>
        /// <param name="contentIdList"> Список приобретаемых идентификаторов фотографий. </param>
        /// <param name="amount"> Стоимость заказа. </param>
        /// <param name="currency"> Тип валюты оплаты заказа. </param>
        /// <param name="description"> Детализированное описание заказа. </param>
        /// <param name="userId"> Идентификатор пользователя инициатора заказа. </param>
        /// <param name="userSession"> Данные пользовательской сессии инициатора заказа. </param>
        /// <returns></returns>
        public bool Orders_AddNewRecord(string orderNumber, DateTime orderDatetime, List<string> contentIdList, int amount, Int16 currency, string description, string userId, string userSession)
        {
            bool result = false;
            try
            {
                if (contentIdList != null && contentIdList.Count > 0)
                {
                    int count = 0;
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < contentIdList.Count; i++)
                    {
                        Guid guid;
                        if (contentIdList[i] != null && Guid.TryParse(contentIdList[i], out guid))
                        {
                            if (count > 0) sb.Append(";");
                            sb.Append(guid.ToString());
                            count++;
                        }
                    }
                    string contentList = sb.ToString();
                    sb = null;

                    if (!String.IsNullOrEmpty(contentList))
                    {
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            string query = "INSERT INTO Orders (order_number, order_datetime, content_id_list, amount, currency, description, user_id, user_session) VALUES ( @order_number, @order_datetime, @content_id_list, @amount, @currency, @description, @user_id, @user_session );";
                            using (SqlCommand command = new SqlCommand())
                            {
                                command.Parameters.Add("@order_number", SqlDbType.VarChar).Value = orderNumber;
                                command.Parameters.Add("@order_datetime", SqlDbType.DateTimeOffset).Value = orderDatetime;
                                command.Parameters.Add("@content_id_list", SqlDbType.VarChar).Value = contentList;
                                command.Parameters.Add("@amount", SqlDbType.Int).Value = amount;
                                command.Parameters.Add("@currency", SqlDbType.SmallInt).Value = currency;
                                command.Parameters.Add("@description", SqlDbType.NVarChar).Value = description;
                                command.Parameters.Add("@user_id", SqlDbType.VarChar).Value = userId;
                                command.Parameters.Add("@user_session", SqlDbType.VarChar).Value = userSession;

                                command.Connection = connection;
                                command.CommandText = query;

                                if (connection.State != ConnectionState.Open) connection.Open();
                                int res = command.ExecuteNonQuery();
                                if (res == 1) result = true;
                                if (connection.State == ConnectionState.Open) connection.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                // при дублировании записи ключа: Violation of PRIMARY KEY constraint 'PK__Orders__730E34DEE2F55DD2'. Cannot insert duplicate key in object 'photocoreuser.Orders'. The duplicate key value is (lebyzhiy_1234567890). The statement has been terminated.

            }
            return result;
        }

        /// <summary> Метод обновления заказа данными адресами доставки </summary>
        /// <param name="orderNumber"> Назначенный системой уникальный идентификатор заказа. </param>
        /// <param name="addressDeliveryUpdateDatetime"> Дата и время обновления. </param>
        /// <param name="mainEmail"> Основной E-mail доставки заказа. </param>
        /// <param name="backupEmail"> Резервный E-mail доставки заказа. </param>
        /// <param name="phone"> Номер телефона для доставки посредством мэссанджеров. </param>
        /// <param name="messangers"> Значение в битовой матрице списка используемых мэссанджеров для доставки контента. </param>
        /// <param name="bankOrderId"> Назначенный банком идентификатор платежа. </param>
        /// <returns></returns>
        public bool Orders_UpdateAddressAndOrderId(string orderNumber, DateTime addressDeliveryUpdateDatetime, string mainEmail, string backupEmail, string phone, Int16 messangers, string bankOrderId)
        {
            bool result = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Orders SET address_update_datetime='@address_update_datetime', email_1='@email_1', email_2='@email_2', phone='@phone', messagers='@messagers', order_id='@order_id' WHERE order_number='@order_number'";
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Parameters.Add("@address_update_datetime", SqlDbType.DateTimeOffset).Value = addressDeliveryUpdateDatetime;
                        command.Parameters.Add("@email_1", SqlDbType.VarChar).Value = mainEmail;
                        command.Parameters.Add("@email_2", SqlDbType.VarChar).Value = backupEmail;
                        command.Parameters.Add("@phone", SqlDbType.VarChar).Value = phone;
                        command.Parameters.Add("@messagers", SqlDbType.SmallInt).Value = messangers;
                        command.Parameters.Add("@order_id", SqlDbType.VarChar).Value = bankOrderId;
                        command.Parameters.Add("@order_number", SqlDbType.VarChar).Value = orderNumber;

                        command.Connection = connection;
                        command.CommandText = query;

                        if (connection.State != ConnectionState.Open) connection.Open();
                        int res = command.ExecuteNonQuery();
                        if (res == 0) result = true;
                        if (connection.State == ConnectionState.Open) connection.Close();
                    }
                }
            }
            catch (Exception exc)
            {
                // при дублировании записи ключа: Violation of PRIMARY KEY constraint 'PK__Orders__730E34DEE2F55DD2'. Cannot insert duplicate key in object 'photocoreuser.Orders'. The duplicate key value is (lebyzhiy_1234567890). The statement has been terminated.
            }
            return result;
        }

        /// <summary> Метод обновления данных заказа данными попытки платежа </summary>
        /// <param name="orderNumber"> Назначенный системой уникальный идентификатор заказа. </param>
        /// <param name="paymentResultUpdateDatetime"> Дата и время поступления информации о статусе платежа. </param>
        /// <param name="errorCode"> Код ошибки. </param>
        /// <param name="errorMessage"> Описание ошибки на языке, переданном в параметре Language в запросе. </param>
        /// <param name="orderStatus"> По значению этого параметра определяется состояние заказа в платежной системе. Список возможных значений приведен в таблице ниже. Отсутствует, если заказ не был найден. </param>
        /// <param name="pan">  Маскированный номер карты, которая использовалась для оплаты. Указан только после оплаты заказа. </param>
        /// <param name="expiration"> Срок истечения действия карты в формате YYYYMM. Указан только после оплаты заказа. </param>
        /// <param name="cardholderName"> Имя держателя карты. Указан только после оплаты заказа. </param>
        /// <param name="approvalCode"> Код авторизации МПС. Поле фиксированной длины(6 символов), может содержать цифры и латинские буквы. </param>
        /// <param name="authCode"> Это поле является устаревшим.Его значение всегда равно "2", независимо от состояния заказа и кода авторизации процессинговой системы. </param>
        /// <param name="ip"> IP адрес пользователя, который оплачивал заказ. </param>
        /// <returns></returns>
        public bool Orders_UpdatePaymentResult(string orderNumber, DateTime paymentResultUpdateDatetime, Int16 errorCode, string errorMessage, byte orderStatus, string pan, string expiration, string cardholderName, string approvalCode, string authCode, string ip)
        {
            bool result = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Orders SET payment_update_datetime='@payment_update_datetime', error_code='@error_code', error_message='@error_message', order_status='@order_status', pan='@pan', expiration='@expiration', cardholder_name='@cardholder_name', approval_code='@approval_code', auth_code='@auth_code', ip='@ip' WHERE order_number='@order_number'";
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Parameters.Add("@payment_update_datetime", SqlDbType.DateTimeOffset).Value = paymentResultUpdateDatetime;
                        command.Parameters.Add("@error_code", SqlDbType.SmallInt).Value = errorCode;
                        command.Parameters.Add("@error_message", SqlDbType.NVarChar).Value = errorMessage;
                        command.Parameters.Add("@order_status", SqlDbType.TinyInt).Value = orderStatus;
                        command.Parameters.Add("@pan", SqlDbType.VarChar).Value = pan;
                        command.Parameters.Add("@expiration", SqlDbType.VarChar).Value = expiration;
                        command.Parameters.Add("@cardholder_name", SqlDbType.VarChar).Value = cardholderName;
                        command.Parameters.Add("@approval_code", SqlDbType.VarChar).Value = approvalCode;
                        command.Parameters.Add("@auth_code", SqlDbType.VarChar).Value = authCode;
                        command.Parameters.Add("@ip", SqlDbType.VarChar).Value = ip;
                        command.Parameters.Add("@order_number", SqlDbType.VarChar).Value = orderNumber;

                        command.Connection = connection;
                        command.CommandText = query;

                        if (connection.State != ConnectionState.Open) connection.Open();
                        int res = command.ExecuteNonQuery();
                        if (res == 0) result = true;
                        if (connection.State == ConnectionState.Open) connection.Close();
                    }
                }
            }
            catch (Exception exc)
            {
            }
            return result;
        }

        /// <summary> Метод обновления данных заказа данными доставки </summary>
        /// <param name="orderNumber"> Назначенный системой уникальный идентификатор заказа. </param>
        /// <param name="deliverytResultUpdateDatetime">  Дата и время поступления информации о статусе доставки. </param>
        /// <param name="deliveryStatus"> Статус доставки заказа. </param>
        /// <returns></returns>
        public bool Orders_UpdateDeliveryResult(string orderNumber, DateTime deliveryResultUpdateDatetime, bool deliveryStatus)
        {
            bool result = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Orders SET delivery_update_datetime='@delivery_update_datetime', delivery_status='@delivery_status' WHERE order_number='@order_number'";
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Parameters.Add("@delivery_update_datetime", SqlDbType.DateTimeOffset).Value = deliveryResultUpdateDatetime;
                        command.Parameters.Add("@delivery_status", SqlDbType.Bit).Value = deliveryStatus;
                        command.Parameters.Add("@order_number", SqlDbType.VarChar).Value = orderNumber;

                        command.Connection = connection;
                        command.CommandText = query;

                        if (connection.State != ConnectionState.Open) connection.Open();
                        int res = command.ExecuteNonQuery();
                        if (res == 0) result = true;
                        if (connection.State == ConnectionState.Open) connection.Close();
                    }
                }
            }
            catch (Exception exc)
            {

            }
            return result;
        }


        public bool Orders_UpdateBankOrderIdInfo(string orderNumber, DateTime deliveryResultUpdateDatetime, bool deliveryStatus)
        {
            bool result = false;














            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Orders SET delivery_update_datetime='@delivery_update_datetime', delivery_status='@delivery_status' WHERE order_number='@order_number'";
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Parameters.Add("@delivery_update_datetime", SqlDbType.DateTimeOffset).Value = deliveryResultUpdateDatetime;
                        command.Parameters.Add("@delivery_status", SqlDbType.Bit).Value = deliveryStatus;
                        command.Parameters.Add("@order_number", SqlDbType.VarChar).Value = orderNumber;

                        command.Connection = connection;
                        command.CommandText = query;

                        if (connection.State != ConnectionState.Open) connection.Open();
                        int res = command.ExecuteNonQuery();
                        if (res == 0) result = true;
                        if (connection.State == ConnectionState.Open) connection.Close();
                    }
                }
            }
            catch (Exception exc)
            {

            }
            return result;
        }


        public bool Orders_GetReservedOrderInfo(string orderNumber, out int amount, out Int16 currency, out string description)
        {
            bool result = false;
            amount = 0; currency = 0; description = null;
            try
            {
                string queryString = String.Format("SELECT amount, currency, description FROM Orders WHERE order_number = {0};", orderNumber);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(queryString, connection))
                    {
                        if (connection.State != ConnectionState.Open) connection.Open();
                        var reader = command.ExecuteReader();
                        if (reader.HasRows) // если есть данные
                        {
                            while (reader.Read()) // построчно считываем данные
                            {
                                amount = (int)reader.GetValue(0);
                                currency = (Int16)reader.GetValue(1);
                                description = (string)reader.GetValue(2);
                            }
                            result = true;
                        }
                        reader.Close();
                    }
                }
            }
            catch (Exception exc)
            {

            }
            return result;
        }


        bool IsTableExist(string TableName, ref SqlConnection connection)
        {
            bool result = false;
            try
            {
                SqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = String.Format("select object_id('{0}')", TableName);
                object id = cmd.ExecuteScalar();
                if (id is System.Int32) return true;
                else if (id is System.DBNull) return false;
            }
            catch (Exception exc)
            {
                //log.Error(String.Format("IsTableExist. Main exception. Message=\"{0}\"", exc.Message));
            }
            return result;
        }


        /// <summary> Метод выборки миниатюр результатов поиска </summary>
        /// <param name="amazomIds"> Список списков похожих лиц на каждое отдельное поисковое лицо </param>
        /// <param name="locationId"> Местонахождение съёмки </param>
        /// <param name="dateMasks"> Список масок дат для поиска </param>
        /// <returns> Отсортированный по времени список миниатюр результатов поиска </returns>
        //public List<SearchedThumbnails> SelectFromContent(List<Guid>[] amazomIds, int locationId, List<string> dateMasks, Source source = Source.Cloud)
        //{
        //    List<Guid> allFacesIds = new List<Guid>(); // все идентификаторы лиц похожих на искомые
        //    Dictionary<Guid, SearchedThumbnails> legalPhotos = new Dictionary<Guid, SearchedThumbnails>();
        //    List<Guid> sortedOrderIds = new List<Guid>();

        //    #region формируем общий список уникальных лиц allFacesIds, избавляемся от возможных дублей
        //    if (amazomIds != null && amazomIds.Length > 0)
        //    {
        //        for (int f = 0; f < amazomIds.Length; f++) // перебираем коллекции
        //        {
        //            if (amazomIds[f] != null && amazomIds[f].Count > 0)
        //            {
        //                for (int i = 0; i < amazomIds[f].Count; i++) if (!allFacesIds.Contains(amazomIds[f][i])) allFacesIds.Add(amazomIds[f][i]);
        //            }
        //        }
        //    }
        //    #endregion формируем общий список уникальных лиц allFacesIds, избавляемся от возможных дублей

        //    if (dateMasks != null && dateMasks.Count > 0)
        //    {
        //        // избавляемся от потенциально неверных дат
        //        for (int i= dateMasks.Count - 1; i > -1; i--) if (dateMasks[i] == null && dateMasks[i].Length != 8) dateMasks.RemoveAt(i);
        //    }

        //    if (allFacesIds.Count > 0 && dateMasks != null && dateMasks.Count > 0)
        //    {
        //        // ШАГ первый - проверить поисковые лица на предмет присутствия отписанных лиц и изъять фотки с запретами.

        //        string queryString = "";
        //        #region ФОРМИРУЕМ СТРОКУ ЗАПРОСА 1
        //        StringBuilder sb1 = new StringBuilder();                
        //        sb1.Append(String.Format("SELECT order_id, shooting_datetime, locked, thumbnail_cloud_id, thumbnail_local_path FROM Content WHERE location_id = {0} and (", locationId));
        //        for (int i = 0; i < dateMasks.Count; i++)
        //        {
        //            sb1.Append(String.Format(" ( shooting_datetime between '{0}-{1}-{2}' and '{0}-{1}-{2} 23:59:59.999' )", dateMasks[i].Substring(0, 4), dateMasks[i].Substring(4, 2), dateMasks[i].Substring(6, 2)));
        //            if (i < dateMasks.Count - 1) sb1.Append(" or"); else sb1.Append(" ) and (");
        //        }
        //        for (int i = 0; i < allFacesIds.Count; i++) // перебираем лица в коллекции
        //        {
        //            sb1.Append(String.Format(" amazon_id='{0}'", allFacesIds[i]));
        //            if (i < allFacesIds.Count - 1) sb1.Append(" or"); else sb1.Append(" ) ORDER BY shooting_datetime ;");
        //        }
        //        queryString = sb1.ToString();  sb1.Clear();
        //        allFacesIds = null;
        //        #endregion ФОРМИРУЕМ СТРОКУ ЗАПРОСА 1

        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        {
        //            using (SqlCommand command = new SqlCommand(queryString, connection))
        //            {
        //                if (connection.State != ConnectionState.Open) connection.Open();
        //                var reader = command.ExecuteReader();
        //                if (reader.HasRows) // если есть данные
        //                {
        //                    while (reader.Read()) // построчно считываем данные
        //                    {
        //                        SearchedThumbnails searchedThumbnails = new SearchedThumbnails();
        //                        searchedThumbnails.order_id = (Guid)reader.GetValue(0);
        //                        searchedThumbnails.shooting_datetime = (DateTime)reader.GetValue(1);
        //                        bool isLocked = (bool)reader.GetValue(2);
        //                        searchedThumbnails.thumbnail_cloud_id = (string)reader.GetValue(3);
        //                        searchedThumbnails.thumbnail_local_path = (string)reader.GetValue(4);

        //                        if (!isLocked)
        //                        {
        //                            if (!legalPhotos.ContainsKey(searchedThumbnails.order_id)) legalPhotos.Add(searchedThumbnails.order_id, searchedThumbnails);
        //                            if (!sortedOrderIds.Contains(searchedThumbnails.order_id)) sortedOrderIds.Add(searchedThumbnails.order_id);
        //                        }
        //                        searchedThumbnails = null;
        //                    }
        //                }
        //                reader.Close();
        //            }

        //            queryString = "";

        //            if (legalPhotos.Count > 0)
        //            {
        //                // ШАГ второй - проверить наличие на легальных фото наличия персон с отпиской

        //                #region ФОРМИРУЕМ СТРОКУ ЗАПРОСА 2
        //                StringBuilder sb2 = new StringBuilder();
        //                sb2.Append("SELECT order_id, x, y, w, h, a FROM Content WHERE locked = 0 and (");

        //                for (int i = 0; i < sortedOrderIds.Count; i++) // перебираем лица в коллекции
        //                {
        //                    sb2.Append(String.Format(" order_id='{0}'", sortedOrderIds[i]));
        //                    if (i < sortedOrderIds.Count - 1) sb2.Append(" or"); else sb2.Append(" );");
        //                }
        //                queryString = sb2.ToString(); sb2.Clear();
        //                #endregion ФОРМИРУЕМ СТРОКУ ЗАПРОСА 2

        //                using (SqlCommand command = new SqlCommand(queryString, connection))
        //                {
        //                    if (connection.State != ConnectionState.Open) connection.Open();
        //                    var reader = command.ExecuteReader();

        //                    if (reader.HasRows) // если есть данные
        //                    {
        //                        while (reader.Read()) // построчно считываем данные
        //                        {
        //                            Guid order_id = (Guid)reader.GetValue(0);
        //                            //string thumbnail_cloud_id = (string)reader.GetValue(1);
        //                            float x = -1;  object objX = reader.GetValue(1);  if (objX != null) x = (float)objX;
        //                            float y = -1;  object objY = reader.GetValue(2);  if (objY != null) y = (float)objY;
        //                            float w = -1;  object objW = reader.GetValue(3);  if (objW != null) w = (float)objW;
        //                            float h = -1;  object objH = reader.GetValue(4);  if (objH != null) h = (float)objH;
        //                            Int16 a = 0;   object objA = reader.GetValue(5);  if (objA != null) a = (Int16)objA;
        //                            if (x >= 0 && y >= 0 && w >= 0 && h >= 0) // признак наличия отписавшегося лица
        //                            {
        //                                if (legalPhotos.ContainsKey(order_id))
        //                                {
        //                                    if (legalPhotos[order_id].rect == null) legalPhotos[order_id].rect = new List<RestrictedFaceArea>();
        //                                    legalPhotos[order_id].rect.Add(new RestrictedFaceArea() { x = x, y = y, w = w, h = h, a = a });
        //                                }
        //                            }
        //                        }
        //                    }
        //                    reader.Close();
        //                }
        //                queryString = null;
        //            }
        //        }
        //    }

        //    if (legalPhotos.Count == 0) // фотки не найдены или их отображение запрещено
        //    {
        //        sortedOrderIds = null;
        //        return new List<SearchedThumbnails>();
        //    }
        //    else
        //    {
        //        if (sortedOrderIds != null && sortedOrderIds.Count > 0)
        //        {
        //            List<SearchedThumbnails> result = new List<SearchedThumbnails>();

        //            for (int i = 0; i < sortedOrderIds.Count; i++)
        //            {
        //                // сюда надо записать извлечение миниатюры и наложение размытости на отписавшиеся лица
        //                if (legalPhotos.ContainsKey(sortedOrderIds[i]))
        //                {
        //                    if (legalPhotos[sortedOrderIds[i]].rect != null && legalPhotos[sortedOrderIds[i]].rect.Count > 0) // здесь нужна замутовка лиц
        //                    {
        //                        if (source == Source.Local)
        //                        {
        //                            BlurFace blurFace = new BlurFace(legalPhotos[sortedOrderIds[i]].thumbnail_local_path, legalPhotos[sortedOrderIds[i]].rect, 95);
        //                        }


        //                    }
        //                    else // замутовка лиц не нужна
        //                    {
        //                        if (source == Source.Cloud && legalPhotos[sortedOrderIds[i]].thumbnail_cloud_id != null && legalPhotos[sortedOrderIds[i]].thumbnail_cloud_id != "")
        //                        {
        //                            // загружаем с облака



        //                        }
        //                        else if (source == Source.Local && legalPhotos[sortedOrderIds[i]].thumbnail_local_path != null && System.IO.File.Exists(legalPhotos[sortedOrderIds[i]].thumbnail_local_path))
        //                        {
        //                            // загружаем с локального хранилища
        //                            int retries = 5;
        //                            const int delay = 100;
        //                            bool fileIsReady = false;
        //                            FileStream input = null;
        //                            do //https://stackoverflow.com/questions/21739242/filestream-and-a-filesystemwatcher-in-c-weird-issue-process-cannot-access-the
        //                            {
        //                                try
        //                                {
        //                                    input = File.OpenRead(legalPhotos[sortedOrderIds[i]].thumbnail_local_path);
        //                                    fileIsReady = true; // success
        //                                }
        //                                catch (IOException) { Console.WriteLine("Ooops"); }
        //                                retries--;
        //                                if (!fileIsReady) Thread.Sleep(delay);
        //                            }
        //                            while (!fileIsReady && retries > 0);

        //                            if (input != null)
        //                            {
        //                                using (var inputStream = new SKManagedStream(input))
        //                                using (var codec = SKCodec.Create(inputStream))
        //                                using (var original = SKBitmap.Decode(codec))
        //                                {


        //                                }
        //                            }
        //                        }
        //                    }

        //                    result.Add(legalPhotos[sortedOrderIds[i]]);
        //                }
        //            }
        //            sortedOrderIds = null;
        //            return result;
        //        }
        //        else
        //        {
        //            sortedOrderIds = null;
        //            return new List<SearchedThumbnails>();
        //        }
        //    }
        //}


        //public bool InsertIntoOrders(OrderReservation data)
        //{
        //    bool result = false;
        //    try
        //    {
        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        {
        //            string query = "INSERT INTO Orders (order_number, order_datetime, content_id_list, amount, currency, description, user_id, user_session) VALUES ( @order_number, @order_datetime, @content_id_list, @amount, @currency, @description, @user_id, @user_session );";
        //            using (SqlCommand command = new SqlCommand())
        //            {
        //                command.Parameters.Add("@order_number", SqlDbType.VarChar).Value = data.orderNumber;
        //                command.Parameters.Add("@order_datetime", SqlDbType.DateTimeOffset).Value = data.orderDatetime;
        //                command.Parameters.Add("@content_id_list", SqlDbType.VarChar).Value = data.contentIdList;
        //                command.Parameters.Add("@amount", SqlDbType.Int).Value = data.amount;
        //                command.Parameters.Add("@currency", SqlDbType.SmallInt).Value = data.currency;
        //                command.Parameters.Add("@description", SqlDbType.NVarChar).Value = data.description;
        //                command.Parameters.Add("@user_id", SqlDbType.VarChar).Value = data.userId;
        //                command.Parameters.Add("@user_session", SqlDbType.VarChar).Value = data.userSession;

        //                command.Connection = connection;
        //                command.CommandText = query;

        //                if (connection.State != ConnectionState.Open) connection.Open();
        //                int res = command.ExecuteNonQuery();
        //                if (res == 1) result = true;
        //                if (connection.State == ConnectionState.Open) connection.Close();
        //            }
        //        }
        //    }
        //    catch (Exception exc)
        //    {
        //        // при дублировании записи ключа: Violation of PRIMARY KEY constraint 'PK__Orders__730E34DEE2F55DD2'. Cannot insert duplicate key in object 'photocoreuser.Orders'. The duplicate key value is (lebyzhiy_1234567890). The statement has been terminated.

        //    }
        //    return result;
        //}

        //public bool UpdateOrders(OrderAddressUpdate data)
        //{
        //    bool result = false;
        //    try
        //    {
        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        {
        //            string query = "UPDATE Orders SET address_update_datetime='@address_update_datetime', email_1='@email_1', email_2='@email_2', phone='@phone', messagers='@messagers', order_id='@order_id' WHERE order_number='@order_number'";
        //            using (SqlCommand command = new SqlCommand())
        //            {
        //                command.Parameters.Add("@address_update_datetime", SqlDbType.DateTimeOffset).Value = data.addressUpdateDatetime;
        //                command.Parameters.Add("@email_1", SqlDbType.VarChar).Value = data.email1;
        //                command.Parameters.Add("@email_2", SqlDbType.VarChar).Value = data.email2;
        //                command.Parameters.Add("@phone", SqlDbType.VarChar).Value = data.phone;
        //                command.Parameters.Add("@messagers", SqlDbType.SmallInt).Value = data.messagers;
        //                command.Parameters.Add("@order_id", SqlDbType.VarChar).Value = data.orderId;
        //                command.Parameters.Add("@order_number", SqlDbType.VarChar).Value = data.orderNumber;

        //                command.Connection = connection;
        //                command.CommandText = query;

        //                if (connection.State != ConnectionState.Open) connection.Open();
        //                int res = command.ExecuteNonQuery();
        //                if (res == 0) result = true;
        //                if (connection.State == ConnectionState.Open) connection.Close();
        //            }
        //        }
        //    }
        //    catch (Exception exc)
        //    {
        //    }
        //    return result;
        //}

        //public bool UpdateOrders(OrderPaymentUpdate data)
        //{
        //    bool result = false;
        //    try
        //    {
        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        {
        //            string query = "UPDATE Orders SET payment_update_datetime='@payment_update_datetime', error_code='@error_code', error_message='@error_message', order_status='@order_status', pan='@pan', expiration='@expiration', cardholder_name='@cardholder_name', approval_code='@approval_code', auth_code='@auth_code', ip='@ip' WHERE order_number='@order_number'";
        //            using (SqlCommand command = new SqlCommand())
        //            {
        //                command.Parameters.Add("@payment_update_datetime", SqlDbType.DateTimeOffset).Value = data.paymentUpdateDatetime;
        //                command.Parameters.Add("@error_code", SqlDbType.SmallInt).Value = data.errorCode;
        //                command.Parameters.Add("@error_message", SqlDbType.NVarChar).Value = data.errorMessage;
        //                command.Parameters.Add("@order_status", SqlDbType.TinyInt).Value = data.orderStatus;
        //                command.Parameters.Add("@pan", SqlDbType.VarChar).Value = data.pan;
        //                command.Parameters.Add("@expiration", SqlDbType.VarChar).Value = data.expiration;
        //                command.Parameters.Add("@cardholder_name", SqlDbType.VarChar).Value = data.cardholderName;
        //                command.Parameters.Add("@approval_code", SqlDbType.VarChar).Value = data.approvalCode;
        //                command.Parameters.Add("@auth_code", SqlDbType.VarChar).Value = data.authCode;
        //                command.Parameters.Add("@ip", SqlDbType.VarChar).Value = data.ip;
        //                command.Parameters.Add("@order_number", SqlDbType.VarChar).Value = data.orderNumber;

        //                command.Connection = connection;
        //                command.CommandText = query;

        //                if (connection.State != ConnectionState.Open) connection.Open();
        //                int res = command.ExecuteNonQuery();
        //                if (res == 0) result = true;
        //                if (connection.State == ConnectionState.Open) connection.Close();
        //            }
        //        }
        //    }
        //    catch (Exception exc)
        //    {
        //        // при дублировании записи ключа: Violation of PRIMARY KEY constraint 'PK__Orders__730E34DEE2F55DD2'. Cannot insert duplicate key in object 'photocoreuser.Orders'. The duplicate key value is (lebyzhiy_1234567890). The statement has been terminated.

        //    }
        //    return result;
        //}

        //public bool UpdateOrders(OrderDeliveryUpdate data)
        //{
        //    bool result = false;
        //    try
        //    {
        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        {
        //            string query = "UPDATE Orders SET delivery_update_datetime='@delivery_update_datetime', delivery_status='@delivery_status' WHERE order_number='@order_number'";
        //            using (SqlCommand command = new SqlCommand())
        //            {
        //                command.Parameters.Add("@delivery_update_datetime", SqlDbType.DateTimeOffset).Value = data.deliveryUpdateDatetime;
        //                command.Parameters.Add("@delivery_status", SqlDbType.Bit).Value = data.deliveryStatus;
        //                command.Parameters.Add("@order_number", SqlDbType.VarChar).Value = data.orderNumber;

        //                command.Connection = connection;
        //                command.CommandText = query;

        //                if (connection.State != ConnectionState.Open) connection.Open();
        //                int res = command.ExecuteNonQuery();
        //                if (res == 0) result = true;
        //                if (connection.State == ConnectionState.Open) connection.Close();
        //            }
        //        }
        //    }
        //    catch (Exception exc)
        //    {
        //        // при дублировании записи ключа: Violation of PRIMARY KEY constraint 'PK__Orders__730E34DEE2F55DD2'. Cannot insert duplicate key in object 'photocoreuser.Orders'. The duplicate key value is (lebyzhiy_1234567890). The statement has been terminated.

        //    }
        //    return result;
        //}

    }
}
