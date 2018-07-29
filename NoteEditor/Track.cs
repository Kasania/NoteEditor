using System;
using System.Xml;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace NoteEditor
{
    class Track
    {
        private static readonly string _XmlAttr = "Attribute";
        private static readonly string _XmlAttrName = "Name";
        private static readonly string _XmlAttrBPM = "BPM";
        private static readonly string _XmlAttrNPB = "NPS";
        private static readonly string _XmlAttrTime = "Time";
        private static readonly string _XmlAttrLevel = "Level";

        private static readonly string _xmlNote = "Note";
        private static readonly string _XmlNoteKey = "Key";
        private static readonly string _XmlNotePosition = "Position";
        private static readonly string _XmlNoteLocation = "Location";

        private static readonly string _XmlNoteLocationUP = "UP";
        //private static readonly string _XmlNoteLocationDOWN = "DOWN";

        private static readonly string defaultTrackName = "(NoName)";


        private NoteList NoteList;

        public string TrackName
        {
            get; set;
        }
        public Time TrackLength
        {
            get;set;
        }
        public int TrackLevel
        {
            get;set;
        }
        public int TrackNPB
        {
            get; set;
        }
        public float TrackBPM
        {
            get;set;
        }

        public Music Music
        {
            get;set;
        }

        public Track()
        {
            NoteList = new NoteList();
            TrackName = defaultTrackName;
            TrackLength = new Time("0.0.0.0");
            TrackLevel = 0;
            TrackBPM = 0.0f;
        }

        private Track(ref NoteList notes, string name, Time length, int Level, int NPB, float BPM)
        {
            this.NoteList = notes;
            this.TrackName = name;
            this.TrackLength = length;
            this.TrackLevel = Level;
            this.TrackNPB = NPB;
            this.TrackBPM = BPM;
        }

        #region Read
        public static Track CreateTrack(string path)
        {

            NoteList notes = new NoteList();

            string TrackName = defaultTrackName;
            Time Time = new Time("0.0.0.0");
            int Level = 0;
            int NPB = 0;
            float BPM = 0.0f;

            ReadXml(ref notes, ref TrackName, ref Time, ref Level, ref NPB, ref BPM, path);

            return new Track(ref notes, TrackName, Time, Level, NPB, BPM);
        }

        private static void ReadXml(ref NoteList notes, ref string Name, ref Time Time, ref int Level, ref int NPB, ref float BPM, string path)
        {
            XmlReader reader;
            try
            {
                reader = XmlReader.Create(path, new XmlReaderSettings
                {
                    IgnoreComments = true,
                    IgnoreWhitespace = true
                });
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name.CompareTo(_XmlAttr) == 0)
                    {
                        ReadTrackAttribute(ref Name, ref Time, ref Level, ref NPB, ref BPM, ref reader);
                    }
                    else if (reader.Name.CompareTo(_xmlNote) == 0)
                    {
                        ReadNote(ref notes, ref reader);
                    }

                }
            }
        }

        private static void ReadTrackAttribute(ref string Name, ref Time time, ref int Level, ref int NPB, ref float BPM, ref XmlReader reader)
        {
            try
            {
                if (reader.MoveToFirstAttribute())
                {
                    do
                    {
                        string attrbName = reader.Name;
                        if (attrbName.CompareTo(_XmlAttrName) == 0)
                            Name = reader.Value;
                        else if (attrbName.CompareTo(_XmlAttrTime) == 0)
                            time = new Time(reader.Value);
                        else if (attrbName.CompareTo(_XmlAttrBPM) == 0)
                            BPM = float.Parse(reader.Value);
                        else if (attrbName.CompareTo(_XmlAttrNPB) == 0)
                            NPB = int.Parse(reader.Value);
                        else if (attrbName.CompareTo(_XmlAttrLevel) == 0)
                            Level = int.Parse(reader.Value);
                    } while (reader.MoveToNextAttribute());
                }
            }
            catch (System.Exception e)
            {
                Console.Write(e.Message);
                return;
            }
        }

        private static void ReadNote(ref NoteList notes, ref XmlReader reader)
        {
            Direction dir = Direction.UP;
            Keys key = Keys.NotAKey;
            int _Position = 0;
            
            try
            {
                if (reader.MoveToFirstAttribute())
                {
                    do
                    {
                        string attrbName = reader.Name;
                        if (attrbName.CompareTo(_XmlNoteKey) == 0)
                        {
                            Enum.TryParse<Keys>(reader.Value, out key);
                        }
                        else if (attrbName.CompareTo(_XmlNotePosition) == 0)
                        {
                            _Position = int.Parse(reader.Value);
                        }
                        else if (attrbName.CompareTo(_XmlNoteLocation) == 0)
                        {
                            if (reader.Value.Equals(_XmlNoteLocationUP)){
                                dir = Direction.UP;
                            }
                            else
                            {
                                dir = Direction.DOWN;
                            }
                        }
                            
                    } while (reader.MoveToNextAttribute());

                }
            }
            catch (System.Exception e)
            {
                Console.Write(e.Message);
                return;
            }


            Note note = new Note(key, dir)
            {
                Position = _Position,
                isActive = true
            };
            int _last = notes.GetNotes(key, dir).Count;
            if (_Position > _last)
            {
                int _resrvPos = _Position + (NoteList.DefaultNoteReserveLength - 
                    (_Position % NoteList.DefaultNoteReserveLength));
                NoteList.ReserveFrom(ref notes.GetNoteListRef(dir), _last, _resrvPos);
                
            }
            notes.GetNotesRef(note.Key,dir)[_Position] = note;


        }
        #endregion

        #region Save
        public void SaveTrack(string path)
        {
            XmlDocument document = new XmlDocument();
            XmlDeclaration declaration = document.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = document.DocumentElement;
            document.InsertBefore(declaration, root);

            XmlElement TrackRoot = document.CreateElement("Track");
            #region CreateTrackAttrubutes
            XmlElement TrackAttrubutes = document.CreateElement(_XmlAttr);
            XmlAttribute TrackName = document.CreateAttribute(_XmlAttrName);
            TrackName.Value = this.TrackName;
            XmlAttribute TrackTime = document.CreateAttribute(_XmlAttrTime);
            TrackTime.Value = this.TrackLength.ToString();
            XmlAttribute TrackLevel = document.CreateAttribute(_XmlAttrLevel);
            TrackLevel.Value = this.TrackLevel.ToString();
            XmlAttribute TrackBPM = document.CreateAttribute(_XmlAttrBPM);
            TrackBPM.Value = this.TrackBPM.ToString();
            XmlAttribute TrackNPS = document.CreateAttribute(_XmlAttrNPB);
            TrackNPS.Value = this.TrackNPB.ToString();
            TrackAttrubutes.Attributes.Append(TrackNPS);
            TrackAttrubutes.Attributes.Append(TrackBPM);
            TrackAttrubutes.Attributes.Append(TrackLevel);
            TrackAttrubutes.Attributes.Append(TrackTime);
            TrackAttrubutes.Attributes.Append(TrackName);
            #endregion

            XmlElement NoteLine = document.CreateElement("NoteLine");
            foreach (ObservableCollection<Note> notes in NoteList.GetNoteListRef(Direction.UP))
            {
                foreach(Note note in notes)
                {
                    AppendNoteXml(note, ref document, ref NoteLine);
                }
            }
            foreach (ObservableCollection<Note> notes in NoteList.GetNoteListRef(Direction.DOWN))
            {
                foreach (Note note in notes)
                {
                     AppendNoteXml(note, ref document, ref NoteLine);
                }
            }

            XmlElement MusicData = document.CreateElement("MusicData");


            TrackRoot.AppendChild(TrackAttrubutes);
            TrackRoot.AppendChild(NoteLine);
            TrackRoot.AppendChild(MusicData);
            document.AppendChild(TrackRoot);
            document.Save(path);
            
        }

        private void AppendNoteXml(Note note,ref XmlDocument document, ref XmlElement NoteLine)
        {
            if (!note.isActive) return;
            XmlElement noteElement = document.CreateElement(_xmlNote);
            XmlAttribute noteKey = document.CreateAttribute(_XmlNoteKey);
            XmlAttribute notePosition = document.CreateAttribute(_XmlNotePosition);
            XmlAttribute noteLocation = document.CreateAttribute(_XmlNoteLocation);
            noteKey.Value = note.Key.ToString();
            notePosition.Value = note.Position.ToString();
            noteLocation.Value = note.Direction.ToString();
            noteElement.Attributes.Append(notePosition);
            noteElement.Attributes.Append(noteKey);
            noteElement.Attributes.Append(noteLocation);
            NoteLine.AppendChild(noteElement);
        }
        #endregion

        #region getNotes
        public ObservableCollection<Note> GetNotes(Keys key,Direction dir)
        {
            return NoteList.GetNotes(key,dir);
        }

        public ref ObservableCollection<Note> GetNotesRef(Keys key, Direction dir)
        {
            return ref NoteList.GetNotesRef(key, dir);
        }

        public ObservableCollection<Note>[] GetNoteList(Direction dir)
        {
            return NoteList.GetNoteList(dir);
        }

        public ref ObservableCollection<Note>[] GetNoteListRef(Direction dir)
        {
            return ref NoteList.GetNoteListRef(dir);
        }
        #endregion

        public void ShowInfo()
        {
            Console.WriteLine($"TrackName : {TrackName}, Time : {TrackLength}, BPM : {TrackBPM}, Level : {TrackLevel}");
            //var Player = new System.Media.SoundPlayer();
            NoteList.ShowInfo();
        }
        public void ShowInfo(Keys key, Direction dir)
        {
            Console.WriteLine($"TrackName : {TrackName}, Time : {TrackLength}, BPM : {TrackBPM}, Level : {TrackLevel}");
            NoteList.ShowInfo(key,dir);
        }

        
    }
}
