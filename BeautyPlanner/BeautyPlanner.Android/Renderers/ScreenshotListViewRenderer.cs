using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Views;
using BeautyPlanner.Controls;
using BeautyPlanner.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Color = Android.Graphics.Color;
using ListView = Xamarin.Forms.ListView;
using Path = System.IO.Path;
using Uri = Android.Net.Uri;
using View = Android.Views.View;


[assembly: ExportRenderer(typeof(ScreenshotListView), typeof(ScreenshotListViewRenderer))]
namespace BeautyPlanner.Droid.Renderers
{
    public class ScreenshotListViewRenderer: ListViewRenderer
    {
        public ScreenshotListViewRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                // unsubscribe
                e.NewElement.PropertyChanged -= OnScreenshotClassIdPropertyChanged;
            }

            if (e.NewElement != null)
            {
                // subscribe
                e.NewElement.PropertyChanged += OnScreenshotClassIdPropertyChanged;
            }
        }

        private void OnScreenshotClassIdPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ClassId")
            {
                var listView = Control;
                var adapter = listView.Adapter;
                var itemsCount = adapter.Count;
                var totalHeight = 0;
                var bmps = new List<Bitmap>();

                for (var i = 0; i < itemsCount-1; i++) //minus one because we have no footer in a list
                {
                    var childView = adapter.GetView(i, null, listView);
                    childView.Measure(MeasureSpec.MakeMeasureSpec(listView.Width, MeasureSpecMode.Exactly),
                        MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified));

                    childView.Layout(0, 0, childView.MeasuredWidth, childView.MeasuredHeight);
                    totalHeight += childView.MeasuredHeight;
                    var bmp = CreateBitmapFromView(childView, childView.MeasuredHeight, childView.MeasuredWidth);
                    bmps.Add(bmp);
                }

                var bigbitmap = Bitmap.CreateBitmap(listView.MeasuredWidth, totalHeight, Bitmap.Config.Argb8888);
                var bigcanvas = new Canvas(bigbitmap);

                var paint = new Paint();
                var iHeight = 0;

                foreach (var bmp in bmps)
                {
                    bigcanvas.DrawBitmap(bmp, 0, iHeight, paint);
                    iHeight += bmp.Height;

                    bmp.Recycle();
                }

                SaveImage(bigbitmap);
            }
        }

        private void SaveImage(Bitmap bitmap) 
        {
            System.IO.Stream fos;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
            {
                ContentResolver resolver = Context.ContentResolver;
                ContentValues contentValues = new ContentValues();
                contentValues.Put(MediaStore.MediaColumns.DisplayName, $"{DateTime.Now:yyyy-MMM-dd-hh-mm-ss}.jpg");
                contentValues.Put(MediaStore.MediaColumns.MimeType, "image/jpg");
                contentValues.Put(MediaStore.MediaColumns.RelativePath, Android.OS.Environment.DirectoryPictures);
                Uri imageUri = resolver.Insert(MediaStore.Images.Media.ExternalContentUri, contentValues);
                fos = resolver.OpenOutputStream(imageUri);
                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, fos);
                fos.Close();
            }
            else 
            {
                var storagePath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures);
                var beautyPlannerPath = Path.Combine(storagePath.ToString(), "BeautyPlanner");
                if (!Directory.Exists(beautyPlannerPath))
                {
                    Directory.CreateDirectory(beautyPlannerPath);
                }

                var myPath = Path.Combine(beautyPlannerPath, $"{DateTime.Now:yyyy-MMM-dd-hh-mm-ss}.jpg");

                using (var stream = new FileStream(myPath, FileMode.Create))
                {
                    bitmap.Compress(Bitmap.CompressFormat.Jpeg, 90, stream);
                }
            }
        }

        private Bitmap CreateBitmapFromView(View view, int height, int width)
        {
            var bitmap =
                Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
            var canvas = new Canvas(bitmap);
            canvas.DrawColor(Color.White);
            view.Draw(canvas);
            return bitmap;
        }
    }
}