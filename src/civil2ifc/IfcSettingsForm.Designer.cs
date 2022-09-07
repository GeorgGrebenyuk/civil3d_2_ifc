
namespace civil2ifc
{
    partial class IfcSettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chb_use_hidded = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button_save_path = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.button_start_export = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listBox1);
            this.groupBox1.Location = new System.Drawing.Point(13, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(283, 132);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Выбор категорий объектов для экспорта";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Items.AddRange(new object[] {
            "Точки COGO (COGO points)",
            "Трассы (Alignments)",
            "Поверхности (TIN surfaces)",
            "Характерные лании (Feature lines)",
            "Трубопроводные сети (Pressure Pipe Network)",
            "Напорные сети (Gravity Pipe Network)"});
            this.listBox1.Location = new System.Drawing.Point(6, 19);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(257, 95);
            this.listBox1.TabIndex = 0;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chb_use_hidded);
            this.groupBox2.Location = new System.Drawing.Point(13, 163);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(283, 100);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Прочие настройки:";
            this.groupBox2.Enter += new System.EventHandler(this.groupBox2_Enter);
            // 
            // chb_use_hidded
            // 
            this.chb_use_hidded.AutoSize = true;
            this.chb_use_hidded.Location = new System.Drawing.Point(7, 20);
            this.chb_use_hidded.Name = "chb_use_hidded";
            this.chb_use_hidded.Size = new System.Drawing.Size(176, 17);
            this.chb_use_hidded.TabIndex = 0;
            this.chb_use_hidded.Text = "Учитывать скрытые объекты";
            this.chb_use_hidded.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.button_start_export);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.button_save_path);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Location = new System.Drawing.Point(13, 270);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(283, 100);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Запуск процедуры экспорта";
            this.groupBox3.Enter += new System.EventHandler(this.groupBox3_Enter);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 26);
            this.label1.TabIndex = 1;
            this.label1.Text = "Выберите место \r\nсохранения файла:";
            // 
            // button_save_path
            // 
            this.button_save_path.Location = new System.Drawing.Point(188, 27);
            this.button_save_path.Name = "button_save_path";
            this.button_save_path.Size = new System.Drawing.Size(75, 23);
            this.button_save_path.TabIndex = 2;
            this.button_save_path.Text = "Обзор";
            this.button_save_path.UseVisualStyleBackColor = true;
            this.button_save_path.Click += new System.EventHandler(this.button_save_path_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(165, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Нажмите для старта экспорта:";
            // 
            // button_start_export
            // 
            this.button_start_export.Location = new System.Drawing.Point(188, 64);
            this.button_start_export.Name = "button_start_export";
            this.button_start_export.Size = new System.Drawing.Size(75, 23);
            this.button_start_export.TabIndex = 4;
            this.button_start_export.Text = "Старт";
            this.button_start_export.UseVisualStyleBackColor = true;
            this.button_start_export.Click += new System.EventHandler(this.button_start_export_Click);
            // 
            // IfcSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "IfcSettingsForm";
            this.Text = "Настройка параметров экспорта";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chb_use_hidded;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_save_path;
        private System.Windows.Forms.Button button_start_export;
        private System.Windows.Forms.Label label2;
    }
}