using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using WpfToolkit.Model;
using WpfToolkit.UserControls.DatagridControl;

namespace TestProject1
{
    [TestFixture]
    public class ItemSourceProviderTests
    {
        [Test]
        public void Test_UpdateDictionary()
        {
            // Arrange
            var itemSourceProvider = new ItemSourceProvider<string>();
            var dataDictionary = new Dictionary<string, string>
            {
                { "1", "One" },
                { "2", "Two" }
            };

            // Act
            itemSourceProvider.UpdateDictionary(dataDictionary);

            // Assert
            Assert.That(itemSourceProvider.ObservableCollection, Has.Count.EqualTo(dataDictionary.Count));
            foreach (var key in dataDictionary.Keys)
            {
                Assert.That(itemSourceProvider.ObservableCollection.Any(item => item == dataDictionary[key]), Is.True);
            }
        }

        [Test]
        public void Test_AddOrUpdate()
        {
            // Arrange
            var itemSourceProvider = new ItemSourceProvider<string>();
            var dataItem = "NewDataItem";

            // Act
            itemSourceProvider.AddOrUpdate("1", dataItem);

            // Assert
            Assert.That(itemSourceProvider.ObservableCollection, Has.Count.EqualTo(1));
            Assert.That(itemSourceProvider.ObservableCollection, Does.Contain(dataItem));
        }

        [Test]
        public void Test_Remove()
        {
            // Arrange
            var itemSourceProvider = new ItemSourceProvider<string>();
            itemSourceProvider.AddOrUpdate("1", "DataItemToRemove");

            // Act
            itemSourceProvider.Remove("1");

            // Assert
            Assert.That(itemSourceProvider.ObservableCollection, Is.Empty);
        }

        [Test]
        public void Test_UpdateProperties()
        {
            // Arrange
            var mDataItemProvider = new ItemSourceProvider<DataItem>(new Dictionary<string, DataItem>
            {
                { "1", new DataItem() { Tone = "1", Rx = 1, Ry = 1, Rz = 1 } },
                { "2", new DataItem() { Tone = "2", Rx = 2, Ry = 2, Rz = 2 } }
            });

            // Act
            // �ե� UpdateProperties ��k�ӧ�s���� "Tone 1" : �ݩʦW�٬� Rz  �Ȭ� 17
            mDataItemProvider.UpdateProperties("1", new Dictionary<string, object>() { { "Rz", 17 } });

            // Assert �ݭn��g�P�_ DataItem.Rz �O�_ = 17
            Assert.That(mDataItemProvider.ObservableCollection.First(item => item.Id == "1").Rz, Is.EqualTo(17));
        }

        [Test]
        public void Test_UpdateDictionaryFromObservableCollection()
        {
            // Arrange
            var mDataItemProvider = new ItemSourceProvider<DataItem>();
            var dataItems = new List<DataItem>
            {
                new() { Id = "1", Tone = "Tone1", Rx = 1, Ry = 1, Rz = 1 },
                new() { Id = "2", Tone = "Tone2", Rx = 2, Ry = 2, Rz = 2 }
            };
            foreach (var item in dataItems)
            {
                mDataItemProvider.ObservableCollection.Add(item);
            }

            // Act
            mDataItemProvider.UpdateDictionaryFromObservableCollection();

            // Assert
            Assert.That(mDataItemProvider.GetDictionary(), Has.Count.EqualTo(2)); // �ˬd�r�夤�O�_����Ӷ���
            foreach (var item in dataItems)
            {
                var id = item.Id;
                Assert.Multiple(() =>
                {
                    Assert.That(mDataItemProvider.GetDictionary().ContainsKey(id!), Is.True); // �ˬd�C�Ӷ��ت� Id �O�_�s�b��r�夤,�B�N id �j���ഫ���D�������H����ĵ�i
                    Assert.That(mDataItemProvider.GetDictionary()[id!], Is.EqualTo(item));    // �ˬd�r�夤�C�Ӷ��جO�_�P��l���جۦP,�B�N id �j���ഫ���D�������H����ĵ�i
                });
            }
        }
    }
}