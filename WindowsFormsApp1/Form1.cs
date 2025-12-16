using System;
using System.Collections; // Критично важливо для роботи інтерфейсів IEnumerable/IEnumerator
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // Базовий клас-сутності "Книга". Це просто контейнер для даних.
        public class MyBook
        {
            public int bookNomer { get; set; }
            public String Avtor { get; set; }
            public String Nazva { get; set; }
            public String Vydavnyctvo { get; set; }
            public Int16 RikVyhodu { get; set; }

            public MyBook(int bookNomer, String Avtor, String Nazva, String Vydavnyctvo, Int16 RikVyhodu)
            {
                this.bookNomer = bookNomer;
                this.Avtor = Avtor;
                this.Nazva = Nazva;
                this.Vydavnyctvo = Vydavnyctvo;
                this.RikVyhodu = RikVyhodu;
            }

            public override string ToString()
            {
                // Форматування рядка для гарного виводу в Label
                return $"ID: {bookNomer} | {Avtor} — '{Nazva}' ({Vydavnyctvo}, {RikVyhodu})";
            }
        }

        // --- СПОСІБ 1: "Лінивий" (через успадкування) ---
        // Ми просто беремо готовий функціонал ArrayList, який вже вміє працювати з foreach.
        public class MyBooks1 : ArrayList, IEnumerable
        {
            // Масив тут виступає як буфер для зручності, хоча ArrayList зберігає дані всередині себе
            public MyBook[] MyBooksArray { get; set; }

            public MyBooks1(int kilkistKnyh)
            {
                MyBooksArray = new MyBook[kilkistKnyh];
            }
            // Тут немає методу GetEnumerator, бо ми його отримали у спадок від ArrayList.
        }

        // --- СПОСІБ 2: "Делегування" (Обгортка) ---
        // Цей клас сам не вміє перебирати елементи, але він "переводить стрілки" на внутрішній масив.
        public class MyBooks2 : IEnumerable
        {
            public MyBook[] MyBooksArray { get; set; }

            public MyBooks2(int kilkistKnyh)
            {
                MyBooksArray = new MyBook[kilkistKnyh];
            }

            // Коли foreach запитує "як тебе читати?", ми віддаємо ітератор нашого масиву.
            public IEnumerator GetEnumerator()
            {
                return MyBooksArray.GetEnumerator();
            }
        }

        // --- СПОСІБ 3: "Повний контроль" (Ручна реалізація машини станів) ---
        /* * Цей клас є одночасно і КОЛЕКЦІЄЮ (IEnumerable), і КУРСОРОМ (IEnumerator).
         * Ми вручну прописуємо, як рухатися по пам'яті (MoveNext) і що повертати (Current).
         */
        public class MyBooks3 : IEnumerable, IEnumerator
        {
            MyBooks3[] myBooksArray; // Внутрішнє сховище
            int kilkistKnyh = 0;     // Ліміт книг
            int CurrentNomer = 0;    // Лічильник заповненості

            // Властивості самої книги (через специфіку завдання цей клас є і колекцією, і елементом)
            int bookNomer { get; set; }
            String Avtor { get; set; }
            String Nazva { get; set; }
            String Vydavnyctvo { get; set; }
            public Int16 RikVyhodu { get; set; }

            // Позиція "курсора". -1 означає, що ми стоїмо ПЕРЕД першим елементом (стартова позиція).
            int position = -1;

            // Конструктор для ініціалізації конкретної книги
            public MyBooks3(int kilkistKnyh, int bookNomer, String Avtor, String Nazva, String Vydavnyctvo, Int16 RikVyhodu)
            {
                if (kilkistKnyh <= 0) return;
                myBooksArray = new MyBooks3[kilkistKnyh];
                this.kilkistKnyh = kilkistKnyh;
                this.bookNomer = bookNomer;
                this.Avtor = Avtor;
                this.Nazva = Nazva;
                this.Vydavnyctvo = Vydavnyctvo;
                this.RikVyhodu = RikVyhodu;
            }

            // Конструктор для ініціалізації порожньої "полиці" (колекції)
            public MyBooks3(int kilkistKnyh)
            {
                myBooksArray = new MyBooks3[kilkistKnyh];
                this.kilkistKnyh = kilkistKnyh;
                bookNomer = 0;
            }

            // Індексатор: дозволяє звертатися до об'єкта як до масиву: collection[0]
            public MyBooks3 this[int index]
            {
                get
                {
                    if (index <= kilkistKnyh && index >= 0)
                        return myBooksArray[index];
                    else return null;
                }
                set
                {
                    if (index < kilkistKnyh) myBooksArray[index] = value;
                }
            }

            // Реалізація IEnumerable: цей метод викликається на початку foreach
            public IEnumerator GetEnumerator()
            {
                foreach (MyBooks3 b in myBooksArray)
                {
                    // yield return дозволяє повертати елементи по черзі, зберігаючи стан між викликами
                    yield return (IEnumerator)b;
                }
            }

            // --- Блок реалізації IEnumerator (навігація) ---

            // Current: повертає те, на що зараз вказує палець (курсор)
            public object Current
            {
                get
                {
                    if (position >= 0 && position < kilkistKnyh) return myBooksArray[position];
                    else return null;
                }
            }

            // MoveNext: крок вперед. Повертає true, якщо ми стали на елемент, і false, якщо впали в прірву (кінець масиву)
            public bool MoveNext()
            {
                position++;
                return (position < kilkistKnyh);
            }

            // Reset: повернення на стартову лінію
            public void Reset()
            {
                position = -1;
            }

            public override string ToString()
            {
                return $"ID: {bookNomer} | {Avtor} — '{Nazva}' ({Vydavnyctvo}, {RikVyhodu})";
            }

            // Метод для безпечного додавання книг з перевіркою переповнення
            public void Add(int bookNomer, String Avtor, String Nazva, String Vydavnyctvo, Int16 RikVyhodu, ref int KodError)
            {
                if (CurrentNomer < kilkistKnyh)
                {
                    // Створення нового об'єкта MyBooks3 як елемента масиву (трохи дивна архітектура, але згідно завдання)
                    this.myBooksArray[CurrentNomer] = new MyBooks3(1);
                    this.myBooksArray[CurrentNomer].bookNomer = bookNomer;
                    this.myBooksArray[CurrentNomer].Avtor = Avtor;
                    this.myBooksArray[CurrentNomer].Nazva = Nazva;
                    this.myBooksArray[CurrentNomer].Vydavnyctvo = Vydavnyctvo;
                    this.myBooksArray[CurrentNomer].RikVyhodu = RikVyhodu;
                    CurrentNomer++;
                    KodError = 0; // Успіх
                }
                else
                {
                    KodError = 1; // Помилка: масив заповнений
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string outputBuffer = "\n === ТЕСТУВАННЯ SPOSIB 1 (Спадкування) === \n\n ";

            // Заповнення даними: Використовуємо класику фантастики
            MyBooks1 mbs1 = new MyBooks1(10);
            mbs1.MyBooksArray[0] = new MyBook(101, "Френк Герберт", "Дюна", "КСД", 1965);
            mbs1.MyBooksArray[1] = new MyBook(102, "Дж. Орвелл", "1984", "Вид-во Жупанського", 1949);
            mbs1.MyBooksArray[2] = new MyBook(103, "Рей Бредбері", "451 градус за Фаренгейтом", "Навчальна книга", 1953);

            // Foreach працює автоматично завдяки ArrayList
            foreach (MyBook b in mbs1.MyBooksArray)
            {
                if (b != null) outputBuffer += b.ToString() + "\n";
            }

            outputBuffer += "\n === ТЕСТУВАННЯ SPOSIB 2 (Композиція) === \n\n ";

            // Заповнення даними: Використовуємо фентезі та пригоди
            MyBooks2 mbs2 = new MyBooks2(10);
            mbs2.MyBooksArray[0] = new MyBook(201, "Дж. Р. Р. Толкін", "Гобіт", "Астролябія", 1937);
            mbs2.MyBooksArray[1] = new MyBook(202, "Джоан Роулінг", "Гаррі Поттер і філософський камінь", "А-ба-ба-га-ла-ма-га", 1997);
            mbs2.MyBooksArray[2] = new MyBook(203, "Анджей Сапковський", "Відьмак. Останнє бажання", "КСД", 1993);

            // Foreach працює, бо ми делегували його внутрішньому масиву
            foreach (MyBook b in mbs2.MyBooksArray)
            {
                if (b != null) outputBuffer += b.ToString() + "\n";
            }

            outputBuffer += "\n === ТЕСТУВАННЯ SPOSIB 3 (Ручний контроль) === \n\n ";

            // Створення складної колекції
            MyBooks3 mbs3 = new MyBooks3(5);
            int errCode = 0;

            // Додаємо українську класику та історію
            mbs3.Add(301, "Тарас Шевченко", "Кобзар", "В типографії Лисенка", 1840, ref errCode);
            mbs3.Add(302, "Ліна Костенко", "Маруся Чурай", "Дніпро", 1979, ref errCode);
            mbs3.Add(303, "Іван Франко", "Захар Беркут", "Світ", 1883, ref errCode);
            mbs3.Add(304, "Василь Стус", "Палімсести", "Сучасність", 1986, ref errCode);
            mbs3.Add(305, "Всеволод Нестайко", "Тореадори з Васюківки", "А-ба-ба-га-ла-ма-га", 1964, ref errCode);

            // Перевірка на переповнення (спробуємо додати 6-ту книгу в масив розміром 5)
            if (errCode > 0) MessageBox.Show("Увага! Бібліотека переповнена.");

            // Виведення через стандартний foreach
            foreach (MyBooks3 b in mbs3)
            {
                if (b != null) outputBuffer += b.ToString() + " \n";
            }

            // Демонстрація "ручного" керування ітератором (те, що відбувається "під капотом" foreach)
            mbs3.Reset(); // Скидання на початок

            mbs3.MoveNext(); // Крок 1
            outputBuffer += " \n --- Ручний виклик MoveNext() --- \n";
            outputBuffer += $"Поточний елемент: {mbs3.Current.ToString()} \n";

            mbs3.MoveNext(); // Крок 2
            outputBuffer += " \n --- Ще один крок MoveNext() --- \n ";
            outputBuffer += $"Поточний елемент: {mbs3.Current.ToString()}";

            label1.Text = outputBuffer;
        }
    }
}