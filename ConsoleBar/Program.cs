using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

class Program
{
    private static readonly char[] SpinnerChars = { '|', '/', '-', '\\' };
    private static int _spinnerIndex = 0;
    private static readonly object ConsoleLock = new();

    static async Task Main()
    {
        // 一応 UTF-8 にしておく（スピナーなどで Unicode 使うなら）
        Console.OutputEncoding = Encoding.UTF8;

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
            int ms = Random.Shared.Next(500, 2500);
            await Task.Delay(ms, token);

            int finished = Interlocked.Increment(ref done);
            double percent = (double)finished / total * 100.0;
            int spinnerIndex = Interlocked.Increment(ref _spinnerIndex);

            DrawProgressBar(percent, finished, total, spinnerIndex);
        });

        Console.WriteLine();
        Console.WriteLine("✔ 完了しました。Enterで終了");
        Console.ReadLine();
    }

    private static void DrawProgressBar(double progressPercent, int current, int total, int spinnerIndex)
    {
        int barWidth = 40;
        int filled = (int)(barWidth * progressPercent / 100.0);
        int empty = barWidth - filled;

        char spinner = SpinnerChars[spinnerIndex % SpinnerChars.Length];

        lock (ConsoleLock)
        {
            var oldFg = Console.ForegroundColor;
            var oldBg = Console.BackgroundColor;

            Console.Write("\r");

            // スピナー
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(spinner);
            Console.Write(' ');

            // バー本体
            Console.ForegroundColor = oldFg;
            Console.Write('[');

            // ★ filled 部分を背景色で塗る（文字はスペース）
            Console.BackgroundColor = ConsoleColor.Green;
            Console.Write(new string(' ', filled));

            // 空き部分
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.Write(new string(' ', empty));

            // [] の右側は元の色に戻す
            Console.BackgroundColor = oldBg;
            Console.Write(']');

            Console.Write($"  {progressPercent,6:F2}%  ({current}/{total})");
        }
    }
}
