using System;
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

        private Stack<string> textHistory = new Stack<string>();
        private const int MaxHistoryCount = 10; // 最多紀錄10個紀錄

        private void rtbText_TextChanged(object sender, EventArgs e)
        {
            // 將當前的文本內容加入堆疊
            textHistory.Push(rtbText.Text);

            // 確保堆疊中只保留最多10個紀錄
            if (textHistory.Count > MaxHistoryCount)
            {
                // 用一個臨時堆疊，將除了最下面一筆的文字記錄之外，將文字紀錄堆疊由上而下，逐一移除再堆疊到臨時堆疊之中
                Stack<string> tempStack = new Stack<string>();
                for (int i = 0; i < MaxHistoryCount; i++)
                {
                    tempStack.Push(textHistory.Pop());
                }
                textHistory.Clear(); // 清空堆疊
                                     // 文字編輯堆疊紀錄清空之後，再將暫存堆疊（tempStack）中的資料，逐一放回到文字編輯堆疊紀錄
                foreach (string item in tempStack)
                {
                    textHistory.Push(item);
                }
            }
            UpdateListBox(); // 更新 ListBox          
        }

        // 更新 ListBox
        void UpdateListBox()
        {
            listUndo.Items.Clear(); // 清空 ListBox 中的元素

            // 將堆疊中的內容逐一添加到 ListBox 中
            foreach (string item in textHistory)
            {
                listUndo.Items.Add(item);
            }
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            if (textHistory.Count > 1)
            {
                textHistory.Pop(); // 移除當前的文本內容
                rtbText.Text = textHistory.Peek(); // 將堆疊頂部的文本內容設置為當前的文本內容                
            }
            UpdateListBox(); // 更新 ListBox
        }
    }
    }
  

