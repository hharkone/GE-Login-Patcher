using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GE_Login_Patcher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public void ChooseFolder()
        {
            if (folderBrowserElement.ShowDialog() == DialogResult.OK)
            {
                textBoxPath.Text = folderBrowserElement.SelectedPath;
            }
        }

        private void InitializeComponent()
        {
            this.folderBrowserElement = new System.Windows.Forms.FolderBrowserDialog();
            this.textBoxPath = new System.Windows.Forms.TextBox();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.browseLabel = new System.Windows.Forms.Label();
            this.buttonPatch = new System.Windows.Forms.Button();
            this.labelError = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // folderBrowserElement
            // 
            this.folderBrowserElement.SelectedPath = "C:\\Program Files\\NVIDIA Corporation\\NVIDIA GeForce Experience\\www\\";
            // 
            // textBoxPath
            // 
            this.textBoxPath.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxPath.Location = new System.Drawing.Point(12, 103);
            this.textBoxPath.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.textBoxPath.Name = "textBoxPath";
            this.textBoxPath.Size = new System.Drawing.Size(486, 22);
            this.textBoxPath.TabIndex = 0;
            this.textBoxPath.Text = this.folderBrowserElement.SelectedPath;
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Location = new System.Drawing.Point(12, 62);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(75, 23);
            this.buttonBrowse.TabIndex = 1;
            this.buttonBrowse.Text = "Browse";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.ButtonBrowse_Click);
            // 
            // browseLabel
            // 
            this.browseLabel.AutoSize = true;
            this.browseLabel.Location = new System.Drawing.Point(12, 12);
            this.browseLabel.Margin = new System.Windows.Forms.Padding(3, 3, 3, 15);
            this.browseLabel.MaximumSize = new System.Drawing.Size(0, 32);
            this.browseLabel.Name = "browseLabel";
            this.browseLabel.Size = new System.Drawing.Size(371, 32);
            this.browseLabel.TabIndex = 2;
            this.browseLabel.Text = "Input directory with \"app.js\" file. \nIf you used default install directory, this " +
    "should be correct.";
            // 
            // buttonPatch
            // 
            this.buttonPatch.Location = new System.Drawing.Point(105, 62);
            this.buttonPatch.Margin = new System.Windows.Forms.Padding(15, 3, 15, 3);
            this.buttonPatch.Name = "buttonPatch";
            this.buttonPatch.Size = new System.Drawing.Size(75, 23);
            this.buttonPatch.TabIndex = 3;
            this.buttonPatch.Text = "Patch";
            this.buttonPatch.UseVisualStyleBackColor = true;
            this.buttonPatch.Click += new System.EventHandler(this.ButtonPatch_Click);
            // 
            // labelError
            // 
            this.labelError.AutoSize = true;
            this.labelError.Location = new System.Drawing.Point(12, 143);
            this.labelError.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.labelError.Name = "labelError";
            this.labelError.Size = new System.Drawing.Size(48, 17);
            this.labelError.TabIndex = 4;
            this.labelError.Text = "";
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(510, 181);
            this.Controls.Add(this.labelError);
            this.Controls.Add(this.buttonPatch);
            this.Controls.Add(this.browseLabel);
            this.Controls.Add(this.buttonBrowse);
            this.Controls.Add(this.textBoxPath);
            this.Name = "Form1";
            this.Text = "GE Login Patcher";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void ButtonBrowse_Click(object sender, EventArgs e)
        {
            ChooseFolder();
        }
        private void ButtonPatch_Click(object sender, EventArgs e)
        {
            Patcher p = new Patcher();
            string errorString = "";
            this.labelError.Text = "";
            this.labelError.ForeColor = Color.Red;

            if (!p.Patch(folderBrowserElement.SelectedPath, ref errorString))
            {
                this.labelError.Text = "Error: " + errorString;
            }
            else
            {
                this.labelError.ForeColor = Color.Green;
                this.labelError.Text = "Success!";
            }
        }
    }
}
