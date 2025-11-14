using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

class Program
{
    static Random rnd = new Random();
    static Dictionary<int, int> eatingTime = new Dictionary<int, int>();
    static bool running = true;

    static void Main(string[] args)
    {
        int philosophers = 5;

        // Gabeln (Locks)
        object[] forks = new object[philosophers];
        for (int i = 0; i < philosophers; i++)
            forks[i] = new object();

        // Dictionary initialisieren
        for (int i = 0; i < philosophers; i++)
            eatingTime[i] = 0;

        // Philosophen-Threads
        Thread[] threads = new Thread[philosophers];

        for (int i = 0; i < philosophers; i++)
        {
            int id = i; // capture loop variable for the thread
            int left = id;
            int right = (id + 1) % philosophers;

            // Deadlock-Vermeidung: letzter Philosoph nimmt erst rechts!
            if (id == philosophers - 1)
                threads[id] = new Thread(() => TryDinner(id, forks[right], forks[left]));
            else
                threads[id] = new Thread(() => TryDinner(id, forks[left], forks[right]));
        }

        Stopwatch sw = Stopwatch.StartNew();

        // Threads starten
        foreach (Thread t in threads)
            t.Start();

        // 10 Sekunden laufen lassen
        Thread.Sleep(10000);
        running = false;

        // Warten bis fertig
        foreach (Thread t in threads)
            t.Join();

        sw.Stop();

        // Ergebnis
        Console.WriteLine("=== Essensdauer pro Philosoph ===");
        foreach (var kv in eatingTime)
            Console.WriteLine($"Philosoph {kv.Key}: {kv.Value} ms");

        Console.WriteLine("=== Programm beendet ===");
    }

    public static void Think()
    {
        Thread.Sleep(rnd.Next(1, 10));
    }

    public static void Eat(int philosoph)
    {
        int t = rnd.Next(1, 10);
        eatingTime[philosoph] += t;
        Thread.Sleep(t);
    }

    public static void TryDinner(int philosoph, object fork1, object fork2)
    {
        while (running)
        {
            Think();

            bool taken1 = false;
            bool taken2 = false;

            try
            {
                // Erste Gabel nehmen
                Monitor.TryEnter(fork1, 5, ref taken1);

                if (!taken1)
                    continue; // weiter denken

                // Zweite Gabel nehmen
                Monitor.TryEnter(fork2, 5, ref taken2);

                if (!taken2)
                    continue; // Gabel 2 nicht verfügbar → später weiter

                // beide Gabeln → essen!
                Eat(philosoph);
            }
            finally
            {
                // Gabeln freigeben
                if (taken1) Monitor.Exit(fork1);
                if (taken2) Monitor.Exit(fork2);
            }
        }
    }
}
