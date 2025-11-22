using System;
using System.Threading;
using System.Threading.Tasks;

class Program
{
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
            MaxDegreeOfParallelism = 4 // 並列数（調整してみてOK）
        };

        Console.WriteLine("★ 進捗バーのサンプル開始");

        await Parallel.ForEachAsync(items, options, async (item, token) =>
        {
            // ダミーの処理（ランダム時間スリープ）
            int ms = Random.Shared.Next(500, 2500);
            Thread.Sleep(ms); // わざとブロッキング

            // 完了数をスレッド安全に加算
            int finished = Interlocked.Increment(ref done);

            // % へ換算
            double percent = (double)finished / total * 100.0;

            // 進捗バー描画
            DrawProgressBar(percent, finished, total);
        });

        // 行を整える
        Console.WriteLine();
        Console.WriteLine("✔ 完了しました。Enterで終了");
        Console.ReadLine();
    }

    // ★ 蛍光緑の進捗バー描画メソッド
    private static void DrawProgressBar(double progressPercent, int current, int total)
    {
        int barWidth = 40;
        int filled = (int)(barWidth * progressPercent / 100.0);

        string barFilled = new('■', filled);
        string barEmpty = new('-', barWidth - filled);

        var oldColor = Console.ForegroundColor;

        Console.Write("\r[");

        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(barFilled);

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write(barEmpty);

        Console.ForegroundColor = oldColor;
        Console.Write($"]  {progressPercent,6:F2}%  ({current}/{total})");
    }
}
