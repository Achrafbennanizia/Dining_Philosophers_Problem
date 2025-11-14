## Dining Philosophers Problem (C#)

A small .NET console implementation of the classical Dining Philosophers concurrency problem.

This repository contains a minimal, educational simulation of five philosophers who alternately think and eat, sharing forks (locks). It demonstrates thread usage and simple deadlock avoidance strategies.

---

## Requirements

- .NET 10 SDK (the project targets `net10.0`).
- Works on macOS, Linux and Windows with the appropriate .NET SDK installed.

If you don't have the SDK installed, get it from https://dotnet.microsoft.com/.

---

## Build

Open a terminal (zsh on macOS) in the repository root and run:

```bash
dotnet build /Users/achraf/pro/C#_Net/Dining_Philosophers_Problem/Dining_Philosophers_Problem.csproj
```

---

## Run

Run the console application with:

```bash
dotnet run --project /Users/achraf/pro/C#_Net/Dining_Philosophers_Problem/Dining_Philosophers_Problem.csproj
```

The program runs the simulation for 10 seconds and then prints the total eating time per philosopher, for example:

```
=== Essensdauer pro Philosoph ===
Philosoph 0: 2787 ms
Philosoph 1: 2885 ms
Philosoph 2: 2883 ms
Philosoph 3: 3225 ms
Philosoph 4: 2850 ms
=== Programm beendet ===
```

---

## How the program works (short)

- The program creates N philosophers (default 5).
- Each philosopher runs on its own thread and loops while a shared `running` flag is true.
- A philosopher alternates between `Think()` and `Eat()`.
- There is an object array `forks` representing each fork (lock).
- To pick up forks, the code uses `Monitor.TryEnter` with a small timeout. If the first fork can't be taken, the philosopher continues thinking; if the second can't be taken within the timeout, they release the first and try again later.
- To avoid a simple deadlock, the last philosopher acquires the forks in reversed order (right first) so not all philosophers try to take the same sequence.

---

## Implementation details (current code)

The current `Program.cs` implements the simulation using:

- `object[] forks` — per-fork locks.
- `Dictionary<int,int> eatingTime` — keeps accumulated eating time per philosopher.
- `Random rnd` (single shared instance) used to generate think/eat durations.
- `Monitor.TryEnter(fork, timeout, ref taken)` — the ref overload which sets `taken` when the lock is acquired.
- `Thread` array and lambdas to start philosopher threads.
- The main thread sleeps 10 seconds then flips `running = false` and joins the threads.

The file path: `Program.cs`.

---

## Known issues and suggestions

This project is intentionally simple and educational. The current implementation works for demonstrations but contains some thread-safety issues and places where behavior could be improved. Below are the important ones and suggested fixes:

1. Shared System.Random
   - Problem: `System.Random` is not thread-safe. Using a single static instance from multiple threads causes race conditions and unpredictable results.
   - Suggestion: Use `ThreadLocal<Random>` or `System.Random.Shared` (with caution) or better, `System.Security.Cryptography.RandomNumberGenerator` for cryptographically secure randomness if required. Example fix:

```csharp
static ThreadLocal<Random> rnd = new ThreadLocal<Random>(() => new Random());
// use rnd.Value.Next(...)
```

2. `Dictionary<int,int>` is not thread-safe for concurrent writes
   - Problem: Multiple threads update `eatingTime[philosoph] += t;` which is a read-modify-write and not atomic; `Dictionary` is not safe for concurrent writes.
   - Suggestion: Replace the dictionary with an `int[]` sized to the number of philosophers and update entries using `Interlocked.Add(ref eatingTime[i], t);` or protect updates with a lock.

3. Monitor.TryEnter overload usage and taken flags
   - Problem: The current code uses `Monitor.TryEnter(fork, timeout, ref taken)`. This overload sets `taken` to true if the lock is acquired. That's OK, but be mindful to correctly release only if `taken` is true. Alternatively, use the boolean-returning overload `Monitor.TryEnter(fork, timeout)` which returns true/false.

4. Timeout/backoff tuning
   - The `TryEnter` timeout is currently very small (5 ms). Depending on workload this leads to heavy spinning and CPU usage. Consider using a backoff strategy (short sleep) or increasing the timeout.

5. Optional: Use higher-level concurrency primitives
   - Consider using `SemaphoreSlim`, `Mutex`, or higher-level constructs, or re-architect to avoid explicit locks (channels/actors) if you plan to expand the simulation.

6. Thread capture of loop variable
   - The original code had a classic closure bug when creating threads inside a loop (the loop variable `i` captured by lambdas). The current file already captures `id = i` before creating the lambda which avoids that bug. Keep that pattern when modifying thread creation logic.

---

## Suggested small improvements to add (I can implement any of these):

- Replace `Dictionary<int,int>` with `int[]` and use `Interlocked.Add`.
- Replace shared `Random` with `ThreadLocal<Random>`.
- Improve locking/backoff strategy and increase TryEnter timeout or add a small sleep when failing to acquire locks.
- Add command-line options to configure number of philosophers and run duration.
- Add unit/integration tests for the simulation harness.

---

## Contributing

Contributions are welcome. Open an issue or a pull request to discuss changes.

---

## License

This repository uses the MIT license. If you want a different license, update this section accordingly.
# Dining_Philosophers_Problem
