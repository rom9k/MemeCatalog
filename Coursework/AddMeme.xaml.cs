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
                //путь к изображению
                string selectedImagePath = openFileDialog.FileName;
                //имя категория теги
                string memeName = name.Text;
                string categoryName = namecategory.Text;
                string tags = nametag.Text;

                UpdateMemeImagePaths(memeName, selectedImagePath, categoryName, tags);

                //обновление данных в MainWindow
                ((MainWindow)this.Owner).UpdateListBoxAndComboBox(memeName, categoryName);
            }

        }

        private void UpdateMemeImagePaths(string memeName, string imagePath, string categoryName, string tags)
        {
            //обновляем словарь memeImagePaths
            ((MainWindow)this.Owner).memeImagePaths.Add(memeName, imagePath);

            //проверка наличие категории в словаре категорий
            if (!((MainWindow)this.Owner).categoryMemes.ContainsKey(categoryName))
            {
                //добавление новой категории и мема в эту категорию
                ((MainWindow)this.Owner).categoryMemes.Add(categoryName, new List<string> { memeName });
            }
            else
            {
                //добавление мема в категорию
                ((MainWindow)this.Owner).categoryMemes[categoryName].Add(memeName);
            }

            //сохранение тегов для данного мема
            List<string> tagList = tags.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            MemesData memeData = new MemesData
            {
                Category = categoryName,
                Name = memeName,
                ImagePath = imagePath,
                Tags = tagList
            };

            //добавление тегов мема в коллекцию
            foreach (string tag in tagList)
            {
                if (((MainWindow)this.Owner).tagMemes.ContainsKey(tag))
                {
                    ((MainWindow)this.Owner).tagMemes[tag].Add(memeData);
                }
                else
                {
                    ((MainWindow)this.Owner).tagMemes[tag] = new List<MemesData> { memeData };
                }
            }
        }
    }
}
