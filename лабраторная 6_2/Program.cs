using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace лабраторная_6_2
{
    
    class Grandma
    {
        public string Name { get; set; }
        public int Age { get; set; }
        // болезнь -> лекарство
        public Dictionary<string, string> Diseases { get; set; } = new Dictionary<string, string>();

        public override string ToString()
        {
            if (Diseases.Count == 0)
                return $"{Name}, {Age} лет — болезней нет (хочет только спросить)";

            var items = string.Join(", ",
                Diseases.Select(kv => $"{kv.Key} (лекарство: {kv.Value})"));
            return $"{Name}, {Age} лет — болезни: {items}";
        }
    }

    class Hospital
    {
        public string Name { get; set; }
        public HashSet<string> Treats { get; set; }
        public int Capacity { get; set; }
        public List<Grandma> Patients { get; set; } = new List<Grandma>();

        public Hospital(string name, IEnumerable<string> treats, int capacity)
        {
            Name = name;
            Treats = new HashSet<string>(treats, StringComparer.OrdinalIgnoreCase);
            Capacity = capacity;
        }

        public bool HasSpace => Patients.Count < Capacity;

        public double LoadPercent =>
            Capacity == 0 ? 0 : 100.0 * Patients.Count / Capacity;

        public override string ToString()
        {
            var treats = Treats.Count > 0 ? string.Join(", ", Treats) : "—";
            return $"Больница «{Name}» | лечит: {treats} | " +
                   $"пациентов: {Patients.Count}/{Capacity} " +
                   $"({LoadPercent:F1}% заполнено)";
        }
    }

  

    class Program
    {
        static string Ask(string prompt)
        {
            Console.Write(prompt);
            return (Console.ReadLine() ?? "").Trim();
        }

        static int AskInt(string prompt, int minimum = 0)
        {
            while (true)
            {
                var s = Ask(prompt);
                if (int.TryParse(s, out int v) && v >= minimum) return v;
                Console.WriteLine($"  ⚠ Введите целое число >= {minimum}");
            }
        }

        static List<string> AskDiseaseList(string prompt = "  Болезни через запятую: ")
        {
            var s = Ask(prompt);
            if (string.IsNullOrWhiteSpace(s)) return new List<string>();
            return s.Split(',')
                    .Select(x => x.Trim())
                    .Where(x => x.Length > 0)
                    .ToList();
        }

        // ─────────────────── ВВОД БОЛЬНИЦ (СТЕК) ───────────────────────
        static List<Hospital> InputHospitals()
        {
            var stack = new List<Hospital>();     // вершина = последний элемент
            int n = AskInt("Сколько больниц? ", 1);

            for (int i = 0; i < n; i++)
            {
                Console.WriteLine($"\n--- Больница #{i + 1} ---");
                string name = Ask("  Название: ");
                var treats = AskDiseaseList("  Какие болезни лечит (через запятую): ");
                int cap = AskInt("  Вместимость: ", 1);
                stack.Add(new Hospital(name, treats, cap));
            }
            return stack;
        }

        // ──────────── ЛОГИКА ПОДБОРА БОЛЬНИЦЫ ──────────────────────────
        // Стек больниц: идём от вершины (последняя добавленная) вниз.
        // Условие: лечат СТРОГО > 50 % болезней бабули.
        // Если болезней нет — первая свободная клиника.
        static Hospital FindHospital(Grandma g, List<Hospital> hospitals)
        {
            for (int i = hospitals.Count - 1; i >= 0; i--)
            {
                var h = hospitals[i];
                if (!h.HasSpace) continue;

                if (g.Diseases.Count == 0) return h;   // «хочет только спросить»

                int treated = g.Diseases.Keys.Count(d => h.Treats.Contains(d));
                if (treated * 2 > g.Diseases.Count) return h;
            }
            return null;
        }

       
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== ВВОД БОЛЬНИЦ ===");
            var hospitals = InputHospitals();

            Console.WriteLine("\n=== ВВОД БАБУЛЬ ===");
        }

    }
}цукенг