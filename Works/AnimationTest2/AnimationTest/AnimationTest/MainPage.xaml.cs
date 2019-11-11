using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using Xamarin.Forms;

namespace AnimationTest
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage
    {
        private readonly List<View> views = new List<View>();

        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button1_OnClicked(object sender, EventArgs e)
        {
            ClearViews();

            // TODO Animation基本機構
            // - 旧ビューの無効化、見た目？
            // - ビューの分離とインタラクションがわかるように
            // - 新ビューのアニメーション中イベント(Navigation自体が終わっていれば、発生しても問題ない？)
            //   新も終わった段階でEnableにする？
            // - 分離ビュー更新
            // - Taskが返ったとして、呼び出す側はawaitする？(あまり関係ないか？)
            // - Stack時、Enableの復帰、位置情報の復帰、Visibleの変更と気に
            // - Navigationテストプロジェクト作成？
            //   再入は禁止の問題！
            //   Deactivateタイミングの問題

            // アニメーションパターン網羅
            // FadeIn     : Open   : addTop   , newView opacity 0 -> 1
            // FadeOut    : Close  : addBottom, oldView opacity 1 -> 0
            // SlideLeft  : Next   : addTop   , newView margin  100->0, old...
            // SlideRight : Back   : addTop   , newView margin -100->0, old...
            // TopDown    : Notify :
            // TopUp      : Close  :
            // BottomUp   : Notify :
            // BottomDown : Close  :
            //
            // Scale : like fade ? Dialog? Scale and fade ? [center, 0% to 100%]
            // Flip  : card (mode change?)
            // Notify + opacity ?

            var oldView = new BoxView { BackgroundColor = Color.OrangeRed };
            AddView(oldView);
            var newView = new BoxView { BackgroundColor = Color.CornflowerBlue };
            AddView(newView);

            // Pre
            oldView.IsEnabled = false;

            // Animation
            await Animations.Slide(Container, oldView, newView, 2000U);

            // Post
            Container.Children.Remove(oldView);

            Debug.WriteLine("After slide call");
        }

        private void Button2_OnClicked(object sender, EventArgs e)
        {
            ClearViews();
        }

        private void Button3_OnClicked(object sender, EventArgs e)
        {
            ClearViews();
        }

        private void Button4_OnClicked(object sender, EventArgs e)
        {
            ClearViews();
        }

        // TODO Save and Restore focus after animation
        // TODO Enable ?, IsVisible(stack) ?

        private void AddView(View view)
        {
            AbsoluteLayout.SetLayoutFlags(view, AbsoluteLayoutFlags.WidthProportional | AbsoluteLayoutFlags.HeightProportional);
            AbsoluteLayout.SetLayoutBounds(view, new Rectangle(0, 0, 1, 1));
            // TODO Insert first or last ?
            Container.Children.Add(view);

            views.Add(view);
        }

        private void ClearViews()
        {
            foreach (var view in views)
            {
                Container.Children.Remove(view);
            }

            views.Clear();
        }
    }
}
