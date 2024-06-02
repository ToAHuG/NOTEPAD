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

        private bool isUndoRedo = false;                           
        private Stack<string> undoStack = new Stack<string>();     
        private Stack<string> redoStack = new Stack<string>();     
        private const int MaxHistoryCount = 10;
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
        private void btnUndo_Click(object sender, EventArgs e)
        {
            if (undoStack.Count > 1)
            {
                isUndoRedo = true;
                redoStack.Push(undoStack.Pop()); 
                rtbText.Text = undoStack.Peek(); 
                UpdateListBox();
                isUndoRedo = false;
            }
        }

        private void btnRedo_Click(object sender, EventArgs e)
        {
            if (redoStack.Count > 0)
            {
                isUndoRedo = true;
                undoStack.Push(redoStack.Pop()); 
                rtbText.Text = undoStack.Peek();
                UpdateListBox();
                isUndoRedo = false;
            }
        }

        private void rtbText_TextChanged(object sender, EventArgs e)
        {
            
            {
                undoStack.Push(rtbText.Text); 
                redoStack.Clear();           

                // 確保堆疊中只保留最多10個紀錄
                if (undoStack.Count > MaxHistoryCount)
                {
                    Stack<string> tempStack = new Stack<string>();
                    for (int i = 0; i < MaxHistoryCount; i++)
                    {
                        tempStack.Push(undoStack.Pop());
                    }
                    undoStack.Clear(); 
                                      
                    foreach (string item in tempStack)
                    {
                        undoStack.Push(item);
                    }
                }
                UpdateListBox(); 
            }
        }

        
        void UpdateListBox()
        {
            listUndo.Items.Clear(); 

         
            foreach (string item in undoStack)
            {
                listUndo.Items.Add(item);
            }
        }

        private void comboBoxFont_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            
                foreach (FontFamily font in FontFamily.Families)
                {
                    comboBoxFont.Items.Add(font.Name);
                }
              
                comboBoxFont.SelectedIndex = 0;
            }

        private void comboBoxSize_SelectedIndexChanged(object sender, EventArgs e)
        {
           
            for (int i = 8; i <= 72; i += 2)
            {
                comboBoxSize.Items.Add(i);
            }
            
            comboBoxSize.SelectedIndex = 2;
        }

        private void comboBoxStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
           
            comboBoxStyle.Items.Add(FontStyle.Regular.ToString());   
            comboBoxStyle.Items.Add(FontStyle.Bold.ToString());      
            comboBoxStyle.Items.Add(FontStyle.Italic.ToString());    
            comboBoxStyle.Items.Add(FontStyle.Underline.ToString()); 
            comboBoxStyle.Items.Add(FontStyle.Strikeout.ToString()); 
                                                                     
            comboBoxStyle.SelectedIndex = 0;
        }
    }
    }

  

