using NotificationsExtensions.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;

namespace MyNote
{
    class TileCreator
    {
        private static TileCreator _creator = new TileCreator();
        /// <summary>
        /// 获取TileCreator对象实例
        /// </summary>
        public static TileCreator Creator
        {
            get { return _creator; }
        }
        private TileCreator()
        {
        }
        /// <summary>
        /// 创建动态磁贴
        /// </summary>
        /// <param name="item">记事项</param>
        /// <returns>磁贴</returns>
        private TileContent CreateTileContent(Note item)
        {
            //创建磁贴模板
            var content = new TileBindingContentAdaptive();
            //标题
            if (!string.IsNullOrEmpty(item.Title))
            {
                content.Children.Add(new TileText
                {
                    Text = item.Title,
                    Style = TileTextStyle.Subtitle
                });
            }
            //详情
            if (!string.IsNullOrEmpty(item.Detail))
            {
                content.Children.Add(new TileText
                {
                    Text = GetDetailText(item.Detail),
                    Style = TileTextStyle.CaptionSubtle,
                    Wrap = true
                });
            }
            //生成绑定
            var binding = new TileBinding
            {
                Content = content
            };
            //生成磁贴
            var tile = new TileContent
            {
                Visual = new TileVisual
                {
                    TileWide = binding,
                    TileMedium = binding,
                }
            };
            return tile;
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string GetDetailText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            int index = -1;
            for (int i = 0; i < 3; i++)
            {
                index = text.IndexOf("\n", index + 1);
                if (index == -1 || index == text.Length - 1)
                    return text;
            }
            return text.Substring(0, index);
        }
        /// <summary>
        /// 创建辅助磁贴
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private SecondaryTile CreateSecondaryTile(Note item)
        {
            string title;
            if (!string.IsNullOrEmpty(item.Title))
                title = item.Title;
            else
                title = "简易记事";
            var tile = new SecondaryTile(item.Guid, string.Empty, title, item.Guid, TileOptions.None, 
                new Uri("ms-appx:///Assets/Square150x150Logo.scale-200.png"), new Uri("ms-appx:///Assets/Wide310x150Logo.scale-200.png"));
            return tile;
        }
        /// <summary>
        /// 更新磁贴
        /// </summary>
        /// <param name="item">记事项</param>
        public async void UpdateSecondaryTile(Note item)
        {
            try
            {
                if (SecondaryTile.Exists(item.Guid))
                {
                    var secondaryTile = CreateSecondaryTile(item);
                    await secondaryTile.UpdateAsync();
                    var tile = CreateTileContent(item);
                    TileUpdateManager.CreateTileUpdaterForSecondaryTile(item.Guid).Update(new TileNotification(tile.GetXml()));
                }
            }
            catch
            {
                //忽略磁贴异常
            }
        }
        /// <summary>
        /// 固定到开始屏幕
        /// </summary>
        /// <param name="item"></param>
        public async void PinSecondaryTile(Note item)
        {
            try
            {
                var secondaryTile = CreateSecondaryTile(item);
                if (await secondaryTile.RequestCreateAsync())
                {
                    var tile = CreateTileContent(item);
                    TileUpdateManager.CreateTileUpdaterForSecondaryTile(item.Guid).Update(new TileNotification(tile.GetXml()));
                }
            }
            catch
            {
            }
        }
        /// <summary>
        /// 删除磁贴
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task DeleteTileAsync(Note item)
        {
            try
            {
                if (SecondaryTile.Exists(item.Guid))
                {
                    var tiles = await SecondaryTile.FindAllAsync();
                    var tile = tiles.FirstOrDefault(t => t.TileId == item.Guid);
                    if (tile != null)
                    {
                        await tile.RequestDeleteAsync();
                    }
                }
            }
            catch
            {
            }
        }

        public bool ExistsSecondaryTile(Note item)
        {
            return SecondaryTile.Exists(item.Guid);
        }
    }
}
