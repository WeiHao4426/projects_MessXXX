namespace DIP_02
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.open = new System.Windows.Forms.Button();
            this.save = new System.Windows.Forms.Button();
            this.close = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.histogram = new System.Windows.Forms.Button();
            this.GausFliter = new System.Windows.Forms.Button();
            this.lowpass = new System.Windows.Forms.Button();
            this.FTT = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // open
            // 
            this.open.Location = new System.Drawing.Point(12, 12);
            this.open.Name = "open";
            this.open.Size = new System.Drawing.Size(156, 95);
            this.open.TabIndex = 0;
            this.open.Text = "打开图像";
            this.open.UseVisualStyleBackColor = true;
            this.open.Click += new System.EventHandler(this.open_Click);
            // 
            // save
            // 
            this.save.Location = new System.Drawing.Point(12, 113);
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(155, 46);
            this.save.TabIndex = 1;
            this.save.Text = "保存";
            this.save.UseVisualStyleBackColor = true;
            this.save.Click += new System.EventHandler(this.save_Click);
            // 
            // close
            // 
            this.close.Location = new System.Drawing.Point(11, 165);
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(156, 38);
            this.close.TabIndex = 2;
            this.close.Text = "关闭";
            this.close.UseVisualStyleBackColor = true;
            this.close.Click += new System.EventHandler(this.close_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 209);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(155, 51);
            this.button1.TabIndex = 3;
            this.button1.Text = "直方均衡";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // histogram
            // 
            this.histogram.Location = new System.Drawing.Point(12, 266);
            this.histogram.Name = "histogram";
            this.histogram.Size = new System.Drawing.Size(156, 40);
            this.histogram.TabIndex = 4;
            this.histogram.Text = "绘制直方图";
            this.histogram.UseVisualStyleBackColor = true;
            this.histogram.Click += new System.EventHandler(this.histogram_Click);
            // 
            // GausFliter
            // 
            this.GausFliter.Location = new System.Drawing.Point(11, 312);
            this.GausFliter.Name = "GausFliter";
            this.GausFliter.Size = new System.Drawing.Size(155, 42);
            this.GausFliter.TabIndex = 5;
            this.GausFliter.Text = "高斯滤波";
            this.GausFliter.UseVisualStyleBackColor = true;
            this.GausFliter.Click += new System.EventHandler(this.button2_Click);
            // 
            // lowpass
            // 
            this.lowpass.Location = new System.Drawing.Point(12, 360);
            this.lowpass.Name = "lowpass";
            this.lowpass.Size = new System.Drawing.Size(151, 42);
            this.lowpass.TabIndex = 6;
            this.lowpass.Text = "低通滤波";
            this.lowpass.UseVisualStyleBackColor = true;
            this.lowpass.Click += new System.EventHandler(this.lowpass_Click);
            // 
            // FTT
            // 
            this.FTT.Location = new System.Drawing.Point(12, 408);
            this.FTT.Name = "FTT";
            this.FTT.Size = new System.Drawing.Size(146, 55);
            this.FTT.TabIndex = 7;
            this.FTT.Text = "傅里叶变换";
            this.FTT.UseVisualStyleBackColor = true;
            this.FTT.Click += new System.EventHandler(this.FTT_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1101, 690);
            this.Controls.Add(this.FTT);
            this.Controls.Add(this.lowpass);
            this.Controls.Add(this.GausFliter);
            this.Controls.Add(this.histogram);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.close);
            this.Controls.Add(this.save);
            this.Controls.Add(this.open);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button open;
        private System.Windows.Forms.Button save;
        private System.Windows.Forms.Button close;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button histogram;
        private System.Windows.Forms.Button GausFliter;
        private System.Windows.Forms.Button lowpass;
        private System.Windows.Forms.Button FTT;
    }
}

