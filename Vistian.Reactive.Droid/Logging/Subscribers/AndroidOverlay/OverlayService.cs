using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ReactiveUI;

// ReSharper disable once CheckNamespace
namespace Vistian.Reactive.Droid.Logging
{
    /// <summary>
    /// Our console service.
    /// </summary>
    [Service(Name = "vistian.Reactive.Logging.Overlay", Exported = false, Enabled = true)]
    public class OverlayService : Service, View.IOnTouchListener
    {
        private ImageButton _restoreButton;
        private ImageButton _minimizeButton;
        private LinearLayout _minimalLayout;
        private FrameLayout _restoredLayout;

        private View _theView;
        private int _originalXPos;
        private int _originalYPos;
        private float _offsetX;
        private float _offsetY;

        private bool _moving;
        private IWindowManager _wm;
        private ListView _logListView;

        /// <summary>
        /// Always keep the most active item visible?
        /// </summary>
        private bool _trackToActive = true;

        /// <summary>
        /// Are we currently playing or not?
        /// </summary>
        private bool _playing = true;


        private ImageButton _trackActiveButton;
        private ImageButton _playPauseButton;
        private ImageButton _clearButton;

        private ImageButton _copyClipboardButton;

        private ImageButton _closeButton;
        private RelativeLayout _toolbarLayout;
        private RxOverlayLogAdapter _adapter;
        private IDisposable _firehoseSub;


        /// <summary>
        /// Constant intent parameter
        /// </summary>
        public const string ListSizeIntentExtra = "ListSize";


        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            return base.OnStartCommand(intent, flags, startId);
        }

