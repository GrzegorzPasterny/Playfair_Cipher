using System;
using System.Text;

namespace Playfair_Cipher
{
    class Program
    {
        static void Main(string[] args)
        {
			String tekstJawny = "";
			String klucz = "";
			Console.WriteLine("Program szyfrujacy metoda PlayFair.");
			Console.WriteLine("Podaj klucz (bez polskich znakow, cyfr i znakow specjalnych):");
			klucz = Console.ReadLine();
			Console.WriteLine("Podaj tekst do zaszyfrowania (bez polskich znakow, cyfr i znakow specjalnych):");
			tekstJawny = Console.ReadLine();
			Console.WriteLine("Klucz: " + klucz);
			Console.WriteLine("Tekst do szyfrowania: " + tekstJawny);
			klucz = przygotowanieKlucza(klucz);
			tekstJawny = przygotowanieTekstuJawnego(tekstJawny);
			char[,] tab = new char[5,5];
			generowanieTablicy(klucz, tab);
			String tekstZaszyfrowany = szyfrowanie(tekstJawny, tab);
			String tekstOdszyfrowany = odszyfrowywanie(tekstZaszyfrowany, tab);
			Console.WriteLine("Tekst zaszyfrowany: " + tekstZaszyfrowany);
			Console.WriteLine("Tekst odszyfrowany: " + tekstOdszyfrowany);
		}

		private static String szyfrowanie(String tekstJawny, char[,] tablicaSzyfru)
		{
			Tuple<int, int> koordynatyPierwszego;
			Tuple<int, int> koordynatyDrugiego;
			String tekstZaszyfrowany = "";

			//Rozpoczynamy szyfrowanie zakladajac parzysta liczbe znakow w tekscie jawnym
			for (int x = 0; x < tekstJawny.Length; x += 2)
			{
				//Wyszukiwanie koordynatow obu znakow digrafu
				koordynatyPierwszego = znajdzKoordynatyZnaku(tekstJawny.ToCharArray()[x], tablicaSzyfru);
				koordynatyDrugiego = znajdzKoordynatyZnaku(tekstJawny.ToCharArray()[x + 1], tablicaSzyfru);

				if (koordynatyPierwszego == null || koordynatyDrugiego == null)
				{
					Console.WriteLine("NIE ZNALEZIONO ZNAKU W TABLICY! - Zakonczenie pracy");
					return "";
				}

				//Pierwszy przypadek
				if (koordynatyPierwszego.Item1 == koordynatyDrugiego.Item1)
				{
					//Jesli znaki sa w tej samej kolumnie zamieniamy je na znaki znajdujace sie nizej
					//Dzialanie modulo 5 jest potrzebne, aby uchronic sie od wyjscia poza zakres tablicy
					//w przypadku znaku bedacego ostatnim znakiem w kolumnie
					tekstZaszyfrowany += tablicaSzyfru[koordynatyPierwszego.Item1, (koordynatyPierwszego.Item2 + 1) % 5];
					tekstZaszyfrowany += tablicaSzyfru[koordynatyDrugiego.Item1, (koordynatyDrugiego.Item2 + 1) % 5];
				}
				//Drugi przypadek
				else if (koordynatyPierwszego.Item2 == koordynatyDrugiego.Item2)
				{
					//Jesli znaki sa w tym samym wierszu zamieniamy je na znaki znajdujace znajdujace sie po prawej
					//Dzialanie modulo 5 jest potrzebne, aby uchronic sie od wyjscia poza zakres tablicy
					//w przypadku znaku bedacym ostatnim znakiem w wierszu
					tekstZaszyfrowany += tablicaSzyfru[(koordynatyPierwszego.Item1 + 1) % 5, koordynatyPierwszego.Item2];
					tekstZaszyfrowany += tablicaSzyfru[(koordynatyDrugiego.Item1 + 1) % 5, koordynatyDrugiego.Item2];
				}
				//Pozostale przypadki
				else
				{
					//Szukamy znakow na przecieciu wiersza znaku pierwszego z kolumna drugiego oraz
					//przeciecia wiersza drugiego z kolumna pierwszego
					tekstZaszyfrowany += tablicaSzyfru[koordynatyDrugiego.Item1, koordynatyPierwszego.Item2];
					tekstZaszyfrowany += tablicaSzyfru[koordynatyPierwszego.Item1, koordynatyDrugiego.Item2];
				}
			}

			return tekstZaszyfrowany;
		}

