using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteEditor
{
    public enum Direction
    {
        UP,DOWN
    }
    class NoteList
    {

        private const int KeyNum = 8;
        public const int DefaultNoteCount = 100;
        public const int DefaultNoteReserveLength = 4;

        private static ObservableCollection<Note>[] NoteListsUP;
        private static ObservableCollection<Note>[] NoteListsDOWN;

        public NoteList() : this(DefaultNoteCount)
        {
        }

        public NoteList(int reserve)
        {
            NoteListsUP = new ObservableCollection<Note>[KeyNum];
            NoteListsDOWN = new ObservableCollection<Note>[KeyNum];
            InitializeNoteList(reserve);
        }


        private static void InitializeNoteList(int count)
        {
            for (int i = 0; i < KeyNum; ++i)
            {
                NoteListsUP[i] = new ObservableCollection<Note>();
                NoteListsDOWN[i] = new ObservableCollection<Note>();
                Reserve(ref NoteListsUP[i], count);
                Reserve(ref NoteListsDOWN[i], count);
            }
        }

        #region reserveMethods
        public static void ReserveFrom(ref ObservableCollection<Note> notes, int from, int to)
        {
            Keys k = Keys.NotAKey;
            Direction d = Direction.UP;
            for(int i = 0; i<KeyNum; ++i)
            {
                if(notes == NoteListsUP[i])
                {
                    k = (Keys)i;
                    d = Direction.UP;
                }
            }
            for (int i = 0; i < KeyNum; ++i)
            {
                if (notes == NoteListsDOWN[i])
                {
                    k = (Keys)i;
                    d = Direction.DOWN;
                }
            }

            bool eq = (notes == NoteListsUP[(int)Keys.S]);
            for (int i = from; i <= to; ++i)
            {
                notes.Add(new Note(k, d));
            }
        }

        public static void ReserveTo(ref ObservableCollection<Note> notes, int to)
        {
            ReserveFrom(ref notes, notes.Count, to);
        }

        public static void Reserve(ref ObservableCollection<Note> notes, int count = DefaultNoteReserveLength)
        {
            ReserveFrom(ref notes, 0, count);
        }

        public static void Reserve(ref ObservableCollection<Note>[] notes, int count = DefaultNoteReserveLength)
        {
            for (int j = 0; j < KeyNum; ++j)
            {
                Reserve(ref notes[j], count);
            }
        }
        public static void ReserveFrom(ref ObservableCollection<Note>[] notes, int from, int to)
        {
            for (int j = 0; j < KeyNum; ++j)
            {
                ReserveFrom(ref notes[j], from, to);

            }
        }
        #endregion

        public ObservableCollection<Note> GetNotes(Keys key, Direction dir)
        {
            //if(dir == Direction.UP)
            //{
            //    ObservableCollection<Note> reversed = new ObservableCollection<Note>();
            //    reversed = NoteLists[(int)key, (int)dir];
            //    reversed.Reverse();
            //    return reversed;
            //}
            return (dir == Direction.UP) ? NoteListsUP[(int)key] : NoteListsDOWN[(int)key];
        }

        public ref ObservableCollection<Note> GetNotesRef(Keys key, Direction dir)
        {
            if(dir == Direction.UP)
            {
                return ref NoteListsUP[(int)key];

            }
            return ref NoteListsDOWN[(int)key];
        }

        public ObservableCollection<Note>[] GetNoteList(Direction dir)
        {
            if (dir == Direction.UP)
            {
                return NoteListsUP;

            }
            return NoteListsDOWN;
        }

        public ref ObservableCollection<Note>[] GetNoteListRef(Direction dir)
        {
            if (dir == Direction.UP)
            {
                return ref NoteListsUP;

            }
            return ref NoteListsDOWN;
        }

        #region showInfos
        public void ShowInfo()
        {
            for (int i = 0; i < KeyNum; ++i)
            {
                foreach (Note note in NoteListsUP[i])
                {
                    note.ShowInfo();
                }
                foreach (Note note in NoteListsDOWN[i])
                {
                    note.ShowInfo();
                }
            }
        }
        public void ShowInfo(Keys key,Direction dir)
        {
            if(dir == Direction.UP)
            {
                foreach (Note note in NoteListsUP[(int)key])
                {
                    note.ShowInfo();
                }
            }
            else
            {
                foreach (Note note in NoteListsDOWN[(int)key])
                {
                    note.ShowInfo();
                }
            }
        }
        public void ShowInfo(Keys key)
        {
            foreach (Note note in NoteListsUP[(int)key])
            {
                note.ShowInfo();
            }
            foreach (Note note in NoteListsDOWN[(int)key])
            {
                note.ShowInfo();
            }
        }
        public void ShowInfo(Direction dir)
        {
            if (dir == Direction.UP)
            {
                for (int i = 0; i < KeyNum; ++i)
                {
                    foreach (Note note in NoteListsUP[i])
                    {
                        note.ShowInfo();
                    }
                }
            }
            else
            {
                for (int i = 0; i < KeyNum; ++i)
                {
                    foreach (Note note in NoteListsDOWN[i])
                    {
                        note.ShowInfo();
                    }
                   
                }
            }
        }
        #endregion
    }
}


