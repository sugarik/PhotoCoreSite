namespace TestLab
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_TestBlurFace = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btn_SearchFaces = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btn_SearchPhotos = new System.Windows.Forms.Button();
            this.btn_CreateContent = new System.Windows.Forms.Button();
            this.btn_InsertContent = new System.Windows.Forms.Button();
            this.btn_SelectContent = new System.Windows.Forms.Button();
            this.btn_DeleteContent = new System.Windows.Forms.Button();
            this.btn_TotalRecordsContent = new System.Windows.Forms.Button();
            this.btn_CreateAllDatabases = new System.Windows.Forms.Button();
            this.btn_DropAllDatabases = new System.Windows.Forms.Button();
            this.btn_DatebaseRowCount = new System.Windows.Forms.Button();
            this.btn_RegisterDo = new System.Windows.Forms.Button();
            this.l_mdOrder = new System.Windows.Forms.Label();
            this.tb_mdOrder = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.btn_UpdateOrdersDelivery = new System.Windows.Forms.Button();
            this.btn_InsertOrders = new System.Windows.Forms.Button();
            this.btn_UpdateOrdersPaymentSucc = new System.Windows.Forms.Button();
            this.btn_UpdateOrdersPaymentNotSucc = new System.Windows.Forms.Button();
            this.btn_UpdateOrdersAddress = new System.Windows.Forms.Button();
            this.btn_CreateOrders = new System.Windows.Forms.Button();
            this.btn_DeleteOrders = new System.Windows.Forms.Button();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.btn_OrderReservation = new System.Windows.Forms.Button();
            this.btn_Register = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_TestBlurFace
            // 
            this.btn_TestBlurFace.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_TestBlurFace.ForeColor = System.Drawing.Color.Black;
            this.btn_TestBlurFace.Location = new System.Drawing.Point(15, 27);
            this.btn_TestBlurFace.Name = "btn_TestBlurFace";
            this.btn_TestBlurFace.Size = new System.Drawing.Size(263, 29);
            this.btn_TestBlurFace.TabIndex = 0;
            this.btn_TestBlurFace.Text = "Тестирование сокрытия лица";
            this.btn_TestBlurFace.UseVisualStyleBackColor = true;
            this.btn_TestBlurFace.Click += new System.EventHandler(this.btn_TestBlurFace_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btn_TestBlurFace);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox1.ForeColor = System.Drawing.Color.Blue;
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(294, 74);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Модуль BlurFace";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btn_SearchFaces);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox2.ForeColor = System.Drawing.Color.Blue;
            this.groupBox2.Location = new System.Drawing.Point(324, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(294, 74);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Модуль SearchFaces";
            // 
            // btn_SearchFaces
            // 
            this.btn_SearchFaces.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_SearchFaces.ForeColor = System.Drawing.Color.Black;
            this.btn_SearchFaces.Location = new System.Drawing.Point(19, 27);
            this.btn_SearchFaces.Name = "btn_SearchFaces";
            this.btn_SearchFaces.Size = new System.Drawing.Size(259, 29);
            this.btn_SearchFaces.TabIndex = 1;
            this.btn_SearchFaces.Text = "Тестирование поиска лица";
            this.btn_SearchFaces.UseVisualStyleBackColor = true;
            this.btn_SearchFaces.Click += new System.EventHandler(this.btn_SearchFaces_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btn_SearchPhotos);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox3.ForeColor = System.Drawing.Color.Blue;
            this.groupBox3.Location = new System.Drawing.Point(654, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(295, 74);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Модуль SearchPhotos";
            // 
            // btn_SearchPhotos
            // 
            this.btn_SearchPhotos.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_SearchPhotos.ForeColor = System.Drawing.Color.Black;
            this.btn_SearchPhotos.Location = new System.Drawing.Point(15, 27);
            this.btn_SearchPhotos.Name = "btn_SearchPhotos";
            this.btn_SearchPhotos.Size = new System.Drawing.Size(263, 29);
            this.btn_SearchPhotos.TabIndex = 0;
            this.btn_SearchPhotos.Text = "Тестирование поиска фотографий";
            this.btn_SearchPhotos.UseVisualStyleBackColor = true;
            // 
            // btn_CreateContent
            // 
            this.btn_CreateContent.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_CreateContent.ForeColor = System.Drawing.Color.Black;
            this.btn_CreateContent.Location = new System.Drawing.Point(15, 29);
            this.btn_CreateContent.Name = "btn_CreateContent";
            this.btn_CreateContent.Size = new System.Drawing.Size(199, 47);
            this.btn_CreateContent.TabIndex = 4;
            this.btn_CreateContent.Text = "Создать таблицу";
            this.btn_CreateContent.UseVisualStyleBackColor = true;
            this.btn_CreateContent.Click += new System.EventHandler(this.btn_CreateContent_Click);
            // 
            // btn_InsertContent
            // 
            this.btn_InsertContent.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_InsertContent.ForeColor = System.Drawing.Color.Black;
            this.btn_InsertContent.Location = new System.Drawing.Point(218, 29);
            this.btn_InsertContent.Name = "btn_InsertContent";
            this.btn_InsertContent.Size = new System.Drawing.Size(199, 47);
            this.btn_InsertContent.TabIndex = 5;
            this.btn_InsertContent.Text = "Добавить запись";
            this.btn_InsertContent.UseVisualStyleBackColor = true;
            this.btn_InsertContent.Click += new System.EventHandler(this.btn_InsertContent_Click);
            // 
            // btn_SelectContent
            // 
            this.btn_SelectContent.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_SelectContent.ForeColor = System.Drawing.Color.Black;
            this.btn_SelectContent.Location = new System.Drawing.Point(628, 29);
            this.btn_SelectContent.Name = "btn_SelectContent";
            this.btn_SelectContent.Size = new System.Drawing.Size(199, 47);
            this.btn_SelectContent.TabIndex = 6;
            this.btn_SelectContent.Text = "Выбрать записи из Content\r\nSELECT";
            this.btn_SelectContent.UseVisualStyleBackColor = true;
            this.btn_SelectContent.Click += new System.EventHandler(this.btn_SelectContent_Click);
            // 
            // btn_DeleteContent
            // 
            this.btn_DeleteContent.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_DeleteContent.ForeColor = System.Drawing.Color.Black;
            this.btn_DeleteContent.Location = new System.Drawing.Point(833, 29);
            this.btn_DeleteContent.Name = "btn_DeleteContent";
            this.btn_DeleteContent.Size = new System.Drawing.Size(199, 47);
            this.btn_DeleteContent.TabIndex = 7;
            this.btn_DeleteContent.Text = "Удалить таблицу Content\r\nDROP";
            this.btn_DeleteContent.UseVisualStyleBackColor = true;
            this.btn_DeleteContent.Click += new System.EventHandler(this.btn_DeleteContent_Click);
            // 
            // btn_TotalRecordsContent
            // 
            this.btn_TotalRecordsContent.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_TotalRecordsContent.ForeColor = System.Drawing.Color.Black;
            this.btn_TotalRecordsContent.Location = new System.Drawing.Point(423, 29);
            this.btn_TotalRecordsContent.Name = "btn_TotalRecordsContent";
            this.btn_TotalRecordsContent.Size = new System.Drawing.Size(199, 47);
            this.btn_TotalRecordsContent.TabIndex = 9;
            this.btn_TotalRecordsContent.Text = "Посчитать записи Content\r\nSELECT";
            this.btn_TotalRecordsContent.UseVisualStyleBackColor = true;
            this.btn_TotalRecordsContent.Click += new System.EventHandler(this.btn_TotalRecordsContent_Click);
            // 
            // btn_CreateAllDatabases
            // 
            this.btn_CreateAllDatabases.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_CreateAllDatabases.ForeColor = System.Drawing.Color.Black;
            this.btn_CreateAllDatabases.Location = new System.Drawing.Point(15, 21);
            this.btn_CreateAllDatabases.Name = "btn_CreateAllDatabases";
            this.btn_CreateAllDatabases.Size = new System.Drawing.Size(199, 47);
            this.btn_CreateAllDatabases.TabIndex = 10;
            this.btn_CreateAllDatabases.Text = "Создать все таблицы базы";
            this.btn_CreateAllDatabases.UseVisualStyleBackColor = true;
            this.btn_CreateAllDatabases.Click += new System.EventHandler(this.btn_CreateAllTables_Click);
            // 
            // btn_DropAllDatabases
            // 
            this.btn_DropAllDatabases.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_DropAllDatabases.ForeColor = System.Drawing.Color.Black;
            this.btn_DropAllDatabases.Location = new System.Drawing.Point(220, 21);
            this.btn_DropAllDatabases.Name = "btn_DropAllDatabases";
            this.btn_DropAllDatabases.Size = new System.Drawing.Size(197, 47);
            this.btn_DropAllDatabases.TabIndex = 11;
            this.btn_DropAllDatabases.Text = "Удалить все таблицы базы";
            this.btn_DropAllDatabases.UseVisualStyleBackColor = true;
            this.btn_DropAllDatabases.Click += new System.EventHandler(this.btn_DropAllTables_Click);
            // 
            // btn_DatebaseRowCount
            // 
            this.btn_DatebaseRowCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_DatebaseRowCount.ForeColor = System.Drawing.Color.Black;
            this.btn_DatebaseRowCount.Location = new System.Drawing.Point(423, 21);
            this.btn_DatebaseRowCount.Name = "btn_DatebaseRowCount";
            this.btn_DatebaseRowCount.Size = new System.Drawing.Size(199, 47);
            this.btn_DatebaseRowCount.TabIndex = 12;
            this.btn_DatebaseRowCount.Text = "Подсчитать строки таблиц базы";
            this.btn_DatebaseRowCount.UseVisualStyleBackColor = true;
            this.btn_DatebaseRowCount.Click += new System.EventHandler(this.btn_TableRowCount_Click);
            // 
            // btn_RegisterDo
            // 
            this.btn_RegisterDo.Location = new System.Drawing.Point(376, 25);
            this.btn_RegisterDo.Name = "btn_RegisterDo";
            this.btn_RegisterDo.Size = new System.Drawing.Size(231, 23);
            this.btn_RegisterDo.TabIndex = 1;
            this.btn_RegisterDo.Text = "RegisterDo";
            this.btn_RegisterDo.UseVisualStyleBackColor = true;
            this.btn_RegisterDo.Click += new System.EventHandler(this.button1_Click);
            // 
            // l_mdOrder
            // 
            this.l_mdOrder.AutoSize = true;
            this.l_mdOrder.Location = new System.Drawing.Point(376, 55);
            this.l_mdOrder.Name = "l_mdOrder";
            this.l_mdOrder.Size = new System.Drawing.Size(53, 13);
            this.l_mdOrder.TabIndex = 2;
            this.l_mdOrder.Text = "mdOrder=";
            // 
            // tb_mdOrder
            // 
            this.tb_mdOrder.Location = new System.Drawing.Point(426, 52);
            this.tb_mdOrder.Name = "tb_mdOrder";
            this.tb_mdOrder.Size = new System.Drawing.Size(181, 20);
            this.tb_mdOrder.TabIndex = 3;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btn_CreateContent);
            this.groupBox4.Controls.Add(this.btn_InsertContent);
            this.groupBox4.Controls.Add(this.btn_SelectContent);
            this.groupBox4.Controls.Add(this.btn_DeleteContent);
            this.groupBox4.Controls.Add(this.btn_TotalRecordsContent);
            this.groupBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox4.ForeColor = System.Drawing.Color.Blue;
            this.groupBox4.Location = new System.Drawing.Point(12, 247);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(1047, 93);
            this.groupBox4.TabIndex = 15;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Операции с таблицей Content";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.btn_CreateAllDatabases);
            this.groupBox5.Controls.Add(this.btn_DropAllDatabases);
            this.groupBox5.Controls.Add(this.btn_DatebaseRowCount);
            this.groupBox5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox5.ForeColor = System.Drawing.Color.Blue;
            this.groupBox5.Location = new System.Drawing.Point(12, 138);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(1047, 83);
            this.groupBox5.TabIndex = 16;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Операции с базой";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.btn_UpdateOrdersDelivery);
            this.groupBox6.Controls.Add(this.btn_InsertOrders);
            this.groupBox6.Controls.Add(this.btn_UpdateOrdersPaymentSucc);
            this.groupBox6.Controls.Add(this.btn_UpdateOrdersPaymentNotSucc);
            this.groupBox6.Controls.Add(this.btn_UpdateOrdersAddress);
            this.groupBox6.Controls.Add(this.btn_CreateOrders);
            this.groupBox6.Controls.Add(this.btn_DeleteOrders);
            this.groupBox6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox6.ForeColor = System.Drawing.Color.Blue;
            this.groupBox6.Location = new System.Drawing.Point(12, 375);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(1047, 193);
            this.groupBox6.TabIndex = 17;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Операции с таблицей Orders";
            // 
            // btn_UpdateOrdersDelivery
            // 
            this.btn_UpdateOrdersDelivery.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_UpdateOrdersDelivery.ForeColor = System.Drawing.Color.Black;
            this.btn_UpdateOrdersDelivery.Location = new System.Drawing.Point(628, 135);
            this.btn_UpdateOrdersDelivery.Name = "btn_UpdateOrdersDelivery";
            this.btn_UpdateOrdersDelivery.Size = new System.Drawing.Size(199, 47);
            this.btn_UpdateOrdersDelivery.TabIndex = 14;
            this.btn_UpdateOrdersDelivery.Text = "Обновить данными результата доставки";
            this.btn_UpdateOrdersDelivery.UseVisualStyleBackColor = true;
            this.btn_UpdateOrdersDelivery.Click += new System.EventHandler(this.btn_UpdateOrdersDelivery_Click);
            // 
            // btn_InsertOrders
            // 
            this.btn_InsertOrders.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_InsertOrders.ForeColor = System.Drawing.Color.Black;
            this.btn_InsertOrders.Location = new System.Drawing.Point(218, 29);
            this.btn_InsertOrders.Name = "btn_InsertOrders";
            this.btn_InsertOrders.Size = new System.Drawing.Size(199, 47);
            this.btn_InsertOrders.TabIndex = 13;
            this.btn_InsertOrders.Text = "Бронирование заказа\r\n(без данных доставки)";
            this.btn_InsertOrders.UseVisualStyleBackColor = true;
            this.btn_InsertOrders.Click += new System.EventHandler(this.btn_InsertOrders_Click_1);
            // 
            // btn_UpdateOrdersPaymentSucc
            // 
            this.btn_UpdateOrdersPaymentSucc.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_UpdateOrdersPaymentSucc.ForeColor = System.Drawing.Color.Black;
            this.btn_UpdateOrdersPaymentSucc.Location = new System.Drawing.Point(628, 82);
            this.btn_UpdateOrdersPaymentSucc.Name = "btn_UpdateOrdersPaymentSucc";
            this.btn_UpdateOrdersPaymentSucc.Size = new System.Drawing.Size(199, 47);
            this.btn_UpdateOrdersPaymentSucc.TabIndex = 12;
            this.btn_UpdateOrdersPaymentSucc.Text = "Обновить данными успешной оплаты";
            this.btn_UpdateOrdersPaymentSucc.UseVisualStyleBackColor = true;
            this.btn_UpdateOrdersPaymentSucc.Click += new System.EventHandler(this.btn_UpdateOrdersPaymentSucc_Click);
            // 
            // btn_UpdateOrdersPaymentNotSucc
            // 
            this.btn_UpdateOrdersPaymentNotSucc.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_UpdateOrdersPaymentNotSucc.ForeColor = System.Drawing.Color.Black;
            this.btn_UpdateOrdersPaymentNotSucc.Location = new System.Drawing.Point(628, 29);
            this.btn_UpdateOrdersPaymentNotSucc.Name = "btn_UpdateOrdersPaymentNotSucc";
            this.btn_UpdateOrdersPaymentNotSucc.Size = new System.Drawing.Size(199, 47);
            this.btn_UpdateOrdersPaymentNotSucc.TabIndex = 11;
            this.btn_UpdateOrdersPaymentNotSucc.Text = "Обновить данными неуспешной оплаты";
            this.btn_UpdateOrdersPaymentNotSucc.UseVisualStyleBackColor = true;
            this.btn_UpdateOrdersPaymentNotSucc.Click += new System.EventHandler(this.btn_UpdateOrdersPaymentNotSucc_Click);
            // 
            // btn_UpdateOrdersAddress
            // 
            this.btn_UpdateOrdersAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_UpdateOrdersAddress.ForeColor = System.Drawing.Color.Black;
            this.btn_UpdateOrdersAddress.Location = new System.Drawing.Point(423, 29);
            this.btn_UpdateOrdersAddress.Name = "btn_UpdateOrdersAddress";
            this.btn_UpdateOrdersAddress.Size = new System.Drawing.Size(199, 47);
            this.btn_UpdateOrdersAddress.TabIndex = 10;
            this.btn_UpdateOrdersAddress.Text = "Обновить данными адреса доставки";
            this.btn_UpdateOrdersAddress.UseVisualStyleBackColor = true;
            this.btn_UpdateOrdersAddress.Click += new System.EventHandler(this.btn_UpdateOrdersAddress_Click);
            // 
            // btn_CreateOrders
            // 
            this.btn_CreateOrders.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_CreateOrders.ForeColor = System.Drawing.Color.Black;
            this.btn_CreateOrders.Location = new System.Drawing.Point(15, 29);
            this.btn_CreateOrders.Name = "btn_CreateOrders";
            this.btn_CreateOrders.Size = new System.Drawing.Size(199, 47);
            this.btn_CreateOrders.TabIndex = 4;
            this.btn_CreateOrders.Text = "Создать таблицу";
            this.btn_CreateOrders.UseVisualStyleBackColor = true;
            this.btn_CreateOrders.Click += new System.EventHandler(this.btn_CreateOrders_Click);
            // 
            // btn_DeleteOrders
            // 
            this.btn_DeleteOrders.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_DeleteOrders.ForeColor = System.Drawing.Color.Black;
            this.btn_DeleteOrders.Location = new System.Drawing.Point(833, 29);
            this.btn_DeleteOrders.Name = "btn_DeleteOrders";
            this.btn_DeleteOrders.Size = new System.Drawing.Size(199, 47);
            this.btn_DeleteOrders.TabIndex = 7;
            this.btn_DeleteOrders.Text = "Удалить таблицу";
            this.btn_DeleteOrders.UseVisualStyleBackColor = true;
            this.btn_DeleteOrders.Click += new System.EventHandler(this.btn_DeleteOrders_Click);
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.btn_Register);
            this.groupBox7.Controls.Add(this.btn_OrderReservation);
            this.groupBox7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox7.ForeColor = System.Drawing.Color.Blue;
            this.groupBox7.Location = new System.Drawing.Point(1097, 12);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(295, 266);
            this.groupBox7.TabIndex = 18;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Модуль OrderReservation";
            // 
            // btn_OrderReservation
            // 
            this.btn_OrderReservation.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_OrderReservation.ForeColor = System.Drawing.Color.Black;
            this.btn_OrderReservation.Location = new System.Drawing.Point(15, 27);
            this.btn_OrderReservation.Name = "btn_OrderReservation";
            this.btn_OrderReservation.Size = new System.Drawing.Size(263, 29);
            this.btn_OrderReservation.TabIndex = 0;
            this.btn_OrderReservation.Text = "Тестирование заказа";
            this.btn_OrderReservation.UseVisualStyleBackColor = true;
            this.btn_OrderReservation.Click += new System.EventHandler(this.btn_OrderReservation_Click);
            // 
            // btn_Register
            // 
            this.btn_Register.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_Register.ForeColor = System.Drawing.Color.Black;
            this.btn_Register.Location = new System.Drawing.Point(15, 76);
            this.btn_Register.Name = "btn_Register";
            this.btn_Register.Size = new System.Drawing.Size(263, 29);
            this.btn_Register.TabIndex = 1;
            this.btn_Register.Text = "Тестирование Register";
            this.btn_Register.UseVisualStyleBackColor = true;
            this.btn_Register.Click += new System.EventHandler(this.btn_Register_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1444, 605);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.tb_mdOrder);
            this.Controls.Add(this.l_mdOrder);
            this.Controls.Add(this.btn_RegisterDo);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_RegisterDo;
        private System.Windows.Forms.Label l_mdOrder;
        private System.Windows.Forms.TextBox tb_mdOrder;

        private System.Windows.Forms.Button btn_TestBlurFace;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btn_SearchFaces;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btn_SearchPhotos;

        private System.Windows.Forms.Button btn_CreateContent;
        private System.Windows.Forms.Button btn_InsertContent;
        private System.Windows.Forms.Button btn_SelectContent;
        private System.Windows.Forms.Button btn_DeleteContent;
        private System.Windows.Forms.Button btn_TotalRecordsContent;
        private System.Windows.Forms.Button btn_CreateAllDatabases;
        private System.Windows.Forms.Button btn_DropAllDatabases;
        private System.Windows.Forms.Button btn_DatebaseRowCount;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Button btn_CreateOrders;
        private System.Windows.Forms.Button btn_DeleteOrders;
        private System.Windows.Forms.Button btn_UpdateOrdersPaymentSucc;
        private System.Windows.Forms.Button btn_UpdateOrdersPaymentNotSucc;
        private System.Windows.Forms.Button btn_UpdateOrdersAddress;
        private System.Windows.Forms.Button btn_InsertOrders;
        private System.Windows.Forms.Button btn_UpdateOrdersDelivery;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Button btn_OrderReservation;
        private System.Windows.Forms.Button btn_Register;
    }
}