		private static Tuple<int, int> znajdzKoordynatyZnaku(char znak, char[,] tablica)
		{ //wykorzystujemy istniejaca juz klase SimpleEntry z biblioteki java.util.AbstractMap

			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j < 5; j++)
				{
					if (tablica[i,j] == znak)
					{
						Tuple<int, int> koordynaty = Tuple.Create(i, j);
						
						return koordynaty;
					}
				}
			}
			return null;
		}

		private static String prepare(String wyraz)
		{
			//Usuniecie spacji
			wyraz = wyraz.Replace(" ", "");


			//Zmiana wszystkich liter na male
			wyraz = wyraz.ToLower();


			//zamiana liter j na i
			wyraz = wyraz.Replace("j", "i");

			return wyraz;
		}
		private static String przygotowanieKlucza(String wyraz)
		{
			if (wyraz.Length == 0)
			{
				wyraz = "binary";
			}
			return prepare(wyraz);
		}

		private static String przygotowanieTekstuJawnego(String wyraz)
		{
			if (wyraz.Length == 0)
			{
				wyraz = "programowanie";
			}
			wyraz = prepare(wyraz);
			if (wyraz.Length % 2 != 0)
			{
				wyraz = wyraz + "z";
			}
			return wyraz;
		}

		private static void generowanieTablicy(String klucz, char[,] tablicaSzyfru)
		{

			String kontrolny = "";
			String alfabet = "abcdefghiklmnopqrstuvwxyz";


			int x = 0, y = 0;
			int pozycja;

			//Korzystajac z zaawansowanej petli przechodzimy przez kolejne znaki w kluczu
			foreach (char znak in klucz.ToCharArray())
			{
				//Sprawdzamy czy dodany znak  znajduje się już w tablicy
				pozycja = kontrolny.IndexOf(znak);

				//Jesli nie dodajemy do tablicy i stringa kontrolnego
				if (pozycja == -1)
				{
					if (y == 5)
					{
						//Jesli tekscie jawnym znajduja sie inne znaki oprocz liter
						//wielkosc tablicy zostanie przekroczona - program nie moze wykonac swojego dzialania
						Console.WriteLine("BLAD PODCZAS SZYFROWANIA! - Za duza ilosc znakow w tablicy");
						return;
					}

					//Dodajemy znak do kontrolnego stringa i tablicy
					kontrolny += znak;
					tablicaSzyfru[x,y] = znak;
					x++;

					//Jesli doszlismy do konca wiersza przechodzimy do kolejnego
					if (x == 5)
					{
						x = 0;
						y++;
					}

				}
			}

			foreach (char znak in alfabet.ToCharArray())
			{
				pozycja = kontrolny.IndexOf(znak);

				if (pozycja == -1)
				{
					if (y == 5)
					{
						Console.WriteLine("BLAD PODCZAS SZYFROWANIA! - Za duza ilosc znakow w tablicy");
						return;
					}

					kontrolny += znak;
					tablicaSzyfru[x,y] = znak;
					x++;

					if (x == 5)
					{
						x = 0;
						y++;
					}
				}
			}

			Console.WriteLine("Tablica szyfrowania:");
			printTab(tablicaSzyfru);

		}

		private static void printTab(char[, ] tablicaSzyfru)
		{
			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j < 5; j++)
				{
					Console.Write(tablicaSzyfru[j, i] + " | ");
				}
				Console.WriteLine();
			}
		}

		private static String odszyfrowywanie(String tekstZaszyfrowany, char[,] tablicaSzyfru)
		{
			Tuple<int,int> koordynatyPierwszego;
			Tuple<int, int> koordynatyDrugiego;
			String tekstJawny = "";

			//Rozpoczynamy szyfrowanie zakladajac parzysta liczbe znakow w tekscie jawnym
			for (int x = 0; x < tekstZaszyfrowany.Length; x += 2)
			{
				//Wyszukiwanie koordynatow obu znakow digrafu
				koordynatyPierwszego = znajdzKoordynatyZnaku(tekstZaszyfrowany.ToCharArray()[x], tablicaSzyfru);
				koordynatyDrugiego = znajdzKoordynatyZnaku(tekstZaszyfrowany.ToCharArray()[x + 1], tablicaSzyfru);

				if (koordynatyPierwszego == null || koordynatyDrugiego == null)
				{
					Console.WriteLine("NIE ZNALEZIONO ZNAKU W TABLICY! - Zakonczenie pracy");
					return "";
				}
				//Pierwszy przypadek
				if (koordynatyPierwszego.Item1 == koordynatyDrugiego.Item1)
				{
					//Jesli znaki sa w tej samej kolumnie zamieniamy je na znaki znajdujace sie wyzej
					//Jesli znak znajduje sie na poczatku kolumny, bierzemy ostatni znak z tej kolumny
					if (koordynatyPierwszego.Item2 - 1 < 0)
					{
						tekstJawny += tablicaSzyfru[koordynatyPierwszego.Item1, 4];
					}
					else
					{
						tekstJawny += tablicaSzyfru[koordynatyPierwszego.Item1, koordynatyPierwszego.Item2 - 1];
					}

					if (koordynatyDrugiego.Item2 - 1 < 0)
					{
						tekstJawny += tablicaSzyfru[koordynatyDrugiego.Item1, 4];
					}
					else
					{
						tekstJawny += tablicaSzyfru[koordynatyDrugiego.Item1, koordynatyDrugiego.Item2 - 1];
					}



				}
				//Drugi przypadek
				else if (koordynatyPierwszego.Item2 == koordynatyDrugiego.Item2)
				{
					//Jesli znaki sa w tym samym wierszu zamieniamy je na znaki znajdujace znajdujace sie po lewej
					//Jesli znak znajduje sie na poczatku wiersza, bierzemy ostatni znak z tego wiersza
					if (koordynatyPierwszego.Item1 - 1 < 0)
					{
						tekstJawny += tablicaSzyfru[4, koordynatyPierwszego.Item2];
					}
					else
					{
						tekstJawny += tablicaSzyfru[koordynatyPierwszego.Item1 - 1, koordynatyPierwszego.Item2];
					}

					if (koordynatyDrugiego.Item1 - 1 < 0)
					{
						tekstJawny += tablicaSzyfru[4, koordynatyDrugiego.Item2];
					}
					else
					{
						tekstJawny += tablicaSzyfru[koordynatyDrugiego.Item1 - 1, koordynatyDrugiego.Item2];
					}
				}
				//Pozostale przypadki
				else
				{
					//Szukamy znakow na przecieciu wiersza znaku pierwszego z kolumna drugiego oraz
					//przeciecia wiersza drugiego z kolumna pierwszego
					tekstJawny += tablicaSzyfru[koordynatyDrugiego.Item1, koordynatyPierwszego.Item2];
					tekstJawny += tablicaSzyfru[koordynatyPierwszego.Item1, koordynatyDrugiego.Item2];
				}
			}

			return tekstJawny;
		}
	}
}
