using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNote
{
    /// <summary>
    /// 应用程序状态管理
    /// </summary>
    class AppDeferral
    {
        /// <summary>
        /// MainPage的ViewModel
        /// </summary>
        public static readonly ViewModel MainPageViewModel = new ViewModel();
        /// <summary>
        /// 最后编辑的记事GUID
        /// </summary>
        public static string LastGUID
        {
            get;
            private set;
        }
        /// <summary>
        /// 保存App状态
        /// </summary>
        public static async Task SaveAppStateAsync()
        {
            try
            {
                //记住最后编辑的记事
                if (MainPageViewModel.SelectedNote != null)
                {
                    var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
                    if (MainPageViewModel.SelectedNote != null)
                    {
                        settings.Values["LastGUID"] = MainPageViewModel.SelectedNote.Guid;
                        if (MainPageViewModel.SelectedNote.HasChanged)
                        {
                            await MainPageViewModel.SelectedNote.SaveAsync();
                            TileCreator.Creator.UpdateSecondaryTile(MainPageViewModel.SelectedNote);
                        }
                    }
                    else
                        settings.Values["LastGUID"] = string.Empty;
                }
            }
            catch
            {
            }
        }
        /// <summary>
        /// 恢复App状态
        /// </summary>
        public static void ResumeAppState()
        {
            try
            {
                var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
                LastGUID = settings.Values["LastGUID"] as string;
            }
            catch
            {
            }
        }
        /// <summary>
        /// 导航到指定Note
        /// </summary>
        /// <param name="guid"></param>
        public static void NavigateToNote(string guid)
        {
            try
            {
                //app未启动
                if (MainPageViewModel.NoteList.Count == 0)
                    LastGUID = guid;
                else
                {
                    //app已启动
                    if (!string.IsNullOrEmpty(guid))
                    {
                        var note = MainPageViewModel.NoteList.FirstOrDefault(t => t.Guid == guid);
                        if (note != null)
                            MainPageViewModel.SelectedNote = note;
                    }
                }
            }
            catch
            {
            }
        }
    }
}
