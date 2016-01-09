namespace HiddenConsole {
    partial class ConsoleForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.commandPanel = new System.Windows.Forms.Panel();
            this.commandEnterButton = new System.Windows.Forms.Button();
            this.commandTextBox = new System.Windows.Forms.TextBox();
            this.conOutRichTextBox = new System.Windows.Forms.RichTextBox();
            this.conOutContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.commandPanel.SuspendLayout();
            this.conOutContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // commandPanel
            // 
            this.commandPanel.AutoSize = true;
            this.commandPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.commandPanel.Controls.Add(this.commandTextBox);
            this.commandPanel.Controls.Add(this.commandEnterButton);
            this.commandPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.commandPanel.Location = new System.Drawing.Point(0, 661);
            this.commandPanel.Name = "commandPanel";
            this.commandPanel.Size = new System.Drawing.Size(1264, 20);
            this.commandPanel.TabIndex = 0;
            // 
            // commandEnterButton
            // 
            this.commandEnterButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.commandEnterButton.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.commandEnterButton.Location = new System.Drawing.Point(1239, 0);
            this.commandEnterButton.Name = "commandEnterButton";
            this.commandEnterButton.Size = new System.Drawing.Size(25, 20);
            this.commandEnterButton.TabIndex = 0;
            this.commandEnterButton.Text = ">";
            this.commandEnterButton.UseVisualStyleBackColor = true;
            this.commandEnterButton.Click += new System.EventHandler(this.commandEnterButton_Click);
            // 
            // commandTextBox
            // 
            this.commandTextBox.BackColor = System.Drawing.Color.Black;
            this.commandTextBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.commandTextBox.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.commandTextBox.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.commandTextBox.Location = new System.Drawing.Point(0, 0);
            this.commandTextBox.Name = "commandTextBox";
            this.commandTextBox.Size = new System.Drawing.Size(1239, 20);
            this.commandTextBox.TabIndex = 1;
            this.commandTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.commandTextBox_KeyDown);
            this.commandTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.commandTextBox_KeyPress);
            // 
            // conOutRichTextBox
            // 
            this.conOutRichTextBox.BackColor = System.Drawing.Color.Black;
            this.conOutRichTextBox.ContextMenuStrip = this.conOutContextMenu;
            this.conOutRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.conOutRichTextBox.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.conOutRichTextBox.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.conOutRichTextBox.Location = new System.Drawing.Point(0, 0);
            this.conOutRichTextBox.Name = "conOutRichTextBox";
            this.conOutRichTextBox.ReadOnly = true;
            this.conOutRichTextBox.Size = new System.Drawing.Size(1264, 661);
            this.conOutRichTextBox.TabIndex = 1;
            this.conOutRichTextBox.Text = "";
            // 
            // conOutContextMenu
            // 
            this.conOutContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem});
            this.conOutContextMenu.Name = "conOutContextMenu";
            this.conOutContextMenu.Size = new System.Drawing.Size(103, 26);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // ConsoleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.conOutRichTextBox);
            this.Controls.Add(this.commandPanel);
            this.Name = "ConsoleForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultBounds;
            this.Text = "ConsoleForm";
            this.commandPanel.ResumeLayout(false);
            this.commandPanel.PerformLayout();
            this.conOutContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel commandPanel;
        private System.Windows.Forms.TextBox commandTextBox;
        private System.Windows.Forms.Button commandEnterButton;
        private System.Windows.Forms.RichTextBox conOutRichTextBox;
        private System.Windows.Forms.ContextMenuStrip conOutContextMenu;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
    }
}