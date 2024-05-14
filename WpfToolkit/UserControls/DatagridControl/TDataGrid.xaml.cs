using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfToolkit.Attribute;

namespace WpfToolkit.UserControls.DatagridControl
{
    /// <summary>
    /// TDataGrid.xaml 的互動邏輯
    /// </summary>
    public partial class TDataGrid<T> : UserControl where T : class
    {
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable<T>), typeof(TDataGrid<T>), new PropertyMetadata(null, OnItemsSourceChanged));

        public IEnumerable<T> ItemsSource
        {
            get { return (IEnumerable<T>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        private ItemSourceProvider<T> _itemSourceProvider;

        public TDataGrid(ItemSourceProvider<T> itemSourceProvider)
        {
            _itemSourceProvider = itemSourceProvider;

            _itemSourceProvider.PropertyChanged += ItemSourceProvider_PropertyChanged;

            Loaded += TDataGrid_Loaded;
        }

        private void TDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeDataGrid();
        }

        private void ItemSourceProvider_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ItemSourceProvider<T>.ObservableCollection))
            {
                ItemsSource = _itemSourceProvider.ObservableCollection;
            }
        }

        private void InitializeDataGrid()
        {
            if (Content == null)
                Content = new Grid();

            if (((Grid)Content).Children.Count == 0)
            {
                DataGrid dataGrid = new DataGrid
                {
                    AutoGenerateColumns = false,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                };
                
                // 在這裡加入設置行標識列寬度的代碼 For .NET(8.0)
                dataGrid.RowHeaderWidth = 0;

                ((Grid)Content).Children.Add(dataGrid);
            }

            ItemsSource = _itemSourceProvider.ObservableCollection;

        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TDataGrid<T> customDataGrid)
            {
                if (e.NewValue != null)
                {
                    // 更新 DataGrid 的 ItemsSource
                    ((DataGrid)((Grid)customDataGrid.Content).Children[0]).ItemsSource = (IEnumerable<T>)e.NewValue;
                    customDataGrid.GenerateColumns();
                }
                else
                {
                    // 如果新值為空，清除 DataGrid 的 ItemsSource
                    ((DataGrid)((Grid)customDataGrid.Content).Children[0]).ItemsSource = null;
                }
            }
        }

        private void GenerateColumns()
        {
            if (ItemsSource == null)
                return;

            DataGrid? dataGrid = ((Grid)Content).Children[0] as DataGrid;
            dataGrid?.Columns.Clear();

            PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (property.GetCustomAttribute<IdColumnAttribute>() != null)
                {
                    // 如果屬性標記了 IdColumnAttribute，則不生成列
                    continue;
                }

                if (property.GetCustomAttribute<NumericColumnAttribute>() != null)
                {
                    // 如果屬性標記了 NumericColumnAttribute，則創建具有增加和減少按鈕的數字列
                    DataGridTemplateColumn column = new DataGridTemplateColumn
                    {
                        Header = property.Name
                    };

                    FrameworkElementFactory stackPanelFactory = new FrameworkElementFactory(typeof(StackPanel));
                    stackPanelFactory.SetValue(StackPanel.OrientationProperty, Orientation.Vertical); // 水平排列

                    FrameworkElementFactory contentPresenterFactory = new FrameworkElementFactory(typeof(ContentPresenter));
                    contentPresenterFactory.SetBinding(ContentPresenter.ContentProperty, new Binding(property.Name));
                    stackPanelFactory.AppendChild(contentPresenterFactory);

                    FrameworkElementFactory increaseButton = new FrameworkElementFactory(typeof(Button));
                    increaseButton.SetValue(Button.ContentProperty, "+");
                    increaseButton.SetValue(Button.HeightProperty, 10.0);
                    increaseButton.SetValue(Button.WidthProperty, 5.0);
                    increaseButton.AddHandler(Button.ClickEvent, new RoutedEventHandler((sender, e) =>
                    {
                        if (dataGrid?.SelectedItem != null)
                        {
                            PropertyInfo? selectedProperty = typeof(T).GetProperty(property.Name);
                            int value = selectedProperty?.GetValue(dataGrid.SelectedItem) as int? ?? 0;
                            selectedProperty?.SetValue(dataGrid.SelectedItem, value + 1);
                        }
                    }));
                    stackPanelFactory.AppendChild(increaseButton);

                    FrameworkElementFactory decreaseButton = new FrameworkElementFactory(typeof(Button));
                    decreaseButton.SetValue(Button.ContentProperty, "-");
                    decreaseButton.SetValue(Button.HeightProperty, 10.0);
                    decreaseButton.SetValue(Button.WidthProperty, 5.0);
                    decreaseButton.AddHandler(Button.ClickEvent, new RoutedEventHandler((sender, e) =>
                    {
                        if (dataGrid?.SelectedItem != null)
                        {
                            PropertyInfo? selectedProperty = typeof(T).GetProperty(property.Name);
                            int value = selectedProperty?.GetValue(dataGrid.SelectedItem) as int? ?? 0;
                            selectedProperty?.SetValue(dataGrid.SelectedItem, value - 1);
                        }
                    }));
                    stackPanelFactory.AppendChild(decreaseButton);

                    DataTemplate cellTemplate = new DataTemplate();
                    cellTemplate.VisualTree = stackPanelFactory;

                    column.CellTemplate = cellTemplate;

                    dataGrid?.Columns.Add(column);
                }
                else if (property.GetCustomAttribute<ReadOnlyColumnAttribute>() != null)
                {
                    // 如果屬性標記了 ReadOnlyColumnAttribute，則創建唯讀的文本列
                    DataGridTextColumn column = new DataGridTextColumn
                    {
                        Header = property.Name,
                        Binding = new Binding(property.Name),
                        IsReadOnly = true // 設置為唯讀
                    };
                    dataGrid?.Columns.Add(column);
                }
                else if (property.PropertyType == typeof(string) || property.PropertyType == typeof(int))
                {
                    // 否則，創建普通的文本列
                    DataGridTextColumn column = new DataGridTextColumn
                    {
                        Header = property.Name,
                        Binding = new Binding(property.Name),
                        IsReadOnly = false
                    };
                    dataGrid?.Columns.Add(column);
                }
            }
        }
    }
}
