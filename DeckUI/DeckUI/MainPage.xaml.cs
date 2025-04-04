namespace DeckUI
{
    public partial class MainPage : ContentPage
    {
        private static readonly (string Color1, string Color2)[] Colors =
        [
            new("#F44336", "#FF8A80"),
            new("#E91E63", "#FF80AB"),
            new("#9C27B0", "#EA80FC"),
            new("#673AB7", "#B388FF"),
            new("#3F51B5", "#8C9EFF"),
            new("#2196F3", "#82B1FF"),
            new("#03A9F4", "#80D8FF"),
            new("#00BCD4", "#84FFFF"),
            new("#009688", "#A7FFEB"),
            new("#4CAF50", "#B9F6CA"),
            new("#8BC34A", "#CCFF90"),
            new("#CDDC39", "#F4FF81"),
            new("#FFEB3B", "#FFFF8D"),
            new("#FFC107", "#FFE57F"),
            new("#FF9800", "#FFD180"),
            new("#FF5722", "#FF9E80")
        ];

        public MainPage()
        {
            InitializeComponent();

            _ = LazyInitAsync();
        }

        private async Task LazyInitAsync()
        {
            // https://fonts.google.com/icons
            // https://gradientbuttons.colorion.co/

            var model = new DeckModel { Rows = 4, Columns = 8 };

            // Grid
            for (var row = 0; row < model.Rows; row++)
            {
                DeckGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            }
            for (var col = 0; col < model.Columns; col++)
            {
                DeckGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            }

            // Row1 & Row2
            for (var i = 0; i < Colors.Length; i++)
            {
                model.Buttons.Add(new ButtonModel
                {
                    Row = i / model.Columns,
                    Column = i % model.Columns,
                    ButtonType = ButtonType.Text,
                    Label = $"Color{i + 1}",
                    Text = "Color",
                    BackColor1 = Color.FromArgb(Colors[i].Color1),
                    BackColor2 = Color.FromArgb(Colors[i].Color2)
                });
            }

            // Row3
            model.Buttons.Add(new ButtonModel
            {
                Row = 2,
                Column = 0,
                ButtonType = ButtonType.Image,
                Label = "Folder1",
                Image = "folder.png",
                BackColor1 = Color.FromArgb("#ffb347"),
                BackColor2 = Color.FromArgb("#ffcc33")
            });
            model.Buttons.Add(new ButtonModel
            {
                Row = 2,
                Column = 1,
                ButtonType = ButtonType.Image,
                Label = "Folder2",
                Image = "folder.png",
                BackColor1 = Color.FromArgb("#ffb347"),
                BackColor2 = Color.FromArgb("#ffcc33")
            });
            model.Buttons.Add(new ButtonModel
            {
                Row = 2,
                Column = 2,
                ButtonType = ButtonType.Image,
                Label = "Sound1",
                Image = "music_note.png",
                BackColor1 = Color.FromArgb("#fc00ff"),
                BackColor2 = Color.FromArgb("#00dbde")
            });
            model.Buttons.Add(new ButtonModel
            {
                Row = 2,
                Column = 3,
                ButtonType = ButtonType.Image,
                Label = "Sound2",
                Image = "music_note.png",
                BackColor1 = Color.FromArgb("#fc00ff"),
                BackColor2 = Color.FromArgb("#00dbde")
            });

            model.Buttons.Add(new ButtonModel
            {
                Row = 2,
                Column = 5,
                ButtonType = ButtonType.Image,
                Label = "00:30",
                Image = "timer.png",
                BackColor1 = Color.FromArgb("#1fa2ff"),
                BackColor2 = Color.FromArgb("#12d8fa")
            });
            model.Buttons.Add(new ButtonModel
            {
                Row = 2,
                Column = 6,
                ButtonType = ButtonType.Image,
                Label = "Lock",
                Image = "lock.png",
                BackColor1 = Color.FromArgb("#a770ef"),
                BackColor2 = Color.FromArgb("#cf8bf3")
            });
            model.Buttons.Add(new ButtonModel
            {
                Row = 2,
                Column = 7,
                ButtonType = ButtonType.Image,
                Label = "Settings",
                Image = "settings.png",
                BackColor1 = Color.FromArgb("#0cebeb"),
                BackColor2 = Color.FromArgb("#20e3b2")
            });

            // Row4
            model.Buttons.Add(new ButtonModel
            {
                Row = 3,
                Column = 0,
                ButtonType = ButtonType.Image,
                Label = "Volume up",
                Image = "volume_off.png",
                BackColor1 = Color.FromArgb("#f46b45"),
                BackColor2 = Color.FromArgb("#eea849")
            });
            model.Buttons.Add(new ButtonModel
            {
                Row = 3,
                Column = 1,
                ButtonType = ButtonType.Image,
                Label = "Mute",
                Image = "volume_up.png",
                BackColor1 = Color.FromArgb("#f46b45"),
                BackColor2 = Color.FromArgb("#eea849")
            });
            model.Buttons.Add(new ButtonModel
            {
                Row = 3,
                Column = 2,
                ButtonType = ButtonType.Image,
                Label = "Volume down",
                Image = "volume_down.png",
                BackColor1 = Color.FromArgb("#f46b45"),
                BackColor2 = Color.FromArgb("#eea849")
            });

            model.Buttons.Add(new ButtonModel
            {
                Row = 3,
                Column = 6,
                ButtonType = ButtonType.Text,
                Label = "CPU",
                Text = String.Join(Environment.NewLine, "CPU", "13%"),
                BackColor1 = Color.FromArgb("#616161"),
                BackColor2 = Color.FromArgb("#424242")
            });
            model.Buttons.Add(new ButtonModel
            {
                Row = 3,
                Column = 7,
                ButtonType = ButtonType.Text,
                Label = "Memory",
                Text = String.Join(Environment.NewLine, "MEM", "74%"),
                BackColor1 = Color.FromArgb("#616161"),
                BackColor2 = Color.FromArgb("#424242")
            });

            foreach (var button in model.Buttons)
            {
                if (!String.IsNullOrEmpty(button.Image))
                {
                    await using var stream = await FileSystem.OpenAppPackageFileAsync(button.Image);
                    using var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    button.ImageBytes = memoryStream.ToArray();
                }
            }

            BindingContext = model;
        }
    }
}
