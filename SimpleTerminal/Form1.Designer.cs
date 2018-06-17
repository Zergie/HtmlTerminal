namespace SimpleTerminal
{
    partial class Form1
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.terminal1 = new HtmlTerminal.Terminal();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyHtmlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.terminal1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.777778F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 90.22222F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(800, 450);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // terminal1
            // 
            this.terminal1.ContextMenuStrip = this.contextMenuStrip1;
            this.terminal1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.terminal1.Location = new System.Drawing.Point(3, 3);
            this.terminal1.Name = "terminal1";
            this.terminal1.Prompt = "$";
            this.terminal1.PromptVisible = true;
            this.terminal1.Size = new System.Drawing.Size(794, 444);
            this.terminal1.SuggestionDisplayFormat = HtmlTerminal.EnumListDisplay.Table3Columns;
            this.terminal1.TabIndex = 1;
            this.terminal1.CommandException += new HtmlTerminal.Terminal.CommandExceptionEventHandler(this.terminal1_CommandException);
            this.terminal1.Loaded += new HtmlTerminal.Terminal.SimpleEventHandler(this.terminal1_Loaded);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyHtmlToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(129, 26);
            // 
            // copyHtmlToolStripMenuItem
            // 
            this.copyHtmlToolStripMenuItem.Name = "copyHtmlToolStripMenuItem";
            this.copyHtmlToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.copyHtmlToolStripMenuItem.Text = "copy html";
            this.copyHtmlToolStripMenuItem.Click += new System.EventHandler(this.copyHtmlToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private HtmlTerminal.Terminal terminal1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem copyHtmlToolStripMenuItem;
    }
}

