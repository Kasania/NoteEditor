using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


using Microsoft.Win32;

namespace NoteEditor
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 
    //todo : utilPanel
    //todo : short cut
    //todo : music binding

    public partial class MainWindow : Window
    {

        //Main->Track->Notes
        //               └>Each NoteLine

        Track CurrentTrack { get; set; }

        List<Note> UPSNotes { get; set; }

        private int UPNoteCount;
        private int DownNoteCount;
        private string FileName = "New Track";
        private string LastFilePath = "";

        public static RoutedCommand SaveShortcut = new RoutedCommand();

        public MainWindow()
        {
            InitializeComponent();
            CurrentTrack = new Track();
            SaveShortcut.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(SaveShortcut,SaveWithoutDialog));
            RegisterNoteItems();
        }

        private void RegisterNoteItems()
        {

            TrackNameBox.Text = CurrentTrack.TrackName;
            TrackLevelBox.Text = CurrentTrack.TrackLevel.ToString();
            TrackBPMBox.Text = CurrentTrack.TrackBPM.ToString();
            TrackNPSBox.Text = CurrentTrack.TrackNPB.ToString();
            TrackLengthBox.Text = CurrentTrack.TrackLength.ToString();

            UPS.ItemsSource = CurrentTrack.GetNotes(Keys.S, Direction.UP);
            UPD.ItemsSource = CurrentTrack.GetNotes(Keys.D, Direction.UP);
            UPF.ItemsSource = CurrentTrack.GetNotes(Keys.F, Direction.UP);
            UPSPACE.ItemsSource = CurrentTrack.GetNotes(Keys.SPACE, Direction.UP);
            UPJ.ItemsSource = CurrentTrack.GetNotes(Keys.J, Direction.UP);
            UPK.ItemsSource = CurrentTrack.GetNotes(Keys.K, Direction.UP);
            UPL.ItemsSource = CurrentTrack.GetNotes(Keys.L, Direction.UP);

            UpScroll.ScrollToBottom();

            DOWNS.ItemsSource = CurrentTrack.GetNotes(Keys.S, Direction.DOWN);
            DOWND.ItemsSource = CurrentTrack.GetNotes(Keys.D, Direction.DOWN);
            DOWNF.ItemsSource = CurrentTrack.GetNotes(Keys.F, Direction.DOWN);
            DOWNSPACE.ItemsSource = CurrentTrack.GetNotes(Keys.SPACE, Direction.DOWN);
            DOWNJ.ItemsSource = CurrentTrack.GetNotes(Keys.J, Direction.DOWN);
            DOWNK.ItemsSource = CurrentTrack.GetNotes(Keys.K, Direction.DOWN);
            DOWNL.ItemsSource = CurrentTrack.GetNotes(Keys.L, Direction.DOWN);

            UpdateTitle(false);

        }

        public void UpdateTitle(bool isModifiedContent)
        {
            if (isModifiedContent)
            {
                Title = string.Format($"NTED - {FileName} *");
            }
            else
            {
                Title = string.Format($"NTED - {FileName}");
            }

        }

        #region MenuItemsEvent
        private void MenuNewFile(object sender, RoutedEventArgs e)
        {
            CurrentTrack = new Track();
            RegisterNoteItems();
        }

        private void MenuOpenFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileOpen = new OpenFileDialog
            {
                FileName = FileName,
                DefaultExt = ".xml",
                Filter = "XML Files (*.xml)|*.xml"
            };
            var result = fileOpen.ShowDialog();

            if (result == true)
            {
                CurrentTrack = Track.CreateTrack(fileOpen.FileName);
                LastFilePath = fileOpen.FileName;
                string[] FullName = fileOpen.FileName.Split('\\');
                FileName = FullName[FullName.Length - 1];
                RegisterNoteItems();
                //MessageBox.Show(((CurrentTrack.BPM / 60.0) / 16).ToString() + "s/beat");
            }
        }

        private void MenuSaveFile(object sender, RoutedEventArgs e)
        {
            SaveWithDialog(sender, e);
        }

        private void MenuExitProgram(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void SaveWithDialog(object sender, RoutedEventArgs e)
        {
            SaveFileDialog fileSave = new SaveFileDialog
            {
                FileName = FileName,
                DefaultExt = ".xml",
                Filter = "XML Files (*.xml)|*.xml"
            };
            var result = fileSave.ShowDialog();
            if (result == true)
            {

                CurrentTrack.SaveTrack(fileSave.FileName);
                string[] FullName = fileSave.FileName.Split('\\');
                FileName = FullName[FullName.Length - 1];
                UpdateTitle(false);
            }
        }

        private void SaveWithoutDialog(object sender, RoutedEventArgs e)
        {
            if (LastFilePath.Equals(""))
            {
                SaveWithDialog(sender, e);
            }
            else
            {
                CurrentTrack.SaveTrack(LastFilePath);
                UpdateTitle(false);
            }
        }

        #endregion

        #region NoteLineEvents
        private void UpScrollLoaded(object sender, RoutedEventArgs e)
        {
            UpScroll.AddHandler(MouseWheelEvent, new RoutedEventHandler(UpScrollWheel),true);
        }

        private void UpScrollWheel(object sender, RoutedEventArgs e)
        {
            MouseWheelEventArgs m = (MouseWheelEventArgs)e;
            double offset = m.Delta*2/3;
            double value = UpScroll.VerticalOffset;
            if(UPNoteCount > 0)
            {
                --UPNoteCount;
                return;
            }
            UPNoteCount = 8;
            if((UpScroll.VerticalOffset <= UpScroll.ScrollableHeight * 0.1) && (offset > 0))
            {
                NoteList.Reserve(ref CurrentTrack.GetNoteListRef(Direction.UP));
                return;
            }
            UpScroll.ScrollToVerticalOffset(value - offset);
        }

        private void DownScrollLoaded(object sender, RoutedEventArgs e)
        {
            DownScroll.AddHandler(MouseWheelEvent, new RoutedEventHandler(DownScrollWheel), true);
            ListBox listbox = (ListBox)sender;
        }

        private void DownScrollWheel(object sender, RoutedEventArgs e)
        {
            MouseWheelEventArgs m = (MouseWheelEventArgs)e;
            double offset = m.Delta*2/3;
            double value = DownScroll.VerticalOffset;

            if (DownNoteCount > 0)
            {
                --DownNoteCount;
                return;
            }
            DownNoteCount = 8;
            if ((DownScroll.VerticalOffset >= DownScroll.ScrollableHeight * 0.9) && (offset < 0))
            {
                NoteList.Reserve(ref CurrentTrack.GetNoteListRef(Direction.DOWN));
                //increse notelist
                return;
            }

            DownScroll.ScrollToVerticalOffset(value - offset);
        }

        private void Note_RightMouseDown(object sender, MouseButtonEventArgs e)
        {
            var BoxItem = (sender as ListBoxItem);
            var item = BoxItem.Content as Note;

            if (!item.isActive) return;

            var notelist = CurrentTrack.GetNotesRef(item.Key, item.Direction);
            var index = CurrentTrack.GetNotesRef(item.Key, item.Direction).IndexOf(item);
            var newNote = new Note(item.Key, item.Direction);//delete Note at index
            notelist[index] = newNote;

            UpdateTitle(true);
            e.Handled = true;
        }

        private void Note_LeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var BoxItem = (sender as ListBoxItem);
            var item = BoxItem.Content as Note;

            if (item.isActive) return;

            var notelist = CurrentTrack.GetNotesRef(item.Key, item.Direction);
            var index = CurrentTrack.GetNotesRef(item.Key, item.Direction).IndexOf(item);

            var newNote = new Note(item.Key, item.Direction)
            {
                Position = index,
                isActive = true
            };
            notelist[index] = newNote;

            UpdateTitle(true);

            e.Handled = true;
        }
        #endregion

        #region TrackAttrubutes
        

        private void TrackLevelBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void TrackBPMBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, @"[0-9]+([.][0-9]+)?");//double regex
        }

        private void TrackNPBBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void TrackNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CurrentTrack.TrackName = (sender as TextBox).Text;
        }

        private void TrackLevelBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CurrentTrack.TrackLevel = int.Parse((sender as TextBox).Text);
        }

        private void TrackBPMBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CurrentTrack.TrackBPM = float.Parse((sender as TextBox).Text);
        }

        private void TrackNPBBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CurrentTrack.TrackNPB = int.Parse((sender as TextBox).Text);
        }

        #endregion

        private void MusicOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileOpen = new OpenFileDialog
            {
                FileName = "Musicfile",
                DefaultExt = ".mp3",
                Filter = "Mp3 Files (*.mp3)|*.mp3|Wave Files (*.wav)|*.wav"
            };
            var result = fileOpen.ShowDialog();

            if (result == true)
            {
                CurrentTrack.Music?.Stop();
                CurrentTrack.Music = new Music(fileOpen.FileName);
                UpdateMusicTime();
                //MusicNameTextBox.Text = CurrentTrack.Music.MusicName;
            }
            e.Handled = true;
        }
        private void MusicPlay_Click(object sender, RoutedEventArgs e)
        {

            if(CurrentTrack.Music != null)
            {
                CurrentTrack.Music.Play();
            }
            
            e.Handled = true;
        }
        private void MusicPause_Click(object sender, RoutedEventArgs e)
        {
            CurrentTrack.Music?.Pause();
            e.Handled = true;
        }
        private void MusicStop_Click(object sender, RoutedEventArgs e)
        {
            CurrentTrack.Music?.Stop();
            e.Handled = true;
        }
        
        public void UpdateMusicTime()
        {
            Timer timer = new Timer(50);
            timer.Elapsed += 
                (s,e) => MusicNameTextBox.Dispatcher.Invoke(
                    (Action)(() => MusicNameTextBox.Text = CurrentTrack.Music.GetMusicProgress())
                );
            timer.Start();
        }

        private void ListBoxItem_PreviewDragOver(object sender, DragEventArgs e)
        {
            if(e.Data.GetDataPresent(DataFormats.StringFormat)){
                Console.WriteLine((string)e.Data.GetData(DataFormats.StringFormat));
            }
        }
    }
}
