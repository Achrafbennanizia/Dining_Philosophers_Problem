# Dining Philosophers Problem (C# / .NET)

This project is a C# console implementation of the classic **Dining Philosophers Problem**.  
It simulates five philosophers sitting around a table, sharing five forks, and competing for resources using multiple threads.

The focus is on:

- **Threading & Synchronization**
- **Safe access to shared resources (locks)**
- **Deadlock avoidance**
- **Measuring total eating time per philosopher**

---

## 🧠 Problem Overview

Five philosophers sit at a round table.  
Between each pair of philosophers lies **one fork** (so 5 forks in total).

Each philosopher repeatedly:

1. **Thinks** for a short, random time  
2. **Gets hungry** and tries to:
   - take the fork on one side
   - then the fork on the other side
3. **Eats** for a short, random time (only if both forks are held)
4. Puts both forks back and starts **thinking again**

### The concurrency issue (Deadlock)

Wenn alle Philosophen gleichzeitig die linke Gabel nehmen und auf die rechte warten, kann es passieren, dass **kein Philosoph weiterkommt** → ein klassischer **Deadlock**.

In der Implementierung wird daher bewusst eine Strategie zur **Deadlock-Vermeidung** eingesetzt.

---

## 🛠️ Implementation Details

### Technologies

- **Language:** C#
- **Framework:** .NET (6/7 – works with `dotnet run`)
- **Threads:** `System.Threading.Thread`
- **Synchronization:** `Monitor.TryEnter` / `Monitor.Exit`
- **Timing:** `System.Diagnostics.Stopwatch`
- **Random delays:** `System.Random`
- **Statistics:** `Dictionary<int, int>` for total eating time per philosopher

### Core Concepts

- **Philosophers** are modeled as **threads** in a `Thread[]`.
- **Forks** are modeled as simple `object` instances in an `object[]`.
- Each philosopher gets:
  - a **left fork**
  - a **right fork** (using modulo `%` for the round table)
- Each thread runs `TryDinner(int philosoph, object fork1, object fork2)` in a loop while the simulation is active.

### Methods

The project implements (or should implement) at least:

```csharp
public static void Think()
public static void Eat(int philosoph)
public static void TryDinner(int philosoph, object fork1, object fork2)
````

* `Think()`
  Philosopher sleeps for a random time (e.g. 1–10 ms) to simulate thinking.

* `Eat(int philosoph)`
  Philosopher eats for a random time (e.g. 1–10 ms).
  The time is added to `Dictionary<int, int> eatingTime`, where the key is the philosopher’s ID and the value is the **total time spent eating (ms)**.

* `TryDinner(int philosoph, object fork1, object fork2)`
  The main loop for each philosopher:

  * Think
  * Try to take first fork via `Monitor.TryEnter`
  * If successful, try to take second fork
  * If both are acquired → `Eat(philosoph)`
  * Always release taken forks in a `finally` block

### Deadlock Avoidance Strategy

To avoid a circular wait and thus a deadlock:

* Philosophers **0–3** take forks in the order: **left → right**
* The **last philosopher (4)** takes forks in the order: **right → left**

This small change breaks the symmetric cycle of dependencies and prevents a classic deadlock scenario.

---

## 📁 Project Structure

Typical structure (for a simple console app):

```text
Dining_Philosophers_Problem/
├── Program.cs
├── Dining_Philosophers_Problem.csproj
└── README.md
```

* `Program.cs` – contains the full implementation (threads, forks, logic)
* `.csproj` – standard .NET SDK project file
* `README.md` – this file

---

## ▶️ How to Build & Run

### Prerequisites

* .NET SDK installed
  You can check with:

```bash
dotnet --version
```

### Clone the repository

```bash
git clone <your-repo-url>.git
cd Dining_Philosophers_Problem
```

### Run the simulation

```bash
dotnet run
```

You should see output similar to:

```text
=== Essensdauer pro Philosoph ===
Philosoph 0: 123 ms
Philosoph 1: 110 ms
Philosoph 2: 135 ms
Philosoph 3: 118 ms
Philosoph 4: 127 ms
=== Programm beendet ===
```

(The exact values will vary due to randomness.)

---

## ⏱️ Timing & Duration

* Each philosopher:

  * Thinks for **1–10 ms**
  * Eats for **1–10 ms**
* The entire simulation runs for about **10 seconds** (configurable via `Thread.Sleep(10000)` or a constant).
* A `Stopwatch` measures the global runtime, and eating times are accumulated per philosopher.

---

## ⚙️ Configuration Ideas (Optional)

You can easily adapt the simulation by introducing constants at the top of `Program`:

```csharp
const int NUM_PHILOSOPHERS = 5;
const int SIMULATION_DURATION_MS = 10_000;
const int MAX_THINK_MS = 10;
const int MAX_EAT_MS = 10;
```

And then use these instead of magic numbers, e.g.:

* Loop `for (int i = 0; i < NUM_PHILOSOPHERS; i++)`
* `Thread.Sleep(SIMULATION_DURATION_MS);`
* `Thread.Sleep(rnd.Next(1, MAX_THINK_MS));` etc.

---

## 🧪 Possible Extensions

If you want to extend the project further:

* Log every **state change** (Thinking → Hungry → Eating → Finished)
* Add colors to console output per philosopher
* Add command-line arguments for:

  * number of philosophers
  * simulation duration
  * max think/eat time
* Implement alternative strategies:

  * with intentional deadlock
  * with a waiter (central arbiter) pattern
  * with `SemaphoreSlim` instead of bare `Monitor`

---

## 🎯 Learning Goals

This project demonstrates:

* How to use **threads** in C#
* Safe **synchronization** with `Monitor`
* How **deadlocks** can arise and how to **avoid** them with simple strategies
* How to **measure and aggregate timing** per concurrent unit (per philosopher)

---

## 📚 References

* Classic concurrency problem: **Dining Philosophers (Dijkstra, 1965)**
* C# docs: `System.Threading.Thread`, `System.Threading.Monitor`
* C# docs: `System.Diagnostics.Stopwatch`

```
