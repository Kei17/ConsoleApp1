using System;
using System.IO;

class Program
{
    static ConsoleColor[] szinek = { ConsoleColor.White, ConsoleColor.Blue, ConsoleColor.DarkYellow, ConsoleColor.Green, ConsoleColor.Red };
    static string[] szinNevek = { "Fehér", "Kék", "Sárga", "Zöld", "Piros" };
    static int aktualisSzinIndex = 0;
    static ConsoleColor aktualisSzin = ConsoleColor.White;

    static string[] karakterek = { "█", "▓", "░", "●", "■" };
    static string[] karakterNevek = { "Teli blokk", "Félig teli blokk", "Kissé teli blokk", "Kör", "Négyzet" };
    static int aktualisKarakterIndex = 0;
    static string aktualisKarakter = "█";

    static (string karakter, ConsoleColor szin)[,] vaszon;

    static bool radirMod = false;
    static string rajzokDirectory = "Rajzok";

    static void Main(string[] args)
    {
        if (!Directory.Exists(rajzokDirectory))
        {
            Directory.CreateDirectory(rajzokDirectory);
        }

        vaszon = new (string karakter, ConsoleColor szin)[Console.WindowWidth, Console.WindowHeight];
        UjVaszon();
    }

    static void UjVaszon()
    {
        vaszon = new (string karakter, ConsoleColor szin)[Console.WindowWidth, Console.WindowHeight];
        Console.Clear();
        RajzMenu();

        int cursorX = 0;
        int cursorY = 0;
        Console.SetCursorPosition(cursorX, cursorY);

        while (true)
        {
            if (Console.KeyAvailable)
            {
                ConsoleKey key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.C:
                        SzintValaszt();
                        RajzMenu();
                        break;
                    case ConsoleKey.K:
                        KaraktertValaszt();
                        RajzMenu();
                        break;
                    case ConsoleKey.R:
                        radirMod = !radirMod;
                        Console.Clear();
                        Console.WriteLine(radirMod ? "Radír mód: BE" : "Radír mód: KI");
                        RajzMenu();
                        break;
                    case ConsoleKey.D1:
                        UjVaszon();
                        return;
                    case ConsoleKey.D2:
                        MentesRajz();
                        RajzMenu();
                        break;
                    case ConsoleKey.D3:
                        ListazRajzok();
                        return;
                    case ConsoleKey.D4:
                        TorlesRajz();
                        RajzMenu();
                        break;
                    case ConsoleKey.D5:
                        return;
                    case ConsoleKey.M:
                        MegnyitMenu();
                        RajzMenu();
                        break;
                }

                switch (key)
                {
                    case ConsoleKey.LeftArrow:
                        if (cursorX > 0)
                            cursorX--;
                        break;
                    case ConsoleKey.RightArrow:
                        if (cursorX < Console.WindowWidth - 1)
                            cursorX++;
                        break;
                    case ConsoleKey.UpArrow:
                        if (cursorY > 0)
                            cursorY--;
                        break;
                    case ConsoleKey.DownArrow:
                        if (cursorY < Console.WindowHeight - 2)
                            cursorY++;
                        break;
                }

                if (radirMod)
                {
                    vaszon[cursorX, cursorY] = (" ", Console.BackgroundColor);
                }
                else
                {
                    vaszon[cursorX, cursorY] = (aktualisKarakter, aktualisSzin);
                }

                UjrarajzolVaszon();
                Console.SetCursorPosition(cursorX, cursorY);
            }
        }
    }

    static void RajzMenu()
    {
        Console.SetCursorPosition(0, Console.WindowHeight - 1);
        Console.WriteLine("Gombok és funkciók:");
        Console.WriteLine("--------------------");
        Console.WriteLine("C = Színváltás");
        Console.WriteLine("K = Karakterváltás");
        Console.WriteLine("R = Radír be-ki kapcsolása");
        Console.WriteLine("1 = Új vászon");
        Console.WriteLine("2 = Mentés");
        Console.WriteLine("3 = Szerkesztés");
        Console.WriteLine("4 = Törlés");
        Console.WriteLine("5 = Kilépés");
        Console.WriteLine("M = Menü");
    }

    static void MentesRajz()
    {
        Console.Clear();
        Console.Write("Add meg a rajz nevét: ");
        string nev = Console.ReadLine();
        string filePath = Path.Combine(rajzokDirectory, nev + ".txt");

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            for (int y = 0; y < Console.WindowHeight - 1; y++)
            {
                for (int x = 0; x < Console.WindowWidth; x++)
                {
                    (string karakter, ConsoleColor szin) cella = vaszon[x, y];
                    writer.Write($"{cella.karakter},{(int)cella.szin}|");
                }
                writer.WriteLine();
            }
        }

        Console.WriteLine($"Rajz mentve: {nev}");
        Console.ReadKey();
    }

    static void ListazRajzok()
    {
        Console.Clear();
        string[] fajlok = Directory.GetFiles(rajzokDirectory, "*.txt");

        if (fajlok.Length == 0)
        {
            Console.WriteLine("Nincs mentett rajz.");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Mentett rajzok:");
        for (int i = 0; i < fajlok.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {Path.GetFileNameWithoutExtension(fajlok[i])}");
        }

        Console.Write("Válassz egy rajzot a folytatáshoz (szám): ");
        int valasztas = int.Parse(Console.ReadLine()) - 1;

        if (valasztas >= 0 && valasztas < fajlok.Length)
        {
            BetoltRajz(fajlok[valasztas]);
        }
    }

    static void BetoltRajz(string filePath)
    {
        vaszon = new (string karakter, ConsoleColor szin)[Console.WindowWidth, Console.WindowHeight];

        string[] sorok = File.ReadAllLines(filePath);

        for (int y = 0; y < sorok.Length; y++)
        {
            string[] cellak = sorok[y].Split('|');
            for (int x = 0; x < cellak.Length - 1; x++)
            {
                string[] adatok = cellak[x].Split(',');
                vaszon[x, y] = (adatok[0], (ConsoleColor)int.Parse(adatok[1]));
            }
        }

        Console.WriteLine($"Rajz betöltve: {Path.GetFileNameWithoutExtension(filePath)}");
        Console.ReadKey();
        UjVaszon();
    }

    static void TorlesRajz()
    {
        Console.Clear();
        string[] fajlok = Directory.GetFiles(rajzokDirectory, "*.txt");

        if (fajlok.Length == 0)
        {
            Console.WriteLine("Nincs mentett rajz.");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Mentett rajzok:");
        for (int i = 0; i < fajlok.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {Path.GetFileNameWithoutExtension(fajlok[i])}");
        }

        Console.Write("Válassz egy rajzot a törléshez (szám): ");
        int valasztas = int.Parse(Console.ReadLine()) - 1;

        if (valasztas >= 0 && valasztas < fajlok.Length)
        {
            File.Delete(fajlok[valasztas]);
            Console.WriteLine("Rajz törölve.");
        }

        Console.ReadKey();
    }

    static void MegnyitMenu()
    {
        bool kilepes = false;

        while (!kilepes)
        {
            Console.Clear();
            Console.WriteLine("Menü:");
            Console.WriteLine("1 = Új vászon");
            Console.WriteLine("2 = Mentés");
            Console.WriteLine("3 = Szerkesztés");
            Console.WriteLine("4 = Törlés");
            Console.WriteLine("5 = Kilépés");
            Console.WriteLine("Válassz egy opciót (vagy nyomj Esc-t a visszalépéshez):");

            ConsoleKey key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.D1:
                    UjVaszon();
                    return;
                case ConsoleKey.D2:
                    MentesRajz();
                    break;
                case ConsoleKey.D3:
                    ListazRajzok();
                    break;
                case ConsoleKey.D4:
                    TorlesRajz();
                    break;
                case ConsoleKey.D5:
                    kilepes = true;
                    break;
                case ConsoleKey.Escape:
                    kilepes = true;
                    break;
            }
        }
    }

    static void SzintValaszt()
    {
        int valasztottSzin = 0;
        bool szinKivalasztva = false;

        Console.Clear();
        Console.WriteLine("Nyílbillentyűkkel válassz egy színt, majd nyomj Enter-t:");

        while (!szinKivalasztva)
        {
            for (int i = 0; i < szinek.Length; i++)
            {
                if (i == valasztottSzin)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                }

                Console.ForegroundColor = szinek[i];
                Console.Write("  {0}  ", szinNevek[i]);
                Console.ResetColor();
                Console.WriteLine();
            }

            ConsoleKey key = Console.ReadKey(true).Key;
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    if (valasztottSzin > 0)
                        valasztottSzin--;
                    break;
                case ConsoleKey.DownArrow:
                    if (valasztottSzin < szinek.Length - 1)
                        valasztottSzin++;
                    break;
                case ConsoleKey.Enter:
                    aktualisSzinIndex = valasztottSzin;
                    aktualisSzin = szinek[aktualisSzinIndex];
                    szinKivalasztva = true;
                    break;
            }

            Console.Clear();
            Console.WriteLine("Nyílbillentyűkkel válassz egy színt, majd nyomj Enter-t:");
        }

        Console.Clear();
    }

    static void KaraktertValaszt()
    {
        int valasztottKarakter = 0;
        bool karakterKivalasztva = false;

        Console.Clear();
        Console.WriteLine("Nyílbillentyűkkel válassz egy karaktert, majd nyomj Enter-t:");

        while (!karakterKivalasztva)
        {
            for (int i = 0; i < karakterek.Length; i++)
            {
                if (i == valasztottKarakter)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                }

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("  {0}  ", karakterNevek[i]);
                Console.ResetColor();
                Console.WriteLine();
            }

            ConsoleKey key = Console.ReadKey(true).Key;
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    if (valasztottKarakter > 0)
                        valasztottKarakter--;
                    break;
                case ConsoleKey.DownArrow:
                    if (valasztottKarakter < karakterek.Length - 1)
                        valasztottKarakter++;
                    break;
                case ConsoleKey.Enter:
                    aktualisKarakterIndex = valasztottKarakter;
                    aktualisKarakter = karakterek[aktualisKarakterIndex];
                    karakterKivalasztva = true;
                    break;
            }

            Console.Clear();
            Console.WriteLine("Nyílbillentyűkkel válassz egy karaktert, majd nyomj Enter-t:");
        }

        Console.Clear();
    }

    static void UjrarajzolVaszon()
    {
        Console.Clear();
        for (int y = 0; y < Console.WindowHeight - 1; y++)
        {
            for (int x = 0; x < Console.WindowWidth; x++)
            {
                if (vaszon[x, y].karakter != null && vaszon[x, y].karakter != "")
                {
                    Console.SetCursorPosition(x, y);
                    Console.ForegroundColor = vaszon[x, y].szin;
                    Console.Write(vaszon[x, y].karakter);
                }
            }
        }
        RajzMenu();
    }
}
