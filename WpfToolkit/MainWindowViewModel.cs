using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using WpfToolkit.Model;
using WpfToolkit.UserControls.DatagridControl;

namespace WpfToolkit
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        public ICommand AddUserCommand { get; private set; }

        private void AddUser(object? parameter)
        {
            // 添加用戶的邏輯
            // 調用 UpdateProperties 方法來更新項目 "Tone 1" : 屬性名稱為 Rz  值為 17
            mDataItemProvider.UpdateProperties("1", new Dictionary<string, object>() { { "Rz", 17 } });
        }

        private UserControl? _userControlContent;
        public UserControl? UserControlContent
        {
            get { return _userControlContent; }
            set
            {
                _userControlContent = value;
                OnPropertyChanged(nameof(UserControlContent));
            }
        }

        private readonly ItemSourceProvider<DataItem> mDataItemProvider;
        
        private readonly TDataGrid<DataItem> mCustomDataGrid;
        
        public MainWindowViewModel()
        {
            // 初始化命令
            AddUserCommand = new RelayCommand(AddUser);

            mDataItemProvider = new ItemSourceProvider<DataItem>(new Dictionary<string, DataItem>
            {
                { "1", new DataItem() { Tone = "1", Rx = 1, Ry = 1, Rz = 1 } },
                { "2", new DataItem() { Tone = "2", Rx = 2, Ry = 2, Rz = 2 } }
            });

            // 創建並設置 TDataGrid 控制項
            mCustomDataGrid = new TDataGrid<DataItem>(mDataItemProvider);
            UserControlContent = mCustomDataGrid;
        }

        // 實現INotifyPropertyChanged接口
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
