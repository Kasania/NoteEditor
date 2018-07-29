using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteEditor
{
    class Note
    {

        public Note(Keys key, Direction dir)
        {
            Key = key;
            Direction = dir;
        }

        public int Position { get; set; }
        public bool isActive { get; set; }
        public Keys Key
        {
            get;set;
        }
        public Direction Direction
        {
            get;set;
        }

        public void ShowInfo()
        {
            Console.WriteLine($"Key : {Key}, Position : {Position}");
        }

        public int GetKeyValue()
        {
            return (int) Key;
        }

        public override string ToString()
        {
            //return $"{StartTime} ~ {EndTime}";
            return $"{Position}";
        }
    }

    enum Keys
    {
        S, D, F, SPACE, J, K, L, NotAKey
    }
}
