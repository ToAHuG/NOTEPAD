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
            InitializeFontComboBox();
            InitializeFontSizeComboBox();
            InitializeFontStyleComboBox();
        }

        private bool isUndoRedo = false;                           // 是否在回復或重作階段
        private Stack<MemoryStream> undoStack = new Stack<MemoryStream>(); // 回復堆疊
        private Stack<MemoryStream> redoStack = new Stack<MemoryStream>(); // 重作堆疊
        //private Stack<string> undoStack = new Stack<string>();     
        //private Stack<string> redoStack = new Stack<string>();     
        private const int MaxHistoryCount = 10;                    // 最多紀錄10個紀錄

        private int selectionStart = 0;                            // 記錄文字反白的起點
        private int selectionLength = 0;
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
                redoStack.Push(undoStack.Pop()); // 將回復堆疊最上面的紀錄移出，再堆到重作堆疊
                MemoryStream lastSavedState = undoStack.Peek(); // 將回復堆疊最上面一筆紀錄顯示
                LoadFromMemory(lastSavedState);
                UpdateListBox();
                isUndoRedo = false;
            }
        }

        private void btnRedo_Click(object sender, EventArgs e)
        {
            if (redoStack.Count > 0)
            {
                isUndoRedo = true;
                undoStack.Push(redoStack.Pop()); // 將重作堆疊最上面的紀錄移出，再堆到回復堆疊
                MemoryStream lastSavedState = undoStack.Peek(); // 將回復堆疊最上面一筆紀錄顯示
                LoadFromMemory(lastSavedState);
                UpdateListBox();
                isUndoRedo = false;
            }
        }

        private void rtbText_TextChanged(object sender, EventArgs e)
        {
            if (isUndoRedo == false)
            {
                SaveCurrentStateToStack(); // 將當前的文本內容加入堆疊
                redoStack.Clear();            // 清空重作堆疊

                // 確保堆疊中只保留最多10個紀錄
                if (undoStack.Count > MaxHistoryCount)
                {
                    // 用一個臨時堆疊，將除了最下面一筆的文字記錄之外，將文字紀錄堆疊由上而下，逐一移除再堆疊到臨時堆疊之中
                    Stack<MemoryStream> tempStack = new Stack<MemoryStream>();
                    for (int i = 0; i < MaxHistoryCount; i++)
                    {
                        tempStack.Push(undoStack.Pop());
                    }
                    undoStack.Clear(); // 清空堆疊
                                       // 文字編輯堆疊紀錄清空之後，再將暫存堆疊（tempStack）中的資料，逐一放回到文字編輯堆疊紀錄
                    foreach (MemoryStream item in tempStack)
                    {
                        undoStack.Push(item);
                    }
                }
                UpdateListBox(); // 更新 ListBox
            }
        }

        // 更新 ListBox
        void UpdateListBox()
        {
            
            {
                listUndo.Items.Clear(); // 清空 ListBox 中的元素

                // 將堆疊中的內容逐一添加到 ListBox 中
                foreach (MemoryStream item in undoStack)
                {
                    listUndo.Items.Add(item);
                }
            }
        }

            private void InitializeFontComboBox()
        {

            // 將所有系統字體名稱添加到字體選擇框中
            foreach (FontFamily font in FontFamily.Families)
            {
                comboBoxFont.Items.Add(font.Name);
            }
            // 設置預設選中的項目為第一個字體
            comboBoxFont.SelectedIndex = 0;
        }

        // 初始化字體大小下拉選單
        private void InitializeFontSizeComboBox()
        {
            // 從8開始，每次增加2，直到72，將這些數值添加到字體大小選擇框中
            for (int i = 8; i <= 72; i += 2)
            {
                comboBoxSize.Items.Add(i);
            }
            // 設置預設選中的項目為第三個大小，即12字體大小
            comboBoxSize.SelectedIndex = 2;
        }

        // 初始化字體樣式下拉選單
        private void InitializeFontStyleComboBox()
        {
            // 將不同的字體樣式添加到字體樣式選擇框中
            comboBoxStyle.Items.Add(FontStyle.Regular.ToString());   // 正常
            comboBoxStyle.Items.Add(FontStyle.Bold.ToString());      // 粗體
            comboBoxStyle.Items.Add(FontStyle.Italic.ToString());    // 斜體
            comboBoxStyle.Items.Add(FontStyle.Underline.ToString()); // 底線
            comboBoxStyle.Items.Add(FontStyle.Strikeout.ToString()); // 刪除線
                                                                     // 設置預設選中的項目為第一個樣式，即正常字體
            comboBoxStyle.SelectedIndex = 0;
        }

       
        private void comboBoxFont_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 檢查當前選擇的文字是否有字型，如果有，則進行後續處理
            if (rtbText.SelectionFont != null)
            {
                // 從下拉選單中獲取選擇的字型、大小和樣式
                string selectedFont = comboBoxFont.SelectedItem?.ToString();
                string selectedSizeStr = comboBoxSize.SelectedItem?.ToString();
                string selectedStyleStr = comboBoxStyle.SelectedItem?.ToString();

                // 確保字型、大小和樣式都已選擇
                if (selectedFont != null && selectedSizeStr != null && selectedStyleStr != null)
                {
                    // 將選擇的大小字串轉換為浮點數
                    float selectedSize = float.Parse(selectedSizeStr);
                    // 將選擇的樣式字串轉換為 FontStyle 枚舉值
                    FontStyle selectedStyle = (FontStyle)Enum.Parse(typeof(FontStyle), selectedStyleStr);

                    // 獲取當前選擇的文字的字型
                    Font currentFont = rtbText.SelectionFont;
                    FontStyle newStyle = currentFont.Style;

                    // 檢查是否需要應用新的樣式，並更新樣式
                    if (comboBoxStyle.SelectedItem.ToString() == FontStyle.Bold.ToString())
                        newStyle = FontStyle.Bold;
                    else if (comboBoxStyle.SelectedItem.ToString() == FontStyle.Italic.ToString())
                        newStyle = FontStyle.Italic;
                    else if (comboBoxStyle.SelectedItem.ToString() == FontStyle.Underline.ToString())
                        newStyle = FontStyle.Underline;
                    else if (comboBoxStyle.SelectedItem.ToString() == FontStyle.Strikeout.ToString())
                        newStyle = FontStyle.Strikeout;
                    else
                        newStyle = FontStyle.Regular;

                    // 創建新的字型並應用到選擇的文字
                    Font newFont = new Font(selectedFont, selectedSize, newStyle);
                    rtbText.SelectionFont = newFont;
                }
            }
        }
        private void SaveCurrentStateToStack()
        {
            // 創建一個新的 MemoryStream 來保存文字編輯狀態
            MemoryStream memoryStream = new MemoryStream();
            // 將 RichTextBox 的內容保存到 memoryStream
            rtbText.SaveFile(memoryStream, RichTextBoxStreamType.RichText);
            // 將 memoryStream 放入回復堆疊
            undoStack.Push(memoryStream);
        }
        private void LoadFromMemory(MemoryStream memoryStream)
        {
            // 將 memoryStream 的指標重置到開始位置
            memoryStream.Seek(0, SeekOrigin.Begin);
            // 將 memoryStream 的內容放到到 RichTextBox
            rtbText.LoadFile(memoryStream, RichTextBoxStreamType.RichText);
        }
    }
    
    }


  

