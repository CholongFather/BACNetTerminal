namespace CommBACNetDNET
{
    partial class frmBACNetDNET
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			this.button1 = new System.Windows.Forms.Button();
			this.richTextBox = new System.Windows.Forms.RichTextBox();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.button3 = new System.Windows.Forms.Button();
			this.txtControl = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.txtVal = new System.Windows.Forms.TextBox();
			this.txtTagID = new System.Windows.Forms.TextBox();
			this.lblTitle = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(262, 5);
			this.button1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(110, 35);
			this.button1.TabIndex = 4;
			this.button1.Text = "Clear";
			this.button1.UseVisualStyleBackColor = true;
			// 
			// richTextBox
			// 
			this.richTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
			this.richTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.richTextBox.Font = new System.Drawing.Font("굴림체", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.richTextBox.Location = new System.Drawing.Point(0, 23);
			this.richTextBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.richTextBox.Name = "richTextBox";
			this.richTextBox.ReadOnly = true;
			this.richTextBox.Size = new System.Drawing.Size(375, 349);
			this.richTextBox.TabIndex = 10;
			this.richTextBox.Text = "";
			// 
			// timer1
			// 
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(262, 40);
			this.button3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(110, 35);
			this.button3.TabIndex = 13;
			this.button3.Text = "Hide";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// txtControl
			// 
			this.txtControl.Location = new System.Drawing.Point(185, 5);
			this.txtControl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.txtControl.Name = "txtControl";
			this.txtControl.Size = new System.Drawing.Size(70, 70);
			this.txtControl.TabIndex = 23;
			this.txtControl.Text = "Send";
			this.txtControl.UseVisualStyleBackColor = true;
			this.txtControl.Click += new System.EventHandler(this.txtControl_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(14, 45);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(26, 15);
			this.label2.TabIndex = 22;
			this.label2.Text = "Val";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(14, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(44, 15);
			this.label1.TabIndex = 21;
			this.label1.Text = "TagID";
			// 
			// txtVal
			// 
			this.txtVal.Location = new System.Drawing.Point(64, 41);
			this.txtVal.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.txtVal.Name = "txtVal";
			this.txtVal.Size = new System.Drawing.Size(114, 25);
			this.txtVal.TabIndex = 20;
			// 
			// txtTagID
			// 
			this.txtTagID.Location = new System.Drawing.Point(64, 5);
			this.txtTagID.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.txtTagID.Name = "txtTagID";
			this.txtTagID.Size = new System.Drawing.Size(114, 25);
			this.txtTagID.TabIndex = 19;
			// 
			// lblTitle
			// 
			this.lblTitle.AutoSize = true;
			this.lblTitle.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblTitle.Location = new System.Drawing.Point(0, 0);
			this.lblTitle.Name = "lblTitle";
			this.lblTitle.Padding = new System.Windows.Forms.Padding(10, 4, 10, 4);
			this.lblTitle.Size = new System.Drawing.Size(20, 23);
			this.lblTitle.TabIndex = 24;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this.button1);
			this.panel1.Controls.Add(this.txtControl);
			this.panel1.Controls.Add(this.button3);
			this.panel1.Controls.Add(this.txtTagID);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.txtVal);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 372);
			this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(375, 80);
			this.panel1.TabIndex = 25;
			// 
			// frmBACNetDNET
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(375, 452);
			this.Controls.Add(this.richTextBox);
			this.Controls.Add(this.lblTitle);
			this.Controls.Add(this.panel1);
			this.KeyPreview = true;
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.MaximizeBox = false;
			this.MinimumSize = new System.Drawing.Size(391, 488);
			this.Name = "frmBACNetDNET";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "CommBACNetDNET";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmBACnet_FormClosing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmBACnet_FormClosed);
			this.Load += new System.EventHandler(this.frmBACnet_Load);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmBACnet_KeyDown);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.RichTextBox richTextBox;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button txtControl;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtVal;
        private System.Windows.Forms.TextBox txtTagID;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel panel1;


    }
}