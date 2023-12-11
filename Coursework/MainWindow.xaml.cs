using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace Coursework
{
    public partial class MainWindow : Window
    {
        public Dictionary<string, string> memeImagePaths = new Dictionary<string, string>(); // Словарь для хранения мемов и путей к изображениям
        public Dictionary<string, List<string>> categoryMemes = new Dictionary<string, List<string>>(); // Словарь для хранения категорий и мемов в каждой категории

        public MainWindow()
        {
            InitializeComponent();
            category.SelectionChanged += category_SelectionChanged;
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            AddMeme addMemeWindow = new AddMeme();
            addMemeWindow.Owner = this; // Установите MainWindow как владельца окна AddMeme
            addMemeWindow.ShowDialog();
        }

        public void UpdateListBoxAndComboBox(string memeName, string categoryName)
        {
            // Обновление ListBox и ComboBox здесь
            spisok.Items.Add(memeName); // Добавление имени мема в ListBox

            bool categoryExists = false;
            foreach (ComboBoxItem item in category.Items)
            {
                if (item.Content.ToString() == categoryName)
                {
                    categoryExists = true;
                    break;
                }
            }

            if (!categoryExists)
            {
                ComboBoxItem newCategoryItem = new ComboBoxItem();
                newCategoryItem.Content = categoryName;
                category.Items.Add(newCategoryItem); // Добавление новой категории в ComboBox
            }
        }

        private void spisok_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (spisok.SelectedItem != null)
            {
                string selectedMemeName = spisok.SelectedItem.ToString();
                // Получите изображение по имени мема и отобразите его в Image (img)
                DisplayMemeImage(selectedMemeName);
            }
        }

        private void DisplayMemeImage(string memeName)
        {
            // Получите путь к изображению для выбранного мема (memeName)
            // Например, если у вас есть словарь с путями к изображениям, используйте его
            string imagePath = GetImagePathForMeme(memeName);

            if (!string.IsNullOrEmpty(imagePath))
            {
                try
                {
                    // Установите изображение в элемент Image (img)
                    Uri uri = new Uri(imagePath);
                    img.Source = new System.Windows.Media.Imaging.BitmapImage(uri);
                }
                catch (Exception ex)
                {
                    // Обработайте ошибки загрузки изображения
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }

        private string GetImagePathForMeme(string memeName)
        {
            if (memeImagePaths.ContainsKey(memeName))
            {
                return memeImagePaths[memeName];
            }
            else
            {
                return string.Empty; // Если путь к изображению не найден
            }
        }

        private void category_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterMemesByCategory();
            UpdateSpisokWithSelectedCategory();
        }

        public void FilterMemesByCategory()
        {
            string selectedCategory = (category.SelectedItem as ComboBoxItem)?.Content?.ToString();

            if (!string.IsNullOrEmpty(selectedCategory))
            {
                // Очищаем список мемов
                spisok.Items.Clear();

                // Проверяем наличие выбранной категории в словаре категорий
                if (categoryMemes.ContainsKey(selectedCategory))
                {
                    // Добавляем мемы выбранной категории в ListBox (spisok)
                    foreach (var meme in categoryMemes[selectedCategory])
                    {
                        spisok.Items.Add(meme);
                    }
                }
            }
            else
            {
                // Если категория не выбрана, отображаем все мемы
                foreach (var meme in memeImagePaths.Keys)
                {
                    spisok.Items.Add(meme);
                }
            }
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            if (spisok.SelectedItem != null)
            {
                string selectedMeme = spisok.SelectedItem.ToString();
                spisok.Items.Remove(selectedMeme);

                // Удаление также из словаря или других структур данных, хранящих мемы
                // Например, из словаря memeImagePaths
                if (memeImagePaths.ContainsKey(selectedMeme))
                {
                    memeImagePaths.Remove(selectedMeme);
                }

                // Также удалите из категорий, если они использовались для хранения мемов
                foreach (var categoryList in categoryMemes.Values)
                {
                    if (categoryList.Contains(selectedMeme))
                    {
                        categoryList.Remove(selectedMeme);
                    }
                }
            }
        }

        private void poisk_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = poisk.Text.ToLower(); // Получаем текст из TextBox и приводим к нижнему регистру для поиска без учета регистра
            spisok.Items.Clear(); // Очищаем ListBox для вывода результата поиска

            // Перебираем все категории
            foreach (var categoryMemeList in categoryMemes.Values)
            {
                // Перебираем мемы в текущей категории
                foreach (var meme in categoryMemeList)
                {
                    if (meme.ToLower().Contains(searchText)) // Проверяем, содержит ли название мема текст из TextBox
                    {
                        spisok.Items.Add(meme); // Если да, добавляем мем в ListBox
                    }
                }
            }
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            List<MemesData> memesToSave = new List<MemesData>();

            // Заполнение списка memesToSave данными о мемах
            foreach (var category in categoryMemes)
            {
                foreach (var meme in category.Value)
                {
                    MemesData memeData = new MemesData
                    {
                        Category = category.Key,
                        Name = meme,
                        ImagePath = memeImagePaths[meme]
                    };
                    memesToSave.Add(memeData);
                }
            }

            // Выбор места сохранения файла
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JSON Files (*.json)|*.json";
            if (saveFileDialog.ShowDialog() == true)
            {
                // Сериализация списка в JSON и сохранение в выбранный файл
                string jsonData = JsonConvert.SerializeObject(memesToSave);
                File.WriteAllText(saveFileDialog.FileName, jsonData);
            }
        }

        private void load_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JSON Files (*.json)|*.json";

            if (openFileDialog.ShowDialog() == true)
            {
                string json = File.ReadAllText(openFileDialog.FileName);

                List<MemesData> loadedMemes = JsonConvert.DeserializeObject<List<MemesData>>(json);

                UpdateMemesAfterLoad(loadedMemes);

                // Отображение названий загруженных мемов в label "loadspisok" в окне LoadMeme
                LoadMeme loadMemeWindow = new LoadMeme();
                loadMemeWindow.Owner = this;
                loadMemeWindow.loadspisok.Content = string.Join("\n", loadedMemes.Select(meme => meme.Name));
                loadMemeWindow.ShowDialog();
            }
        }

        private void UpdateMemesAfterLoad(List<MemesData> loadedMemes)
        {
            // Очищаем данные перед загрузкой новых мемов
            memeImagePaths.Clear();
            categoryMemes.Clear();
            category.Items.Clear();
            spisok.Items.Clear();

            // Загрузка мемов из списка loadedMemes
            foreach (var memeData in loadedMemes)
            {
                // Добавление мема в словарь memeImagePaths
                memeImagePaths.Add(memeData.Name, memeData.ImagePath);

                // Добавление категории, если ее еще нет
                if (!categoryMemes.ContainsKey(memeData.Category))
                {
                    categoryMemes.Add(memeData.Category, new List<string>());

                    // Создание объекта ComboBoxItem для новой категории
                    ComboBoxItem newItem = new ComboBoxItem();
                    newItem.Content = memeData.Category;

                    // Добавление объекта ComboBoxItem в ComboBox "category"
                    category.Items.Add(newItem);
                }

                // Добавление мема в соответствующую категорию
                categoryMemes[memeData.Category].Add(memeData.Name);
            }

            // Обновление интерфейса
            foreach (var memeCategory in categoryMemes)
            {
                foreach (var meme in memeCategory.Value)
                {
                    spisok.Items.Add(meme); // Добавляем мемы в ListBox "spisok"
                }
            }
        }

            private void UpdateSpisokWithSelectedCategory()
        {
            string selectedCategory = category.SelectedItem?.ToString();

            if (!string.IsNullOrEmpty(selectedCategory) && categoryMemes.ContainsKey(selectedCategory))
            {
                spisok.Items.Clear();

                // Добавляем мемы выбранной категории в ListBox (spisok)
                foreach (var meme in categoryMemes[selectedCategory])
                {
                    spisok.Items.Add(meme);
                }
            }
        }
    }
}
