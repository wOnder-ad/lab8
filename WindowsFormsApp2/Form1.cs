using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // Головна подія натискання кнопки. Тут відбувається "магія" об'єднання різних об'єктів.
        private void button1_Click(object sender, EventArgs e)
        {
            string output = "=== Система Друківні v2.0 ===\n\n";

            // 1. Створення конкретних об'єктів. Це "виконавці" нашої роботи.
            // Замінили дані на більш серйозні/бізнесові.
            Document contract = new Document("Контракт_на_постачання_Піци.pdf", 42);
            Photo evidence = new Photo("Скріншот_помилки.png", "1920x1080");

            // 2. Створення поліморфного списку.
            // Замість конкретних типів (Document/Photo), ми використовуємо тип Інтерфейсу.
            // Це як коробка з написом "Речі, що друкуються". Ми можемо кинути туди все, що підтримує цей стандарт.
            List<IPrintable> printQueue = new List<IPrintable> { contract, evidence };

            output += "Черга друку:\n";

            // 3. Універсальний цикл.
            // Нам не потрібно писати if (item is Document) ... else if (item is Photo).
            // Ми просто тиснемо кнопку "Print" на кожному об'єкті, і він сам знає, як це зробити.
            foreach (IPrintable item in printQueue)
            {
                // Викликається унікальна реалізація методу для кожного конкретного об'єкта
                output += item.Print() + "\n";
            }

            // Виведення результату на екран (пошук label1 на формі)
            Label label1 = this.Controls.Find("label1", true).FirstOrDefault() as Label;
            if (label1 != null)
            {
                label1.Text = output;
            }
        }
    }

    // =================================================================================
    // Оголошення КОНТРАКТУ (Інтерфейсу)
    // =================================================================================
    // Інтерфейс не містить коду (логіки), він містить лише вимоги.
    // Це як посадова інструкція: "Ти повинен вміти робити Print".
    public interface IPrintable
    {
        string Print(); // Сигнатура методу: назва та тип повернення.
    }

    // =================================================================================
    // Виконавець №1: Текстовий Документ
    // =================================================================================
    public class Document : IPrintable
    {
        public string Title { get; set; }
        public int PageCount { get; set; }

        public Document(string title, int pageCount)
        {
            Title = title;
            PageCount = pageCount;
        }

        // Цей клас "підписав контракт" IPrintable, тому він ЗОБОВ'ЯЗАНИЙ
        // написати код для методу Print. Інакше програма не скомпілюється.
        public string Print()
        {
            // Специфічна логіка для документа: нам важлива назва і сторінки.
            return $"[DOC] Друкуємо стос паперу: '{Title}'. Витрата: {PageCount} арк.";
        }
    }

    // =================================================================================
    // Виконавець №2: Зображення
    // =================================================================================
    public class Photo : IPrintable
    {
        public string FileName { get; set; }
        public string Resolution { get; set; }

        public Photo(string fileName, string resolution)
        {
            FileName = fileName;
            Resolution = resolution;
        }

        // У фотографії процес друку інший, ніж у документа, але назва методу
        // мусить бути така сама, як в інтерфейсі (Print).
        public string Print()
        {
            // Специфічна логіка для фото: нам важлива роздільна здатність.
            return $"[IMG] Друкуємо картинку '{FileName}' на глянці. Розмір: {Resolution}.";
        }
    }
}