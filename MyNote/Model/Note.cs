using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Data.Xml.Dom;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyNote
{
    /// <summary>
    /// 记事项
    /// </summary>
    class Note : PropertyChangedBase
    {
        /// <summary>
        /// 保存文件名
        /// </summary>
        private static string _config = "config.xml";

        public Note()
        {
            Guid = System.Guid.NewGuid().ToString();
            Time = DateTime.Now;
        }

        private bool _hasChanged;
        /// <summary>
        /// 记事是否已改变
        /// </summary>
        public bool HasChanged
        {
            get
            {
                return _hasChanged;
            }
        }
        /// <summary>
        /// GUID
        /// </summary>
        public string Guid
        {
            get; private set;
        }
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime Time
        {
            get; private set;
        }
        private string _title;
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                OnProperChange();
            }
        }
        private string _detail;
        /// <summary>
        /// 详情
        /// </summary>
        public string Detail
        {
            get
            {
                return _detail;
            }
            set
            {
                _detail = value;
                OnProperChange();
            }
        }
        protected override void OnProperChange([CallerMemberName] string propertyName = null)
        {
            _hasChanged = true;
            base.OnProperChange(propertyName);
        }

        /// <summary>
        /// 保存事项
        /// </summary>
        public async Task SaveAsync()
        {
            //存储到漫游目录，支持云同步
            var folder = ApplicationData.Current.RoamingFolder;
            var file = await folder.GetFileAsync(_config);
            var doc = await XmlDocument.LoadFromFileAsync(file);
            var root = doc.LastChild;
            var node = root.ChildNodes.FirstOrDefault(t => t.Element(nameof(Guid)).InnerText == Guid);
            //新增
            if (node == null)
            {
                node = doc.CreateElement(nameof(Note));
                node.AppendChild(doc.CreateElement(nameof(Guid), Guid));
                node.AppendChild(doc.CreateElement(nameof(Time), Time.ToString()));
                node.AppendChild(doc.CreateElement(nameof(Title), Title));
                node.AppendChild(doc.CreateElement(nameof(Detail), Detail));
                root.AppendChild(node);
            }
            //修改
            else
            {
                node.Element(nameof(Title)).InnerText = Title ?? string.Empty;
                node.Element(nameof(Detail)).InnerText = Detail ?? string.Empty;
            }
            await doc.SaveToFileAsync(file);
            _hasChanged = false;
        }
        /// <summary>
        /// 删除事项
        /// </summary>
        public async Task DeleteAsync()
        {
            var folder = ApplicationData.Current.RoamingFolder;
            var file = await folder.GetFileAsync(_config);
            var doc = await XmlDocument.LoadFromFileAsync(file);
            var root = doc.LastChild;
            var node = root.ChildNodes.FirstOrDefault(t => t.ChildNodes.Any(child => child.InnerText == Guid));
            if (node != null)
            {
                root.RemoveChild(node);
            }
            await doc.SaveToFileAsync(file);
        }
        /// <summary>
        /// 加载事项
        /// </summary>
        /// <returns>事项列表</returns>
        public async static Task<IEnumerable<Note>> LoadAsync()
        {
            var folder = ApplicationData.Current.RoamingFolder;
            var item = await folder.TryGetItemAsync(_config);
            if (item == null || !item.IsOfType(StorageItemTypes.File))
            {
                var doc = new XmlDocument();
                doc.AppendChild(doc.CreateElement("Root"));
                var file = await folder.CreateFileAsync(_config);
                await doc.SaveToFileAsync(file);
                return new Note[0];
            }
            else
            {
                var file = item as StorageFile;
                var doc = await XmlDocument.LoadFromFileAsync(file);
                var query = from element in doc.LastChild.ChildNodes
                            select new Note
                            {
                                Guid = element.Element(nameof(Guid)).InnerText,
                                Time = DateTime.Parse(element.Element(nameof(Time)).InnerText),
                                _title = element.Element(nameof(Title)).InnerText,
                                _detail = element.Element(nameof(Detail)).InnerText
                            };
                return query;
            }
        }
        /// <summary>
        /// 创建空文件
        /// </summary>
        public async static Task CreateEmptyFileAsync()
        {
            var folder = ApplicationData.Current.RoamingFolder;
            var item = await folder.TryGetItemAsync(_config);
            if (item != null && item.IsOfType(StorageItemTypes.File))
                await item.DeleteAsync();
            var doc = new XmlDocument();
            doc.AppendChild(doc.CreateElement("Root"));
            var file = await folder.CreateFileAsync(_config);
            await doc.SaveToFileAsync(file);
        }
    }
}
