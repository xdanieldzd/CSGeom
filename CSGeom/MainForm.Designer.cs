namespace CSGeom
{
    partial class MainForm
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
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openTexturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openGeomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveColladaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveScreenshotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ofdGeomFiles = new System.Windows.Forms.OpenFileDialog();
            this.fbdColladaPath = new System.Windows.Forms.FolderBrowserDialog();
            this.renderControl = new Cobalt.RenderControl();
            this.sfdScreenshot = new System.Windows.Forms.SaveFileDialog();
            this.fbdTextures = new System.Windows.Forms.FolderBrowserDialog();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(784, 24);
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openTexturesToolStripMenuItem,
            this.openGeomToolStripMenuItem,
            this.saveColladaToolStripMenuItem,
            this.toolStripMenuItem1,
            this.saveScreenshotToolStripMenuItem,
            this.toolStripMenuItem2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openTexturesToolStripMenuItem
            // 
            this.openTexturesToolStripMenuItem.Name = "openTexturesToolStripMenuItem";
            this.openTexturesToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.openTexturesToolStripMenuItem.Text = "Open &Textures...";
            this.openTexturesToolStripMenuItem.Click += new System.EventHandler(this.openTexturesToolStripMenuItem_Click);
            // 
            // openGeomToolStripMenuItem
            // 
            this.openGeomToolStripMenuItem.Enabled = false;
            this.openGeomToolStripMenuItem.Name = "openGeomToolStripMenuItem";
            this.openGeomToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.openGeomToolStripMenuItem.Text = "&Open Geom...";
            this.openGeomToolStripMenuItem.Click += new System.EventHandler(this.openGeomToolStripMenuItem_Click);
            // 
            // saveColladaToolStripMenuItem
            // 
            this.saveColladaToolStripMenuItem.Enabled = false;
            this.saveColladaToolStripMenuItem.Name = "saveColladaToolStripMenuItem";
            this.saveColladaToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.saveColladaToolStripMenuItem.Text = "&Save Collada...";
            this.saveColladaToolStripMenuItem.Click += new System.EventHandler(this.saveColladaToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(165, 6);
            // 
            // saveScreenshotToolStripMenuItem
            // 
            this.saveScreenshotToolStripMenuItem.Enabled = false;
            this.saveScreenshotToolStripMenuItem.Name = "saveScreenshotToolStripMenuItem";
            this.saveScreenshotToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.saveScreenshotToolStripMenuItem.Text = "&Save Screenshot...";
            this.saveScreenshotToolStripMenuItem.Click += new System.EventHandler(this.saveScreenshotToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(165, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.aboutToolStripMenuItem.Text = "&About...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // ofdGeomFiles
            // 
            this.ofdGeomFiles.Filter = "Cyber Sleuth Geometry Files (*.geom)|*.geom|All Files (*.*)|*.*";
            // 
            // fbdColladaPath
            // 
            this.fbdColladaPath.Description = "Please select the directory to write the Collada files and textures to.";
            // 
            // renderControl
            // 
            this.renderControl.BackColor = System.Drawing.Color.DimGray;
            this.renderControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.renderControl.Location = new System.Drawing.Point(0, 24);
            this.renderControl.Name = "renderControl";
            this.renderControl.Size = new System.Drawing.Size(784, 538);
            this.renderControl.TabIndex = 0;
            this.renderControl.VSync = false;
            this.renderControl.Render += new System.EventHandler<System.EventArgs>(this.renderControl_Render);
            this.renderControl.Load += new System.EventHandler(this.renderControl_Load);
            this.renderControl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.renderControl_KeyDown);
            this.renderControl.Resize += new System.EventHandler(this.renderControl_Resize);
            // 
            // sfdScreenshot
            // 
            this.sfdScreenshot.Filter = "PNG Files (*.png)|*.png|All Files (*.*)|*.*";
            // 
            // fbdTextures
            // 
            this.fbdTextures.Description = "Please select the directory containing pre-converted textures!";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.renderControl);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Cobalt.RenderControl renderControl;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openGeomToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveColladaToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog ofdGeomFiles;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.FolderBrowserDialog fbdColladaPath;
        private System.Windows.Forms.ToolStripMenuItem saveScreenshotToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.SaveFileDialog sfdScreenshot;
        private System.Windows.Forms.ToolStripMenuItem openTexturesToolStripMenuItem;
        private System.Windows.Forms.FolderBrowserDialog fbdTextures;
    }
}

