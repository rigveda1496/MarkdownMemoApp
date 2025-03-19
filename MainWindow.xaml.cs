using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using Markdig;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Text;

namespace MarkdownMemoApp
{
    public partial class MainWindow : Window
    {
        private string currentFilePath = null;
        private List<string> memoFiles = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            LoadMemoList();
        }

        private void NewMemo_Click(object sender, RoutedEventArgs e)
        {
            MemoTextBox.Clear();
            currentFilePath = null;
            MemoTextBox.Clear();
            currentFilePath = $"memo_{DateTime.Now:yyyyMMdd_HHmmss}.md"; // 一時的なファイル名
            File.WriteAllText(currentFilePath, ""); // 空のファイルを作成
            LoadMemoList();
        }

        private void SaveMemo_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(currentFilePath))
            {
                SaveAsMemo();
            }
            else
            {
                File.WriteAllText(currentFilePath, MemoTextBox.Text, Encoding.UTF8); // UTF-8 で保存
                LoadMemoList();
            }
        }

        private void SaveAsMemo()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Markdown ファイル (*.md)|*.md|テキスト ファイル (*.txt)|*.txt",
                DefaultExt = ".md",
                Title = "名前を付けて保存"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                currentFilePath = saveFileDialog.FileName;
                File.WriteAllText(currentFilePath, MemoTextBox.Text);
                LoadMemoList();
            }
        }

        private void DeleteMemo_Click(object sender, RoutedEventArgs e)
        {
            if (MemoListBox.SelectedItem != null)
            {
                int index = MemoListBox.SelectedIndex;
                string filePath = memoFiles[index];

                if (MessageBox.Show("このメモを削除しますか？", "確認", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    try
                    {
                        File.Delete(filePath);
                        LoadMemoList();
                        MemoTextBox.Clear();
                        currentFilePath = null;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("ファイルの削除に失敗しました: " + ex.Message);
                    }
                }
            }
        }

        private void MemoTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var markdown = MemoTextBox.Text;
            var htmlContent = Markdown.ToHtml(markdown);

            // UTF-8 の meta タグを追加して、日本語の文字化けを防ぐ
            string fullHtml = $@"
        <html>
        <head>
            <meta charset='UTF-8'>
            <style>
                body {{ font-family: 'Meiryo', 'Yu Gothic', sans-serif; }}
            </style>
        </head>
        <body>
            {htmlContent}
        </body>
        </html>";

            Dispatcher.Invoke(() => MarkdownPreview.NavigateToString(fullHtml));
        }

        private void LoadMemoList()
        {
            MemoListBox.Items.Clear();
            memoFiles.Clear();

            try
            {
                // カレントディレクトリ内の .md ファイルを取得
                string[] files = Directory.GetFiles(".", "*.md");
                foreach (string file in files)
                {
                    memoFiles.Add(file);
                    MemoListBox.Items.Add(System.IO.Path.GetFileName(file)); // 日本語対応のためファイル名のみ取得
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("メモリストの読み込みに失敗しました: " + ex.Message);
            }
        }

        private void MemoListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MemoListBox.SelectedItem != null)
            {
                int index = MemoListBox.SelectedIndex;
                currentFilePath = memoFiles[index];

                if (!currentFilePath.EndsWith(".md"))
                {
                    MessageBox.Show("Markdown ファイルのみ開けます。");
                    return;
                }

                try
                {
                    MemoTextBox.Text = File.ReadAllText(currentFilePath, Encoding.UTF8); // UTF-8 で読み込む
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ファイルの読み込みに失敗しました: " + ex.Message);
                }
            }
        }
    }
}