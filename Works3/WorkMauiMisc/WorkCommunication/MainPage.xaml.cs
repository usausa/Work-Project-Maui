namespace WorkCommunication;

using static System.Net.Mime.MediaTypeNames;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    // TODO Wrapper

    //  Add to AndroidManifest.xml
    //
    //  <queries>
    //    <intent>
    //      <action android:name="android.intent.action.DIAL" />
    //      <data android:scheme="tel"/>
    //    </intent>
    //  </queries>
    //  <queries>
    //    <intent>
    //      <action android:name="android.intent.action.VIEW" />
    //      <data android:scheme="smsto"/>
    //    </intent>
    //  </queries>
    //  <queries>
    //    <intent>
    //      <action android:name="android.intent.action.SENDTO" />
    //      <data android:scheme="mailto" />
    //    </intent>
    //  </queries>

    private void OnDialClicked(object? sender, EventArgs e)
    {
        if (PhoneDialer.Default.IsSupported)
        {
            PhoneDialer.Default.Open("000-0000-0000");
        }
    }

    private async void OnSmsClicked(object? sender, EventArgs e)
    {
        if (Sms.Default.IsComposeSupported)
        {
            var text = "Hello world.";
            var recipients = new List<string> { "000-0000-0000" };
            var message = new SmsMessage(text, recipients);
            await Sms.Default.ComposeAsync(message);
        }
    }

    private async void OnEmailClicked(object? sender, EventArgs e)
    {
        if (Email.Default.IsComposeSupported)
        {
            var message = new EmailMessage
            {
                Subject = "Hi how are you?",
                Body = "Thanks for being here, nice to meet you!",
                BodyFormat = EmailBodyFormat.PlainText,
                To = ["dummy@example.com"]
            };
            //message.Attachments.Add(new EmailAttachment(Path.Combine(FileSystem.CacheDirectory, "dotnet_bot.svg")));

            await Email.Default.ComposeAsync(message);
        }
    }
}