        /// <summary>
        /// Create the service.
        /// </summary>
        public override void OnCreate()
        {
            base.OnCreate();

            // get the window manager, and create it with the attributes so its always around
            _wm = this.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();

            var ps = new WindowManagerLayoutParams(WindowManagerTypes.SystemAlert)
            {
                Flags = WindowManagerFlags.NotTouchModal | WindowManagerFlags.NotFocusable,
                Format = Format.Translucent,
                Gravity = GravityFlags.Left | GravityFlags.Top,
                X = 0,
                Y = 0,
                Width = ViewGroup.LayoutParams.MatchParent,
                Height = ViewGroup.LayoutParams.WrapContent
            };


            // inflate the console view
            var v = this.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
            _theView = v.Inflate((int)Resource.Layout.overlay, null);

            // find out key buttons and lists
            _restoreButton = _theView.FindViewById<ImageButton>(Resource.Id.consoleRestoreButton);
            _minimizeButton = _theView.FindViewById<ImageButton>(Resource.Id.consoleMinimizeButton);
            _minimalLayout = _theView.FindViewById<LinearLayout>(Resource.Id.consoleMinimizedLayout);
            _restoredLayout = _theView.FindViewById<FrameLayout>(Resource.Id.consoleExpandedLayout);
            _toolbarLayout = _theView.FindViewById<RelativeLayout>(Resource.Id.consoleToolbar);
            _logListView = _theView.FindViewById<ListView>(Resource.Id.consoleLogList);
            _trackActiveButton = _theView.FindViewById<ImageButton>(Resource.Id.consoleTrackLatestButton);
            _playPauseButton = _theView.FindViewById<ImageButton>(Resource.Id.consolePlayPauseButton);
            _clearButton = _theView.FindViewById<ImageButton>(Resource.Id.consoleClearButton);
            _copyClipboardButton = _theView.FindViewById<ImageButton>(Resource.Id.consoleCopyClipboardButton);
            _closeButton = _theView.FindViewById<ImageButton>(Resource.Id.consoleCloseButton);

            // setup listensers to allow for things to be moved around
            _toolbarLayout.SetOnTouchListener(this);
            _restoreButton.SetOnTouchListener(this);

            // listen to our toolbar buttons.
            _minimizeButton.Click += MinimizeButtonClick;
            _restoreButton.Click += RestoreButtonClick;
            _trackActiveButton.Click += TrackActiveButtonClick;
            _playPauseButton.Click += PlayPauseButtonClick;
            _clearButton.Click += ClearButtonClick;
            _copyClipboardButton.Click += CopyClipboardButtonClick;
            _closeButton.Click += CloseButtonClick;

            // finally add the view.
            _wm.AddView(_theView, ps);


            // host look to observable the logging firehose

            // monitor the fire hose and format to text...
            var diagnosticsSubscriber = Overlay.DiagnosticsSubscriber;

            _adapter = new RxOverlayLogAdapter(diagnosticsSubscriber.BufferSize);
            _logListView.Adapter = _adapter;

            // and create the subscription which gives us the things to be presented
            _firehoseSub = diagnosticsSubscriber.Entries.Where(_ => _playing).
                Scan(new OverlayListEntry(0, string.Empty), (e, s) => new OverlayListEntry(e.Index + 1, s)).
                ObserveOn(RxApp.MainThreadScheduler).
                Do(s =>
                {
                    _adapter.Add(s);
                }).
                Subscribe(s =>
                {
                    if (_trackToActive && _adapter.Count > 0)
                    {
                        _logListView.SmoothScrollToPosition(_adapter.Count - 1);
                    }
                });

        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        /// <summary>
        /// User selected close button,bin myself off.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButtonClick(object sender, EventArgs e)
        {
            StopSelf();
        }

        /// <summary>
        /// Remove the view, and get rid off the subscriptions.
        /// </summary>
        public override void OnDestroy()
        {
            _wm.RemoveView(_theView);
            _firehoseSub?.Dispose();
            _firehoseSub = null;
        }

        /// <summary>
        /// Copy the contents of the lift to the clipboard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyClipboardButtonClick(object sender, EventArgs e)
        {
            // now need to copy all of the stuff to the clipboard...
            var cm = this.GetSystemService(ClipboardService).JavaCast<ClipboardManager>();

            var items = _adapter.Items.Select(i => i.Text);

            var finalString = string.Join((string)"\n", items);

            var cd = ClipData.NewPlainText("log", finalString);

            cm.PrimaryClip = cd;
        }

        /// <summary>
        /// Clear all the content of the adapter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearButtonClick(object sender, EventArgs e)
        {
            _adapter.Clear();
        }

        /// <summary>
        /// Toggle the play/pause state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayPauseButtonClick(object sender, EventArgs e)
        {
            _playing = !_playing;
            _playPauseButton.SetImageDrawable(this.GetDrawable(_playing ? Resource.Drawable.ic_pause_white_24dp : Resource.Drawable.ic_play_arrow_white_24dp));
        }

        /// <summary>
        /// Toggle the 'active button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TrackActiveButtonClick(object sender, EventArgs e)
        {
            _trackToActive = !_trackToActive;
            _trackActiveButton.Alpha = _trackToActive ? 1.0f : 0.4f;
        }

        /// <summary>
        /// Restore the console back
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RestoreButtonClick(object sender, EventArgs e)
        {
            _restoredLayout.Visibility = ViewStates.Visible;
            _minimalLayout.Visibility = ViewStates.Gone;
        }

        private void MinimizeButtonClick(object sender, EventArgs e)
        {
            _restoredLayout.Visibility = ViewStates.Gone;
            _minimalLayout.Visibility = ViewStates.Visible;
        }


        /// <summary>
        /// Move the window around the screen
        /// </summary>
        /// <param name="v"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public bool OnTouch(View v, MotionEvent e)
        {
            if (e.Action == MotionEventActions.Down)
            {
                var x = e.RawX;

                var y = e.RawY;

                var location = new int[2];
                _theView.GetLocationOnScreen(location);

                _originalXPos = location[0];
                _originalYPos = location[1];

                _offsetX = _originalXPos - x;
                _offsetY = _originalYPos - y;
                _moving = false;

                return v == _toolbarLayout;
            }
            if (e.Action == MotionEventActions.Move)
            {
                var location = new int[2];

                _theView.GetLocationOnScreen(location);

                var x = e.RawX;
                var y = e.RawY;

                var lp = _theView.LayoutParameters as WindowManagerLayoutParams;

                var newX = (int)(_offsetX + x);
                var newY = (int)(_offsetY + y);

                if (Math.Abs(newX - _originalXPos) < 1 && Math.Abs(newY - _originalYPos) < 1 && !_moving)
                {
                    return false;
                }

                lp.X = newX;
                lp.Y = newY;

                _wm.UpdateViewLayout(_theView, lp);
                _moving = true;

                return v == _toolbarLayout;
            }

            if (e.Action != MotionEventActions.Up) return false;

            return _moving;
        }


    }
}