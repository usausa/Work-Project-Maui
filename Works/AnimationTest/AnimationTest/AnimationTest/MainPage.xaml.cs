namespace AnimationTest
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using Smart.ComponentModel;
    using Smart.Forms.Input;

    using Xamarin.Forms;

    public partial class MainPage
    {
        public static MainPage DesignInstance => null;

        private readonly NotificationValue<bool> executing = new NotificationValue<bool>();

        public ICommand PushCommand { get; }
        public ICommand PopCommand { get; }
        public ICommand NextCommand { get; }
        public ICommand BackCommand { get; }

        public MainPage()
        {
            InitializeComponent();

            PushCommand = MakeAsyncCommand(PushAnimation);
            PopCommand = MakeAsyncCommand(PopAnimation);
            NextCommand = MakeAsyncCommand(NextAnimation);
            BackCommand = MakeAsyncCommand(BackAnimation);
            BindingContext = this;
        }

        private ICommand MakeAsyncCommand(Func<uint, Task> func)
        {
            return new AsyncCommand<int>(
                    async x =>
                    {
                        executing.Value = true;
                        try
                        {
                            await func((uint)x);
                        }
                        finally
                        {
                            executing.Value = false;
                        }
                    },
                    x => !executing.Value)
                .Observe(executing);
        }

        //--------------------------------------------------------------------------------
        // Animation
        //--------------------------------------------------------------------------------

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
        //
        // Scale : like fade ? Dialog? Scale and fade ? [center, 0% to 100%]
        // Flip  : card (mode change?)
        // Notify + opacity ?
        // TODO Strategyにオプションを設定できるようにする感じか？

        private async Task PushAnimation(uint length)
        {
            ClearViews();

            // Preset
            var view1 = new View1();
            AddView(view1);

            await Task.Delay(500);

            // Push (Open:New, DeActive:Old)
            var view2 = new View2();
            OpenView(view2);

            await FadeIn(view2, length);

            DeActiveView(view1);

            // Animation
            static Task<bool> FadeIn(View newView, uint length = 250U)
            {
                newView.Opacity = 0;

                var tcs = new TaskCompletionSource<bool>();
                newView.ScaleTo(0, 0U).ContinueWith(x => newView.ScaleTo(1, length));
                newView.Animate(
                    "FadeIn",
                    x => newView.Opacity = x,
                    16U,
                    length,
                    Easing.Linear,
                    (v, c) => tcs.SetResult(c));

                return tcs.Task;
            }
        }

        private async Task PopAnimation(uint length)
        {
            ClearViews();

            // Preset
            var view1 = new View1();
            AddView(view1);
            DeActiveView(view1);

            var view2 = new View2();
            AddView(view2);

            await Task.Delay(500);

            // Pop (Active:New, Close:Old)
            ActiveView(view2);

            // TODO Animation(with disable old?)

            CloseView(view1);
        }

        private async Task NextAnimation(uint length)
        {
            ClearViews();

            // Preset
            var view1 = new View1();
            AddView(view1);

            await Task.Delay(500);

            // Forward (Open:New, Close:Old)
            var view2 = new View2();
            OpenView(view2);

            await SlideNext(Container, view1, view2, length);

            CloseView(view1);

            // Animation
            static Task<bool> SlideNext(AbsoluteLayout container, View oldView, View newView, uint length = 250U)
            {
                var width = container.Width;

                var tcs = new TaskCompletionSource<bool>();
                newView.Animate(
                    "Slide",
                    x =>
                    {
                        var oldMargin = (int)(x * width);
                        var newMargin = width - oldMargin;
                        oldView.Margin = new Thickness(-oldMargin, 0, oldMargin, 0);
                        newView.Margin = new Thickness(newMargin, 0, -newMargin, 0);

                    },
                    16U,
                    length,
                    Easing.Linear,
                    (v, c) => tcs.SetResult(c));

                return tcs.Task;
            }
        }

        private async Task BackAnimation(uint length)
        {
            ClearViews();

            // Preset
            var view2 = new View2();
            AddView(view2);

            await Task.Delay(500);

            // Forward (Open:New, Close:Old)
            var view1 = new View1();
            OpenView(view1);

            await SlideBack(Container, view1, view2, length);

            CloseView(view2);

            // Animation
            static Task<bool> SlideBack(AbsoluteLayout container, View oldView, View newView, uint length = 250U)
            {
                var width = container.Width;

                var tcs = new TaskCompletionSource<bool>();
                newView.Animate(
                    "Slide",
                    x =>
                    {
                        var newMargin = (int)(x * width);
                        var oldMargin = width - newMargin;
                        oldView.Margin = new Thickness(-oldMargin, 0, oldMargin, 0);
                        newView.Margin = new Thickness(newMargin, 0, -newMargin, 0);

                    },
                    16U,
                    length,
                    Easing.Linear,
                    (v, c) => tcs.SetResult(c));

                return tcs.Task;
            }
        }

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        private void OpenView(View view) => AddView(view);

        private void CloseView(View view) => Container.Children.Remove(view);

        private void ActiveView(View view) => view.IsVisible = true;

        private void DeActiveView(View view) =>
            view.IsVisible = false;

        private void AddView(View view)
        {
            AbsoluteLayout.SetLayoutFlags(view, AbsoluteLayoutFlags.WidthProportional | AbsoluteLayoutFlags.HeightProportional);
            AbsoluteLayout.SetLayoutBounds(view, new Rectangle(0, 0, 1, 1));
            Container.Children.Add(view);
        }

        private void ClearViews()
        {
            Container.Children.Clear();
        }
    }
}
