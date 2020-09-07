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
        public ICommand FlipCommand { get; }

        private BoxView guard;

        public MainPage()
        {
            InitializeComponent();

            PushCommand = MakeAsyncCommand(PushAnimation);
            PopCommand = MakeAsyncCommand(PopAnimation);
            NextCommand = MakeAsyncCommand(NextAnimation);
            BackCommand = MakeAsyncCommand(BackAnimation);
            FlipCommand = MakeAsyncCommand(FlipAnimation);
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

        // TODO Strategyにオプションを設定できるようにする感じか？
        // TODO Animation基本機構
        // - Stack時、Enableの復帰、位置情報の復帰、Visibleの変更と気をつける点
        // - 新も終わった段階でEnableにする？、これが効かない！
        // - Navigationテストプロジェクト作成、ナビゲーションのキューイング？
        // - Deactivateタイミングの問題(?)
        // RotateYTo :(mode change?)
        // Flip  : card
        // Notify + opacity ? Down

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

            LockContainer();
            await FadeIn(view2, length);
            UnlockContainer();

            DeActiveView(view1);

            // Animation
            static Task<bool> FadeIn(View newView, uint length = 250U)
            {
                newView.Opacity = 0;

                var tcs = new TaskCompletionSource<bool>();
                newView.Animate(
                    "FadeIn",
                    x =>
                    {
                        newView.Opacity = x;
                        newView.Scale = x;
                    },
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
            ActiveView(view1);

            LockContainer();
            await FadeOut(view2, length);
            UnlockContainer();

            CloseView(view2);

            // Animation
            static Task<bool> FadeOut(View oldView, uint length = 250U)
            {
                var tcs = new TaskCompletionSource<bool>();
                oldView.Animate(
                    "FadeIn",
                    x =>
                    {
                        oldView.Opacity = (1 - x);
                        oldView.Scale = (1 - x);
                    },
                    16U,
                    length,
                    Easing.Linear,
                    (v, c) => tcs.SetResult(c));

                return tcs.Task;
            }
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

            LockContainer();
            await SlideNext(Container, view1, view2, length);
            UnlockContainer();

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

            LockContainer();
            await SlideBack(Container, view1, view2, length);
            UnlockContainer();

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

        private async Task FlipAnimation(uint length)
        {
            ClearViews();

            // Preset
            var view1 = new View1();
            AddView(view1);

            await Task.Delay(500);

            // Forward (Open:New, Close:Old)
            var view2 = new View2();
            OpenView(view2);

            LockContainer();
            await Flip(view1, view2, length);
            UnlockContainer();

            CloseView(view1);

            // Animation
            static async Task Flip(View oldView, View newView, uint length = 250U)
            {
                newView.IsVisible = false;

                oldView.RotationY = 360;
                await oldView.RotateYTo(270, length, Easing.Linear);

                oldView.IsVisible = false;
                newView.IsVisible = true;

                newView.RotationY = 90;
                await newView.RotateYTo(0, length, Easing.Linear);
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

        private void LockContainer()
        {
            guard = new BoxView();
            guard.Background = Brush.Transparent;

            AddView(guard);

            Container.RaiseChild(guard);
        }

        private void UnlockContainer()
        {
            Container.Children.Remove(guard);
        }
    }
}
