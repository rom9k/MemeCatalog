using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Coursework
{
    /// <summary>
    /// Логика взаимодействия для AddMeme.xaml
    /// </summary>
    public partial class AddMeme : Window
    {
        public AddMeme()
        {
            InitializeComponent();
        }

        private void search_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.jpg; *.jpeg; *.png; *.gif)|*.jpg; *.jpeg; *.png; *.gif|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedImagePath = openFileDialog.FileName;
                // Действия с выбранным изображением здесь

                // Получите имя и категорию из текстовых полей
                string memeName = name.Text;
                string categoryName = namecategory.Text;
                UpdateMemeImagePaths(memeName, selectedImagePath, categoryName);

                // Обновление данных в MainWindow
                ((MainWindow)this.Owner).UpdateListBoxAndComboBox(memeName, categoryName);
            }
        }

        private void UpdateMemeImagePaths(string memeName, string imagePath, string categoryName)
        {
            // Обновляем словарь memeImagePaths
            ((MainWindow)this.Owner).memeImagePaths.Add(memeName, imagePath);

            // Проверяем наличие категории в словаре категорий
            if (!((MainWindow)this.Owner).categoryMemes.ContainsKey(categoryName))
            {
                // Если категории нет, добавляем новую категорию и мем в эту категорию
                ((MainWindow)this.Owner).categoryMemes.Add(categoryName, new List<string> { memeName });
            }
            else
            {
                // Если категория уже существует, добавляем мем в эту категорию
                ((MainWindow)this.Owner).categoryMemes[categoryName].Add(memeName);
            }
        }
    }
}
