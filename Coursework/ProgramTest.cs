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

        public void SearchMeme()
        {
            var mainWindow = new MainWindow();
            mainWindow.poisk.Text = ""; //текст для поиска
            //проверка наличия найденных мемов
            CollectionAssert.Contains(mainWindow.spisok.Items, "");
        }

        [Test]

        public void LoadMemes()
        {
            var mainWindow = new MainWindow();
            List<MemesData> testMemes = new List<MemesData>
            {
                new MemesData { Category = "", Name = ""},
                new MemesData { Category = "", Name = ""}
            };
            //проверка добавления мемов в соответствующие списки и элементы интерфейса
            CollectionAssert.IsNotEmpty(mainWindow.category.Items);
            CollectionAssert.IsNotEmpty(mainWindow.spisok.Items);
        }

        [Test]

        public void FilterMemesByCategory()
        {
            var mainWindow = new MainWindow();
            mainWindow.category.SelectedItem = ""; //выбор категории
            //проверка обновления списка мемов по выбранной категории
            CollectionAssert.DoesNotContain(mainWindow.spisok.Items, "");
        }
    }
}
