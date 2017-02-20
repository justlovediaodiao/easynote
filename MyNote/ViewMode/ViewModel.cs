using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNote
{
    class ViewModel : PropertyChangedBase
    {
        private ObservableCollection<Note> _noteList = new ObservableCollection<Note>();
        /// <summary>
        /// 记事列表
        /// </summary>
        public ObservableCollection<Note> NoteList
        {
            get { return _noteList; }
        }

        private Note _selectedNote;
        /// <summary>
        /// 选择的记事项
        /// </summary>
        public Note SelectedNote
        {
            get
            {
                return _selectedNote;
            }
            set
            {
                //保存上一条记事
                SaveNote(_selectedNote);
                _selectedNote = value;
                OnProperChange();
            }
        }
        /// <summary>
        /// 弹出菜单的目标Item
        /// </summary>
        public Note MenuTartgetItem
        {
            get; set;
        }

        /// <summary>
        /// 新建记事命令
        /// </summary>
        public DelegateCommand NewNoteCommand
        {
            get
            {
                return new DelegateCommand(arg => AddNewNote());
            }
        }

        private DelegateCommand _deleteCommand;
        /// <summary>
        /// 删除记事命令
        /// </summary>
        public DelegateCommand DeleteCommand
        {
            get
            {
                if(_deleteCommand == null)
                    _deleteCommand = new DelegateCommand(arg => DeleteNote());
                return _deleteCommand;
            }
        }

        private DelegateCommand _pinCommand;
        /// <summary>
        /// 固定到开始屏幕命令
        /// </summary>
        public DelegateCommand PinCommand
        {
            get
            {
                if(_pinCommand == null)
                    _pinCommand = new DelegateCommand(arg => PinToStartScreent(),arg => MenuTartgetItem != null && !TileCreator.Creator.ExistsSecondaryTile(MenuTartgetItem));
                return _pinCommand;
            }
        }

        /// <summary>
        /// 加载记事命令
        /// </summary>
        public DelegateCommand LoadNoteCommand
        {
            get
            {
                return new DelegateCommand(arg => LoadNotesAsync());
            }
        }


        public async void LoadNotesAsync()
        {
            try
            {
                var query = await Note.LoadAsync();
                foreach (var item in query)
                {
                    _noteList.Add(item);
                }
                if (!string.IsNullOrEmpty(AppDeferral.LastGUID))
                {
                    var note = _noteList.FirstOrDefault(t => t.Guid == AppDeferral.LastGUID);
                    if (note != null)
                        SelectedNote = note;
                }
            }
            catch
            {
                //加载配置文件错误时增加清空数据操作
                var dialog = new Windows.UI.Popups.MessageDialog("加载记事出错！是否恢复应用初始数据(此操作将清空所有记事项)？", "错误");
                dialog.Commands.Add(new Windows.UI.Popups.UICommand("清空记事项", async command =>
                {
                    await Note.CreateEmptyFileAsync();
                    _noteList.Clear();
                }));
                dialog.Commands.Add(new Windows.UI.Popups.UICommand("退出应用", command =>
                {
                    App.Current.Exit();
                }));
                await dialog.ShowAsync();
            }
        }

        private async void AddNewNote()
        {
            try
            {
                var note = new Note();
                await note.SaveAsync();
                _noteList.Add(note);
                SelectedNote = note;
            }
            catch (Exception e)
            {
                await new Windows.UI.Popups.MessageDialog(e.Message, "添加记事失败").ShowAsync();
            }
        }

        private async void SaveNote(Note note)
        {
            try
            {
                if (note != null && note.HasChanged)
                {
                    await note.SaveAsync();
                    TileCreator.Creator.UpdateSecondaryTile(note);
                }
            }
            catch (Exception e)
            {
                await new Windows.UI.Popups.MessageDialog(e.Message, "保存记事失败").ShowAsync();
            }
        }

        private async void DeleteNote()
        {
            try
            {
                if (MenuTartgetItem != null)
                {
                    await MenuTartgetItem.DeleteAsync();
                    _noteList.Remove(MenuTartgetItem);
                    await TileCreator.Creator.DeleteTileAsync(MenuTartgetItem);
                }
            }
            catch (Exception e)
            {
                await new Windows.UI.Popups.MessageDialog(e.Message, "删除记事失败").ShowAsync();
            }
        }

        private void PinToStartScreent()
        {
            if(MenuTartgetItem != null)
                TileCreator.Creator.PinSecondaryTile(MenuTartgetItem);
        }
    }
}
