namespace Automata
{
    partial class DemoFrm
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
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.button1 = new System.Windows.Forms.Button();
            this.btnCreateAutomata = new System.Windows.Forms.Button();
            this.nmrStateCount = new System.Windows.Forms.NumericUpDown();
            this.btnCreateTable = new System.Windows.Forms.Button();
            this.gridAutomata = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbxChar = new System.Windows.Forms.TextBox();
            this.selectableContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.itemStraightLine = new System.Windows.Forms.ToolStripMenuItem();
            this.itemCurve = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.itemAdjustCurve = new System.Windows.Forms.ToolStripMenuItem();
            this.itemFinalState = new System.Windows.Forms.ToolStripMenuItem();
            this.gridContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.stateMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tbxInput = new System.Windows.Forms.TextBox();
            this.automataView = new AutomataLib.AutomataView();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmrStateCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridAutomata)).BeginInit();
            this.selectableContextMenu.SuspendLayout();
            this.gridContextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.automataView)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tbxInput);
            this.splitContainer1.Panel1.Controls.Add(this.button1);
            this.splitContainer1.Panel1.Controls.Add(this.btnCreateAutomata);
            this.splitContainer1.Panel1.Controls.Add(this.nmrStateCount);
            this.splitContainer1.Panel1.Controls.Add(this.btnCreateTable);
            this.splitContainer1.Panel1.Controls.Add(this.gridAutomata);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.tbxChar);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.automataView);
            this.splitContainer1.Size = new System.Drawing.Size(693, 357);
            this.splitContainer1.SplitterDistance = 306;
            this.splitContainer1.TabIndex = 8;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(215, 312);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 15;
            this.button1.Text = "Demo";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnCreateAutomata
            // 
            this.btnCreateAutomata.Location = new System.Drawing.Point(12, 269);
            this.btnCreateAutomata.Name = "btnCreateAutomata";
            this.btnCreateAutomata.Size = new System.Drawing.Size(96, 26);
            this.btnCreateAutomata.TabIndex = 14;
            this.btnCreateAutomata.Text = "Tạo Automat";
            this.btnCreateAutomata.UseVisualStyleBackColor = true;
            this.btnCreateAutomata.Click += new System.EventHandler(this.btnCreateAutomata_Click);
            // 
            // nmrStateCount
            // 
            this.nmrStateCount.Location = new System.Drawing.Point(136, 12);
            this.nmrStateCount.Name = "nmrStateCount";
            this.nmrStateCount.Size = new System.Drawing.Size(46, 20);
            this.nmrStateCount.TabIndex = 13;
            this.nmrStateCount.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // btnCreateTable
            // 
            this.btnCreateTable.Location = new System.Drawing.Point(12, 227);
            this.btnCreateTable.Name = "btnCreateTable";
            this.btnCreateTable.Size = new System.Drawing.Size(128, 26);
            this.btnCreateTable.TabIndex = 12;
            this.btnCreateTable.Text = "Tạo bảng trạng thái";
            this.btnCreateTable.UseVisualStyleBackColor = true;
            this.btnCreateTable.Click += new System.EventHandler(this.btnCreateTable_Click);
            // 
            // gridAutomata
            // 
            this.gridAutomata.AllowUserToAddRows = false;
            this.gridAutomata.AllowUserToDeleteRows = false;
            this.gridAutomata.BackgroundColor = System.Drawing.Color.LavenderBlush;
            this.gridAutomata.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridAutomata.ColumnHeadersVisible = false;
            this.gridAutomata.GridColor = System.Drawing.SystemColors.GrayText;
            this.gridAutomata.Location = new System.Drawing.Point(12, 71);
            this.gridAutomata.Name = "gridAutomata";
            this.gridAutomata.RowHeadersVisible = false;
            this.gridAutomata.Size = new System.Drawing.Size(278, 150);
            this.gridAutomata.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Danh sách kí tự:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Số lượng trạng thái:";
            // 
            // tbxChar
            // 
            this.tbxChar.Location = new System.Drawing.Point(136, 38);
            this.tbxChar.Name = "tbxChar";
            this.tbxChar.Size = new System.Drawing.Size(154, 20);
            this.tbxChar.TabIndex = 8;
            // 
            // selectableContextMenu
            // 
            this.selectableContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itemStraightLine,
            this.itemCurve,
            this.toolStripSeparator1,
            this.itemAdjustCurve,
            this.itemFinalState});
            this.selectableContextMenu.Name = "connectorContextMenu";
            this.selectableContextMenu.Size = new System.Drawing.Size(175, 98);
            this.selectableContextMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.connectorContextMenu_ItemClicked);
            // 
            // itemStraightLine
            // 
            this.itemStraightLine.Checked = true;
            this.itemStraightLine.CheckOnClick = true;
            this.itemStraightLine.CheckState = System.Windows.Forms.CheckState.Checked;
            this.itemStraightLine.Name = "itemStraightLine";
            this.itemStraightLine.Size = new System.Drawing.Size(174, 22);
            this.itemStraightLine.Text = "Vẽ thẳng";
            // 
            // itemCurve
            // 
            this.itemCurve.CheckOnClick = true;
            this.itemCurve.Name = "itemCurve";
            this.itemCurve.Size = new System.Drawing.Size(174, 22);
            this.itemCurve.Text = "Vẽ cong";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(171, 6);
            // 
            // itemAdjustCurve
            // 
            this.itemAdjustCurve.Name = "itemAdjustCurve";
            this.itemAdjustCurve.Size = new System.Drawing.Size(174, 22);
            this.itemAdjustCurve.Text = "Chỉnh đường cong";
            // 
            // itemFinalState
            // 
            this.itemFinalState.Name = "itemFinalState";
            this.itemFinalState.Size = new System.Drawing.Size(174, 22);
            this.itemFinalState.Text = "Trạng thái kết thúc";
            // 
            // gridContextMenu
            // 
            this.gridContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stateMenuItem});
            this.gridContextMenu.Name = "contextMenuStrip1";
            this.gridContextMenu.Size = new System.Drawing.Size(175, 26);
            // 
            // stateMenuItem
            // 
            this.stateMenuItem.Name = "stateMenuItem";
            this.stateMenuItem.Size = new System.Drawing.Size(174, 22);
            this.stateMenuItem.Text = "Trạng thái kết thúc";
            // 
            // tbxInput
            // 
            this.tbxInput.Location = new System.Drawing.Point(12, 314);
            this.tbxInput.Name = "tbxInput";
            this.tbxInput.Size = new System.Drawing.Size(146, 20);
            this.tbxInput.TabIndex = 16;
            // 
            // automataView
            // 
            this.automataView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.automataView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.automataView.ErrorImage = null;
            this.automataView.Location = new System.Drawing.Point(0, 0);
            this.automataView.Name = "automataView";
            this.automataView.Size = new System.Drawing.Size(383, 357);
            this.automataView.TabIndex = 0;
            this.automataView.TabStop = false;
            this.automataView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.automataView_MouseClick);
            // 
            // DemoFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(693, 357);
            this.Controls.Add(this.splitContainer1);
            this.Name = "DemoFrm";
            this.Text = "DemoFrm";
            this.Load += new System.EventHandler(this.DemoFrm_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DemoFrm_FormClosed);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.DemoFrm_KeyUp);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nmrStateCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridAutomata)).EndInit();
            this.selectableContextMenu.ResumeLayout(false);
            this.gridContextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.automataView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnCreateAutomata;
        private System.Windows.Forms.NumericUpDown nmrStateCount;
        private System.Windows.Forms.Button btnCreateTable;
        private System.Windows.Forms.DataGridView gridAutomata;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbxChar;
        private AutomataLib.AutomataView automataView;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ContextMenuStrip selectableContextMenu;
        private System.Windows.Forms.ToolStripMenuItem itemStraightLine;
        private System.Windows.Forms.ToolStripMenuItem itemCurve;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem itemAdjustCurve;
        private System.Windows.Forms.ContextMenuStrip gridContextMenu;
        private System.Windows.Forms.ToolStripMenuItem stateMenuItem;
        private System.Windows.Forms.ToolStripMenuItem itemFinalState;
        private System.Windows.Forms.TextBox tbxInput;

    }
}