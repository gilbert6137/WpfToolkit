using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WpfToolkit.UserControls.DatagridControl
{
    /// <summary>
    /// 提供對數據源的管理和通知功能的類別。
    /// </summary>
    /// <typeparam name="T">數據項的類型。</typeparam>
    public class ItemSourceProvider<T> : INotifyPropertyChanged where T : class
    {
        private Dictionary<string, T> _dataDictionary = new Dictionary<string, T>();
        private ObservableCollection<T> _observableCollection = new ObservableCollection<T>();

        public Dictionary<string, T>  GetDictionary()
        {
            return _dataDictionary;
        }

        /// <summary>
        /// 當屬性更改時觸發的事件。
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// 可觀察的集合，用於在 UI 中綁定數據。
        /// </summary>
        public ObservableCollection<T> ObservableCollection
        {
            get { return _observableCollection; }
            private set
            {
                _observableCollection = value;
                OnPropertyChanged(nameof(ObservableCollection));
            }
        }

        /// <summary>
        /// 初始化 ItemSourceProvider 類別的新實例。
        /// </summary>
        /// <param name="initialData">初始化時使用的數據字典。</param>
        public ItemSourceProvider(Dictionary<string, T>? initialData = null)
        {
            if (initialData != null)
            {
                _dataDictionary = initialData;
                RefreshObservableCollection();
            }
        }

        /// <summary>
        /// 更新數據字典。
        /// </summary>
        /// <param name="dataDictionary">要更新的新數據字典。</param>
        public void UpdateDictionary(Dictionary<string, T> dataDictionary)
        {
            if (dataDictionary == null)
                throw new ArgumentNullException(nameof(dataDictionary));

            _dataDictionary = dataDictionary;
            RefreshObservableCollection();
        }

        /// <summary>
        /// 添加或更新數據項。
        /// </summary>
        /// <param name="id">數據項的唯一標識符。</param>
        /// <param name="dataItem">要添加或更新的數據項。</param>
        public void AddOrUpdate(string id, T dataItem)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("ID 不能為空", nameof(id));

            // 如果 dataItem 為空，則將其設置為預設值或者空實例，具體取決於類型 T 是否是引用類型
            if (dataItem == null)
            {
                if (typeof(T).IsClass)
                {
                    dataItem = Activator.CreateInstance<T>();
                }
                else
                {
                    // 如果 T 是值類型，則無法將其設置為空，這裡可以根據實際情況採取其他措施，例如 throw 異常
                    // 或者設置為 T 的默認值，這裡為了簡單起見，直接返回
                    return;
                }
            }

            _dataDictionary[id] = dataItem;
            RefreshObservableCollection();
        }

        /// <summary>
        /// 移除指定 ID 的數據項。
        /// </summary>
        /// <param name="id">要移除的數據項的 ID。</param>
        public void Remove(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("ID 不能為空", nameof(id));

            _dataDictionary.Remove(id);
            RefreshObservableCollection();
        }

        /// <summary>
        /// 更新指定數據項的屬性值。
        /// </summary>
        /// <param name="id">要更新的數據項的 ID。</param>
        /// <param name="propertiesToUpdate">要更新的屬性字典。</param>
        public void UpdateProperties(string id, Dictionary<string, object> propertiesToUpdate)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("ID 不能為空", nameof(id));

            if (propertiesToUpdate == null)
                throw new ArgumentNullException(nameof(propertiesToUpdate));

            if (_dataDictionary.ContainsKey(id))
            {
                T item = _dataDictionary[id];
                Type itemType = typeof(T);

                foreach (var property in propertiesToUpdate)
                {
                    PropertyInfo? propInfo = itemType.GetProperty(property.Key);
                    if (propInfo != null)
                    {
                        propInfo.SetValue(item, Convert.ChangeType(property.Value, propInfo.PropertyType));
                    }
                }

                RefreshObservableCollection();
            }
        }

        /// <summary>
        /// 屬性更改時調用的方法，觸發 PropertyChanged 事件。
        /// </summary>
        /// <param name="propertyName">已更改的屬性名稱。</param>
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 刷新可觀察集合，以反映最新的數據字典內容。
        /// </summary>
        private void RefreshObservableCollection()
        {
            ObservableCollection = new ObservableCollection<T>(_dataDictionary.Values);
        }

        public void UpdateDictionaryFromObservableCollection()
        {
            _dataDictionary.Clear();
            foreach (T item in _observableCollection)
            {
                // 假設每個數據項都有一個唯一的 Id 用於字典的索引
                PropertyInfo? idProperty = typeof(T).GetProperty("Id"); // 假設 Id 是唯一識別符的屬性名稱
                string? id = idProperty?.GetValue(item) as string;

                // 如果 column value 被刪除成空，則對應的值類型屬性會被設置為默認值
                // 例如，對於 int 類型，默認值為 0
                _dataDictionary[id ?? ""] = item;
            }
        }
    }
}
