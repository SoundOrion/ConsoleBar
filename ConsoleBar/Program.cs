using System;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    // スピナー用の文字
    private static readonly char[] SpinnerChars = { '|', '/', '-', '\\' };
    private static int _spinnerIndex = 0;

    // Console 出力を守るロック
    private static readonly object ConsoleLock = new();

    static async Task Main()
    {
        // ダミーの「サーバー」リスト
        var items = new[]
        {
            "server1", "server2", "server3", "server4", "server5",
            "server6", "server7", "server8", "server9", "server10",
        };

        int total = items.Length;
        int done = 0;

        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = 4
        };

        Console.WriteLine("★ 進捗バー（スピナー付き）サンプル開始");

        await Parallel.ForEachAsync(items, options, async (item, token) =>
        {
            // ダミーの処理（ランダム時間スリープ）
            int ms = Random.Shared.Next(500, 2500);
            await Task.Delay(ms, token); // async 版

            // 完了数をスレッド安全に加算
            int finished = Interlocked.Increment(ref done);

            // % へ換算
            double percent = (double)finished / total * 100.0;

            // スピナーのインデックスをインクリメント
            int spinnerIndex = Interlocked.Increment(ref _spinnerIndex);

            // 進捗バー描画
            DrawProgressBar(percent, finished, total, spinnerIndex);
        });

        // 行を整える
        Console.WriteLine();
        Console.WriteLine("✔ 完了しました。Enterで終了");
        Console.ReadLine();
    }

    // ★ スピナー付き蛍光緑の進捗バー描画メソッド
    private static void DrawProgressBar(double progressPercent, int current, int total, int spinnerIndex)
    {
        int barWidth = 40;
        int filled = (int)(barWidth * progressPercent / 100.0);

        string barFilled = new('■', filled);
        string barEmpty = new('-', barWidth - filled);

        char spinner = SpinnerChars[spinnerIndex % SpinnerChars.Length];

        lock (ConsoleLock)
        {
            var oldColor = Console.ForegroundColor;

            // 行頭に戻して上書き
            Console.Write("\r");

            // スピナー
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(spinner);
            Console.Write(' ');

            // バー本体
            Console.Write('[');

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(barFilled);

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(barEmpty);

            Console.ForegroundColor = oldColor;

            Console.Write($"]  {progressPercent,6:F2}%  ({current}/{total})");
        }
    }
}
