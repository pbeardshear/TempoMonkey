using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TempoMonkey
{
    public class Tutorial
    {
        static List<Tutorial> _tutorials = new List<Tutorial>();
        public static int _tutorialIndex;
        string _name, _instructions;
        public delegate bool check();
        Uri _source;
        check _checker;
        public static bool doNext = false;

        public Tutorial(string name, string instructions, Uri source, check checker)
        {
            _name = name;
            _instructions = instructions;
            _source = source;
            _checker = checker;
        }

        public Uri getSource()
        {
            return _source;
        }

        public string getInstructions()
        {
            return _instructions;
        }

        public static void setIndex(int index)
        {
            _tutorialIndex = index;
        }

        public string getName()
        {
            return _name;
        }

        public static Tutorial getCurrentTutorial()
        {
            return _tutorials[_tutorialIndex];
        }

        public static void addTutorial(Tutorial tutorial)
        {
            _tutorials.Add(tutorial);
        }

        public static bool checkTask()
        {
            if (doNext)
            {
                doNext = false;
                return true;
            }
            return _tutorials[_tutorialIndex]._checker();
        }

        public static Tutorial nextTutorial()
        {
            _tutorialIndex++;
            if (_tutorialIndex < _tutorials.Count)
            {
                return _tutorials[_tutorialIndex];
            }
            else
            {
                return null;
            }
        }
    }
}
