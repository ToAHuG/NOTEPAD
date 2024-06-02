﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NOTEPAD
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            
            openFileDialog1.Title = "選擇檔案";
            
            openFileDialog1.Filter = "文字檔案 (*.txt)|*.txt|所有檔案 (*.*)|*.*";
            
            openFileDialog1.FilterIndex = 1;
            
            openFileDialog1.InitialDirectory = "C:\\";
            
            openFileDialog1.Multiselect = true;

            
            DialogResult result = openFileDialog1.ShowDialog();

            
            if (result == DialogResult.OK)
            {
                try
                {
                    
                    string selectedFileName = openFileDialog1.FileName;

                    

                    
                    using (FileStream fileStream = new FileStream(selectedFileName, FileMode.Open, FileAccess.Read))
                    {
                        
                        using (StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8))
                        {
                            
                            rtbText.Text = streamReader.ReadToEnd();
                        }
                    }
                }
                catch (Exception ex)
                {
                   
                    MessageBox.Show("讀取檔案時發生錯誤: " + ex.Message, "錯誤訊息", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("使用者取消了選擇檔案操作。", "訊息", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            saveFileDialog1.Title = "儲存檔案";
            saveFileDialog1.Filter = "文字檔案 (*.txt)|*.txt|所有檔案 (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.InitialDirectory = "C:\\";
            saveFileDialog1.FileName = "newfile.txt";

            DialogResult result = saveFileDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                try
                {
                    string selectedFileName = saveFileDialog1.FileName;

                    using (FileStream fileStream = new FileStream(selectedFileName, FileMode.Create, FileAccess.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
                        {
                            streamWriter.Write(rtbText.Text);
                        }
                    }

                    MessageBox.Show("檔案已成功儲存。", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("儲存檔案時發生錯誤: " + ex.Message, "錯誤訊息", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("使用者取消了儲存檔案操作。", "訊息", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
            }
        }
    }
    }
  

