using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace MyNote
{
    /// <summary>
    /// MasterDetail状态
    /// </summary>
    enum MasterDetailState
    {
        /// <summary>
        /// 宽屏，同时显示Master和Detail
        /// </summary>
        WideState = 0,
        /// <summary>
        /// 宽屏，显示Master，Detail为空白
        /// </summary>
        WideBlankState,
        /// <summary>
        /// 窄屏，只显示Master
        /// </summary>
        NarrowMasterState,
        /// <summary>
        /// 窄屏，只显示Detail
        /// </summary>
        NarrowDetailState
    }
    /// <summary>
    /// MasterDetail状态触发器
    /// </summary>
    class MasterDetailTrigger : StateTriggerBase
    {
        public MasterDetailTrigger()
        {
            ApplicationView.GetForCurrentView().VisibleBoundsChanged += (s, e) => UpdateTrigger();
        }

        /// <summary>
        /// ShowDetailView的依赖属性
        /// </summary>
        public static readonly DependencyProperty ShowDetailViewProperty = DependencyProperty.Register("ShowDetailView", typeof(bool), typeof(MasterDetailTrigger), new PropertyMetadata(false, OnShowDetailViewPropertyChanged));
        /// <summary>
        /// 页面临界点宽度
        /// </summary>
        public double WindowWidth
        {
            get;
            set;
        }
        /// <summary>
        /// 是否显示返回键
        /// </summary>
        public bool ShowBackKey
        {
            get; set;
        }
        /// <summary>
        /// 期待的状态
        /// </summary>
        public MasterDetailState ViewState
        {
            get;
            set;
        }
        /// <summary>
        /// 实际是否显示DetailView
        /// </summary>
        public bool ShowDetailView
        {
            get
            {
                return (bool)GetValue(ShowDetailViewProperty);
            }
            set
            {
                SetValue(ShowDetailViewProperty, value);
            }
        }
        private static void OnShowDetailViewPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var trigger = (MasterDetailTrigger)d;
            trigger.UpdateTrigger();
        }
        /// <summary>
        /// 更新触发器状态
        /// </summary>
        private void UpdateTrigger()
        {
            var isActive = false;
            var width = Window.Current.Bounds.Width;
            var showDetail = ShowDetailView;
            switch (ViewState)
            {
                case MasterDetailState.WideState:
                    isActive = width >= WindowWidth && showDetail;
                    break;
                case MasterDetailState.WideBlankState:
                    isActive = width >= WindowWidth && !showDetail;
                    break;
                case MasterDetailState.NarrowMasterState:
                    isActive = width < WindowWidth && !showDetail;
                    break;
                case MasterDetailState.NarrowDetailState:
                    isActive = width < WindowWidth && showDetail;
                    break;
            }
            //返回键可见性
            if (isActive)
            {
                var manager = SystemNavigationManager.GetForCurrentView();
                if (ShowBackKey)
                    manager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                else
                    manager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }
            SetActive(isActive);
        }
    }
}
