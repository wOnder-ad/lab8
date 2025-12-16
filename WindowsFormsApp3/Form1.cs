using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public class MyBook : IComparable
        {
            public int bookNomer { get; set; }
            public String Avtor { get; set; }
            public String Nazva { get; set; }
            public String Vydavnyctvo { get; set; }
            public Int16 RikVyhodu { get; set; }

            // Ініціалізація об'єкта: заповнюємо властивості конкретними даними при створенні
            public MyBook(int bookNomer, String Avtor, String Nazva, String Vydavnyctvo, Int16 RikVyhodu)
            {
                this.bookNomer = bookNomer; this.Avtor = Avtor; this.Nazva = Nazva; this.Vydavnyctvo = Vydavnyctvo;
                this.RikVyhodu = RikVyhodu;
            }

            // Перевизначення методу для зручного перетворення об'єкта у текстовий рядок
            public override string ToString()
            {
                return "Книга №" + bookNomer.ToString() + " Автор: " + Avtor + " Назва: " + Nazva + " Видавництво: "
              + Vydavnyctvo + " Рік: " + RikVyhodu.ToString();
            }

            // Реалізація логіки порівняння для автоматичного сортування
            int IComparable.CompareTo(object obj)
            {
                MyBook pobj = obj as MyBook; // Спроба перетворити отриманий універсальний об'єкт у тип MyBook

                if (pobj != null)  // Перевірка успішності перетворення (чи дійсно нам передали книгу)
                {
                    // Логіка порівняння за ID:
                    // Повертаємо 0, якщо номери рівні
                    // 1, якщо поточна книга має більший номер
                    // -1, якщо поточна книга має менший номер
                    if (this.bookNomer == pobj.bookNomer) return 0;
                    if (this.bookNomer > pobj.bookNomer) return 1;
                    if (this.bookNomer < pobj.bookNomer) return -1;
                    else return 0;
                }
                else // Генеруємо помилку, якщо спробували порівняти книгу з чимось іншим (наприклад, з числом)
                    throw new ArgumentException("Помилка: об'єкт для порівняння не є книгою (тип MyBook)");
            }
        }

        public class MyBooks : ArrayList, IEnumerable
        {
            public MyBook[] MyBooksArray { get; set; }

            // Конструктор колекції: виділяє пам'ять під масив заданої ємності
            public MyBooks(int kilkistKnyh)
            {
                MyBooksArray = new MyBook[kilkistKnyh];
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Змінна-буфер для накопичення всього тексту, який потім покажемо користувачу
            string ss = "--- 1. Початковий стан масиву (без сортування) --- \n";
            ss = ss + "\n"; // Додаємо порожній рядок для візуального розділення блоків

            MyBooks mbs1 = new MyBooks(100); // Створення контейнера для книг

            // Заповнення масиву тестовими даними (Фантастика/Кіберпанк)
            // Зверніть увагу: ID йдуть у хаотичному порядку (101 -> 42 -> 1 -> 50 -> 13)
            mbs1.MyBooksArray[0] = new MyBook(101, "Вільям Гібсон", "Нейромант", "Ace Books", 1984);
            mbs1.MyBooksArray[1] = new MyBook(42, "Дуглас Адамс", "Автостопом по галактиці", "Pan Books", 1979);
            mbs1.MyBooksArray[2] = new MyBook(1, "Айзек Азімов", "Фундація", "Gnome Press", 1951);
            mbs1.MyBooksArray[3] = new MyBook(50, "Філіп К. Дік", "Чи мріють андроїди...", "Doubleday", 1968);
            mbs1.MyBooksArray[4] = new MyBook(13, "Станіслав Лем", "Соляріс", "Мон", 1961);

            // Цикл для перебору та запису невідсортованих даних у рядок виводу
            foreach (MyBook b in mbs1.MyBooksArray)
            {
                if (b != null) ss = ss + b.ToString() + "\n";
            }

            ss = ss + "\n"; // Візуальний відступ перед наступним блоком

            // Запуск сортування. Array.Sort автоматично використовує наш метод CompareTo
            Array.Sort(mbs1.MyBooksArray);

            // Формування заголовка для відсортованої частини
            ss = ss + "--- 2. Масив після автоматичного сортування (за номером ID) --- \n";
            ss = ss + "\n";

            // Виведення результату: книги мають вишикуватися як: 1, 13, 42, 50, 101
            foreach (MyBook b in mbs1.MyBooksArray)
            {
                if (b != null) ss = ss + b.ToString() + "\n";
            }

            // Фінальне відображення тексту на формі
            label1.Text = ss;
        }
    }
}