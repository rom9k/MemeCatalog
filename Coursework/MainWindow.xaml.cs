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
        public Dictionary<string, string> memeImagePaths = new Dictionary<string, string>(); //хранение мемов и путей к ним
        public Dictionary<string, List<string>> categoryMemes = new Dictionary<string, List<string>>(); //хранение категорий и мемов в каждой из них
        public Dictionary<string, List<MemesData>> tagMemes = new Dictionary<string, List<MemesData>>();//хранение тегов

        public MainWindow()
        {
            InitializeComponent();
            category.SelectionChanged += category_SelectionChanged;
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {

            AddMeme addMemeWindow = new AddMeme();
            addMemeWindow.Owner = this; //MainWindow как владелец окна AddMeme
            addMemeWindow.ShowDialog();
        }

        public void UpdateListBoxAndComboBox(string memeName, string categoryName)
        {
            //обновление ListBox и ComboBox
            spisok.Items.Add(memeName);

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
                category.Items.Add(newCategoryItem);
            }
        }

        private void spisok_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (spisok.SelectedItem != null)
            {
                string selectedMemeName = spisok.SelectedItem.ToString();
                //изображение по имени мема
                DisplayMemeImage(selectedMemeName);
            }
        }

        private void DisplayMemeImage(string memeName)
        {
            //путь к изображению для выбранного мема
            string imagePath = GetImagePathForMeme(memeName);

            if (!string.IsNullOrEmpty(imagePath))
            {
                    //изображение в элемент Image
                    Uri uri = new Uri(imagePath);
                    img.Source = new System.Windows.Media.Imaging.BitmapImage(uri);
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
                return string.Empty; //путь к изображению не найден
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
                //очистка списка мемов
                spisok.Items.Clear();

                //проверка наличия выбранной категории в словаре категорий
                if (categoryMemes.ContainsKey(selectedCategory))
                {
                    //добавление мема выбранной категории в ListBox
                    foreach (var meme in categoryMemes[selectedCategory])
                    {
                        spisok.Items.Add(meme);
                    }
                }
            }
            else
            {
                //категория не выбрана
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
                //удаление из словаря
                if (memeImagePaths.ContainsKey(selectedMeme))
                {
                    memeImagePaths.Remove(selectedMeme);
                }
                //удаление из категорий
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
            string searchText = poisk.Text.ToLower(); //приведение к нижнему регистру
            spisok.Items.Clear(); //очистка ListBox

            //перебор всех категории
            foreach (var categoryMemeList in categoryMemes.Values)
            {
                //перебор мемов
                foreach (var meme in categoryMemeList)
                {
                    if (meme.ToLower().Contains(searchText)) //проверка на название мема
                    {
                        spisok.Items.Add(meme);
                    }
                }
            }
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            List<MemesData> memesToSave = new List<MemesData>();

            // заполнение списка memesToSave данными о мемах
            foreach (var category in categoryMemes)
            {
                foreach (var meme in category.Value)
                {
                    MemesData memeData = new MemesData
                    {
                        Category = category.Key,
                        Name = meme,
                        ImagePath = memeImagePaths[meme],
                        Tags = GetTagsForMeme(meme)
                    };
                    memesToSave.Add(memeData);
                }
            }

            // выбор места сохранения файла
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JSON Files (*.json)|*.json";
            if (saveFileDialog.ShowDialog() == true)
            {
                // перевод в формат JSON и сохранение в выбранный файл
                string jsonData = JsonConvert.SerializeObject(memesToSave);
                File.WriteAllText(saveFileDialog.FileName, jsonData);
            }
        }

        private List<string> GetTagsForMeme(string memeName)
        {
            List<string> tags = new List<string>();

            foreach (var tagMemeList in tagMemes)
            {
                foreach (var memeData in tagMemeList.Value)
                {
                    if (memeData.Name == memeName)
                    {
                        tags.AddRange(memeData.Tags);
                    }
                }
            }

            return tags.Distinct().ToList();
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

                // отображение названий загруженных мемов при загрузке
                LoadMeme loadMemeWindow = new LoadMeme();
                loadMemeWindow.Owner = this;
                loadMemeWindow.loadspisok.Content = string.Join("\n", loadedMemes.Select(meme => meme.Name));
                loadMemeWindow.ShowDialog();
            }
        }

        private void UpdateMemesAfterLoad(List<MemesData> loadedMemes)
        {
            // очистка данных перед загрузкой новых мемов
            memeImagePaths.Clear();
            categoryMemes.Clear();
            category.Items.Clear();
            spisok.Items.Clear();
            tagMemes.Clear();

            // загрузка мемов
            foreach (var memeData in loadedMemes)
            {
                // добавление мема в словарь memeImagePaths
                memeImagePaths.Add(memeData.Name, memeData.ImagePath);

                // добавление категории
                if (!categoryMemes.ContainsKey(memeData.Category))
                {
                    categoryMemes.Add(memeData.Category, new List<string>());
                    ComboBoxItem newItem = new ComboBoxItem();
                    newItem.Content = memeData.Category;
                    category.Items.Add(newItem);
                }

                // добавление мема в соответствующую категорию
                categoryMemes[memeData.Category].Add(memeData.Name);

                // добавление тегов
                foreach (var tag in memeData.Tags)
                {
                    if (tagMemes.ContainsKey(tag))
                    {
                        tagMemes[tag].Add(memeData);
                    }
                    else
                    {
                        tagMemes[tag] = new List<MemesData> { memeData };
                    }
                }
            }

            // обновление интерфейса
            foreach (var memeCategory in categoryMemes)
            {
                foreach (var meme in memeCategory.Value)
                {
                    spisok.Items.Add(meme);
                }
            }
        }

        private void UpdateSpisokWithSelectedCategory()
        { 
            string selectedCategory = category.SelectedItem?.ToString();

            if (!string.IsNullOrEmpty(selectedCategory) && categoryMemes.ContainsKey(selectedCategory))
            {
                spisok.Items.Clear();
                //добавление мемов выбранной категории в ListBox
                foreach (var meme in categoryMemes[selectedCategory])
                {
                    spisok.Items.Add(meme);
                }
            }
        }

        private void poisktag_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = poisktag.Text.ToLower(); // приведение к нижнему регистру
            spisok.Items.Clear(); // очистка ListBox

            var searchTags = searchText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                        .Select(tag => tag.Trim().ToLower())
                                        .ToList();

            foreach (var tagMemeList in tagMemes)
            {
                foreach (var memeData in tagMemeList.Value)
                {
                    if (searchTags.All(searchTag => memeData.Tags.Any(memeTag => memeTag.ToLower().Contains(searchTag))))
                    {
                        if (!spisok.Items.Contains(memeData.Name))
                        {
                            spisok.Items.Add(memeData.Name);
                        }
                    }
                }
            }
        }
    }
}
