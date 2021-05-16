using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Smart.Forms.Interactivity;
using Xamarin.Forms;
using ZXing;
using ZXing.Net.Mobile.Forms;

namespace WorkFocusBarcode
{
    using System.Diagnostics;

    public sealed class BarcodeEntryBehavior : BehaviorBase<Entry>
    {
        // TODO camera changed ?
        public static readonly BindableProperty CameraProperty = BindableProperty.Create(
            nameof(Camera),
            typeof(ZXingScannerView),
            typeof(BarcodeEntryBehavior));

        public ZXingScannerView Camera
        {
            get => (ZXingScannerView)GetValue(CameraProperty);
            set => SetValue(CameraProperty, value);
        }

        protected override void OnAttachedTo(Entry bindable)
        {
            base.OnAttachedTo(bindable);

            bindable.Focused += BindableOnFocused;
            bindable.Unfocused += BindableOnUnfocused;
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.Focused -= BindableOnFocused;
            bindable.Unfocused -= BindableOnUnfocused;

            base.OnDetachingFrom(bindable);
        }

        private void BindableOnFocused(object sender, FocusEventArgs e)
        {
            Debug.WriteLine("******** Focused");
            var camera = Camera;
            if (camera is not null)
            {
                camera.OnScanResult += CameraOnOnScanResult;
                //camera.IsScanning = true;
            }
        }

        private void BindableOnUnfocused(object sender, FocusEventArgs e)
        {
            Debug.WriteLine("******** Unfocused");
            var camera = Camera;
            if (camera is not null)
            {
                //camera.IsScanning = false;
                camera.OnScanResult -= CameraOnOnScanResult;
            }
        }

        private void CameraOnOnScanResult(Result result)
        {
            Debug.WriteLine($"******** ScanResult {Device.IsInvokeRequired}");

            Device.BeginInvokeOnMainThread(() =>
            {
                Debug.WriteLine("******** ScanResult");
                AssociatedObject.Text = result.Text;
            });
        }
    }
}
