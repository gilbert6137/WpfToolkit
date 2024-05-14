using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfToolkit.Attribute;

namespace WpfToolkit.Model
{
    public class DataItem
    {
        private string? _tone;

        [IdColumn]
        public string? Id { get; set; } // 主鍵

        [ReadOnlyColumn]
        public string? Tone
        {
            get => _tone;
            set
            {
                _tone = value;
                Id = value; // 在設置 Tone 時同時設置 Id
            }
        }
        public int Rx { get; set; }
        public int Ry { get; set; }
        public int Rz { get; set; }
    }
}
