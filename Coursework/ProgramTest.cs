using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Coursework
{
    [TestFixture]

    public class MemesTests
    {
        [Test]

        public void AddMeme_WhenNewMemeAdded_CountIncreased()
        {
            // Arrange
            // Подготовка данных и сценария
            var mainWindow = new MainWindow();
            int initialCount = mainWindow.spisok.Items.Count;

            // Act
            // Добавление нового мема
            // Предположим, что добавление мема происходит при нажатии на кнопку или другое событие
            //mainWindow.add.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

            // Assert
            // Проверка увеличения количества мемов в списке
            Assert.AreEqual(initialCount + 1, mainWindow.spisok.Items.Count);
        }

        [Test]

        public void DeleteMeme_WhenMemeDeleted_CountDecreased()
        {
            // Arrange
            // Подготовка данных и сценария
            var mainWindow = new MainWindow();
            mainWindow.spisok.Items.Add("Test Meme"); // Добавляем тестовый мем

            int initialCount = mainWindow.spisok.Items.Count;

            // Act
            // Удаление мема
            // Предположим, что удаление мема происходит при нажатии на кнопку или другое событие
            // mainWindow.delete.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

            // Assert
            // Проверка уменьшения количества мемов в списке
            Assert.AreEqual(initialCount - 1, mainWindow.spisok.Items.Count);
        }

        [Test]

        public void SearchMeme_WhenMatchingMemeFound_ReturnsExpectedResult()
        {
            // Arrange
            // Подготовка данных и сценария
            var mainWindow = new MainWindow();
            mainWindow.poisk.Text = "Test"; // Устанавливаем текст для поиска

            // Act
            // Выполнение поиска мема
            // Предположим, что поиск мема происходит при вводе текста в TextBox или при нажатии на кнопку поиска
            // mainWindow.Poisk_TextChanged(this, null);

            // Assert
            // Проверка наличия найденных мемов
            CollectionAssert.Contains(mainWindow.spisok.Items, "Test Meme");
        }

        [Test]

        public void LoadMemes_WhenFileLoaded_PopulatesMemeLists()
        {
            // Arrange
            // Подготовка данных и сценария
            var mainWindow = new MainWindow();
            List<MemesData> testMemes = new List<MemesData>
            {
                new MemesData { Category = "Category 1", Name = "Meme 1"},
                new MemesData { Category = "Category 2", Name = "Meme 2"}
                // Добавьте другие тестовые мемы по необходимости
            };
            // Simulate loading testMemes from a JSON file
            // mainWindow.UpdateMemesAfterLoad(testMemes);

            // Act
            // Вызов метода обновления после загрузки

            // Assert
            // Проверка добавления мемов в соответствующие списки и элементы интерфейса
            CollectionAssert.IsNotEmpty(mainWindow.category.Items);
            CollectionAssert.IsNotEmpty(mainWindow.spisok.Items);
        }

        [Test]

        public void FilterMemesByCategory_WhenCategorySelected_UpdatesMemeList()
        {
            // Arrange
            // Подготовка данных и сценария
            var mainWindow = new MainWindow();
            mainWindow.category.SelectedItem = "Category 1"; // Выбор категории

            // Act
            // Вызов метода фильтрации по категории
            // mainWindow.FilterMemesByCategory();

            // Assert
            // Проверка обновления списка мемов по выбранной категории
            CollectionAssert.DoesNotContain(mainWindow.spisok.Items, "Meme from Category 2");
        }
    }
}
