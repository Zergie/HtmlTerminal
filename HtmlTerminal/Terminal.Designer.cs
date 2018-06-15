namespace HtmlTerminal
{
    partial class Terminal
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

        #region Vom Komponenten-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.Webview = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // Webview
            // 
            this.Webview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Webview.IsWebBrowserContextMenuEnabled = false;
            this.Webview.Location = new System.Drawing.Point(0, 0);
            this.Webview.MinimumSize = new System.Drawing.Size(20, 20);
            this.Webview.Name = "Webview";
            this.Webview.Size = new System.Drawing.Size(800, 450);
            this.Webview.TabIndex = 0;
            this.Webview.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.Webview_DocumentCompleted);
            // 
            // Terminal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Webview);
            this.Name = "Terminal";
            this.Size = new System.Drawing.Size(800, 450);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser Webview;
    }
}
