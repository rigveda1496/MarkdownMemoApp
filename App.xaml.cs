using System;
using System.IO;
using System.Windows;

namespace MarkdownMemoApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // メモの保存フォルダを作成
            string savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MarkdownMemoApp");
            Directory.CreateDirectory(savePath); // フォルダがなければ作成

            // 他の初期化処理があればここに追加
        }
    }
}
